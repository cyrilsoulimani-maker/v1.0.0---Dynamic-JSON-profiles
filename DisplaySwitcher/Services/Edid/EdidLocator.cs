using DisplaySwitcher.Models;
using DisplaySwitcher.Services.Edid.Models;
using Microsoft.Win32;

namespace DisplaySwitcher.Services.Edid;

public class EdidLocator
{
    private const string DisplayRegistry =
        @"SYSTEM\CurrentControlSet\Enum\DISPLAY";

    public IReadOnlyList<EdidMonitor> GetConnectedMonitors()
    {
        List<EdidMonitor> result = new();

        MonitorIdentificationService monitorService = new();

        IReadOnlyList<MonitorIdentity> monitors =
            monitorService.GetAllIdentities();

        using RegistryKey? displayRoot =
            Registry.LocalMachine.OpenSubKey(DisplayRegistry);

        if (displayRoot == null)
            return result;

        foreach (MonitorIdentity monitor in monitors)
        {
            if (string.IsNullOrWhiteSpace(monitor.MonitorUid))
                continue;

            foreach (string manufacturer in displayRoot.GetSubKeyNames())
            {
                using RegistryKey? manufacturerKey =
                    displayRoot.OpenSubKey(manufacturer);

                if (manufacturerKey == null)
                    continue;

                foreach (string instance in manufacturerKey.GetSubKeyNames())
                {
                    if (!instance.Contains(
                            monitor.MonitorUid,
                            StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }

                    using RegistryKey? instanceKey =
                        manufacturerKey.OpenSubKey(instance);

                    using RegistryKey? deviceParameters =
                        instanceKey?.OpenSubKey("Device Parameters");

                    byte[]? edid =
                        deviceParameters?.GetValue("EDID") as byte[];

                    if (edid == null)
                        continue;

                    result.Add(new EdidMonitor
                    {
                        FriendlyName = monitor.FriendlyName,
                        MonitorUid = monitor.MonitorUid,
                        RegistryPath =
                            $@"{DisplayRegistry}\{manufacturer}\{instance}",
                        DisplayDeviceName = monitor.InstanceName,
                        Edid = edid
                    });
                }
            }
        }

        return result;
    }
}