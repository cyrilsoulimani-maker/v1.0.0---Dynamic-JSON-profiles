using System.Runtime.InteropServices;

namespace DisplaySwitcher.Native;

internal static class NativeMethods
{
	[DllImport("user32.dll")]
	internal static extern int GetDisplayConfigBufferSizes(
		uint flags,
		out uint numPathArrayElements,
		out uint numModeInfoArrayElements);

	[DllImport("user32.dll")]
	internal static extern int QueryDisplayConfig(
		uint flags,
		ref uint numPathArrayElements,
		[Out] DISPLAYCONFIG_PATH_INFO[] pathInfoArray,
		ref uint numModeInfoArrayElements,
		[Out] DISPLAYCONFIG_MODE_INFO[] modeInfoArray,
		IntPtr currentTopologyId);

	[DllImport("user32.dll", CharSet = CharSet.Unicode)]
	internal static extern int DisplayConfigGetDeviceInfo(
		ref DISPLAYCONFIG_SOURCE_DEVICE_NAME requestPacket);

	[DllImport("user32.dll", CharSet = CharSet.Unicode)]
	internal static extern int DisplayConfigGetDeviceInfo(
		ref DISPLAYCONFIG_TARGET_DEVICE_NAME requestPacket);
}