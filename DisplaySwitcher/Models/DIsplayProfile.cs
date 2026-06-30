namespace DisplaySwitcher.Models
{
    public class DisplayProfile
    {

        public string Name { get; set; } = "";
        public string DisplayDeviceName { get; set; } = string.Empty;
        public int Width { get; set; }
        public int Height { get; set; }
        public int Frequency { get; set; }
        public bool IsSelected { get; set; }
        public bool CreateCustomResolution { get; set; }
    }
}
