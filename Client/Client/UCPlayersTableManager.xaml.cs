﻿using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace Client
{
    /// <summary>
    /// Interaction logic for Win2Player.xaml
    /// </summary>
    public partial class PlayersTableManager : UserControl
    {
        private static Image[] _imgPlayerCardArray = new Image[10];
        private static Label[] _labPlayerName = new Label[10];
        private static Random rng = new Random();
        public enum NameOfBackground { Palm, Wood, Bokeh, Wood500 }

        public PlayersTableManager(byte numbersOfPlayers)
        {
            InitializeComponent();

            Application.Current.Dispatcher.BeginInvoke(
                        DispatcherPriority.Background,
                        new Action(() =>
                                {
                                    _imgPlayerCardArray = new Image[10]; //clear
                                    _labPlayerName = new Label[10];

                                    CreatePlayersAndLabel(numbersOfPlayers, "Player");
                                    ChangeBackground("wood500");
                                }
                        ));
        }

        public static void ChangePlayerCard(byte nrPlayerOnTable, byte cardNr)
        {
            try
            {
                Application.Current.Dispatcher.BeginInvoke(
                        DispatcherPriority.Background,
                        new Action(() =>
                                _imgPlayerCardArray[nrPlayerOnTable].Source =
                                new BitmapImage(
                                new Uri(@"/Pictures/Cards/" + cardNr.ToString() + ".jpg", UriKind.Relative))
                        ));
            }
            catch (Exception)
            {
                MessageBox.Show("Karta do wyświetlenia nie została znaleziona w katalogu gry");
            }
         
        }

        public static void ChangeCardRotation(byte nrPlayerOnTable, int angle)
        {
            try
            {
                Application.Current.Dispatcher.BeginInvoke(
                        DispatcherPriority.Background,
                        new Action(() =>
                                {
                                    _imgPlayerCardArray[nrPlayerOnTable].RenderTransform = 
                                            new RotateTransform(
                                                angle, 
                                                _imgPlayerCardArray[nrPlayerOnTable].Width  /2, 
                                                _imgPlayerCardArray[nrPlayerOnTable].Height /2);
                                }
                        ));
            }
            catch(Exception)
            {

            }
        }

        public static void ChangeCardRandomRotation(byte nrPlayerOnTable)
        {
            try
            {
                int angle = rng.Next(0, 360);

                Application.Current.Dispatcher.BeginInvoke(
                        DispatcherPriority.Background,
                        new Action(() =>
                        {
                            _imgPlayerCardArray[nrPlayerOnTable].RenderTransform = 
                                new RotateTransform(
                                    angle, 
                                    _imgPlayerCardArray[nrPlayerOnTable].Width / 2,
                                    _imgPlayerCardArray[nrPlayerOnTable].Height / 2);
                        }
                        ));
            }
            catch (Exception)
            {
                
            }

        }

        private void CreateLabel(byte playersNumberOnTable, string name)
        {
            var distance = (2 * Math.PI) / playersNumberOnTable;
            var radius = 0.75 * (GridPlayer.Height - 10);

            for (byte num = 0; num < playersNumberOnTable; num++)
            {
                var pos = distance * num;
                var posY = -Math.Sin(pos + 1.5 * Math.PI) * radius;
                var posX = -Math.Cos(pos + 0.5 * Math.PI) * radius;
                
                var lab = new Label
                {
                    Content = name + num,
                    Margin = new Thickness(posX, posY - 120, 0, 0),
                    Background = Brushes.DarkGray,
                    Height = 30,
                    Width = 100,
                };

                _labPlayerName[num] = lab;
                GridPlayer.Children.Add(lab);
            }
        }

        private  void CreatePlayer(byte playersAmount)
        {
            var distance = (2 * Math.PI) / playersAmount;
            var radius = 0.75 * (GridPlayer.Height - 10);

            for (byte num = 0; num < playersAmount; num++)
            {
                var pos = distance * num;
                var posY = -Math.Sin(pos + 1.5 * Math.PI) * radius;
                var posX = -Math.Cos(pos + 0.5 * Math.PI) * radius;

                var card = new Image
                {
                    Name = "ImgCardPlayer" + num.ToString(),
                    Width = 100,
                    Height = 100,
                    Margin = new Thickness(posX, posY + 30, 0, 0),
                };

                _imgPlayerCardArray[num] = card;
                GridPlayer.Children.Add(card);
            }
        }

        private  void CreatePlayersAndLabel(byte playersAmount, string name)
        {
            var distance = (2 * Math.PI) / playersAmount;
            var radius = 0.75 * (GridPlayer.Height - 10);

            for (byte num = 0; num < playersAmount; num++)
            {
                var pos = distance * num;
                var posY = -Math.Sin(pos + 1.5 * Math.PI) * radius;
                var posX = -Math.Cos(pos + 0.5 * Math.PI) * radius;

                var lab = new Label
                {
                    Content = name + num,
                    Margin = new Thickness(posX, posY - 120, 0, 0),
                    Background = Brushes.DarkGray,
                    Height = 30,
                    Width = 100,
                };
                
                _labPlayerName[num] = lab;
                GridPlayer.Children.Add(lab);
            }
        

            for (byte num=0; num<playersAmount; num++)
            {
                var pos = distance*num;
                var posY = -Math.Sin(pos + 1.5*Math.PI) * radius;
                var posX = -Math.Cos(pos + 0.5*Math.PI) * radius;

                var card = new Image
                    {
                        Name = "ImgCardPlayer" + num.ToString(),
                        Width = 100,
                        Height = 100,
                        Margin =  new Thickness(posX, posY+30, 0, 0),
                    };
                
                _imgPlayerCardArray[num] = card;
                GridPlayer.Children.Add(card);
            }
        }

        public static void ChangeNamePlayer(byte nrPlayerOnTable, string name)
        {
            try
            {
                Application.Current.Dispatcher.BeginInvoke(
                        DispatcherPriority.Background,
                        new Action(() =>
                        {
                            _labPlayerName[nrPlayerOnTable].Content = name;
                        }
                        ));
            }
            catch (Exception)
            {
                Application.Current.Dispatcher.BeginInvoke(
                        DispatcherPriority.Background,
                        new Action(() =>
                        {
                            _labPlayerName[nrPlayerOnTable].Content = "Player";
                        }
                        ));
            }
        }

        private static void ChangeBackground(string name)
        {
            var background = new Image
            {
                Name = "ImgBackground",
                Width = 100,
                Height = 100,
                Source = new BitmapImage(
                    new Uri(@"/Pictures/" + name + ".jpg", UriKind.Relative)),
            };
            ImageBrush ib = new ImageBrush();
            ib.ImageSource = new BitmapImage(new Uri("/Pictures/Cards/1.jpg", UriKind.Relative));
            //GridPlayer.Background = new ImageBrush(new BitmapImage (new Uri("/Pictures/Cards/1.jpg", UriKind.Relative)));
            Grid mygrid = new Grid();
            mygrid.Background = ib;
            //GridPlayer.Background = ib;
        }
    }
}
