using DisplaySwitcher.Services.Edid.Models;
using Microsoft.Win32;

namespace DisplaySwitcher.Services.Edid;

public class EdidLocator
{
    private const string DisplayRegistryPath =
        @"SYSTEM\CurrentControlSet\Enum\DISPLAY";

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
}