namespace DisplaySwitcher.Models;

public class DisplayModesInfo
{
    public string DeviceName { get; init; } = string.Empty;

    public DisplayModeInfo CurrentMode { get; init; } = new();

    public List<DisplayModeInfo> AvailableModes { get; init; } = new();
}