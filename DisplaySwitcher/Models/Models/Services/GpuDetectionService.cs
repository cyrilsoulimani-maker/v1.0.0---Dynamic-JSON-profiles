using System.Management;
using DisplaySwitcher.Models;

namespace DisplaySwitcher.Services;

public class GpuDetectionService
{
    public GpuVendor DetectGpu()
    {
        try
        {
            using var searcher = new ManagementObjectSearcher(
                "SELECT Name FROM Win32_VideoController");

            foreach (var obj in searcher.Get())
            {
                var name = obj["Name"]?.ToString();

                if (string.IsNullOrWhiteSpace(name))
                    continue;

                if (name.Contains("NVIDIA", StringComparison.OrdinalIgnoreCase))
                    return GpuVendor.Nvidia;

                if (name.Contains("AMD", StringComparison.OrdinalIgnoreCase) ||
                    name.Contains("Radeon", StringComparison.OrdinalIgnoreCase))
                    return GpuVendor.AMD;

                if (name.Contains("Intel", StringComparison.OrdinalIgnoreCase))
                    return GpuVendor.Intel;
            }
        }
        catch
        {
            return GpuVendor.Unknown;
        }

        return GpuVendor.Unknown;
    }
}