using MaterialSkin.Controls;

public class SaveSlot
{
    public int Index;
    public PictureBox ChapterThumbnail;
    public MaterialButton BackupButton;
    public MaterialButton RestoreButton;
    public MaterialLabel ChapterLabel;

    public SaveSlot(int index, Form form)
    {
        this.Index = index;

        PictureBox chapterThumbnail = (PictureBox)form.Controls.Find("slotThumbnail" + index, true)[0];
        MaterialButton backupButton = (MaterialButton)form.Controls.Find("slotBackupButton" + index, true)[0];
        MaterialButton restoreButton = (MaterialButton)form.Controls.Find("slotRestoreButton" + index, true)[0];
        MaterialLabel chapterLabel = (MaterialLabel)form.Controls.Find("slotChapterLabel" + index, true)[0];

        this.ChapterThumbnail = chapterThumbnail;
        this.BackupButton = backupButton;
        this.RestoreButton = restoreButton;
        this.ChapterLabel = chapterLabel;
    }

    public void UpdateImage(string chapterName)
    {
        Chapter chapter = Chapters.Get(chapterName);
        Bitmap chapterImage = chapter.Image;
        this.ChapterThumbnail.Image = chapterImage;
    }

    public void UpdateChapterLabel(string chapterName)
    {
        Chapter chapter = Chapters.Get(chapterName);
        string name = chapter.Name;
        this.ChapterLabel.Text = name;
    }

    /// <summary>
    /// Enables the Controls associated with this slot.
    /// </summary>
    public void Enable()
    {
        this.BackupButton.Enabled = true;
        this.RestoreButton.Enabled = true;
    }

    /// <summary>
    /// Disables the Controls associated with this slot.
    /// </summary>
    public void Disable()
    {
        this.BackupButton.Enabled = false;
        this.RestoreButton.Enabled = false;
    }

    public void EnableBackup()
    {
        this.BackupButton.Enabled = true;
    }

    public void DisableBackup()
    {
        this.BackupButton.Enabled = false;
    }

    public void EnableRestore()
    {
        this.RestoreButton.Enabled = true;
    }
    public void DisableRestore()
    {
        this.RestoreButton.Enabled = false;
    }
}