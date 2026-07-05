using DisplaySwitcher.Models;
using DisplaySwitcher.Services;
using DisplaySwitcher.Services.Edid;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;

namespace DisplaySwitcher
{
    public partial class MainWindow : Window
    {
        private readonly TrayIconService _trayIconService;
        private readonly GpuVendor _gpuVendor;

        public ObservableCollection<DisplayProfile> Profiles { get; } = new();

        public MainWindow()
        {
            MonitorIdentificationService service = new();

            service.DumpMonitors(
                Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                    "DisplaySwitcher_Monitors_v2.txt"));

            InitializeComponent();

            _gpuVendor = new GpuDetectionService().DetectGpu();

            LoadProfiles();

            new DisplayEnumerationService().DumpMonitors();

            _trayIconService = new TrayIconService(Profiles);

            _trayIconService.OpenRequested += () =>
            {
                Show();
                WindowState = WindowState.Normal;
                Activate();
            };

            _trayIconService.ProfileRequested += profile =>
            {
                ApplyProfile(profile);
            };

            _trayIconService.ManageProfilesRequested += OpenProfileManager;

            DataContext = this;

            RefreshCurrentResolutionText();
            SelectActiveProfile();
            RefreshCurrentProfileCard();
        }

        private void LoadProfiles()
        {
            Profiles.Clear();

            foreach (DisplayProfile profile in ProfileService.LoadProfiles())
            {
                Profiles.Add(profile);
            }
        }

        private void ApplyButton_Click(object sender, RoutedEventArgs e)
        {
            DisplayProfile? selectedProfile =
                Profiles.FirstOrDefault(profile => profile.IsSelected);

            if (selectedProfile != null)
            {
                ApplyProfile(selectedProfile);
            }
            else
            {
                System.Windows.MessageBox.Show("Veuillez sélectionner un profil.");
            }
        }

        private void ApplyProfile(DisplayProfile profile)
        {
            foreach (DisplayProfile item in Profiles)
            {
                item.IsSelected = false;
            }

            profile.IsSelected = true;

            if (string.IsNullOrWhiteSpace(profile.DisplayDeviceName))
            {
                System.Windows.MessageBox.Show(
                    $"Impossible d'appliquer le profil {profile.Name} : aucun écran n'est associé à ce profil.");

                return;
            }

            bool success = DisplayService.SetDisplayMode(
                profile.DisplayDeviceName,
                profile.Width,
                profile.Height,
                profile.Frequency);

            if (success)
            {
                RefreshCurrentResolutionText();
                RefreshCurrentProfileCard();
                _trayIconService.RefreshContextMenu();
            }
            else
            {
                System.Windows.MessageBox.Show($"Impossible d'appliquer le mode {profile.Name}.");
            }
        }

        private void RefreshCurrentResolutionText()
        {
            DisplayConfigService displayConfigService = new();

            CurrentDisplayState? state =
                displayConfigService.GetCurrentDisplayState();

            CurrentDisplayNameText.Text =
                state?.FriendlyName ?? "Écran principal";

            if (state != null)
            {
                CurrentResolutionText.Text =
                    $"{state.Width} × {state.Height} @ {state.RefreshRate:F0} Hz";
            }
            else
            {
                CurrentResolutionText.Text = "-";
            }
        }

        private void SelectActiveProfile()
        {
            var currentMode = DisplayService.GetCurrentMode();

            DisplayProfile? activeProfile = Profiles.FirstOrDefault(profile =>
                profile.Width == currentMode.Width &&
                profile.Height == currentMode.Height &&
                profile.Frequency == currentMode.Frequency);

            if (activeProfile != null)
            {
                foreach (DisplayProfile profile in Profiles)
                {
                    profile.IsSelected = false;
                }

                activeProfile.IsSelected = true;
            }
        }

        private void ManageProfilesButton_Click(object sender, RoutedEventArgs e)
        {
            OpenProfileManager();
        }

        private void OpenProfileManager()
        {
            ProfileManagerWindow window = new ProfileManagerWindow(Profiles);
            window.Owner = this;

            window.ProfilesSaved += () =>
            {
                _trayIconService.RefreshContextMenu();
            };

            window.ShowDialog();
        }

        private void RefreshCurrentProfileCard()
        {
            DisplayProfile? activeProfile =
                Profiles.FirstOrDefault(profile => profile.IsSelected);

            if (activeProfile != null)
            {
                CurrentProfileStatusText.Text = "● Profil actif";
                CurrentProfileNameText.Text = activeProfile.Name;
            }
            else
            {
                CurrentProfileStatusText.Text = "● Aucun profil correspondant";
                CurrentProfileNameText.Text = "Configuration personnalisée";
            }
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            RefreshCurrentResolutionText();
            SelectActiveProfile();
            RefreshCurrentProfileCard();

            EdidOverrideService edidOverrideService = new();
            EdidPreviewService edidPreviewService = new();

            string dumpPath =
                edidOverrideService.DumpDetectedEdidsToDesktop();

            string backupReportPath =
                edidOverrideService.BackupActiveEdidsToDesktop();

            string previewReportPath =
                edidPreviewService.ExportPreviewForActiveDisplaysToDesktop(
                    width: 1500,
                    height: 790,
                    refreshRate: 60);

            System.Windows.MessageBox.Show(
                $"Dump EDID généré :\n{dumpPath}\n\nBackup EDID généré :\n{backupReportPath}\n\nPreview EDID généré :\n{previewReportPath}",
                "DisplaySwitcher EDID",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }

        protected override void OnStateChanged(EventArgs e)
        {
            base.OnStateChanged(e);

            if (WindowState == WindowState.Minimized)
            {
                Hide();
            }
        }
    }
}