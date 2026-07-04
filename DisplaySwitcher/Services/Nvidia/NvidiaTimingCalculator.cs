using DisplaySwitcher.Services.Nvidia.Interop.Structures;

namespace DisplaySwitcher.Services.Nvidia;

public static class NvidiaTimingCalculator
{
    public static NvTiming CreateReducedBlankingTiming(
        uint width,
        uint height,
        uint refreshRate)
    {
        uint hFrontPorch = 48;
        uint hSyncWidth = 32;
        uint hBackPorch = 80;

        uint vFrontPorch = 3;
        uint vSyncWidth = 5;
        uint vBackPorch = 23;

        uint hTotal = width + hFrontPorch + hSyncWidth + hBackPorch;
        uint vTotal = height + vFrontPorch + vSyncWidth + vBackPorch;

        uint pixelClockHz = hTotal * vTotal * refreshRate;
        uint pixelClock10Khz = pixelClockHz / 10_000;

        NvTiming timing = NvTiming.Create();

        timing.HVisible = (ushort)width;
        timing.HBorder = 0;
        timing.HFrontPorch = (ushort)hFrontPorch;
        timing.HSyncWidth = (ushort)hSyncWidth;
        timing.HTotal = (ushort)hTotal;
        timing.HSyncPol = 1;

        timing.VVisible = (ushort)height;
        timing.VBorder = 0;
        timing.VFrontPorch = (ushort)vFrontPorch;
        timing.VSyncWidth = (ushort)vSyncWidth;
        timing.VTotal = (ushort)vTotal;
        timing.VSyncPol = 0;

        timing.Interlaced = 0;
        timing.PixelClock = pixelClock10Khz;

        timing.Extra.RefreshRate = (ushort)refreshRate;
        timing.Extra.RefreshRateX1000 = refreshRate * 1000;

        return timing;
    }
}