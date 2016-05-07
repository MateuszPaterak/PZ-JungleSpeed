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
        private static Dictionary<byte, string> NameOfPlayer = new Dictionary<byte, string>();
        private static Dictionary<int, int> AmountOfPlayerCards = new Dictionary<int, int>();
        private static Dictionary<byte, byte> IdPlayerToIdPlaceOnTable = new Dictionary<byte,byte>();

        //on start: create table with player
        //create label with name of players
        public static void ClearGameClass()
        {
            IdListPlayer = new List<int>();
            NameOfPlayer = new Dictionary<byte, string>();
            AmountOfPlayerCards = new Dictionary<int, int>();
            IdPlayerToIdPlaceOnTable = new Dictionary<byte, byte>();
        }
        public static void ClearIdListPlayer()
        {
            IdListPlayer = new List<int>();
        }
        public static void ClearNameOfPlayer()
        {
            NameOfPlayer = new Dictionary<byte, string>();
        }
        public static void ClearAmountOPlayerCards()
        {
            AmountOfPlayerCards = new Dictionary<int, int>();
        }

        public static void AddPlayersName(byte idPlayer, string name)
        {
            if (NameOfPlayer.ContainsKey(idPlayer))
            {
                NameOfPlayer[idPlayer] = name;
            }
            else
            {
                NameOfPlayer.Add(idPlayer, name);
            }
            
            if (!IdListPlayer.Contains(idPlayer))
            {
                IdListPlayer.Add(idPlayer);
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

        public static void ChangeCardAtThePlayer(byte idPlayer, byte idCard)
        {
            var idPlaceOnTable = IdPlayerToIdPlaceOnTable[idPlayer];
            PlayersTableManager.ChangePlayerCard(idPlaceOnTable, idCard);
            PlayersTableManager.ChangeCardRandomRotation(idPlaceOnTable);
        }
        public static void SetNewPlayerName(byte idPlayer, string name)
        {
            byte idOnTable = IdPlayerToIdPlaceOnTable[idPlayer];
            PlayersTableManager.ChangeNamePlayer(idOnTable, name);
        }
        public static void SetAllNewPlayerName()
        {
            foreach (byte idPlayer in IdListPlayer)
            {
                string name = NameOfPlayer[idPlayer];
                SetNewPlayerName(idPlayer,name);
            }
        }

        /// <summary>
        /// To convert IdPlayer to IdPlace at the table.
        /// IdPlace are number from 0 to AmountOfPlayers
        /// </summary>
        public static void AddIdPlaceIdPlayer()
        {
            byte count = 0;
            foreach (byte  idPlayer in IdListPlayer)
            {
                if (IdPlayerToIdPlaceOnTable.ContainsKey(idPlayer))
                {
                    IdPlayerToIdPlaceOnTable[idPlayer] = count;
                }
                else
                {
                    IdPlayerToIdPlaceOnTable.Add(idPlayer,count);
                }
                count++;
            }
        }
    }

    public static class GameRoom
    {
        public static string NameRoom { get; set; }
        public static List<int> IdListRoom = new List<int>();
        public static List<int> IdListPlayerInRoom = new List<int>();
        public static List<int> IdListPlayerToStartGame = new List<int>();
        public static Dictionary<int, string> NameOfRoom = new Dictionary<int, string>();
        public static Dictionary<int, string> NameOfPlayers = new Dictionary<int, string>();

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
        {
            ClearGameRoom();

            Network.ConnectToServer();
            Network.BeginReceiveDataFromServer();

            NewRoom createRoom = new NewRoom();
            if (createRoom.ShowDialog() != true) return;
            if(createRoom.DialogResult != true) return;
            //todo check correct set data from server??
            //((MainWindow)Application.Current.MainWindow).CUserControl.Content 
            //    = MyContentClassWindow.ChangeContent(ContNum.PlayersBoard);
            Network.SendCommand(GameSendCommand.StartGame);
            //At this moment, server will control step of the game
        }

        public static void JoinToRoom()
        {//TODO
            ClearGameRoom();

            Network.ConnectToServer();
            Network.BeginReceiveDataFromServer();
            
            JoinWindowObj.WindowJoinRoom = new JoinRoom();
            JoinWindowObj.WindowJoinRoom.ShowDialog();
        }

        public static void InitGame(byte playerAmount)
        {
            try
            {
                JoinWindowObj.WindowJoinRoom.Close();
            }
            catch (Exception)
            {
                //ignored
            }

            ((MainWindow)Application.Current.MainWindow).CUserControl.Content 
                = MyContentClassWindow.ChangeContent(ContNum.PlayersBoard);

            ((MainWindow) Application.Current.MainWindow).CUserControl.Content
                = new PlayersTableManager(playerAmount);

            GameClass.AddIdPlaceIdPlayer(); //create list of idPlace at the table

            GameClass.SetAllNewPlayerName();

            //waiting for control at the gamelogic from server
        }
    }

    public static class JoinWindowObj
    {
        public static JoinRoom WindowJoinRoom = new JoinRoom();
    }
}
