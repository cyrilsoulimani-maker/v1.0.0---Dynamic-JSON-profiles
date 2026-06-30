using DisplaySwitcher.Models;
using DisplaySwitcher.Services;
using System.Collections.Generic;
using System.Windows;
using System.Linq;

namespace DisplaySwitcher
{
    public partial class MainWindow : Window
    {
        public List<DisplayProfile> Profiles { get; set; } = new List<DisplayProfile>();

        public MainWindow()
        {
            InitializeComponent();

            Profiles = ProfileService.LoadProfiles();

            var currentMode = DisplayService.GetCurrentMode();

            DisplayProfile? activeProfile = Profiles.FirstOrDefault(profile =>
                profile.Width == currentMode.Width &&
                profile.Height == currentMode.Height &&
                profile.Frequency == currentMode.Frequency);

            if (activeProfile != null)
            {
                activeProfile.IsSelected = true;
            }

            DataContext = this;

            CurrentResolutionText.Text =
                $"Résolution actuelle : {currentMode.Width} × {currentMode.Height} @ {currentMode.Frequency} Hz";
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
                MessageBox.Show("Veuillez sélectionner un profil.");
            }
        }

        private void ApplyProfile(DisplayProfile profile)
        {
            bool success = DisplayService.SetDisplayMode(
                profile.Width,
                profile.Height,
                profile.Frequency);

            if (success)
            {
                MessageBox.Show(
                    $"Mode {profile.Name} appliqué : {profile.Width} × {profile.Height} @ {profile.Frequency} Hz");

                CurrentResolutionText.Text =
                    $"Résolution actuelle : {profile.Width} × {profile.Height} @ {profile.Frequency} Hz";
            }
            else
            {
                MessageBox.Show($"Impossible d'appliquer le mode {profile.Name}.");
            }
        }
    }
}