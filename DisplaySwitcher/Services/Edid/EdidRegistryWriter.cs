using DisplaySwitcher.Services.Edid.Models;
using Microsoft.Win32;

namespace DisplaySwitcher.Services.Edid;

public class EdidRegistryWriter
{
    public bool TryWriteOverride(
        LocatedEdid monitor,
        byte[] modifiedEdid,
        out string message)
    {
        message = string.Empty;

        if (modifiedEdid.Length < 128 || modifiedEdid.Length % 128 != 0)
        {
            message = "Modified EDID length must be a multiple of 128 bytes.";
            return false;
        }

        int blockCount = modifiedEdid.Length / 128;

        for (int blockIndex = 0; blockIndex < blockCount; blockIndex++)
        {
            byte[] blockData = modifiedEdid
                .Skip(blockIndex * 128)
                .Take(128)
                .ToArray();

            if (!TryWriteBlock(monitor, blockIndex, blockData, out string blockMessage))
            {
                message = $"Block {blockIndex} failed: {blockMessage}";
                return false;
            }
        }

        message = $"OK - {blockCount} block(s) written";
        return true;
    }

    public bool TryWriteBlock(
        LocatedEdid monitor,
        int blockIndex,
        byte[] blockData,
        out string message)
    {
        message = string.Empty;

        if (blockData.Length != 128)
        {
            message = "An EDID block must contain exactly 128 bytes.";
            return false;
        }

        try
        {
            using RegistryKey? deviceKey =
                Registry.LocalMachine.OpenSubKey(
                    monitor.RegistryPath,
                    writable: true);

            if (deviceKey == null)
            {
                message = "Registry path not found.";
                return false;
            }

            using RegistryKey overrideKey =
                deviceKey.CreateSubKey(
                    "EDID_OVERRIDE",
                    writable: true);

            string valueName = blockIndex.ToString();

            overrideKey.SetValue(
                valueName,
                blockData,
                RegistryValueKind.Binary);

            byte[]? readBack =
                overrideKey.GetValue(valueName) as byte[];

            if (readBack == null)
            {
                message = "Unable to read written block.";
                return false;
            }

            if (!readBack.SequenceEqual(blockData))
            {
                message = "Verification failed.";
                return false;
            }

            message = "OK";
            return true;
        }
        catch (Exception ex)
        {
            message = ex.Message;
            return false;
        }
    }
}