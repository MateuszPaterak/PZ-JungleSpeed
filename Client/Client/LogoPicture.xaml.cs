using System.Windows;
using System.Windows.Controls;

namespace Client
{
    /// <summary>
    /// Interaction logic for LogoPicture.xaml
    /// </summary>
    public partial class LogoPicture : UserControl
    {
        public LogoPicture()
        {
            InitializeComponent();
        }

        private void BtMenuImg_Click(object sender, RoutedEventArgs e)
        {
            this.Content = MyContentClassWindow.ChangeContent(ContNum.PlayersBoard);
        }
    }
}
