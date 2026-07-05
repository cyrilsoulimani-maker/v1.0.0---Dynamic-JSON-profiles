namespace DisplaySwitcher.Services.Edid.Models;

public class EdidOverrideBuildResult
{
    public byte[] OriginalEdid { get; init; } = [];

    public byte[] ModifiedEdid { get; init; } = [];

    public bool OriginalChecksumValid { get; init; }

    public bool ModifiedChecksumValid { get; init; }

    public int ChangedByteCount { get; init; }

    public IReadOnlyList<EdidByteDifference> Differences { get; init; } = [];
}