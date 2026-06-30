using DisplaySwitcher.Services.Nvidia.Interop;
using DisplaySwitcher.Services.Nvidia.Models;

namespace DisplaySwitcher.Services.Nvidia;

public class NvidiaDisplayService
{
    public List<NvidiaDisplay> GetConnectedDisplays()
    {
        NvApiStatus gpuStatus = NvApi.EnumPhysicalGpus(out var gpuHandles);

        if (gpuStatus != NvApiStatus.Ok)
            return new List<NvidiaDisplay>();

        List<NvidiaDisplay> displays = new();

        foreach (var gpuHandle in gpuHandles)
        {
            NvApiStatus displayStatus = NvApi.GetConnectedDisplayIds(
                gpuHandle,
                out var displayIds);

            if (displayStatus != NvApiStatus.Ok)
                continue;

            foreach (var displayId in displayIds)
            {
                displays.Add(new NvidiaDisplay
                {
                    DisplayId = displayId.DisplayId,
                    ConnectorType = displayId.ConnectorType,
                    IsConnected = NvGpuDisplayFlags.IsConnected(displayId.Flags),
                    IsActive = NvGpuDisplayFlags.IsActive(displayId.Flags)
                });
            }
        }

        return displays;
    }
}