namespace DisplaySwitcher.Services.Edid.Models;

public class EdidByteDifference
{
    public int Offset { get; init; }

    public byte OriginalValue { get; init; }

    public byte ModifiedValue { get; init; }
}