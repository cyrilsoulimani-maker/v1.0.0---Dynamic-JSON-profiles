using DisplaySwitcher.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace DisplaySwitcher.Services;

public static class DisplayService
{
    private const int ENUM_CURRENT_SETTINGS = -1;

    private const int DM_PELSWIDTH = 0x00080000;
    private const int DM_PELSHEIGHT = 0x00100000;
    private const int DM_DISPLAYFREQUENCY = 0x00400000;

    private const int DISP_CHANGE_SUCCESSFUL = 0;

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    private struct DEVMODE
    {
        private const int CCHDEVICENAME = 32;
        private const int CCHFORMNAME = 32;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = CCHDEVICENAME)]
        public string dmDeviceName;

        public short dmSpecVersion;
        public short dmDriverVersion;
        public short dmSize;
        public short dmDriverExtra;
        public int dmFields;

        public int dmPositionX;
        public int dmPositionY;
        public int dmDisplayOrientation;
        public int dmDisplayFixedOutput;

        public short dmColor;
        public short dmDuplex;
        public short dmYResolution;
        public short dmTTOption;
        public short dmCollate;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = CCHFORMNAME)]
        public string dmFormName;

        public short dmLogPixels;

        public int dmBitsPerPel;
        public int dmPelsWidth;
        public int dmPelsHeight;

        public int dmDisplayFlags;
        public int dmDisplayFrequency;

        public int dmICMMethod;
        public int dmICMIntent;
        public int dmMediaType;
        public int dmDitherType;
        public int dmReserved1;
        public int dmReserved2;

        public int dmPanningWidth;
        public int dmPanningHeight;
    }

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    private static extern bool EnumDisplaySettings(
        string? lpszDeviceName,
        int iModeNum,
        ref DEVMODE lpDevMode);

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    private static extern int ChangeDisplaySettingsEx(
        string? lpszDeviceName,
        ref DEVMODE lpDevMode,
        IntPtr hwnd,
        uint dwflags,
        IntPtr lParam);

    public static DisplayModeInfo GetCurrentMode()
    {
        return GetCurrentMode(null);
    }

    public static DisplayModeInfo GetCurrentMode(string? deviceName)
    {
        DEVMODE mode = new();
        mode.dmSize = (short)Marshal.SizeOf<DEVMODE>();

        if (!EnumDisplaySettings(deviceName, ENUM_CURRENT_SETTINGS, ref mode))
            throw new Exception("Impossible de lire les paramètres d'affichage.");

        return new DisplayModeInfo
        {
            Width = mode.dmPelsWidth,
            Height = mode.dmPelsHeight,
            Frequency = mode.dmDisplayFrequency
        };
    }

    public static List<DisplayModeInfo> GetAvailableModes(string deviceName)
    {
        List<DisplayModeInfo> modes = new();

        int modeIndex = 0;

        while (true)
        {
            DEVMODE mode = new();
            mode.dmSize = (short)Marshal.SizeOf<DEVMODE>();

            if (!EnumDisplaySettings(deviceName, modeIndex, ref mode))
                break;

            DisplayModeInfo current = new()
            {
                Width = mode.dmPelsWidth,
                Height = mode.dmPelsHeight,
                Frequency = mode.dmDisplayFrequency
            };

            if (!modes.Any(m =>
                m.Width == current.Width &&
                m.Height == current.Height &&
                m.Frequency == current.Frequency))
            {
                modes.Add(current);
            }

            modeIndex++;
        }

        return modes
            .OrderBy(m => m.Width)
            .ThenBy(m => m.Height)
            .ThenBy(m => m.Frequency)
            .ToList();
    }

    public static bool SetDisplayMode(
        string deviceName,
        int width,
        int height,
        int frequency)
    {
        DEVMODE mode = new();
        mode.dmSize = (short)Marshal.SizeOf<DEVMODE>();

        if (!EnumDisplaySettings(deviceName, ENUM_CURRENT_SETTINGS, ref mode))
            return false;

        mode.dmPelsWidth = width;
        mode.dmPelsHeight = height;
        mode.dmDisplayFrequency = frequency;

        mode.dmFields =
            DM_PELSWIDTH |
            DM_PELSHEIGHT |
            DM_DISPLAYFREQUENCY;

        int result =
            ChangeDisplaySettingsEx(
                deviceName,
                ref mode,
                IntPtr.Zero,
                0,
                IntPtr.Zero);

        return result == DISP_CHANGE_SUCCESSFUL;
    }
}