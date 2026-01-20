namespace Playlist_Manager;
using System;
using Microsoft.Data.Sqlite;
using SQLitePCL;

public class DatabaseStorage : IStorageService
{
    private readonly string _connectionString;
    private static bool _isInitialized;

    public DatabaseStorage(string connectionString)
    {
        EnsureSqlite();
        _connectionString = connectionString;
    }

    public void Save(Playlist playlist)
    {
        if (playlist == null)
            throw new ArgumentNullException(nameof(playlist));

        using SqliteConnection connection = new SqliteConnection(_connectionString);
        connection.Open();
        EnableForeignKeys(connection);
        EnsureSchema(connection);

        using SqliteTransaction transaction = connection.BeginTransaction();

        using (SqliteCommand insertPlaylist = connection.CreateCommand())
        {
            insertPlaylist.CommandText = "INSERT OR IGNORE INTO Playlists (Name) VALUES ($name);";
            insertPlaylist.Parameters.AddWithValue("$name", playlist.Name);
            insertPlaylist.Transaction = transaction;
            insertPlaylist.ExecuteNonQuery();
        }

        using (SqliteCommand deleteItems = connection.CreateCommand())
        {
            deleteItems.CommandText = "DELETE FROM MediaItems WHERE PlaylistName = $name;";
            deleteItems.Parameters.AddWithValue("$name", playlist.Name);
            deleteItems.Transaction = transaction;
            deleteItems.ExecuteNonQuery();
        }

        foreach (MediaItem item in playlist.Items)
        {
            using SqliteCommand insertMedia = connection.CreateCommand();
            insertMedia.CommandText =
                @"
                INSERT INTO MediaItems
                    (Id, PlaylistName, Title, DurationSeconds, ItemType)
                VALUES
                    ($id, $playlist, $title, $duration, $type);
                ";
            insertMedia.Parameters.AddWithValue("$id", item.Id.ToString());
            insertMedia.Parameters.AddWithValue("$playlist", playlist.Name);
            insertMedia.Parameters.AddWithValue("$title", item.Title);
            insertMedia.Parameters.AddWithValue("$duration", (long)item.Duration.TotalSeconds);

            if (item is Song song)
            {
                insertMedia.Parameters.AddWithValue("$type", "Song");
                insertMedia.Transaction = transaction;
                insertMedia.ExecuteNonQuery();

                using SqliteCommand insertSong = connection.CreateCommand();
                insertSong.CommandText =
                    @"
                    INSERT INTO Songs
                        (MediaItemId, Artist, Album)
                    VALUES
                        ($id, $artist, $album);
                    ";
                insertSong.Parameters.AddWithValue("$id", item.Id.ToString());
                insertSong.Parameters.AddWithValue("$artist", song.Artist);
                insertSong.Parameters.AddWithValue("$album", song.Album);
                insertSong.Transaction = transaction;
                insertSong.ExecuteNonQuery();
            }
            else if (item is PodcastEpisode episode)
            {
                insertMedia.Parameters.AddWithValue("$type", "PodcastEpisode");
                insertMedia.Transaction = transaction;
                insertMedia.ExecuteNonQuery();

                using SqliteCommand insertEpisode = connection.CreateCommand();
                insertEpisode.CommandText =
                    @"
                    INSERT INTO PodcastEpisodes
                        (MediaItemId, Host, EpisodeNumber)
                    VALUES
                        ($id, $host, $episode);
                    ";
                insertEpisode.Parameters.AddWithValue("$id", item.Id.ToString());
                insertEpisode.Parameters.AddWithValue("$host", episode.Host);
                insertEpisode.Parameters.AddWithValue("$episode", episode.EpisodeNumber);
                insertEpisode.Transaction = transaction;
                insertEpisode.ExecuteNonQuery();
            }
        }

        transaction.Commit();
    }

