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
        private ContentWindow CW = new ContentWindow();

        public MainWindow()
        {
            InitializeComponent();

            ContentControl.Content = CW.ChangeContent(ContNum.StartImg);
        }

        private void NewRoom(object sender, RoutedEventArgs e)
        {
            NewRoom CreateRoom = new NewRoom();

            if (CreateRoom.ShowDialog() == true)
            {

            }
        }

        private void JoinToRoom(object sender, RoutedEventArgs e)
        {
            JoinRoom JoinToRoom = new JoinRoom();
            JoinToRoom.ShowDialog();
        }

        private void GameLoop()
        {
            
        }
    }
}
