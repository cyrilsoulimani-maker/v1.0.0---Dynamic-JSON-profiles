using DisplaySwitcher.Models;
using DisplaySwitcher.Services.Nvidia;
using DisplaySwitcher.Services.Nvidia.Models;
using System.Management;
using WinForms = System.Windows.Forms;

namespace DisplaySwitcher.Services;

public class DisplayEnumerationService
{
    public List<DisplayDeviceInfo> GetDisplays()
    {
        MonitorIdentificationService monitorIdentificationService = new();
        NvidiaDisplayService nvidiaDisplayService = new();

        List<NvidiaDisplay> nvidiaDisplays =
            nvidiaDisplayService
                .GetConnectedDisplays()
                .Where(display => display.IsConnected && display.IsActive)
                .ToList();

        List<DisplayDeviceInfo> displays = new();

        WinForms.Screen[] screens = WinForms.Screen.AllScreens;

        for (int i = 0; i < screens.Length; i++)
        {
            WinForms.Screen screen = screens[i];

            var currentMode = DisplayService.GetCurrentMode(screen.DeviceName);

            MonitorIdentity identity =
                monitorIdentificationService.GetIdentity(screen.DeviceName);

            NvidiaDisplay? nvidiaDisplay =
                i < nvidiaDisplays.Count
                    ? nvidiaDisplays[i]
                    : null;

            displays.Add(new DisplayDeviceInfo
            {
                WindowsName = screen.DeviceName,
                FriendlyName = identity.IsDetected
                    ? identity.FriendlyName
                    : screen.Primary ? "Écran principal" : "Écran secondaire",
                IsPrimary = screen.Primary,
                Width = currentMode.Width,
                Height = currentMode.Height,
                Frequency = currentMode.Frequency,
                NvidiaDisplay = nvidiaDisplay
            });
        }

        return displays;
    }

    public void DumpMonitors()
    {
        using ManagementObjectSearcher searcher =
            new("SELECT * FROM Win32_DesktopMonitor");

        foreach (ManagementObject monitor in searcher.Get())
        {
            string? name = monitor["Name"]?.ToString();

            System.Diagnostics.Debug.WriteLine(name);
        }
    }
}