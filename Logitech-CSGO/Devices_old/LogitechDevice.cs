using LedCSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logitech_CSGO.Devices
{
    class LogitechDevice : Device
    {
        private String devicename = "Logitech";

        //Keyboard stuff
        private byte[] bitmap = new byte[LogitechGSDK.LOGI_LED_BITMAP_SIZE];
        private Color peripheral_Color = Color.Black;

        public void Initialize()
        {
            try
            {
                if (!LogitechGSDK.LogiLedInit())
                {
                    System.Windows.MessageBox.Show("Logitech LED SDK could not be initialized.");
                    return false;
                }

                LogitechGSDK.LogiLedSetTargetDevice(LogitechGSDK.LOGI_DEVICETYPE_RGB | LogitechGSDK.LOGI_DEVICETYPE_PERKEY_RGB);
                LogitechGSDK.LogiLedSaveCurrentLighting();

                SetAllKeys(Color.White);

                update_timer = new Timer(10);
                update_timer.Elapsed += new ElapsedEventHandler(update_timer_Tick);
                update_timer.Interval = 10; // in miliseconds
                update_timer.Start();

                isInitialized = true;

                return true;
            }
            catch (Exception exc)
            {
                System.Windows.MessageBox.Show("There was an error initializing Logitech LED SDK.\r\n" + exc.Message);

                return false;
            }
        }

        public void Shutdown()
        {
            if (isInitialized)
            {
                LogitechGSDK.LogiLedSetTargetDevice(LogitechGSDK.LOGI_DEVICETYPE_RGB | LogitechGSDK.LOGI_DEVICETYPE_PERKEY_RGB);

                LogitechGSDK.LogiLedRestoreLighting();
                LogitechGSDK.LogiLedShutdown();

                update_timer.Stop();
                bombtimer.Stop();
            }
        }

        public string GetDeviceName()
        {
            return devicename;
        }
    }
}
