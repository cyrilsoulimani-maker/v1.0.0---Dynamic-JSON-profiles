using DisplaySwitcher.Models;
using DisplaySwitcher.Services;
using DisplaySwitcher.Services.Nvidia;
using DisplaySwitcher.Services.Nvidia.Interop;
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
            if (!TryCreateCustomResolutionIfNeeded())
                return;

            ProfileService.SaveProfiles(Profiles.ToList());

            ProfilesSaved?.Invoke();

            Close();
        }

        private bool TryCreateCustomResolutionIfNeeded()
        {
            if (ProfilesListBox.SelectedItem is not DisplayProfile profile)
                return true;

            if (!profile.CreateCustomResolution)
                return true;

            DisplayDeviceInfo? display =
                Displays.FirstOrDefault(display =>
                    display.WindowsName == profile.DisplayDeviceName);

            if (display?.NvidiaDisplay == null)
            {
                System.Windows.MessageBox.Show(
                    "Impossible de créer la résolution personnalisée : aucun DisplayId NVIDIA n'a été trouvé pour cet écran.",
                    "Résolution personnalisée",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return false;
            }

            MessageBoxResult confirmation =
                System.Windows.MessageBox.Show(
                    $"Créer la résolution personnalisée suivante ?\n\n{profile.Width} × {profile.Height} @ {profile.Frequency} Hz",
                    "Résolution personnalisée NVIDIA",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

            if (confirmation != MessageBoxResult.Yes)
                return false;

            System.Windows.MessageBox.Show(
    $"Écran Windows : {display.WindowsName}\n" +
    $"Nom : {display.DisplayName}\n" +
    $"DisplayId NVIDIA : {display.NvidiaDisplay.DisplayId}\n" +
    $"Connecteur : {display.NvidiaDisplay.ConnectorType}\n" +
    $"Connecté : {display.NvidiaDisplay.IsConnected}\n" +
    $"Actif : {display.NvidiaDisplay.IsActive}",
    "Debug NVIDIA",
    MessageBoxButton.OK,
    MessageBoxImage.Information);

            NvidiaCustomResolutionService customResolutionService = new();

            NvApiStatus status =
                customResolutionService.CreateCustomResolution(
                    display.NvidiaDisplay.DisplayId,
                    (uint)profile.Width,
                    (uint)profile.Height,
                    (uint)profile.Frequency);

            if (status != NvApiStatus.Ok)
            {
                System.Windows.MessageBox.Show(
                    $"La création de la résolution personnalisée a échoué.\n\nÉtape : {customResolutionService.LastStep}\nCode NVAPI : {status}",
                    "Résolution personnalisée NVIDIA",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                return false;
            }

            System.Windows.MessageBox.Show(
                "Résolution personnalisée créée avec succès.",
                "Résolution personnalisée NVIDIA",
                MessageBoxButton.OK,
                MessageBoxImage.Information);

            return true;
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