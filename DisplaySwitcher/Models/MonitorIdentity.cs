namespace DisplaySwitcher.Models;

public class MonitorIdentity
{
    public static MonitorIdentity Unknown => new()
    {
        IsDetected = false,
        FriendlyName = "Moniteur inconnu"
    };

    public bool IsDetected { get; init; }

    public string FriendlyName { get; init; } = string.Empty;

    public string Manufacturer { get; init; } = string.Empty;

    public string Model { get; init; } = string.Empty;

    public string SerialNumber { get; init; } = string.Empty;
}