using System;
using System.IO;
using System.Net.Sockets;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            CUserControl.Content = new UCMainScreen();
        }

        private void NewRoom(object sender, RoutedEventArgs e) //menu
        {
            try
            {
                Application.Current.Dispatcher.BeginInvoke(
                     DispatcherPriority.Background,
                     new Action(() =>
                     {
                         UCMainScreen uc = (UCMainScreen)CUserControl.Content;
                         GameClass.MyPlayerName = uc.TbPlayerName.Text ?? "Player";
                     }));
            }
            catch (Exception)
            {
                GameClass.MyPlayerName = "Player";
            }
            
            GameRoom.NewRoom();
            
        }

        private void JoinToRoom(object sender, RoutedEventArgs e) //menu
        {
            try
            {
                Application.Current.Dispatcher.BeginInvoke(
                      DispatcherPriority.Background,
                      new Action(() =>
                      {
                          UCMainScreen uc = (UCMainScreen)CUserControl.Content;
                          GameClass.MyPlayerName = uc.TbPlayerName.Text ?? "Player";
                      }));
            }
            catch (Exception)
            {
                GameClass.MyPlayerName = "Player";
            }

            GameRoom.JoinToRoom();
        }

        private void BtGetUpCard_Click(object sender, RoutedEventArgs e)
        {
            Network.SendCommand(GameSendCommand.GetUpMyCard);
        }

        private void BtGetTotem_Click(object sender, RoutedEventArgs e)
        {
            Network.SendCommand(GameSendCommand.GetTotem);
        }

        private void BtOutOfGameplay_Click(object sender, RoutedEventArgs e)
        {
            Network.SendCommand(GameSendCommand.Disconnect);
        }

        private void ConfigIpServer(object sender, RoutedEventArgs e)
        {
            Application.Current.Dispatcher.BeginInvoke(
                            DispatcherPriority.Background,
                            new Action(() => (
                            (MainWindow)Application.Current.MainWindow).CUserControl.Content = new UCConfigServer()
                            ));
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Network.Disconnect();
        }
    }
}
