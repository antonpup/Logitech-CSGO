using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Xceed.Wpf.Toolkit;

namespace Logitech_CSGO
{
    enum KeyboardRecordingType
    {
        None,
        HealthKeys,
        AmmoKeys,
        BombKeys,
        StaticKeys
    }
    
    /// <summary>
    /// Interaction logic for ConfigUI.xaml
    /// </summary>
    public partial class ConfigUI : Window
    {
        private bool settingsloaded = false;

        private Timer virtual_keyboard_timer;
        private TextBlock last_selected_key;
        private KeyboardRecordingType recordingKeystrokes = KeyboardRecordingType.None;
        private Stopwatch recording_stopwatch = new Stopwatch();
        private List<LedCSharp.keyboardBitmapKeys> recordedKeys = new List<LedCSharp.keyboardBitmapKeys>();

        private Timer preview_bomb_timer;
        private Timer preview_bomb_remove_effect_timer;

        public ConfigUI()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (!settingsloaded)
            {
                this.background_enabled.IsChecked = Global.Configuration.bg_team_enabled;
                this.t_colorpicker.SelectedColor = System.Windows.Media.Color.FromArgb(Global.Configuration.t_color.A, Global.Configuration.t_color.R, Global.Configuration.t_color.G, Global.Configuration.t_color.B);
                this.ct_colorpicker.SelectedColor = System.Windows.Media.Color.FromArgb(Global.Configuration.ct_color.A, Global.Configuration.ct_color.R, Global.Configuration.ct_color.G, Global.Configuration.ct_color.B);

                this.health_enabled.IsChecked = Global.Configuration.health_enabled;
                this.health_healthy_colorpicker.SelectedColor = System.Windows.Media.Color.FromArgb(Global.Configuration.healthy_color.A, Global.Configuration.healthy_color.R, Global.Configuration.healthy_color.G, Global.Configuration.healthy_color.B);
                this.health_hurt_colorpicker.SelectedColor = System.Windows.Media.Color.FromArgb(Global.Configuration.hurt_color.A, Global.Configuration.hurt_color.R, Global.Configuration.hurt_color.G, Global.Configuration.hurt_color.B);
                this.health_effect_type.Items.Add("All At Once");
                this.health_effect_type.Items.Add("Progressive");
                this.health_effect_type.Items.Add("Progressive (Gradual)");
                this.health_effect_type.SelectedIndex = (int)Global.Configuration.health_effect_type;
                foreach (var key in Global.Configuration.healthKeys)
                {
                    this.health_keysequence.Items.Add(key);
                }

                this.ammo_enabled.IsChecked = Global.Configuration.ammo_enabled;
                this.ammo_hasammo_colorpicker.SelectedColor = System.Windows.Media.Color.FromArgb(Global.Configuration.ammo_color.A, Global.Configuration.ammo_color.R, Global.Configuration.ammo_color.G, Global.Configuration.ammo_color.B);
                this.ammo_noammo_colorpicker.SelectedColor = System.Windows.Media.Color.FromArgb(Global.Configuration.noammo_color.A, Global.Configuration.noammo_color.R, Global.Configuration.noammo_color.G, Global.Configuration.noammo_color.B);
                this.ammo_effect_type.Items.Add("All At Once");
                this.ammo_effect_type.Items.Add("Progressive");
                this.ammo_effect_type.Items.Add("Progressive (Gradual)");
                this.ammo_effect_type.SelectedIndex = (int)Global.Configuration.ammo_effect_type;
                foreach (var key in Global.Configuration.ammoKeys)
                {
                    this.ammo_keysequence.Items.Add(key);
                }

                this.bomb_enabled.IsChecked = Global.Configuration.bomb_enabled;
                this.bomb_flash_color_colorpicker.SelectedColor = System.Windows.Media.Color.FromArgb(Global.Configuration.bomb_flash_color.A, Global.Configuration.bomb_flash_color.R, Global.Configuration.bomb_flash_color.G, Global.Configuration.bomb_flash_color.B);
                this.bomb_primed_color_colorpicker.SelectedColor = System.Windows.Media.Color.FromArgb(Global.Configuration.bomb_primed_color.A, Global.Configuration.bomb_primed_color.R, Global.Configuration.bomb_primed_color.G, Global.Configuration.bomb_primed_color.B);
                this.bomb_display_winner.IsChecked = Global.Configuration.bomb_display_winner_color;
                this.bomb_gradual_effect.IsChecked = Global.Configuration.bomb_gradual;
                foreach (var key in Global.Configuration.bombKeys)
                {
                    this.bomb_keysequence.Items.Add(key);
                }

                this.statickeys_enabled.IsChecked = Global.Configuration.statickeys_enabled;
                this.statickeys_color_colorpicker.SelectedColor = System.Windows.Media.Color.FromArgb(Global.Configuration.statickeys_color.A, Global.Configuration.statickeys_color.R, Global.Configuration.statickeys_color.G, Global.Configuration.statickeys_color.B);
                foreach (var key in Global.Configuration.staticKeys)
                {
                    this.statickeys_keysequence.Items.Add(key);
                }

                this.flashbang_enabled.IsChecked = Global.Configuration.flashbang_enabled;
                this.flashbang_color_colorpicker.SelectedColor = System.Windows.Media.Color.FromArgb(Global.Configuration.flash_color.A, Global.Configuration.flash_color.R, Global.Configuration.flash_color.G, Global.Configuration.flash_color.B);
                

                virtual_keyboard_timer = new Timer(100);
                virtual_keyboard_timer.Elapsed += new ElapsedEventHandler(virtual_keyboard_timer_Tick);
                virtual_keyboard_timer.Start();


                preview_bomb_timer = new Timer(45000);
                preview_bomb_timer.Elapsed += new ElapsedEventHandler(preview_bomb_timer_Tick);

                preview_bomb_remove_effect_timer = new Timer(5000);
                preview_bomb_remove_effect_timer.Elapsed += new ElapsedEventHandler(preview_bomb_remove_effect_timer_Tick);
                //preview_bomb_remove_effect_timer.Start();

                

                settingsloaded = true;
            }
            //System.Windows.Media.Color.FromArgb(Global.Configuration.healthy_color.A, Global.Configuration.healthy_color.R, Global.Configuration.healthy_color.G, Global.Configuration.healthy_color.B);
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            virtual_keyboard_timer.Stop();

