using CSGSI;
using CSGSI.Nodes;
using System;

namespace Logitech_CSGO
{
    public static class Global {

        public static GameEventHandler geh = new GameEventHandler();
        public static GameStateListener gsl;
    }
    
    class Program
    {
        static bool IsPlanted = false;

        static void Main(string[] args)
        {
            if (!Global.geh.Init())
                return;

            Global.gsl = new GameStateListener("http://127.0.0.1:1337/");
            Global.gsl.NewGameState += new NewGameStateHandler(OnNewGameState);

            if (!Global.gsl.Start())
            {
                Environment.Exit(0);
            }
            Console.WriteLine("Listening for game integration calls...");
            Console.WriteLine("You can close this window to quit the program.");
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
        }
    }
}
