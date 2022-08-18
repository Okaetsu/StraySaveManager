using MaterialSkin.Controls;

public class PracticeSave
{
    public string ZoneName;
    public string ChapterId;
    public string Description;
    public string FileLocation;
    public int ChapterIndex;
    public MaterialButton Button;

    public PracticeSave(string description, string fileLocation)
    {
        this.Description = description;
        this.FileLocation = fileLocation;

        if (File.Exists(fileLocation))
        {
            this.ZoneName = SaveParser.GetZoneName(fileLocation);
            this.ChapterId = SaveParser.GetChapterName(fileLocation);
            this.ChapterIndex = Chapters.GetChapterIndex(ChapterId);
        }
    }
}