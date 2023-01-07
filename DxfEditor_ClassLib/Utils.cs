namespace DxfEditor_ClassLib;

internal static class Utils
{
    internal static string TrimPath(string path)
    {
        string currentDirectory = Directory.GetCurrentDirectory();

        if (path.IndexOf(currentDirectory) == 0 && path.Length > currentDirectory.Length)
            path = path.Substring(currentDirectory.Length);

        return path;
    }
}