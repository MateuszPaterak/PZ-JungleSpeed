using System;
using System.Collections.Generic;
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
        private volatile bool _runningRefresh;
        private volatile object _selectedItem;
        public JoinRoom()
        {
            InitializeComponent();

            BindLvListRoom();//start list view binding
            BindLvListPlayerToStart();
            BindLvPlayerList();

            _runningRefresh = true;
            refreshAllListThread = new Thread(RefreshAllList);
            refreshAllListThread.Start();
            
            BtJoinToGame.IsEnabled = false;
            BtExitFromGame.IsEnabled = false;

            Network.SendCommand(GameSendCommand.GetListAllRoom);
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
            // Network.SendCommand(GameSendCommand.ExitFromRoom); //exit 
            
            //when window was closed - stop refresh list
            try
            {
                _runningRefresh = false;
                Thread.Sleep(1500);
                refreshAllListThread.Interrupt(); //todo check closing thread
                refreshAllListThread.Join();
            }
            catch (Exception)
            {
                //ignored
            }
        }
        
        public void RefreshAllList()
        {
            try
            {
                while (_runningRefresh)
                {
                    Refresh_LVListRoom();
                    Refresh_LVPlayerList();
                    Refresh_LVListPlayerToStart();
                    Thread.Sleep(1000);
                }
            }
            catch (ThreadInterruptedException)
            {
                _runningRefresh = false;
            }
        }

        public void BindLvListRoom()
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

        public void BindLvPlayerList()
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

        public void BindLvListPlayerToStart()
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
            GetSelectedRoom();

            try
            { 
                ClearAllIn_LVListRoom();//clear
                foreach (var room in GameRoom.IdListRoom) //add new value
                {
                    string name = GameRoom.NameOfRoom[room];
                    AddItemTo_LVListRoom(new ListViewRecord(room, name));
                }

                if (_selectedItem!=null ) //select row
                {
                    SelectRoom(_selectedItem);
                }
            }
            catch (Exception)
            {
                //ignored
            }
        }

        private void GetSelectedRoom()
        {
            try
            {
                if (!LVListOfRoom.Dispatcher.CheckAccess())//check to we are now in controls thread. If no, we must turn to it
                {
                    Dispatcher.Invoke(GetSelectedRoom); //turn to controls thread
                }
                else
                {
                    _selectedItem = LVListOfRoom.SelectedItem;
                }
            }
            catch (Exception)
            {
                //ignored
            }
        }

        private void SelectRoom(object selectedObject)
        {
            try
            {
                if (!LVListOfRoom.Dispatcher.CheckAccess())
                {
                    Dispatcher.Invoke(()=>SelectRoom(selectedObject));
                }
                else
                {
                    ListViewRecord mySelectedRecord = (ListViewRecord)selectedObject;
                    for (var i = 0; i < LVListOfRoom.Items.Count; i++)
                    {
                        var obj = LVListOfRoom.Items.GetItemAt(i);
                        ListViewRecord rec = (ListViewRecord) obj;

                        if (rec.Id != mySelectedRecord.Id) continue;
                        LVListOfRoom.SelectedIndex = i;
                        break;
                    }
                }
            }
            catch (Exception)
            {
                //ignored
            }
        }

        private void Refresh_LVPlayerList()
        {
            try
            {
                ClearAllIn_LVPlayerList();//clear

                foreach (var room in GameRoom.IdListPlayerInRoom) //add new value
                {
                    string name = GameRoom.NameOfPlayers[room];
                    AddItemTo_LVPlayerList(new ListViewRecord(room, name));
                }
            }
            catch (Exception)
            {
                //ignored
            }
        }

        private void Refresh_LVListPlayerToStart()
        {
            try
            {
                ClearAllIn_LVListPlayerToStart();//clear

                foreach (var room in GameRoom.IdListPlayerToStartGame) //add new value
                {
                    string name = GameRoom.NameOfPlayers[room];
                    AddItemTo_LVListPlayerToStart(new ListViewRecord(room, name));
                }
            }
            catch (Exception)
            {
                //ignored
            }
        }

        private void ClearAllIn_LVListRoom()
        {
            try
            {
                if (!LVListOfRoom.Dispatcher.CheckAccess())//check to we are now in controls thread. If no, we must turn to it
                {
                    Dispatcher.Invoke(ClearAllIn_LVListRoom); //turn to controls thread
                }
                else
                {
                    LVListOfRoom.Items.Clear();
                }
            }
            catch (Exception)
            {
                //ignored
            }
        }

        private void ClearAllIn_LVPlayerList()
        {
            try
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
            catch (Exception)
            {
                //ignored
            }
        }

        private void ClearAllIn_LVListPlayerToStart()
        {
            try
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
            catch (Exception)
            {
                //ignored
            }
        }

        private void AddItemTo_LVListRoom(ListViewRecord item)
        {
            try
            {
                if (!LVListOfRoom.Dispatcher.CheckAccess())
                {
                    Dispatcher.Invoke(() => AddItemTo_LVListRoom(item));
                }
                else
                {
                    LVListOfRoom.Items.Add(item);
                }
            }
            catch (Exception)
            {
                //ignored
            }
        }

        private void AddItemTo_LVPlayerList(ListViewRecord item)
        {
            try
            {
                if (!LVListOfPlayer.Dispatcher.CheckAccess())
                {
                    Dispatcher.Invoke(() => AddItemTo_LVPlayerList(item));
                }
                else
                {
                    LVListOfPlayer.Items.Add(item);
                }
            }
            catch (Exception)
            {
                //ignored
            }
        }

        private void AddItemTo_LVListPlayerToStart(ListViewRecord item)
        {
            try
            {
                if (!LVListOfPlayerToStart.Dispatcher.CheckAccess())
                {
                    Dispatcher.Invoke(() => AddItemTo_LVListPlayerToStart(item));
                }
                else
                {
                    LVListOfPlayerToStart.Items.Add(item);
                }
            }
            catch (Exception)
            {
                //ignored
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
