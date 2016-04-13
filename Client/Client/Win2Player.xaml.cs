using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Interaction logic for Win2Player.xaml
    /// </summary>
    public partial class PlayersTableManager : UserControl
    {
        public PlayersTableManager()
        {
            InitializeComponent();
            
            /*
            CreatePlayers(6, "Player");
            ChangePlayerCard(0, 1);
            ChangePlayerCard(1, 4);
            ChangePlayerCard(2, 3);
            ChangePlayerCard(3, 2);
            ChangePlayerCard(4, 2);
            ChangePlayerCard(5, 4);

            ChangeNamePlayer("Ja", 0);
            ChangeNamePlayer("Jan", 3);
            */
        }

        public PlayersTableManager(byte numbersOfPlayers)
        {
            InitializeComponent();
            CreatePlayers(numbersOfPlayers, "Player");
        }

        private static Image[] _imgPlayerCardArray = new Image[10];
        private static Label[] _labPlayerName = new Label[10];
        public enum NameOfBackground {Palm, Wood, Bokeh}

        public static void ChangePlayerCard(byte nrPlayer, byte cardNr)
        {
         _imgPlayerCardArray[nrPlayer].Source =
            new BitmapImage(
                new Uri(@"/Pictures/Cards/" + cardNr.ToString() + ".png", UriKind.Relative));
        }

        private void CreatePlayers(byte playersNumber, string name)
        {
            var distance = (2 * Math.PI) / playersNumber;
            var radius = 0.75 * (GridPlayer.Height - 10);

            for (byte num=0; num<playersNumber; num++)
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
                
                var lab = new Label
                {
                    Content = name+num,
                    Margin = new Thickness(posX, posY-120, 0, 0),
                    Background = Brushes.DarkGray, 
                    Height = 30,
                    Width = 100,
                };

                _imgPlayerCardArray[num] = card;
                _labPlayerName[num] = lab;
                GridPlayer.Children.Add(card);
                GridPlayer.Children.Add(lab);
            }
        }

        public static void ChangeNamePlayer(string name, byte numer)
        {
            _labPlayerName[numer].Content = name;
        }

        /*private void ChangeBackround(String name)
        {
            ImgBackground.ImageSource = 
                    new BitmapImage(
                        new Uri(@"/Pictures/" + name + ".jpg", UriKind.Relative));
        }*/

    }
}
