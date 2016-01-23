using CSGSI;
using CSGSI.Nodes;
using System;
using System.Windows;

namespace Logitech_CSGO
{
    public static class Global {

        public static GameEventHandler geh = new GameEventHandler();
        public static GameStateListener gsl;
        public static Configuration Configuration { get; set; }
    }
    
    static class Program
    {
        public static Application WinApp { get; private set; }
        public static Window MainWindow;

        public static bool isSilent = false;
        
        static bool IsPlanted = false;

        [STAThread]
        static void Main(string[] args)
        {
            foreach(string arg in args)
            {
                switch (arg)
                {
                    case("-silent"):
                        isSilent = true;
                        break;
                }
            }
            
            //Load config
            Global.Configuration = ConfigManager.Load("Config");

            if (!Global.geh.Init())
                return;

            Global.gsl = new GameStateListener("http://127.0.0.1:1337/");
            Global.gsl.NewGameState += new NewGameStateHandler(OnNewGameState);

            if (!Global.gsl.Start())
            {
                System.Windows.MessageBox.Show("GameStateListener could not start. Try running this program as Administrator.\r\nExiting.");
                Environment.Exit(0);
            }
            Console.WriteLine("Listening for game integration calls...");
            Console.WriteLine("You can close this window to quit the program.");

            MainWindow = new ConfigUI();
            WinApp = new Application();
            WinApp.ShutdownMode = ShutdownMode.OnExplicitShutdown;
            WinApp.MainWindow = MainWindow;
            WinApp.Run(MainWindow);

            ConfigManager.Save("Config", Global.Configuration);

            Global.geh.Destroy();
            Global.gsl.Stop();

            Environment.Exit(0);
        }

        static void OnNewGameState(GameState gs)
        {
            if (!IsPlanted &&
               gs.Round.Phase == RoundPhase.Live &&
               gs.Round.Bomb == BombState.Planted &&
               gs.Previously.Round.Bomb == BombState.Undefined)
            {
                IsPlanted = true;
                Global.geh.SetBombState(BombState.Planted);
            }
            else if (IsPlanted && gs.Round.Phase == RoundPhase.FreezeTime)
            {
                IsPlanted = false;
                Global.geh.SetBombState(BombState.Undefined);
            }
            else if (IsPlanted && gs.Round.Bomb == BombState.Defused)
            {
                Global.geh.SetBombState(BombState.Defused);
            }
            else if (IsPlanted && gs.Round.Bomb == BombState.Exploded)
            {
                Global.geh.SetBombState(BombState.Exploded);
            }

            Global.geh.SetTeam(gs.Player.Team);
            Global.geh.SetHealth(gs.Player.State.Health);
            Global.geh.SetFlashAmount(gs.Player.State.Flashed);
            Global.geh.SetClip(gs.Player.Weapons.ActiveWeapon.AmmoClip);
            Global.geh.SetClipMax(gs.Player.Weapons.ActiveWeapon.AmmoClipMax);
            Global.geh.SetPlayerActivity(gs.Player.Activity);
        }
    }
}
