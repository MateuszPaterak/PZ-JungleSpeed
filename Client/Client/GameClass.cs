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
    {
        public static string MyPlayerName { get; set; }
        private static List<int> IdListPlayer = new List<int>();
        private static Dictionary<int, string> NameOfPlayer = new Dictionary<int, string>();
        private static Dictionary<int, int> AmountOfPlayerCards = new Dictionary<int, int>();
        
        //on start: create table with player
        //create label with name of players
        public static void ClearGameClass()
        {
            IdListPlayer = new List<int>();
            NameOfPlayer = new Dictionary<int, string>();
            AmountOfPlayerCards = new Dictionary<int, int>();
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
            AmountOfPlayerCards = new Dictionary<int, int>();
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
            if (AmountOfPlayerCards.ContainsKey(idPlayer))
            {
                AmountOfPlayerCards[idPlayer] = idCard;
            }
            else
            {
                AmountOfPlayerCards.Add(idPlayer, idCard);
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
        public static List<int> IdListPlayerToStartGame = new List<int>();
        private static Dictionary<int, string> NameOfRoom = new Dictionary<int, string>();
        private static Dictionary<int, string> NameOfPlayers = new Dictionary<int, string>();
        
        public static void ClearGameRoom()
        {
            NameRoom = String.Empty;
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
            ClearGameRoom();

            Network.ConnectToServer();
            Network.BeginReceiveDataFromServer();

            NewRoom createRoom = new NewRoom();
            if (createRoom.ShowDialog() != true) return;
            //todo check correct set data from server
            ((MainWindow)Application.Current.MainWindow).CUserControl.Content = MyContentClassWindow.ChangeContent(ContNum.PlayersBoard);
            Network.SendCommand(GameSendCommand.StartGame);
        }

        public static void JoinToRoom()
        {//TODO
            ClearGameRoom();

            Network.ConnectToServer();
            Network.BeginReceiveDataFromServer();
            
            JoinWindowObj.WindowJoinRoom = new JoinRoom();
            JoinWindowObj.WindowJoinRoom.ShowDialog();
        }

        public static void StartGameFromJoinRoom(byte playerAmount)
        {//todo
            JoinWindowObj.WindowJoinRoom.Close();

            ((MainWindow)Application.Current.MainWindow).CUserControl.Content 
                = MyContentClassWindow.ChangeContent(ContNum.PlayersBoard);

            ((MainWindow) Application.Current.MainWindow).CUserControl.Content
                = new PlayersTableManager(playerAmount);
            
        }
    }

    public static class JoinWindowObj
    {
        public static JoinRoom WindowJoinRoom = new JoinRoom();
    }
}
