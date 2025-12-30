namespace Playlist_Manager;
using Xunit;
using Moq;

public class PlaylistManagerTests
{
     private Mock<IStorageService> _storageMock;
    private Mock<IConsoleWrapper> _consoleMock;
    private PlaylistManager _manager;

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
    public void LoadData_ReturnsPlaylist_WhenPlaylistExists()
    {
        // Arrange
        Playlist playlist = new Playlist();
        playlist.Name = "Test";

        _storageMock
            .Setup(s => s.Load("Test"))
            .Returns(playlist);

        // Act
        Playlist result = _manager.LoadData("Test");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Test", result.Name);

        _storageMock.Verify(s => s.Load("Test"), Times.Once);
    }

    [Fact]
    public void LoadData_PrintsMessage_WhenPlaylistDoesNotExist()
    {
        // Arrange
        _storageMock
            .Setup(s => s.Load("Missing"))
            .Returns((Playlist)null);

        // Act
        Playlist result = _manager.LoadData("Missing");

        // Assert
        Assert.Null(result);

        _consoleMock.Verify(
            c => c.Print(It.IsAny<string>()),
            Times.Once
        );
    }

    [Fact]
    public void GetTotalDuration_ReturnsCorrectSum()
    {
        // Arrange
        Playlist playlist = new Playlist();
        playlist.Name = "Test";

        Song song = new Song
        {
            Id = Guid.NewGuid(),
            Title = "Song",
            Duration = TimeSpan.FromMinutes(3),
            Artist = "A",
            Album = "B"
        };

        PodcastEpisode episode = new PodcastEpisode
        {
            Id = Guid.NewGuid(),
            Title = "Episode",
            Duration = TimeSpan.FromMinutes(10),
            Host = "H",
            EpisodeNumber = 1
        };

        playlist.Items.Add(song);
        playlist.Items.Add(episode);

        // Act
        int totalSeconds = _manager.GetTotalDuration(playlist);

        // Assert
        Assert.Equal(780, totalSeconds);
}