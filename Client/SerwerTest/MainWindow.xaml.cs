using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows;

namespace SerwerTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private int _port;
        private TcpListener _server;
        private TcpClient _client;
        private delegate void DelegForTextCallBack(string tekst);
        private byte[] _byteData = new byte[100];

        private void WriteToTbDisplay(string msg) //przełączenie się na wątek obsługujący listBoxa z innego wątku
        {
            if (!TbDisplay.Dispatcher.CheckAccess())
                //sprawdzenie czy jest się w wątku obsługującym tę kontrolkę, 
                //jeśli nie to trzeba w dalszym kroku przełączyć się na niego
            {
                DelegForTextCallBack f = WriteToTbDisplay; //utwórz wskaźnik na tą bieżącą funkcję
                Dispatcher.Invoke(f, msg);  //przełączenie się na wątek naszej kontrolki
            }
            else
            {
                TbDisplay.Text += (msg);
            }
        }

        private void WriteToTbDisplayNewline(string msg)
        {
            msg += "\n";
            WriteToTbDisplay(msg);
        }

        private void BtExecute_Click(object sender, RoutedEventArgs e)
        {
            //WriteToClient.Write(TbCommand.Text.ToString());
            try
            {
                _byteData = new byte[100];
                _byteData = Encoding.UTF8.GetBytes(TbCommand.Text);

                _client.Client.BeginSend(
                    _byteData,
                    0,
                    _byteData.Length,
                    SocketFlags.None,
                    SendToClient,
                    null);

                _byteData = new byte[100];
                TbCommand.Text = "";
            }
            catch (Exception ex)
            {
                WriteToTbDisplayNewline("Błąd podczas wysyłania: "+ ex);
            }
        }

        private void SendToClient(IAsyncResult arg)
        {
            try
            {
                _client.Client.EndSend(arg);
            }
            catch (Exception ex)
            {
                WriteToTbDisplayNewline("Błąd wysłania danych. " + ex);
            }
        }

        private void BtListen_Click(object sender, RoutedEventArgs e)
        {
            IPAddress adresIp;
            try
            {
                adresIp = IPAddress.Parse(TbIP.Text);
            }
            catch(Exception ex)
            {
                WriteToTbDisplayNewline("Błędny format adresu IP! " + ex);
                TbIP.Text = string.Empty;
                return;
            }

            _port = Convert.ToInt16(TbPort.Text);

            try
            {
                _server = new TcpListener(adresIp, _port);
                _server.Start();
                _server.BeginAcceptTcpClient(
                    AcceptTcpClientCallback,
                    _server); //asynchroniczne nasłuchiwanie na nadejście klienta
                WriteToTbDisplayNewline("Serwer wystartował.");
                WriteToTbDisplayNewline("Oczekiwanie na połączenie ...");
            }
            catch (Exception ex)
            {
                WriteToTbDisplayNewline("Błąd: " + ex.Message);
            }
        }

        //obługa nasłuchiwania serwera
        private void AcceptTcpClientCallback(IAsyncResult asyncResult) //metoda zwrotna wykonywana w momencie połączenia z klientem
        {
            try
            {
                TcpListener serv = (TcpListener) asyncResult.AsyncState; //lokalny obiekt serwera komunikujący się z klientem 
                _client = serv.EndAcceptTcpClient(asyncResult); //kończy operację połączenia z klientem
                WriteToTbDisplayNewline("Połączenie z klientem powiodło się."); //wyświetl napis z odpowiedniego wątku 
            }
            catch (Exception)
            {
                //WriteToTbDisplayNewline(ex.ToString());
            }

            //run reveive data mode
            if (_client == null) return;
            if (!_client.Connected) return;
            try
            {
                _client.Client.BeginReceive(_byteData,
                    0,
                    _byteData.Length,
                    SocketFlags.None,
                    ReceiveDataFromClient,
                    _client);
            }
            catch (Exception ex)
            {
                WriteToTbDisplayNewline("Błąd odbiarania danych: " + ex);
            }
        }

        //wykonywanie rekurencyjne odbioru danych
        private void ReceiveDataFromClient(IAsyncResult arg)
        {
            if (_client == null) return;
            if (!_client.Connected) return;
            try
            {
                _client.Client.EndReceive(arg);
                
                var datalength = _byteData[0];
                byte[] arraytmp = new byte[datalength];
                Array.Copy(_byteData, arraytmp, datalength);
                WriteToTbDisplayNewline("Odebrano od klienta: " + Encoding.UTF8.GetString(arraytmp));
                WriteToTbDisplayNewline("Długość całkowita: " + Convert.ToString(arraytmp[0]));
                WriteToTbDisplayNewline("Kod komunikatu: " + Convert.ToString(arraytmp[1]));

                _byteData = new byte[1000];

                _client.Client.BeginReceive(
                    _byteData, //wait for new next messages
                    0,
                    _byteData.Length,
                    SocketFlags.None,
                    ReceiveDataFromClient,
                    null);

            }
            catch (Exception ex)
            {
                WriteToTbDisplayNewline("Błąd odbioru danych. " + ex);
            }
        }

        private void BrStopServer_Click(object sender, RoutedEventArgs e)
        {
            if (_server != null)//&& _server.Server.Connected
            {
                try
                {
                    _server.Stop();
                    _server = null;
                    WriteToTbDisplayNewline("Zakończono pracę serwera");
                }
                catch (Exception ex)
                {
                    WriteToTbDisplayNewline(ex.ToString());
                }
            }
            if (_client != null )//&& _client.Connected
            {
                try
                {
                    _client.Close();
                    //ReadFromClient = null;
                    //WriteToClient = null;
                    _client = null;
                    WriteToTbDisplayNewline("Zakończono pracę klienta");
                }
                catch (Exception ex)
                {
                    WriteToTbDisplayNewline(ex.ToString());
                }
            }
        }
    }
}
