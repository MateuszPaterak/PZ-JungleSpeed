using System;
using System.Windows;

namespace Client
{
    /// <summary>
    /// Interaction logic for PlayOff.xaml
    /// </summary>
    public partial class PlayOff : Window
    {
        public PlayOff()
        {
            InitializeComponent();
        }

        private void btPaper_Click(object sender, RoutedEventArgs e)
        {
            byte[] tab = new byte[1];
            tab[0] = Convert.ToByte(1);
            Network.SendCommand(GameSendCommand.PlayOff,tab);
        }

        private void btRock_Click(object sender, RoutedEventArgs e)
        {
            byte[] tab = new byte[1];
            tab[0] = Convert.ToByte(2);
            Network.SendCommand(GameSendCommand.PlayOff, tab);
        }

        private void btScissors_Click(object sender, RoutedEventArgs e)
        {
            byte[] tab = new byte[1];
            tab[0] = Convert.ToByte(3);
            Network.SendCommand(GameSendCommand.PlayOff, tab);
        }
    }
}
