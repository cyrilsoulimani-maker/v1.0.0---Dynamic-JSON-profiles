using System;
using System.Linq;
using System.Runtime.InteropServices;

namespace DisplaySwitcher.Services.Nvidia.Interop;

public static partial class NvApi
{
    private const int NvApiMaxPhysicalGpus = 64;
    private const uint NvApiEnumPhysicalGpusId = 0xE5AC921F;

    private delegate NvApiStatus NvApiEnumPhysicalGpusDelegate(
        [Out] IntPtr[] gpuHandles,
        out int gpuCount);

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