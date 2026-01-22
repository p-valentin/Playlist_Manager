namespace Playlist_Manager;

public interface IStorageService
{
    void Save(Playlist playlist);
    Playlist Load(string name);
    IEnumerable<string> ListPlaylists();
    bool Delete(string name);
}