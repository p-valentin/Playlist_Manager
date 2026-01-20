namespace Playlist_Manager;
using System;
using System.Collections.Generic;
using System.Linq;

public class PlaylistManager
{
    private readonly IStorageService _storage;
    private readonly IConsoleWrapper _view;

    //Playlist-ul curent din memorie
    public Playlist CurrentPlaylist { get; private set; }

    public PlaylistManager(IStorageService storage, IConsoleWrapper view)
    {
        _storage = storage;
        _view = view;
    }
    
    public void LoadData()
    {
        string playlistName = _view.Read();
        try
        {
            CurrentPlaylist = _storage.Load(playlistName);
            if (CurrentPlaylist == null)
            {
                CurrentPlaylist = new Playlist(playlistName);
            }
        }
        catch (Exception ex)
        {
            _view.Print("Eroare la incarcare: " + ex.Message);
            CurrentPlaylist = new Playlist(playlistName);
        }
    }

    public List<Song> SearchByArtist(string artist)
    {
        if (CurrentPlaylist == null)
            return new List<Song>();

        List<Song> result = CurrentPlaylist.Items
            .OfType<Song>()
            .Where(song => string.Equals(song.Artist, artist, StringComparison.OrdinalIgnoreCase))
            .ToList();

        return result;
    }

    public double GetTotalDuration()
    {
        if (CurrentPlaylist == null)
            return 0;

        return CurrentPlaylist.Items.Sum(item => item.Duration.TotalSeconds);
    }
}