namespace DisplaySwitcher.Services.Timings;

public class CvtReducedBlankingTimingCalculator : ITimingCalculator
{
    public TimingResult Calculate(
        int width,
        int height,
        int refreshRate,
        int horizontalImageSizeMm,
        int verticalImageSizeMm)
    {
        int horizontalFrontPorch = 48;
        int horizontalSyncWidth = 32;
        int horizontalBackPorch = 80;

        int verticalFrontPorch = 3;
        int verticalSyncWidth = 5;
        int verticalBackPorch = 23;

        int horizontalBlanking =
            horizontalFrontPorch +
            horizontalSyncWidth +
            horizontalBackPorch;

        int verticalBlanking =
            verticalFrontPorch +
            verticalSyncWidth +
            verticalBackPorch;

        int horizontalTotal = width + horizontalBlanking;
        int verticalTotal = height + verticalBlanking;

        int pixelClockKhz =
            (horizontalTotal * verticalTotal * refreshRate) / 1000;

        return new TimingResult
        {
            Width = width,
            Height = height,
            RefreshRate = refreshRate,
            PixelClockKhz = pixelClockKhz,

            HorizontalBlanking = horizontalBlanking,
            VerticalBlanking = verticalBlanking,

            HorizontalFrontPorch = horizontalFrontPorch,
            HorizontalSyncWidth = horizontalSyncWidth,

            VerticalFrontPorch = verticalFrontPorch,
            VerticalSyncWidth = verticalSyncWidth,

            HorizontalImageSizeMm = horizontalImageSizeMm,
            VerticalImageSizeMm = verticalImageSizeMm
        };
    }
}