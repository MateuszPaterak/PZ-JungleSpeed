using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Windows;

namespace Client
{
    public class Network
    {
        public static TcpClient MyClient;
        private static byte[] _byteDataSend = new byte[100];
        private static byte[] _byteDataReceve = new byte[100];
        private static int port = 1000;
        private static string host = "127.0.0.1";
        
        public static void ConnectToServer()
        {
            try
            {
                MyClient = new TcpClient(host, port);
                MessageBox.Show("Nawiązano połączenie z " + host + " na porcie " + port);
            }
            catch (Exception)
            {
                MessageBox.Show("Nie udało się nawiązać połączenia z serwerem");
            }
        }

        public static void BeginReceiveDataFromServer()
        {
            try
            {
                if (MyClient != null && MyClient.Connected)
                {
                    MyClient.Client.BeginReceive(
                        _byteDataReceve,
                        0,
                        _byteDataReceve.Length,
                        SocketFlags.None,
                        OnReceiveLoop,
                        MyClient);
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.ToString());
            }
        }

        private static void OnReceiveLoop(IAsyncResult arg)
        {
            if (MyClient == null || !MyClient.Connected) return;
            try
            {
                MyClient.Client.EndReceive(arg);
                ReceiveComand();
                // MessageBox.Show("Odebrano od serwera: " + Encoding.UTF8.GetString(_byteData));
                _byteDataReceve = null;                                             //->>>>problem with null

                MyClient.Client.BeginReceive(
                    _byteDataReceve, //wait for new next messages
                    0,
                    _byteDataReceve.Length,
                    SocketFlags.None,
                    OnReceiveLoop,
                    null);
            }
            catch (Exception ex)
            {
                //MessageBox.Show("Błąd odbioru danych. "+ex);
            }
        }

        public static void Disconnect()
        {
            if (MyClient!=null && MyClient.Connected)
            {
                _byteDataSend = new byte[2];
                _byteDataSend[1] = 53;//code
                _byteDataSend[0] = Convert.ToByte(_byteDataSend.Length);
                BeginSendComm(_byteDataSend);

                MyClient.Close();
                MessageBox.Show("Rozłączono z serwerem");
            }
            else
            {
                //MessageBox.Show("Klient był już rozłączony");
            }
        }

