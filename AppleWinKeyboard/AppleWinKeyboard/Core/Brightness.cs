using System;
using System.Drawing;
using System.Management;
using System.Runtime.InteropServices;

namespace AppleWinKeyboard.Core
{
    public static class Brightness
    {
        [DllImport("gdi32.dll")]
        private unsafe static extern bool SetDeviceGammaRamp(Int32 hdc, void* ramp);

        private static bool initialized = false;
        private static Int32 hdc;

        private static int _step = 50;

        private static void InitializeClass()
        {
            if (initialized)
                return;
            hdc = Graphics.FromHwnd(IntPtr.Zero).GetHdc().ToInt32();
            initialized = true;
        }

        public static void BrightnessUp()
        {
            int currentBrightness = GetCurrentBrightness();
            //if (currentBrightness >= 0 && currentBrightness < 100)
                SetBrightness((short)(currentBrightness + _step));
        }

        public static void BrightnessDown()
        {
            int currentBrightness = GetCurrentBrightness();
            if (currentBrightness <= 100 && currentBrightness > 0)
                SetBrightness((short)(currentBrightness - _step));
        }

        public static short GetCurrentBrightness()
        {
            ManagementScope scope = new ManagementScope("\\\\.\\ROOT\\WMI");
            ObjectQuery query = new ObjectQuery("SELECT * FROM WmiMonitorBrightness");
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(scope, query);
            ManagementObjectCollection queryCollection = searcher.Get();

            foreach (ManagementObject m in queryCollection)
            {
                if (short.TryParse(m["CurrentBrightness"].ToString(), out short brightness))
                    return brightness;
            }
            return 0;
        }

        public static unsafe bool SetBrightness(short brightness)
        {
            InitializeClass();

            if (brightness > 255)
                brightness = 255;

            if (brightness < 0)
                brightness = 0;

            short* gArray = stackalloc short[3 * 256];
            short* idx = gArray;

            for (int j = 0; j < 3; j++)
            {
                for (int i = 0; i < 256; i++)
                {
                    int arrayVal = i * (brightness + 128);

                    if (arrayVal > 65535)
                        arrayVal = 65535;

                    *idx = (short)arrayVal;
                    idx++;
                }
            }

            //For some reason, this always returns false?
            bool retVal = SetDeviceGammaRamp(hdc, gArray);

            //Memory allocated through stackalloc is automatically free'd
            //by the CLR.

            return retVal;

        }
    }
}
