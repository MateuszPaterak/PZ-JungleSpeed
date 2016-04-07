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
    /// Interaction logic for Win2Player.xaml
    /// </summary>
    public partial class Win2Player : UserControl
    {
        public Win2Player()
        {
            InitializeComponent();

            LabPlayer1.Content = "sdfs";

            CreatePlayer();
        }

        private void CreatePlayer() //-s
        {
            var cart = new Image
            {
                Width = 100,
                Height = 100,
                Margin = new Thickness(280, 280, 0, 0),
                Source = new BitmapImage(new Uri(@"/Pictures/totem.png", UriKind.Relative))
            };
            GridPlayer.Children.Add(cart);

        }

    }
}
