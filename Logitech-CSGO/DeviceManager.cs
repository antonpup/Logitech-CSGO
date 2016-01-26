using Logitech_CSGO.Devices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logitech_CSGO
{
    class DeviceManager
    {
        private List<Device> devices = new List<Device>();

        public DeviceManager()
        {
            devices.Add(new LogitechDevice());
        }

        public bool Initialize()
        {
            bool anyInitialized = false;
            
            foreach(Device device in devices)
            {
                if (device.Initialize())
                    anyInitialized = true;
            }

            return anyInitialized;
        }

        public void ResetDevices()
        {
            foreach (Device device in devices)
            {
                if (device.IsInitialized())
                {
                    device.Reset();
                }
            }
        }

        public bool UpdateDevices(Dictionary<DeviceKeys, System.Drawing.Color> keyColors, bool forced = false)
        {
            bool anyUpdated = false;
            Dictionary<DeviceKeys, System.Drawing.Color> _keyColors = new Dictionary<DeviceKeys, System.Drawing.Color>(keyColors);

            foreach (Device device in devices)
            {
                if (device.IsInitialized())
                {
                    if (device.UpdateDevice(_keyColors, forced))
                        anyUpdated = true;
                }
            }

            return anyUpdated;
        }

    }
}
