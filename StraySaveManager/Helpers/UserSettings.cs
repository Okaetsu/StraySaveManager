namespace StraySaveManager.Helpers;

public static class UserSettings
{
    public static string SaveFolder = "";
    public static bool SaveOverwritePrompt = false;

    public static void Save()
    {
        string straySaveFolder = Properties.Settings.Default.straySaveFolder;
        bool saveOverwritePrompt = Properties.Settings.Default.saveOverwritePrompt;

        // Check if Settings have changed to avoid unnecessary writes to disk.
        if (straySaveFolder != SaveFolder || saveOverwritePrompt != SaveOverwritePrompt)
        {
            Properties.Settings.Default.straySaveFolder = SaveFolder;
            Properties.Settings.Default.saveOverwritePrompt = SaveOverwritePrompt;
            Properties.Settings.Default.Save();
        }
    }

    public static void Load()
    {
        SaveFolder = Properties.Settings.Default.straySaveFolder;
        SaveOverwritePrompt = Properties.Settings.Default.saveOverwritePrompt;
    }
}