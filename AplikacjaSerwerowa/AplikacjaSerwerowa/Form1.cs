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

        public byte[] temp2byte;
        private byte[] _buffer = new byte[256];
        private byte[] byteToSend = new byte[256];
        private byte[] _toSend = new byte[256];
        private byte[] _messageContent = new byte[256];
        public static Gracze Pokoj1 = new Gracze();
        public Socket _serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        

        #region Rzeczy do zmienienia
        public static Pokoj glownyPokoj = new Pokoj(0,"Glowny pokoj");
        LogikaGry NowaGra = new LogikaGry(glownyPokoj);
        #endregion

        int dlugoscPakietu, kodWiadomosci;
        string daneWiadomosci = "", nazwaKlienta = "";

        private void Form1_Load(object sender, EventArgs e)
        {
        }

        private void SetupServer()
        {
            try
            {
                //dziennik.Items.Add("Serwer włącza się...");
                _serverSocket.Bind(new IPEndPoint(IPAddress.Any, 1000));
                _serverSocket.Listen(4); //sprawdz co to dokladnie robi
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

            Pokoj1.WszyscyGracze.Add(new Gracz(socket));
            listaGraczy.Items.Add(socket.RemoteEndPoint.ToString());

            dziennik.Items.Add("Hura, dołączył kolejny gracz! Obecna liczba graczy to: " + Pokoj1.WszyscyGracze.Count.ToString());
            //dziennik.Items.Add"Klient połączony...";
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
                    //Encoding.UTF8.GetBytes

                    dlugoscPakietu = _buffer[0];
                    kodWiadomosci = _buffer[1];
                    if((dlugoscPakietu-2)>0){
                        for (int i = 0; i < dlugoscPakietu - 2; i++) {//slice etc.?
                            _messageContent[i] = _buffer[2+i];
                        }
                    }

                    switch (kodWiadomosci)
                    {
					//DOIMPLEMENTOWAĆ
					//gracz żąda podniesienia totemu
						case 50:
                            {
                                dziennik.Items.Add("Gracz " + nazwaKlienta + " chwycił totem");
                                for (int i = 0; i < Pokoj1.WszyscyGracze.Count; i++)
                                {
                                    if (socket == Pokoj1.WszyscyGracze[i]._Socket)
                                    {
                                        Pokoj1.SprawdzCzyNaStoleJestSymbol(Pokoj1.WszyscyGracze[i].OnTable
                                            [Pokoj1.WszyscyGracze[i].OnTable.Count-1], Pokoj1.WszyscyGracze[i].id);
                                    }
                                    //SendCommand 2x (do zwyciezcy i do przegranego)
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
                                for (int i = 0; i < Pokoj1.WszyscyGracze.Count; i++)
                                {
                                    if (socket == Pokoj1.WszyscyGracze[i]._Socket)
                                    {
                                        //kod na odwrocenie (przerobic lekko logike)
                                    }
                                }
                                //poinformowanie wszystkich graczy
                                SendCommand(GameSendCommand.PickedCard, 1, 2, null, 0);

                                break;
                            }
                        //DOIMPLEMENTOWAĆ PÓŹNIEJ
                        //gracz wychodzi z gry
                        case 53:
                            {
                                dziennik.Items.Add("Gracz " + nazwaKlienta + " niestety wyszedł z gry :(");

                                //CzyjRuch.Gracz
                                for (int i = 0; i < Pokoj1.WszyscyGracze.Count; i++)
                                {

                                    //DOIMPLEMENTUJ TO GUNWO
                                }
                                //poinformowanie wszystkich graczy
                                SendCommand(GameSendCommand.PickedCard, 1, 2, null, 0);

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
                                SendCommand(GameSendCommand.ListAllRooms, 0 , 0, socket, 0);
                                break;
                            }
                        case 56:
                            {
                                //na szytwno bo jest 1 pokój xD
                                //potem trzeba dopisac obsluge pola [ID pokoju(byte)]
                                dziennik.Items.Add("Gracz " + nazwaKlienta + " zażądał podania nazw graczy oraz ID gracz z danego pokoju");
                                //poinformowanie gracza o dostępnych pokojach
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
                                //na sztywno jest
                                dziennik.Items.Add("Gracz " + nazwaKlienta + " chce dolaczyc do pokoju");
                                //dodaj do pokoju
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
                                //potem
                                break;
                            }
                        case 61:
                            {
                                //na sztywno jest
                                dziennik.Items.Add("Gracz " + nazwaKlienta + " chce odlaczyc sie od rozgrywki");
                                //potem
                                break;
                            }
                        case 62:
                            {
                                //na sztywno jest
                                dziennik.Items.Add("Gracz " + nazwaKlienta + " chce utworzyc pokoj");
                                //potem
                                break;
                            }
                        case 63:
                            {
                                //na sztywno jest
                                dziennik.Items.Add("Gracz " + nazwaKlienta + " chce usunac swoj pokoj");
                                //potem
                                break;
                            }
                        case 64:
                            {
                                //na sztywno jest
                                dziennik.Items.Add("Gracz " + nazwaKlienta + " chce usunac swoj pokoj");
                                //uwun konkretnego gracza
                                break;
                            }
                        case 65:
                            {
                                dziennik.Items.Add("Gracz " + nazwaKlienta + " zażądał rozpoczęcia gry");
                                //na razie na sztywno, ale tu musi byc doimlementowane
                                NowaGra.Run();
                                break;
                            }
                        default:
                            dziennik.Items.Add("WIADOMOSC NIEISTOTNA");
                            break;
                    }
                    /*
                    if (msgToSend.cmdCommand != Command.List)   //List messages are not broadcasted
                    {
                        message = msgToSend.ToByte();

                        foreach (KlientSerwera client in __ClientSockets)
                        {
                            if (client._Socket != socket ||
                                msgToSend.cmdCommand != Command.Login)
                            {
                                //Send the message to all users
                                client._Socket.BeginSend(message, 0, message.Length, SocketFlags.None,
                                    new AsyncCallback(SendCallback), client._Socket);
                            }
                        }

                        Invoke((MethodInvoker)delegate()
                        {
                            dziennik.Items.Add(msgToSend.strMessage);
                        });
                    }*/

                    /*

                    //If the user is logging out then we need not listen from her
                    if (msgReceived.cmdCommand != Command.Logout)
                    {
                        //Start listening to the message send by the user
                        socket.BeginReceive(message, 0, message.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), socket);
                    }
                    */

                    //Dziennik.Text = "Otrzymano tekst: " + text;


                    /*
                    for (int i = 0; i < __ClientSockets.Count; i++)
                    {
                        if (socket.RemoteEndPoint.ToString().Equals(__ClientSockets[i]._Socket.RemoteEndPoint.ToString()))
                        {
                            dziennik.Items.Add("\n" + __ClientSockets[i]._Socket.RemoteEndPoint.ToString() + ": " + text);
                        }
                    }
                     */

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
            socket.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), socket);
        }

        

		public void SendCommand(GameSendCommand command, int idGracza, int idKarty, Socket socket, int idPokoju, byte[] arg=null)
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
                    Sendata(socket, _toSend);
                    break;
                }
                case GameSendCommand.ListAllPlayerInRoom:
                {
                    //na razie sztywniutko (1 pokój)
                    for (int i = 0; i < Pokoj1.WszyscyGracze.Count; i++)
                    {
                            byte[] tempbyte = new byte[2+Pokoj1.WszyscyGracze[i]._Name.Length];
                            tempbyte[0]=Convert.ToByte(Pokoj1.WszyscyGracze[i].id);
                            tempbyte[1] = Convert.ToByte(Pokoj1.WszyscyGracze[i]._Name.Length);
                            byte[] stringbyte = Encoding.UTF8.GetBytes(Pokoj1.WszyscyGracze[i]._Name);
                            Array.Copy(stringbyte, 0, tempbyte, 2, stringbyte.Length);
                            Buffer.BlockCopy(tempbyte, 0, temp2byte, temp2byte.Length, tempbyte.Length);
                    }
                    _toSend = new byte[3 + temp2byte.Length];
                    Array.Copy(temp2byte, 0, _toSend, 3, temp2byte.Length);
                    _toSend[2] = Convert.ToByte(Pokoj1.WszyscyGracze.Count());
                    _toSend[1] = 9;//code
                    _toSend[0] = Convert.ToByte(_toSend.Length);
                    Sendata(socket, _toSend);
                    Array.Clear(temp2byte, 0, temp2byte.Length);
                    break;
                }
                case GameSendCommand.ListAllPlayerInRoomToStartGame:
                {
                    //na razie sztywniutko (1 pokój) //na razie to samo co w poprzednim case [ZMIENIC!!!]
                    for (int i = 0; i < Pokoj1.WszyscyGracze.Count; i++)
                    {
                        byte[] tempbyte = new byte[2 + Pokoj1.WszyscyGracze[i]._Name.Length];
                        tempbyte[0] = Convert.ToByte(Pokoj1.WszyscyGracze[i].id);
                        tempbyte[1] = Convert.ToByte(Pokoj1.WszyscyGracze[i]._Name.Length);
                        Buffer.BlockCopy(tempbyte, 0, temp2byte, temp2byte.Length, tempbyte.Length);
                    }
                    _toSend = new byte[3 + temp2byte.Length];
                    Array.Copy(temp2byte, 0, _toSend, 3, temp2byte.Length);
                    _toSend[2] = Convert.ToByte(Pokoj1.WszyscyGracze.Count());
                    _toSend[1] = 10;//code
                    _toSend[0] = Convert.ToByte(_toSend.Length);
                    SendDataToALL(_toSend);
                    Array.Clear(temp2byte, 0, temp2byte.Length);
                    break;
                }
                case GameSendCommand.NumberOfCards:
                {//laduj od razu po starcie gry (po wykonaniu Run)
                    for (int i = 0; i < Pokoj1.WszyscyGracze.Count; i++)
                    {
                        byte[] tempbyte = new byte[2 + Pokoj1.WszyscyGracze[i]._Name.Length];
                        tempbyte[0] = Convert.ToByte(Pokoj1.WszyscyGracze[i].id);
                        tempbyte[1] = Convert.ToByte(Pokoj1.WszyscyGracze[i].InHand);
                        Buffer.BlockCopy(tempbyte, 0, temp2byte, temp2byte.Length, tempbyte.Length);
                    }
                    _toSend = new byte[3 + temp2byte.Length];
                    Array.Copy(temp2byte, 0, _toSend, 3, temp2byte.Length);
                    _toSend[2] = Convert.ToByte(Pokoj1.WszyscyGracze.Count());
                    _toSend[1] = 1;//code
                    _toSend[0] = Convert.ToByte(_toSend.Length);
                    SendDataToALL(_toSend);
                    Array.Clear(temp2byte, 0, temp2byte.Length);
                    break;
                }
                case GameSendCommand.ListAllPlayerIDs:
                {
                    //na razie kopia 9 (1 pokój etc.)
                    for (int i = 0; i < Pokoj1.WszyscyGracze.Count; i++)
                    {
                        byte[] tempbyte = new byte[2 + Pokoj1.WszyscyGracze[i]._Name.Length];
                        tempbyte[0] = Convert.ToByte(Pokoj1.WszyscyGracze[i].id);
                        tempbyte[1] = Convert.ToByte(Pokoj1.WszyscyGracze[i]._Name.Length);
                        byte[] stringbyte = Encoding.UTF8.GetBytes(Pokoj1.WszyscyGracze[i]._Name);
                        Array.Copy(stringbyte, 0, tempbyte, 2, stringbyte.Length);
                        Buffer.BlockCopy(tempbyte, 0, temp2byte, temp2byte.Length, tempbyte.Length);
                    }
                    _toSend = new byte[3 + temp2byte.Length];
                    Array.Copy(temp2byte, 0, _toSend, 3, temp2byte.Length);
                    _toSend[2] = Convert.ToByte(Pokoj1.WszyscyGracze.Count());
                    _toSend[1] = 2;//code
                    _toSend[0] = Convert.ToByte(_toSend.Length);
                    Sendata(socket, _toSend);
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
                    _toSend[2] = Convert.ToByte(Pokoj1.WszyscyGracze[NowaGra.zwyciezca].id);//id zwyciezcy
                    _toSend[1] = 11;//code
                    _toSend[0] = Convert.ToByte(3);//length
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
			foreach (Gracz client in Pokoj1.WszyscyGracze) {
				if (client._Socket != _serverSocket) {
					//Send the message to all users
					client._Socket.BeginSend (data, 0, data.Length, SocketFlags.None,
						new AsyncCallback (SendCallback), client._Socket);
					//byc moze sendcallbackall?
				}
			}
			_serverSocket.BeginAccept (new AsyncCallback (AppceptCallback), null);
            //_serverSocket.BeginAccept(new AsyncCallback(ReceiveCallback), null);
		}
        private void SendCallback(IAsyncResult AR)
        {
            Socket socket = (Socket)AR.AsyncState;
            socket.EndSend(AR);
        }
        private void button1_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < listaGraczy.SelectedItems.Count; i++)
            {
                string t = listaGraczy.SelectedItems[i].ToString();
                for (int j = 0; j < Pokoj1.WszyscyGracze.Count; j++)
                {
                    //if (__ClientSockets[j]._Socket.Connected && __ClientSockets[j]._Name.Equals("@"+t))
                    {
                        //potrzebne to?
                    }
                }
            }
        }

    }
}