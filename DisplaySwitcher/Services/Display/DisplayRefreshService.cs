using System;
using System.Runtime.InteropServices;

namespace DisplaySwitcher.Services.Display;

public class DisplayRefreshService
{
    private const int HWND_BROADCAST = 0xFFFF;
    private const int WM_DISPLAYCHANGE = 0x007E;
    private const int WM_SETTINGCHANGE = 0x001A;

    [DllImport("user32.dll", SetLastError = true)]
    private static extern IntPtr SendMessageTimeout(
        IntPtr hWnd,
        uint msg,
        IntPtr wParam,
        IntPtr lParam,
        uint fuFlags,
        uint uTimeout,
        out IntPtr lpdwResult);

    public bool NotifyDisplaySettingsChanged()
    {
        bool displayChangeOk =
            SendBroadcastMessage(WM_DISPLAYCHANGE);

        bool settingChangeOk =
            SendBroadcastMessage(WM_SETTINGCHANGE);

        return displayChangeOk && settingChangeOk;
    }

    private static bool SendBroadcastMessage(uint message)
    {
        IntPtr result;

        IntPtr sendResult =
            SendMessageTimeout(
                new IntPtr(HWND_BROADCAST),
                message,
                IntPtr.Zero,
                IntPtr.Zero,
                0x0002,
                5000,
                out result);

        return sendResult != IntPtr.Zero;
    }
}