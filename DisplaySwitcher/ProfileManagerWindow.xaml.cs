using DisplaySwitcher.Models;
using DisplaySwitcher.Services;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace DisplaySwitcher
{
    public partial class ProfileManagerWindow : Window
    {
        private bool _isLoadingModes;

        public ObservableCollection<DisplayProfile> Profiles { get; }
        public ObservableCollection<DisplayModeInfo> AvailableModes { get; } = new();
        public ObservableCollection<DisplayDeviceInfo> Displays { get; } = new();

        public event Action? ProfilesSaved;

        public ProfileManagerWindow(ObservableCollection<DisplayProfile> profiles)
        {
            InitializeComponent();

            Profiles = profiles;

            DisplayEnumerationService displayService = new();

            foreach (DisplayDeviceInfo display in displayService.GetDisplays())
            {
                Displays.Add(display);
            }

            DataContext = this;
        }

        private void LoadAvailableModes(string deviceName)
        {
            _isLoadingModes = true;

            AvailableModes.Clear();
            ModeComboBox.SelectedItem = null;

            if (string.IsNullOrWhiteSpace(deviceName))
            {
                _isLoadingModes = false;
                return;
            }

            foreach (DisplayModeInfo mode in DisplayService.GetAvailableModes(deviceName))
            {
                AvailableModes.Add(mode);
            }

            if (ProfilesListBox.SelectedItem is DisplayProfile profile)
            {
                DisplayModeInfo? matchingMode =
                    AvailableModes.FirstOrDefault(mode =>
                        mode.Width == profile.Width &&
                        mode.Height == profile.Height &&
                        mode.Frequency == profile.Frequency);

                if (matchingMode != null)
                {
                    ModeComboBox.SelectedItem = matchingMode;
                }
            }

            _isLoadingModes = false;
        }

        private void ProfilesListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ProfilesListBox.SelectedItem is not DisplayProfile selectedProfile)
                return;

            if (string.IsNullOrWhiteSpace(selectedProfile.DisplayDeviceName))
            {
                DisplayDeviceInfo? primaryDisplay =
                    Displays.FirstOrDefault(display => display.IsPrimary);

                if (primaryDisplay != null)
                {
                    selectedProfile.DisplayDeviceName = primaryDisplay.WindowsName;
                }
            }

            LoadAvailableModes(selectedProfile.DisplayDeviceName);
        }

        private void DisplayComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ProfilesListBox.SelectedItem is not DisplayProfile selectedProfile)
                return;

            LoadAvailableModes(selectedProfile.DisplayDeviceName);
        }

        private void ModeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_isLoadingModes)
                return;

            if (ProfilesListBox.SelectedItem is not DisplayProfile profile)
                return;

            if (ModeComboBox.SelectedItem is not DisplayModeInfo mode)
                return;

            profile.Width = mode.Width;
            profile.Height = mode.Height;
            profile.Frequency = mode.Frequency;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            ProfileService.SaveProfiles(Profiles.ToList());

            ProfilesSaved?.Invoke();

            Close();
        }

        private void NewProfileButton_Click(object sender, RoutedEventArgs e)
        {
            DisplayDeviceInfo? primaryDisplay =
                Displays.FirstOrDefault(display => display.IsPrimary)
                ?? Displays.FirstOrDefault();

            DisplayProfile profile = new DisplayProfile
            {
                Name = GetUniqueProfileName("Nouveau profil"),
                DisplayDeviceName = primaryDisplay?.WindowsName ?? string.Empty,
                Width = primaryDisplay?.Width ?? 1920,
                Height = primaryDisplay?.Height ?? 1080,
                Frequency = primaryDisplay?.Frequency ?? 60
            };

            Profiles.Add(profile);

            ProfilesListBox.SelectedItem = profile;

            NameTextBox.Focus();
            NameTextBox.SelectAll();
        }

        private string GetUniqueProfileName(string baseName)
        {
            string name = baseName;
            int index = 2;

            while (Profiles.Any(profile => profile.Name == name))
            {
                name = $"{baseName} ({index})";
                index++;
            }

            return name;
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (ProfilesListBox.SelectedItem is not DisplayProfile profile)
            {
                System.Windows.MessageBox.Show("Veuillez sélectionner un profil.");
                return;
            }

            if (Profiles.Count == 1)
            {
                System.Windows.MessageBox.Show("Impossible de supprimer le dernier profil.");
                return;
            }

            MessageBoxResult result =
                System.Windows.MessageBox.Show(
                    $"Supprimer le profil \"{profile.Name}\" ?",
                    "Confirmation",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

            if (result != MessageBoxResult.Yes)
                return;

            int index = Profiles.IndexOf(profile);

            Profiles.Remove(profile);

            if (Profiles.Count > 0)
            {
                int newIndex = Math.Min(index, Profiles.Count - 1);
                ProfilesListBox.SelectedIndex = newIndex;
            }
        }

        private void NumericOnly_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            e.Handled = !int.TryParse(e.Text, out _);
        }
    }
}