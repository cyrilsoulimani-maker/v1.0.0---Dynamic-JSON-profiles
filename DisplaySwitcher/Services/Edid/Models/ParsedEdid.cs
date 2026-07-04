namespace DisplaySwitcher.Services.Edid.Models;

public class ParsedEdid
{
    public bool IsValidHeader { get; init; }

    public bool IsChecksumValid { get; init; }

    public int BlockCount { get; init; }

    public string ManufacturerId { get; init; } = string.Empty;

    public string ProductCode { get; init; } = string.Empty;

    public string SerialNumber { get; init; } = string.Empty;

    public string MonitorName { get; init; } = string.Empty;
}