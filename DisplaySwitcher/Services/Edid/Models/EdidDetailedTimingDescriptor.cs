namespace DisplaySwitcher.Services.Edid.Models;

public class EdidDetailedTimingDescriptor
{
    public int Width { get; init; }

    public int Height { get; init; }

    public int RefreshRate { get; init; }

    public int PixelClockKhz { get; init; }

    public int HorizontalBlanking { get; init; }

    public int VerticalBlanking { get; init; }

    public int HorizontalFrontPorch { get; init; }

    public int HorizontalSyncWidth { get; init; }

    public int VerticalFrontPorch { get; init; }

    public int VerticalSyncWidth { get; init; }

    public int HorizontalImageSizeMm { get; init; }

    public int VerticalImageSizeMm { get; init; }

    public byte[] ToBytes()
    {
        byte[] bytes = new byte[18];

        int pixelClock10Khz = PixelClockKhz / 10;

        bytes[0] = (byte)(pixelClock10Khz & 0xFF);
        bytes[1] = (byte)((pixelClock10Khz >> 8) & 0xFF);

        bytes[2] = (byte)(Width & 0xFF);
        bytes[3] = (byte)(HorizontalBlanking & 0xFF);
        bytes[4] = (byte)(((Width >> 8) & 0x0F) << 4 | ((HorizontalBlanking >> 8) & 0x0F));

        bytes[5] = (byte)(Height & 0xFF);
        bytes[6] = (byte)(VerticalBlanking & 0xFF);
        bytes[7] = (byte)(((Height >> 8) & 0x0F) << 4 | ((VerticalBlanking >> 8) & 0x0F));

        bytes[8] = (byte)(HorizontalFrontPorch & 0xFF);
        bytes[9] = (byte)(HorizontalSyncWidth & 0xFF);
        bytes[10] = (byte)(((VerticalFrontPorch & 0x0F) << 4) | (VerticalSyncWidth & 0x0F));

        bytes[11] =
            (byte)(((HorizontalFrontPorch >> 8) & 0x03) << 6 |
                   ((HorizontalSyncWidth >> 8) & 0x03) << 4 |
                   ((VerticalFrontPorch >> 4) & 0x03) << 2 |
                   ((VerticalSyncWidth >> 4) & 0x03));

        bytes[12] = (byte)(HorizontalImageSizeMm & 0xFF);
        bytes[13] = (byte)(VerticalImageSizeMm & 0xFF);
        bytes[14] =
            (byte)(((HorizontalImageSizeMm >> 8) & 0x0F) << 4 |
                   ((VerticalImageSizeMm >> 8) & 0x0F));

        bytes[15] = 0;
        bytes[16] = 0;

        bytes[17] = 0x1A;

        return bytes;
    }
}