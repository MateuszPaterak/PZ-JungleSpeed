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
    enum Command
    {
        Login,      //Log into the server
        Logout,     //Logout of the server
        Message,    //Send a text message to all the chat clients
        List,       //Get a list of users in the chat room from the server
        Null        //No command
    }

    public partial class Form1 : Form
    {
        public Form1()
        {
            CheckForIllegalCrossThreadCalls = false;
            __ClientSockets = new List<Gracz>();
            InitializeComponent();
            SetupServer();
        }

        private byte[] _buffer = new byte[256];
        //byte[] _toSend = new byte[];
        public static List<Gracz> __ClientSockets;
        public Socket _serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        string sekretnaWiadomosc = "Witamy w pokoju nr 1";
        
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

            __ClientSockets.Add(new Gracz(socket));
            listaGraczy.Items.Add(socket.RemoteEndPoint.ToString());

            dziennik.Items.Add("Hura, dołączył kolejny gracz! Obecna liczba graczy to: " + __ClientSockets.Count.ToString());
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
                    for (int i = 0; i < __ClientSockets.Count; i++)
                    {
                        if (__ClientSockets[i]._Socket.RemoteEndPoint.ToString().Equals(socket.RemoteEndPoint.ToString()))
                        {
                            __ClientSockets.RemoveAt(i);
                            listaGraczy.Items.RemoveAt(i);
                            dziennik.Items.Add("Gracz niestety opuścił serwer. Obecna liczba graczy to: " + __ClientSockets.Count.ToString());

                        }
                    }
                    return;
                }
                if (received != 0)
                {
                    //Encoding.UTF8.GetBytes

                    dlugoscPakietu = _buffer[0];
                    kodWiadomosci = _buffer[1];
                    if((dlugoscPakietu-2)!=0){
                        daneWiadomosci = Encoding.UTF8.GetString(_buffer,2,dlugoscPakietu-2);
                        daneWiadomosci = daneWiadomosci.Replace(" ",string.Empty);
                    }

                    switch (kodWiadomosci)
                    {
                        case 50:
                            {
                                dziennik.Items.Add("Gracz " + nazwaKlienta + " chwycił totem");
                                break;
                            }
                        case 51:
                            {
                                dziennik.Items.Add("Gracz " + nazwaKlienta + " wybrał " + daneWiadomosci);
                                break;
                                //tego nie kmin na razie, bo dodatkowe
                            }
                        case 52:
                            {
                                dziennik.Items.Add("Gracz " + nazwaKlienta + " obrócił kartę");
                                for (int i = 0; i < __ClientSockets.Count; i++)
                                {
                                    if (__ClientSockets[i]._Socket.RemoteEndPoint.ToString().Equals(socket.RemoteEndPoint.ToString()))
                                    {
                                        __ClientSockets.RemoveAt(i);
                                        listaGraczy.Items.RemoveAt(i);
                                    }
                                }
                                //poinformowanie wszystkich graczy
                                //SendDataToALL("aaa");

                                break;
                            }
                        case 65:
                            {
                                dziennik.Items.Add("Gracz " + nazwaKlienta + " zażądał rozpoczęcia gry");
                                LogikaGry NowaGra = new LogikaGry();
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

                    string text = Encoding.ASCII.GetString(_buffer);
                    //Dziennik.Text = "Otrzymano tekst: " + text;

                    string reponse = string.Empty;

                    /*
                    for (int i = 0; i < __ClientSockets.Count; i++)
                    {
                        if (socket.RemoteEndPoint.ToString().Equals(__ClientSockets[i]._Socket.RemoteEndPoint.ToString()))
                        {
                            dziennik.Items.Add("\n" + __ClientSockets[i]._Socket.RemoteEndPoint.ToString() + ": " + text);
                        }
                    }
                     */

                    reponse = "Serwer otrzymał wiadomość o treści: " + text;
                    Sendata(socket, reponse);
                }
               
                else
                {
                    for (int i = 0; i < __ClientSockets.Count; i++)
                    {
                        if (__ClientSockets[i]._Socket.RemoteEndPoint.ToString().Equals(socket.RemoteEndPoint.ToString()))
                        {
                            __ClientSockets.RemoveAt(i);
                            dziennik.Items.Add("Liczba klientów połączonych: " + __ClientSockets.Count.ToString());
                        }
                    }
                }
                
            }
            socket.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), socket);
        }
        void Sendata(Socket socket, string message)
        {
            byte[] data = Encoding.ASCII.GetBytes(message);
            socket.BeginSend(data, 0, data.Length, SocketFlags.None, new AsyncCallback(SendCallback), socket);
            _serverSocket.BeginAccept(new AsyncCallback(AppceptCallback), null);
        }
        void SendDataToALL(byte[] data)
        {
            foreach (Gracz client in __ClientSockets)
            {
                if (client._Socket != _serverSocket)
                {
                    //Send the message to all users
                    client._Socket.BeginSend(data, 0, data.Length, SocketFlags.None,
                        new AsyncCallback(SendCallback), client._Socket);
                    //byc moze sendcallbackall?
                }
            }
            _serverSocket.BeginAccept(new AsyncCallback(AppceptCallback), null);
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
                for (int j = 0; j < __ClientSockets.Count; j++)
                {
                    //if (__ClientSockets[j]._Socket.Connected && __ClientSockets[j]._Name.Equals("@"+t))
                    {
                        Sendata(__ClientSockets[j]._Socket, sekretnaWiadomosc);
                        dziennik.Items.Add(sekretnaWiadomosc);
                    }
                }
            }
        }

    }
}