using System;
using System.Runtime.InteropServices;

namespace DisplaySwitcher.Services.Nvidia.Interop;

public static partial class NvApi
{
    private const uint NvApiGpuGetConnectedDisplayIdsId = 0x0078DBA2;

    private delegate NvApiStatus NvApiGpuGetConnectedDisplayIdsDelegate(
        IntPtr gpuHandle,
        IntPtr displayIds,
        ref uint displayIdCount,
        uint flags);

    public static NvApiStatus GetConnectedDisplayIds(
        IntPtr gpuHandle,
        out NvGpuDisplayIds[] displayIds)
    {
        displayIds = Array.Empty<NvGpuDisplayIds>();

        IntPtr functionPointer = QueryInterface(NvApiGpuGetConnectedDisplayIdsId);

        if (functionPointer == IntPtr.Zero)
            return NvApiStatus.NotAvailable;

        var getConnectedDisplayIds =
            Marshal.GetDelegateForFunctionPointer<NvApiGpuGetConnectedDisplayIdsDelegate>(
                functionPointer);

        uint displayIdCount = 0;

        NvApiStatus status = getConnectedDisplayIds(
            gpuHandle,
            IntPtr.Zero,
            ref displayIdCount,
            0);

        if (status != NvApiStatus.Ok || displayIdCount == 0)
            return status;

        int structSize = Marshal.SizeOf<NvGpuDisplayIds>();
        IntPtr buffer = Marshal.AllocHGlobal(structSize * (int)displayIdCount);

        try
        {
            for (int i = 0; i < displayIdCount; i++)
            {
                NvGpuDisplayIds item = NvGpuDisplayIds.Create();

                Marshal.StructureToPtr(
                    item,
                    buffer + i * structSize,
                    false);
            }

            status = getConnectedDisplayIds(
                gpuHandle,
                buffer,
                ref displayIdCount,
                0);

            if (status != NvApiStatus.Ok)
                return status;

            displayIds = new NvGpuDisplayIds[displayIdCount];

            for (int i = 0; i < displayIdCount; i++)
            {
                displayIds[i] =
                    Marshal.PtrToStructure<NvGpuDisplayIds>(
                        buffer + i * structSize);
            }

            return NvApiStatus.Ok;
        }
        finally
        {
            Marshal.FreeHGlobal(buffer);
        }
    }
}