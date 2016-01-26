using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logitech_CSGO.Devices
{
    interface Device
    {
        void Initialize();

        void Shutdown();

        String GetDeviceName();
    }
}
