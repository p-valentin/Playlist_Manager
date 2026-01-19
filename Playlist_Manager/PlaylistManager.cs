namespace Playlist_Manager;
using System;
using System.Collections.Generic;

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
    
    public void LoadData(string playlistName)
    {
        try
        {
            CurrentPlaylist = _storage.Load(playlistName);
            _view.Print("Playlist-ul a fost incarcat cu succes.");
        }
        catch (Exception ex)
        {
            _view.Print("Eroare la incarcare: " + ex.Message);
            
            //Daca nu exista, cream unul gol
            CurrentPlaylist = new Playlist();
            CurrentPlaylist.Name = playlistName;
        }
    }

    public List<Song> SearchByArtist(string artist)
    {
        if (CurrentPlaylist == null)
            return new List<Song>();

        List<Song> result = CurrentPlaylist.Items
            .OfType<Song>()
            .Where(song => song.Artist.ToLower() == artist.ToLower())
            .ToList();

        return result;
    }

    public double GetTotalDuration()
    {
        if (CurrentPlaylist == null)
            return 0;

        double totalSeconds = 0;
        foreach (var item in CurrentPlaylist.Items)
        {
            totalSeconds += item.Duration.TotalSeconds;
        }

        return totalSeconds;
    }
}