        private static void ReceiveComand()
        {
            if(_byteDataReceve==null)   return;

            switch (_byteDataReceve[1])
            {
                case 1:
                    {//receive amount of card all players
                        GameClass.ClearIdListPlayer();
                        GameClass.ClearAmountOPlayerCards();
                        var amountOfPlayers = _byteDataReceve[2];
                        for (var i = 0; i < amountOfPlayers; i++)
                            {
                                var idPlayer = _byteDataReceve[3 + i*2];
                                var amountOfCards = _byteDataReceve[4 + i*2];
                                GameClass.AddPlayerAmountCard(idPlayer,amountOfCards);
                            }
                        GameRoom.StartGameFromJoinRoom(amountOfPlayers);
                        break;
                    }
                case 2:
                    {//receive list of player name in game
                        GameClass.ClearNameOfPlayer();
                        GameClass.ClearIdListPlayer();
                        var amountOfPlayers = _byteDataReceve[2];
                        var pointerToRead = 3;

                        for (var i = 0; i < amountOfPlayers; i++)
                        {

                            int idPlayer = _byteDataReceve[pointerToRead];
                            int lengthOfLogin = _byteDataReceve[pointerToRead+1];
                            var login = new byte[lengthOfLogin];
                            Array.Copy(_byteDataReceve, pointerToRead+2, login, 0, lengthOfLogin);
                            GameClass.AddPlayersName(idPlayer, Encoding.UTF8.GetString(login));
                            pointerToRead += (2 + lengthOfLogin);
                        }
                        break;
                    }
                case 3:
                    {//enable button GetMyCard
                        ((MainWindow)Application.Current.MainWindow).BtGetUpCard.IsEnabled = true;
                        break;
                    }
                case 4:
                    {//disable button GetMyCard
                        ((MainWindow)Application.Current.MainWindow).BtGetUpCard.IsEnabled = false;
                        break;
                    }
                case 5:
                    {//display card at the player
                        var idPlayer = _byteDataReceve[2];
                        var idCard = _byteDataReceve[3];
                        PlayersTableManager.ChangePlayerCard(idPlayer,idCard);
                        PlayersTableManager.ChangeCardRandomRotation(idPlayer);
                        break;
                    }
                case 6:
                    {//message to winner of the fight for totem
                        MessageBox.Show("Wygrałeś pojedynek o totem!!!");
                        break;
                    }
                case 7:
                    {//message to loser of the fight for totem
                        MessageBox.Show("Przegrałeś pojedynek o totem!!!");
                        break;
                    }
                case 8:
                    {//list of room and ID
                        GameRoom.ClearIdListRoom();
                        GameRoom.ClearNameOfRoom();
                        var amountOfRooms = _byteDataReceve[2];
                        var pointerToRead = 3;

                        for (var i = 0; i < amountOfRooms; i++)
                        {
                            int idRoom = _byteDataReceve[pointerToRead];
                            int lengthOfName = _byteDataReceve[pointerToRead + 1];
                            var name = new byte[lengthOfName];
                            Array.Copy(_byteDataReceve, pointerToRead + 2, name, 0, lengthOfName);
                            GameRoom.AddRoomAndId(idRoom,Encoding.UTF8.GetString(name));
                            pointerToRead += (2 + lengthOfName);
                        }
                        break;
                    }
                case 9:
                    {//list of player name +ID in room
                        GameRoom.ClearNameOfPlayers();
                        GameRoom.ClearIdListPlayerInRoom();
                        var amountOfPlayers = _byteDataReceve[2];
                        var pointerToRead = 3;

                        for (var i = 0; i < amountOfPlayers; i++)
                        {
                            int idPlayer = _byteDataReceve[pointerToRead];
                            int lengthOfLogin = _byteDataReceve[pointerToRead + 1];
                            var login = new byte[lengthOfLogin];
                            Array.Copy(_byteDataReceve, pointerToRead + 2, login, 0, lengthOfLogin);
                            GameRoom.AddPlayerNameInRoom(idPlayer, Encoding.UTF8.GetString(login));
                            pointerToRead += (2 + lengthOfLogin);
                        }
                        break;
                    }
                case 10:
                    {//list of players+ID to start game in room
                        GameRoom.ClearIdListPlayerToStartGame();
                        GameRoom.ClearNameOfPlayers();
                        var amountOfPlayers = _byteDataReceve[2];
                        var pointerToRead = 3;

                        for (var i = 0; i < amountOfPlayers; i++)
                        {
                            int idPlayer = _byteDataReceve[pointerToRead];
                            int lengthOfLogin = _byteDataReceve[pointerToRead + 1];
                            var login = new byte[lengthOfLogin];
                            Array.Copy(_byteDataReceve, pointerToRead + 2, login, 0, lengthOfLogin);
                            GameRoom.AddPlayerToStartGame(idPlayer, Encoding.UTF8.GetString(login));
                            pointerToRead += (2 + lengthOfLogin);
                        }
                        break;
                    }
                case 11://todo
                    {//end of game
                        GameClass.ClearGameClass();
                        ((MainWindow)Application.Current.MainWindow).CUserControl.Content = new UCMainScreen();
                        break;
                    }

            }
        }
        public static void SendCommand(GameSendCommand command, byte[] arg=null)
        {
            switch (command)
            {
                case GameSendCommand.GetTotem:
                {
                        _byteDataSend = new byte[2];
                        _byteDataSend[1] = 50;//code
                        _byteDataSend[0] = Convert.ToByte(_byteDataSend.Length);

                        BeginSendComm(_byteDataSend);
                        break;
                }
                case GameSendCommand.PlayOff:
                    {
                        _byteDataSend = new byte[3];
                        if (arg != null)
                        {
                            _byteDataSend[2] = arg[0];
                        }
                        else
                        {
                            _byteDataSend[2] = Convert.ToByte(5);
                        }
                        _byteDataSend[1] = 51;//code
                        _byteDataSend[0] = Convert.ToByte(_byteDataSend.Length);
                        BeginSendComm(_byteDataSend);
                        break;
                    }
                case GameSendCommand.GetUpMyCard:
                {
                        _byteDataSend = new byte[2];
                        _byteDataSend[1] = 52;//code
                        _byteDataSend[0] = Convert.ToByte(_byteDataSend.Length);
                        BeginSendComm(_byteDataSend);
                        break;
                }
                case GameSendCommand.Disconnect:
                {
                        Disconnect();
                        break;
                }
                
                case GameSendCommand.SendMyLogin:
                    {
                        //_byteData = Encoding.UTF8.GetBytes();
                        byte[] namebyte = Encoding.UTF8.GetBytes(GameClass.MyPlayerName);
                        int len = namebyte.Length + 2;

                        _byteDataSend = new byte[len];
                        _byteDataSend[1] = 54;//code
                        Array.Copy(namebyte, 0, _byteDataSend, 2, _byteDataSend.Length);
                        _byteDataSend[0] = Convert.ToByte(_byteDataSend.Length);
                        
                        BeginSendComm(_byteDataSend);
                        break;
                    }
                case GameSendCommand.GetListAllRoom:
                    {
                        _byteDataSend = new byte[2];
                        _byteDataSend[1] = 55;//code
                        _byteDataSend[0] = Convert.ToByte(_byteDataSend.Length);
                        BeginSendComm(_byteDataSend);
                        break;
                    }
                case GameSendCommand.GetListAllPlayerInRoom:
                    {
                        _byteDataSend = new byte[3];
                        _byteDataSend[1] = 56;//code
                        if (arg == null)
                        {
                            _byteDataSend[2] = Convert.ToByte(255);
                        }
                        else
                        {
                            _byteDataSend[2] = arg[0];
                        }
                        _byteDataSend[0] = Convert.ToByte(_byteDataSend.Length);
                        BeginSendComm(_byteDataSend);
                        break;
                    }
                case GameSendCommand.GetListAllPlayerInRoomToStartGame:
                    {

                        _byteDataSend = new byte[3];
                        _byteDataSend[1] = 57;//code
                        if (arg == null)
                        {
                            _byteDataSend[2] = Convert.ToByte(255);
                        }
                        else
                        {
                            _byteDataSend[2] = arg[0];
                        }
                        _byteDataSend[0] = Convert.ToByte(_byteDataSend.Length);
                        BeginSendComm(_byteDataSend);
                        break;
                    }
                case GameSendCommand.JoinToRoom:
                    {
                        _byteDataSend = new byte[3];
                        _byteDataSend[1] = 58;//code
                        if (arg == null)
                        {
                            _byteDataSend[2] = Convert.ToByte(255);
                        }
                        else
                        {
                            _byteDataSend[2] = arg[0];
                        }
                        _byteDataSend[0] = Convert.ToByte(_byteDataSend.Length);
                        BeginSendComm(_byteDataSend);
                        break;
                    }
                case GameSendCommand.ExitFromRoom:
                    {

                        _byteDataSend = new byte[2];
                        _byteDataSend[1] = 59;//code
                        _byteDataSend[0] = Convert.ToByte(_byteDataSend.Length);
                        BeginSendComm(_byteDataSend);
                        break;
                    }
                case GameSendCommand.JoinToGameInRoom:
                    {

                        _byteDataSend = new byte[2];
                        _byteDataSend[1] = 60;//code
                        _byteDataSend[0] = Convert.ToByte(_byteDataSend.Length);
                        BeginSendComm(_byteDataSend);
                        break;
                    }
                case GameSendCommand.ExitFromGameInRoom:
                    {
                        _byteDataSend = new byte[2];
                        _byteDataSend[1] = 61;//code
                        _byteDataSend[0] = Convert.ToByte(_byteDataSend.Length);
                        BeginSendComm(_byteDataSend);
                        break;
                    }
                case GameSendCommand.CreateNewRoom:
                    {
                        byte[] namebyte = Encoding.UTF8.GetBytes(GameRoom.NameRoom);
                        int len = namebyte.Length + 2;

                        _byteDataSend = new byte[len];
                        _byteDataSend[1] = 62;//code
                        Array.Copy(namebyte, 0, _byteDataSend, 2, _byteDataSend.Length);
                        _byteDataSend[0] = Convert.ToByte(_byteDataSend.Length);

                        BeginSendComm(_byteDataSend);
                        break;
                    }
                case GameSendCommand.CloseMyRoom:
                    {
                        _byteDataSend = new byte[2];
                        _byteDataSend[1] = 63;//code
                        _byteDataSend[0] = Convert.ToByte(_byteDataSend.Length);
                        BeginSendComm(_byteDataSend);
                        break;
                    }
                case GameSendCommand.RemovePlayerFromRoom:
                    {
                        _byteDataSend = new byte[3];
                        _byteDataSend[1] = 64;//code
                        if (arg == null)
                        {
                            _byteDataSend[2] = Convert.ToByte(255);
                        }
                        else
                        {
                            _byteDataSend[2] = arg[0];
                        }
                        _byteDataSend[0] = Convert.ToByte(_byteDataSend.Length);
                        BeginSendComm(_byteDataSend);
                        break;
                    }
                case GameSendCommand.StartGame:
                    {
                        _byteDataSend = new byte[2];
                        _byteDataSend[1] = 65;//code
                        _byteDataSend[0] = Convert.ToByte(_byteDataSend.Length);
                        BeginSendComm(_byteDataSend);
                        break;
                    }
            }
        }

        private static void BeginSendComm(byte[] data)
        {
            try
            {
                MyClient.Client.BeginSend(
                data,
                0,
                data.Length,
                SocketFlags.None,
                EndSend,
                null);
            }
            catch (Exception ex)
            {
                //MessageBox.Show("Błąd podczas wysyłania: " + ex);
            }
}
        private static void EndSend(IAsyncResult arg)
        {
            try
            {
                MyClient.Client.EndSend(arg);
                _byteDataSend = null;
            }
            catch (Exception ex)
            {
                //MessageBox.Show("Błąd żakończenia wysłania danych. " + ex);
            }
        }
    }

    public enum GameSendCommand
    {
        GetTotem,
        PlayOff,
        GetUpMyCard,
        Disconnect,
        SendMyLogin,
        GetListAllRoom,
        GetListAllPlayerInRoom,
        GetListAllPlayerInRoomToStartGame,
        JoinToRoom,
        ExitFromRoom,
        JoinToGameInRoom,
        ExitFromGameInRoom,
        CreateNewRoom,
        CloseMyRoom,
        RemovePlayerFromRoom,
        StartGame
    };
}
