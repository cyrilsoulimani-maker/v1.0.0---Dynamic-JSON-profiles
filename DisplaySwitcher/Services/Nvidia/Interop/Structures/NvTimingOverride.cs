namespace DisplaySwitcher.Services.Nvidia.Interop.Structures;

public enum NvTimingOverride : uint
{
    Current = 0,
    Auto = 1,
    Edid = 2,
    Dmt = 3,
    DmtReducedBlanking = 4,
    Cvt = 5,
    CvtReducedBlanking = 6,
    Gtf = 7,
    Eia861 = 8,
    AnalogTv = 9,
    Custom = 10
}