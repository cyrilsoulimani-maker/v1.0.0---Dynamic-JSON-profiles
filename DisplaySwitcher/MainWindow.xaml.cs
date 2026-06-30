using DisplaySwitcher.Models;
using DisplaySwitcher.Services;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using DisplaySwitcher.Services.Nvidia.Interop;

namespace DisplaySwitcher
{
    public partial class MainWindow : Window
    {
        private readonly TrayIconService _trayIconService;
        private readonly GpuVendor _gpuVendor;

        public ObservableCollection<DisplayProfile> Profiles { get; } = new();

        public MainWindow()
        {
            InitializeComponent();

            _gpuVendor = new GpuDetectionService().DetectGpu();

            LoadProfiles();

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

            _trayIconService.ManageProfilesRequested += () =>
            {
                ProfileManagerWindow window = new ProfileManagerWindow(Profiles);
                window.Owner = this;

                window.ProfilesSaved += () =>
                {
                    _trayIconService.RefreshContextMenu();
                };

                window.ShowDialog();
            };

            DataContext = this;

            RefreshCurrentResolutionText();
            SelectActiveProfile();
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

            bool success = DisplayService.SetDisplayMode(
                profile.Width,
                profile.Height,
                profile.Frequency);

            if (success)
            {
                RefreshCurrentResolutionText();
                _trayIconService.RefreshContextMenu();
            }
            else
            {
                System.Windows.MessageBox.Show($"Impossible d'appliquer le mode {profile.Name}.");
            }
        }

        private void RefreshCurrentResolutionText()
        {
            var mode = DisplayService.GetCurrentMode();

            CurrentResolutionText.Text =
                $"Résolution actuelle : {mode.Width} × {mode.Height} @ {mode.Frequency} Hz";
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