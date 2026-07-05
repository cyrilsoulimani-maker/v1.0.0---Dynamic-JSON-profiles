using DisplaySwitcher.Models;
using DisplaySwitcher.Services.Edid.Models;
using System;
using System.IO;
using System.Linq;
using System.Text;

namespace DisplaySwitcher.Services.Edid;

public class EdidPreviewService
{
    public string ExportPreviewForActiveDisplaysToDesktop(
        int width,
        int height,
        int refreshRate)
    {
        string previewDirectory = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
            "DisplaySwitcher_EDID_Previews");

        Directory.CreateDirectory(previewDirectory);

        DisplayConfigService displayConfigService = new();
        EdidLocator locator = new();
        EdidOverrideBuilder builder = new();
        EdidRegistryWriter registryWriter = new();

        IReadOnlyList<DisplayConfigMonitor> monitors =
            displayConfigService.GetCurrentConfiguration();

        StringBuilder report = new();

        report.AppendLine("========== EDID PREVIEW ==========");
        report.AppendLine($"Date        : {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
        report.AppendLine($"Resolution  : {width} × {height} @ {refreshRate} Hz");
        report.AppendLine();

        foreach (DisplayConfigMonitor monitor in monitors)
        {
            LocatedEdid? locatedEdid =
                locator.LocateByDisplayConfigMonitor(monitor);

            report.AppendLine($"Windows device : {monitor.DeviceName}");
            report.AppendLine($"Friendly name  : {monitor.FriendlyName}");
            report.AppendLine($"Monitor UID    : {monitor.MonitorUid}");

            if (locatedEdid == null)
            {
                report.AppendLine("Preview       : SKIPPED - EDID not found");
                report.AppendLine();
                continue;
            }

            EdidOverrideBuildResult result =
                builder.BuildWithBaseDetailedTiming(
                    locatedEdid.Edid,
                    width,
                    height,
                    refreshRate,
                    horizontalImageSizeMm: 600,
                    verticalImageSizeMm: 320);

            string safeName = MakeSafeFileName(
                $"{monitor.DeviceName}_{monitor.FriendlyName}_{locatedEdid.RegistryManufacturer}_{locatedEdid.MonitorUid}");

            string originalPath = Path.Combine(
                previewDirectory,
                $"{safeName}_original.bin");

            string modifiedPath = Path.Combine(
                previewDirectory,
                $"{safeName}_{width}x{height}_{refreshRate}hz_preview.bin");

            File.WriteAllBytes(originalPath, result.OriginalEdid);
            File.WriteAllBytes(modifiedPath, result.ModifiedEdid);

            bool writeOk =
                registryWriter.TryWriteOverride(
                    locatedEdid,
                    result.ModifiedEdid,
                    out string writeMessage);

            report.AppendLine("Preview       : OK");
            report.AppendLine($"EDID name     : {locatedEdid.DisplayName}");
            report.AppendLine($"Registry path : {locatedEdid.RegistryPath}");
            report.AppendLine($"Original OK   : {result.OriginalChecksumValid}");
            report.AppendLine($"Modified OK   : {result.ModifiedChecksumValid}");
            report.AppendLine($"Changed bytes : {result.ChangedByteCount}");
            report.AppendLine($"Original file : {originalPath}");
            report.AppendLine($"Preview file  : {modifiedPath}");
            report.AppendLine();
            report.AppendLine("Registry write test:");
            report.AppendLine($"Block count   : {result.ModifiedEdid.Length / 128}");
            report.AppendLine($"Write OK      : {writeOk}");
            report.AppendLine($"Message       : {writeMessage}");

            report.AppendLine();
            report.AppendLine("Byte diff:");
            foreach (EdidByteDifference diff in result.Differences)
            {
                report.AppendLine(
                    $"Offset 0x{diff.Offset:X2} : {diff.OriginalValue:X2} -> {diff.ModifiedValue:X2}");
            }

            report.AppendLine();
        }

        string reportPath = Path.Combine(
            previewDirectory,
            "preview_report.txt");

        File.WriteAllText(reportPath, report.ToString());

        return reportPath;
    }

    private static string MakeSafeFileName(string value)
    {
        foreach (char invalidChar in Path.GetInvalidFileNameChars())
        {
            value = value.Replace(invalidChar, '_');
        }

        return value.Replace("\\", "_").Replace(".", "_");
    }
}