            Application.Current.Shutdown();
        }

        public static bool ApplicationIsActivated()
        {
            var activatedHandle = GetForegroundWindow();
            if (activatedHandle == IntPtr.Zero)
            {
                return false;       // No window is currently activated
            }

            var procId = Process.GetCurrentProcess().Id;
            int activeProcId;
            GetWindowThreadProcessId(activatedHandle, out activeProcId);

            return activeProcId == procId;
        }

        [System.Runtime.InteropServices.DllImport("user32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto, ExactSpelling = true)]
        private static extern IntPtr GetForegroundWindow();

        [System.Runtime.InteropServices.DllImport("user32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto, SetLastError = true)]
        private static extern int GetWindowThreadProcessId(IntPtr handle, out int processId);

        private void virtual_keyboard_timer_Tick(object sender, EventArgs e)
        {
            if (!ApplicationIsActivated())
                return;
            
            Dispatcher.Invoke(
                        () =>
                        {
                            Dictionary<LedCSharp.keyboardBitmapKeys, System.Drawing.Color> keylights = new Dictionary<LedCSharp.keyboardBitmapKeys, System.Drawing.Color>();

                            if ((this.preview_enabled.IsChecked.HasValue) ? this.preview_enabled.IsChecked.Value : false)
                                keylights = Global.geh.GetKeyboardLights();

                                foreach (var child in this.keyboard_grid.Children)
                                {
                                    if (child is TextBlock &&
                                        (child as TextBlock).Tag is LedCSharp.keyboardBitmapKeys
                                        )
                                    {
                                        if(keylights.ContainsKey((LedCSharp.keyboardBitmapKeys)(child as TextBlock).Tag))
                                        {
                                            System.Drawing.Color keycolor = keylights[(LedCSharp.keyboardBitmapKeys)(child as TextBlock).Tag];

                                            (child as TextBlock).Foreground = new SolidColorBrush(System.Windows.Media.Color.FromArgb(keycolor.A, keycolor.R, keycolor.G, keycolor.B));
                                        }
                                        
                                        if (this.recordedKeys.Contains((LedCSharp.keyboardBitmapKeys)(child as TextBlock).Tag))
                                            (child as TextBlock).Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb((byte)(Math.Min(Math.Pow(Math.Sin((double)recording_stopwatch.ElapsedMilliseconds / 1000.0) + 0.05, 2.0), 1.0) * 255), (byte)0, (byte)255, (byte)0));
                                        else
                                            (child as TextBlock).Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb((byte)0, (byte)0, (byte)0, (byte)0));
                                    }
                                    else if (child is Border &&
                                        (child as Border).Child is TextBlock &&
                                        ((child as Border).Child as TextBlock).Tag is LedCSharp.keyboardBitmapKeys
                                        )
                                    {
                                        if (keylights.ContainsKey((LedCSharp.keyboardBitmapKeys)((child as Border).Child as TextBlock).Tag))
                                        {
                                            System.Drawing.Color keycolor = keylights[(LedCSharp.keyboardBitmapKeys)((child as Border).Child as TextBlock).Tag];

                                            ((child as Border).Child as TextBlock).Foreground = new SolidColorBrush(System.Windows.Media.Color.FromArgb(keycolor.A, keycolor.R, keycolor.G, keycolor.B));
                                        }
                                        
                                        if (this.recordedKeys.Contains((LedCSharp.keyboardBitmapKeys)((child as Border).Child as TextBlock).Tag))
                                            (child as Border).Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb((byte)(Math.Min(Math.Pow(Math.Sin((double)recording_stopwatch.ElapsedMilliseconds / 1000.0) + 0.05, 2.0), 1.0) * 255), (byte)0, (byte)255, (byte)0));
                                        else
                                            (child as Border).Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb((byte)0, (byte)0, (byte)0, (byte)0));
                                    }

                                }
                        });
        }

