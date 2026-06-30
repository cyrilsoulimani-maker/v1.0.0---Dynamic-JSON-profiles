using System;
using System.Runtime.InteropServices;
using System.Linq;

namespace DisplaySwitcher.Services.Nvidia.Interop;

public static class NvApi
{
    private const string NvApiDll = "nvapi64.dll";

    private const uint NvApiInitializeId = 0x0150E828;

    private delegate int NvApiInitializeDelegate();

    private const int NvApiMaxPhysicalGpus = 64;
    private const uint NvApiEnumPhysicalGpusId = 0xE5AC921F;

    private delegate NvApiStatus NvApiEnumPhysicalGpusDelegate(
    [Out] IntPtr[] gpuHandles,
    out int gpuCount);

    [DllImport(NvApiDll, EntryPoint = "nvapi_QueryInterface", CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr QueryInterface(uint functionId);

    public static bool IsAvailable()
    {
        try
        {
            IntPtr initializePointer = QueryInterface(NvApiInitializeId);

            return initializePointer != IntPtr.Zero;
        }
        catch (DllNotFoundException)
        {
            return false;
        }
        catch (EntryPointNotFoundException)
        {
            return false;
        }
        catch (BadImageFormatException)
        {
            return false;
        }
    }

    public static NvApiStatus Initialize()
    {
        IntPtr initializePointer = QueryInterface(NvApiInitializeId);

        if (initializePointer == IntPtr.Zero)
            return NvApiStatus.NotAvailable;

        var initialize =
            Marshal.GetDelegateForFunctionPointer<NvApiInitializeDelegate>(initializePointer);

        return (NvApiStatus)initialize();
    }

    public static NvApiStatus EnumPhysicalGpus(out IntPtr[] gpuHandles)
    {
        gpuHandles = new IntPtr[NvApiMaxPhysicalGpus];

        IntPtr functionPointer = QueryInterface(NvApiEnumPhysicalGpusId);

        if (functionPointer == IntPtr.Zero)
            return NvApiStatus.NotAvailable;

        var enumPhysicalGpus =
            Marshal.GetDelegateForFunctionPointer<NvApiEnumPhysicalGpusDelegate>(functionPointer);

        NvApiStatus status = enumPhysicalGpus(gpuHandles, out int gpuCount);

        if (status != NvApiStatus.Ok)
        {
            gpuHandles = Array.Empty<IntPtr>();
            return status;
        }

        gpuHandles = gpuHandles.Take(gpuCount).ToArray();
        return NvApiStatus.Ok;
    }
}