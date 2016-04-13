using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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

            CUserControl.Content = MyContentClassWindow.ChangeContent(ContNum.StartImg);
            //CUserControl.Content = MyContentClassWindow.ChangeContent(ContNum.PlayersBoard);

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

        private void GameLoop()
        {
            
        }

        private void BtGetUpCard_Click(object sender, RoutedEventArgs e)
        {
            //testy
            PlayOff po = new PlayOff();
            po.ShowDialog();
        }

        private void BtGetTotem_Click(object sender, RoutedEventArgs e)
        {

            CUserControl.Content = new PlayersTableManager(8);
            for (byte i = 0; i < 8; i++)
            {
                PlayersTableManager.ChangeNamePlayer("Test", i );
                byte tmp = Convert.ToByte( i%4 +1);
                PlayersTableManager.ChangePlayerCard(i, tmp );

            }
            
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