    public Playlist Load(string name)
    {
        using SqliteConnection connection = new SqliteConnection(_connectionString);
        connection.Open();
        EnableForeignKeys(connection);
        EnsureSchema(connection);

        using (SqliteCommand checkPlaylist = connection.CreateCommand())
        {
            checkPlaylist.CommandText = "SELECT 1 FROM Playlists WHERE Name = $name LIMIT 1;";
            checkPlaylist.Parameters.AddWithValue("$name", name);
            object? exists = checkPlaylist.ExecuteScalar();
            if (exists == null)
                return null;
        }

        Playlist playlist = new Playlist(name);

        using SqliteCommand loadItems = connection.CreateCommand();
        loadItems.CommandText =
            @"
            SELECT
                m.Id,
                m.Title,
                m.DurationSeconds,
                m.ItemType,
                s.Artist,
                s.Album,
                p.Host,
                p.EpisodeNumber
            FROM MediaItems m
            LEFT JOIN Songs s ON s.MediaItemId = m.Id
            LEFT JOIN PodcastEpisodes p ON p.MediaItemId = m.Id
            WHERE m.PlaylistName = $name
            ORDER BY m.RowId;
            ";
        loadItems.Parameters.AddWithValue("$name", name);

        using SqliteDataReader reader = loadItems.ExecuteReader();
        while (reader.Read())
        {
            Guid id = Guid.Parse(reader.GetString(0));
            string title = reader.GetString(1);
            long durationSeconds = reader.GetInt64(2);
            TimeSpan duration = TimeSpan.FromSeconds(durationSeconds);
            string type = reader.GetString(3);

            if (type == "Song")
            {
                string artist = reader.IsDBNull(4) ? string.Empty : reader.GetString(4);
                string album = reader.IsDBNull(5) ? string.Empty : reader.GetString(5);
                Song song = new Song(title, duration, artist, album) { Id = id };
                playlist.Items.Add(song);
            }
            else if (type == "PodcastEpisode")
            {
                string host = reader.IsDBNull(6) ? string.Empty : reader.GetString(6);
                int episodeNumber = reader.IsDBNull(7) ? 0 : reader.GetInt32(7);
                PodcastEpisode episode = new PodcastEpisode(title, duration, host, episodeNumber) { Id = id };
                playlist.Items.Add(episode);
            }
        }

        return playlist;
    }

    public bool Delete(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Playlist name is required.", nameof(name));

        using SqliteConnection connection = new SqliteConnection(_connectionString);
        connection.Open();
        EnableForeignKeys(connection);
        EnsureSchema(connection);

        using SqliteCommand deletePlaylist = connection.CreateCommand();
        deletePlaylist.CommandText = "DELETE FROM Playlists WHERE Name = $name;";
        deletePlaylist.Parameters.AddWithValue("$name", name);

        int affected = deletePlaylist.ExecuteNonQuery();
        return affected > 0;
    }

    private static void EnableForeignKeys(SqliteConnection connection)
    {
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText = "PRAGMA foreign_keys = ON;";
        command.ExecuteNonQuery();
    }

    private static void EnsureSchema(SqliteConnection connection)
    {
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText =
            @"
            CREATE TABLE IF NOT EXISTS Playlists (
                Name TEXT PRIMARY KEY
            );

            CREATE TABLE IF NOT EXISTS MediaItems (
                Id TEXT PRIMARY KEY,
                PlaylistName TEXT NOT NULL,
                Title TEXT NOT NULL,
                DurationSeconds INTEGER NOT NULL,
                ItemType TEXT NOT NULL,
                FOREIGN KEY (PlaylistName) REFERENCES Playlists(Name) ON DELETE CASCADE
            );

            CREATE TABLE IF NOT EXISTS Songs (
                MediaItemId TEXT PRIMARY KEY,
                Artist TEXT NOT NULL,
                Album TEXT NOT NULL,
                FOREIGN KEY (MediaItemId) REFERENCES MediaItems(Id) ON DELETE CASCADE
            );

            CREATE TABLE IF NOT EXISTS PodcastEpisodes (
                MediaItemId TEXT PRIMARY KEY,
                Host TEXT NOT NULL,
                EpisodeNumber INTEGER NOT NULL,
                FOREIGN KEY (MediaItemId) REFERENCES MediaItems(Id) ON DELETE CASCADE
            );
            ";
        command.ExecuteNonQuery();
    }

    private static void EnsureSqlite()
    {
        if (_isInitialized)
            return;

        Batteries.Init();
        _isInitialized = true;
    }
}