using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms.VisualStyles;

namespace Client
{
    static class GameClass
    {
        private static volatile bool runGame;
        public static void GameLoop()
        {
            string message = null;

            while (runGame)
            {
                try
                { message = Network.ReadFromServer.ReadString(); }
                catch(Exception ex)
                { }

                if (message != null)
                {
                    MessageBox.Show("Odebrano: " + message);
                    message = null;
                }
            }
        }

        public static void SetGameMode(GameMode Mode)
        {
            //runGame = TurnOn;
            if (Mode == GameMode.On)
            {
                runGame = true;
            }
            if (Mode == GameMode.Off)
            {
                runGame = false;
            }
        }
    }

    public enum GameMode
    {
        On,
        Off
    };
}
