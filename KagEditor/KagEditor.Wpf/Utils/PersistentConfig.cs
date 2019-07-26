using KagEditor.Wpf.Properties;

namespace KagEditor.Wpf.Utils
{
    public static class PersistentConfig
    {
        public static string OpenFilePhysicalLocation
        {
            get => Settings.Default.OpenFilePhysicalLocation;
            set
            {
                Settings.Default.OpenFilePhysicalLocation = value;
                Settings.Default.Save();
            }
        }

        public static string OriginalFilePhysicalLocation
        {
            get => Settings.Default.OriginalFilePhysicalLocation;
            set
            {
                Settings.Default.OriginalFilePhysicalLocation = value;
                Settings.Default.Save();
            }
        }

        public static string OriginalFolderPhysicalLocation
        {
            get => Settings.Default.OriginalFolderPhysicalLocation;
            set
            {
                Settings.Default.OriginalFolderPhysicalLocation = value;
                Settings.Default.Save();
            }
        }

        public static string CleanedFolderPhysicalLocation
        {
            get => Settings.Default.CleanedFolderPhysicalLocation;
            set
            {
                Settings.Default.CleanedFolderPhysicalLocation = value;
                Settings.Default.Save();
            }
        }

        public static string CountFolderPhysicalLocation
        {
            get => Settings.Default.CountFolderPhysicalLocation;
            set
            {
                Settings.Default.CountFolderPhysicalLocation = value;
                Settings.Default.Save();
            }
        }

        public static string TranslatedFolderPhysicalLocation
        {
            get => Settings.Default.TranslatedFolderPhysicalLocation;
            set
            {
                Settings.Default.TranslatedFolderPhysicalLocation = value;
                Settings.Default.Save();
            }
        }

        public static string RescriptedFolderPhysicalLocation
        {
            get => Settings.Default.RescriptedFolderPhysicalLocation;
            set
            {
                Settings.Default.RescriptedFolderPhysicalLocation = value;
                Settings.Default.Save();
            }
        }

        public static string ToTranslateFolderPhysicalLocation
        {
            get => Settings.Default.ToTranslateFolderPhysicalLocation;
            set
            {
                Settings.Default.ToTranslateFolderPhysicalLocation = value;
                Settings.Default.Save();
            }
        }

        public static string RetranslatedFolderPhysicalLocation
        {
            get => Settings.Default.RetranslatedFolderPhysicalLocation;
            set
            {
                Settings.Default.RetranslatedFolderPhysicalLocation = value;
                Settings.Default.Save();
            }
        }
    }
}
