using LedCSharp;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logitech_CSGO
{
    public enum PercentEffectType
    {
        AllAtOnce = 0,
        Progressive = 1,
        Progressive_Gradual = 2
    }

    public class Configuration
    {
        public Configuration()
        {
            //Effects
            //// Background
            bg_team_enabled = true;
            ct_color = Color.FromArgb(158, 205, 255);
            t_color = Color.FromArgb(221, 99, 33);
            ambient_color = Color.FromArgb(158, 205, 255);
            bg_peripheral_use = true;

            //// Health
            health_enabled = true;
            healthy_color = Color.FromArgb(0, 255, 0);
            hurt_color = Color.FromArgb(255, 0, 0);
            health_effect_type = PercentEffectType.Progressive_Gradual;
            healthKeys = new List<Devices.DeviceKeys>() { Devices.DeviceKeys.F1, Devices.DeviceKeys.F2, Devices.DeviceKeys.F3, Devices.DeviceKeys.F4, Devices.DeviceKeys.F5, Devices.DeviceKeys.F6, Devices.DeviceKeys.F7, Devices.DeviceKeys.F8, Devices.DeviceKeys.F9, Devices.DeviceKeys.F10, Devices.DeviceKeys.F11, Devices.DeviceKeys.F12 };

            //// Ammo
            ammo_enabled = true;
            ammo_color = Color.FromArgb(0, 0, 255);
            noammo_color = Color.FromArgb(255, 0, 0);
            ammo_effect_type = PercentEffectType.Progressive;
            ammoKeys = new List<Devices.DeviceKeys>() { Devices.DeviceKeys.ONE, Devices.DeviceKeys.TWO, Devices.DeviceKeys.THREE, Devices.DeviceKeys.FOUR, Devices.DeviceKeys.FIVE, Devices.DeviceKeys.SIX, Devices.DeviceKeys.SEVEN, Devices.DeviceKeys.EIGHT, Devices.DeviceKeys.NINE, Devices.DeviceKeys.ZERO, Devices.DeviceKeys.MINUS, Devices.DeviceKeys.EQUALS };

            //// Bomb
            bomb_enabled = true;
            bomb_flash_color = Color.FromArgb(255, 0, 0);
            bomb_primed_color = Color.FromArgb(0, 255, 0);
            bomb_display_winner_color = true;
            bomb_gradual = true;
            bombKeys = new List<Devices.DeviceKeys>() { Devices.DeviceKeys.NUM_LOCK, Devices.DeviceKeys.NUM_SLASH, Devices.DeviceKeys.NUM_ASTERISK, Devices.DeviceKeys.NUM_MINUS, Devices.DeviceKeys.NUM_SEVEN, Devices.DeviceKeys.NUM_EIGHT, Devices.DeviceKeys.NUM_NINE, Devices.DeviceKeys.NUM_PLUS, Devices.DeviceKeys.NUM_FOUR, Devices.DeviceKeys.NUM_FIVE, Devices.DeviceKeys.NUM_SIX, Devices.DeviceKeys.NUM_ONE, Devices.DeviceKeys.NUM_TWO, Devices.DeviceKeys.NUM_THREE, Devices.DeviceKeys.NUM_ZERO, Devices.DeviceKeys.NUM_PERIOD, Devices.DeviceKeys.NUM_ENTER };
            bomb_peripheral_use = true;

            //// Static Keys
            statickeys_enabled = true;
            statickeys_color = Color.FromArgb(255, 220, 0);
            staticKeys = new List<Devices.DeviceKeys>() { Devices.DeviceKeys.W, Devices.DeviceKeys.A, Devices.DeviceKeys.S, Devices.DeviceKeys.D, Devices.DeviceKeys.LEFT_SHIFT, Devices.DeviceKeys.LEFT_CONTROL, Devices.DeviceKeys.SPACE };

            //// Burning
            burning_enabled = true;
            burning_color = Color.FromArgb(255, 70, 0);
            burning_animation = true;
            burning_peripheral_use = true;

            //// Flashbang
            flashbang_enabled = true;
            flash_color = Color.FromArgb(255, 255, 255);
            flashbang_peripheral_use = true;

            ////Typing Keys
            typing_enabled = true;
            typing_color = Color.FromArgb(0, 255, 0);
            typingKeys = new List<Devices.DeviceKeys>() { Devices.DeviceKeys.TILDE, Devices.DeviceKeys.ONE, Devices.DeviceKeys.TWO, Devices.DeviceKeys.THREE, Devices.DeviceKeys.FOUR, Devices.DeviceKeys.FIVE, Devices.DeviceKeys.SIX, Devices.DeviceKeys.SEVEN, Devices.DeviceKeys.EIGHT, Devices.DeviceKeys.NINE, Devices.DeviceKeys.ZERO, Devices.DeviceKeys.MINUS, Devices.DeviceKeys.EQUALS, Devices.DeviceKeys.BACKSPACE, 
                                                    Devices.DeviceKeys.TAB, Devices.DeviceKeys.Q, Devices.DeviceKeys.W, Devices.DeviceKeys.E, Devices.DeviceKeys.R, Devices.DeviceKeys.T, Devices.DeviceKeys.Y, Devices.DeviceKeys.U, Devices.DeviceKeys.I, Devices.DeviceKeys.O, Devices.DeviceKeys.P, Devices.DeviceKeys.CLOSE_BRACKET, Devices.DeviceKeys.OPEN_BRACKET, Devices.DeviceKeys.BACKSLASH, 
                                                    Devices.DeviceKeys.CAPS_LOCK, Devices.DeviceKeys.A, Devices.DeviceKeys.S, Devices.DeviceKeys.D, Devices.DeviceKeys.F, Devices.DeviceKeys.G, Devices.DeviceKeys.H, Devices.DeviceKeys.J, Devices.DeviceKeys.K, Devices.DeviceKeys.L, Devices.DeviceKeys.SEMICOLON, Devices.DeviceKeys.APOSTROPHE, Devices.DeviceKeys.HASHTAG, Devices.DeviceKeys.ENTER, 
                                                    Devices.DeviceKeys.LEFT_SHIFT, Devices.DeviceKeys.BACKSLASH_UK, Devices.DeviceKeys.Z, Devices.DeviceKeys.X, Devices.DeviceKeys.C, Devices.DeviceKeys.V, Devices.DeviceKeys.B, Devices.DeviceKeys.N, Devices.DeviceKeys.M, Devices.DeviceKeys.COMMA, Devices.DeviceKeys.PERIOD, Devices.DeviceKeys.FORWARD_SLASH, Devices.DeviceKeys.RIGHT_SHIFT,
                                                    Devices.DeviceKeys.LEFT_CONTROL, Devices.DeviceKeys.LEFT_WINDOWS, Devices.DeviceKeys.LEFT_ALT, Devices.DeviceKeys.SPACE, Devices.DeviceKeys.RIGHT_ALT, Devices.DeviceKeys.RIGHT_WINDOWS, Devices.DeviceKeys.APPLICATION_SELECT, Devices.DeviceKeys.RIGHT_CONTROL,
                                                    Devices.DeviceKeys.ARROW_UP, Devices.DeviceKeys.ARROW_LEFT, Devices.DeviceKeys.ARROW_DOWN, Devices.DeviceKeys.ARROW_RIGHT, Devices.DeviceKeys.ESC
                                                  };
        }

        //Effects
        //// Background
        public bool bg_team_enabled;
        public Color ct_color;
        public Color t_color;
        public Color ambient_color;
        public bool bg_peripheral_use;

        //// Health
        public bool health_enabled;
        public Color healthy_color;
        public Color hurt_color;
        public PercentEffectType health_effect_type;
        public List<Devices.DeviceKeys> healthKeys { get; set; }

        //// Ammo
        public bool ammo_enabled;
        public Color ammo_color;
        public Color noammo_color;
        public PercentEffectType ammo_effect_type;
        public List<Devices.DeviceKeys> ammoKeys { get; set; }

        //// Bomb
        public bool bomb_enabled;
        public Color bomb_flash_color;
        public Color bomb_primed_color;
        public bool bomb_display_winner_color;
        public bool bomb_gradual;
        public List<Devices.DeviceKeys> bombKeys { get; set; }
        public bool bomb_peripheral_use;

        //// Static Keys
        public bool statickeys_enabled;
        public Color statickeys_color;
        public List<Devices.DeviceKeys> staticKeys { get; set; }

        //// Burning
        public bool burning_enabled;
        public Color burning_color;
        public bool burning_animation;
        public bool burning_peripheral_use;

        //// Flashbang
        public bool flashbang_enabled;
        public Color flash_color;
        public bool flashbang_peripheral_use;

        ////Typing Keys
        public bool typing_enabled;
        public Color typing_color;
        public List<Devices.DeviceKeys> typingKeys { get; set; }

    }

    public class ConfigManager
    {
        private const string ConfigExtension = ".json";

        public static Configuration Load(string fileNameWithoutExtension)
        {
            var configPath = fileNameWithoutExtension + ConfigExtension;

            if (!File.Exists(configPath))
                return CreateDefaultConfigurationFile(fileNameWithoutExtension);

            string content = File.ReadAllText(configPath, Encoding.UTF8);
            return JsonConvert.DeserializeObject<Configuration>(content, new JsonSerializerSettings { ObjectCreationHandling = ObjectCreationHandling.Replace });
        }

        public static void Save(string fileNameWithoutExtension, Configuration configuration)
        {
            var configPath = fileNameWithoutExtension + ConfigExtension;
            string content = JsonConvert.SerializeObject(configuration, Formatting.Indented);

            File.WriteAllText(configPath, content, Encoding.UTF8);
        }

        private static Configuration CreateDefaultConfigurationFile(string fileNameWithoutExtension)
        {
            Configuration config = new Configuration();
            var configData = JsonConvert.SerializeObject(config, Formatting.Indented);
            var configPath = fileNameWithoutExtension + ConfigExtension;

            File.WriteAllText(configPath, configData, Encoding.UTF8);

            return config;
        }
    }
}
