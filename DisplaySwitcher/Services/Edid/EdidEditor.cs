using DisplaySwitcher.Services.Edid.Models;

namespace DisplaySwitcher.Services.Edid;

public class EdidEditor
{
    private readonly byte[] _edid;

    public EdidEditor(byte[] edid)
    {
        if (edid.Length < 128)
            throw new ArgumentException("L'EDID doit contenir au moins 128 octets.");

        _edid = edid.ToArray();
    }

    public byte[] Build()
    {
        return _edid.ToArray();
    }

    public void ReplaceBaseDetailedTiming(
        int index,
        EdidDetailedTimingDescriptor descriptor)
    {
        if (index < 0 || index > 3)
            throw new ArgumentOutOfRangeException(nameof(index), "Index DTD invalide. Valeurs acceptées : 0 à 3.");

        int offset = 54 + index * 18;

        byte[] descriptorBytes = descriptor.ToBytes();

        Array.Copy(
            descriptorBytes,
            0,
            _edid,
            offset,
            descriptorBytes.Length);

        byte[] baseBlock = _edid.Take(128).ToArray();

        EdidChecksum.UpdateBlockChecksum(baseBlock);

        Array.Copy(
            baseBlock,
            0,
            _edid,
            0,
            128);
    }

    public bool IsBaseBlockChecksumValid()
    {
        byte[] baseBlock = _edid.Take(128).ToArray();

        return EdidChecksum.IsBlockChecksumValid(baseBlock);
    }
}