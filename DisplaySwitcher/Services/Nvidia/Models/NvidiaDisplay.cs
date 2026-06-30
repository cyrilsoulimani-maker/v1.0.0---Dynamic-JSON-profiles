namespace DisplaySwitcher.Services.Nvidia.Models;

public class NvidiaDisplay
{
    public uint DisplayId { get; init; }

    public uint ConnectorType { get; init; }

    public bool IsConnected { get; init; }

    public bool IsActive { get; init; }

    public override string ToString()
    {
        return $"DisplayId={DisplayId}";
    }
}