using StraySaveManager.Helpers;
public class UserSave
{
    private const string BackupFolderName = Constants.BackupFolderName;
    private const string SaveFileName = Constants.SaveFileName;

    public string? ZoneName;
    public string? ChapterId;
    public string FileLocation;
    public int ChapterIndex;
    public int SlotNumber;
    public SaveSlot SaveSlot;

    public UserSave(string fileLocation, int slotNumber, SaveSlot saveSlot)
    {
        this.FileLocation = fileLocation;
        this.SlotNumber = slotNumber;
        this.SaveSlot = saveSlot;

        if (File.Exists(fileLocation))
        {
            UpdateSaveData();
            Refresh();
        }
    }

    public void UpdateSaveData()
    {
        this.ZoneName = SaveParser.GetZoneName(this.FileLocation);
        this.ChapterId = SaveParser.GetChapterName(this.FileLocation);
        this.ChapterIndex = Chapters.GetChapterIndex(ChapterId);
    }

    public void Refresh()
    {
        this.SaveSlot.UpdateImage(this.ChapterId);
        this.SaveSlot.UpdateChapterLabel(this.ChapterId);
        this.SaveSlot.EnableBackup();
    }

    public bool Import(PracticeSave practiceSave)
    {
        if (practiceSave != null)
        {
            if (File.Exists(practiceSave.FileLocation))
            {
                if (File.Exists(this.FileLocation))
                {
                    if (UserSettings.SaveOverwritePrompt == true)
                    {
                        var confirmResult = MessageBox.Show("Save file exists in this location, overwrite?", "Confirm", MessageBoxButtons.YesNo);
                        if (confirmResult == DialogResult.No)
                        {
                            return false;
                        }
                    }
                    if (!SaveFile.IsReadOnly(this.FileLocation))
                    {
                        File.Delete(this.FileLocation);
                    }
                    else
                    {
                        MessageBox.Show("Unable to import because the save in Slot " + this.SlotNumber + " is set to read-only.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }
                }
                string folder = this.FileLocation.Substring(0, this.FileLocation.Length - 9);
                this.ChapterId = practiceSave.ChapterId;
                this.ZoneName = practiceSave.ZoneName;
                if (!Directory.Exists(folder))
                {
                    Directory.CreateDirectory(folder);
                }
                File.Copy(practiceSave.FileLocation, this.FileLocation);
                Refresh();
                return true;
            }
        }
        return false;
    }

    public bool Backup()
    {
        string slotName = "Slot_" + SlotNumber;
        string backupDir = Path.Combine(BackupFolderName, slotName);
        if (!Directory.Exists(backupDir))
        {
            Directory.CreateDirectory(backupDir);
        }
        if (File.Exists(this.FileLocation))
        {
            string backupDestinationName = Path.Combine(backupDir, SaveFileName);
            if (File.Exists(backupDestinationName))
            {
                if (!SaveFile.IsReadOnly(backupDestinationName))
                {
                    File.Delete(backupDestinationName);
                }
                else
                {
                    MessageBox.Show("Unable to create a backup because the existing backup in Slot " + this.SlotNumber + " is set to read-only.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }
            File.Copy(this.FileLocation, backupDestinationName);
            SaveFile.SetReadOnly(backupDestinationName, false);
            return true;
        }
        return false;
    }

    public bool Restore()
    {
        string slotName = "Slot_" + SlotNumber;
        string backupDir = Path.Combine(BackupFolderName, slotName, SaveFileName);
        if (File.Exists(backupDir))
        {
            string saveDestinationName = this.FileLocation;
            if (File.Exists(saveDestinationName))
            {
                if (UserSettings.SaveOverwritePrompt == true)
                {
                    var confirmResult = MessageBox.Show("Save file exists in this location, overwrite?", "Confirm", MessageBoxButtons.YesNo);
                    if (confirmResult == DialogResult.No)
                    {
                        return false;
                    }
                }
                if (!SaveFile.IsReadOnly(this.FileLocation))
                {
                    File.Delete(saveDestinationName);
                }
                else
                {
                    MessageBox.Show("Unable to restore because the save in Slot " + this.SlotNumber + " is set to read-only.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }
            File.Copy(backupDir, saveDestinationName);
            UpdateSaveData();
            Refresh();
            return true;
        }
        return false;
    }
}