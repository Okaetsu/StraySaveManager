using System.Text.RegularExpressions;
using ExtensionMethods;

public class PracticeManager
{
    private readonly Form Form;
    private const string PracticeFolderName = Constants.PracticeFolderName;
    private const string SaveFileName = Constants.SaveFileName;

    public readonly List<PracticeSave> SaveList = new();
    
    public PracticeManager(Form form)
    {
        this.Form = form;
    }

    /// <summary>
    /// Loads Practice Saves into the application. Entries that already exist will be skipped.
    /// </summary>
    public void Load()
    {
        if (!Directory.Exists(PracticeFolderName))
        {
            Directory.CreateDirectory(PracticeFolderName);
        }

        Clean();

        string[] practiceFolders = Directory.GetDirectories(PracticeFolderName);
        foreach (var practiceFolder in practiceFolders)
        {
            string practiceFile = Path.Combine(practiceFolder, SaveFileName);
            if (File.Exists(practiceFile))
            {
                string folderName = Path.GetFileName(practiceFolder);
                string description = Regex.Match(folderName, @"\[([^)]*)\]").Groups[1].Value;
                try
                {
                    int chapterIndex = int.Parse(folderName.Substring(8, 2).Trim());

                    if (chapterIndex.IsWithinRange(1, Chapters.Length()))
                    {
                        PracticeSave? existingSave = Get(chapterIndex, description);
                        if (existingSave == null)
                        {
                            PracticeSave practiceSave = new PracticeSave(description, practiceFile);
                            SaveList.Add(practiceSave);
                        }
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
    }

    /// <summary>
    /// Cleans up entries that have a missing save file.
    /// </summary>
    private void Clean()
    {
        // Clone the List to avoid errors when deleting during enumeration.
        foreach (var practiceSave in SaveList.ToList())
        {
            if (!File.Exists(practiceSave.FileLocation))
            {
                Remove(practiceSave);
            }
        }
    }

    /// <summary>
    /// Adds a Practice Save entry and removes any duplicates.
    /// </summary>
    public void Add(PracticeSave practiceSave)
    {
        PracticeSave? existingSave = Get(practiceSave.ChapterIndex, practiceSave.Description);

        if (existingSave != null)
        {
            Remove(existingSave);
        }
        SaveList.Add(practiceSave);
    }

    /// <summary>
    /// Removes a Practice Save entry along with its button.
    /// </summary>
    public void Remove(PracticeSave practiceSave)
    {
        Form.Controls.Remove(practiceSave.Button);
        practiceSave.Button.Dispose();
        SaveList.Remove(practiceSave);
    }

    /// <summary>
    /// Searches for a Practice Save entry by its Chapter Index and Description.
    /// </summary>
    public PracticeSave? Get(int chapterIndex, string description)
    {
        PracticeSave? practiceSave = SaveList.Find(save => save.ChapterIndex == chapterIndex && save.Description == description);
        return practiceSave;
    }
}