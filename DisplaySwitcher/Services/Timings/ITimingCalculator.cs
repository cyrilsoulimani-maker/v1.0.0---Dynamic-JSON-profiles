namespace DisplaySwitcher.Services.Timings;

public interface ITimingCalculator
{
    TimingResult Calculate(
        int width,
        int height,
        int refreshRate,
        int horizontalImageSizeMm,
        int verticalImageSizeMm);
}