using DisplaySwitcher.Models;
using DisplaySwitcher.Services.Edid.Models;
using Microsoft.Win32;

namespace DisplaySwitcher.Services.Edid;

public class EdidLocator
{
    private const string DisplayRegistryPath =
        @"SYSTEM\CurrentControlSet\Enum\DISPLAY";

    public LocatedEdid? LocateByDisplayConfigMonitor(DisplayConfigMonitor monitor)
    {
        if (string.IsNullOrWhiteSpace(monitor.MonitorUid))
            return null;

        string manufacturerFromDevicePath =
            ExtractManufacturerFromDevicePath(monitor.DevicePath);

        return GetLocatedEdids()
            .FirstOrDefault(edid =>
                string.Equals(
                    edid.MonitorUid,
                    monitor.MonitorUid,
                    StringComparison.OrdinalIgnoreCase) &&
                string.Equals(
                    edid.RegistryManufacturer,
                    manufacturerFromDevicePath,
                    StringComparison.OrdinalIgnoreCase));
    }

    public IReadOnlyList<LocatedEdid> GetLocatedEdids()
    {
        List<LocatedEdid> result = new();
        EdidParser parser = new();

        using RegistryKey? displayRoot =
            Registry.LocalMachine.OpenSubKey(DisplayRegistryPath);

        if (displayRoot == null)
            return result;

        foreach (string manufacturerKeyName in displayRoot.GetSubKeyNames())
        {
            using RegistryKey? manufacturerKey =
                displayRoot.OpenSubKey(manufacturerKeyName);

            if (manufacturerKey == null)
                continue;

            foreach (string instanceName in manufacturerKey.GetSubKeyNames())
            {
                using RegistryKey? instanceKey =
                    manufacturerKey.OpenSubKey(instanceName);

                using RegistryKey? deviceParametersKey =
                    instanceKey?.OpenSubKey("Device Parameters");

                byte[]? edid =
                    deviceParametersKey?.GetValue("EDID") as byte[];

                if (edid == null || edid.Length < 128)
                    continue;

                ParsedEdid parsed = parser.Parse(edid);

                result.Add(new LocatedEdid
                {
                    RegistryManufacturer = manufacturerKeyName,
                    RegistryInstance = instanceName,
                    RegistryPath =
                        $@"{DisplayRegistryPath}\{manufacturerKeyName}\{instanceName}",
                    MonitorUid = ExtractMonitorUid(instanceName),
                    Edid = edid,
                    Parsed = parsed
                });
            }
        }

        return result
            .OrderBy(edid => edid.DisplayName)
            .ThenBy(edid => edid.MonitorUid)
            .ToList();
    }

    private static string ExtractMonitorUid(string instanceName)
    {
        if (string.IsNullOrWhiteSpace(instanceName))
            return string.Empty;

        int uidIndex =
            instanceName.IndexOf("UID", StringComparison.OrdinalIgnoreCase);

        if (uidIndex < 0)
            return string.Empty;

        return instanceName[uidIndex..];
    }

    private static string ExtractManufacturerFromDevicePath(string devicePath)
    {
        if (string.IsNullOrWhiteSpace(devicePath))
            return string.Empty;

        string marker = @"DISPLAY#";

        int markerIndex =
            devicePath.IndexOf(marker, StringComparison.OrdinalIgnoreCase);

        if (markerIndex < 0)
            return string.Empty;

        int startIndex = markerIndex + marker.Length;

        int endIndex =
            devicePath.IndexOf('#', startIndex);

        if (endIndex < 0)
            return string.Empty;

        return devicePath[startIndex..endIndex];
    }
}