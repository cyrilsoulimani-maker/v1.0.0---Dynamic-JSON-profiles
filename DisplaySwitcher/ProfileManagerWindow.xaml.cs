using System.Collections.ObjectModel;
using System.Windows;
using DisplaySwitcher.Models;
using DisplaySwitcher.Services;
using System;
using System.Linq;

namespace DisplaySwitcher
{
    public partial class ProfileManagerWindow : Window
    {
        public ObservableCollection<DisplayProfile> Profiles { get; }
        public ObservableCollection<DisplayDeviceInfo> Displays { get; } = new();

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

        public event Action? ProfilesSaved;

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            ProfileService.SaveProfiles(Profiles.ToList());

            ProfilesSaved?.Invoke();

            Close();
        }
        private void NewProfileButton_Click(object sender, RoutedEventArgs e)
        {
            DisplayProfile profile = new DisplayProfile
            {
                Name = GetUniqueProfileName("Nouveau profil"),
                Width = 1920,
                Height = 1080,
                Frequency = 60
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
                System.Windows.MessageBox.Show(
                    "Veuillez sélectionner un profil.");

                return;
            }

            if (Profiles.Count == 1)
            {
                System.Windows.MessageBox.Show(
                    "Impossible de supprimer le dernier profil.");

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