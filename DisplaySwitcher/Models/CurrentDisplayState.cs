namespace DisplaySwitcher.Models;

public class CurrentDisplayState
{
    public string FriendlyName { get; init; } = string.Empty;

    public uint Width { get; init; }

    public uint Height { get; init; }

    public double RefreshRate { get; init; }
}