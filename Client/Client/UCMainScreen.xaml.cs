﻿using System;
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
    /// Interaction logic for UCMainScreen.xaml
    /// </summary>
    public partial class UCMainScreen : UserControl
    {
        public UCMainScreen()
        {
            InitializeComponent();
            if(GameClass.MyPlayerName!=null)
            {
                TbPlayerName.Text = GameClass.MyPlayerName;
            }
        }
        
        private void BtNewServer_Click(object sender, RoutedEventArgs e)
        {
            GameClass.MyPlayerName = TbPlayerName.Text ?? "Player";
            
            GameRoom.NewRoom();
        }

        private void BtJoinToServer_Click(object sender, RoutedEventArgs e)
        {
            GameClass.MyPlayerName = TbPlayerName.Text ?? "Player";
            
            GameRoom.JoinToRoom();
        }

        
    }
}
