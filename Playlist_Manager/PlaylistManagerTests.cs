namespace Playlist_Manager;
using Moq;

public class PlaylistManagerTests
{
    private Mock<IStorageService> _mockStorage;
    private Mock<IConsoleWrapper> _mockConsole;

    public void Test_SearchByArtist_ReturnsCorrectSongs()
    {
        // test implementation omitted
    }

    public void Test_GetTotalDuration_CalculatesCorrectly()
    {
        // test implementation omitted
    }
}