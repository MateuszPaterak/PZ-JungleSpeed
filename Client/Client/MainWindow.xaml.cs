using System;
using System.IO;
using System.Net.Sockets;
using System.Windows;

namespace Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int nr = 1;
        public MainWindow()
        {
            InitializeComponent();

            //CUserControl.Content = MyContentClassWindow.ChangeContent(ContNum.StartImg);
            //CUserControl.Content = MyContentClassWindow.ChangeContent(ContNum.PlayersBoard);
            CUserControl.Content = new UCMainScreen();
        }

        private TcpClient klient;
        private BinaryReader ReadFromServer;

        private void NewRoom(object sender, RoutedEventArgs e)
        {
            NewRoom createRoom = new NewRoom();

            if (createRoom.ShowDialog() == true)
            {

            }
        }

        private void JoinToRoom(object sender, RoutedEventArgs e)
        {
            JoinRoom joinToRoom = new JoinRoom();
            joinToRoom.ShowDialog();
        }

        private void GameLoop()
        {
            
        }

        private void BtGetUpCard_Click(object sender, RoutedEventArgs e)
        {
            //testy
            //PlayOff po = new PlayOff();
            //po.ShowDialog();

            string host = "127.0.0.1"; //textBox1.Text;
            int port = 4000; //System.Convert.ToInt16(numericUpDown1.Value);
            try
            {
                klient = new TcpClient(host, port);
                ListView.Items.Add("Nawiązano połączenie z " + host + " na porcie: " + port);

                ReadFromServer = new BinaryReader(klient.GetStream());
            }
            catch (Exception ex)
            {
                MessageBox.Show("Błąd: Nie udało się nawiązać połączenia!");
                //MessageBox.Show(ex.ToString());
            }

            MessageBox.Show(ReadFromServer.ReadString());
        }

        private void BtGetTotem_Click(object sender, RoutedEventArgs e)
        {
            klient.Close();
            MessageBox.Show("Rozłączono klienta");
            /*
            CUserControl.Content = new PlayersTableManager(8);
            
            for (byte i = 0; i < 8; i++)
            {
                PlayersTableManager.ChangeNamePlayer("Test", i);
            }

            for (byte i = 0; i < 8; i++)
            {
                byte tmp = Convert.ToByte( i%4 +1);
                PlayersTableManager.ChangePlayerCard(i, tmp );
                
                PlayersTableManager.ChangeCardRandomRotation(i);
            }
            */
        }

        private void BtOutOfGameplay_Click(object sender, RoutedEventArgs e)
        {
            nr = ((nr+=1)%9 ) + (nr/9)*2;
            CUserControl.Content = new PlayersTableManager(Convert.ToByte(nr));
            for (int i = 0; i < nr; i++)
            {
                PlayersTableManager.ChangeNamePlayer("Test", Convert.ToByte(i));
                byte tmp = Convert.ToByte(i % 4 + 1);
                PlayersTableManager.ChangePlayerCard(Convert.ToByte(i), tmp);

            }
        }
    }
}
