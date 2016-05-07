using System;
using System.Net.Sockets;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Client
{
    /// <summary>
    /// Interaction logic for NewRoom.xaml
    /// </summary>
    public partial class NewRoom : Window
    {
        private Thread refreshAllListThread;
        public NewRoom()
        {
            InitializeComponent();

            refreshAllListThread = new Thread(RefreshAllList);
            refreshAllListThread.Start();

            BindLvListRoom();//start list view binding
            BindLvListPlayerToStart();
            BindLvPlayerList();
        }

        private void BtCreateRoom_Click(object sender, RoutedEventArgs e)
        {
            if (TBNameRoom.Text == null)
            {
                MessageBox.Show("Nazwa pokoju nie może być pusta.");
                return;
            }
            else
            {
                GameRoom.NameRoom = TBNameRoom.Text;
            }
            Network.SendCommand(GameSendCommand.CreateNewRoom);
        }

        private void BtRemoveRoom_Click(object sender, RoutedEventArgs e)
        {
            Network.SendCommand(GameSendCommand.CloseMyRoom);
            GameRoom.NameRoom = null;
            Network.SendCommand(GameSendCommand.GetListAllRoom);
        }

        private void BtRemovePlayer_Click(object sender, RoutedEventArgs e)
        {//remove selected player
            if (LVPlayerList.SelectedIndex != -1)
            {
                byte[] selectedId = new byte[1];
                ListViewRecord selected = (ListViewRecord)LVPlayerList.Items.GetItemAt(LVPlayerList.SelectedIndex);
                if (selected.Name == GameClass.MyPlayerName) return; //don't remove myself //todo correct checking my id
                selectedId[0] = Convert.ToByte(selected.Id);
                Network.SendCommand(GameSendCommand.RemovePlayerFromRoom,selectedId);
            }
        }

        private void BtStartGame_Click(object sender, RoutedEventArgs e)
        {
            if (GameRoom.IdListPlayerToStartGame.Count < 2)
            {
                MessageBox.Show("Musi być w pokoju przynajmniej dwóch graczy aby rozpocząć grę!.");
                return; //start when in room are min 2 players
            }
        
            refreshAllListThread.Abort();
            refreshAllListThread.Join();
            
            DialogResult = true; //true if room will be created
            Close();
        }

        private void RefreshAllList()
        {
            //+ sprawdzenie czy jest utworzona lista 
            Refresh_LVListRoom();
            Refresh_LVPlayerList();
            Refresh_LVListPlayerToStart();
            Thread.Sleep(1000);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //when window was closed - stop refresh list
            if (GameRoom.NameRoom != null)
            {
                Network.SendCommand(GameSendCommand.CloseMyRoom);
            }
            GameRoom.NameRoom = null;

            refreshAllListThread.Abort();
            refreshAllListThread.Join();
        }

        private void BtCancelCreateRoom_Click(object sender, RoutedEventArgs e)
        {
            if (GameRoom.NameRoom != null)
            {
                Network.SendCommand(GameSendCommand.CloseMyRoom);
            }
            GameRoom.NameRoom = null;

            refreshAllListThread.Abort();
            refreshAllListThread.Join();
        }

        private void BindLvListRoom()
        {
            var gridView = new GridView();
            LVListRoom.View = gridView;
            gridView.Columns.Add(new GridViewColumn
            {
                Header = "Id",
                DisplayMemberBinding = new Binding("Id")
            });
            gridView.Columns.Add(new GridViewColumn
            {
                Header = "Nazwa pokoju",
                DisplayMemberBinding = new Binding("Name")
            });
        }

        private void BindLvPlayerList()
        {
            var gridView = new GridView();
            LVPlayerList.View = gridView;
            gridView.Columns.Add(new GridViewColumn
            {
                Header = "Id",
                DisplayMemberBinding = new Binding("Id")
            });
            gridView.Columns.Add(new GridViewColumn
            {
                Header = "Nick gracza",
                DisplayMemberBinding = new Binding("Name")
            });
        }

        private void BindLvListPlayerToStart()
        {
            var gridView = new GridView();
            LVListPlayerToStart.View = gridView;
            gridView.Columns.Add(new GridViewColumn
            {
                Header = "Id",
                DisplayMemberBinding = new Binding("Id")
            });
            gridView.Columns.Add(new GridViewColumn
            {
                Header = "Nick gracza",
                DisplayMemberBinding = new Binding("Name")
            });
        }

        private void Refresh_LVListRoom()
        {
            ClearAllIn_LVListRoom();//clear

            foreach (var room in GameRoom.IdListRoom) //add new value
            {
                string name = GameRoom.NameOfRoom[room];
                AddItemTo_LVListRoom(new ListViewRecord(room, name));
            }
        }

        private void Refresh_LVPlayerList()
        {
            ClearAllIn_LVPlayerList();//clear

            foreach (var room in GameRoom.IdListPlayerInRoom) //add new value
            {
                string name = GameRoom.NameOfPlayers[room];
                AddItemTo_LVPlayerList(new ListViewRecord(room, name));
            }
        }

        private void Refresh_LVListPlayerToStart()
        {
            ClearAllIn_LVListPlayerToStart();//clear

            foreach (var room in GameRoom.IdListPlayerToStartGame) //add new value
            {
                string name = GameRoom.NameOfPlayers[room];
                AddItemTo_LVListPlayerToStart(new ListViewRecord(room, name));
            }
        }

        private void ClearAllIn_LVListRoom()
        {
            if (!LVListRoom.Dispatcher.CheckAccess())
            //sprawdzenie czy jest się w wątku obsługującym tę kontrolkę, 
            //jeśli nie to trzeba w dalszym kroku przełączyć się na niego
            {
                Dispatcher.Invoke(ClearAllIn_LVListRoom);  //przełączenie się na wątek naszej kontrolki
            }
            else
            {
                LVListRoom.Items.Clear();
            }
        }

        private void ClearAllIn_LVPlayerList()
        {
            if (!LVPlayerList.Dispatcher.CheckAccess())
            {
                Dispatcher.Invoke(ClearAllIn_LVPlayerList);
            }
            else
            {
                LVPlayerList.Items.Clear();
            }
        }

        private void ClearAllIn_LVListPlayerToStart()
        {
            if (!LVListPlayerToStart.Dispatcher.CheckAccess())
            {
                Dispatcher.Invoke(ClearAllIn_LVListPlayerToStart);
            }
            else
            {
                LVListPlayerToStart.Items.Clear();
            }
        }

        private void AddItemTo_LVListRoom(ListViewRecord item)
        {
            if (!LVListRoom.Dispatcher.CheckAccess())
            //sprawdzenie czy jest się w wątku obsługującym tę kontrolkę, 
            //jeśli nie to trzeba w dalszym kroku przełączyć się na niego
            {
                Dispatcher.Invoke(ClearAllIn_LVListRoom);  //przełączenie się na wątek naszej kontrolki
            }
            else
            {
                LVListRoom.Items.Add(item);
            }
        }

        private void AddItemTo_LVPlayerList(ListViewRecord item)
        {
            if (!LVPlayerList.Dispatcher.CheckAccess())
            {
                Dispatcher.Invoke(ClearAllIn_LVPlayerList);
            }
            else
            {
                LVPlayerList.Items.Add(item);
            }
        }

        private void AddItemTo_LVListPlayerToStart(ListViewRecord item)
        {
            if (!LVListPlayerToStart.Dispatcher.CheckAccess())
            {
                Dispatcher.Invoke(ClearAllIn_LVListPlayerToStart);
            }
            else
            {
                LVListPlayerToStart.Items.Add(item);
            }
        }
        
        public class ListViewRecord
        {
            public int Id { get; set; }
            public string Name { get; set; }

            public ListViewRecord(int id, string name)
            {
                Id = id;
                Name = name;
            }
        }
        
    }
}
