using DisplaySwitcher.Models;
using DisplaySwitcher.Native;
using System.Linq;

namespace DisplaySwitcher.Services;

public class DisplayConfigService
{
    private const uint QdcOnlyActivePaths = 0x00000002;
    public IReadOnlyList<DisplayConfigMonitor> GetCurrentConfiguration()
    {
        DisplayConfigSnapshot? snapshot = GetSnapshot();

        if (snapshot == null)
            return Array.Empty<DisplayConfigMonitor>();

        List<DisplayConfigMonitor> monitors = new();

        for (int i = 0; i < snapshot.PathCount; i++)
        {
            DISPLAYCONFIG_PATH_INFO path = snapshot.Paths[i];

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
    public CurrentDisplayState? GetCurrentDisplayState()
    {
        DisplayConfigSnapshot? snapshot = GetSnapshot();

        if (snapshot == null)
            return null;

        DISPLAYCONFIG_PATH_INFO? primaryPath = null;

        for (int i = 0; i < snapshot.PathCount; i++)
        {
            DISPLAYCONFIG_PATH_INFO path = snapshot.Paths[i];

            string deviceName = GetSourceDeviceName(path);

            if (deviceName == @"\\.\DISPLAY1")
            {
                primaryPath = path;
                break;
            }
        }

        if (primaryPath == null)
            return null;

        var target = GetTargetDeviceName(primaryPath.Value);

        DISPLAYCONFIG_SOURCE_MODE? sourceMode =
    FindSourceMode(snapshot, primaryPath.Value);

        DISPLAYCONFIG_TARGET_MODE? targetMode =
    FindTargetMode(snapshot, primaryPath.Value);

        double refreshRate = GetRefreshRate(targetMode);

        return new CurrentDisplayState
        {
            FriendlyName = target.FriendlyName,
            Width = sourceMode?.width ?? 0,
            Height = sourceMode?.height ?? 0,
            RefreshRate = refreshRate,
        };
    }
    private static DisplayConfigSnapshot? GetSnapshot()
    {
        var bufferSizes = GetBufferSizes();

        if (bufferSizes.Result != 0)
            return null;

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
            return null;

        return new DisplayConfigSnapshot
        {
            Paths = paths,
            Modes = modes,
            PathCount = pathCount,
            ModeCount = modeCount
        };
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
    private static DISPLAYCONFIG_SOURCE_MODE? FindSourceMode(
    DisplayConfigSnapshot snapshot,
    DISPLAYCONFIG_PATH_INFO path)
    {
        for (int i = 0; i < snapshot.ModeCount; i++)
        {
            DISPLAYCONFIG_MODE_INFO mode = snapshot.Modes[i];

            if (mode.infoType == DISPLAYCONFIG_MODE_INFO_TYPE.Source &&
                mode.id == path.sourceInfo.id &&
                mode.adapterId.LowPart == path.sourceInfo.adapterId.LowPart &&
                mode.adapterId.HighPart == path.sourceInfo.adapterId.HighPart)
            {
                return mode.modeInfo.sourceMode;
            }
        }

        return null;
    }
    private static DISPLAYCONFIG_TARGET_MODE? FindTargetMode(
    DisplayConfigSnapshot snapshot,
    DISPLAYCONFIG_PATH_INFO path)
    {
        for (int i = 0; i < snapshot.ModeCount; i++)
        {
            DISPLAYCONFIG_MODE_INFO mode = snapshot.Modes[i];

            if (mode.infoType == DISPLAYCONFIG_MODE_INFO_TYPE.Target &&
                mode.id == path.targetInfo.id &&
                mode.adapterId.LowPart == path.targetInfo.adapterId.LowPart &&
                mode.adapterId.HighPart == path.targetInfo.adapterId.HighPart)
            {
                return mode.modeInfo.targetMode;
            }
        }

        return null;
    }

    private static double GetRefreshRate(DISPLAYCONFIG_TARGET_MODE? targetMode)
    {
        if (targetMode == null)
            return 0;

        DISPLAYCONFIG_RATIONAL refreshRate =
            targetMode.Value.targetVideoSignalInfo.vSyncFreq;

        if (refreshRate.Denominator == 0)
            return 0;

        return (double)refreshRate.Numerator / refreshRate.Denominator;
    }

    private sealed class DisplayConfigSnapshot
    {
        public DISPLAYCONFIG_PATH_INFO[] Paths { get; init; } = [];

        public DISPLAYCONFIG_MODE_INFO[] Modes { get; init; } = [];

        public uint PathCount { get; init; }

        public uint ModeCount { get; init; }
    }

}
