using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Diagnostics;

namespace AplikacjaSerwerowa
{
    public enum GameSendCommand
    {
        NumberOfCards, //1
        ListAllPlayerIDs, //2
        ActivateGetUpCardButton, //3
        DeactivateGetUpCardButton, //4
        PickedCard,//5
        DuelWin,//6
        DuelLose,//7
        ListAllRooms,//8
        ListAllPlayerInRoom,//9
        ListAllPlayerInRoomToStartGame,//10
        EndOfGame,//11
    };

    public partial class Form1 : Form
    {
        public Form1()
        {
            CheckForIllegalCrossThreadCalls = false;
            Pokoj1.WszyscyGracze = new List<Gracz>();
            InitializeComponent();
            SetupServer();
        }

        public byte[] temp2byte = new byte[256];
        int licznikDlugosci = 0;
        private byte[] _buffer = new byte[256];
        private byte[] byteToSend = new byte[256];
        private byte[] _toSend = new byte[256];
        private byte[] _messageContent = new byte[256];
        public static Gracze Pokoj1 = new Gracze();
        public static Gracze RozgrywkaPokoj1 = new Gracze();
        public Socket _serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        Random r = new Random();

        #region Tymczasowo (potem lepiej się rozwiąże)
        bool isCardPickUpRequest = false;
        bool isTakeTotemRequest = false;
        bool duelIsOn = false;
        int playerHavingTurn;
        int playerTakingTotem;
        int roomMaxID = 0;
        #endregion

        #region Rzeczy do zmienienia
        public static Pokoj glownyPokoj = new Pokoj(0, "Glowny pokoj");
        #endregion

        public int dlugoscPakietu, kodWiadomosci;
        string daneWiadomosci = "", nazwaKlienta = "";

        private void Form1_Load(object sender, EventArgs e)
        {
        }

        private void SetupServer()
        {
            try
            {
                _serverSocket.Bind(new IPEndPoint(IPAddress.Any, 1000));
                _serverSocket.Listen(10);
                _serverSocket.BeginAccept(new AsyncCallback(AppceptCallback), null);
                dziennik.Items.Add("Serwer czeka na graczy :)");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Błąd serwera",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void AppceptCallback(IAsyncResult ar)
        {
            Socket socket = _serverSocket.EndAccept(ar);

            Pokoj1.WszyscyGracze.Add(new Gracz(roomMaxID,socket));
            roomMaxID++;
            listaGraczy.Items.Add(socket.RemoteEndPoint.ToString());
            dziennik.Items.Add("Hura, dołączył kolejny gracz! Obecna liczba graczy to: " + Pokoj1.WszyscyGracze.Count.ToString());
            socket.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), socket);
            _serverSocket.BeginAccept(new AsyncCallback(AppceptCallback), null);
        }

