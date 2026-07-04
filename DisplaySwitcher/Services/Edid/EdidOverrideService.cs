using DisplaySwitcher.Models;
using DisplaySwitcher.Services.Timings;
using DisplaySwitcher.Services.Edid.Models;
using Microsoft.Win32;
using System;
using System.IO;
using System.Linq;
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

        AppendDisplayConfigToEdid(builder);
        AppendLocatedEdids(builder);

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

                    if (parsed.IsValidHeader && parsed.IsChecksumValid)
                    {
                        AppendEditorDryRun(builder, edid);
                    }
                }

                builder.AppendLine();
            }
        }

        File.WriteAllText(outputPath, builder.ToString());
        return outputPath;
    }

    private static void AppendDisplayConfigToEdid(StringBuilder builder)
    {
        DisplayConfigService displayConfigService = new();
        EdidLocator locator = new();

        IReadOnlyList<DisplayConfigMonitor> monitors =
            displayConfigService.GetCurrentConfiguration();

        builder.AppendLine("========== DISPLAYCONFIG → EDID ==========");
        builder.AppendLine($"Active monitors : {monitors.Count}");
        builder.AppendLine();

        foreach (DisplayConfigMonitor monitor in monitors)
        {
            LocatedEdid? locatedEdid =
                locator.LocateByDisplayConfigMonitor(monitor);

            builder.AppendLine($"Windows device       : {monitor.DeviceName}");
            builder.AppendLine($"Friendly name        : {monitor.FriendlyName}");
            builder.AppendLine($"Monitor UID          : {monitor.MonitorUid}");
            builder.AppendLine($"Device path          : {monitor.DevicePath}");

            if (locatedEdid == null)
            {
                builder.AppendLine("EDID match           : NOT FOUND");
            }
            else
            {
                builder.AppendLine("EDID match           : FOUND");
                builder.AppendLine($"EDID display name    : {locatedEdid.DisplayName}");
                builder.AppendLine($"Registry path        : {locatedEdid.RegistryPath}");
                builder.AppendLine($"EDID bytes           : {locatedEdid.Edid.Length}");
                builder.AppendLine($"Checksum valid       : {locatedEdid.Parsed.IsChecksumValid}");
            }

            builder.AppendLine();
        }
    }

    private static void AppendLocatedEdids(StringBuilder builder)
    {
        EdidLocator locator = new();

        IReadOnlyList<LocatedEdid> locatedEdids =
            locator.GetLocatedEdids();

        builder.AppendLine("========== LOCATED EDIDS ==========");
        builder.AppendLine($"Count : {locatedEdids.Count}");
        builder.AppendLine();

        foreach (LocatedEdid edid in locatedEdids)
        {
            builder.AppendLine($"Display name          : {edid.DisplayName}");
            builder.AppendLine($"Monitor UID           : {edid.MonitorUid}");
            builder.AppendLine($"Registry manufacturer : {edid.RegistryManufacturer}");
            builder.AppendLine($"Registry instance     : {edid.RegistryInstance}");
            builder.AppendLine($"Registry path         : {edid.RegistryPath}");
            builder.AppendLine($"EDID bytes            : {edid.Edid.Length}");
            builder.AppendLine($"Checksum valid        : {edid.Parsed.IsChecksumValid}");
            builder.AppendLine();
        }
    }

    private static void AppendEditorDryRun(StringBuilder builder, byte[] edid)
    {
        CvtReducedBlankingTimingCalculator calculator = new();

        TimingResult timing =
            calculator.Calculate(
                width: 1500,
                height: 790,
                refreshRate: 60,
                horizontalImageSizeMm: 600,
                verticalImageSizeMm: 320);

        EdidDetailedTimingDescriptor descriptor =
            timing.ToDetailedTimingDescriptor();

        EdidEditor editor = new(edid);
        bool beforeChecksum = editor.IsBaseBlockChecksumValid();

        editor.ReplaceBaseDetailedTiming(
            index: 0,
            descriptor);

        byte[] modifiedEdid = editor.Build();
        bool afterChecksum =
            EdidChecksum.IsBlockChecksumValid(modifiedEdid.Take(128).ToArray());

        builder.AppendLine();
        builder.AppendLine("---- EDID EDITOR DRY RUN ----");
        builder.AppendLine($"Timing calculator      : CVT Reduced Blanking");
        builder.AppendLine($"Resolution             : {timing.Width} × {timing.Height} @ {timing.RefreshRate} Hz");
        builder.AppendLine($"Pixel clock            : {timing.PixelClockKhz} kHz");
        builder.AppendLine($"Horizontal blanking    : {timing.HorizontalBlanking}");
        builder.AppendLine($"Vertical blanking      : {timing.VerticalBlanking}");
        builder.AppendLine($"Before checksum valid  : {beforeChecksum}");
        builder.AppendLine($"After checksum valid   : {afterChecksum}");
        builder.AppendLine($"Modified EDID hex      : {Convert.ToHexString(modifiedEdid)}");
    }
}