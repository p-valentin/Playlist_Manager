namespace Playlist_Manager;
public class SystemConsole : IConsoleWrapper
{
    public void Print(string msg)
    {
        Console.WriteLine(msg);
    }

    public string Read()
    {
        string text = Console.ReadLine();
        if (text == null)
            return "";
        return text;
    }
}