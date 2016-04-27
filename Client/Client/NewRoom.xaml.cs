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
/*
            string host = "127.0.0.1"; //textBox1.Text;
            int port = 4000; //System.Convert.ToInt16(numericUpDown1.Value);
            try
            {
                TcpClient klient = new TcpClient(host, port);
                //listBox1.Items.Add("Nawiązano połączenie z " + host + " na porcie: " + port);
               
                klient.Close();
                MessageBox.Show("Nawiano połączenie");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Błąd: Nie udało się nawiązać połączenia!");
                MessageBox.Show(ex.ToString());
            }
            */
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
            //...
            //start game
            //...

            DialogResult = true;
            //this.Close();
        }
    }
}
