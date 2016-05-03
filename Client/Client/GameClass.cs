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
        public static string MyPlayerName { get; set; }
        private static List<int> IdListPlayer;
        private static Dictionary<int, string> NameOfPlayer;


    }

    static class GameRoom
    {
        public static string NameRoom { get; set; }
        private static List<int> IdListRoom;
        private static List<int> IdListPlayerInRoom;
        private static List<int> IdListPlayerToStartGame;
        private static Dictionary<int, string> NameOfRoom;
        private static Dictionary<int, string> NameOfPlayer;

        public static void NewRoom()
        {//TODO
            //??
            NewRoom createRoom = new NewRoom();

            if (createRoom.ShowDialog() == true)
            {

            }

            //CUserControl.Content = MyContentClassWindow.ChangeContent(ContNum.PlayersBoard);
        }

        public static void JoinToRoom()
        {//TODO
            Network.ConnectToServer();
            Network.BeginReceiveDataFromServer();

            //CUserControl.Content = MyContentClassWindow.ChangeContent(ContNum.PlayersBoard);

            //??
            //JoinRoom joinToRoom = new JoinRoom();
            //joinToRoom.ShowDialog();
        }
    }
}
