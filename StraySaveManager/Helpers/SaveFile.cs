public class SaveFile
{
    public static bool IsReadOnly(string filePath)
    {
        FileInfo fileInfo = new FileInfo(filePath);
        if (fileInfo.IsReadOnly)
        {
            return true;
        }
        return false;
    }

    public static void SetReadOnly(string fileName, bool value)
    {
        if (File.Exists(fileName))
        {
            FileInfo fileInfo = new FileInfo(fileName);
            fileInfo.IsReadOnly = value;
        }
    }

    public static void ToggleReadOnly(string fileName)
    {
        FileInfo fileInfo = new FileInfo(fileName);
        if (!fileInfo.IsReadOnly)
        {
            fileInfo.IsReadOnly = true;
        }
        else
        {
            fileInfo.IsReadOnly = false;
        }
    }
}