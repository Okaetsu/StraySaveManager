namespace StraySaveManager.Helpers;

public static class WindowSettings
{
    public static void Save(Form form)
    {
        // Only save the WindowState if Normal or Maximized
        switch (form.WindowState)
        {
            case FormWindowState.Normal:
            case FormWindowState.Maximized:
                Properties.Settings.Default.WindowState = form.WindowState;
                break;

            default:
                Properties.Settings.Default.WindowState = FormWindowState.Normal;
                break;
        }
        // Reset window state to normal to get the correct bounds and make the form invisible to prevent distracting the user
        form.Visible = false;

        form.WindowState = FormWindowState.Normal;
        Properties.Settings.Default.WindowPosition = form.DesktopBounds;

        Properties.Settings.Default.Save();
    }

    public static void Load(Form form)
    {
        // Default
        form.WindowState = FormWindowState.Normal;
        form.StartPosition = FormStartPosition.CenterScreen;

        // Check if the saved bounds are nonzero and visible on any screen
        if (Properties.Settings.Default.WindowPosition != Rectangle.Empty &&
            IsVisibleOnAnyScreen(Properties.Settings.Default.WindowPosition))
        {
            // First set the bounds
            form.StartPosition = FormStartPosition.Manual;
            form.DesktopBounds = Properties.Settings.Default.WindowPosition;

            // Afterwards set the window state to the saved value (which could be Maximized)
            form.WindowState = Properties.Settings.Default.WindowState;
        }
        else
        {
            // This resets the upper left corner of the window to windows standards
            form.StartPosition = FormStartPosition.CenterScreen;

            // We can still apply the saved size
            form.Size = Properties.Settings.Default.WindowPosition.Size;
        }
    }
    private static bool IsVisibleOnAnyScreen(Rectangle rect)
    {
        foreach (Screen screen in Screen.AllScreens)
        {
            if (screen.WorkingArea.IntersectsWith(rect))
            {
                return true;
            }
        }

        return false;
    }
}