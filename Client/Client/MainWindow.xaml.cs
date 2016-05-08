﻿using System;
using System.IO;
using System.Net.Sockets;
using System.Windows;
using System.Windows.Controls;

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
            
            //CUserControl.Content = MyContentClassWindow.ChangeContent(ContNum.StartImg);
            //CUserControl.Content = MyContentClassWindow.ChangeContent(ContNum.PlayersBoard);
        }

        private void NewRoom(object sender, RoutedEventArgs e) //menu
        {
            try
            {
                UCMainScreen uc = (UCMainScreen) CUserControl.Content;
                GameClass.MyPlayerName = uc.TbPlayerName.Text ?? "Player";
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
                UCMainScreen uc = (UCMainScreen)CUserControl.Content;
                GameClass.MyPlayerName = uc.TbPlayerName.Text ?? "Player";
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
            //testy
            //PlayOff po = new PlayOff();
            //po.ShowDialog();
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
            Network.SendCommand(GameSendCommand.Disconnect);
            
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

        private void ConfigIpServer(object sender, RoutedEventArgs e)
        {
            ((MainWindow)Application.Current.MainWindow).CUserControl.Content =
                new UCConfigServer();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Network.Disconnect();
        }
    }
}
