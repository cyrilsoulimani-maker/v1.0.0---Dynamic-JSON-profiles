using DisplaySwitcher.Models;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace DisplaySwitcher.Services
{
    public static class ProfileService
    {
        private const string ProfilesFilePath = "Data/profiles.json";

        public static List<DisplayProfile> LoadProfiles()
        {
            if (!File.Exists(ProfilesFilePath))
            {
                return new List<DisplayProfile>();
            }

            string json = File.ReadAllText(ProfilesFilePath);

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            List<DisplayProfile>? profiles =
                JsonSerializer.Deserialize<List<DisplayProfile>>(json, options);

            return profiles ?? new List<DisplayProfile>();
        }
    }
}