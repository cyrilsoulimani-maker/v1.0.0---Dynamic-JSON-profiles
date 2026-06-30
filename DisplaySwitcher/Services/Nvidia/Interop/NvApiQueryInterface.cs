using System;
using System.Runtime.InteropServices;

namespace DisplaySwitcher.Services.Nvidia.Interop;

public static partial class NvApi
{
    [DllImport(NvApiDll, EntryPoint = "nvapi_QueryInterface", CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr QueryInterface(uint functionId);
}