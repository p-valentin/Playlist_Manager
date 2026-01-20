namespace Playlist_Manager;
using Xunit;
using Moq;

public class PlaylistManagerTests
{
    private readonly Mock<IStorageService> _storageMock;
    private readonly Mock<IConsoleWrapper> _consoleMock;
    private readonly PlaylistManager _manager;

    public PlaylistManagerTests()
    {
        _storageMock = new Mock<IStorageService>();
        _consoleMock = new Mock<IConsoleWrapper>();

        _manager = new PlaylistManager(
            _storageMock.Object,
            _consoleMock.Object
        );
    }

    [Fact]
    public void Test_SearchByArtist_ReturnsCorrectSongs()
    {
        Playlist playlist = new Playlist("Test");
        playlist.Items.Add(new Song("Song A", TimeSpan.FromMinutes(3), "Artist1", "Album1"));
        playlist.Items.Add(new Song("Song B", TimeSpan.FromMinutes(4), "artist1", "Album2"));
        playlist.Items.Add(new Song("Song C", TimeSpan.FromMinutes(5), "Artist2", "Album3"));
        playlist.Items.Add(new PodcastEpisode("Episode", TimeSpan.FromMinutes(10), "Host", 1));

        _consoleMock.Setup(c => c.Read()).Returns("Test");
        _storageMock.Setup(s => s.Load("Test")).Returns(playlist);

        _manager.LoadData();
        List<Song> result = _manager.SearchByArtist("Artist1");

        Assert.Equal(2, result.Count);
        Assert.All(result, song => Assert.Equal("Artist1", song.Artist, ignoreCase: true));
    }

    [Fact]
    public void Test_GetTotalDuration_CalculatesCorrectly()
    {
        Playlist playlist = new Playlist("Test");
        playlist.Items.Add(new Song("Song", TimeSpan.FromMinutes(3), "A", "B"));
        playlist.Items.Add(new PodcastEpisode("Episode", TimeSpan.FromMinutes(10), "H", 1));

        _consoleMock.Setup(c => c.Read()).Returns("Test");
        _storageMock.Setup(s => s.Load("Test")).Returns(playlist);

        _manager.LoadData();
        double totalSeconds = _manager.GetTotalDuration();

        Assert.Equal(780, totalSeconds);
    }
}