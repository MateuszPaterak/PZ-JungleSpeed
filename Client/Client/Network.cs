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
        private static byte[] _byteData = new byte[100];
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
                MessageBox.Show("Nie udało się nawiązać połączenia!");
            }
        }

        public static void BeginReceiveDataFromServer()
        {
            try
            {
                if (MyClient != null && MyClient.Connected)
                {
                    MyClient.Client.BeginReceive(_byteData,
                        0,
                        _byteData.Length,
                        SocketFlags.None,
                        OnReceive,
                        MyClient);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private static void OnReceive(IAsyncResult arg)
        {
            try
            {
                MyClient.Client.EndReceive(arg);
               // MessageBox.Show("Odebrano od serwera: " + Encoding.UTF8.GetString(_byteData));
                _byteData = new byte[100];

                MyClient.Client.BeginReceive(_byteData, //wait for new next messages
                    0,
                    _byteData.Length,
                    SocketFlags.None,
                    OnReceive,
                    null);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Błąd odbioru danych. "+ex);
            }
        }

        public static void Disconnect()
        {
            if (MyClient!=null && MyClient.Connected)
            {
                _byteData = new byte[2];
                _byteData[1] = 53;//code
                _byteData[0] = Convert.ToByte(_byteData.Length);
                BeginSendComm(_byteData);

                MyClient.Close();
                MessageBox.Show("Rozłączono z serwerem");
            }
            else
            {
                //MessageBox.Show("Klient był już rozłączony");
            }
        }

        public static void SendCommand(GameSendCommand command, byte[] arg=null)
        {
            switch (command)
            {
                case GameSendCommand.GetTotem:
                {
                        _byteData = new byte[2];
                        _byteData[1] = 50;//code
                        _byteData[0] = Convert.ToByte(_byteData.Length);

                        BeginSendComm(_byteData);
                        break;
                }
                case GameSendCommand.PlayOff:
                    {
                        _byteData = new byte[3];
                        if (arg != null)
                        {
                            _byteData[2] = arg[0];
                        }
                        else
                        {
                            _byteData[2] = Convert.ToByte(5);
                        }
                        _byteData[1] = 51;//code
                        _byteData[0] = Convert.ToByte(_byteData.Length);
                        BeginSendComm(_byteData);
                        break;
                    }
                case GameSendCommand.GetUpMyCard:
                {
                        _byteData = new byte[2];
                        _byteData[1] = 52;//code
                        _byteData[0] = Convert.ToByte(_byteData.Length);
                        BeginSendComm(_byteData);
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

                        _byteData = new byte[len];
                        _byteData[1] = 54;//code
                        Array.Copy(namebyte, 0, _byteData, 2, _byteData.Length);
                        _byteData[0] = Convert.ToByte(_byteData.Length);
                        
                        BeginSendComm(_byteData);
                        break;
                    }
                case GameSendCommand.GetListAllRoom:
                    {
                        _byteData = new byte[2];
                        _byteData[1] = 55;//code
                        _byteData[0] = Convert.ToByte(_byteData.Length);
                        BeginSendComm(_byteData);
                        break;
                    }
                case GameSendCommand.GetListAllPlayerInRoom:
                    {
                        _byteData = new byte[3];
                        _byteData[1] = 56;//code
                        if (arg == null)
                        {
                            _byteData[2] = Convert.ToByte(255);
                        }
                        else
                        {
                            _byteData[2] = arg[0];
                        }
                        _byteData[0] = Convert.ToByte(_byteData.Length);
                        BeginSendComm(_byteData);
                        break;
                    }
                case GameSendCommand.GetListAllPlayerInRoomToStartGame:
                    {

                        _byteData = new byte[3];
                        _byteData[1] = 57;//code
                        if (arg == null)
                        {
                            _byteData[2] = Convert.ToByte(255);
                        }
                        else
                        {
                            _byteData[2] = arg[0];
                        }
                        _byteData[0] = Convert.ToByte(_byteData.Length);
                        BeginSendComm(_byteData);
                        break;
                    }
                case GameSendCommand.JoinToRoom:
                    {
                        _byteData = new byte[3];
                        _byteData[1] = 58;//code
                        if (arg == null)
                        {
                            _byteData[2] = Convert.ToByte(255);
                        }
                        else
                        {
                            _byteData[2] = arg[0];
                        }
                        _byteData[0] = Convert.ToByte(_byteData.Length);
                        BeginSendComm(_byteData);
                        break;
                    }
                case GameSendCommand.ExitFromRoom:
                    {

                        _byteData = new byte[2];
                        _byteData[1] = 59;//code
                        _byteData[0] = Convert.ToByte(_byteData.Length);
                        BeginSendComm(_byteData);
                        break;
                    }
                case GameSendCommand.JoinToGameInRoom:
                    {

                        _byteData = new byte[2];
                        _byteData[1] = 60;//code
                        _byteData[0] = Convert.ToByte(_byteData.Length);
                        BeginSendComm(_byteData);
                        break;
                    }
                case GameSendCommand.ExitFromGameInRoom:
                    {
                        _byteData = new byte[2];
                        _byteData[1] = 61;//code
                        _byteData[0] = Convert.ToByte(_byteData.Length);
                        BeginSendComm(_byteData);
                        break;
                    }
                case GameSendCommand.CreateNewRoom:
                    {
                        byte[] namebyte = Encoding.UTF8.GetBytes(GameRoom.NameRoom);
                        int len = namebyte.Length + 2;

                        _byteData = new byte[len];
                        _byteData[1] = 62;//code
                        Array.Copy(namebyte, 0, _byteData, 2, _byteData.Length);
                        _byteData[0] = Convert.ToByte(_byteData.Length);

                        BeginSendComm(_byteData);
                        break;
                    }
                case GameSendCommand.CloseMyRoom:
                    {
                        _byteData = new byte[2];
                        _byteData[1] = 63;//code
                        _byteData[0] = Convert.ToByte(_byteData.Length);
                        BeginSendComm(_byteData);
                        break;
                    }
                case GameSendCommand.RemovePlayerFromRoom:
                    {
                        _byteData = new byte[3];
                        _byteData[1] = 64;//code
                        if (arg == null)
                        {
                            _byteData[2] = Convert.ToByte(255);
                        }
                        else
                        {
                            _byteData[2] = arg[0];
                        }
                        _byteData[0] = Convert.ToByte(_byteData.Length);
                        BeginSendComm(_byteData);
                        break;
                    }
                case GameSendCommand.StartGame:
                    {
                        _byteData = new byte[2];
                        _byteData[1] = 65;//code
                        _byteData[0] = Convert.ToByte(_byteData.Length);
                        BeginSendComm(_byteData);
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
                _byteData = null;
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
