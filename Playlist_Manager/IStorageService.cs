namespace Playlist_Manager;

public interface IStorageService
{
    void Save(Playlist playlist);
    Playlist Load(string name);
    bool Delete(string name);
}