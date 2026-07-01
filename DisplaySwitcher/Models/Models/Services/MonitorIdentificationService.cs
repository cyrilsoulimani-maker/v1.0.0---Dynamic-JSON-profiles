using DisplaySwitcher.Models;

namespace DisplaySwitcher.Services;

public class MonitorIdentificationService
{
    public MonitorIdentity GetIdentity(string windowsDisplayName)
    {
        // TODO : Implémentation WMI
        return MonitorIdentity.Unknown;
    }
}