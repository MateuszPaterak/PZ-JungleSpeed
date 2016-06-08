using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Threading;

namespace Client
{
    public static class GameClass
    {
        public static string MyPlayerName { get; set; }
        private static List<int> _idListPlayer = new List<int>();
        private static Dictionary<byte, string> _nameOfPlayer = new Dictionary<byte, string>();
        private static Dictionary<int, int> _amountOfPlayerCards = new Dictionary<int, int>();
        private static Dictionary<byte, byte> _idPlayerToIdPlaceOnTable = new Dictionary<byte,byte>();
        //todo add MyIdPlayer
        
        public static void ClearGameClass()
        {
            _idListPlayer = new List<int>();
            _nameOfPlayer = new Dictionary<byte, string>();
            _amountOfPlayerCards = new Dictionary<int, int>();
            _idPlayerToIdPlaceOnTable = new Dictionary<byte, byte>();
        }
        public static void ClearIdListPlayer()
        {
            _idListPlayer = new List<int>();
        }
        public static void ClearNameOfPlayer()
        {
            _nameOfPlayer = new Dictionary<byte, string>();
        }
        public static void ClearAmountOPlayerCards()
        {
            _amountOfPlayerCards = new Dictionary<int, int>();
        }

        public static void AddPlayersName(byte idPlayer, string name)
        {
            if (_nameOfPlayer.ContainsKey(idPlayer))
            {
                _nameOfPlayer[idPlayer] = name;
            }
            else
            {
                _nameOfPlayer.Add(idPlayer, name);
            }
            
            if (!_idListPlayer.Exists(item => item == idPlayer))
            {
                _idListPlayer.Add(idPlayer);
            }
        }
        public static void AddPlayerAmountCard(int idPlayer, int idCard)
        {
            if (_amountOfPlayerCards.ContainsKey(idPlayer))
            {
                _amountOfPlayerCards[idPlayer] = idCard;
            }
            else
            {
                _amountOfPlayerCards.Add(idPlayer, idCard);
            }
            
            if (!_idListPlayer.Exists(item => item == idPlayer))
            {
                _idListPlayer.Add(idPlayer);
            }
        }

        public static void ChangeCardAtThePlayer(byte idPlayer, byte idCard)
        {
            try
            {
                var idPlaceOnTable = _idPlayerToIdPlaceOnTable[idPlayer];
                PlayersTableManager.ChangePlayerCard(idPlaceOnTable, idCard);
                PlayersTableManager.ChangeCardRandomRotation(idPlaceOnTable);
            }
            catch (Exception)
            {
            }
        }
        public static void SetNewPlayerName(byte idPlayer, string name)
        {
            try
            {
                byte idOnTable = _idPlayerToIdPlaceOnTable[idPlayer];
                PlayersTableManager.ChangeNamePlayer(idOnTable, name);
            }
            catch (Exception)
            {
            }
        }
        public static void SetAllNewPlayerName()
        {
            foreach (int idPlayer in _idListPlayer)
            {
                try
                {
                    byte id = Convert.ToByte(idPlayer);
                    string name = _nameOfPlayer[id];
                    SetNewPlayerName(id, name);
                }
                catch(Exception)
                { }
            }
        }
        

        /// <summary>
        /// To convert IdPlayer to IdPlace at the table.
        /// IdPlace are number from 0 to AmountOfPlayers
        /// </summary>
        public static void AddIdPlaceIdPlayer()
        {
            byte count = 0;
            foreach (int  idPlayer in _idListPlayer)
            {
                try
                {
                    byte id = Convert.ToByte(idPlayer);
                    if (_idPlayerToIdPlaceOnTable.ContainsKey(id))
                    {
                        _idPlayerToIdPlaceOnTable[id] = count;
                    }
                    else
                    {
                        _idPlayerToIdPlaceOnTable.Add(id, count);
                    }
                    count++;
                }
                catch(Exception)
                { }
            }
        }
    
        public static string GetPlayerNameFromId(byte idPlayer)
        {
            return _nameOfPlayer[idPlayer];
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
        //todo add MyIdRoom
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
            if (!IdListRoom.Exists(item => item == id))
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
            if (!IdListPlayerInRoom.Exists(item => item == idPlayer))
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
            if (!IdListPlayerToStartGame.Exists(item => item == idPlayer))
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
            JoinWindowObj.WindowJoinRoom = null;

            if (!Network.ConnectToServer()) return; //check the connection end return when not connected
            Network.BeginReceiveDataFromServer();

            NewRoom createRoom = new NewRoom();

            if (createRoom.ShowDialog() != true) return;
            if(createRoom.DialogResult != true) return;

            Network.SendCommand(GameSendCommand.StartGame);
            //At this moment I Step: server will control step of the game
        }

        public static void JoinToRoom()
        {
            ClearGameRoom();

            if (!Network.ConnectToServer()) return; //check the connection end return when not connected
            Network.BeginReceiveDataFromServer();

            try
            {
                JoinWindowObj.WindowJoinRoom = new JoinRoom();
                JoinWindowObj.WindowJoinRoom.ShowDialog();
            }
            catch (Exception)
            {
                //ignored
            }

            //waiting for control from server
        }

        public static void InitGame(byte playerAmount)
        {
            try
            {
                if (JoinWindowObj.WindowJoinRoom != null)
                {
                    if (JoinWindowObj.WindowJoinRoom.Dispatcher.CheckAccess())
                        JoinWindowObj.WindowJoinRoom.Close();
                    else
                        JoinWindowObj.WindowJoinRoom.Dispatcher.Invoke(
                            JoinWindowObj.WindowJoinRoom.Close);

                    //JoinWindowObj.WindowJoinRoom.Close();
                }

            }
            catch (Exception )
            {
                //MessageBox.Show(ex.ToString());
                //ignored
            }

            try
            {
            Application.Current.Dispatcher.BeginInvoke(
                            DispatcherPriority.Background,
                            new Action(() => ((MainWindow)Application.Current.MainWindow).CUserControl.Content
                            = MyContentClassWindow.ChangeContent(ContNum.PlayersBoard)));
            }
            catch(Exception )
            { }

            try
            {
            Application.Current.Dispatcher.BeginInvoke(
                        DispatcherPriority.Background,
                        new Action(() => ((MainWindow)Application.Current.MainWindow).CUserControl.Content
                        = new PlayersTableManager(playerAmount)));
            }
            catch (Exception )
            {
            }

            try
            {
                Application.Current.Dispatcher.BeginInvoke(
                            DispatcherPriority.Background,
                            new Action(GameClass.AddIdPlaceIdPlayer)); //create list of idPlace at the table);
            }
            catch (Exception )
            {
            }

            
            try
            {
                Application.Current.Dispatcher.BeginInvoke(
                            DispatcherPriority.Background,
                            new Action(GameClass.SetAllNewPlayerName));
            }
            catch (Exception )
            {
            }
            //waiting for control at the gamelogic from server
        }
    }

    public static class JoinWindowObj
    {
        public static JoinRoom WindowJoinRoom;
    }
}
