using System.Runtime.InteropServices;

namespace DisplaySwitcher.Services.Nvidia.Interop.Structures;

[StructLayout(LayoutKind.Sequential, Pack = 8)]
public struct NvTimingExt
{
    public uint Flag;
    public ushort RefreshRate;
    public uint RefreshRateX1000;
    public uint Aspect;
    public ushort Repetition;
    public uint Status;

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 40)]
    public byte[] Name;

    public static NvTimingExt Create()
    {
        return new NvTimingExt
        {
            Name = new byte[40]
        };
    }
}