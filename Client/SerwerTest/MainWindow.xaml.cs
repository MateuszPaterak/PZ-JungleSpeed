using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
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

        private int port;
        private TcpListener serwer;
        private TcpClient klient;
        private BinaryWriter WriteToClient;
        private BinaryReader ReadFromClient;
        private delegate void SetTextCallBack(string tekst);

        //obługa nasłuchiwania serwera
        private void AcceptTcpClientCallback(IAsyncResult asyncResult) //metoda zwrotna wykonywana w momencie połączenia z klientem
        {
            TcpListener s = (TcpListener)asyncResult.AsyncState; //lokalny obiekt serwera komunikujący się z klientem 
            klient = s.EndAcceptTcpClient(asyncResult); //kończy operację połączenia z klientem
            //TbDisplay.Text += ("Połączenie z kientem powiodło się. \n");
            ChangeToTbDisplay("Połączenie z kientem powiodło się. \n");  //wyświetl napis z odpowiedniego wątku 

            WriteToClient = new BinaryWriter(klient.GetStream());
            ReadFromClient = new BinaryReader(klient.GetStream());
            //WriteToClient.Write("GetThisData");
            while (true)
            {
                string data = null;
                try
                {
                     data = ReadFromClient.ReadString();
                }
                catch (Exception ex)
                {
                    
                }
            
            if(data != null)
                ChangeToTbDisplay("Odebrano: " + data + "\n");
            
            }
        }

        private void ChangeToTbDisplay(string tekst) //przełączenie się na wątek obsługujący listBoxa z innego wątku
        {
            if (!TbDisplay.Dispatcher.CheckAccess())
                //sprawdzenie czy jest się w wątku obsługującym tę kontrolkę, 
                //jeśli nie to trzeba w dalszym kroku przełączyć się na niego
            {
                SetTextCallBack f = new SetTextCallBack(ChangeToTbDisplay); //utwórz wskaźnik na tą bieżącą funkcję
                Dispatcher.Invoke(f, new object[] { tekst });  //przełączenie się na wątek naszej kontrolki
            }
            else
            {
                TbDisplay.Text += (tekst);
            }
        }

        private void BtExecute_Click(object sender, RoutedEventArgs e)
        {
            WriteToClient.Write(TbCommand.Text.ToString());
            TbCommand.Text = "";
        }

        private void BtListen_Click(object sender, RoutedEventArgs e)
        {
            TbDisplay.Text += ("Oczekiwanie na połączenie ...\n");
            IPAddress adresIP;
            try
            {
                adresIP = IPAddress.Parse(TbIP.Text);
            }
            catch
            {
                MessageBox.Show("Błędny format adresu IP!");
                TbIP.Text = String.Empty;
                return;
            }

            port = System.Convert.ToInt16(TbPort.Text);
            //int port = System.Convert.ToInt16(4000);
            try
            {
                serwer = new TcpListener(adresIP, port);
                serwer.Start();
                serwer.BeginAcceptTcpClient(
                    new AsyncCallback(AcceptTcpClientCallback),
                    serwer); //asynchroniczne nasłuchiwanie 
                TbDisplay.Text += "Serwer wystartował.\n";
            }
            catch (Exception ex)
            {
                TbDisplay.Text +=("Błąd: " + ex.Message+"\n");
            }
        }
        
        private void BrStopServer_Click(object sender, RoutedEventArgs e)
        {
            if (serwer != null)
            {
                serwer.Stop();
                TbDisplay.Text += "Zakończono pracę serwera\n";
            }
            if (klient != null)
            {
                klient.Close();
                TbDisplay.Text += "Zakończono pracę klienta\n";
            }
            
        }
    }
}
