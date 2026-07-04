namespace DisplaySwitcher.Services.Edid.Models;

public class EdidMonitor
{
    public string FriendlyName { get; init; } = string.Empty;

    public string DisplayDeviceName { get; init; } = string.Empty;

    public string MonitorUid { get; init; } = string.Empty;

    public string RegistryPath { get; init; } = string.Empty;

    public byte[] Edid { get; init; } = [];

    public override string ToString()
    {
        return $"{FriendlyName} ({MonitorUid})";
    }
}