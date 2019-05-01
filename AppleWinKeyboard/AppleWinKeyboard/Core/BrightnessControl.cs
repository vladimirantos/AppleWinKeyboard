using System;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows;

namespace AppleWinKeyboard.Core
{
    internal class BrightnessControlWinApi
    {
        private const int MONITOR_DEFAULTTONEAREST = 2;

        private const int PHYSICAL_MONITOR_DESCRIPTION_SIZE = 128;

        private const int MC_CAPS_BRIGHTNESS = 0x2;
        private const int MC_CAPS_CONTRAST = 0x4;

        [DllImport("user32.dll", SetLastError = true)]
        private extern static bool GetCursorPos(out POINT lpPoint);

        [DllImport("user32.dll", SetLastError = false)]
        private extern static IntPtr MonitorFromPoint(POINT pt, uint dwFlags);

        [DllImport("dxva2.dll", SetLastError = true)]
        private extern static bool GetMonitorBrightness(IntPtr hMonitor, out uint pdwMinimumBrightness, out uint pdwCurrentBrightness, out uint pdwMaximumBrightness);

        [DllImport("dxva2.dll", SetLastError = true)]
        private extern static bool SetMonitorBrightness(IntPtr hMonitor, uint dwNewBrightness);

        [DllImport("dxva2.dll", SetLastError = true)]
        private extern static bool GetPhysicalMonitorsFromHMONITOR(IntPtr hMonitor, uint dwPhysicalMonitorArraySize, [Out] PHYSICAL_MONITOR[] pPhysicalMonitorArray);

        [DllImport("dxva2.dll", SetLastError = true)]
        private extern static bool GetNumberOfPhysicalMonitorsFromHMONITOR(IntPtr hMonitor, out uint pdwNumberOfPhysicalMonitors);


        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            int x;
            int y;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct PHYSICAL_MONITOR
        {
            public IntPtr hPhysicalMonitor;

            [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U2, SizeConst = PHYSICAL_MONITOR_DESCRIPTION_SIZE)]
            public char[] szPhysicalMonitorDescription;
        }

        public static IntPtr GetCurrentMonitor()
        {
            POINT point = new POINT();
            if (!GetCursorPos(out point))
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }

            return MonitorFromPoint(point, MONITOR_DEFAULTTONEAREST);
        }

        public static PHYSICAL_MONITOR[] GetPhysicalMonitors(IntPtr hMonitor)
        {
            uint dwNumberOfPhysicalMonitors;
            if (!GetNumberOfPhysicalMonitorsFromHMONITOR(hMonitor, out dwNumberOfPhysicalMonitors))
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }
            PHYSICAL_MONITOR[] physicalMonitorArray = new PHYSICAL_MONITOR[dwNumberOfPhysicalMonitors];
            if (!GetPhysicalMonitorsFromHMONITOR(hMonitor, dwNumberOfPhysicalMonitors, physicalMonitorArray))
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }
            return physicalMonitorArray;
        }

        public static double GetMonitorBrightness(PHYSICAL_MONITOR physicalMonitor)
        {
            uint dwMinimumBrightness, dwCurrentBrightness, dwMaximumBrightness;
            if (!GetMonitorBrightness(physicalMonitor.hPhysicalMonitor, out dwMinimumBrightness, out dwCurrentBrightness, out dwMaximumBrightness))
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }
            return (double)(dwCurrentBrightness - dwMinimumBrightness) / (double)(dwMaximumBrightness - dwMinimumBrightness);
        }

        public static void SetMonitorBrightness(PHYSICAL_MONITOR physicalMonitor, double brightness)
        {
            uint dwMinimumBrightness, dwCurrentBrightness, dwMaximumBrightness;
            if (!GetMonitorBrightness(physicalMonitor.hPhysicalMonitor, out dwMinimumBrightness, out dwCurrentBrightness, out dwMaximumBrightness))
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }
            if (!SetMonitorBrightness(physicalMonitor.hPhysicalMonitor, (uint)(dwMinimumBrightness + (dwMaximumBrightness - dwMinimumBrightness) * brightness)))
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }
        }
    }

    internal class BrightnessControl
    {
        const int ERROR_GEN_FAILURE = 0x1F;
        BrightnessControlWinApi.PHYSICAL_MONITOR[] physicalMonitors;
        private double currentMonitorBrightness;
        public BrightnessControl()
        {
            physicalMonitors = BrightnessControlWinApi.GetPhysicalMonitors(BrightnessControlWinApi.GetCurrentMonitor());
            currentMonitorBrightness = BrightnessControlWinApi.GetMonitorBrightness(physicalMonitors[3]) * 100;
        }

        public void BrightnessUp()
        {
            foreach (BrightnessControlWinApi.PHYSICAL_MONITOR physicalMonitor in physicalMonitors)
            {
                try
                {
                    BrightnessControlWinApi.SetMonitorBrightness(physicalMonitor, currentMonitorBrightness + 1);
                }
                catch (Win32Exception e_)
                {
                    // LG Flatron W2443T sometimes causes ERROR_GEN_FAILURE when rapidly changing brightness or contrast
                    if (e_.NativeErrorCode == ERROR_GEN_FAILURE)
                    {
                        break;
                    }
                    else
                    {
                        throw;
                    }
                }
            }
        }

        public void BrightnessDown()
        {
            foreach (BrightnessControlWinApi.PHYSICAL_MONITOR physicalMonitor in physicalMonitors)
            {
                try
                {
                    BrightnessControlWinApi.SetMonitorBrightness(physicalMonitor, currentMonitorBrightness - 1);
                }
                catch (Win32Exception e_)
                {
                    // LG Flatron W2443T sometimes causes ERROR_GEN_FAILURE when rapidly changing brightness or contrast
                    if (e_.NativeErrorCode == ERROR_GEN_FAILURE)
                    {
                        break;
                    }
                    else
                    {
                        throw;
                    }
                }
            }
        }
    }
}
