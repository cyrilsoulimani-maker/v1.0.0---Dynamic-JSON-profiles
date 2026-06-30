namespace DisplaySwitcher.Models
{
    public class DisplayProfile
    {
        public string Name { get; set; } = "";
        public int Width { get; set; }
        public int Height { get; set; }
        public int Frequency { get; set; }
        public bool IsSelected { get; set; }
    }
}
