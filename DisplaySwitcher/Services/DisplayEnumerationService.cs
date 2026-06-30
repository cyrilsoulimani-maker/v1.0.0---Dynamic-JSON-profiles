using DisplaySwitcher.Models;
using System.Windows.Forms;

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
}