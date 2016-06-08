using System;
using System.Net.Sockets;
using System.Text;
using System.Windows;
using System.Windows.Threading;

namespace Client
{
    public static class Network
    {
        public static TcpClient MyClient;
        private static byte[] _byteDataSend = new byte[100];
        private static byte[] _byteDataReceive = new byte[1000];
        private static int port = 1000;
        public static string Host = "127.0.0.1";
        private static bool _runnignReceive;
        
        public static bool ConnectToServer()
        {
            var portOffset = 0;
            if (MyClient==null)//not exist
            {
                do
                {
                    try
                    {
                        MyClient = new TcpClient(Host, port+ portOffset);
                        MessageBox.Show("Nawiązano połączenie z " + Host);
                        return true;
                    }
                    catch (Exception)
                    {
                            // ignored
                    }
                    portOffset++;
                } while (portOffset<11);
            }
            if (MyClient == null) //not connected and exist after trying to connect
            {
                MessageBox.Show("Nie udało się nawiązać połączenia z serwerem");
                return false;
            }

            try
            {
                if (MyClient.Connected) return true; //if exist before and connected
            }
            catch(Exception)
            { }

            //not connected before but exist
            portOffset = 0;
            do
            {
                try
                {
                    MyClient = new TcpClient(Host, port + portOffset);
                    MessageBox.Show("Nawiązano połączenie z serwerem " + Host);
                    return true;
                }
                catch (Exception)
                {
                    // ignored
                }
                portOffset++;
            } while (portOffset < 11);

            MessageBox.Show("Nie udało się nawiązać połączenia z serwerem");
            return false; //exist but not connected after trying to connect
        }

        public static void BeginReceiveDataFromServer()
        {
            try
            {
                if (MyClient == null) return;
                if (!MyClient.Connected) return;

                _runnignReceive = true;

                MyClient.Client.BeginReceive(
                        _byteDataReceive,
                        0,
                        _byteDataReceive.Length,
                        SocketFlags.None,
                        OnReceiveLoop,
                        MyClient);
            }
            catch (Exception)
            {
                //ignored
            }
        }

        private static void OnReceiveLoop(IAsyncResult arg)
        {
            try
            {
                if (MyClient == null)
                {
                    return;
                }
                if (!MyClient.Connected)
                {
                    try
                    {
                        MyClient.Client.EndReceive(arg);
                    }
                    catch (Exception)
                    {
                        //ignored
                    }
                    return;
                }
                
                MyClient.Client.EndReceive(arg);

                if (!_runnignReceive) return; //return when flag is disabled

                try
                {
                    ReceiveCommand();
                }
                catch (Exception)
                {
                }
                

                _byteDataReceive = new byte[1000];

                MyClient.Client.BeginReceive(
                    _byteDataReceive, //wait for new next messages
                    0,
                    _byteDataReceive.Length,
                    SocketFlags.None,
                    OnReceiveLoop,
                    MyClient);
            }
            catch (Exception )
            {
             //   MessageBox.Show("Network.OnReceiveLoop :" + ex);
                //_runnignReceive = false;
            }
        }

        public static void Disconnect()
        {
            try
            {
                if (MyClient != null && MyClient.Connected)
                {
                    _byteDataSend = new byte[2];
                    _byteDataSend[1] = 53; //code
                    _byteDataSend[0] = Convert.ToByte(_byteDataSend.Length);
                    BeginSendComm(_byteDataSend);

                    MyClient.Close();
                    //MessageBox.Show("Rozłączono z serwerem");
                }
                else
                {
                    //MessageBox.Show("Klient był już rozłączony");
                }
            }
            catch (Exception)
            {
                //ignored
            }
        }