        private void preview_bomb_timer_Tick(object sender, EventArgs e)
        {
            Dispatcher.Invoke(
                    () =>
                    {
                        this.preview_bomb_defused.IsEnabled = false;
                        this.preview_bomb_start.IsEnabled = true;

                        Global.geh.SetBombState(CSGSI.Nodes.BombState.Exploded);
                        preview_bomb_timer.Stop();

                        preview_bomb_remove_effect_timer.Start();
                    });
        }

        private void preview_bomb_remove_effect_timer_Tick(object sender, EventArgs e)
        {
            Global.geh.SetBombState(CSGSI.Nodes.BombState.Undefined);
            preview_bomb_remove_effect_timer.Stop();
        }

        private List<LedCSharp.keyboardBitmapKeys> SequenceToList(ItemCollection items)
        {
            List<LedCSharp.keyboardBitmapKeys> newsequence = new List<LedCSharp.keyboardBitmapKeys>();

            foreach (LedCSharp.keyboardBitmapKeys key in items)
            {
                newsequence.Add(key);
            }

            return newsequence;
        }

        private bool ListBoxMoveSelectedUp(ListBox list)
        {
            if (list.Items.Count > 0 && list.SelectedIndex > 0)
            {
                int selected_index = list.SelectedIndex;
                var saved = list.Items[selected_index];
                list.Items[selected_index] = list.Items[selected_index - 1];
                list.Items[selected_index - 1] = saved;
                list.SelectedIndex = selected_index - 1;

                list.ScrollIntoView(list.Items[selected_index - 1]);
                return true;
            }

            return false;
        }

        private bool ListBoxMoveSelectedDown(ListBox list)
        {
            if (list.Items.Count > 0 && list.SelectedIndex < (list.Items.Count - 1))
            {
                int selected_index = list.SelectedIndex;
                var saved = list.Items[selected_index];
                list.Items[selected_index] = list.Items[selected_index + 1];
                list.Items[selected_index + 1] = saved;
                list.SelectedIndex = selected_index + 1;

                list.ScrollIntoView(list.Items[selected_index + 1]);
                return true;
            }

            return false;
        }

        private bool ListBoxRemoveSelected(ListBox list)
        {
            if (list.Items.Count > 0 && list.SelectedIndex >= 0)
            {
                int selected = list.SelectedIndex;
                list.Items.RemoveAt(selected);

                if (list.Items.Count > selected)
                    list.SelectedIndex = selected;
                else
                    list.SelectedIndex = (list.Items.Count - 1);

                if (list.SelectedIndex > -1)
                    list.ScrollIntoView(list.Items[list.SelectedIndex]);

                return true;
            }

            return false;
        }

