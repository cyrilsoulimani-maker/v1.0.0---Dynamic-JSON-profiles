using System.Runtime.InteropServices;

namespace DisplaySwitcher.Services.Nvidia.Interop.Structures;

[StructLayout(LayoutKind.Sequential, Pack = 8)]
public struct NvTiming
{
    public ushort HVisible;
    public ushort HBorder;
    public ushort HFrontPorch;
    public ushort HSyncWidth;
    public ushort HTotal;
    public byte HSyncPol;

    public ushort VVisible;
    public ushort VBorder;
    public ushort VFrontPorch;
    public ushort VSyncWidth;
    public ushort VTotal;
    public byte VSyncPol;

    public ushort Interlaced;
    public uint PixelClock;

    public NvTimingExt Extra;

    public static NvTiming Create()
    {
        return new NvTiming
        {
            Extra = NvTimingExt.Create()
        };
    }
}