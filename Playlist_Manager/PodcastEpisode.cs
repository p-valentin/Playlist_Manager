namespace Playlist_Manager;
public class PodcastEpisode : MediaItem
{
    public string Host { get; set; }
    public int EpisodeNumber { get; set; }

    public PodcastEpisode(string title, TimeSpan duration, string host, int episodeNumber)
        : base(title, duration)
    {
        Host = host;
        EpisodeNumber = episodeNumber;
    }

    public override void Play()
    {
        Console.WriteLine($"Playing podcast episode {EpisodeNumber}: {Title}, hosted by {Host}");
    }
}