        private void RecordKeySequence(KeyboardRecordingType whoisrecording, Button button, ListBox sequence_listbox)
        {

            if (recordingKeystrokes == KeyboardRecordingType.None)
            {
                this.recordedKeys = new List<LedCSharp.keyboardBitmapKeys>();

                button.Content = "Stop Recording";
                recording_stopwatch.Restart();
                recordingKeystrokes = whoisrecording;
            }
            else if (recordingKeystrokes == whoisrecording)
            {
                if (sequence_listbox.SelectedIndex > 0 && sequence_listbox.SelectedIndex < (sequence_listbox.Items.Count - 1))
                {
                    int insertpos = sequence_listbox.SelectedIndex;
                    foreach (var key in this.recordedKeys)
                    {
                        sequence_listbox.Items.Insert(insertpos, key);
                        insertpos++;
                    }
                }
                else
                {
                    foreach (var key in this.recordedKeys)
                        sequence_listbox.Items.Add(key);
                }

                switch(whoisrecording)
                {
                    case(KeyboardRecordingType.HealthKeys):
                        Global.Configuration.healthKeys = SequenceToList(sequence_listbox.Items);
                        break;
                    case (KeyboardRecordingType.AmmoKeys):
                        Global.Configuration.ammoKeys = SequenceToList(sequence_listbox.Items);
                        break;
                    case (KeyboardRecordingType.BombKeys):
                        Global.Configuration.bombKeys = SequenceToList(sequence_listbox.Items);
                        break;
                    case (KeyboardRecordingType.StaticKeys):
                        Global.Configuration.staticKeys = SequenceToList(sequence_listbox.Items);
                        break;
                    default:
                        break;
                }

                this.recordedKeys = new List<LedCSharp.keyboardBitmapKeys>();
                button.Content = "Add/Record";
                recording_stopwatch.Stop();
                recordingKeystrokes = KeyboardRecordingType.None;
            }
            else
            {
                System.Windows.MessageBox.Show("You are already recording a key sequence for " + recordingKeystrokes.ToString());
            }
        }

        private void background_enabled_Checked(object sender, RoutedEventArgs e)
        {
            Global.Configuration.bg_team_enabled = (this.background_enabled.IsChecked.HasValue) ? this.background_enabled.IsChecked.Value : false;
        }

