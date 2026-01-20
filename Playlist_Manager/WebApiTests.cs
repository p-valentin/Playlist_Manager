namespace Playlist_Manager;
using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Xunit;

public class WebApiTests : IClassFixture<WebApiFactory>
{
    private readonly HttpClient _client;

    public WebApiTests(WebApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task PostAndGetPlaylist_ReturnsPersistedPlaylist()
    {
        PlaylistDto playlist = new PlaylistDto("Demo", new List<MediaItemDto>
        {
            new(null, "Song", "Song A", 180, "Artist A", "Album A", null, null),
            new(null, "PodcastEpisode", "Episode 1", 600, null, null, "Host A", 1)
        });

        HttpResponseMessage response = await _client.PostAsJsonAsync("/api/playlists", playlist);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        PlaylistDto? created = await response.Content.ReadFromJsonAsync<PlaylistDto>();
        Assert.NotNull(created);
        Assert.Equal("Demo", created.Name);
        Assert.Equal(2, created.Items.Count);

        HttpResponseMessage getResponse = await _client.GetAsync("/api/playlists/Demo");

        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

        PlaylistDto? loaded = await getResponse.Content.ReadFromJsonAsync<PlaylistDto>();
        Assert.NotNull(loaded);
        Assert.Equal("Demo", loaded.Name);
        Assert.Equal(2, loaded.Items.Count);
    }

    [Fact]
    public async Task PostPlaylistItem_AppendsToPlaylist()
    {
        string playlistName = $"Demo-{Guid.NewGuid():N}";
        MediaItemDto song = new MediaItemDto(null, "Song", "Song A", 180, "Artist A", "Album A", null, null);
        HttpResponseMessage songResponse = await _client.PostAsJsonAsync($"/api/playlists/{playlistName}/items", song);

        Assert.Equal(HttpStatusCode.Created, songResponse.StatusCode);

        MediaItemDto episode = new MediaItemDto(null, "PodcastEpisode", "Episode 1", 600, null, null, "Host A", 1);
        HttpResponseMessage episodeResponse = await _client.PostAsJsonAsync($"/api/playlists/{playlistName}/items", episode);

        Assert.Equal(HttpStatusCode.Created, episodeResponse.StatusCode);

        HttpResponseMessage getResponse = await _client.GetAsync($"/api/playlists/{playlistName}");

        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

        PlaylistDto? loaded = await getResponse.Content.ReadFromJsonAsync<PlaylistDto>();
        Assert.NotNull(loaded);
        Assert.Equal(playlistName, loaded.Name);
        Assert.Equal(2, loaded.Items.Count);
    }

    [Fact]
    public async Task DeletePlaylist_RemovesPlaylist()
    {
        string playlistName = $"DeletePlaylist-{Guid.NewGuid():N}";
        PlaylistDto playlist = new PlaylistDto(playlistName, new List<MediaItemDto>());

        HttpResponseMessage response = await _client.PostAsJsonAsync("/api/playlists", playlist);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        HttpResponseMessage deleteResponse = await _client.DeleteAsync($"/api/playlists/{playlistName}");

        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

        HttpResponseMessage getResponse = await _client.GetAsync($"/api/playlists/{playlistName}");

        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    [Fact]
    public async Task PostPlaylist_WithUnknownType_ReturnsBadRequest()
    {
        PlaylistDto playlist = new PlaylistDto("Bad", new List<MediaItemDto>
        {
            new(null, "Unknown", "Bad Item", 120, null, null, null, null)
        });

        HttpResponseMessage response = await _client.PostAsJsonAsync("/api/playlists", playlist);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task DeletePlaylistItem_RemovesItem()
    {
        string playlistName = $"Delete-{Guid.NewGuid():N}";
        MediaItemDto song = new MediaItemDto(null, "Song", "Song A", 180, "Artist A", "Album A", null, null);
        HttpResponseMessage songResponse = await _client.PostAsJsonAsync($"/api/playlists/{playlistName}/items", song);

        Assert.Equal(HttpStatusCode.Created, songResponse.StatusCode);

        PlaylistDto? created = await songResponse.Content.ReadFromJsonAsync<PlaylistDto>();
        Assert.NotNull(created);
        Assert.Single(created.Items);
        string? itemId = created.Items[0].Id;
        Assert.False(string.IsNullOrWhiteSpace(itemId));

        HttpResponseMessage deleteResponse = await _client.DeleteAsync($"/api/playlists/{playlistName}/items/{itemId}");

        Assert.Equal(HttpStatusCode.OK, deleteResponse.StatusCode);

        PlaylistDto? updated = await deleteResponse.Content.ReadFromJsonAsync<PlaylistDto>();
        Assert.NotNull(updated);
        Assert.Empty(updated.Items);
    }
}

public sealed class WebApiFactory : WebApplicationFactory<Program>
{
    private readonly string _dbPath = Path.Combine(Path.GetTempPath(), $"playlist_api_{Guid.NewGuid():N}.db");

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((_, config) =>
        {
            var settings = new Dictionary<string, string?>
            {
                ["ConnectionStrings:PlaylistDb"] = $"Data Source={_dbPath};Pooling=False"
            };
            config.AddInMemoryCollection(settings);
        });
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        if (File.Exists(_dbPath))
            File.Delete(_dbPath);
    }
}