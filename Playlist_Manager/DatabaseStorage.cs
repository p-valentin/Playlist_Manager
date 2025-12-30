namespace Playlist_Manager;
using Microsoft.Data.Sqlite;

public class DatabaseStorage: IStorageService
{
    private readonly string _connectionString;

    public DatabaseStorage(string connectionString)
    {
        _connectionString = connectionString;
    }
    //TODO: REPLACE STRING TYPE WITH PLAYLIST
    public void Save (String playlist, List<String> songs)
    {
        SqliteConnection connection = new SqliteConnection(_connectionString);
        connection.Open();
        
        SqliteCommand command = connection.CreateCommand();
        command.CommandText = "PRAGMA foreign_keys = ON;";
        command.ExecuteNonQuery();
        
        SqliteTransaction transaction = connection.BeginTransaction();
        
        SqliteCommand insertCommand = connection.CreateCommand();
        insertCommand.CommandText = "INSERT OR IGNORE INTO Playlists (Name) VALUES ($name);";
        insertCommand.ExecuteNonQuery();
        
        SqliteCommand deleteCommand = connection.CreateCommand();
        deleteCommand.CommandText = "DELETE FROM Playlists WHERE Name = $name;";
        deleteCommand.Parameters.AddWithValue("$name", playlist);
        deleteCommand.ExecuteNonQuery();

        foreach (String song in songs)
        {
            SqliteCommand insertMediaCommand = connection.CreateCommand();
            insertMediaCommand.CommandText = " @\"\n            INSERT INTO MediaItems\n                (Id, PlaylistName, Title, DurationSeconds, ItemType)\n            VALUES\n                ($id, $playlist, $title, $duration, $type);\n            \";";
            
            //TODO: REPLACE WITH VALID DATA
            insertMediaCommand.Parameters.AddWithValue("$id", "test");
            insertMediaCommand.Parameters.AddWithValue("$playlist", playlist);
            insertMediaCommand.Parameters.AddWithValue("$title", song);
            insertMediaCommand.Parameters.AddWithValue("$duration", "5min");

/*if (item is Song)
       {
           Song song = (Song)item;

           insertMediaCommand.Parameters.AddWithValue("$type", "Song");
           insertMediaCommand.ExecuteNonQuery();

           SqliteCommand insertSongCommand = connection.CreateCommand();
           insertSongCommand.CommandText =
           @"
           INSERT INTO Songs
               (MediaItemId, Artist, Album)
           VALUES
               ($id, $artist, $album);
           ";

           insertSongCommand.Parameters.AddWithValue("$id", item.Id.ToString());
           insertSongCommand.Parameters.AddWithValue("$artist", song.Artist);
           insertSongCommand.Parameters.AddWithValue("$album", song.Album);
           insertSongCommand.ExecuteNonQuery();
       }
       else if (item is PodcastEpisode)
       {
           PodcastEpisode episode = (PodcastEpisode)item;

           insertMediaCommand.Parameters.AddWithValue("$type", "PodcastEpisode");
           insertMediaCommand.ExecuteNonQuery();

           SqliteCommand insertPodcastCommand = connection.CreateCommand();
           insertPodcastCommand.CommandText =
           @"
           INSERT INTO PodcastEpisodes
               (MediaItemId, Host, EpisodeNumber)
           VALUES
               ($id, $host, $episode);
           ";

           insertPodcastCommand.Parameters.AddWithValue("$id", item.Id.ToString());
           insertPodcastCommand.Parameters.AddWithValue("$host", episode.Host);
           insertPodcastCommand.Parameters.AddWithValue("$episode", episode.EpisodeNumber);
           insertPodcastCommand.ExecuteNonQuery();
       }
   }
   */
        }
        
        transaction.Commit();
        connection.Close();
    }

    public String Load(string name)
    {
        /*
         * SqliteConnection connection = new SqliteConnection(_connectionString);
               connection.Open();

               SqliteCommand pragmaCommand = connection.CreateCommand();
               pragmaCommand.CommandText = "PRAGMA foreign_keys = ON;";
               pragmaCommand.ExecuteNonQuery();

               SqliteCommand checkPlaylistCommand = connection.CreateCommand();
               checkPlaylistCommand.CommandText =
                   "SELECT Name FROM Playlists WHERE Name = $name;";
               checkPlaylistCommand.Parameters.AddWithValue("$name", name);

               object result = checkPlaylistCommand.ExecuteScalar();
               if (result == null)
               {
                   connection.Close();
                   return null;
               }

               Playlist playlist = new Playlist();
               playlist.Name = name;

               SqliteCommand loadItemsCommand = connection.CreateCommand();
               loadItemsCommand.CommandText =
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
               WHERE m.PlaylistName = $name;
               ";

               loadItemsCommand.Parameters.AddWithValue("$name", name);

               SqliteDataReader reader = loadItemsCommand.ExecuteReader();
               while (reader.Read())
               {
                   Guid id = Guid.Parse(reader.GetString(0));
                   string title = reader.GetString(1);
                   TimeSpan duration = TimeSpan.FromSeconds(reader.GetInt32(2));
                   string type = reader.GetString(3);

                   if (type == "Song")
                   {
                       Song song = new Song();
                       song.Id = id;
                       song.Title = title;
                       song.Duration = duration;
                       song.Artist = reader.GetString(4);
                       song.Album = reader.GetString(5);

                       playlist.Items.Add(song);
                   }
                   else if (type == "PodcastEpisode")
                   {
                       PodcastEpisode episode = new PodcastEpisode();
                       episode.Id = id;
                       episode.Title = title;
                       episode.Duration = duration;
                       episode.Host = reader.GetString(6);
                       episode.EpisodeNumber = reader.GetInt32(7);

                       playlist.Items.Add(episode);
                   }
               }

               reader.Close();
               connection.Close();

               return playlist;
           }
         */
        return name;
    }
}