        private static void ReceiveCommand()
        {
            if(_byteDataReceive==null)   return;
            if(MyClient == null) return;
            if (!MyClient.Connected) return;

            switch (_byteDataReceive[1])
            {
                case 1:
                    {//receive amount of card all players and start game
                        //GameClass.ClearIdListPlayer();
                        GameClass.ClearAmountOPlayerCards();
                        var amountOfPlayers = _byteDataReceive[2];
                        for (var i = 0; i < amountOfPlayers; i++)
                            {
                                var idPlayer = _byteDataReceive[3 + i*2];
                                var amountOfCards = _byteDataReceive[4 + i*2];
                                GameClass.AddPlayerAmountCard(idPlayer,amountOfCards);
                            }
                        GameRoom.InitGame(amountOfPlayers);
                        break;
                    }
                case 2:
                    {//receive list of player name in game
                        GameClass.ClearNameOfPlayer();
                        GameClass.ClearIdListPlayer();
                        var amountOfPlayers = _byteDataReceive[2];
                        var pointerToRead = 3;

                        for (var i = 0; i < amountOfPlayers; i++)
                        {

                            byte idPlayer = _byteDataReceive[pointerToRead];
                            int lengthOfLogin = _byteDataReceive[pointerToRead+1];
                            var login = new byte[lengthOfLogin];
                            Array.Copy(_byteDataReceive, pointerToRead+2, login, 0, lengthOfLogin);
                            GameClass.AddPlayersName(idPlayer, Encoding.UTF8.GetString(login));
                            pointerToRead += (2 + lengthOfLogin);
                        }
                        break;
                    }
                case 3:
                    {//enable button GetMyCard
                        if (Application.Current.MainWindow.Dispatcher.CheckAccess())
                            ((MainWindow)Application.Current.MainWindow).BtGetUpCard.IsEnabled = true;
                        else
                            JoinWindowObj.WindowJoinRoom.Dispatcher.Invoke(
                                 DispatcherPriority.Background,
                                new Action (()=>((MainWindow)Application.Current.MainWindow).BtGetUpCard.IsEnabled = true));
                        break;
                    }
                case 4:
                    {//disable button GetMyCard
                        if (Application.Current.MainWindow.Dispatcher.CheckAccess())
                            ((MainWindow)Application.Current.MainWindow).BtGetUpCard.IsEnabled = false;
                        else
                            JoinWindowObj.WindowJoinRoom.Dispatcher.Invoke(
                                 DispatcherPriority.Background,
                                new Action(() => ((MainWindow)Application.Current.MainWindow).BtGetUpCard.IsEnabled = false));
                        break;
                    }
                case 5:
                    {//display card at the player
                        var idPlayer = _byteDataReceive[2];
                        var idCard = _byteDataReceive[3];
                        GameClass.ChangeCardAtThePlayer(idPlayer,idCard);
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
                        var amountOfRooms = _byteDataReceive[2];
                        var pointerToRead = 3;

                        for (var i = 0; i < amountOfRooms; i++)
                        {
                            int idRoom = _byteDataReceive[pointerToRead];
                            int lengthOfName = _byteDataReceive[pointerToRead + 1];
                            var name = new byte[lengthOfName];
                            Array.Copy(_byteDataReceive, pointerToRead + 2, name, 0, lengthOfName);
                            GameRoom.AddRoomAndId(idRoom,Encoding.UTF8.GetString(name));
                            pointerToRead += (2 + lengthOfName);
                        }
                        break;
                    }
                case 9:
                    {//list of player name +ID in room
                        GameRoom.ClearNameOfPlayers();
                        GameRoom.ClearIdListPlayerInRoom();
                        var amountOfPlayers = _byteDataReceive[2];
                        var pointerToRead = 3;

                        for (var i = 0; i < amountOfPlayers; i++)
                        {
                            int idPlayer = _byteDataReceive[pointerToRead];
                            int lengthOfLogin = _byteDataReceive[pointerToRead + 1];
                            var login = new byte[lengthOfLogin];
                            Array.Copy(_byteDataReceive, pointerToRead + 2, login, 0, lengthOfLogin);
                            GameRoom.AddPlayerNameInRoom(idPlayer, Encoding.UTF8.GetString(login));
                            pointerToRead += (2 + lengthOfLogin);
                        }
                        break;
                    }
                case 10:
                    {//list of players+ID to start game in room
                        GameRoom.ClearIdListPlayerToStartGame();
                        GameRoom.ClearNameOfPlayers();
                        var amountOfPlayers = _byteDataReceive[2];
                        var pointerToRead = 3;

                        for (var i = 0; i < amountOfPlayers; i++)
                        {
                            int idPlayer = _byteDataReceive[pointerToRead];
                            int lengthOfLogin = _byteDataReceive[pointerToRead + 1];
                            var login = new byte[lengthOfLogin];
                            Array.Copy(_byteDataReceive, pointerToRead + 2, login, 0, lengthOfLogin);
                            GameRoom.AddPlayerToStartGame(idPlayer, Encoding.UTF8.GetString(login));
                            pointerToRead += (2 + lengthOfLogin);
                        }
                        break;
                    }
                case 11:
                    {//end of game
                        //destruct/reset game object
                        var winnerId = _byteDataReceive[2];
                        string winnerName = GameClass.GetPlayerNameFromId(winnerId);
                        MessageBox.Show("Gra zakończona. \nWygrał gracz: "+ winnerName);

                        GameClass.ClearGameClass();

                        if (Application.Current.MainWindow.Dispatcher.CheckAccess())
                            ((MainWindow)Application.Current.MainWindow).CUserControl.Content = new UCMainScreen();
                        else
                            JoinWindowObj.WindowJoinRoom.Dispatcher.Invoke(
                                 DispatcherPriority.Background,
                                new Action(() => ((MainWindow)Application.Current.MainWindow).CUserControl.Content = new UCMainScreen()));
                        
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
                        Array.Copy(namebyte, 0, _byteDataSend, 2, namebyte.Length);
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
                        Array.Copy(namebyte, 0, _byteDataSend, 2, namebyte.Length);
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
                if (MyClient == null) return;
                if (!MyClient.Connected) return;

                MyClient.Client.BeginSend(
                data,
                0,
                data.Length,
                SocketFlags.None,
                EndSend,
                MyClient);
            }
            catch (Exception)
            {
                //ignored
            }
}

        private static void EndSend(IAsyncResult arg)
        {
            try
            {
                if (MyClient == null) return;
                if (!MyClient.Connected) return;

                MyClient.Client.EndSend(arg);
                _byteDataSend = new byte[1000];
            }
            catch (Exception)
            {
                //ignored
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
