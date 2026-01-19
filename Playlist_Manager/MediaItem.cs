namespace Playlist_Manager;
public abstract class MediaItem
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public TimeSpan Duration { get; set; }

    protected MediaItem(string title, TimeSpan duration)
    {
        Id = Guid.NewGuid();
        Title = title;
        Duration = duration;
    }

    public abstract void Play();
}
