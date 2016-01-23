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
    public class GameEventHandler
    {
        private bool isInitialized = false;
        
        private bool keyboard_updated = false;
        private Timer update_timer;

        private keyboardBitmapKeys[] allKeys = Enum.GetValues(typeof(keyboardBitmapKeys)).Cast<keyboardBitmapKeys>().ToArray();
        
        //Bomb stuff
        private Stopwatch bombtimer = new Stopwatch();
        private bool bombflash = false;
        private int bombflashcount = 0;
        private long bombflashtime = 0;
        private long bombflashedat = 0;

        //Keyboard stuff
        private byte[] bitmap = new byte[LogitechGSDK.LOGI_LED_BITMAP_SIZE];
        private Color peripheral_Color = Color.Black;
        private bool preview_mode = false;

        //Game Integration stuff
        PlayerTeam current_team = PlayerTeam.Undefined;
        int health = 0;
        int health_max = 100;
        int clip = 0;
        int clip_max = 100;
        BombState bombstate = BombState.Undefined;
        int flashamount = 0;
        PlayerActivity current_activity = PlayerActivity.Undefined;

        ~GameEventHandler()
        {
            Destroy();
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
            catch(Exception exc)
            {
                System.Windows.MessageBox.Show("There was an error initializing Logitech LED SDK.\r\n" + exc.Message);

                return false;
            }
        }

        public void Destroy()
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
            if (GetActiveWindowsProcessname().ToLowerInvariant().EndsWith("\\csgo.exe") || preview_mode)
            {
                UpdateKeyboard();
            }
            else
            {
                if (keyboard_updated)
                {
                    LogitechGSDK.LogiLedSetTargetDevice(LogitechGSDK.LOGI_DEVICETYPE_RGB | LogitechGSDK.LOGI_DEVICETYPE_PERKEY_RGB);
                    LogitechGSDK.LogiLedRestoreLighting();
                }
            }
           
        }

        private Color BlendColors(Color background, Color foreground, double percent)
        {
            byte r = (byte)Math.Min((Int32)foreground.R * percent + (Int32)background.R * (1.0 - percent), 255);
            byte g = (byte)Math.Min((Int32)foreground.G * percent + (Int32)background.G * (1.0 - percent), 255);
            byte b = (byte)Math.Min((Int32)foreground.B * percent + (Int32)background.B * (1.0 - percent), 255);

            return Color.FromArgb(r, g, b);
        }

        private void SetAllKeys(Color color)
        {
            foreach (keyboardBitmapKeys key in allKeys)
                SetOneKey(key, color);
        }

        private void SetAllKeysEffect(Color color, double percent)
        {
            foreach (keyboardBitmapKeys key in allKeys)
            {
                Color keycolor = GetOneKey(key);

                SetOneKey(key, BlendColors(keycolor, color, percent));
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

        private Color GetPeripheralColor()
        {
            return this.peripheral_Color;
        }

        private void SetPeripheralColor(Color color)
        {
            this.peripheral_Color = color;
        }

        public void SetPreview(bool preview)
        {
            this.preview_mode = preview;
        }

        public void SetTeam(PlayerTeam team)
        {
            this.current_team = team;
        }

        public void SetHealth(int health)
        {
            this.health = health;
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

        public void SetPlayerActivity(PlayerActivity activity)
        {
            this.current_activity = activity;
        }

        public Dictionary<keyboardBitmapKeys, Color> GetKeyboardLights()
        {
            Dictionary<keyboardBitmapKeys, Color> keycolors = new Dictionary<keyboardBitmapKeys, Color>();

            foreach (keyboardBitmapKeys key in allKeys)
            {
                keycolors.Add(key, Color.FromArgb((int)bitmap[(int)key + 3], (int)bitmap[(int)key + 2], (int)bitmap[(int)key + 1], (int)bitmap[(int)key]));
            }

            return keycolors;
        }

        public void UpdateKeyboard()
        {
            Stopwatch stopwatch = Stopwatch.StartNew();

            //update background
            if (Global.Configuration.bg_team_enabled)
            {
                if (this.current_team == PlayerTeam.T)
                {
                    SetAllKeys(Global.Configuration.t_color);
                    if (Global.Configuration.bg_peripheral_use)
                        SetPeripheralColor(Global.Configuration.t_color);
                }
                else if (this.current_team == PlayerTeam.CT)
                {
                    SetAllKeys(Global.Configuration.ct_color);
                    if (Global.Configuration.bg_peripheral_use)
                        SetPeripheralColor(Global.Configuration.ct_color);
                }
                else
                {
                    SetAllKeys(Global.Configuration.ambient_color);
                    if (Global.Configuration.bg_peripheral_use)
                        SetPeripheralColor(Global.Configuration.ambient_color);
                }
            }
            else
            {
                SetAllKeys(Color.Black);
            }

            //Not initialized
            if (this.current_team != PlayerTeam.Undefined)
            {
                //Update Health
                if(Global.Configuration.health_enabled)
                    PercentEffect(Global.Configuration.healthy_color, Global.Configuration.hurt_color, Global.Configuration.healthKeys.ToArray(), (double)this.health, (double)this.health_max, Global.Configuration.health_effect_type);

                //Update Ammo
                if (Global.Configuration.ammo_enabled)
                    PercentEffect(Global.Configuration.ammo_color, Global.Configuration.noammo_color, Global.Configuration.ammoKeys.ToArray(), (double)this.clip, (double)this.clip_max, Global.Configuration.ammo_effect_type);


                //Update Bomb
                if (Global.Configuration.bomb_enabled)
                {
                    keyboardBitmapKeys[] _bombkeys = Global.Configuration.bombKeys.ToArray();

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
                        else if (bombtimer.ElapsedMilliseconds >= 45000)
                        {
                            bombtimer.Stop();
                            this.bombstate = BombState.Undefined;
                        }

                        if (!isCritical)
                        {
                            if (bombflashamount <= 0.05 && bombflash)
                                bombflash = false;

                            if (!bombflash)
                                bombflashamount = 0.0;
                        }

                        if (!Global.Configuration.bomb_gradual)
                            bombflashamount = Math.Round(bombflashamount);

                        foreach (keyboardBitmapKeys key in _bombkeys)
                        {
                            if (isCritical)
                            {
                                Color bombcolor_critical = Color.FromArgb(
                                    (Int32)((Int32)Global.Configuration.bomb_primed_color.R * Math.Min(bombflashamount, 1.0)),
                                    (Int32)((Int32)Global.Configuration.bomb_primed_color.G * Math.Min(bombflashamount, 1.0)),
                                    (Int32)((Int32)Global.Configuration.bomb_primed_color.B * Math.Min(bombflashamount, 1.0))
                                    );

                                SetOneKey(key, bombcolor_critical);

                                if (Global.Configuration.bomb_peripheral_use)
                                {
                                    SetPeripheralColor(bombcolor_critical);
                                }
                            }
                            else
                            {
                                Color bombcolor = Color.FromArgb(
                                    (Int32)((Int32)Global.Configuration.bomb_flash_color.R * Math.Min(bombflashamount, 1.0)),
                                    (Int32)((Int32)Global.Configuration.bomb_flash_color.G * Math.Min(bombflashamount, 1.0)),
                                    (Int32)((Int32)Global.Configuration.bomb_flash_color.B * Math.Min(bombflashamount, 1.0))
                                    );

                                SetOneKey(key, bombcolor);
                                if (Global.Configuration.bomb_peripheral_use)
                                {
                                    SetPeripheralColor(bombcolor);
                                }
                            }
                        }
                    }
                    else if (this.bombstate == BombState.Defused)
                    {
                        bombtimer.Stop();
                        if (Global.Configuration.bomb_display_winner_color)
                        {
                            foreach (keyboardBitmapKeys key in _bombkeys)
                                SetOneKey(key, Global.Configuration.ct_color);

                            if (Global.Configuration.bomb_peripheral_use)
                                SetPeripheralColor(Global.Configuration.ct_color);
                        }
                    }
                    else if (this.bombstate == BombState.Exploded)
                    {
                        bombtimer.Stop();
                        if (Global.Configuration.bomb_display_winner_color)
                        {
                            foreach (keyboardBitmapKeys key in _bombkeys)
                                SetOneKey(key, Global.Configuration.t_color);
                            if (Global.Configuration.bomb_peripheral_use)
                                SetPeripheralColor(Global.Configuration.t_color);
                        }
                    }
                    else
                    {
                        bombtimer.Stop();
                    }
                }
            }

            //Restore Saved Keys
            if (Global.Configuration.statickeys_enabled)
            {
                keyboardBitmapKeys[] _statickeys = Global.Configuration.staticKeys.ToArray();
                foreach (keyboardBitmapKeys key in _statickeys)
                    SetOneKey(key, Global.Configuration.statickeys_color);
            }

            //Update Flashed
            if (Global.Configuration.flashbang_enabled && flashamount > 0)
            {
                double flash_percent = (double)this.flashamount / 255.0;
                SetAllKeysEffect(Global.Configuration.flash_color, flash_percent);

                if (Global.Configuration.flashbang_peripheral_use)
                    SetPeripheralColor(BlendColors(GetPeripheralColor(), Global.Configuration.flash_color, flash_percent));
            }

            //Update Typing Keys
            if (Global.Configuration.typing_enabled && current_activity == PlayerActivity.TextInput)
            {
                keyboardBitmapKeys[] _typingkeys = Global.Configuration.typingKeys.ToArray();
                foreach (keyboardBitmapKeys key in _typingkeys)
                    SetOneKey(key, Global.Configuration.typing_color);
            }

            SendColorsToKeyboard();

            SendColorToPeripheral();

            stopwatch.Stop();
            //Console.WriteLine("Execution time: " + stopwatch.ElapsedMilliseconds);
        }

        private void SendColorsToKeyboard()
        {
            LogitechGSDK.LogiLedSetTargetDevice(LogitechGSDK.LOGI_DEVICETYPE_PERKEY_RGB);

            LogitechGSDK.LogiLedSetLightingFromBitmap(bitmap);
            keyboard_updated = true;
        }

        private void SendColorToPeripheral()
        {
            LogitechGSDK.LogiLedSetTargetDevice(LogitechGSDK.LOGI_DEVICETYPE_RGB);

            if (!Global.Configuration.bg_peripheral_use)
            {
                LogitechGSDK.LogiLedRestoreLighting();
                return;
            }

            LogitechGSDK.LogiLedSetLighting( (int)this.peripheral_Color.R/255,(int)this.peripheral_Color.G/255, (int)this.peripheral_Color.B/255 );
        }

        private void PercentEffect(Color foregroundColor, Color backgroundColor, keyboardBitmapKeys[] keys, double value, double total, PercentEffectType effectType = PercentEffectType.Progressive)
        {
            double progress_total = value / total;
            if (progress_total < 0.0)
                progress_total = 0.0;
            else if(progress_total > 1.0)
                progress_total = 1.0;

            double progress = progress_total * keys.Count();

            for (int i = 0; i < keys.Count(); i++)
            {
                keyboardBitmapKeys current_key = keys[i];

                switch(effectType)
                {
                    case(PercentEffectType.AllAtOnce):
                        SetOneKey(current_key, Color.FromArgb(
                                (Int32)Math.Min((Int32)foregroundColor.R * progress_total + (Int32)backgroundColor.R * (1.0 - progress_total), 255),
                                (Int32)Math.Min((Int32)foregroundColor.G * progress_total + (Int32)backgroundColor.G * (1.0 - progress_total), 255),
                                (Int32)Math.Min((Int32)foregroundColor.B * progress_total + (Int32)backgroundColor.B * (1.0 - progress_total), 255)
                            ));
                        break;
                    case(PercentEffectType.Progressive_Gradual):
                        if (i == (int)progress)
                        {
                            double percent = (double)progress - i;
                            SetOneKey(current_key, Color.FromArgb(
                                (Int32)Math.Min((Int32)foregroundColor.R * percent + (Int32)backgroundColor.R * (1.0 - percent), 255),
                                (Int32)Math.Min((Int32)foregroundColor.G * percent + (Int32)backgroundColor.G * (1.0 - percent), 255),
                                (Int32)Math.Min((Int32)foregroundColor.B * percent + (Int32)backgroundColor.B * (1.0 - percent), 255)
                                ));
                        }
                        else if (i < (int)progress)
                            SetOneKey(current_key, foregroundColor);
                        else
                            SetOneKey(current_key, backgroundColor);
                        break;
                    default:
                        if (i < (int)progress)
                            SetOneKey(current_key, foregroundColor);
                        else
                            SetOneKey(current_key, backgroundColor);
                        break;
                }
            }
        }
    }
}
