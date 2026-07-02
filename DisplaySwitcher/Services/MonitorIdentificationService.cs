using DisplaySwitcher.Models;
using System.Management;
using System.Text;
using System.IO;

namespace DisplaySwitcher.Services;

public class MonitorIdentificationService
{
    public MonitorIdentity GetIdentity(string windowsDisplayName)
    {
        if (string.IsNullOrWhiteSpace(windowsDisplayName))
            return MonitorIdentity.Unknown;

        try
        {
            using ManagementObjectSearcher searcher =
                new(@"root\wmi", "SELECT * FROM WmiMonitorID");

            string logFile = Path.Combine(
    Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
    "DisplaySwitcher_Monitors.txt");

            File.WriteAllText(logFile, string.Empty);

            foreach (ManagementObject monitor in searcher.Get())
            {
                string instanceName =
                    monitor["InstanceName"]?.ToString() ?? string.Empty;

                string manufacturer =
                    DecodeWmiString(monitor["ManufacturerName"]);

                string productCode =
                    DecodeWmiString(monitor["ProductCodeID"]);

                string friendlyName =
                    DecodeWmiString(monitor["UserFriendlyName"]);

                File.AppendAllText(logFile,
                $"""
========== MONITOR ==========
Instance : {instanceName}
Friendly : {friendlyName}
Maker    : {manufacturer}
Product  : {productCode}
=============================

""");

                if (!string.IsNullOrWhiteSpace(friendlyName))
                {
                   // return new MonitorIdentity
                    //{
                      //  IsDetected = true,
                       // FriendlyName = friendlyName
                    // };
                }
            }
        }
        catch
        {
            return MonitorIdentity.Unknown;
        }

        return MonitorIdentity.Unknown;
    }

    public IReadOnlyList<MonitorIdentity> GetAllIdentities()
    {
        List<MonitorIdentity> identities = new();



        try
        {
            using ManagementObjectSearcher searcher =
                new(@"root\wmi", "SELECT * FROM WmiMonitorID");

            foreach (ManagementObject monitor in searcher.Get())
            {
                string instanceName = monitor["InstanceName"]?.ToString() ?? string.Empty;
                string friendlyName = DecodeWmiString(monitor["UserFriendlyName"]);
                string manufacturer = DecodeWmiString(monitor["ManufacturerName"]);
                string model = DecodeWmiString(monitor["ProductCodeID"]);
                string serialNumber = DecodeWmiString(monitor["SerialNumberID"]);

                if (string.IsNullOrWhiteSpace(friendlyName))
                    continue;

                identities.Add(new MonitorIdentity
                {
                    IsDetected = true,
                    FriendlyName = friendlyName,
                    Manufacturer = manufacturer,
                    Model = model,
                    InstanceName = instanceName,
                    SerialNumber = serialNumber
                });
            }
        }
        catch
        {
            return Array.Empty<MonitorIdentity>();
        }

        return identities;
    }

    public void DumpMonitors(string filePath)
    {
        StringBuilder builder = new();

        foreach (MonitorIdentity identity in GetAllIdentities())
        {
            builder.AppendLine("========== MONITOR ==========");
            builder.AppendLine($"Friendly : {identity.FriendlyName}");
            builder.AppendLine($"Maker    : {identity.Manufacturer}");
            builder.AppendLine($"Model    : {identity.Model}");
            builder.AppendLine($"Serial   : {identity.SerialNumber}");
            builder.AppendLine("=============================");
            builder.AppendLine();
        }

        File.WriteAllText(filePath, builder.ToString());
    }

    private static string DecodeWmiString(object? value)
    {
        if (value is not ushort[] data)
            return string.Empty;

        StringBuilder builder = new();

        foreach (ushort character in data)
        {
            if (character == 0)
                continue;

            builder.Append((char)character);
        }

        return builder.ToString().Trim();
    }
}