using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logitech_CSGO
{
    public class KeyboardKey
    {
        public String visualName;
        public Devices.DeviceKeys tag;
        public bool line_break;
        public double margin_left;
        public double margin_top;
        public double width;
        public double height;
        public double font_size;

        public KeyboardKey(String text, Devices.DeviceKeys tag, bool linebreak = false, double fontsize = 12, double margin_left = 7, double margin_top = 0, double width = 30, double height = 30)
        {
            this.visualName = text;
            this.tag = tag;
            this.line_break = linebreak;
            this.width = width;
            this.height = height;
            this.font_size = fontsize;
            this.margin_left = margin_left;
            this.margin_top = margin_top;
        }
    }
    
    public class KeyboardLayouts
    {
        private List<KeyboardKey> keyboard = new List<KeyboardKey>();

        private String cultures_folder = "kb_layouts";

        public KeyboardLayouts()
        {
            try
            {
                if (Directory.Exists(Path.Combine(cultures_folder)))
                {
                    switch (System.Threading.Thread.CurrentThread.CurrentCulture.Name)
                    {
                        case ("de-DE"):
                        case ("hsb-DE"):
                        case ("dsb-DE"):
                            LoadCulture("de");
                            break;
                        case ("fr-FR"):
                        case ("br-FR"):
                        case ("oc-FR"):
                        case ("co-FR"):
                        case ("gsw-FR"):
                            LoadCulture("fr");
                            break;
                        case ("cy-GB"):
                        case ("gd-GB"):
                        case ("en-GB"):
                            LoadCulture("uk");
                            break;
                        case ("ru-RU"):
                        case ("tt-RU"):
                        case ("ba-RU"):
                        case ("sah-RU"):
                            LoadCulture("ru");
                            break;
                        default:
                            LoadCulture("us");
                            break;
                    }
                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e);
            }

            LoadDefault();
        }

        public void LoadCulture(String culture)
        {
            var layoutPath = Path.Combine(cultures_folder, "layout." + culture + ".json");

            if (!File.Exists(layoutPath))
                LoadDefault();

            string content = File.ReadAllText(layoutPath, Encoding.UTF8);
            keyboard = JsonConvert.DeserializeObject<List<KeyboardKey>>(content, new JsonSerializerSettings { ObjectCreationHandling = ObjectCreationHandling.Replace });
        }

        public void LoadDefault()
        {
            keyboard.Add(new KeyboardKey("ESC", Devices.DeviceKeys.ESC));

            keyboard.Add(new KeyboardKey("F1", Devices.DeviceKeys.F1, false, 12, 32));
            keyboard.Add(new KeyboardKey("F2", Devices.DeviceKeys.F2));
            keyboard.Add(new KeyboardKey("F3", Devices.DeviceKeys.F3));
            keyboard.Add(new KeyboardKey("F4", Devices.DeviceKeys.F4));

            keyboard.Add(new KeyboardKey("F5", Devices.DeviceKeys.F5, false, 12, 34));
            keyboard.Add(new KeyboardKey("F6", Devices.DeviceKeys.F6));
            keyboard.Add(new KeyboardKey("F7", Devices.DeviceKeys.F7));
            keyboard.Add(new KeyboardKey("F8", Devices.DeviceKeys.F8));

            keyboard.Add(new KeyboardKey("F9", Devices.DeviceKeys.F9, false, 12, 29));
            keyboard.Add(new KeyboardKey("F10", Devices.DeviceKeys.F10));
            keyboard.Add(new KeyboardKey("F11", Devices.DeviceKeys.F11));
            keyboard.Add(new KeyboardKey("F12", Devices.DeviceKeys.F12));

            keyboard.Add(new KeyboardKey("PRINT", Devices.DeviceKeys.PRINT_SCREEN, false, 9, 14));
            keyboard.Add(new KeyboardKey("SCRL\r\nLOCK", Devices.DeviceKeys.SCROLL_LOCK, false, 9));
            keyboard.Add(new KeyboardKey("PAUSE", Devices.DeviceKeys.PAUSE_BREAK, true, 9));

            keyboard.Add(new KeyboardKey("~", Devices.DeviceKeys.TILDE));
            keyboard.Add(new KeyboardKey("1", Devices.DeviceKeys.ONE));
            keyboard.Add(new KeyboardKey("2", Devices.DeviceKeys.TWO));
            keyboard.Add(new KeyboardKey("3", Devices.DeviceKeys.THREE));
            keyboard.Add(new KeyboardKey("4", Devices.DeviceKeys.FOUR));
            keyboard.Add(new KeyboardKey("5", Devices.DeviceKeys.FIVE));
            keyboard.Add(new KeyboardKey("6", Devices.DeviceKeys.SIX));
            keyboard.Add(new KeyboardKey("7", Devices.DeviceKeys.SEVEN));
            keyboard.Add(new KeyboardKey("8", Devices.DeviceKeys.EIGHT));
            keyboard.Add(new KeyboardKey("9", Devices.DeviceKeys.NINE));
            keyboard.Add(new KeyboardKey("0", Devices.DeviceKeys.ZERO));
            keyboard.Add(new KeyboardKey("-", Devices.DeviceKeys.MINUS));
            keyboard.Add(new KeyboardKey("=", Devices.DeviceKeys.EQUALS));
            keyboard.Add(new KeyboardKey("BACKSPACE", Devices.DeviceKeys.BACKSPACE, false, 12, 7, 0, 67 ));

            keyboard.Add(new KeyboardKey("INSERT", Devices.DeviceKeys.INSERT, false, 9, 14));
            keyboard.Add(new KeyboardKey("HOME", Devices.DeviceKeys.HOME, false, 9));
            keyboard.Add(new KeyboardKey("PAGE\r\nUP", Devices.DeviceKeys.HOME, false, 9));

            keyboard.Add(new KeyboardKey("NUM\r\nLOCK", Devices.DeviceKeys.NUM_LOCK, false, 9, 14));
            keyboard.Add(new KeyboardKey("/", Devices.DeviceKeys.NUM_SLASH));
            keyboard.Add(new KeyboardKey("*", Devices.DeviceKeys.NUM_ASTERISK));
            keyboard.Add(new KeyboardKey("-", Devices.DeviceKeys.NUM_MINUS, true));

            keyboard.Add(new KeyboardKey("TAB", Devices.DeviceKeys.TAB, false, 12, 7, 0, 50));
            keyboard.Add(new KeyboardKey("Q", Devices.DeviceKeys.Q));
            keyboard.Add(new KeyboardKey("W", Devices.DeviceKeys.W));
            keyboard.Add(new KeyboardKey("E", Devices.DeviceKeys.E));
            keyboard.Add(new KeyboardKey("R", Devices.DeviceKeys.R));
            keyboard.Add(new KeyboardKey("T", Devices.DeviceKeys.T));
            keyboard.Add(new KeyboardKey("Y", Devices.DeviceKeys.Y));
            keyboard.Add(new KeyboardKey("U", Devices.DeviceKeys.U));
            keyboard.Add(new KeyboardKey("I", Devices.DeviceKeys.I));
            keyboard.Add(new KeyboardKey("O", Devices.DeviceKeys.O));
            keyboard.Add(new KeyboardKey("P", Devices.DeviceKeys.P));
            keyboard.Add(new KeyboardKey("{", Devices.DeviceKeys.OPEN_BRACKET));
            keyboard.Add(new KeyboardKey("}", Devices.DeviceKeys.CLOSE_BRACKET));
            keyboard.Add(new KeyboardKey("\\", Devices.DeviceKeys.BACKSLASH, false, 12, 7, 0, 49));

            keyboard.Add(new KeyboardKey("DEL", Devices.DeviceKeys.KEYBOARD_DELETE, false, 9, 12));
            keyboard.Add(new KeyboardKey("END", Devices.DeviceKeys.END, false, 9));
            keyboard.Add(new KeyboardKey("PAGE\r\nDOWN", Devices.DeviceKeys.PAGE_DOWN, false, 9));

            keyboard.Add(new KeyboardKey("7", Devices.DeviceKeys.NUM_SEVEN, false, 12, 14));
            keyboard.Add(new KeyboardKey("8", Devices.DeviceKeys.NUM_EIGHT));
            keyboard.Add(new KeyboardKey("9", Devices.DeviceKeys.NUM_NINE));
            keyboard.Add(new KeyboardKey("+", Devices.DeviceKeys.NUM_PLUS, true, 12, 7, 0, 30, 69));

            keyboard.Add(new KeyboardKey("CAPS\r\nLOCK", Devices.DeviceKeys.CAPS_LOCK, false, 9, 7, 0, 60));
            keyboard.Add(new KeyboardKey("A", Devices.DeviceKeys.A));
            keyboard.Add(new KeyboardKey("S", Devices.DeviceKeys.S));
            keyboard.Add(new KeyboardKey("D", Devices.DeviceKeys.D));
            keyboard.Add(new KeyboardKey("F", Devices.DeviceKeys.F));
            keyboard.Add(new KeyboardKey("G", Devices.DeviceKeys.G));
            keyboard.Add(new KeyboardKey("H", Devices.DeviceKeys.H));
            keyboard.Add(new KeyboardKey("J", Devices.DeviceKeys.J));
            keyboard.Add(new KeyboardKey("K", Devices.DeviceKeys.K));
            keyboard.Add(new KeyboardKey("L", Devices.DeviceKeys.L));
            keyboard.Add(new KeyboardKey(":", Devices.DeviceKeys.SEMICOLON));
            keyboard.Add(new KeyboardKey("\"", Devices.DeviceKeys.APOSTROPHE));
            keyboard.Add(new KeyboardKey("ENTER", Devices.DeviceKeys.ENTER, false, 12, 7, 0, 76));

            keyboard.Add(new KeyboardKey("4", Devices.DeviceKeys.NUM_FOUR, false, 12, 130));
            keyboard.Add(new KeyboardKey("5", Devices.DeviceKeys.NUM_FIVE));
            keyboard.Add(new KeyboardKey("6", Devices.DeviceKeys.NUM_SIX, true));
            //Space taken up by +

            keyboard.Add(new KeyboardKey("SHIFT", Devices.DeviceKeys.LEFT_SHIFT, false, 12, 7, 0, 78));
            keyboard.Add(new KeyboardKey("Z", Devices.DeviceKeys.Z));
            keyboard.Add(new KeyboardKey("X", Devices.DeviceKeys.X));
            keyboard.Add(new KeyboardKey("C", Devices.DeviceKeys.C));
            keyboard.Add(new KeyboardKey("V", Devices.DeviceKeys.V));
            keyboard.Add(new KeyboardKey("B", Devices.DeviceKeys.B));
            keyboard.Add(new KeyboardKey("N", Devices.DeviceKeys.N));
            keyboard.Add(new KeyboardKey("M", Devices.DeviceKeys.M));
            keyboard.Add(new KeyboardKey("<", Devices.DeviceKeys.COMMA));
            keyboard.Add(new KeyboardKey(">", Devices.DeviceKeys.PERIOD));
            keyboard.Add(new KeyboardKey("?", Devices.DeviceKeys.FORWARD_SLASH));
            keyboard.Add(new KeyboardKey("SHIFT", Devices.DeviceKeys.RIGHT_SHIFT, false, 12, 7, 0, 95));

            keyboard.Add(new KeyboardKey("UP", Devices.DeviceKeys.ARROW_UP, false, 9, 49));

            keyboard.Add(new KeyboardKey("1", Devices.DeviceKeys.NUM_ONE, false, 12, 51));
            keyboard.Add(new KeyboardKey("2", Devices.DeviceKeys.NUM_TWO));
            keyboard.Add(new KeyboardKey("3", Devices.DeviceKeys.NUM_THREE));
            keyboard.Add(new KeyboardKey("ENTER", Devices.DeviceKeys.NUM_ENTER, true, 9, 7, 0, 30, 69));

            keyboard.Add(new KeyboardKey("CTRL", Devices.DeviceKeys.RIGHT_CONTROL, false, 12, 7, 0, 51));
            keyboard.Add(new KeyboardKey("WIN", Devices.DeviceKeys.RIGHT_WINDOWS, false, 12, 5, 0, 39));
            keyboard.Add(new KeyboardKey("ALT", Devices.DeviceKeys.RIGHT_ALT, false, 12, 5, 0, 42));

            keyboard.Add(new KeyboardKey("SPACE", Devices.DeviceKeys.SPACE, false, 12, 7, 0, 208));
            keyboard.Add(new KeyboardKey("ALT", Devices.DeviceKeys.LEFT_ALT, false, 12, 5, 0, 41));
            keyboard.Add(new KeyboardKey("WIN", Devices.DeviceKeys.LEFT_ALT, false, 12, 5, 0, 41));
            keyboard.Add(new KeyboardKey("APP", Devices.DeviceKeys.LEFT_ALT, false, 12, 5, 0, 41));
            keyboard.Add(new KeyboardKey("CTRL", Devices.DeviceKeys.LEFT_ALT, false, 12, 5, 0, 50));

            keyboard.Add(new KeyboardKey("LEFT", Devices.DeviceKeys.ARROW_LEFT, false, 9, 12));
            keyboard.Add(new KeyboardKey("DOWN", Devices.DeviceKeys.ARROW_DOWN, false, 9));
            keyboard.Add(new KeyboardKey("RIGHT", Devices.DeviceKeys.ARROW_DOWN, false, 9));

            keyboard.Add(new KeyboardKey("0", Devices.DeviceKeys.NUM_ZERO, false, 12, 14, 0, 67));
            keyboard.Add(new KeyboardKey(".", Devices.DeviceKeys.NUM_PERIOD, true));
        }

        public List<KeyboardKey> GetLayout()
        {
            return keyboard;
        }
    }
}
