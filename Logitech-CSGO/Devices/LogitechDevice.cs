using LedCSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logitech_CSGO.Devices
{
    enum Logitech_keyboardBitmapKeys
    {
        UNKNOWN = -1,
        ESC = 0,
        F1 = 4,
        F2 = 8,
        F3 = 12,
        F4 = 16,
        F5 = 20,
        F6 = 24,
        F7 = 28,
        F8 = 32,
        F9 = 36,
        F10 = 40,
        F11 = 44,
        F12 = 48,
        PRINT_SCREEN = 52,
        SCROLL_LOCK = 56,
        PAUSE_BREAK = 60,
        //64
        //68
        //72
        //76
        //80

        TILDE = 84,
        ONE = 88,
        TWO = 92,
        THREE = 96,
        FOUR = 100,
        FIVE = 104,
        SIX = 108,
        SEVEN = 112,
        EIGHT = 116,
        NINE = 120,
        ZERO = 124,
        MINUS = 128,
        EQUALS = 132,
        BACKSPACE = 136,
        INSERT = 140,
        HOME = 144,
        PAGE_UP = 148,
        NUM_LOCK = 152,
        NUM_SLASH = 156,
        NUM_ASTERISK = 160,
        NUM_MINUS = 164,

        TAB = 168,
        Q = 172,
        W = 176,
        E = 180,
        R = 184,
        T = 188,
        Y = 192,
        U = 196,
        I = 200,
        O = 204,
        P = 208,
        OPEN_BRACKET = 212,
        CLOSE_BRACKET = 216,
        BACKSLASH = 220,
        KEYBOARD_DELETE = 224,
        END = 228,
        PAGE_DOWN = 232,
        NUM_SEVEN = 236,
        NUM_EIGHT = 240,
        NUM_NINE = 244,
        NUM_PLUS = 248,

        CAPS_LOCK = 252,
        A = 256,
        S = 260,
        D = 264,
        F = 268,
        G = 272,
        H = 276,
        J = 280,
        K = 284,
        L = 288,
        SEMICOLON = 292,
        APOSTROPHE = 296,
        HASHTAG = 300,//300
        ENTER = 304,
        //308
        //312
        //316
        NUM_FOUR = 320,
        NUM_FIVE = 324,
        NUM_SIX = 328,
        //332

        LEFT_SHIFT = 336,
        BACKSLASH_UK = 340,
        Z = 344,
        X = 348,
        C = 352,
        V = 356,
        B = 360,
        N = 364,
        M = 368,
        COMMA = 372,
        PERIOD = 376,
        FORWARD_SLASH = 380,
        //384
        RIGHT_SHIFT = 388,
        //392
        ARROW_UP = 396,
        //400
        NUM_ONE = 404,
        NUM_TWO = 408,
        NUM_THREE = 412,
        NUM_ENTER = 416,

        LEFT_CONTROL = 420,
        LEFT_WINDOWS = 424,
        LEFT_ALT = 428,
        //432
        //436
        SPACE = 440,
        //444
        //448
        //452
        //456
        //460
        RIGHT_ALT = 464,
        RIGHT_WINDOWS = 468,
        APPLICATION_SELECT = 472,
        RIGHT_CONTROL = 476,
        ARROW_LEFT = 480,
        ARROW_DOWN = 484,
        ARROW_RIGHT = 488,
        NUM_ZERO = 492,
        NUM_PERIOD = 496,
        //500
    };
    
    class LogitechDevice : Device
    {
        private String devicename = "Logitech";
        private bool isInitialized = false;

        private bool keyboard_updated = false;
        private bool peripheral_updated = false;

        //Keyboard stuff
        private Logitech_keyboardBitmapKeys[] allKeys = Enum.GetValues(typeof(Logitech_keyboardBitmapKeys)).Cast<Logitech_keyboardBitmapKeys>().ToArray();
        private byte[] bitmap = new byte[LogitechGSDK.LOGI_LED_BITMAP_SIZE];
        private Color peripheral_Color = Color.Black;
        //Previous data
        private byte[] previous_bitmap = new byte[LogitechGSDK.LOGI_LED_BITMAP_SIZE];
        private Color previous_peripheral_Color = Color.Black;

        public bool Initialize()
        {
            try
            {
                if (!LogitechGSDK.LogiLedInit())
                {
                    System.Windows.MessageBox.Show("Logitech LED SDK could not be initialized.");

                    isInitialized = false;
                    return false;
                }

                LogitechGSDK.LogiLedSetTargetDevice(LogitechGSDK.LOGI_DEVICETYPE_RGB | LogitechGSDK.LOGI_DEVICETYPE_PERKEY_RGB);
                LogitechGSDK.LogiLedSaveCurrentLighting();

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
                this.Reset();
                LogitechGSDK.LogiLedShutdown();
            }
        }

        public string GetDeviceName()
        {
            return devicename;
        }

        public void Reset()
        {
            if (this.IsInitialized() && (keyboard_updated || peripheral_updated))
            {
                LogitechGSDK.LogiLedSetTargetDevice(LogitechGSDK.LOGI_DEVICETYPE_RGB | LogitechGSDK.LOGI_DEVICETYPE_PERKEY_RGB);
                LogitechGSDK.LogiLedRestoreLighting();
                keyboard_updated = false;
                peripheral_updated = false;
            }
        }

        public bool Reconnect()
        {
            throw new NotImplementedException();
        }

        public bool IsConnected()
        {
            throw new NotImplementedException();
        }

        private void SetOneKey(Logitech_keyboardBitmapKeys key, Color color)
        {
            bitmap[(int)key] = color.B;
            bitmap[(int)key + 1] = color.G;
            bitmap[(int)key + 2] = color.R;
            bitmap[(int)key + 3] = color.A;
        }


        private void SendColorsToKeyboard(bool forced = false)
        {
            if (!Enumerable.SequenceEqual(bitmap, previous_bitmap) || forced)
            {
                LogitechGSDK.LogiLedSetTargetDevice(LogitechGSDK.LOGI_DEVICETYPE_PERKEY_RGB);

                LogitechGSDK.LogiLedSetLightingFromBitmap(bitmap);
                bitmap.CopyTo(previous_bitmap, 0);
                keyboard_updated = true;
            }
        }

        private void SendColorToPeripheral(Color color, bool forced = false)
        {
            if (!previous_peripheral_Color.Equals(color) || forced)
            {
                LogitechGSDK.LogiLedSetTargetDevice(LogitechGSDK.LOGI_DEVICETYPE_RGB);

                if (Global.Configuration.bg_peripheral_use || Global.Configuration.bomb_peripheral_use || Global.Configuration.flashbang_peripheral_use || Global.Configuration.burning_peripheral_use)
                {
                    LogitechGSDK.LogiLedSetLighting((int)color.R / 255, (int)color.G / 255, (int)color.B / 255);
                    previous_peripheral_Color = color;
                    peripheral_updated = true;
                }
                else
                {
                    if (peripheral_updated)
                    {
                        LogitechGSDK.LogiLedRestoreLighting();
                        peripheral_updated = false;
                    }
                }
            }
        }

        public bool IsInitialized()
        {
            return this.isInitialized;
        }

        public bool UpdateDevice(Dictionary<DeviceKeys, Color> keyColors, bool forced = false)
        {
            try
            {
                foreach (KeyValuePair<DeviceKeys, Color> key in keyColors)
                {
                    Logitech_keyboardBitmapKeys localKey = ToLogitechBitmap(key.Key);

                    if (localKey == Logitech_keyboardBitmapKeys.UNKNOWN && key.Key == DeviceKeys.Peripheral)
                    {
                        SendColorToPeripheral(key.Value, forced);
                    }
                    else if(localKey != Logitech_keyboardBitmapKeys.UNKNOWN)
                    {
                        SetOneKey(localKey, key.Value);
                    }
                }

                SendColorsToKeyboard(forced);
                return true;
            }
            catch(Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }

        private Logitech_keyboardBitmapKeys ToLogitechBitmap(DeviceKeys key)
        {
            switch (key)
            {
                case(DeviceKeys.ESC):
                    return Logitech_keyboardBitmapKeys.ESC;
                case(DeviceKeys.F1):
                    return Logitech_keyboardBitmapKeys.F1;
                case(DeviceKeys.F2):
                    return Logitech_keyboardBitmapKeys.F2;
                case(DeviceKeys.F3):
                    return Logitech_keyboardBitmapKeys.F3;
                case(DeviceKeys.F4):
                    return Logitech_keyboardBitmapKeys.F4;
                case(DeviceKeys.F5):
                    return Logitech_keyboardBitmapKeys.F5;
                case(DeviceKeys.F6):
                    return Logitech_keyboardBitmapKeys.F6;
                case(DeviceKeys.F7):
                    return Logitech_keyboardBitmapKeys.F7;
                case(DeviceKeys.F8):
                    return Logitech_keyboardBitmapKeys.F8;
                case(DeviceKeys.F9):
                    return Logitech_keyboardBitmapKeys.F9;
                case(DeviceKeys.F10):
                    return Logitech_keyboardBitmapKeys.F10;
                case(DeviceKeys.F11):
                    return Logitech_keyboardBitmapKeys.F11;
                case(DeviceKeys.F12):
                    return Logitech_keyboardBitmapKeys.F12;
                case(DeviceKeys.PRINT_SCREEN):
                    return Logitech_keyboardBitmapKeys.PRINT_SCREEN;
                case(DeviceKeys.SCROLL_LOCK):
                    return Logitech_keyboardBitmapKeys.SCROLL_LOCK;
                case(DeviceKeys.PAUSE_BREAK):
                    return Logitech_keyboardBitmapKeys.PAUSE_BREAK;
                case(DeviceKeys.TILDE):
                    return Logitech_keyboardBitmapKeys.TILDE;
                case(DeviceKeys.ONE):
                    return Logitech_keyboardBitmapKeys.ONE;
                case(DeviceKeys.TWO):
                    return Logitech_keyboardBitmapKeys.TWO;
                case(DeviceKeys.THREE):
                    return Logitech_keyboardBitmapKeys.THREE;
                case(DeviceKeys.FOUR):
                    return Logitech_keyboardBitmapKeys.FOUR;
                case(DeviceKeys.FIVE):
                    return Logitech_keyboardBitmapKeys.FIVE;
                case(DeviceKeys.SIX):
                    return Logitech_keyboardBitmapKeys.SIX;
                case(DeviceKeys.SEVEN):
                    return Logitech_keyboardBitmapKeys.SEVEN;
                case(DeviceKeys.EIGHT):
                    return Logitech_keyboardBitmapKeys.EIGHT;
                case(DeviceKeys.NINE):
                    return Logitech_keyboardBitmapKeys.NINE;
                case(DeviceKeys.ZERO):
                    return Logitech_keyboardBitmapKeys.ZERO;
                case(DeviceKeys.MINUS):
                    return Logitech_keyboardBitmapKeys.MINUS;
                case(DeviceKeys.EQUALS):
                    return Logitech_keyboardBitmapKeys.EQUALS;
                case(DeviceKeys.BACKSPACE):
                    return Logitech_keyboardBitmapKeys.BACKSPACE;
                case(DeviceKeys.INSERT):
                    return Logitech_keyboardBitmapKeys.INSERT;
                case(DeviceKeys.HOME):
                    return Logitech_keyboardBitmapKeys.HOME;
                case(DeviceKeys.PAGE_UP):
                    return Logitech_keyboardBitmapKeys.PAGE_UP;
                case(DeviceKeys.NUM_LOCK):
                    return Logitech_keyboardBitmapKeys.NUM_LOCK;
                case(DeviceKeys.NUM_SLASH):
                    return Logitech_keyboardBitmapKeys.NUM_SLASH;
                case(DeviceKeys.NUM_ASTERISK):
                    return Logitech_keyboardBitmapKeys.NUM_ASTERISK;
                case(DeviceKeys.NUM_MINUS):
                    return Logitech_keyboardBitmapKeys.NUM_MINUS;
                case(DeviceKeys.TAB):
                    return Logitech_keyboardBitmapKeys.TAB;
                case(DeviceKeys.Q):
                    return Logitech_keyboardBitmapKeys.Q;
                case(DeviceKeys.W):
                    return Logitech_keyboardBitmapKeys.W;
                case(DeviceKeys.E):
                    return Logitech_keyboardBitmapKeys.E;
                case(DeviceKeys.R):
                    return Logitech_keyboardBitmapKeys.R;
                case(DeviceKeys.T):
                    return Logitech_keyboardBitmapKeys.T;
                case(DeviceKeys.Y):
                    return Logitech_keyboardBitmapKeys.Y;
                case(DeviceKeys.U):
                    return Logitech_keyboardBitmapKeys.U;
                case(DeviceKeys.I):
                    return Logitech_keyboardBitmapKeys.I;
                case(DeviceKeys.O):
                    return Logitech_keyboardBitmapKeys.O;
                case(DeviceKeys.P):
                    return Logitech_keyboardBitmapKeys.P;
                case(DeviceKeys.OPEN_BRACKET):
                    return Logitech_keyboardBitmapKeys.OPEN_BRACKET;
                case(DeviceKeys.CLOSE_BRACKET):
                    return Logitech_keyboardBitmapKeys.CLOSE_BRACKET;
                case(DeviceKeys.BACKSLASH):
                    return Logitech_keyboardBitmapKeys.BACKSLASH;
                case(DeviceKeys.KEYBOARD_DELETE):
                    return Logitech_keyboardBitmapKeys.KEYBOARD_DELETE;
                case(DeviceKeys.END):
                    return Logitech_keyboardBitmapKeys.END;
                case(DeviceKeys.PAGE_DOWN):
                    return Logitech_keyboardBitmapKeys.PAGE_DOWN;
                case(DeviceKeys.NUM_SEVEN):
                    return Logitech_keyboardBitmapKeys.NUM_SEVEN;
                case(DeviceKeys.NUM_EIGHT):
                    return Logitech_keyboardBitmapKeys.NUM_EIGHT;
                case(DeviceKeys.NUM_NINE):
                    return Logitech_keyboardBitmapKeys.NUM_NINE;
                case(DeviceKeys.NUM_PLUS):
                    return Logitech_keyboardBitmapKeys.NUM_PLUS;
                case(DeviceKeys.CAPS_LOCK):
                    return Logitech_keyboardBitmapKeys.CAPS_LOCK;
                case(DeviceKeys.A):
                    return Logitech_keyboardBitmapKeys.A;
                case(DeviceKeys.S):
                    return Logitech_keyboardBitmapKeys.S;
                case(DeviceKeys.D):
                    return Logitech_keyboardBitmapKeys.D;
                case(DeviceKeys.F):
                    return Logitech_keyboardBitmapKeys.F;
                case(DeviceKeys.G):
                    return Logitech_keyboardBitmapKeys.G;
                case(DeviceKeys.H):
                    return Logitech_keyboardBitmapKeys.H;
                case(DeviceKeys.J):
                    return Logitech_keyboardBitmapKeys.J;
                case(DeviceKeys.K):
                    return Logitech_keyboardBitmapKeys.K;
                case(DeviceKeys.L):
                    return Logitech_keyboardBitmapKeys.L;
                case(DeviceKeys.SEMICOLON):
                    return Logitech_keyboardBitmapKeys.SEMICOLON;
                case(DeviceKeys.APOSTROPHE):
                    return Logitech_keyboardBitmapKeys.APOSTROPHE;
                case(DeviceKeys.HASHTAG):
                    return Logitech_keyboardBitmapKeys.HASHTAG;
                case(DeviceKeys.ENTER):
                    return Logitech_keyboardBitmapKeys.ENTER;
                case(DeviceKeys.NUM_FOUR):
                    return Logitech_keyboardBitmapKeys.NUM_FOUR;
                case(DeviceKeys.NUM_FIVE):
                    return Logitech_keyboardBitmapKeys.NUM_FIVE;
                case(DeviceKeys.NUM_SIX):
                    return Logitech_keyboardBitmapKeys.NUM_SIX;
                case(DeviceKeys.LEFT_SHIFT):
                    return Logitech_keyboardBitmapKeys.LEFT_SHIFT;
                case(DeviceKeys.BACKSLASH_UK):
                    return Logitech_keyboardBitmapKeys.BACKSLASH_UK;
                case(DeviceKeys.Z):
                    return Logitech_keyboardBitmapKeys.Z;
                case(DeviceKeys.X):
                    return Logitech_keyboardBitmapKeys.X;
                case(DeviceKeys.C):
                    return Logitech_keyboardBitmapKeys.C;
                case(DeviceKeys.V):
                    return Logitech_keyboardBitmapKeys.V;
                case(DeviceKeys.B):
                    return Logitech_keyboardBitmapKeys.B;
                case(DeviceKeys.N):
                    return Logitech_keyboardBitmapKeys.N;
                case(DeviceKeys.M):
                    return Logitech_keyboardBitmapKeys.M;
                case(DeviceKeys.COMMA):
                    return Logitech_keyboardBitmapKeys.COMMA;
                case(DeviceKeys.PERIOD):
                    return Logitech_keyboardBitmapKeys.PERIOD;
                case(DeviceKeys.FORWARD_SLASH):
                    return Logitech_keyboardBitmapKeys.FORWARD_SLASH;
                case(DeviceKeys.RIGHT_SHIFT):
                    return Logitech_keyboardBitmapKeys.RIGHT_SHIFT;
                case(DeviceKeys.ARROW_UP):
                    return Logitech_keyboardBitmapKeys.ARROW_UP;
                case(DeviceKeys.NUM_ONE):
                    return Logitech_keyboardBitmapKeys.NUM_ONE;
                case(DeviceKeys.NUM_TWO):
                    return Logitech_keyboardBitmapKeys.NUM_TWO;
                case(DeviceKeys.NUM_THREE):
                    return Logitech_keyboardBitmapKeys.NUM_THREE;
                case(DeviceKeys.NUM_ENTER):
                    return Logitech_keyboardBitmapKeys.NUM_ENTER;
                case(DeviceKeys.LEFT_CONTROL):
                    return Logitech_keyboardBitmapKeys.LEFT_CONTROL;
                case(DeviceKeys.LEFT_WINDOWS):
                    return Logitech_keyboardBitmapKeys.LEFT_WINDOWS;
                case(DeviceKeys.LEFT_ALT):
                    return Logitech_keyboardBitmapKeys.LEFT_ALT;
                case(DeviceKeys.SPACE):
                    return Logitech_keyboardBitmapKeys.SPACE;
                case(DeviceKeys.RIGHT_ALT):
                    return Logitech_keyboardBitmapKeys.RIGHT_ALT;
                case(DeviceKeys.RIGHT_WINDOWS):
                    return Logitech_keyboardBitmapKeys.RIGHT_WINDOWS;
                case(DeviceKeys.APPLICATION_SELECT):
                    return Logitech_keyboardBitmapKeys.APPLICATION_SELECT;
                case(DeviceKeys.RIGHT_CONTROL):
                    return Logitech_keyboardBitmapKeys.RIGHT_CONTROL;
                case(DeviceKeys.ARROW_LEFT):
                    return Logitech_keyboardBitmapKeys.ARROW_LEFT;
                case(DeviceKeys.ARROW_DOWN):
                    return Logitech_keyboardBitmapKeys.ARROW_DOWN;
                case(DeviceKeys.ARROW_RIGHT):
                    return Logitech_keyboardBitmapKeys.ARROW_RIGHT;
                case(DeviceKeys.NUM_ZERO):
                    return Logitech_keyboardBitmapKeys.NUM_ZERO;
                case(DeviceKeys.NUM_PERIOD):
                    return Logitech_keyboardBitmapKeys.NUM_PERIOD;
                default:
                    return Logitech_keyboardBitmapKeys.UNKNOWN;
            }
        }
    }
}
