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

        public static void EnableGetMyCard()
        {
            try
            {
                Application.Current.Dispatcher.BeginInvoke(
                        DispatcherPriority.Background,
                        new Action(() => 
                                ((MainWindow)Application.Current.MainWindow).BtGetUpCard.IsEnabled = true
                        ));
            }
            catch (Exception)
            {
                //ignored
            }
        }

        public static void DisableGetMyCard()
        {
            try
            {
                Application.Current.Dispatcher.BeginInvoke(
                        DispatcherPriority.Background,
                        new Action(() =>
                                ((MainWindow)Application.Current.MainWindow).BtGetUpCard.IsEnabled = false
                        ));
            }
            catch (Exception)
            {
                //ignored
            }
        }

        public static void AddGameHistoryText(string msg)
        {
            try
            {
                Application.Current.Dispatcher.BeginInvoke(
                        DispatcherPriority.Background,
                        new Action(() =>
                                ((MainWindow)Application.Current.MainWindow).TbGameHistory.Text 
                                    = msg + "\n" +((MainWindow)Application.Current.MainWindow).TbGameHistory.Text
                        ));
            }
            catch (Exception)
            {
                //ignored
            }
        }

        public static void ClearGameHistoryText()
        {
            try
            {
                Application.Current.Dispatcher.BeginInvoke(
                        DispatcherPriority.Background,
                        new Action(() =>
                                ((MainWindow)Application.Current.MainWindow).TbGameHistory.Text
                                    = ""
                        ));
            }
            catch (Exception)
            {
                //ignored
            }
        }

        public static void SetUCMainScreen()
        {
            try
            {
                Application.Current.Dispatcher.BeginInvoke(
                        DispatcherPriority.Background,
                        new Action(() =>
                                ((MainWindow)Application.Current.MainWindow).CUserControl.Content = new UCMainScreen()
                        ));
            }
            catch (Exception)
            {
                //ignored
            }
        }

        private void NewRoom(object sender, RoutedEventArgs e) //menu
        {
            try
            {
                Application.Current.Dispatcher.BeginInvoke(
                     DispatcherPriority.Background,
                     new Action(() =>
                     {
                         //UCMainScreen uc = (UCMainScreen)CUserControl.Content;
                         //GameClass.MyPlayerName = uc.TbPlayerName.Text ?? "Player";
                         GameClass.MyPlayerName = "Player";
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
                          //UCMainScreen uc = (UCMainScreen)CUserControl.Content;
                          //GameClass.MyPlayerName = uc.TbPlayerName.Text ?? "Player";
                          GameClass.MyPlayerName = "Player";
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

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {

        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            string help = "Autorzy:\nMateusz Paterak\nPaweł Wilczyński\n\n";
            help += "Główne okno aplikacji po uruchomieniu.\nDostępne są dwa główne przyciski umożliwiające stworzenie nowego pokoju, lub dołączenie do istniejącego (utworzonego wcześniej przez innego gracza). Gracz może wprowadzić swoją nazwę przez zmianę zawartości pola “Nick”. Po prawej stronie dostępne są trzy przyciski pozwalające na kontrolowanie rozgrywki. Na górze ekranu znajduje się menu pozwalające otworzyć okno pomocy oraz wybrać inny adres IP serwera gry.";
            help += "\n\nOkno pozwalające utworzyć nowy pokój gracza. \nW lewym górnym rogu istnieje możliwość wpisania własnej nazwy pokoju, jaka będzie się pojawiać na liście wyszukiwań dostępnych pokoi. Przycisk “Utwórz pokój” powoduje wysłanie komunikatu do serwera z nazwą pokoju, oraz od tej chwili inni gracze mogą dołączać od pokoju. Przycisk “Usuń pokój” powoduje wysłanie do serwera gry informacji że nie chcemy już kontynuować gry poprzez tworzenie bieżącego pokoju, a usuwamy go wraz z ewentualnymi graczami którzy byli przypisani do niego. “Anuluj tworzenie pokoju” powoduje usunięcie pokoju, oraz dodatkowo zamyka bieżące okno. Przycisk “Rozpocznij grę” powoduje zamknięcie bieżącego okna oraz wysłanie do serwera komunikatu z rozkazem rozpoczęcia gry. Dostępne są 3 listy, które wyświetlają wszystkie pokoje znajdujące się na serwerze, oraz listę graczy wewnątrz naszego pokoju którzy są dopisani do rozgrywki, oraz którzy tego nie zrobili.";
            help += "\n\nOkno pozwalające dołączyć do istniejącego pokoju. \nDostępne są główne 3 listy.Pierwsza od lewej strony wyświetla listę aktualnie dostępnych pokoi.Należy na nią kliknąć, a następnie wybrać przycisk “Dołącz do pokoju”. Kolejne dwie listy wyświetlają graczy którzy są obserwatorami gry lub, znajdują się w kolejce do uruchomienia gry. Przy pomocy dodatkowych przycisków możliwie jest dołączanie do tych list oraz wypisywanie się z nich. Zamknięcie okna przyciskiem w prawym górnym rogu powoduje automatyczne wypisanie się z pokoju oraz zamknięcie okna.";
            help += "\n\nGracze na przemiennie podnoszą karty. W przypadku gdy któryś z dwóch graczy posiada ten sam symbol (kolor nie ma znaczenia), podnosi totem klikając w przycisk “Weź totem”";
            MessageBox.Show(help);
        }
    }
}
