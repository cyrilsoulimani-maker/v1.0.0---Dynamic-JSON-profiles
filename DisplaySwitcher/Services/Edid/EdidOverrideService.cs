using DisplaySwitcher.Services.Edid.Models;
using Microsoft.Win32;
using System;
using System.IO;
using System.Text;

namespace DisplaySwitcher.Services.Edid;

public class EdidOverrideService
{
    private const string DisplayRegistryPath =
        @"SYSTEM\CurrentControlSet\Enum\DISPLAY";

    public string DumpDetectedEdidsToDesktop()
    {
        string outputPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
            "DisplaySwitcher_EDID_Dump.txt");

        StringBuilder builder = new();
        EdidParser parser = new();

        using RegistryKey? displayRoot =
            Registry.LocalMachine.OpenSubKey(DisplayRegistryPath);

        if (displayRoot == null)
        {
            builder.AppendLine("DISPLAY registry path not found.");
            File.WriteAllText(outputPath, builder.ToString());
            return outputPath;
        }

        foreach (string manufacturerKeyName in displayRoot.GetSubKeyNames())
        {
            using RegistryKey? manufacturerKey =
                displayRoot.OpenSubKey(manufacturerKeyName);

            if (manufacturerKey == null)
                continue;

            foreach (string monitorInstanceName in manufacturerKey.GetSubKeyNames())
            {
                using RegistryKey? monitorKey =
                    manufacturerKey.OpenSubKey(monitorInstanceName);

                using RegistryKey? deviceParametersKey =
                    monitorKey?.OpenSubKey("Device Parameters");

                byte[]? edid =
                    deviceParametersKey?.GetValue("EDID") as byte[];

                builder.AppendLine("========== EDID ==========");
                builder.AppendLine($"Registry Manufacturer : {manufacturerKeyName}");
                builder.AppendLine($"Registry Instance     : {monitorInstanceName}");
                builder.AppendLine($"Has EDID              : {(edid != null ? "YES" : "NO")}");
                builder.AppendLine($"EDID bytes            : {edid?.Length ?? 0}");

                if (edid != null)
                {
                    ParsedEdid parsed = parser.Parse(edid);

                    builder.AppendLine($"Header valid          : {parsed.IsValidHeader}");
                    builder.AppendLine($"Checksum valid        : {parsed.IsChecksumValid}");
                    builder.AppendLine($"Block count           : {parsed.BlockCount}");
                    builder.AppendLine($"Manufacturer ID       : {parsed.ManufacturerId}");
                    builder.AppendLine($"Product code          : {parsed.ProductCode}");
                    builder.AppendLine($"Serial number         : {parsed.SerialNumber}");
                    builder.AppendLine($"Monitor name          : {parsed.MonitorName}");
                    builder.AppendLine($"EDID hex              : {Convert.ToHexString(edid)}");
                }

                builder.AppendLine();
            }
        }

        File.WriteAllText(outputPath, builder.ToString());
        return outputPath;
    }
}