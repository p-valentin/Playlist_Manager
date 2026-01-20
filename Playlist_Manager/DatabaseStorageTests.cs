namespace Playlist_Manager;
using System;
using System.IO;
using Microsoft.Data.Sqlite;
using Xunit;

public class DatabaseStorageTests
{
    [Fact]
    public void SaveAndLoad_PersistsPlaylistWithItems()
    {
        string dbPath = Path.Combine(Path.GetTempPath(), $"playlist_{Guid.NewGuid():N}.db");
        string connectionString = $"Data Source={dbPath};Pooling=False";

        try
        {
            DatabaseStorage storage = new DatabaseStorage(connectionString);
            Playlist playlist = new Playlist("Test");
            playlist.Items.Add(new Song("Song A", TimeSpan.FromMinutes(3), "Artist", "Album"));
            playlist.Items.Add(new PodcastEpisode("Episode 1", TimeSpan.FromMinutes(10), "Host", 1));

            storage.Save(playlist);

            Playlist loaded = storage.Load("Test");

            Assert.NotNull(loaded);
            Assert.Equal("Test", loaded.Name);
            Assert.Equal(2, loaded.Items.Count);

            Song loadedSong = Assert.IsType<Song>(loaded.Items[0]);
            Assert.Equal("Song A", loadedSong.Title);
            Assert.Equal("Artist", loadedSong.Artist);
            Assert.Equal("Album", loadedSong.Album);

            PodcastEpisode loadedEpisode = Assert.IsType<PodcastEpisode>(loaded.Items[1]);
            Assert.Equal("Episode 1", loadedEpisode.Title);
            Assert.Equal("Host", loadedEpisode.Host);
            Assert.Equal(1, loadedEpisode.EpisodeNumber);
        }
        finally
        {
            SqliteConnection.ClearAllPools();
            if (File.Exists(dbPath))
                File.Delete(dbPath);
        }
    }

    [Fact]
    public void Delete_RemovesPlaylist()
    {
        string dbPath = Path.Combine(Path.GetTempPath(), $"playlist_{Guid.NewGuid():N}.db");
        string connectionString = $"Data Source={dbPath};Pooling=False";

        try
        {
            DatabaseStorage storage = new DatabaseStorage(connectionString);
            Playlist playlist = new Playlist("ToDelete");
            playlist.Items.Add(new Song("Song A", TimeSpan.FromMinutes(3), "Artist", "Album"));

            storage.Save(playlist);

            bool deleted = storage.Delete("ToDelete");

            Assert.True(deleted);
            Assert.Null(storage.Load("ToDelete"));
        }
        finally
        {
            SqliteConnection.ClearAllPools();
            if (File.Exists(dbPath))
                File.Delete(dbPath);
        }
    }
}