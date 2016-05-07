using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
    /// Interaction logic for UCConfigServer.xaml
    /// </summary>
    public partial class UCConfigServer : UserControl
    {
        public UCConfigServer()
        {
            InitializeComponent();
        }

        private void BtChangeIp_Click(object sender, RoutedEventArgs e)
        {
            IPAddress addrIP;
            try
            {
                addrIP = IPAddress.Parse(TbIpAddress.Text);
                Network.Host = addrIP.ToString();

                ((MainWindow)Application.Current.MainWindow).CUserControl.Content = 
                    new UCMainScreen();
            }
            catch (Exception)
            {
                MessageBox.Show("Błędny format adresu IP!!");
                TbIpAddress.Text = "127.0.0.1";
            }
        }
    }
}
