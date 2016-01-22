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
            healthKeys = new List<keyboardBitmapKeys>();
            ammoKeys = new List<keyboardBitmapKeys>();
            bombKeys = new List<keyboardBitmapKeys>();
            staticKeys = new List<keyboardBitmapKeys>();
        }

        public void DefaultValues()
        {
            detect_csgo = true;
            
            //// Background
            bg_team_enabled = true;
            ct_color = Color.FromArgb(158, 205, 255);
            t_color = Color.FromArgb(221, 99, 33);

            //// Health
            health_enabled = true;
            healthy_color = Color.FromArgb(0, 255, 0);
            hurt_color = Color.FromArgb(255, 0, 0);
            health_effect_type = PercentEffectType.Progressive_Gradual;
            healthKeys = new List<keyboardBitmapKeys>() { keyboardBitmapKeys.F1, keyboardBitmapKeys.F2, keyboardBitmapKeys.F3, keyboardBitmapKeys.F4, keyboardBitmapKeys.F5, keyboardBitmapKeys.F6, keyboardBitmapKeys.F7, keyboardBitmapKeys.F8, keyboardBitmapKeys.F9, keyboardBitmapKeys.F10, keyboardBitmapKeys.F11, keyboardBitmapKeys.F12 };
            
            //// Ammo
            ammo_enabled = true;
            ammo_color = Color.FromArgb(0, 0, 255);
            noammo_color = Color.FromArgb(255, 0, 0);
            ammo_effect_type = PercentEffectType.Progressive;
            ammoKeys = new List<keyboardBitmapKeys>() { keyboardBitmapKeys.ONE, keyboardBitmapKeys.TWO, keyboardBitmapKeys.THREE, keyboardBitmapKeys.FOUR, keyboardBitmapKeys.FIVE, keyboardBitmapKeys.SIX, keyboardBitmapKeys.SEVEN, keyboardBitmapKeys.EIGHT, keyboardBitmapKeys.NINE, keyboardBitmapKeys.ZERO, keyboardBitmapKeys.MINUS, keyboardBitmapKeys.EQUALS };
            
            //// Bomb
            bomb_enabled = true;
            bomb_flash_color = Color.FromArgb(255, 0, 0);
            bomb_primed_color = Color.FromArgb(0, 255, 0);
            bomb_display_winner_color = true;
            bomb_gradual = true;
            bombKeys = new List<keyboardBitmapKeys>() { keyboardBitmapKeys.NUM_LOCK, keyboardBitmapKeys.NUM_SLASH, keyboardBitmapKeys.NUM_ASTERISK, keyboardBitmapKeys.NUM_MINUS, keyboardBitmapKeys.NUM_SEVEN, keyboardBitmapKeys.NUM_EIGHT, keyboardBitmapKeys.NUM_NINE, keyboardBitmapKeys.NUM_PLUS, keyboardBitmapKeys.NUM_FOUR, keyboardBitmapKeys.NUM_FIVE, keyboardBitmapKeys.NUM_SIX, keyboardBitmapKeys.NUM_ONE, keyboardBitmapKeys.NUM_TWO, keyboardBitmapKeys.NUM_THREE, keyboardBitmapKeys.NUM_ZERO, keyboardBitmapKeys.NUM_PERIOD, keyboardBitmapKeys.NUM_ENTER };
            
            //// Static Keys
            statickeys_enabled = true;
            statickeys_color = Color.FromArgb(255, 220, 0);
            staticKeys = new List<keyboardBitmapKeys>() { keyboardBitmapKeys.W, keyboardBitmapKeys.A, keyboardBitmapKeys.S, keyboardBitmapKeys.D, keyboardBitmapKeys.LEFT_SHIFT, keyboardBitmapKeys.LEFT_CONTROL, keyboardBitmapKeys.SPACE };

            //// Flashbang
            flashbang_enabled = true;
            flash_color = Color.FromArgb(255, 255, 255);

        }

        public bool detect_csgo;

        //Effects
        //// Background
        public bool bg_team_enabled;
        public Color ct_color;
        public Color t_color;

        //// Health
        public bool health_enabled;
        public Color healthy_color;
        public Color hurt_color;
        public PercentEffectType health_effect_type;
        public List<keyboardBitmapKeys> healthKeys { get; set; }

        //// Ammo
        public bool ammo_enabled;
        public Color ammo_color;
        public Color noammo_color;
        public PercentEffectType ammo_effect_type;
        public List<keyboardBitmapKeys> ammoKeys { get; set; }

        //// Bomb
        public bool bomb_enabled;
        public Color bomb_flash_color;
        public Color bomb_primed_color;
        public bool bomb_display_winner_color;
        public bool bomb_gradual;
        public List<keyboardBitmapKeys> bombKeys { get; set; }

        //// Static Keys
        public bool statickeys_enabled;
        public Color statickeys_color;
        public List<keyboardBitmapKeys> staticKeys { get; set; }

        //// Flashbang
        public bool flashbang_enabled;
        public Color flash_color;

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
            return JsonConvert.DeserializeObject<Configuration>(content);
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
            config.DefaultValues();
            var configData = JsonConvert.SerializeObject(config, Formatting.Indented);
            var configPath = fileNameWithoutExtension + ConfigExtension;

            File.WriteAllText(configPath, configData, Encoding.UTF8);

            return config;
        }
    }
}
