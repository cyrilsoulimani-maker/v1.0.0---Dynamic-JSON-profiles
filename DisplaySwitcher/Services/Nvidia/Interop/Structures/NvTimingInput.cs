using System.Runtime.InteropServices;

namespace DisplaySwitcher.Services.Nvidia.Interop.Structures;

[StructLayout(LayoutKind.Sequential)]
public struct NvTimingInput
{
    private const uint NvTimingInputVersion = 32u | (1u << 16);

    public uint Version;
    public uint Width;
    public uint Height;
    public float RefreshRate;
    public NvTimingFlag Flag;
    public uint Type;

    public static NvTimingInput Create(uint width, uint height, float refreshRate, uint timingType)
    {
        return new NvTimingInput
        {
            Version = NvTimingInputVersion,
            Width = width,
            Height = height,
            RefreshRate = refreshRate,
            Flag = NvTimingFlag.Default,
            Type = timingType
        };
    }
}