        private void ReceiveCallback(IAsyncResult ar)
        {
            Socket socket = (Socket)ar.AsyncState;
            if (socket.Connected)
            {
                nazwaKlienta = socket.RemoteEndPoint.ToString();

                int received;
                try
                {
                    received = socket.EndReceive(ar);
                }
                catch (Exception)
                {
                    //jesli cos nie pyklo to wywal z Pokoj1 (co z RozgrywkaPokoj1?)
                    for (int i = 0; i < Pokoj1.WszyscyGracze.Count; i++)
                    {
                        if (Pokoj1.WszyscyGracze[i]._Socket.RemoteEndPoint.ToString().Equals(socket.RemoteEndPoint.ToString()))
                        {
                            Pokoj1.WszyscyGracze.RemoveAt(i);
                            listaGraczy.Items.RemoveAt(i);
                            dziennik.Items.Add("Gracz niestety opuścił serwer. Obecna liczba graczy to: " + Pokoj1.WszyscyGracze.Count.ToString());
                        }
                    }
                    return;
                }
                if (received != 0)
                {
                    dlugoscPakietu = _buffer[0];
                    kodWiadomosci = _buffer[1];
                    if ((dlugoscPakietu - 2) > 0)
                    {
                        _messageContent = new byte[dlugoscPakietu - 2];
                        for (int i = 0; i < dlugoscPakietu - 2; i++)
                        {
                            _messageContent[i] = _buffer[2 + i];
                        }
                    }

                    switch (kodWiadomosci)
                    {
                        //DOIMPLEMENTOWAĆ
                        //gracz żąda podniesienia totemu
                        case 50:
                            {
                                dziennik.Items.Add("Gracz " + nazwaKlienta + " chwycił totem");
                                for (int i = 0; i < RozgrywkaPokoj1.WszyscyGracze.Count; i++)
                                {
                                    if (socket == RozgrywkaPokoj1.WszyscyGracze[i]._Socket)
                                    {
                                        isTakeTotemRequest = true;
                                        playerTakingTotem = RozgrywkaPokoj1.WszyscyGracze[i].id;
                                        //stara impl.
                                        //RozgrywkaPokoj1.SprawdzCzyNaStoleJestSymbol(RozgrywkaPokoj1.WszyscyGracze[i].OnTable
                                        //    [RozgrywkaPokoj1.WszyscyGracze[i].OnTable.Count - 1], RozgrywkaPokoj1.WszyscyGracze[i].id);
                                    }
                                }

                                break;
                            }
                        //DOIMPLEMENTOWAĆ PÓŹNIEJ
                        //gracz wybiera papier, kamień lub nożyce
                        //zawartość wiadomości P:[1],K:[2],N:[3];
                        case 51:
                            {
                                dziennik.Items.Add("Gracz " + nazwaKlienta + " wybrał " + daneWiadomosci);
                                break;
                                //tego nie kmin na razie, bo dodatkowe
                            }
                        //gracz zażądał odwrócenia karty ze swojego stosu kart zakrytych
                        case 52:
                            {
                                dziennik.Items.Add("Gracz " + nazwaKlienta + " obrócił kartę");
                                for (int i = 0; i < RozgrywkaPokoj1.WszyscyGracze.Count; i++)
                                {
                                    if (socket.RemoteEndPoint.ToString().Equals(RozgrywkaPokoj1.WszyscyGracze[i]._Socket.RemoteEndPoint.ToString())
                                        && playerHavingTurn == RozgrywkaPokoj1.WszyscyGracze[i].id)
                                    {
                                        isCardPickUpRequest = true;
                                    }
                                    //2016-06-01 - zrob implementacje blokowania przycisku
                                }
                                break;
                            }
                        //DOIMPLEMENTOWAĆ PÓŹNIEJ
                        //gracz wychodzi z gry
                        case 53:
                            {
                                dziennik.Items.Add("Gracz " + nazwaKlienta + " niestety wyszedł z gry :(");

                                //CzyjRuch.Gracz
                                for (int i = 0; i < RozgrywkaPokoj1.WszyscyGracze.Count; i++)
                                {

                                    //DOIMPLEMENTUJ TO GUNWO
                                }
                                //poinformowanie wszystkich graczy
                                //to ponizej chyba nie stad? 2016-06-01
                                //SendCommand(GameSendCommand.PickedCard, 1, 2, null, 0);

                                break;
                            }
                        case 54:
                            {
                                dziennik.Items.Add("Gracz " + nazwaKlienta + " podał swój login");
                                daneWiadomosci = Encoding.UTF8.GetString(_messageContent);
                                for (int i = 0; i < Pokoj1.WszyscyGracze.Count; i++)
                                {
                                    if (socket == Pokoj1.WszyscyGracze[i]._Socket)
                                    {
                                        Pokoj1.WszyscyGracze[i]._Name = daneWiadomosci;
                                    }
                                }
                                for (int i = 0; i < listaGraczy.Items.Count; i++)
                                {
                                    if (nazwaKlienta == listaGraczy.Items[i].ToString())
                                    {
                                        listaGraczy.Items[i] = daneWiadomosci;
                                    }
                                }
                                break;
                            }
                        case 55:
                            {
                                dziennik.Items.Add("Gracz " + nazwaKlienta + " zażądał podania nazw oraz ID wszystkich dostępnych pokoi");
                                //poinformowanie gracza o dostępnych pokojach
                                SendCommand(GameSendCommand.ListAllRooms, 0, 0, socket, 0);
                                break;
                            }
                        case 56:
                            {
                                //na szytwno bo jest 1 pokój xD
                                //potem trzeba dopisac obsluge pola [ID pokoju(byte)]
                                //w tej chwili jest na 4 pozycji sendcommand "0", czyli zawsze podaje liste ludzi z pokoju o numerze "0"
                                dziennik.Items.Add("Gracz " + nazwaKlienta + " zażądał podania nazw graczy oraz ID gracz z danego pokoju");
                                //poinformowanie gracza o dostępnych w pokoju graczach
                                SendCommand(GameSendCommand.ListAllPlayerInRoom, 0, 0, socket, 0);
                                break;
                            }
                        case 57:
                            {
                                //poki co realizuje DOKŁADNIE to samo, co case 56, tylko rozsyla do wszystkich graczy xD
                                //na szytwno bo jest 1 pokój xD
                                //potem trzeba dopisac obsluge pola [ID pokoju(byte)]
                                dziennik.Items.Add("Gracz " + nazwaKlienta + " zażądał podania nazw graczy oraz ID graczy, którzy dołączyli do rozgrywki");
                                //poinformowanie gracza o dostępnych pokojach
                                SendCommand(GameSendCommand.ListAllPlayerInRoomToStartGame, 0, 0, socket, 0);
                                break;
                            }
                        case 58:
                            {
                                //na sztywno jest //na razie zbędnę, bo jest 1 pokój, w którym lista użytkowników = lista obecnych na serwerze
                                dziennik.Items.Add("Gracz " + nazwaKlienta + " chce dolaczyc do pokoju");
                                //dodaj do pokoju
                                SendCommand(GameSendCommand.ListAllPlayerInRoom, 0, 0, null, 0);
                                //wyslij liste z graczami
                                break;
                            }
                        case 59:
                            {
                                //na sztywno jest
                                dziennik.Items.Add("Gracz " + nazwaKlienta + " chce wyjsc z pokoju");
                                //wypisz z pokoju
                                break;
                            }
                        case 60:
                            {
                                //na sztywno jest
                                dziennik.Items.Add("Gracz " + nazwaKlienta + " chce dolaczyc do gry w pokoju");
                                for (int i = 0; i < Pokoj1.WszyscyGracze.Count; i++)
                                {
                                    if (socket == Pokoj1.WszyscyGracze[i]._Socket)
                                    {
                                        RozgrywkaPokoj1.WszyscyGracze.Add(Pokoj1.WszyscyGracze[i]);
                                    }
                                }
                                //wyslij liste z graczami bioracymi udzial
                                SendCommand(GameSendCommand.ListAllPlayerInRoomToStartGame, 0, 0, null, 0);
                                break;
                            }
                        case 61:
                            {
                                //na sztywno jest //na razie zbedne
                                dziennik.Items.Add("Gracz " + nazwaKlienta + " chce odlaczyc sie od rozgrywki");
                                //potem
                                break;
                            }
                        case 62:
                            {
                                //na sztywno jest
                                dziennik.Items.Add("Gracz " + nazwaKlienta + " chce utworzyc pokoj");
                                for (int i = 0; i < Pokoj1.WszyscyGracze.Count; i++)
                                {
                                    if (socket.RemoteEndPoint.ToString().Equals(Pokoj1.WszyscyGracze[i]._Socket.RemoteEndPoint.ToString()))
                                    {
                                        RozgrywkaPokoj1.WszyscyGracze.Add(Pokoj1.WszyscyGracze[i]);
                                    }
                                }
                                glownyPokoj.nazwa = Encoding.UTF8.GetString(_messageContent);

                                break;
                            }
                        case 63:
                            {
                                //na sztywno jest //na razie zbedne
                                dziennik.Items.Add("Gracz " + nazwaKlienta + " chce usunac swoj pokoj");
                                //potem
                                break;
                            }
                        case 64:
                            {
                                //na sztywno jest //na razie zbedne
                                dziennik.Items.Add("Gracz " + nazwaKlienta + " chce usunac gracza z pokoju pokoj");
                                //uwun konkretnego gracza
                                break;
                            }
                        case 65:
                            {
                                dziennik.Items.Add("Gracz " + nazwaKlienta + " zażądał rozpoczęcia gry");
                                SendCommand(GameSendCommand.ListAllPlayerIDs, 0, 0, null, 0); //kod 2
                                Run();
                                break;
                            }
                        default:
                                dziennik.Items.Add("WIADOMOSC NIEISTOTNA");
                                break;
                    }
                }

                else
                {
                    for (int i = 0; i < Pokoj1.WszyscyGracze.Count; i++)
                    {
                        if (Pokoj1.WszyscyGracze[i]._Socket.RemoteEndPoint.ToString().Equals(socket.RemoteEndPoint.ToString()))
                        {
                            Pokoj1.WszyscyGracze.RemoveAt(i);
                            dziennik.Items.Add("Liczba klientów połączonych: " + Pokoj1.WszyscyGracze.Count.ToString());
                        }
                    }
                }

            }
            try
            {
                socket.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), socket);
            }
            catch (Exception)
            {
                //ignored
            }
        }

        public void SendCommand(GameSendCommand command, int idGracza, int idKarty, Socket socket, int idPokoju, byte[] arg = null)
        {
            switch (command)
            {
                case GameSendCommand.PickedCard:
                    {
                        _toSend = new byte[4];
                        _toSend[3] = Convert.ToByte(idKarty);
                        _toSend[2] = Convert.ToByte(idGracza);
                        _toSend[1] = 5;//code
                        _toSend[0] = Convert.ToByte(_toSend.Length);
                        SendDataToALL(_toSend);
                        break;
                    }
                case GameSendCommand.ListAllRooms:
                    {
                        //na razie sztywniutko (1 pokój)
                        byte[] stringbyte = Encoding.UTF8.GetBytes(glownyPokoj.nazwa);
                        _toSend = new byte[5 + stringbyte.Length];
                        Array.Copy(stringbyte, 0, _toSend, 5, stringbyte.Length);
                        _toSend[4] = Convert.ToByte(glownyPokoj.nazwa.Length);
                        _toSend[3] = Convert.ToByte(glownyPokoj.id);
                        _toSend[2] = Convert.ToByte(1);
                        _toSend[1] = 8;//code
                        _toSend[0] = Convert.ToByte(_toSend.Length);
                        SendDataToALL(_toSend);
                        //Sendata(socket, _toSend);
                        break;
                    }
                case GameSendCommand.ListAllPlayerInRoom:
                    {
                        //na razie sztywniutko (1 pokój)
                        for (int i = 0; i < Pokoj1.WszyscyGracze.Count; i++)
                        {
                            byte[] tempbyte = new byte[2 + Pokoj1.WszyscyGracze[i]._Name.Length];
                            tempbyte[0] = Convert.ToByte(Pokoj1.WszyscyGracze[i].id);
                            tempbyte[1] = Convert.ToByte(Pokoj1.WszyscyGracze[i]._Name.Length);
                            byte[] stringbyte = Encoding.UTF8.GetBytes(Pokoj1.WszyscyGracze[i]._Name);
                            Array.Copy(stringbyte, 0, tempbyte, 2, stringbyte.Length);
                            Buffer.BlockCopy(tempbyte, 0, temp2byte, licznikDlugosci, tempbyte.Length);
                            licznikDlugosci += tempbyte.Length;
                        }
                        _toSend = new byte[3 + licznikDlugosci];
                        Array.Copy(temp2byte, 0, _toSend, 3, licznikDlugosci);
                        licznikDlugosci = 0;
                        _toSend[2] = Convert.ToByte(Pokoj1.WszyscyGracze.Count());
                        _toSend[1] = 9;//code
                        _toSend[0] = Convert.ToByte(_toSend.Length);
                        SendDataToALL(_toSend);
                        //Sendata(socket, _toSend);
                        Array.Clear(temp2byte, 0, temp2byte.Length);
                        break;
                    }
                case GameSendCommand.ListAllPlayerInRoomToStartGame:
                    {
                        licznikDlugosci = 0;
                        //na razie sztywniutko (1 pokój) //na razie to samo co w poprzednim case [ZMIENIC!!!]
                        for (int i = 0; i < RozgrywkaPokoj1.WszyscyGracze.Count; i++)
                        {
                            byte[] tempbyte = new byte[2 + RozgrywkaPokoj1.WszyscyGracze[i]._Name.Length];
                            tempbyte[0] = Convert.ToByte(RozgrywkaPokoj1.WszyscyGracze[i].id);
                            tempbyte[1] = Convert.ToByte(RozgrywkaPokoj1.WszyscyGracze[i]._Name.Length);
                            byte[] stringbyte = Encoding.UTF8.GetBytes(RozgrywkaPokoj1.WszyscyGracze[i]._Name);
                            Array.Copy(stringbyte, 0, tempbyte, 2, stringbyte.Length);
                            Buffer.BlockCopy(tempbyte, 0, temp2byte, licznikDlugosci, tempbyte.Length);
                            licznikDlugosci += tempbyte.Length;
                        }
                        _toSend = new byte[3 + licznikDlugosci];
                        Array.Copy(temp2byte, 0, _toSend, 3, licznikDlugosci);
                        licznikDlugosci = 0;
                        _toSend[2] = Convert.ToByte(RozgrywkaPokoj1.WszyscyGracze.Count());
                        _toSend[1] = 10;//code
                        _toSend[0] = Convert.ToByte(_toSend.Length);
                        //Sendata(socket, _toSend);
                        SendDataToALL(_toSend);
                        Array.Clear(temp2byte, 0, temp2byte.Length);
                        break;
                    }
                case GameSendCommand.NumberOfCards:
                    {//laduj od razu po starcie gry (po wykonaniu Run)
                        licznikDlugosci = 0;
                        for (int i = 0; i < RozgrywkaPokoj1.WszyscyGracze.Count; i++)
                        {
                            byte[] tempbyte = new byte[2 + RozgrywkaPokoj1.WszyscyGracze[i]._Name.Length];
                            tempbyte[0] = Convert.ToByte(RozgrywkaPokoj1.WszyscyGracze[i].id);
                            tempbyte[1] = Convert.ToByte(RozgrywkaPokoj1.WszyscyGracze[i].InHand.Count);
                            Buffer.BlockCopy(tempbyte, 0, temp2byte, licznikDlugosci, tempbyte.Length);
                            licznikDlugosci += tempbyte.Length;
                        }
                        _toSend = new byte[3 + licznikDlugosci];
                        Array.Copy(temp2byte, 0, _toSend, 3, licznikDlugosci);
                        _toSend[2] = Convert.ToByte(RozgrywkaPokoj1.WszyscyGracze.Count());
                        _toSend[1] = 1;//code
                        _toSend[0] = Convert.ToByte(_toSend.Length);
                        SendDataToALL(_toSend);
                        Array.Clear(temp2byte, 0, temp2byte.Length);
                        break;
                    }
                case GameSendCommand.ListAllPlayerIDs:
                    {
                        licznikDlugosci = 0;
                        //na razie kopia 9 (1 pokój etc.)
                        for (int i = 0; i < RozgrywkaPokoj1.WszyscyGracze.Count; i++)
                        {
                            byte[] tempbyte = new byte[2 + RozgrywkaPokoj1.WszyscyGracze[i]._Name.Length];
                            tempbyte[0] = Convert.ToByte(RozgrywkaPokoj1.WszyscyGracze[i].id);
                            tempbyte[1] = Convert.ToByte(RozgrywkaPokoj1.WszyscyGracze[i]._Name.Length);
                            byte[] stringbyte = Encoding.UTF8.GetBytes(RozgrywkaPokoj1.WszyscyGracze[i]._Name);
                            Array.Copy(stringbyte, 0, tempbyte, 2, stringbyte.Length);
                            Buffer.BlockCopy(tempbyte, 0, temp2byte, licznikDlugosci, tempbyte.Length);
                            licznikDlugosci += tempbyte.Length;
                        }
                        _toSend = new byte[3 + licznikDlugosci];
                        Array.Copy(temp2byte, 0, _toSend, 3, licznikDlugosci);
                        _toSend[2] = Convert.ToByte(RozgrywkaPokoj1.WszyscyGracze.Count());
                        _toSend[1] = 2;//code
                        _toSend[0] = Convert.ToByte(_toSend.Length);
                        SendDataToALL(_toSend);
                        //Sendata(socket, _toSend); //moze jednak to :D
                        Array.Clear(temp2byte, 0, temp2byte.Length);
                        break;
                    }
                case GameSendCommand.ActivateGetUpCardButton:
                    {
                        _toSend[1] = 3;//code
                        _toSend[0] = Convert.ToByte(2);//length
                        SendDataToALL(_toSend);
                        break;
                    }
                case GameSendCommand.DeactivateGetUpCardButton:
                    {
                        _toSend[1] = 4;//code
                        _toSend[0] = Convert.ToByte(2);//length
                        SendDataToALL(_toSend);
                        break;
                    }
                //nie jest uberkonieczne
                case GameSendCommand.DuelWin:
                    {
                        _toSend[1] = 6;//code
                        _toSend[0] = Convert.ToByte(2);//length
                        Sendata(socket, _toSend);//tu dodac socket wygranego
                        break;
                    }
                //nie jest uberkonieczne
                case GameSendCommand.DuelLose:
                    {
                        _toSend[1] = 7;//code
                        _toSend[0] = Convert.ToByte(2);//length
                        Sendata(socket, _toSend);//tu dodac socket przegranego
                        break;
                    }
                case GameSendCommand.EndOfGame:
                    {
                        _toSend[2] = Convert.ToByte(RozgrywkaPokoj1.WszyscyGracze[zwyciezca].id);//id zwyciezcy
                        _toSend[1] = 11;//code
                        _toSend[0] = Convert.ToByte(3);//length
                        SendDataToALL(_toSend);
                        break;
                    }
            }

        }

        //wyslij do 1 osoby
        void Sendata(Socket socket, byte[] data)
        {
            socket.BeginSend(data, 0, data.Length, SocketFlags.None, new AsyncCallback(SendCallback), socket);
            _serverSocket.BeginAccept(new AsyncCallback(AppceptCallback), null);
            //_serverSocket.BeginAccept(new AsyncCallback(ReceiveCallback), null);
        }
        //wyslij do wszystkich graczy
        void SendDataToALL(byte[] data)
        {
            foreach (Gracz client in Pokoj1.WszyscyGracze)
            {
                if (client._Socket != _serverSocket)
                {
                    try { 
                    //Send the message to all users
                    client._Socket.BeginSend(data, 0, data.Length, SocketFlags.None,
                        new AsyncCallback(SendCallback), client._Socket);
                    //byc moze sendcallbackall?
                }
                    catch (Exception)
                    {
                        //2016-06-01-edit
                    }
                    }
            }
            _serverSocket.BeginAccept(new AsyncCallback(AppceptCallback), null);
            //_serverSocket.BeginAccept(new AsyncCallback(ReceiveCallback), null);
        }
        void SendDataToAllPlayers(byte[] data)
        {
            foreach (Gracz client in RozgrywkaPokoj1.WszyscyGracze)
            {
                if (client._Socket != _serverSocket)
                {
                    try
                    {
                        //Send the message to all users
                        client._Socket.BeginSend(data, 0, data.Length, SocketFlags.None,
                            new AsyncCallback(SendCallback), client._Socket);
                        //byc moze sendcallbackall?
                    }
                    catch (Exception)
                    {
                        //2016-06-01-edit
                    }
                }
            }
            _serverSocket.BeginAccept(new AsyncCallback(AppceptCallback), null);
            //_serverSocket.BeginAccept(new AsyncCallback(ReceiveCallback), null);
        }
        private void SendCallback(IAsyncResult AR)
        {
            Socket socket = (Socket)AR.AsyncState;
            socket.EndSend(AR);
        }

        #region Logika Gry

        bool isWinner = false;
        public int zwyciezca;
        Talia TaliaDoGry = new Talia();
        Karta temp = new Karta();
        int liczbaGraczy;
        string stanStolu = "";
        public List<Karta> podTotemem = new List<Karta>();
        //1. normalny tryb (porownuj wzor) 
        //2. tryb po wystapieniu karty "kolor" (porownuj po kolorze)

        public void Run()
        {
            Console.WriteLine("Talia przed zmieszaniem");
            TaliaDoGry.UtworzTalie();
            TaliaDoGry.PokazTalie();
            Console.WriteLine("Talia po zmieszaniu");
            TaliaDoGry.ZmieszajKarty();
            TaliaDoGry.PokazTalie();

            Console.WriteLine("Ilu ma być graczy?");
            liczbaGraczy = RozgrywkaPokoj1.WszyscyGracze.Count();
            Console.Write(liczbaGraczy + "\n");

            //1 faza rozgrywki - rozdanie kart
            while ((TaliaDoGry.TaliaKart).Any())
            {
                for (int j = 0; j < liczbaGraczy; j++)
                {
                    temp = TaliaDoGry.TaliaKart[0];
                    if(RozgrywkaPokoj1.WszyscyGracze[j]._Name!="GM"){
                    RozgrywkaPokoj1.WszyscyGracze[j].InHand.Add(temp); //dodanie 1 karty z talii
                    TaliaDoGry.TaliaKart.RemoveRange(0, 1); //usuniecie karty z talii
                    }
                    if (!(TaliaDoGry.TaliaKart).Any()) break;
                }
            }
            SendCommand(GameSendCommand.NumberOfCards, 0, 0, null, 0);
            for (int i = 1; i <= liczbaGraczy; i++)
            {
                Console.WriteLine("Gracz " + i + ":");
                RozgrywkaPokoj1.WszyscyGracze[i - 1].PokazKarty(RozgrywkaPokoj1.WszyscyGracze[i - 1].InHand);
            }

            //2 faza rozgrywki - gra
            while (!isWinner) {
                for (int j = 0; j < RozgrywkaPokoj1.WszyscyGracze.Count; j++)
                {
                    playerHavingTurn = RozgrywkaPokoj1.WszyscyGracze[j].id;
                    Pojedynek();
                    if (RozgrywkaPokoj1.WszyscyGracze[j]._Name == "GM") j++;
                    if (!isCardPickUpRequest) j = j - 1;
                    else{

                    //ruch 1 gracza - jeśli gracz ma jakąś kartę w dłoni, to podnies ją
                    if (RozgrywkaPokoj1.WszyscyGracze[j].InHand.Any())
                    {
                        temp = RozgrywkaPokoj1.WszyscyGracze[j].InHand[0];
                        if (temp.wzor == 21)
                        {
                            Kolory(j);
                        }
                        if (temp.wzor == 22)
                        {
                            WszyscyNaraz(j);
                        }
                        if (temp.wzor == 23)
                        {
                            Glosowanie(j);
                        }
                        RozgrywkaPokoj1.WszyscyGracze[j].OnTable.Add(temp); //wyrzucenie 1 karty na stol
                        RozgrywkaPokoj1.WszyscyGracze[j].InHand.RemoveRange(0, 1); //usuniecie karty z reki
                        
                        SendCommand(GameSendCommand.PickedCard, RozgrywkaPokoj1.WszyscyGracze[j].id, temp.idKarty, null, 0);

                        Pojedynek();

                    }
                    else if (RozgrywkaPokoj1.WszyscyGracze[j].OnTable.Any())
                    {
                        Pojedynek();
                        //RozgrywkaPokoj1.SprawdzCzyNaStoleJestSymbol(RozgrywkaPokoj1.WszyscyGracze[j].OnTable
                            //[RozgrywkaPokoj1.WszyscyGracze[j].OnTable.Count - 1], j);
                    }

                    for (int i = 1; i <= liczbaGraczy; i++)
                    {
                        stanStolu = "G" + i + ":";
                        Console.WriteLine(stanStolu);
                        RozgrywkaPokoj1.WszyscyGracze[i - 1].PokazKarty(RozgrywkaPokoj1.WszyscyGracze[i - 1].InHand);
                    }

                //warunek zwycięski [blokada z j >= 0 tymczasowo :D]
                    if (j >= 0 && !(RozgrywkaPokoj1.WszyscyGracze[j].InHand).Any() && !(RozgrywkaPokoj1.WszyscyGracze[j].OnTable).Any())
                    {
                        isWinner = true;
                        zwyciezca = RozgrywkaPokoj1.WszyscyGracze[j].id;
                        Console.WriteLine("Zwyciężył gracz: " + RozgrywkaPokoj1.WszyscyGracze[j].id + ". GRATULUJĘ!");
                        SendCommand(GameSendCommand.EndOfGame,0,0,null,0,null);
                    }
                    if (isWinner == true)
                    {
                        break;
                    }
                    //mamy zwyciezce - zatrzymaj petle glowna
                    isCardPickUpRequest = false;
                    }
                }
            }
        }

        private void Pojedynek()
        {
            duelIsOn = true;
            Stopwatch sw = new Stopwatch(); // sw cotructor
            sw.Start(); // starts the stopwatch
            for (int i = 0; ; i++)
            {
                if (i % 100000 == 0) // if in 100000th iteration (could be any other large number
                // depending on how often you want the time to be checked) 
                {
                    sw.Stop(); // stop the time measurement
                    if (sw.ElapsedMilliseconds > 5000) // check if desired period of time has elapsed
                    {
                        break; // if more than 5000 milliseconds have passed, stop looping and return
                        // to the existing code
                    }
                    else
                    {
                        sw.Start(); // if less than 5000 milliseconds have elapsed, continue looping
                        // and resume time measurement
                        if (isTakeTotemRequest) { 
                            RozgrywkaPokoj1.SprawdzCzyNaStoleJestSymbol(temp, playerTakingTotem);
                            duelIsOn = false;
                            isTakeTotemRequest = false;
                        }
                    }
                    sw.Stop();
                    break;
                }
            }
        }

        public void Kolory(int graczWyrzucajacyKolor)
        {
            int i = 1;
            for (int j = 0; j < liczbaGraczy; j++)
            {
                i = j + graczWyrzucajacyKolor;
                if (i >= liczbaGraczy) i = 0;
                if (RozgrywkaPokoj1.WszyscyGracze[i].InHand.Any())
                {

                    for (int k = 0; k < 1; k++)
                    {
                        temp = RozgrywkaPokoj1.WszyscyGracze[i].InHand[k];
                    }

                    if (RozgrywkaPokoj1.SprawdzCzyNaStoleJestKolor(temp, i, liczbaGraczy)) break;

                    RozgrywkaPokoj1.WszyscyGracze[i].OnTable.Add(temp); //wyrzucenie 1 karty na stol
                    RozgrywkaPokoj1.WszyscyGracze[i].InHand.RemoveRange(0, 1); //usuniecie karty z reki
                }
                else if (RozgrywkaPokoj1.WszyscyGracze[i].OnTable.Any())
                {
                    if (RozgrywkaPokoj1.SprawdzCzyNaStoleJestKolor(RozgrywkaPokoj1.WszyscyGracze[i].OnTable
                        [RozgrywkaPokoj1.WszyscyGracze[i].OnTable.Count - 1], i, liczbaGraczy))
                        break;
                }
                for (int a = 1; a <= liczbaGraczy; a++)
                {
                    stanStolu = "G" + a + ":";
                    Console.WriteLine(stanStolu);
                    RozgrywkaPokoj1.WszyscyGracze[a - 1].PokazKarty(RozgrywkaPokoj1.WszyscyGracze[a - 1].InHand);
                }
                Console.WriteLine();
                if (!(RozgrywkaPokoj1.WszyscyGracze[i].InHand).Any() && !(RozgrywkaPokoj1.WszyscyGracze[i].OnTable).Any())
                {
                    isWinner = true;
                    zwyciezca = i;
                    Console.WriteLine("Zwyciężył gracz: " + (i + 1) + ". GRATULUJĘ!");
                }
                if (isWinner == true)
                {
                    break;
                }
            }
        }

        public void Glosowanie(int j)
        {

            Console.WriteLine("Ktory z graczy ma oddac karty pod totem?");
            podTotemem.AddRange(RozgrywkaPokoj1.WszyscyGracze[j].OnTable);
            RozgrywkaPokoj1.WszyscyGracze[j].OnTable.Clear();
            if (!(RozgrywkaPokoj1.WszyscyGracze[j].InHand).Any() && !(RozgrywkaPokoj1.WszyscyGracze[j].OnTable).Any())
            {
                isWinner = true;
                Console.WriteLine("Zwyciężył gracz: " + (j + 1) + ". GRATULUJĘ!");
            }

        }

        public void WszyscyNaraz(int j)
        {
            //brak mechanizmu
            //timer odliczanie do 3 sekund i mozliwosc chwycenia przez kogokolwiek
        }


    }
    #endregion
}