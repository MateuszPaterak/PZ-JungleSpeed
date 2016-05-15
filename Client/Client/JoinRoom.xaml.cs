using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Client
{
    /// <summary>
    /// Interaction logic for JoinRoom.xaml
    /// </summary>
    public partial class JoinRoom : Window
    {
        private Thread refreshAllListThread;
        public JoinRoom()
        {
            InitializeComponent();

            BindLvListRoom();//start list view binding
            BindLvListPlayerToStart();
            BindLvPlayerList();

            refreshAllListThread = new Thread(RefreshAllList);
            refreshAllListThread.Start();

            Network.SendCommand(GameSendCommand.GetListAllRoom);

            BtJoinToGame.IsEnabled = false;
            BtExitFromGame.IsEnabled = false;
        }

        private void btJoinToRoom_Click(object sender, RoutedEventArgs e)
        {
            if (LVListOfRoom.SelectedIndex == -1) return;
            byte[] selectedId = new byte[1];//get id selected room
            ListViewRecord selected = (ListViewRecord)LVListOfRoom.Items.GetItemAt(LVListOfRoom.SelectedIndex);
            selectedId[0] = Convert.ToByte(selected.Id);

            Network.SendCommand(GameSendCommand.JoinToRoom, selectedId);
            Network.SendCommand(GameSendCommand.SendMyLogin);

            BtJoinToGame.IsEnabled = true; //todo not ideal when room was full
            BtExitFromGame.IsEnabled = true; //todo set new button state when player will be deleted from player list
        }

        private void btExitFromRoom_Click(object sender, RoutedEventArgs e)
        {
            Network.SendCommand(GameSendCommand.ExitFromRoom);
            GameRoom.ClearIdListPlayerToStartGame();
            GameRoom.ClearIdListPlayerInRoom();
            //Network.SendCommand(GameSendCommand.GetListAllRoom);

            BtJoinToGame.IsEnabled = false;
            BtExitFromGame.IsEnabled = false;
        }

        private void btJoinToGame_Click(object sender, RoutedEventArgs e)
        {
            Network.SendCommand(GameSendCommand.JoinToGameInRoom);
            //Network.SendCommand(GameSendCommand.GetListAllPlayerInRoomToStartGame);
        }

        private void btExitFromGamePlay_Click(object sender, RoutedEventArgs e)
        {
            Network.SendCommand(GameSendCommand.ExitFromGameInRoom);
            //Network.SendCommand(GameSendCommand.GetListAllPlayerInRoomToStartGame);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Network.SendCommand(GameSendCommand.ExitFromRoom); //exit 

            //when window was closed - stop refresh list
            refreshAllListThread.Abort();
            refreshAllListThread.Join();
        }

        private void RefreshAllList()
        {
            //+ check for created list
            Refresh_LVListRoom();
            Refresh_LVPlayerList();
            Refresh_LVListPlayerToStart();
            Thread.Sleep(1000);
        }

        private void BindLvListRoom()
        {
            var gridView = new GridView();
            LVListOfRoom.View = gridView;
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
            LVListOfPlayer.View = gridView;
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
            LVListOfPlayerToStart.View = gridView;
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
            if (!LVListOfRoom.Dispatcher.CheckAccess())
            //sprawdzenie czy jest się w wątku obsługującym tę kontrolkę, 
            //jeśli nie to trzeba w dalszym kroku przełączyć się na niego
            {
                Dispatcher.Invoke(ClearAllIn_LVListRoom);  //przełączenie się na wątek naszej kontrolki
            }
            else
            {
                LVListOfRoom.Items.Clear();
            }
        }

        private void ClearAllIn_LVPlayerList()
        {
            if (!LVListOfPlayer.Dispatcher.CheckAccess())
            {
                Dispatcher.Invoke(ClearAllIn_LVPlayerList);
            }
            else
            {
                LVListOfPlayer.Items.Clear();
            }
        }

        private void ClearAllIn_LVListPlayerToStart()
        {
            if (!LVListOfPlayerToStart.Dispatcher.CheckAccess())
            {
                Dispatcher.Invoke(ClearAllIn_LVListPlayerToStart);
            }
            else
            {
                LVListOfPlayerToStart.Items.Clear();
            }
        }

        private void AddItemTo_LVListRoom(ListViewRecord item)
        {
            if (!LVListOfRoom.Dispatcher.CheckAccess())
            //sprawdzenie czy jest się w wątku obsługującym tę kontrolkę, 
            //jeśli nie to trzeba w dalszym kroku przełączyć się na niego
            {
                Dispatcher.Invoke(ClearAllIn_LVListRoom);  //przełączenie się na wątek naszej kontrolki
            }
            else
            {
                LVListOfRoom.Items.Add(item);
            }
        }

        private void AddItemTo_LVPlayerList(ListViewRecord item)
        {
            if (!LVListOfPlayer.Dispatcher.CheckAccess())
            {
                Dispatcher.Invoke(ClearAllIn_LVPlayerList);
            }
            else
            {
                LVListOfPlayer.Items.Add(item);
            }
        }

        private void AddItemTo_LVListPlayerToStart(ListViewRecord item)
        {
            if (!LVListOfPlayerToStart.Dispatcher.CheckAccess())
            {
                Dispatcher.Invoke(ClearAllIn_LVListPlayerToStart);
            }
            else
            {
                LVListOfPlayerToStart.Items.Add(item);
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
