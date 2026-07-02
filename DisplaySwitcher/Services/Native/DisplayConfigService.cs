using DisplaySwitcher.Models;

namespace DisplaySwitcher.Services.Native;

public class DisplayConfigService
{
    public IReadOnlyList<DisplayConfigMonitor> GetConnectedMonitors()
    {
        return Array.Empty<DisplayConfigMonitor>();
    }
}