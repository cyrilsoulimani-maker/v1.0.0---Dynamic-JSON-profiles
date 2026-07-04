using System.Runtime.InteropServices;

namespace DisplaySwitcher.Services.Nvidia.Interop.Structures;

[StructLayout(LayoutKind.Sequential)]
public struct NvViewportF
{
    public float X;
    public float Y;
    public float W;
    public float H;

    public static NvViewportF Full =>
        new()
        {
            X = 0f,
            Y = 0f,
            W = 1f,
            H = 1f
        };
}