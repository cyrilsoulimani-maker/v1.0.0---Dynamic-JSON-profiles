using DisplaySwitcher.Services.Edid.Models;

namespace DisplaySwitcher.Services.Timings;

public class TimingResult
{
    public int Width { get; init; }

    public int Height { get; init; }

    public int RefreshRate { get; init; }

    public int PixelClockKhz { get; init; }

    public int HorizontalBlanking { get; init; }

    public int VerticalBlanking { get; init; }

    public int HorizontalFrontPorch { get; init; }

    public int HorizontalSyncWidth { get; init; }

    public int VerticalFrontPorch { get; init; }

    public int VerticalSyncWidth { get; init; }

    public int HorizontalImageSizeMm { get; init; }

    public int VerticalImageSizeMm { get; init; }

    public EdidDetailedTimingDescriptor ToDetailedTimingDescriptor()
    {
        return new EdidDetailedTimingDescriptor
        {
            Width = Width,
            Height = Height,
            RefreshRate = RefreshRate,
            PixelClockKhz = PixelClockKhz,
            HorizontalBlanking = HorizontalBlanking,
            VerticalBlanking = VerticalBlanking,
            HorizontalFrontPorch = HorizontalFrontPorch,
            HorizontalSyncWidth = HorizontalSyncWidth,
            VerticalFrontPorch = VerticalFrontPorch,
            VerticalSyncWidth = VerticalSyncWidth,
            HorizontalImageSizeMm = HorizontalImageSizeMm,
            VerticalImageSizeMm = VerticalImageSizeMm
        };
    }
}