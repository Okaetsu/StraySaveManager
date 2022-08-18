using MaterialSkin;
using MaterialSkin.Controls;
using ExtensionMethods;
using StraySaveManager.Helpers;

namespace StraySaveManager
{
    public partial class Main : MaterialForm
    {
        private System.Timers.Timer TextFadeoutTimer = new();

        private string PracticeFolderNamePreview = "";
        private int SelectedSaveSlot = 1;
        private bool HasLoadedPracticeSaves = false;
        private bool IsCreatingPracticeSave = false;

        private const string DefaultPracticeTag = "New";
        private const string PracticeFolderName = Constants.PracticeFolderName;
        private const string SaveFileName = Constants.SaveFileName;
        private const int MaxSlots = Constants.MaxSlots;
        private const int StatusMessageDuration = Constants.StatusMessageDuration;

        private SlotManager SlotManager = new SlotManager();
        private PracticeManager PracticeManager;

        public Main()
        {
            InitializeComponent();

            PracticeManager = new PracticeManager(this);

            versionNumberLabel.Text = $"Build: {Constants.BuildAuthor} (Version {ProductVersion})";

            WindowSettings.Load(this);
            UserSettings.Load();

            MaterialSkinManager materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.AddFormToManage(this);
            materialSkinManager.Theme = MaterialSkinManager.Themes.DARK;
            materialSkinManager.ColorScheme = new ColorScheme(Primary.BlueGrey800, Primary.BlueGrey900, Primary.BlueGrey500, Accent.LightBlue200, TextShade.WHITE);

            this.Icon = Properties.Resources.Icon;
            this.Text = "Stray Save Manager";
        }

        private void Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            WindowSettings.Save(this);
            UserSettings.Save();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Setup();
            this.Activated += RefreshOnFocus;
        }

        void RefreshOnFocus(object sender, EventArgs e)
        {
            PracticeManager.Load();
            LoadPracticeButtons();

            if (IsCreatingPracticeSave == true)
            {
                UserSave userSave = SlotManager.GetSave(SelectedSaveSlot);

                if (userSave != null)
                {
                    userSave.UpdateSaveData();
                    userSave.Refresh();

                    Chapter chapter = Chapters.Get(userSave.ChapterId);

                    if (chapter != null)
                    {
                        PracticeFolderNamePreview = "Chapter " + chapter.Index + " - " + chapter.Name;

                        if (practiceNoteTextBox.Text != "")
                        {
                            practiceFilePreviewLabel.Text = PracticeFolderNamePreview + " [" + practiceNoteTextBox.Text + "]";
                            practiceButtonPreview.Text = practiceNoteTextBox.Text;
                        }
                        else
                        {
                            practiceFilePreviewLabel.Text = PracticeFolderNamePreview;
                            practiceButtonPreview.Text = "New";
                        }
                        practiceButtonPreview.Size = new Size(25 + (practiceNoteTextBox.Text.Length * 8), 30);
                    }
                }
            }
        }

        void Setup()
        {
            if (UserSettings.SaveFolder != "")
            {
                if (Directory.Exists(UserSettings.SaveFolder))
                {
                    SlotManager.Load(this);
                }
                mainTabControl.SelectedIndex = Properties.Settings.Default.TabIndex;
            }
            else
            {
                // Show Settings Tab when launching for the first time and disable other tabs until a valid save folder has been selected.
                mainTabControl.SelectTab(2);
                mainTab.Enabled = false;
                practiceTab.Enabled = false;
            }

            if (!HasLoadedPracticeSaves)
            {
                PracticeManager.Load();
                LoadPracticeButtons();
            }

            saveDirectoryTextBox.Text = UserSettings.SaveFolder;
            saveOverwriteCheckbox.Checked = UserSettings.SaveOverwritePrompt;
        }

