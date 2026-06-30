using System.Runtime.InteropServices;

namespace DisplaySwitcher.Services.Nvidia.Interop.Structures;

[StructLayout(LayoutKind.Sequential)]
public struct NvGpuDisplayIds
{
    public uint Version;
    public uint ConnectorType;
    public uint DisplayId;
    public uint Flags;

    public static NvGpuDisplayIds Create()
    {
        return new NvGpuDisplayIds
        {
            Version = (uint)(Marshal.SizeOf<NvGpuDisplayIds>() | (3 << 16))
        };
    }
}