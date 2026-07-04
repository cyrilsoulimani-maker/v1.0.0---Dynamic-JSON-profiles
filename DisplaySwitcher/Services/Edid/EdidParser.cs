using DisplaySwitcher.Services.Edid.Models;
using System.Text;

namespace DisplaySwitcher.Services.Edid;

public class EdidParser
{
    public ParsedEdid Parse(byte[] edid)
    {
        if (edid.Length < 128)
            return new ParsedEdid();

        return new ParsedEdid
        {
            IsValidHeader = IsHeaderValid(edid),
            IsChecksumValid = IsChecksumValid(edid),
            BlockCount = edid.Length / 128,
            ManufacturerId = DecodeManufacturerId(edid),
            ProductCode = DecodeProductCode(edid),
            SerialNumber = DecodeSerialNumber(edid),
            MonitorName = DecodeMonitorName(edid)
        };
    }

    private static bool IsHeaderValid(byte[] edid)
    {
        byte[] expected = [0x00, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x00];

        for (int i = 0; i < expected.Length; i++)
        {
            if (edid[i] != expected[i])
                return false;
        }

        return true;
    }

    private static bool IsChecksumValid(byte[] edid)
    {
        int blockCount = edid.Length / 128;

        for (int block = 0; block < blockCount; block++)
        {
            int sum = 0;

            for (int i = 0; i < 128; i++)
            {
                sum += edid[block * 128 + i];
            }

            if ((sum & 0xFF) != 0)
                return false;
        }

        return true;
    }

    private static string DecodeManufacturerId(byte[] edid)
    {
        ushort value = (ushort)((edid[8] << 8) | edid[9]);

        char c1 = (char)(((value >> 10) & 0x1F) + 64);
        char c2 = (char)(((value >> 5) & 0x1F) + 64);
        char c3 = (char)((value & 0x1F) + 64);

        return $"{c1}{c2}{c3}";
    }

    private static string DecodeProductCode(byte[] edid)
    {
        ushort productCode = (ushort)(edid[10] | (edid[11] << 8));

        return productCode.ToString("X4");
    }

    private static string DecodeSerialNumber(byte[] edid)
    {
        uint serial =
            (uint)(edid[12] |
            (edid[13] << 8) |
            (edid[14] << 16) |
            (edid[15] << 24));

        return serial == 0 ? string.Empty : serial.ToString();
    }

    private static string DecodeMonitorName(byte[] edid)
    {
        for (int offset = 54; offset <= 108; offset += 18)
        {
            bool isMonitorNameDescriptor =
                edid[offset] == 0x00 &&
                edid[offset + 1] == 0x00 &&
                edid[offset + 2] == 0x00 &&
                edid[offset + 3] == 0xFC;

            if (!isMonitorNameDescriptor)
                continue;

            byte[] nameBytes = edid
                .Skip(offset + 5)
                .Take(13)
                .Where(value => value != 0x00 && value != 0x0A && value != 0x0D)
                .ToArray();

            return Encoding.ASCII.GetString(nameBytes).Trim();
        }

        return string.Empty;
    }
}