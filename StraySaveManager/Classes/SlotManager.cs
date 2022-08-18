using ExtensionMethods;
using StraySaveManager.Helpers;

public class SlotManager
{
    private const string BackupFolderName = Constants.BackupFolderName;
    private const string SaveFileName = Constants.SaveFileName;
    private const int MaxSlots = Constants.MaxSlots;

    private readonly Dictionary<int, UserSave> UserSaves = new();

    /// <summary>
    /// Loads User Saves into the application.
    /// </summary>
    public void Load(Form form)
    {
        string saveFolder = UserSettings.SaveFolder;

        if (IsValidSaveFolder(saveFolder))
        {
            for (int i = 1; i <= MaxSlots; i++)
            {
                string slotName = "Slot_" + i;
                string slotPath = Path.Combine(saveFolder, slotName);
                if (!Directory.Exists(slotPath))
                {
                    Directory.CreateDirectory(slotPath);
                }

                string saveFile = Path.Combine(slotPath, SaveFileName);
                SaveSlot saveSlot = new SaveSlot(i, form);
                if (!File.Exists(saveFile))
                {
                    saveSlot.BackupButton.Enabled = false;
                    saveSlot.ChapterThumbnail.Image = null;
                }
                else
                {
                    saveSlot.BackupButton.Enabled = true;
                }
                UserSave userSave = new UserSave(saveFile, i, saveSlot);
                Add(i, userSave);
            }
            CheckBackups();
        }
        else
        {
            MessageBox.Show("Invalid save folder location specified.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    public void CheckBackups()
    {
        for (int i = 1; i <= MaxSlots; i++)
        {
            CheckBackup(i);
        }
    }

    /// <summary>
    /// Checks if a backup for the specified slot exists. If not, disable the Restore button.
    /// </summary>
    public void CheckBackup(int slotNumber)
    {
        string slotName = "Slot_" + slotNumber;

        string slotPath = Path.Combine(BackupFolderName, slotName);
        if (!Directory.Exists(slotPath))
        {
            Directory.CreateDirectory(slotPath);
        }

        SaveSlot saveSlot = GetSlot(slotNumber);
        string backupFile = Path.Combine(slotPath, SaveFileName);
        if (saveSlot != null)
        {
            if (!File.Exists(backupFile))
            {
                saveSlot.DisableRestore();
            }
            else
            {
                saveSlot.EnableRestore();
            }
        }
    }

    /// <summary>
    /// Adds a save file into the application or refreshes save info if it was already loaded.
    /// </summary>
    public void Add(int slotNumber, UserSave userSave)
    {
        if(slotNumber.IsWithinRange(1, MaxSlots))
        {
            if (userSave != null)
            {
                if (!UserSaves.ContainsKey(slotNumber))
                {
                    UserSaves.Add(slotNumber, userSave);
                } else
                {
                    if (File.Exists(userSave.FileLocation))
                    {
                        // If a save in the same slot is already loaded then store the file location and refresh the save info.
                        UserSaves[slotNumber].FileLocation = userSave.FileLocation;
                        UserSaves[slotNumber].UpdateSaveData();
                        UserSaves[slotNumber].Refresh();
                    }
                }
            }
        }
    }

    public void Remove(int slotNumber)
    {
        if (slotNumber.IsWithinRange(1, MaxSlots))
        {
            if (UserSaves[slotNumber] == null)
            {
                UserSaves.Remove(slotNumber);
            }
        }
    }

    public UserSave? GetSave(int slotNumber)
    {
        if (slotNumber.IsWithinRange(1, MaxSlots))
        {
            UserSave userSave = UserSaves[slotNumber];
            if (userSave != null)
            {
                return userSave;
            }
        }
        return null;
    }

    public SaveSlot? GetSlot(int slotNumber)
    {
        if (slotNumber.IsWithinRange(1, MaxSlots))
        {
            UserSave userSave = UserSaves[slotNumber];
            if (userSave != null)
            {
                return userSave.SaveSlot;
            }
        }
        return null;
    }

    public static bool IsValidSaveFolder(string directoryPath)
    {
        string validPath = @"AppData\Local\Hk_project\Saved\SaveGames";
        if (directoryPath.Contains(validPath))
        {
            if (directoryPath.EndsWith("Slots"))
            {
                return true;
            }
        }
        return false;
    }
}