using System.Text;

/// <summary>
/// Provides methods for parsing data from Stray Save Files.
/// </summary>
public static class SaveParser
{
    public class Offset
    {
        public const int ZoneName = 1309; // Positive offset, starts from the beginning of the file.
        public const int ChapterName = -71; // Negative offset, starts from the end of the file.
    }

    private static readonly int[] ignoredBytes = new int[] { 0, 5, 8, 7, 9 };

    private static byte[] Read(string sourceFile, int offset, int count, SeekOrigin seekOrigin)
    {
        using FileStream fs = new FileStream(sourceFile, FileMode.Open, FileAccess.Read);
        try
        {
            fs.Seek(offset, seekOrigin);

            byte[] b = new byte[count];
            fs.Read(b, 0, (int)(count));

            return b;
        }
        catch (Exception)
        {
            throw;
        }
    }

    public static string GetZoneName(string sourceFile)
    {
        byte[] bytes = Read(sourceFile, Offset.ZoneName, 32, SeekOrigin.Begin);
        string zoneName = Encoding.UTF8.GetString(bytes);
        return zoneName;
    }

    public static string GetChapterName(string sourceFile)
    {
        byte[] bytes = Read(sourceFile, Offset.ChapterName, 13, SeekOrigin.End)
        .Where(c => !ignoredBytes.Contains(c)).ToArray();
        string chapterName = Encoding.UTF8.GetString(bytes);
        return chapterName.Trim();
    }
}