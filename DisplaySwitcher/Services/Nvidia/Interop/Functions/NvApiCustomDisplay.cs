using DisplaySwitcher.Services.Nvidia.Interop.Structures;
using System;
using System.Runtime.InteropServices;

namespace DisplaySwitcher.Services.Nvidia.Interop;

public static partial class NvApi
{
    private const uint NvApiDispGetTimingId = 0x175167E9;
    private const uint NvApiDispTryCustomDisplayId = 0x1F7DB630;
    private const uint NvApiDispSaveCustomDisplayId = 0x49882876;
    private const uint NvApiDispRevertCustomDisplayTrialId = 0xCBBD40F0;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate NvApiStatus NvApiDispGetTimingDelegate(
        uint displayId,
        ref NvTimingInput timingInput,
        ref NvTiming timing);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate NvApiStatus NvApiDispTryCustomDisplayDelegate(
        IntPtr displayIds,
        uint count,
        IntPtr customDisplays);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate NvApiStatus NvApiDispSaveCustomDisplayDelegate(
        IntPtr displayIds,
        uint count,
        uint isThisOutputIdOnly,
        uint isThisMonitorIdOnly);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate NvApiStatus NvApiDispRevertCustomDisplayTrialDelegate(
        IntPtr displayIds,
        uint count);

    public static NvApiStatus GetTiming(
        uint displayId,
        uint width,
        uint height,
        float refreshRate,
           uint timingType,
        out NvTiming timing)
    {
        timing = NvTiming.Create();

        IntPtr functionPointer = QueryInterface(NvApiDispGetTimingId);

        if (functionPointer == IntPtr.Zero)
            return NvApiStatus.NotAvailable;

        var getTiming =
            Marshal.GetDelegateForFunctionPointer<NvApiDispGetTimingDelegate>(
                functionPointer);

        NvTimingInput input =
            NvTimingInput.Create(width, height, refreshRate, timingType);

        return getTiming(displayId, ref input, ref timing);
    }

    public static NvApiStatus TryCustomDisplay(
        uint displayId,
        NvCustomDisplay customDisplay)
    {
        IntPtr functionPointer = QueryInterface(NvApiDispTryCustomDisplayId);

        if (functionPointer == IntPtr.Zero)
            return NvApiStatus.NotAvailable;

        var tryCustomDisplay =
            Marshal.GetDelegateForFunctionPointer<NvApiDispTryCustomDisplayDelegate>(
                functionPointer);

        IntPtr displayIdsBuffer = Marshal.AllocHGlobal(sizeof(uint));
        IntPtr customDisplayBuffer = Marshal.AllocHGlobal(Marshal.SizeOf<NvCustomDisplay>());

        try
        {
            Marshal.WriteInt32(displayIdsBuffer, unchecked((int)displayId));

            Marshal.StructureToPtr(
                customDisplay,
                customDisplayBuffer,
                false);

            return tryCustomDisplay(displayIdsBuffer, 1, customDisplayBuffer);
        }
        finally
        {
            Marshal.FreeHGlobal(displayIdsBuffer);
            Marshal.FreeHGlobal(customDisplayBuffer);
        }
    }

    public static NvApiStatus SaveCustomDisplay(
        uint displayId,
        bool outputIdOnly,
        bool monitorIdOnly)
    {
        IntPtr functionPointer = QueryInterface(NvApiDispSaveCustomDisplayId);

        if (functionPointer == IntPtr.Zero)
            return NvApiStatus.NotAvailable;

        var saveCustomDisplay =
            Marshal.GetDelegateForFunctionPointer<NvApiDispSaveCustomDisplayDelegate>(
                functionPointer);

        IntPtr displayIdsBuffer = Marshal.AllocHGlobal(sizeof(uint));

        try
        {
            Marshal.WriteInt32(displayIdsBuffer, unchecked((int)displayId));

            return saveCustomDisplay(
                displayIdsBuffer,
                1,
                outputIdOnly ? 1u : 0u,
                monitorIdOnly ? 1u : 0u);
        }
        finally
        {
            Marshal.FreeHGlobal(displayIdsBuffer);
        }
    }

    public static NvApiStatus RevertCustomDisplayTrial(uint displayId)
    {
        IntPtr functionPointer = QueryInterface(NvApiDispRevertCustomDisplayTrialId);

        if (functionPointer == IntPtr.Zero)
            return NvApiStatus.NotAvailable;

        var revertCustomDisplayTrial =
            Marshal.GetDelegateForFunctionPointer<NvApiDispRevertCustomDisplayTrialDelegate>(
                functionPointer);

        IntPtr displayIdsBuffer = Marshal.AllocHGlobal(sizeof(uint));

        try
        {
            Marshal.WriteInt32(displayIdsBuffer, unchecked((int)displayId));

            return revertCustomDisplayTrial(displayIdsBuffer, 1);
        }
        finally
        {
            Marshal.FreeHGlobal(displayIdsBuffer);
        }
    }
}