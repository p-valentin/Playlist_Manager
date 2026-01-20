namespace Playlist_Manager;
using System;
using System.Collections.Generic;
using System.Linq;

public record PlaylistDto(string Name, List<MediaItemDto> Items);

public record MediaItemDto(
    string? Id,
    string Type,
    string Title,
    double DurationSeconds,
    string? Artist,
    string? Album,
    string? Host,
    int? EpisodeNumber);

public static class PlaylistApiMapper
{
    public static PlaylistDto ToDto(Playlist playlist)
    {
        if (playlist == null)
            throw new ArgumentNullException(nameof(playlist));

        List<MediaItemDto> items = playlist.Items.Select(ToDto).ToList();
        return new PlaylistDto(playlist.Name, items);
    }

    public static Playlist FromDto(PlaylistDto dto)
    {
        if (dto == null)
            throw new ArgumentNullException(nameof(dto));
        if (string.IsNullOrWhiteSpace(dto.Name))
            throw new ArgumentException("Playlist name is required.");

        Playlist playlist = new Playlist(dto.Name);
        if (dto.Items == null)
            return playlist;

        foreach (MediaItemDto item in dto.Items)
        {
            playlist.Items.Add(FromItemDto(item));
        }

        return playlist;
    }

    public static MediaItem FromItemDto(MediaItemDto item)
    {
        if (item == null)
            throw new ArgumentNullException(nameof(item));
        if (string.IsNullOrWhiteSpace(item.Title))
            throw new ArgumentException("Media item title is required.");
        if (item.DurationSeconds < 0)
            throw new ArgumentException("DurationSeconds must be non-negative.");

        string type = item.Type?.Trim() ?? string.Empty;
        if (string.IsNullOrWhiteSpace(type))
            throw new ArgumentException("Media item type is required.");

        MediaItem created;
        if (string.Equals(type, "Song", StringComparison.OrdinalIgnoreCase))
        {
            if (string.IsNullOrWhiteSpace(item.Artist) || string.IsNullOrWhiteSpace(item.Album))
                throw new ArgumentException("Song items require artist and album.");

            created = new Song(item.Title, TimeSpan.FromSeconds(item.DurationSeconds), item.Artist, item.Album);
        }
        else if (string.Equals(type, "PodcastEpisode", StringComparison.OrdinalIgnoreCase))
        {
            if (string.IsNullOrWhiteSpace(item.Host) || item.EpisodeNumber == null)
                throw new ArgumentException("Podcast episodes require host and episode number.");
            if (item.EpisodeNumber < 0)
                throw new ArgumentException("EpisodeNumber must be non-negative.");

            created = new PodcastEpisode(item.Title, TimeSpan.FromSeconds(item.DurationSeconds), item.Host, item.EpisodeNumber.Value);
        }
        else
        {
            throw new ArgumentException($"Unsupported media item type: {item.Type}.");
        }

        if (!string.IsNullOrWhiteSpace(item.Id))
        {
            if (!Guid.TryParse(item.Id, out Guid parsedId))
                throw new ArgumentException("Media item id is invalid.");

            created.Id = parsedId;
        }

        return created;
    }

    private static MediaItemDto ToDto(MediaItem item)
    {
        if (item is Song song)
        {
            return new MediaItemDto(
                song.Id.ToString(),
                "Song",
                song.Title,
                song.Duration.TotalSeconds,
                song.Artist,
                song.Album,
                null,
                null);
        }

        if (item is PodcastEpisode episode)
        {
            return new MediaItemDto(
                episode.Id.ToString(),
                "PodcastEpisode",
                episode.Title,
                episode.Duration.TotalSeconds,
                null,
                null,
                episode.Host,
                episode.EpisodeNumber);
        }

        throw new ArgumentException($"Unsupported media item type: {item.GetType().Name}.");
    }
}