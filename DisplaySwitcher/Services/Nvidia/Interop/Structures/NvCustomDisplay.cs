using System.Runtime.InteropServices;

namespace DisplaySwitcher.Services.Nvidia.Interop.Structures;

[StructLayout(LayoutKind.Sequential, Pack = 8)]
public struct NvCustomDisplay
{
    private const uint NvCustomDisplayVersion = 144u | (1u << 16);

    public uint Version;
    public uint Width;
    public uint Height;
    public uint Depth;
    public NvFormat ColorFormat;
    public NvViewportF SourcePartition;
    public float XRatio;
    public float YRatio;
    public NvTiming Timing;
    public uint HardwareModeSetOnly;

    public static NvCustomDisplay Create(uint width, uint height, NvTiming timing)
    {
        return new NvCustomDisplay
        {
            Version = NvCustomDisplayVersion,
            Width = width,
            Height = height,
            Depth = 32,
            ColorFormat = NvFormat.A8R8G8B8,
            SourcePartition = NvViewportF.Full,
            XRatio = 1f,
            YRatio = 1f,
            Timing = timing,
            HardwareModeSetOnly = 0
        };
    }
}