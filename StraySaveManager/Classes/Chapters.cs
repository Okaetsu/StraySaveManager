using StraySaveManager.Properties;

public class Chapters
{
    private static readonly Dictionary<string, Chapter> ChapterList = new Dictionary<string, Chapter>(12)
    {
        ["InsideTheWall"] = new Chapter() { Index = 1, Image = Resources.InsideTheWall, Name = "Inside The Wall" },
        ["DeadCity"] = new Chapter() { Index = 2, Image = Resources.DeadCity, Name = "Dead City" },
        ["B12Flat"] = new Chapter() { Index = 3, Image = Resources.B12Flat, Name = "The Flat" },
        ["SlumsPart1"] = new Chapter() { Index = 4, Image = Resources.TheSlums, Name = "The Slums" },
        ["Rooftops"] = new Chapter() { Index = 5, Image = Resources.Rooftops, Name = "Rooftops" },
        ["SlumsPart2"] = new Chapter() { Index = 6, Image = Resources.TheSlums2, Name = "The Slums - Part 2" },
        ["DeadEnd"] = new Chapter() { Index = 7, Image = Resources.DeadEnd, Name = "Dead End" },
        ["Sewers"] = new Chapter() { Index = 8, Image = Resources.Sewers, Name = "The Sewers" },
        ["AntVillage"] = new Chapter() { Index = 9, Image = Resources.Antvillage, Name = "Antvillage" },
        ["Midtown"] = new Chapter() { Index = 10, Image = Resources.Midtown, Name = "Midtown" },
        ["Jail"] = new Chapter() { Index = 11, Image = Resources.Jail, Name = "Jail" },
        ["ControlRoom"] = new Chapter() { Index = 12, Image = Resources.ControlRoom, Name = "Control Room" }
    };

    public static Chapter Get(string chapterId)
    {
        return ChapterList[chapterId];
    }

    public static Bitmap GetChapterImage(string chapterId)
    {
        return Get(chapterId).Image;
    }

    public static int GetChapterIndex(string chapterId)
    {
        return Get(chapterId).Index;
    }
    public static string GetChapterName(string chapterId)
    {
        return Get(chapterId).Name;
    }

    public static int Length()
    {
        return ChapterList.Count;
    }
}