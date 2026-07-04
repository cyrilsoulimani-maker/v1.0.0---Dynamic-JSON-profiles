namespace DisplaySwitcher.Services.Edid.Models;

public class LocatedEdid
{
    public string RegistryManufacturer { get; init; } = string.Empty;

    public string RegistryInstance { get; init; } = string.Empty;

    public string RegistryPath { get; init; } = string.Empty;

    public string MonitorUid { get; init; } = string.Empty;

    public byte[] Edid { get; init; } = [];

    public ParsedEdid Parsed { get; init; } = new();

    public string DisplayName
    {
        get
        {
            if (!string.IsNullOrWhiteSpace(Parsed.MonitorName))
                return Parsed.MonitorName;

            return $"{Parsed.ManufacturerId} {Parsed.ProductCode}";
        }
    }
}