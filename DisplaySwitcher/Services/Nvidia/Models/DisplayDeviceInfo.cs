using DisplaySwitcher.Services.Nvidia.Models;

namespace DisplaySwitcher.Models;

public class DisplayDeviceInfo
{
    public string WindowsName { get; init; } = string.Empty;

    public bool IsPrimary { get; init; }

    public int Width { get; init; }

    public int Height { get; init; }

    public int Frequency { get; init; }

    public NvidiaDisplay? NvidiaDisplay { get; init; }

    public string DisplayName
    {
        get
        {
            string primaryText = IsPrimary ? " principal" : string.Empty;

            return $"{WindowsName}{primaryText} — {Width}×{Height} @ {Frequency} Hz";
        }
    }
}