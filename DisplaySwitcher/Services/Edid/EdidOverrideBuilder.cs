using DisplaySwitcher.Services.Edid.Models;
using DisplaySwitcher.Services.Timings;

namespace DisplaySwitcher.Services.Edid;

public class EdidOverrideBuilder
{
    public EdidOverrideBuildResult BuildWithBaseDetailedTiming(
        byte[] originalEdid,
        int width,
        int height,
        int refreshRate,
        int horizontalImageSizeMm,
        int verticalImageSizeMm)
    {
        CvtReducedBlankingTimingCalculator calculator = new();

        TimingResult timing =
            calculator.Calculate(
                width,
                height,
                refreshRate,
                horizontalImageSizeMm,
                verticalImageSizeMm);

        EdidDetailedTimingDescriptor descriptor =
            timing.ToDetailedTimingDescriptor();

        EdidEditor editor = new(originalEdid);

        bool originalChecksumValid =
            editor.IsBaseBlockChecksumValid();

        editor.ReplaceBaseDetailedTiming(
            index: 0,
            descriptor);

        byte[] modifiedEdid =
            editor.Build();

        bool modifiedChecksumValid =
            EdidChecksum.IsBlockChecksumValid(
                modifiedEdid.Take(128).ToArray());

        var differences = GetDifferences(originalEdid, modifiedEdid);

        return new EdidOverrideBuildResult
        {
            OriginalEdid = originalEdid.ToArray(),
            ModifiedEdid = modifiedEdid,
            OriginalChecksumValid = originalChecksumValid,
            ModifiedChecksumValid = modifiedChecksumValid,
            ChangedByteCount = differences.Count,
            Differences = differences
        };
    }

    private static List<EdidByteDifference> GetDifferences(
        byte[] original,
        byte[] modified)
    {
        List<EdidByteDifference> result = new();

        int length = Math.Min(original.Length, modified.Length);

        for (int i = 0; i < length; i++)
        {
            if (original[i] == modified[i])
                continue;

            result.Add(new EdidByteDifference
            {
                Offset = i,
                OriginalValue = original[i],
                ModifiedValue = modified[i]
            });
        }

        return result;
    }
}