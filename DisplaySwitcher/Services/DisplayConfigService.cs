using DisplaySwitcher.Models;
using DisplaySwitcher.Native;

namespace DisplaySwitcher.Services;

public class DisplayConfigService
{
    private const uint QdcOnlyActivePaths = 0x00000002;
    public IReadOnlyList<DisplayConfigMonitor> GetCurrentConfiguration()
    {
        var bufferSizes = GetBufferSizes();

        if (bufferSizes.Result != 0)
            return Array.Empty<DisplayConfigMonitor>();

        DISPLAYCONFIG_PATH_INFO[] paths =
            new DISPLAYCONFIG_PATH_INFO[bufferSizes.PathCount];

        DISPLAYCONFIG_MODE_INFO[] modes =
            new DISPLAYCONFIG_MODE_INFO[bufferSizes.ModeCount];

        uint pathCount = bufferSizes.PathCount;
        uint modeCount = bufferSizes.ModeCount;

        int queryResult = NativeMethods.QueryDisplayConfig(
            QdcOnlyActivePaths,
            ref pathCount,
            paths,
            ref modeCount,
            modes,
            IntPtr.Zero);

        if (queryResult != 0)
            return Array.Empty<DisplayConfigMonitor>();

        List<DisplayConfigMonitor> monitors = new();

        for (int i = 0; i < pathCount; i++)
        {
            DISPLAYCONFIG_PATH_INFO path = paths[i];

            string deviceName = GetSourceDeviceName(path);
            var target = GetTargetDeviceName(path);

            monitors.Add(new DisplayConfigMonitor
            {
                DeviceName = deviceName,
                FriendlyName = target.FriendlyName,
                DevicePath = target.DevicePath,
                MonitorUid = ExtractMonitorUid(target.DevicePath)
            });
        }

        return monitors;
    }
    private static (int Result, uint PathCount, uint ModeCount) GetBufferSizes()
    {

        int result = NativeMethods.GetDisplayConfigBufferSizes(
    QdcOnlyActivePaths,
    out uint pathCount,
    out uint modeCount);

        return (result, pathCount, modeCount);
    }

    private static string GetSourceDeviceName(DISPLAYCONFIG_PATH_INFO path)
    {
        DISPLAYCONFIG_SOURCE_DEVICE_NAME sourceName = new()
        {
            header = new DISPLAYCONFIG_DEVICE_INFO_HEADER
            {
                type = DISPLAYCONFIG_DEVICE_INFO_TYPE.GetSourceName,
                size = (uint)System.Runtime.InteropServices.Marshal.SizeOf<DISPLAYCONFIG_SOURCE_DEVICE_NAME>(),
                adapterId = path.sourceInfo.adapterId,
                id = path.sourceInfo.id
            }
        };

        int result = NativeMethods.DisplayConfigGetDeviceInfo(ref sourceName);

        return result == 0
            ? sourceName.viewGdiDeviceName
            : string.Empty;
    }

    private static (string FriendlyName, string DevicePath) GetTargetDeviceName(DISPLAYCONFIG_PATH_INFO path)
    {
        DISPLAYCONFIG_TARGET_DEVICE_NAME targetName = new()
        {
            header = new DISPLAYCONFIG_DEVICE_INFO_HEADER
            {
                type = DISPLAYCONFIG_DEVICE_INFO_TYPE.GetTargetName,
                size = (uint)System.Runtime.InteropServices.Marshal.SizeOf<DISPLAYCONFIG_TARGET_DEVICE_NAME>(),
                adapterId = path.targetInfo.adapterId,
                id = path.targetInfo.id
            }
        };

        int result = NativeMethods.DisplayConfigGetDeviceInfo(ref targetName);

        if (result != 0)
            return (string.Empty, string.Empty);

        return (
            targetName.monitorFriendlyDeviceName,
            targetName.monitorDevicePath
        );
    }

    private static string ExtractMonitorUid(string devicePath)
    {
        if (string.IsNullOrWhiteSpace(devicePath))
            return string.Empty;

        int uidIndex = devicePath.IndexOf("UID", StringComparison.OrdinalIgnoreCase);

        if (uidIndex < 0)
            return string.Empty;

        int endIndex = devicePath.IndexOf('#', uidIndex);

        if (endIndex < 0)
            endIndex = devicePath.IndexOf('_', uidIndex);

        if (endIndex < 0)
            return devicePath[uidIndex..];

        return devicePath[uidIndex..endIndex];
    }
}
