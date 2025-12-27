namespace Playlist_Manager;

public class DatabaseStorage: IStorageService
{
    //TODO: REPLACE STRING TYPE WITH PLAYLIST
    public void Save(String playlist)
    {
        Console.Write(playlist);
    }

    public String Load(string name)
    {
        Console.WriteLine(name);
        return name;
    }
}