namespace DisplaySwitcher.Services.Nvidia.Interop;

public static class NvGpuDisplayFlags
{
    public static bool IsDynamic(uint flags)
    {
        return (flags & (1 << 0)) != 0;
    }

    public static bool IsMultiStreamRootNode(uint flags)
    {
        return (flags & (1 << 1)) != 0;
    }

    public static bool IsActive(uint flags)
    {
        return (flags & (1 << 2)) != 0;
    }

    public static bool IsCluster(uint flags)
    {
        return (flags & (1 << 3)) != 0;
    }

    public static bool IsOSVisible(uint flags)
    {
        return (flags & (1 << 4)) != 0;
    }

    public static bool IsWFD(uint flags)
    {
        return (flags & (1 << 5)) != 0;
    }

    public static bool IsConnected(uint flags)
    {
        return (flags & (1 << 6)) != 0;
    }
}