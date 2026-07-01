using DisplaySwitcher.Services.Nvidia.Models;

namespace DisplaySwitcher.Models;

public class DisplayDeviceInfo
{
    public string WindowsName { get; init; } = string.Empty;

    public bool IsPrimary { get; init; }

    public int Width { get; init; }

    public int Height { get; init; }

    public int Frequency { get; init; }

    public string FriendlyName { get; init; } = string.Empty;

    public NvidiaDisplay? NvidiaDisplay { get; init; }

    public string DisplayName
    {
        get
        {
            string name = string.IsNullOrWhiteSpace(FriendlyName)
                ? (IsPrimary ? "Écran principal" : "Écran secondaire")
                : FriendlyName;

            return $"{name} — {Width} × {Height} @ {Frequency} Hz";
        }
    }
}