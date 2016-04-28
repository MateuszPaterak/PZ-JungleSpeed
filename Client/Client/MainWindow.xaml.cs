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

        private void BtGetUpCard_Click(object sender, RoutedEventArgs e)
        {
            //testy
            //PlayOff po = new PlayOff();
            //po.ShowDialog();
            Network.SendCommand(GameSendCommand.GetUpMyCard);
        }

        private void BtGetTotem_Click(object sender, RoutedEventArgs e)
        {
            Network.SendCommand(GameSendCommand.GetTotem);
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
            Network.Disconnect();

            /*
            nr = ((nr+=1)%9 ) + (nr/9)*2;
            CUserControl.Content = new PlayersTableManager(Convert.ToByte(nr));
            for (int i = 0; i < nr; i++)
            {
                PlayersTableManager.ChangeNamePlayer("Test", Convert.ToByte(i));
                byte tmp = Convert.ToByte(i % 4 + 1);
                PlayersTableManager.ChangePlayerCard(Convert.ToByte(i), tmp);

            }
            */
        }
    }
}
