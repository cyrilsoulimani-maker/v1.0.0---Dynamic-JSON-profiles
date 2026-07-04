namespace DisplaySwitcher.Services.Edid;

public static class EdidChecksum
{
    public static bool IsBlockChecksumValid(byte[] block)
    {
        if (block.Length != 128)
            return false;

        int sum = 0;

        foreach (byte value in block)
        {
            sum += value;
        }

        return (sum & 0xFF) == 0;
    }

    public static void UpdateBlockChecksum(byte[] block)
    {
        if (block.Length != 128)
            throw new ArgumentException("Un bloc EDID doit contenir exactement 128 octets.");

        block[127] = 0;

        int sum = 0;

        for (int i = 0; i < 127; i++)
        {
            sum += block[i];
        }

        block[127] = (byte)((256 - (sum & 0xFF)) & 0xFF);
    }
}