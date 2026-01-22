using Playlist_Manager;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
});

string? connectionString = builder.Configuration.GetConnectionString("PlaylistDb");
if (string.IsNullOrWhiteSpace(connectionString))
    connectionString = "Data Source=playlists.db";

builder.Services.AddSingleton<IStorageService>(_ => new DatabaseStorage(connectionString));

var app = builder.Build();

app.MapGet("/", () => Results.Content(AppPages.IndexHtml, "text/html"));

app.MapGet("/api/playlists", (IStorageService storage) =>
{
    return Results.Ok(storage.ListPlaylists());
});

app.MapGet("/api/playlists/{name}", (string name, IStorageService storage) =>
{
    Playlist playlist = storage.Load(name);
    if (playlist == null)
        return Results.NotFound();

    return Results.Ok(PlaylistApiMapper.ToDto(playlist));
});

app.MapPost("/api/playlists", (PlaylistDto playlistDto, IStorageService storage) =>
{
    if (playlistDto == null || string.IsNullOrWhiteSpace(playlistDto.Name))
        return Results.BadRequest("Playlist name is required.");

    Playlist playlist;
    try
    {
        playlist = PlaylistApiMapper.FromDto(playlistDto);
    }
    catch (ArgumentException ex)
    {
        return Results.BadRequest(ex.Message);
    }

    storage.Save(playlist);
    return Results.Created($"/api/playlists/{playlist.Name}", PlaylistApiMapper.ToDto(playlist));
});

app.MapDelete("/api/playlists/{name}", (string name, IStorageService storage) =>
{
    if (string.IsNullOrWhiteSpace(name))
        return Results.BadRequest("Playlist name is required.");

    bool deleted = storage.Delete(name);
    if (!deleted)
        return Results.NotFound();

    return Results.NoContent();
});

app.MapPost("/api/playlists/{name}/items", (string name, MediaItemDto itemDto, IStorageService storage) =>
{
    if (string.IsNullOrWhiteSpace(name))
        return Results.BadRequest("Playlist name is required.");

    Playlist playlist = storage.Load(name) ?? new Playlist(name);

    MediaItem item;
    try
    {
        item = PlaylistApiMapper.FromItemDto(itemDto);
    }
    catch (ArgumentException ex)
    {
        return Results.BadRequest(ex.Message);
    }

    playlist.Items.Add(item);
    storage.Save(playlist);
    return Results.Created($"/api/playlists/{playlist.Name}", PlaylistApiMapper.ToDto(playlist));
});

app.MapDelete("/api/playlists/{name}/items/{itemId}", (string name, string itemId, IStorageService storage) =>
{
    if (string.IsNullOrWhiteSpace(name))
        return Results.BadRequest("Playlist name is required.");
    if (!Guid.TryParse(itemId, out Guid parsedId))
        return Results.BadRequest("Media item id is invalid.");

    Playlist playlist = storage.Load(name);
    if (playlist == null)
        return Results.NotFound();

    int removed = playlist.Items.RemoveAll(item => item.Id == parsedId);
    if (removed == 0)
        return Results.NotFound();

    storage.Save(playlist);
    return Results.Ok(PlaylistApiMapper.ToDto(playlist));
});

app.Run();

public partial class Program
{
}