        void Backup(int slotNumber)
        {
            if (slotNumber.IsWithinRange(1, MaxSlots))
            {
                UserSave userSave = SlotManager.GetSave(slotNumber);
                if (userSave != null)
                {
                    bool wasSuccessful = userSave.Backup();
                    if (wasSuccessful)
                    {
                        SlotManager.CheckBackup(slotNumber);
                        ShowTimedText(mainPageStatusLabel, "Successfully backed up save in Slot " + slotNumber, StatusMessageDuration);
                        return;
                    }
                }
                return;
            }
            MessageBox.Show("Slot number wasn't within the allowed range. Must be within " + 1 + " - " + MaxSlots, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        void Restore(int slotNumber)
        {
            if (slotNumber.IsWithinRange(1, MaxSlots))
            {
                UserSave userSave = SlotManager.GetSave(slotNumber);
                if (userSave != null)
                {
                    bool wasSuccessful = userSave.Restore();
                    if (wasSuccessful)
                    {
                        if (userSave.ChapterId != "" && userSave.ChapterId != null)
                        {
                            practiceSlotChapterLabel.Text = Chapters.GetChapterName(userSave.ChapterId);
                            creatorPageButton.Enabled = true;
                            ShowTimedText(mainPageStatusLabel, "Successfully restored save in Slot " + slotNumber, StatusMessageDuration);
                            return;
                        }
                    }
                    return;
                }
                MessageBox.Show("Something went wrong with restoring the file. No changes were made.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        void LoadPracticeFile(PracticeSave practiceSave)
        {
            string? practiceFile = practiceSave.FileLocation;
            if (File.Exists(practiceFile))
            {
                if (SelectedSaveSlot.IsWithinRange(1, MaxSlots))
                {
                    UserSave? userSave = SlotManager.GetSave(SelectedSaveSlot);
                    if (userSave != null)
                    {
                        bool wasSuccessful = userSave.Import(practiceSave);
                        if (wasSuccessful)
                        {
                            practiceSlotChapterLabel.Text = Chapters.GetChapterName(practiceSave.ChapterId);
                            slotReadOnlyCheckbox.Enabled = true;
                            slotReadOnlyCheckbox.Checked = SaveFile.IsReadOnly(userSave.FileLocation);
                            creatorPageButton.Enabled = true;
                            ShowTimedText(importStatusLabel, "Save imported. Reload Checkpoint or select Continue from the Main Menu.", StatusMessageDuration);
                        }
                    }
                }
            }
        }

        void SelectPracticeSlot(int slotNumber)
        {
            if (slotNumber.IsWithinRange(1, MaxSlots))
            {
                levelSelectSubMenu.SelectTab(1);
                SelectedSaveSlot = slotNumber;
                selectedSlotLabel.Text = "Slot " + slotNumber;
                practiceSelectedSlotLabel.Text = "Slot " + slotNumber;

                UserSave? userSave = SlotManager.GetSave(SelectedSaveSlot);

                if (userSave != null)
                {
                    if (userSave.ChapterId != "" && userSave.ChapterId != null)
                    {
                        practiceSlotChapterLabel.Text = Chapters.GetChapterName(userSave.ChapterId);
                        creatorPageButton.Enabled = true;
                    }
                    else
                    {
                        practiceSlotChapterLabel.Text = "Empty";
                        creatorPageButton.Enabled = false;
                    }

                    if (File.Exists(userSave.FileLocation))
                    {
                        slotReadOnlyCheckbox.Enabled = true;
                        slotReadOnlyCheckbox.Checked = SaveFile.IsReadOnly(userSave.FileLocation);
                    }
                    else
                    {
                        slotReadOnlyCheckbox.Enabled = false;
                        slotReadOnlyCheckbox.Checked = false;
                    }
                }
            }
        }

        void LoadPracticeButtons()
        {
            foreach (var practiceSave in PracticeManager.SaveList)
            {
                if (practiceSave.Button == null)
                {
                    AddPracticeButton(practiceSave);
                }
            }
        }

        void AddPracticeButton(PracticeSave practiceSave)
        {
            MaterialButton btn = new()
            {
                Text = practiceSave.Description == "" ? DefaultPracticeTag : practiceSave.Description,
                Visible = true,
                Cursor = Cursors.Hand
            };
            btn.Click += (sender, e) => { LoadPracticeFile(practiceSave); };

            Control flowPanel = this.Controls.Find("flowLayoutPanel" + practiceSave.ChapterIndex, true)[0];
            if (flowPanel != null)
            {
                flowPanel.Controls.Add(btn);
            }

            practiceSave.Button = btn;
        }

        void ShowTimedText(MaterialLabel label, string text, int interval)
        {
            // If there is already another text visible, hide and unhide the label to make it obvious something happened.
            label.Visible = false;
            label.Visible = true;

            TextFadeoutTimer.Interval = interval;
            label.Invoke(new Action(() => label.Text = text));

            TextFadeoutTimer.Elapsed += (s, en) => {
                label.Invoke(new Action(() => label.Text = ""));
                TextFadeoutTimer.Stop();
            };
            TextFadeoutTimer.Restart();
        }

        void OpenFolderBrowser()
        {
            string saveGameFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Hk_project\Saved\SaveGames";
            FolderBrowserDialog dialog = new FolderBrowserDialog
            {
                InitialDirectory = saveGameFolder
            };

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                if (SlotManager.IsValidSaveFolder(dialog.SelectedPath))
                {
                    UserSettings.SaveFolder = dialog.SelectedPath;
                    saveDirectoryTextBox.Text = UserSettings.SaveFolder;
                    mainTab.Enabled = true;
                    practiceTab.Enabled = true;
                    SlotManager.Load(this);
                }
                else
                {
                    MessageBox.Show("Invalid save folder location specified.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        void OpenCreationPage()
        {
            UserSave? userSave = SlotManager.GetSave(SelectedSaveSlot);
            if (userSave != null)
            {
                userSave.UpdateSaveData();
                userSave.Refresh();

                Chapter chapter = Chapters.Get(userSave.ChapterId);
                if (chapter != null)
                {
                    IsCreatingPracticeSave = true;
                    PracticeFolderNamePreview = "Chapter " + chapter.Index + " - " + chapter.Name;
                    practiceFilePreviewLabel.Text = PracticeFolderNamePreview;
                    practiceButtonPreview.Text = DefaultPracticeTag;
                    practiceNoteTextBox.Text = "";
                    levelSelectSubMenu.SelectTab(2);
                }
            }
        }

        void FinishCreation()
        {
            IsCreatingPracticeSave = false;
            string folderName = practiceFilePreviewLabel.Text;
            string folderPath = Path.Combine(PracticeFolderName, folderName);
            string practiceFile = Path.Combine(folderPath, SaveFileName);
            string description = practiceNoteTextBox.Text;

            Directory.CreateDirectory(folderPath);

            UserSave userSave = SlotManager.GetSave(SelectedSaveSlot);

            if (userSave != null)
            {
                string saveFile = userSave.FileLocation;
                if (File.Exists(saveFile))
                {
                    File.Copy(saveFile, practiceFile, true);

                    PracticeSave practiceSave = new PracticeSave(description, practiceFile);
                    PracticeManager.Add(practiceSave);
                    AddPracticeButton(practiceSave);

                    levelSelectSubMenu.SelectTab(1);
                    practiceFilePreviewLabel.Text = PracticeFolderNamePreview;
                    practiceButtonPreview.Text = DefaultPracticeTag;
                    chapterDropdown.SelectedIndex = userSave.ChapterIndex - 1;
                    ShowTimedText(importStatusLabel, "Practice File Created.", StatusMessageDuration);
                }
            }
        }
        void CancelCreation()
        {
            IsCreatingPracticeSave = false;
            levelSelectSubMenu.SelectTab(1);
            practiceFilePreviewLabel.Text = PracticeFolderNamePreview;
            practiceButtonPreview.Text = DefaultPracticeTag;
            practiceButtonPreview.Refresh();
        }

        private void slotBackupButton1_Click(object sender, EventArgs e)
        {
            Backup(1);
        }

        private void slotBackupButton2_Click(object sender, EventArgs e)
        {
            Backup(2);
        }

        private void slotBackupButton3_Click(object sender, EventArgs e)
        {
            Backup(3);
        }

        private void slotRestoreButton1_Click(object sender, EventArgs e)
        {
            Restore(1);
        }

        private void slotRestoreButton2_Click(object sender, EventArgs e)
        {
            Restore(2);
        }

        private void slotRestoreButton3_Click(object sender, EventArgs e)
        {
            Restore(3);
        }

        private void slotSelectButton1_Click(object sender, EventArgs e)
        {
            SelectPracticeSlot(1);
        }

        private void slotSelectButton2_Click(object sender, EventArgs e)
        {
            SelectPracticeSlot(2);
        }

        private void slotSelectButton3_Click(object sender, EventArgs e)
        {
            SelectPracticeSlot(3);
        }

        private void slotChangeButton_Click(object sender, EventArgs e)
        {
            levelSelectSubMenu.SelectTab(0);
        }

        private void fileBrowseButton_Click(object sender, EventArgs e)
        {
            OpenFolderBrowser();
        }

        private void saveOverwriteCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            UserSettings.SaveOverwritePrompt = saveOverwriteCheckbox.Checked;
        }

        private void chapterDropdown_SelectedIndexChanged(object sender, EventArgs e)
        {
            practiceButtonsTab.SelectTab(chapterDropdown.SelectedIndex);
        }

        private void refreshButton_Click(object sender, EventArgs e)
        {
            SlotManager.Load(this);
        }

        private void slotReadOnlyCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            UserSave? userSave = SlotManager.GetSave(SelectedSaveSlot);
            if (userSave != null)
            {
                if (File.Exists(userSave.FileLocation))
                {
                    SaveFile.SetReadOnly(userSave.FileLocation, slotReadOnlyCheckbox.Checked);
                }
                else
                {
                    practiceSlotChapterLabel.Text = "Empty";
                    slotReadOnlyCheckbox.Enabled = false;
                    slotReadOnlyCheckbox.Checked = false;
                }
            }
        }

        private void creatorPageButton_Click(object sender, EventArgs e)
        {
            OpenCreationPage();
        }

        private void practiceNoteTextBox_TextChanged(object sender, EventArgs e)
        {
            if (practiceNoteTextBox.Text != "")
            {
                practiceFilePreviewLabel.Text = PracticeFolderNamePreview + " [" + practiceNoteTextBox.Text + "]";
                practiceButtonPreview.Text = practiceNoteTextBox.Text;
                practiceButtonPreview.Size = new Size(25 + (practiceNoteTextBox.Text.Length * 8), 30);
            }
            else
            {
                practiceFilePreviewLabel.Text = PracticeFolderNamePreview;
                practiceButtonPreview.Text = DefaultPracticeTag;
                practiceButtonPreview.Size = new Size(25, 30);
            }
        }

        private void finishCreationButton_Click(object sender, EventArgs e)
        {
            FinishCreation();
        }

        private void cancelCreationButton_Click(object sender, EventArgs e)
        {
            CancelCreation();
        }
    }
}