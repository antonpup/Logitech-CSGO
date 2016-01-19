using CSGSI.Nodes;
using LedCSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace Logitech_CSGO
{
    public enum keyboardBitmapKeys
    {
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
        ENTER = 304,
        NUM_FOUR =  320,
        NUM_FIVE = 324,
        NUM_SIX = 328,
        LEFT_SHIFT = 336,
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
        RIGHT_SHIFT = 388,
        ARROW_UP = 396,
        NUM_ONE = 404,
        NUM_TWO = 408,
        NUM_THREE = 412,
        NUM_ENTER = 416,
        LEFT_CONTROL = 420,
        LEFT_WINDOWS = 424,
        LEFT_ALT = 428,
        SPACE = 440,
        RIGHT_ALT = 464,
        RIGHT_WINDOWS = 468,
        APPLICATION_SELECT = 472,
        RIGHT_CONTROL = 476,
        ARROW_LEFT = 480,
        ARROW_DOWN = 484,
        ARROW_RIGHT = 488,
        NUM_ZERO = 492,
        NUM_PERIOD = 496,
    };
    
    public class GameEventHandler
    {
        private bool isInitialized = false;
        
        private bool keyboard_updated = false;
        private Timer update_timer;

        private Color ct_color = Color.FromArgb(158, 205, 255);
        private Color t_color = Color.FromArgb(221, 99, 33);
        private Color ammo_color = Color.FromArgb(255, 0, 255);
        private Color noammo_color = Color.FromArgb(255, 0, 0);
        private Color savekey_color = Color.FromArgb(255, 220, 0);

        private keyboardBitmapKeys[] allKeys = Enum.GetValues(typeof(keyboardBitmapKeys)).Cast<keyboardBitmapKeys>().ToArray();
        private keyboardBitmapKeys[] hpKeys = { keyboardBitmapKeys.F1, keyboardBitmapKeys.F2, keyboardBitmapKeys.F3, keyboardBitmapKeys.F4, keyboardBitmapKeys.F5, keyboardBitmapKeys.F6, keyboardBitmapKeys.F7, keyboardBitmapKeys.F8, keyboardBitmapKeys.F9, keyboardBitmapKeys.F10, keyboardBitmapKeys.F11, keyboardBitmapKeys.F12 };
        private keyboardBitmapKeys[] ammocountKeys = { keyboardBitmapKeys.ONE, keyboardBitmapKeys.TWO, keyboardBitmapKeys.THREE, keyboardBitmapKeys.FOUR, keyboardBitmapKeys.FIVE, keyboardBitmapKeys.SIX, keyboardBitmapKeys.SEVEN, keyboardBitmapKeys.EIGHT, keyboardBitmapKeys.NINE, keyboardBitmapKeys.ZERO, keyboardBitmapKeys.MINUS, keyboardBitmapKeys.EQUALS };
        private keyboardBitmapKeys[] bombKeys = { keyboardBitmapKeys.NUM_LOCK, keyboardBitmapKeys.NUM_SLASH, keyboardBitmapKeys.NUM_ASTERISK, keyboardBitmapKeys.NUM_MINUS, keyboardBitmapKeys.NUM_SEVEN, keyboardBitmapKeys.NUM_EIGHT, keyboardBitmapKeys.NUM_NINE, keyboardBitmapKeys.NUM_PLUS, keyboardBitmapKeys.NUM_FOUR, keyboardBitmapKeys.NUM_FIVE, keyboardBitmapKeys.NUM_SIX, keyboardBitmapKeys.NUM_ONE, keyboardBitmapKeys.NUM_TWO, keyboardBitmapKeys.NUM_THREE, keyboardBitmapKeys.NUM_ZERO, keyboardBitmapKeys.NUM_PERIOD, keyboardBitmapKeys.NUM_ENTER };
        private keyboardBitmapKeys[] saveKeys = { keyboardBitmapKeys.W, keyboardBitmapKeys.A, keyboardBitmapKeys.S, keyboardBitmapKeys.D, keyboardBitmapKeys.LEFT_SHIFT, keyboardBitmapKeys.LEFT_CONTROL, keyboardBitmapKeys.SPACE };

        private Stopwatch bombtimer = new Stopwatch();
        private bool bombflash = false;
        private int bombflashcount = 0;
        private long bombflashtime = 0;
        private long bombflashedat = 0;
        private bool gradualbombtimer = true;

        private byte[] bitmap = new byte[LogitechGSDK.LOGI_LED_BITMAP_SIZE];
        PlayerTeam current_team = PlayerTeam.Undefined;
        int current_health = 0;
        int clip = 1;
        int clip_max = 1;
        BombState bombstate = BombState.Undefined;
        int flashamount = 0;

        ~GameEventHandler()
        {
            if (isInitialized)
            {
                LogitechGSDK.LogiLedRestoreLighting();
                LogitechGSDK.LogiLedShutdown();
            }
        }
        
        public bool Init()
        {
            try
            {
                if (!LogitechGSDK.LogiLedInit())
                {
                    System.Windows.MessageBox.Show("Logitech LED SDK could not be initialized.");
                    return false;
                }

                LogitechGSDK.LogiLedSetTargetDevice(LogitechGSDK.LOGI_DEVICETYPE_PERKEY_RGB); 
                LogitechGSDK.LogiLedSaveCurrentLighting();

                SetAllKeys(Color.White);

                update_timer = new Timer(10);
                update_timer.Elapsed += new ElapsedEventHandler(update_timer_Tick);
                update_timer.Interval = 10; // in miliseconds
                update_timer.Start();

                isInitialized = true;

                return true;
            }
            catch(Exception exc)
            {
                System.Windows.MessageBox.Show("There was an error initializing Logitech LED SDK.\r\n" + exc.Message);

                return false;
            }
        }

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();

        [System.Runtime.InteropServices.DllImport("user32.dll", SetLastError = true)]
        static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        private string GetActiveWindowsProcessname()
        {
            try
            {
                IntPtr hWnd = GetForegroundWindow();
                uint procId = 0;
                GetWindowThreadProcessId(hWnd, out procId);
                var proc = Process.GetProcessById((int)procId);
                return proc.MainModule.FileName;
            }
            catch(Exception exc)
            {
                //Console.WriteLine(exc);
                return "";
            }
        }

        private void update_timer_Tick(object sender, EventArgs e)
        {
            if (GetActiveWindowsProcessname().ToLowerInvariant().EndsWith("\\csgo.exe"))
            {
                UpdateKeyboard();
            }
            else
            {
                if (keyboard_updated)
                    LogitechGSDK.LogiLedRestoreLighting();
            }
           
        }

        private void SetAllKeys(Color color)
        {
            foreach (keyboardBitmapKeys key in allKeys)
                SetOneKey(key, color);
        }

        private void SetAllKeysEffect(Color color)
        {
            foreach (keyboardBitmapKeys key in allKeys)
            {
                Color keycolor = GetOneKey(key);

                byte r = Math.Max(color.R, keycolor.R);
                byte g = Math.Max(color.G, keycolor.G);
                byte b = Math.Max(color.B, keycolor.B);

                SetOneKey(key, Color.FromArgb(r, g, b));
            }
        }

        private Color GetOneKey(keyboardBitmapKeys key)
        {
            Color ret = Color.FromArgb(bitmap[(int)key + 2], bitmap[(int)key + 1], bitmap[(int)key]);

            return ret;
        }

        private void SetOneKey(keyboardBitmapKeys key, Color color)
        {
            bitmap[(int)key] = color.B;
            bitmap[(int)key + 1] = color.G;
            bitmap[(int)key + 2] = color.R;
            bitmap[(int)key + 3] = (byte)255;
        }

        public void SetTeam(PlayerTeam team)
        {
            this.current_team = team;
        }

        public void SetHealth(int health)
        {
            this.current_health = health;
        }

        public void SetClip(int clip)
        {
            this.clip = clip;
        }

        public void SetClipMax(int clipmax)
        {
            this.clip_max = clipmax;
        }

        public void SetBombState(BombState state)
        {
            this.bombstate = state;
        }

        public void SetFlashAmount(int flash)
        {
            this.flashamount = flash;
        }

        public void UpdateKeyboard()
        {
            Stopwatch stopwatch = Stopwatch.StartNew();

            //update background
            if (this.current_team == PlayerTeam.T)
                SetAllKeys(t_color);
            else
                SetAllKeys(ct_color); //Acts as CT team and ambient background

            //Not initialized
            if (this.current_team != PlayerTeam.Undefined)
            {
                //Update Health
                double hpstatus = ((double)this.current_health / 100.0) * hpKeys.Count();
                for (int i = 0; i < hpKeys.Count(); i++)
                {
                    keyboardBitmapKeys current_key = hpKeys[i];

                    if (i == (int)hpstatus)
                    {
                        double percent = (double)hpstatus - i;
                        SetOneKey(current_key, Color.FromArgb((int)((1.0 - percent) * 255), (int)((percent) * 255), 0));
                    }
                    else if (i > hpstatus)
                        SetOneKey(current_key, Color.FromArgb(255, 0, 0));
                    else
                        SetOneKey(current_key, Color.FromArgb(0, 255, 0));

                }

                //Update Ammo
                int ammocount = (int)(((double)this.clip / (double)this.clip_max) * ammocountKeys.Count());
                for (int i = 0; i < ammocountKeys.Count(); i++)
                {
                    keyboardBitmapKeys current_name = ammocountKeys[i];
                    if (i < ammocount)
                        SetOneKey(current_name, Color.FromArgb((int)((ammo_color.R / 255.0) * 255), (int)((ammo_color.G / 255.0) * 255), (int)((ammo_color.B / 255.0) * 255)));
                    else
                        SetOneKey(current_name, Color.FromArgb((int)((noammo_color.R / 255.0) * 255), (int)((noammo_color.G / 255.0) * 255), (int)((noammo_color.B / 255.0) * 255)));
                }

                //Update Bomb
                if (this.bombstate == BombState.Planted)
                {
                    if (!bombtimer.IsRunning)
                    {
                        bombtimer.Restart();
                        bombflashcount = 0;
                        bombflashtime = 0;
                        bombflashedat = 0;
                    }

                    double bombflashamount = 1.0;
                    bool isCritical = false;


                    if (bombtimer.ElapsedMilliseconds < 38000)
                    {

                        if (bombtimer.ElapsedMilliseconds >= bombflashtime)
                        {
                            bombflash = true;
                            bombflashedat = bombtimer.ElapsedMilliseconds;
                            bombflashtime = bombtimer.ElapsedMilliseconds + (1000 - (bombflashcount++ * 13));
                            //Console.WriteLine("Next flash at: " + bombflashtime + ", deviation: " + (1000 - (bombflashcount * 13)));
                        }

                        bombflashamount = Math.Pow(Math.Sin((bombtimer.ElapsedMilliseconds - bombflashedat) / 80.0 + 0.25), 2.0);
                        //Console.WriteLine("Flash amount: " + bombflashamount);

                    }
                    else if (bombtimer.ElapsedMilliseconds >= 38000)
                    {
                        isCritical = true;
                        bombflashamount = (double)bombtimer.ElapsedMilliseconds / 40000.0;
                    }

                    /*
                    if (bombtimer.ElapsedMilliseconds < 30000)
                        bombflashamount = Math.Pow(Math.Sin(2.0 * (bombtimer.ElapsedMilliseconds / 1000.0)), 2.0);
                    else if (bombtimer.ElapsedMilliseconds > 26000 && bombtimer.ElapsedMilliseconds < 37000)
                        bombflashamount = Math.Pow(Math.Sin(2.0 * (bombtimer.ElapsedMilliseconds / 500.0)), 2.0);
                    else if (bombtimer.ElapsedMilliseconds > 37000)
                    {
                        isCritical = true;
                        bombflashamount = (double)bombtimer.ElapsedMilliseconds / 40000.0;
                    }
                    else
                        bombtimer.Stop();
                    */

                    if (!isCritical)
                    {
                        if (bombflashamount <= 0.05 && bombflash)
                            bombflash = false;

                        if (!bombflash)
                            bombflashamount = 0.0;
                    }

                    if (!gradualbombtimer)
                        bombflashamount = Math.Round(bombflashamount);

                    foreach (keyboardBitmapKeys key in bombKeys)
                    {
                        if (isCritical)
                            SetOneKey(key, Color.FromArgb(0, (int)(255 * Math.Min(bombflashamount, 1.0)), 0));
                        else
                            SetOneKey(key, Color.FromArgb((int)(255 * Math.Min(bombflashamount, 1.0)), 0, 0));
                    }
                }
                else if (this.bombstate == BombState.Defused)
                {
                    bombtimer.Stop();
                    foreach (keyboardBitmapKeys key in bombKeys)
                        SetOneKey(key, ct_color);
                }
                else if (this.bombstate == BombState.Exploded)
                {
                    bombtimer.Stop();
                    foreach (keyboardBitmapKeys key in bombKeys)
                        SetOneKey(key, t_color);
                }
                else
                {
                    bombtimer.Stop();
                }
            }

            //Restore Saved Keys
            foreach (keyboardBitmapKeys key in saveKeys)
                SetOneKey(key, savekey_color);

            //Update Flashed
            if (flashamount > 0)
                SetAllKeysEffect(Color.FromArgb(this.flashamount, this.flashamount, this.flashamount));

            SendColorsToKeyboard();

            stopwatch.Stop();
            //Console.WriteLine("Execution time: " + stopwatch.ElapsedMilliseconds);
        }

        private void SendColorsToKeyboard()
        {
            LogitechGSDK.LogiLedSetLightingFromBitmap(bitmap);
            keyboard_updated = true;
        }
    }
}
