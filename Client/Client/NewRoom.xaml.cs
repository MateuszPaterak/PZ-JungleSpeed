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
        private Thread _refreshAllListThread;
        private volatile bool _runningRefresh;
        private volatile object _selectedItem;
        public NewRoom()
        {//todo autochecked list when was checked
            InitializeComponent();

            BindLvListRoom();//start list view binding
            BindLvListPlayerToStart();
            BindLvPlayerList();

            _runningRefresh = true;
            _refreshAllListThread = new Thread(RefreshAllList);
            _refreshAllListThread.Start();
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
            Network.SendCommand(GameSendCommand.SendMyLogin);
        }

        private void BtRemoveRoom_Click(object sender, RoutedEventArgs e)
        {
            Network.SendCommand(GameSendCommand.CloseMyRoom);
            GameRoom.NameRoom = null;
            Network.SendCommand(GameSendCommand.GetListAllRoom); //todo to commend?
        }

        private void BtRemovePlayer_Click(object sender, RoutedEventArgs e)
        {//remove selected player
            if (LVPlayerList.SelectedIndex == -1) return;

            byte[] selectedId = new byte[1];
            ListViewRecord selected = (ListViewRecord)LVPlayerList.Items.GetItemAt(LVPlayerList.SelectedIndex);
            if (selected.Name == GameClass.MyPlayerName) return; //don't remove myself 
            //todo correct checking my id / add new communicate with myself id
            selectedId[0] = Convert.ToByte(selected.Id);

            Network.SendCommand(GameSendCommand.RemovePlayerFromRoom,selectedId);
        }

        private void BtStartGame_Click(object sender, RoutedEventArgs e)
        {
            if (GameRoom.IdListPlayerToStartGame.Count < 2)
            {
                MessageBox.Show("Musi być w pokoju gotowych przynajmniej dwóch graczy aby rozpocząć grę!");
                return; //start when in room are min 2 players
            }
        
            DialogResult = true; //true if room will be created
            Close();
        }
        
        private void BtCancelCreateRoom_Click(object sender, RoutedEventArgs e)
        {
            if (GameRoom.NameRoom != null)
            {
                Network.SendCommand(GameSendCommand.CloseMyRoom);
            }
            GameRoom.NameRoom = null;
            
            DialogResult = false;
            Close();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                if (DialogResult == null) //window was closed manually
                {
                    if (GameRoom.NameRoom != null)
                        Network.SendCommand(GameSendCommand.CloseMyRoom);
                    
                    GameRoom.NameRoom = null;
                    DialogResult = false;
                    _runningRefresh = false;
                    _refreshAllListThread.Interrupt();
                    _refreshAllListThread.Join();
                    return;
                }
                if (!DialogResult.Value) //new room was not created and started
                {
                    if (GameRoom.NameRoom != null)
                        Network.SendCommand(GameSendCommand.CloseMyRoom);

                    GameRoom.NameRoom = null;
                }
            }
            catch (Exception)
            {
            //ignored
            }

            try
            {//when window was closed - stop refresh list
                _runningRefresh = false;
                _refreshAllListThread.Interrupt();
                _refreshAllListThread.Join();
            }
            catch (Exception)
            {
                //ignored
            }
        }

        private void RefreshAllList()
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

            GetSelectedRoom();

            try
            {
                ClearAllIn_LVListRoom();//clear
                foreach (var room in GameRoom.IdListRoom) //add new value
                {
                    string name = GameRoom.NameOfRoom[room];
                    AddItemTo_LVListRoom(new ListViewRecord(room, name));
                }

                if (_selectedItem != null) //select row
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
                if (!LVListRoom.Dispatcher.CheckAccess())//check to we are now in controls thread. If no, we must turn to it
                {
                    Dispatcher.Invoke(GetSelectedRoom); //turn to controls thread
                }
                else
                {
                    _selectedItem = LVListRoom.SelectedItem;
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
                if (!LVListRoom.Dispatcher.CheckAccess())
                {
                    Dispatcher.Invoke(() => SelectRoom(selectedObject));
                }
                else
                {
                    ListViewRecord mySelectedRecord = (ListViewRecord)selectedObject;
                    for (var i = 0; i < LVListRoom.Items.Count; i++)
                    {
                        var obj = LVListRoom.Items.GetItemAt(i);
                        ListViewRecord rec = (ListViewRecord)obj;

                        if (rec.Id != mySelectedRecord.Id) continue;
                        LVListRoom.SelectedIndex = i;
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
                if (!LVListRoom.Dispatcher.CheckAccess())//check to we are now in controls thread. If no, we must turn to it
                {   
                    Dispatcher.Invoke(ClearAllIn_LVListRoom); //turn to controls thread
                }
                else
                {
                    LVListRoom.Items.Clear();
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
                if (!LVPlayerList.Dispatcher.CheckAccess())
                {
                
                    Dispatcher.Invoke(ClearAllIn_LVPlayerList);
             
                }
                else
                {
                    LVPlayerList.Items.Clear();
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
            if (!LVListPlayerToStart.Dispatcher.CheckAccess())
                {
                    Dispatcher.Invoke(ClearAllIn_LVListPlayerToStart);
                }
                else
                {
                    LVListPlayerToStart.Items.Clear();
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
                if (!LVListRoom.Dispatcher.CheckAccess())
                //check to we are now in controls thread. If no, we must turn to it
                {
                    Dispatcher.Invoke(() => AddItemTo_LVListRoom(item));  //turn to controls thread   
                }
                else
                {
                    LVListRoom.Items.Add(item);
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
                if (!LVPlayerList.Dispatcher.CheckAccess())
                {
                    Dispatcher.Invoke(() => AddItemTo_LVPlayerList(item));
                }
                else
                {
                    LVPlayerList.Items.Add(item);
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
                if (!LVListPlayerToStart.Dispatcher.CheckAccess())
                {
                    Dispatcher.Invoke(() => AddItemTo_LVListPlayerToStart(item));
                }
                else
                {
                    LVListPlayerToStart.Items.Add(item);
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
