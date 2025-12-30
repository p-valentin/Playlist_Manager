namespace Playlist_Manager;

public interface IStorageService
{
    //TODO: REPLACE STRING TYPE WITH PLAYLIST
    public void Save(String playlist, List<String> songs);
    public String Load(string name);
}