using System;
using System.Runtime.InteropServices;

namespace DisplaySwitcher.Services.Nvidia.Interop;

public static partial class NvApi
{
    private const uint NvApiInitializeId = 0x0150E828;

    private delegate NvApiStatus NvApiInitializeDelegate();

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

        return initialize();
    }
}