using DisplaySwitcher.Models;
using DisplaySwitcher.Services;
using System.Windows;
using System.Windows.Controls;

namespace DisplaySwitcher
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            var mode = DisplayService.GetCurrentMode();

            CurrentResolutionText.Text =
                $"Résolution actuelle : {mode.Width} × {mode.Height} @ {mode.Frequency} Hz";

            LoadProfiles();
        }

        private void ApplyButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (RadioButton radio in ProfilesPanel.Children)
            {
                if (radio.IsChecked == true)
                {
                    DisplayProfile profile = (DisplayProfile)radio.Tag;

                    ApplyProfile(profile);

                    return;
                }
            }

            MessageBox.Show("Veuillez sélectionner un profil.");
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
        private void LoadProfiles()
        {
            ProfilesPanel.Children.Clear();

            var profiles = ProfileService.LoadProfiles();

            foreach (DisplayProfile profile in profiles)
            {
                RadioButton radio = new RadioButton();

                radio.Content =
                    $"{profile.Name} - {profile.Width} × {profile.Height} @ {profile.Frequency} Hz";

                radio.Tag = profile;

                radio.FontSize = 18;
                radio.Foreground = System.Windows.Media.Brushes.White;
                radio.Margin = new Thickness(0, 10, 0, 10);

                ProfilesPanel.Children.Add(radio);
            }
        }
    }
}