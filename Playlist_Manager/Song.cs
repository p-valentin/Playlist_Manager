namespace Playlist_Manager;
public class Song : MediaItem
{
    public string Artist { get; set; }
    public string Album { get; set; }

    public Song(string title, TimeSpan duration, string artist, string album)
        : base(title, duration)
    {
        Artist = artist;
        Album = album;
    }

    public override void Play()
    {
        Console.WriteLine($"Playing song: {Title} by {Artist}");
    }
}