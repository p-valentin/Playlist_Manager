namespace Playlist_Manager;

public class Playlist
{
    public string Name { get; set; }
    public List<MediaItem> Items { get; set; }

    public Playlist()
    {
        Name = string.Empty;
        Items = new List<MediaItem>();
    }

    public Playlist(string name)
    {
        Name = name;
        Items = new List<MediaItem>();
    }

    public void AddItem(MediaItem item)
    {
        Items.Add(item);
    }
}