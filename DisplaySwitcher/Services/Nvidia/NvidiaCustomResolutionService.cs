using DisplaySwitcher.Services.Nvidia.Interop;
using DisplaySwitcher.Services.Nvidia.Interop.Structures;

namespace DisplaySwitcher.Services.Nvidia;

public class NvidiaCustomResolutionService
{
    public string LastStep { get; private set; } = string.Empty;

    private static readonly (uint Type, string Name)[] TimingTypes =
    {
        (6u, "CVT_RB"),
        (5u, "CVT"),
        (4u, "DMT_RB"),
        (3u, "DMT"),
        (1u, "AUTO")
    };

    public bool IsAvailable()
    {
        return NvApi.IsAvailable();
    }

    public NvApiStatus CreateCustomResolution(
        uint displayId,
        uint width,
        uint height,
        uint refreshRate)
    {
        LastStep = "Initialize";

        NvApiStatus initializeStatus = NvApi.Initialize();

        if (initializeStatus != NvApiStatus.Ok)
            return initializeStatus;

        foreach ((uint timingType, string timingName) in TimingTypes)
        {
            LastStep = $"GetTiming ({timingName})";

            NvApiStatus timingStatus =
                NvApi.GetTiming(
                    displayId,
                    width,
                    height,
                    refreshRate,
                    timingType,
                    out NvTiming timing);

            if (timingStatus != NvApiStatus.Ok)
                continue;

            NvApiStatus result =
                TryAndSave(displayId, width, height, timing, timingName);

            if (result == NvApiStatus.Ok)
                return NvApiStatus.Ok;
        }

        LastStep = "ManualTiming";

        NvTiming manualTiming =
            NvidiaTimingCalculator.CreateReducedBlankingTiming(
                width,
                height,
                refreshRate);

        return TryAndSave(
            displayId,
            width,
            height,
            manualTiming,
            "Manual RB");
    }

    private NvApiStatus TryAndSave(
        uint displayId,
        uint width,
        uint height,
        NvTiming timing,
        string timingName)
    {
        NvCustomDisplay customDisplay =
            NvCustomDisplay.Create(
                width,
                height,
                timing);

        LastStep = $"TryCustomDisplay ({timingName})";

        NvApiStatus tryStatus =
            NvApi.TryCustomDisplay(
                displayId,
                customDisplay);

        if (tryStatus != NvApiStatus.Ok)
        {
            NvApi.RevertCustomDisplayTrial(displayId);
            return tryStatus;
        }

        LastStep = $"SaveCustomDisplay ({timingName})";

        NvApiStatus saveStatus =
            NvApi.SaveCustomDisplay(
                displayId,
                outputIdOnly: false,
                monitorIdOnly: false);

        if (saveStatus != NvApiStatus.Ok)
        {
            NvApi.RevertCustomDisplayTrial(displayId);
            return saveStatus;
        }

        LastStep = $"Success ({timingName})";

        return NvApiStatus.Ok;
    }
}