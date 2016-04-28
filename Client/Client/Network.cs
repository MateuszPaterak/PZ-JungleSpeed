using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Client
{
    class Network
    {
        public static TcpClient client;
        public static BinaryReader ReadFromServer;
        public static BinaryWriter WriteToServer;
        private static int port = 4000;
        private static string host = "127.0.0.1";

        public static void ConnectToServer()
        {
            try
            {
                Network.client = new TcpClient(host, port);
                MessageBox.Show("Nawiązano połączenie z " + host + " na porcie " + port);

                Network.ReadFromServer = new BinaryReader(Network.client.GetStream());
                Network.WriteToServer = new BinaryWriter(Network.client.GetStream());
            }
            catch (Exception ex)
            {
                MessageBox.Show("Nie udało się nawiązać połączenia!");
            }
        }

        public static void Disconnect()
        {
            if (Network.client != null)
            {
                try
                {
                    Network.WriteToServer.Write("DISCONNECT");
                }
                catch (Exception ex)
                {
                    
                }
                Network.client.Close();
                Network.WriteToServer = null;
                Network.ReadFromServer = null;
                MessageBox.Show("Rozłączono klienta");

                GameClass.SetGameMode(GameMode.Off);
            }
        }

        public static void SendCommand(GameSendCommand command)
        {
            switch (command)
            {
                    case GameSendCommand.GetTotem:
                {
                    try
                    {
                        Network.WriteToServer.Write("GETTOTEM");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Nie można wysłać danych. Brak połączenia z serwerem");
                    }
                    break;
                }

                    case GameSendCommand.GetUpMyCard:
                {
                        try
                        {
                            Network.WriteToServer.Write("GETUPMYCARD");
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Nie można wysłać danych. Brak połączenia z serwerem");
                        }
                        break;
                }
                    case GameSendCommand.Disconnect:
                {
                        Disconnect();
                    break;
                }
                default:
                    //throw new ArgumentOutOfRangeException(nameof(command), command, null);
                    break;
            }
        }
    }

    public enum GameSendCommand
    {
        GetUpMyCard,
        GetTotem,
        Disconnect
    };
}
