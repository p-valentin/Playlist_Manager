namespace Playlist_Manager;
using System;
using System.IO;
using Microsoft.Data.Sqlite;
using Xunit;
using System.Linq;

public class DatabaseStorageListTests
{
    [Fact]
    public void ListPlaylists_ReturnsAllPlaylistsOrderedByName()
    {
        string dbPath = Path.Combine(Path.GetTempPath(), $"playlist_{Guid.NewGuid():N}.db");
        string connectionString = $"Data Source={dbPath};Pooling=False";

        try
        {
            DatabaseStorage storage = new DatabaseStorage(connectionString);
            
            storage.Save(new Playlist("Banana"));
            storage.Save(new Playlist("Apple"));
            storage.Save(new Playlist("Cherry"));

            var list = storage.ListPlaylists().ToList();

            Assert.Equal(3, list.Count);
            Assert.Equal("Apple", list[0]);
            Assert.Equal("Banana", list[1]);
            Assert.Equal("Cherry", list[2]);
        }
        finally
        {
            SqliteConnection.ClearAllPools();
            if (File.Exists(dbPath))
                File.Delete(dbPath);
        }
    }
}
