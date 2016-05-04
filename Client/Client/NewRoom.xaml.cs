using System;
using System.Net.Sockets;
using System.Windows;

namespace Client
{
    /// <summary>
    /// Interaction logic for NewRoom.xaml
    /// </summary>
    public partial class NewRoom : Window
    {
        public NewRoom()
        {
            InitializeComponent();
        }

        private void BtCreateRoom_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtRemoveRoom_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtRemovePlayer_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtStartGame_Click(object sender, RoutedEventArgs e)
        {
            if (GameRoom.IdListPlayerToStartGame.Count < 2) return; //start when in room are min 2 players
            
            DialogResult = true;
            this.Close();
        }
    }
}
