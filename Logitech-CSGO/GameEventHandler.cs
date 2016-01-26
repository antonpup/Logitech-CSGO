using CSGSI.Nodes;
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

        private static Dictionary<Devices.DeviceKeys, Color> keyColors = new Dictionary<Devices.DeviceKeys, Color>();

        private static Dictionary<Devices.DeviceKeys, Color> final_keyColors = new Dictionary<Devices.DeviceKeys, Color>();

        private DeviceManager dev_manager = new DeviceManager();

        private bool keyboard_updated = false;
        private Timer update_timer;

        private Stopwatch animations_time = Stopwatch.StartNew();
        private Random randomizer = new Random();

        //Bomb stuff
        private Stopwatch bombtimer = new Stopwatch();
        private bool bombflash = false;
        private int bombflashcount = 0;
        private long bombflashtime = 0;
        private long bombflashedat = 0;

        private bool preview_mode = false;

        //Game Integration stuff
        PlayerTeam current_team = PlayerTeam.Undefined;
        int health = 0;
        int health_max = 100;
        int clip = 0;
        int clip_max = 100;
        BombState bombstate = BombState.Undefined;
        int flashamount = 0;
        int burnamount = 0;
        PlayerActivity current_activity = PlayerActivity.Undefined;



        public GameEventHandler()
        {
            Devices.DeviceKeys[] allKeys = Enum.GetValues(typeof(Devices.DeviceKeys)).Cast<Devices.DeviceKeys>().ToArray();
            
            foreach(Devices.DeviceKeys key in allKeys)
            {
                keyColors.Add(key, Color.Black);
            }
        }

        ~GameEventHandler()
        {
            Destroy();
        }
        
        public bool Init()
        {
            bool devices_inited = dev_manager.Initialize();

            if(devices_inited)
            {
                update_timer = new Timer(10);
                update_timer.Elapsed += new ElapsedEventHandler(update_timer_Tick);
                update_timer.Interval = 10; // in miliseconds
                update_timer.Start();

                animations_time.Start();
            }

            return devices_inited;
        }

        public void Destroy()
        {
            update_timer.Stop();
            bombtimer.Stop();
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
                    dev_manager.ResetDevices();
                }
            }
           
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

        public void SetBurnAmount(int burn)
        {
            this.burnamount = burn;
        }

        public void SetPlayerActivity(PlayerActivity activity)
        {
            this.current_activity = activity;
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
                        SetOneKey(Devices.DeviceKeys.Peripheral, Global.Configuration.t_color);
                }
                else if (this.current_team == PlayerTeam.CT)
                {
                    SetAllKeys(Global.Configuration.ct_color);
                    if (Global.Configuration.bg_peripheral_use)
                        SetOneKey(Devices.DeviceKeys.Peripheral, Global.Configuration.ct_color);
                }
                else
                {
                    SetAllKeys(Global.Configuration.ambient_color);
                    if (Global.Configuration.bg_peripheral_use)
                        SetOneKey(Devices.DeviceKeys.Peripheral, Global.Configuration.ambient_color);
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
                    Devices.DeviceKeys[] _bombkeys = Global.Configuration.bombKeys.ToArray();

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

                        foreach (Devices.DeviceKeys key in _bombkeys)
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
                                    SetOneKey(Devices.DeviceKeys.Peripheral, bombcolor_critical);
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
                                    SetOneKey(Devices.DeviceKeys.Peripheral, bombcolor);
                                }
                            }
                        }
                    }
                    else if (this.bombstate == BombState.Defused)
                    {
                        bombtimer.Stop();
                        if (Global.Configuration.bomb_display_winner_color)
                        {
                            foreach (Devices.DeviceKeys key in _bombkeys)
                                SetOneKey(key, Global.Configuration.ct_color);

                            if (Global.Configuration.bomb_peripheral_use)
                                SetOneKey(Devices.DeviceKeys.Peripheral, Global.Configuration.ct_color);
                        }
                    }
                    else if (this.bombstate == BombState.Exploded)
                    {
                        bombtimer.Stop();
                        if (Global.Configuration.bomb_display_winner_color)
                        {
                            foreach (Devices.DeviceKeys key in _bombkeys)
                                SetOneKey(key, Global.Configuration.t_color);
                            if (Global.Configuration.bomb_peripheral_use)
                                SetOneKey(Devices.DeviceKeys.Peripheral, Global.Configuration.t_color);
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
                Devices.DeviceKeys[] _statickeys = Global.Configuration.staticKeys.ToArray();
                foreach (Devices.DeviceKeys key in _statickeys)
                    SetOneKey(key, Global.Configuration.statickeys_color);
            }

            //Update Burning
            if (Global.Configuration.burning_enabled && burnamount > 0)
            {
                double burning_percent = (double)this.burnamount / 255.0;
                Color burncolor = Global.Configuration.burning_color;

                if (Global.Configuration.burning_animation)
                {
                    int green_adjusted = (int)(Global.Configuration.burning_color.G + (Math.Cos((animations_time.ElapsedMilliseconds + randomizer.Next(150)) / 75.0) * 0.15 * 255));
                    byte green = 0;

                    if(green_adjusted > 255)
                        green = 255;
                    else if(green_adjusted < 0)
                        green = 0;
                    else 
                        green = (byte)green_adjusted;

                    burncolor = Color.FromArgb(burncolor.R, green, burncolor.B);
                }

                SetAllKeysEffect(burncolor, burning_percent);

                if (Global.Configuration.burning_peripheral_use)
                    SetOneKey(Devices.DeviceKeys.Peripheral, BlendColors(GetOneKey(Devices.DeviceKeys.Peripheral), burncolor, burning_percent));
            }

            //Update Flashed
            if (Global.Configuration.flashbang_enabled && flashamount > 0)
            {
                double flash_percent = (double)this.flashamount / 255.0;
                SetAllKeysEffect(Global.Configuration.flash_color, flash_percent);

                if (Global.Configuration.flashbang_peripheral_use)
                    SetOneKey(Devices.DeviceKeys.Peripheral, BlendColors(GetOneKey(Devices.DeviceKeys.Peripheral), Global.Configuration.flash_color, flash_percent));
            }

            //Update Typing Keys
            if (Global.Configuration.typing_enabled && current_activity == PlayerActivity.TextInput)
            {
                Devices.DeviceKeys[] _typingkeys = Global.Configuration.typingKeys.ToArray();
                foreach (Devices.DeviceKeys key in _typingkeys)
                    SetOneKey(key, Global.Configuration.typing_color);
            }

            keyboard_updated = dev_manager.UpdateDevices(keyColors);

            final_keyColors = keyColors;

            stopwatch.Stop();
            //Console.WriteLine("Execution time: " + stopwatch.ElapsedMilliseconds);
        }

        private void SetAllKeysEffect(Color color, double percent)
        {
            foreach (Devices.DeviceKeys key in keyColors.Keys.ToArray())
            {
                Color keycolor = GetOneKey(key);

                SetOneKey(key, BlendColors(keycolor, color, percent));
            }
        }

        public Dictionary<Devices.DeviceKeys, System.Drawing.Color> GetKeyboardLights()
        {
            return final_keyColors;
        }

        public Color BlendColors(Color background, Color foreground, double percent)
        {
            byte r = (byte)Math.Min((Int32)foreground.R * percent + (Int32)background.R * (1.0 - percent), 255);
            byte g = (byte)Math.Min((Int32)foreground.G * percent + (Int32)background.G * (1.0 - percent), 255);
            byte b = (byte)Math.Min((Int32)foreground.B * percent + (Int32)background.B * (1.0 - percent), 255);

            return Color.FromArgb(r, g, b);
        }

        private Color GetOneKey(Devices.DeviceKeys key)
        {
            Color ret = Color.Black;

            if(keyColors.ContainsKey(key))
                ret = keyColors[key];

            return ret;
        }

        private void SetAllKeys(Color color)
        {
            foreach (Devices.DeviceKeys key in keyColors.Keys.ToArray())
            {
                SetOneKey(key, color);
            }
        }

        private void SetOneKey(Devices.DeviceKeys key, Color color)
        {
            keyColors[key] = color;
        }

        private void PercentEffect(Color foregroundColor, Color backgroundColor, Devices.DeviceKeys[] keys, double value, double total, PercentEffectType effectType = PercentEffectType.Progressive)
        {
            double progress_total = value / total;
            if (progress_total < 0.0)
                progress_total = 0.0;
            else if (progress_total > 1.0)
                progress_total = 1.0;

            double progress = progress_total * keys.Count();

            for (int i = 0; i < keys.Count(); i++)
            {
                Devices.DeviceKeys current_key = keys[i];

                switch (effectType)
                {
                    case (PercentEffectType.AllAtOnce):
                        SetOneKey(current_key, Color.FromArgb(
                                (Int32)Math.Min((Int32)foregroundColor.R * progress_total + (Int32)backgroundColor.R * (1.0 - progress_total), 255),
                                (Int32)Math.Min((Int32)foregroundColor.G * progress_total + (Int32)backgroundColor.G * (1.0 - progress_total), 255),
                                (Int32)Math.Min((Int32)foregroundColor.B * progress_total + (Int32)backgroundColor.B * (1.0 - progress_total), 255)
                            ));
                        break;
                    case (PercentEffectType.Progressive_Gradual):
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
