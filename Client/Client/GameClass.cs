using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms.VisualStyles;

namespace Client
{
    public static class GameClass
    {//add initialize (eraseing data) in Game and Room Class
        public static string MyPlayerName { get; set; }
        private static List<int> IdListPlayer = new List<int>();
        private static Dictionary<int, string> NameOfPlayer = new Dictionary<int, string>();
        private static Dictionary<int, int> AmountOPlayerCards = new Dictionary<int, int>();
        
        //on start: create table with player
        //create label with name of players
        public static void ClearGameClass()
        {
            IdListPlayer = new List<int>();
            NameOfPlayer = new Dictionary<int, string>();
            AmountOPlayerCards = new Dictionary<int, int>();
        }

        public static void ClearIdListPlayer()
        {
            IdListPlayer = new List<int>();
        }
        public static void ClearNameOfPlayer()
        {
            NameOfPlayer = new Dictionary<int, string>();
        }
        public static void ClearAmountOPlayerCards()
        {
            AmountOPlayerCards = new Dictionary<int, int>();
        }
        public static void AddPlayersName(int id, string name)
        {
            if (NameOfPlayer.ContainsKey(id))
            {
                NameOfPlayer[id] = name;
            }
            else
            {
                NameOfPlayer.Add(id, name);
            }
            
            if (!IdListPlayer.Contains(id))
            {
                IdListPlayer.Add(id);
            }
        }
        public static void AddPlayerAmountCard(int idPlayer, int idCard)
        {
            if (AmountOPlayerCards.ContainsKey(idPlayer))
            {
                AmountOPlayerCards[idPlayer] = idCard;
            }
            else
            {
                AmountOPlayerCards.Add(idPlayer, idCard);
            }
            
            if (!IdListPlayer.Contains(idPlayer))
            {
                IdListPlayer.Add(idPlayer);
            }
        }
        
    }

    public static class GameRoom
    {
        public static string NameRoom { get; set; }
        private static List<int> IdListRoom = new List<int>();
        private static List<int> IdListPlayerInRoom = new List<int>();
        private static List<int> IdListPlayerToStartGame = new List<int>();
        private static Dictionary<int, string> NameOfRoom = new Dictionary<int, string>();
        private static Dictionary<int, string> NameOfPlayers = new Dictionary<int, string>();
        //add erase/update method
        public static void ClearGameRoom()
        {
            IdListRoom = new List<int>();
            IdListPlayerInRoom = new List<int>();
            IdListPlayerToStartGame = new List<int>();
            NameOfRoom = new Dictionary<int, string>();
            NameOfPlayers = new Dictionary<int, string>();
        }

        public static void AddRoomAndId(int id, string name)
        {
            if (!IdListRoom.Contains(id))
            {
                IdListRoom.Add(id);
            }
            if (NameOfRoom.ContainsKey(id))
            {
                NameOfRoom[id] = name;
            }
            else
            {
                NameOfRoom.Add(id, name);
            }
        }

        public static void AddPlayerNameInRoom(int idPlayer, string playerName)
        {
            if (NameOfPlayers.ContainsKey(idPlayer))
            {
                NameOfPlayers[idPlayer] = playerName;
            }
            else
            {
                NameOfPlayers.Add(idPlayer, playerName);
            }
            if (!IdListPlayerInRoom.Contains(idPlayer))
            {
                IdListPlayerInRoom.Add(idPlayer);
            }
        }

        public static void AddPlayerToStartGame(int idPlayer, string playerName)
        {
            if (NameOfPlayers.ContainsKey(idPlayer))
            {
                NameOfPlayers[idPlayer] = playerName;
            }
            else
            {
                NameOfPlayers.Add(idPlayer, playerName);
            }
            if (!IdListPlayerToStartGame.Contains(idPlayer))
            {
                IdListPlayerToStartGame.Add(idPlayer);
            }
        }

        public static void ClearIdListRoom()
        {
            IdListRoom = new List<int>();
        }
        public static void ClearIdListPlayerInRoom()
        {
            IdListPlayerInRoom = new List<int>();
        }
        public static void ClearIdListPlayerToStartGame()
        {
            IdListPlayerToStartGame = new List<int>();
        }
        public static void ClearNameOfRoom()
        {
            NameOfRoom = new Dictionary<int, string>();
        }
        public static void ClearNameOfPlayers()
        {
            NameOfPlayers = new Dictionary<int, string>();
        }
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
