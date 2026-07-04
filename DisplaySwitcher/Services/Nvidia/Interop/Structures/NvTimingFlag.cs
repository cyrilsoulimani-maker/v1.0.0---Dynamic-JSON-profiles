using System.Runtime.InteropServices;

namespace DisplaySwitcher.Services.Nvidia.Interop.Structures;

[StructLayout(LayoutKind.Sequential)]
public struct NvTimingFlag
{
    public uint InterlacedAndReserved;
    public uint TimingId;
    public uint Scaling;

    public static NvTimingFlag Default =>
        new()
        {
            InterlacedAndReserved = 0,
            TimingId = 0,
            Scaling = 0
        };
}