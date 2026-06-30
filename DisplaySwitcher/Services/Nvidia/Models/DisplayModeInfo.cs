namespace DisplaySwitcher.Models;

public class DisplayModeInfo
{
    public int Width { get; init; }

    public int Height { get; init; }

    public int Frequency { get; init; }

    public override string ToString()
    {
        return $"{Width} × {Height} @ {Frequency} Hz";
    }
}