        private void t_colorpicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            if (this.t_colorpicker.SelectedColor.HasValue)
                Global.Configuration.t_color = System.Drawing.Color.FromArgb(this.t_colorpicker.SelectedColor.Value.A, this.t_colorpicker.SelectedColor.Value.R, this.t_colorpicker.SelectedColor.Value.G, this.t_colorpicker.SelectedColor.Value.B);
        }

        private void ct_colorpicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            if (this.ct_colorpicker.SelectedColor.HasValue)
                Global.Configuration.ct_color = System.Drawing.Color.FromArgb(this.ct_colorpicker.SelectedColor.Value.A, this.ct_colorpicker.SelectedColor.Value.R, this.ct_colorpicker.SelectedColor.Value.G, this.ct_colorpicker.SelectedColor.Value.B);
        }

        private void health_enabled_Checked(object sender, RoutedEventArgs e)
        {
            Global.Configuration.health_enabled = (this.health_enabled.IsChecked.HasValue) ? this.health_enabled.IsChecked.Value : false;
        }

        private void health_healthy_colorpicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            if (this.health_healthy_colorpicker.SelectedColor.HasValue)
                Global.Configuration.healthy_color = System.Drawing.Color.FromArgb(this.health_healthy_colorpicker.SelectedColor.Value.A, this.health_healthy_colorpicker.SelectedColor.Value.R, this.health_healthy_colorpicker.SelectedColor.Value.G, this.health_healthy_colorpicker.SelectedColor.Value.B);
        }

        private void health_hurt_colorpicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            if (this.health_hurt_colorpicker.SelectedColor.HasValue)
                Global.Configuration.hurt_color = System.Drawing.Color.FromArgb(this.health_hurt_colorpicker.SelectedColor.Value.A, this.health_hurt_colorpicker.SelectedColor.Value.R, this.health_hurt_colorpicker.SelectedColor.Value.G, this.health_hurt_colorpicker.SelectedColor.Value.B);
        }

        private void sequence_remove_statickeys_Click(object sender, RoutedEventArgs e)
        {
            if (ListBoxRemoveSelected(this.statickeys_keysequence))
                Global.Configuration.staticKeys = SequenceToList(this.statickeys_keysequence.Items);
        }

        private void sequence_up_statickeys_Click(object sender, RoutedEventArgs e)
        {
            if (ListBoxMoveSelectedUp(this.statickeys_keysequence))
                Global.Configuration.staticKeys = SequenceToList(this.statickeys_keysequence.Items);
        }

        private void sequence_down_statickeys_Click(object sender, RoutedEventArgs e)
        {
            if (ListBoxMoveSelectedDown(this.statickeys_keysequence))
                Global.Configuration.staticKeys = SequenceToList(this.statickeys_keysequence.Items);
        }

        private void statickeys_color_colorpicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            if (this.statickeys_color_colorpicker.SelectedColor.HasValue)
                Global.Configuration.statickeys_color = System.Drawing.Color.FromArgb(this.statickeys_color_colorpicker.SelectedColor.Value.A, this.statickeys_color_colorpicker.SelectedColor.Value.R, this.statickeys_color_colorpicker.SelectedColor.Value.G, this.statickeys_color_colorpicker.SelectedColor.Value.B);
        }

        private void statickeys_enabled_Checked(object sender, RoutedEventArgs e)
        {
            Global.Configuration.statickeys_enabled = (this.statickeys_enabled.IsChecked.HasValue) ? this.statickeys_enabled.IsChecked.Value : false;
        }

        private void sequence_remove_health_Click(object sender, RoutedEventArgs e)
        {
            if (ListBoxRemoveSelected(this.health_keysequence))
                Global.Configuration.healthKeys = SequenceToList(this.health_keysequence.Items);
        }

        private void sequence_up_health_Click(object sender, RoutedEventArgs e)
        {
            if (ListBoxMoveSelectedUp(this.health_keysequence))
                Global.Configuration.healthKeys = SequenceToList(this.health_keysequence.Items);
        }

        private void sequence_down_health_Click(object sender, RoutedEventArgs e)
        {
            if (ListBoxMoveSelectedDown(this.health_keysequence))
                Global.Configuration.healthKeys = SequenceToList(this.health_keysequence.Items);
        }

        private void sequence_record_statickeys_Click(object sender, RoutedEventArgs e)
        {
            RecordKeySequence(KeyboardRecordingType.StaticKeys, (sender as Button), this.statickeys_keysequence);
        }

        private void virtualkeyboard_key_selected(TextBlock key)
        {
            if (recordingKeystrokes != KeyboardRecordingType.None && key.Tag is LedCSharp.keyboardBitmapKeys)
            {
                if (recordedKeys.Contains((LedCSharp.keyboardBitmapKeys)(key.Tag)))
                    recordedKeys.Remove((LedCSharp.keyboardBitmapKeys)(key.Tag));
                else
                    recordedKeys.Add((LedCSharp.keyboardBitmapKeys)(key.Tag));
                last_selected_key = key;
            }
        }

        private void keyboard_grid_pressed(object sender, MouseButtonEventArgs e)
        {
            if(sender is TextBlock)
            {
                virtualkeyboard_key_selected(sender as TextBlock);
            }
        }

        private void keyboard_grid_moved(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && sender is TextBlock && last_selected_key != sender as TextBlock)
            {
                virtualkeyboard_key_selected(sender as TextBlock);
            }
        }

        private void preview_team_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch ((CSGSI.Nodes.PlayerTeam)this.preview_team.Items[this.preview_team.SelectedIndex])
            {
                case (CSGSI.Nodes.PlayerTeam.T):
                    Global.geh.SetTeam(CSGSI.Nodes.PlayerTeam.T);
                    break;
                case (CSGSI.Nodes.PlayerTeam.CT):
                    Global.geh.SetTeam(CSGSI.Nodes.PlayerTeam.CT);
                    break;
                default:
                    Global.geh.SetTeam(CSGSI.Nodes.PlayerTeam.Undefined);
                    break;
            }
        }

        private void preview_health_slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            int hp_val = (int)this.preview_health_slider.Value;
            if (this.preview_health_amount is Label)
            {
                this.preview_health_amount.Content = hp_val + "%";
                Global.geh.SetHealth(hp_val);
            }
        }

        private void preview_ammo_slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            int ammo_val = (int)this.preview_ammo_slider.Value;
            if (this.preview_ammo_amount is Label)
            {
                this.preview_ammo_amount.Content = ammo_val + "%";
                Global.geh.SetClip(ammo_val);
            }
        }

        private void preview_flash_slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            int flash_val = (int)this.preview_flash_slider.Value;
            if (this.preview_flash_amount is Label)
            {
                this.preview_flash_amount.Content = flash_val + "%";
                Global.geh.SetFlashAmount((int)(((double)flash_val / 100.0) * 255.0));
            }
        }

        private void health_effect_type_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Global.Configuration.health_effect_type = (PercentEffectType)Enum.Parse(typeof(PercentEffectType), this.health_effect_type.SelectedIndex.ToString());
        }

        private void sequence_record_health_Click(object sender, RoutedEventArgs e)
        {
            RecordKeySequence(KeyboardRecordingType.HealthKeys, (sender as Button), this.health_keysequence);
        }

        private void ammo_enabled_Checked(object sender, RoutedEventArgs e)
        {
            Global.Configuration.ammo_enabled = (this.ammo_enabled.IsChecked.HasValue) ? this.ammo_enabled.IsChecked.Value : false;
        }

        private void ammo_hasammo_colorpicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            if (this.ammo_hasammo_colorpicker.SelectedColor.HasValue)
                Global.Configuration.ammo_color = System.Drawing.Color.FromArgb(this.ammo_hasammo_colorpicker.SelectedColor.Value.A, this.ammo_hasammo_colorpicker.SelectedColor.Value.R, this.ammo_hasammo_colorpicker.SelectedColor.Value.G, this.ammo_hasammo_colorpicker.SelectedColor.Value.B);
        }

        private void ammo_noammo_colorpicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            if (this.ammo_noammo_colorpicker.SelectedColor.HasValue)
                Global.Configuration.noammo_color = System.Drawing.Color.FromArgb(this.ammo_noammo_colorpicker.SelectedColor.Value.A, this.ammo_noammo_colorpicker.SelectedColor.Value.R, this.ammo_noammo_colorpicker.SelectedColor.Value.G, this.ammo_noammo_colorpicker.SelectedColor.Value.B);
        }

        private void ammo_effect_type_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Global.Configuration.ammo_effect_type = (PercentEffectType)Enum.Parse(typeof(PercentEffectType), this.ammo_effect_type.SelectedIndex.ToString());
        }

        private void sequence_record_ammo_Click(object sender, RoutedEventArgs e)
        {
            RecordKeySequence(KeyboardRecordingType.AmmoKeys, (sender as Button), this.ammo_keysequence);
        }

        private void sequence_remove_ammo_Click(object sender, RoutedEventArgs e)
        {
            if (ListBoxRemoveSelected(this.ammo_keysequence))
                Global.Configuration.ammoKeys = SequenceToList(this.ammo_keysequence.Items);
        }

        private void sequence_up_ammo_Click(object sender, RoutedEventArgs e)
        {
            if (ListBoxMoveSelectedUp(this.ammo_keysequence))
                Global.Configuration.ammoKeys = SequenceToList(this.ammo_keysequence.Items);
        }

        private void sequence_down_ammo_Click(object sender, RoutedEventArgs e)
        {
            if (ListBoxMoveSelectedDown(this.ammo_keysequence))
                Global.Configuration.ammoKeys = SequenceToList(this.ammo_keysequence.Items);
        }

        private void bomb_enabled_Checked(object sender, RoutedEventArgs e)
        {
            Global.Configuration.bomb_enabled = (this.bomb_enabled.IsChecked.HasValue) ? this.bomb_enabled.IsChecked.Value : false;
        }

        private void bomb_flash_color_colorpicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            if (this.bomb_flash_color_colorpicker.SelectedColor.HasValue)
                Global.Configuration.bomb_flash_color = System.Drawing.Color.FromArgb(this.bomb_flash_color_colorpicker.SelectedColor.Value.A, this.bomb_flash_color_colorpicker.SelectedColor.Value.R, this.bomb_flash_color_colorpicker.SelectedColor.Value.G, this.bomb_flash_color_colorpicker.SelectedColor.Value.B);
        }

        private void bomb_primed_color_colorpicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            if (this.bomb_primed_color_colorpicker.SelectedColor.HasValue)
                Global.Configuration.bomb_primed_color = System.Drawing.Color.FromArgb(this.bomb_primed_color_colorpicker.SelectedColor.Value.A, this.bomb_primed_color_colorpicker.SelectedColor.Value.R, this.bomb_primed_color_colorpicker.SelectedColor.Value.G, this.bomb_primed_color_colorpicker.SelectedColor.Value.B);
        }

        private void sequence_record_bomb_Click(object sender, RoutedEventArgs e)
        {
            RecordKeySequence(KeyboardRecordingType.BombKeys, (sender as Button), this.bomb_keysequence);
        }

        private void sequence_remove_bomb_Click(object sender, RoutedEventArgs e)
        {
            if (ListBoxRemoveSelected(this.bomb_keysequence))
                Global.Configuration.bombKeys = SequenceToList(this.bomb_keysequence.Items);
        }

        private void sequence_up_bomb_Click(object sender, RoutedEventArgs e)
        {
            if (ListBoxMoveSelectedUp(this.bomb_keysequence))
                Global.Configuration.bombKeys = SequenceToList(this.bomb_keysequence.Items);
        }

        private void sequence_down_bomb_Click(object sender, RoutedEventArgs e)
        {
            if (ListBoxMoveSelectedDown(this.bomb_keysequence))
                Global.Configuration.bombKeys = SequenceToList(this.bomb_keysequence.Items);
        }

        private void bomb_display_winner_Checked(object sender, RoutedEventArgs e)
        {
            Global.Configuration.bomb_display_winner_color = (this.bomb_display_winner.IsChecked.HasValue) ? this.bomb_display_winner.IsChecked.Value : false;
        }

        private void bomb_gradual_effect_Checked(object sender, RoutedEventArgs e)
        {
            Global.Configuration.bomb_gradual = (this.bomb_gradual_effect.IsChecked.HasValue) ? this.bomb_gradual_effect.IsChecked.Value : false;
        }

        private void preview_bomb_start_Click(object sender, RoutedEventArgs e)
        {
            this.preview_bomb_defused.IsEnabled = true;
            this.preview_bomb_start.IsEnabled = false;

            Global.geh.SetBombState(CSGSI.Nodes.BombState.Planted);
            preview_bomb_timer.Start();
            preview_bomb_remove_effect_timer.Stop();
        }

        private void preview_bomb_defused_Click(object sender, RoutedEventArgs e)
        {
            this.preview_bomb_defused.IsEnabled = false;
            this.preview_bomb_start.IsEnabled = true;

            Global.geh.SetBombState(CSGSI.Nodes.BombState.Defused);
            preview_bomb_timer.Stop();
            preview_bomb_remove_effect_timer.Start();
        }

        private void flashbang_enabled_Checked(object sender, RoutedEventArgs e)
        {
            Global.Configuration.flashbang_enabled = (this.flashbang_enabled.IsChecked.HasValue) ? this.flashbang_enabled.IsChecked.Value : false;
        }

        private void flashbang_color_colorpicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            if (this.flashbang_color_colorpicker.SelectedColor.HasValue)
                Global.Configuration.flash_color = System.Drawing.Color.FromArgb(this.flashbang_color_colorpicker.SelectedColor.Value.A, this.flashbang_color_colorpicker.SelectedColor.Value.R, this.flashbang_color_colorpicker.SelectedColor.Value.G, this.flashbang_color_colorpicker.SelectedColor.Value.B);
        }

        private void preview_enabled_Checked(object sender, RoutedEventArgs e)
        {
            Global.geh.SetPreview((this.preview_enabled.IsChecked.HasValue) ? this.preview_enabled.IsChecked.Value : false);
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }
        
    }
}
