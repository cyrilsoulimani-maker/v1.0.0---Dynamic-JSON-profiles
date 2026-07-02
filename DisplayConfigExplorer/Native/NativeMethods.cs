using System.Runtime.InteropServices;

namespace DisplayConfigExplorer.Native;

internal static class NativeMethods
{
    [DllImport("user32.dll")]
    internal static extern int GetDisplayConfigBufferSizes(
        uint flags,
        out uint numPathArrayElements,
        out uint numModeInfoArrayElements);
}