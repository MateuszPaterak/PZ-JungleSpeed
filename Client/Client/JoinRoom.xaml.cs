using System.Windows;

namespace Client
{
    /// <summary>
    /// Interaction logic for JoinRoom.xaml
    /// </summary>
    public partial class JoinRoom : Window
    {
        public JoinRoom()
        {
            InitializeComponent();
        }

        private void btJoinToRoom_Click(object sender, RoutedEventArgs e)
        {//todo
        
        }

        private void btExitFromRoom_Click(object sender, RoutedEventArgs e)
        {//todo

        }

        private void btJoinToGame_Click(object sender, RoutedEventArgs e)
        {//todo
            //start game
            //DialogResult = true;
            //this.Close();
        }

        private void btExitFromGame_Click(object sender, RoutedEventArgs e)
        {//todo

        }

        private void BtGetRoomList_Click(object sender, RoutedEventArgs e)
        {//todo
            Network.SendCommand(GameSendCommand.GetListAllRoom);
        }
    }
}
