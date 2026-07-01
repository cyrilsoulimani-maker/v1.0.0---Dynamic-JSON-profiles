using DisplaySwitcher.Models;
using System.Windows.Forms;
using System.Management;

namespace DisplaySwitcher.Services;


public class DisplayEnumerationService
{
    public List<DisplayDeviceInfo> GetDisplays()
    {
        List<DisplayDeviceInfo> displays = new();

        foreach (Screen screen in Screen.AllScreens)
        {
            var currentMode = DisplayService.GetCurrentMode(screen.DeviceName);

            displays.Add(new DisplayDeviceInfo
            {
                WindowsName = screen.DeviceName,
                IsPrimary = screen.Primary,
                Width = currentMode.Width,
                Height = currentMode.Height,
                Frequency = currentMode.Frequency
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

