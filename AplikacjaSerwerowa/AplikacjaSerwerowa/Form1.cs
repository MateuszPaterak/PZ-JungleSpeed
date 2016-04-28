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
            __ClientSockets = new List<KlientSerwera>();
            InitializeComponent();
            SetupServer();
        }

        private byte[] _buffer = new byte[8];
        public List<KlientSerwera> __ClientSockets { get; set; }
        List<string> _names = new List<string>();
        private Socket _serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        string sekretnaWiadomosc = "Witamy w pokoju nr 1";
        string odpowiedzAplikacji = "";
        string nazwaKlienta = "";

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

            __ClientSockets.Add(new KlientSerwera(socket));
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
                    odpowiedzAplikacji = Encoding.ASCII.GetString(_buffer);
                    odpowiedzAplikacji = odpowiedzAplikacji.Replace(" ",string.Empty);


                    switch (odpowiedzAplikacji)
                    {

                        case "FLIPCARD":
                            {
                                dziennik.Items.Add("Gracz " + nazwaKlienta + " obrócił kartę");
                                break;
                            }

                        case "GRABTOTE":
                            {
                                dziennik.Items.Add("Gracz " + nazwaKlienta + " chwycił totem");
                                break;
                            }

                            /*
                        case Command.Login:

                            //When a user logs in to the server then we add her to our
                            //list of clients

                            KlientSerwera Gracz = new KlientSerwera();
                            Gracz._Socket = socket;
                            Gracz._Name = msgReceived.strName;

                            listaGraczy.Items.Add(Gracz);

                            //Set the text of the message that we will broadcast to all users

                            msgToSend.strMessage = "<<<" + msgReceived.strName + " pojawił się na serwerze!>>>";
                            break;

                        case Command.Logout:

                            //When a user wants to log out of the server then we search for her 
                            //in the list of clients and close the corresponding connection

                            int nIndex = 0;
                            foreach (KlientSerwera client in __ClientSockets)
                            {
                                if (client._Socket == socket)
                                {
                                    listaGraczy.Items.RemoveAt(nIndex);
                                    __ClientSockets.RemoveAt(nIndex);
                                    break;
                                }
                                ++nIndex;
                                (client._Socket).Close(); //działa?
                            }

                            msgToSend.strMessage = "<<<" + msgReceived.strName + " opuścił ten przybytek :(>>>";
                            break;

                        case Command.Message:

                            //Set the text of the message that we will broadcast to all users
                            msgToSend.strMessage = msgReceived.strName + ": " + msgReceived.strMessage;
                            break;

                        case Command.List:

                            //Send the names of all users in the chat room to the new user
                            msgToSend.cmdCommand = Command.List;
                            msgToSend.strName = null;
                            msgToSend.strMessage = null;

                            //Collect the names of the user in the chat room
                            foreach (KlientSerwera client in __ClientSockets)
                            {
                                //To keep things simple we use asterisk as the marker to separate the user names
                                msgToSend.strMessage += client._Name + "*";
                             *  */


                            //message = msgToSend.ToByte();

                            //Send the name of the users in the chat room
                            //socket.BeginSend(message, 0, message.Length, SocketFlags.None,
                             //       new AsyncCallback(SendCallback), socket);
                           //break;
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
        void Sendata(Socket socket, string noidung)
        {
            byte[] data = Encoding.ASCII.GetBytes(noidung);
            socket.BeginSend(data, 0, data.Length, SocketFlags.None, new AsyncCallback(SendCallback), socket);
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



        public class KlientSerwera
        {
            public Socket _Socket { get; set; }
            public string _Name { get; set; }
            public KlientSerwera(Socket socket)
            {
                this._Socket = socket;
            }
            public KlientSerwera()
            {

            }
        }



        public class Talia
        {
            public List<Karta> TaliaKart = new List<Karta>();
            private static Random rng = new Random();

            public void UtworzTalie()
            {
                TaliaKart.Add(new Karta() { wzor = 1, kolor = "czarny" });
                TaliaKart.Add(new Karta() { wzor = 1, kolor = "zielony" });
                TaliaKart.Add(new Karta() { wzor = 1, kolor = "niebieski" });
                TaliaKart.Add(new Karta() { wzor = 1, kolor = "czerwony" });
                TaliaKart.Add(new Karta() { wzor = 2, kolor = "czarny" });
                TaliaKart.Add(new Karta() { wzor = 2, kolor = "zielony" });
                TaliaKart.Add(new Karta() { wzor = 2, kolor = "niebieski" });
                TaliaKart.Add(new Karta() { wzor = 2, kolor = "czerwony" });
                /*TaliaKart.Add (new Karta () { wzor = 3, kolor = "czarny" });
                TaliaKart.Add (new Karta () { wzor = 3, kolor = "zielony" });
                TaliaKart.Add (new Karta () { wzor = 3, kolor = "niebieski" });
                TaliaKart.Add (new Karta () { wzor = 3, kolor = "czerwony" });
                TaliaKart.Add (new Karta () { wzor = 4, kolor = "czarny" });
                TaliaKart.Add (new Karta () { wzor = 4, kolor = "zielony" });
                TaliaKart.Add (new Karta () { wzor = 4, kolor = "niebieski" });
                TaliaKart.Add (new Karta () { wzor = 4, kolor = "czerwony" });
                TaliaKart.Add (new Karta () { wzor = 5, kolor = "czarny" });
                TaliaKart.Add (new Karta () { wzor = 5, kolor = "zielony" });
                TaliaKart.Add (new Karta () { wzor = 5, kolor = "niebieski" });
                TaliaKart.Add (new Karta () { wzor = 5, kolor = "czerwony" });
                */
                //96 kart "normalnych", 24 rozne wzory
                //TaliaKart.Add (new Karta () { wzor = 25, kolor = "specjalny"}); //kolor
                //TaliaKart.Add (new Karta () { wzor = 26, kolor = "specjalny"}); //wszyscy naraz
                //TaliaKart.Add (new Karta () { wzor = 27, kolor = "specjalny"}); //glosowanie
                //8 kart specjalnych lacznie - 3x glosowanie, 3x wszyscy naraz, 2x kolor
            }

            public void PokazTalie()
            {
                TaliaKart.ForEach(delegate(Karta abc)
                {
                    Console.WriteLine(abc.wzor + " " + abc.kolor);
                });
            }

            public void ZmieszajKarty()
            {

                int n = TaliaKart.Count;
                while (n > 1)
                {
                    n--;
                    int k = rng.Next(n + 1);
                    Karta value = TaliaKart[k];
                    TaliaKart[k] = TaliaKart[n];
                    TaliaKart[n] = value;
                }
            }

        }

        public class Karta
        {
            public int wzor;
            public string kolor;
            //img
        }

        public class Gracze
        {
            public List<Gracz> WszyscyGracze = new List<Gracz>();
            int winner;
            string KartyGracz1 = "";
            string KartyGracz2 = "";

            public void DodajGracza()
            {
                WszyscyGracze.Add(new Gracz() { });
            }

            public void SprawdzCzyNaStoleJestSymbol(Karta Karta1, int IDGracza, int liczbaGraczy)
            {
                for (int i = 0; i < liczbaGraczy; i++)
                {
                    if ((WszyscyGracze[i].OnTable).Any())
                    {
                        if (IDGracza != i && Karta1.wzor == WszyscyGracze[i].OnTable[WszyscyGracze[i].OnTable.Count - 1].wzor)
                        {
                            Walka(IDGracza, i);

                        }
                    }
                }

            }

            public bool SprawdzCzyNaStoleJestKolor(Karta Karta1, int IDGracza, int liczbaGraczy)
            {
                bool colorMatch = false;
                for (int i = 0; i < liczbaGraczy; i++)
                {
                    if ((WszyscyGracze[i].OnTable).Any())
                    {
                        if (IDGracza != i && Karta1.kolor == WszyscyGracze[i].OnTable[WszyscyGracze[i].OnTable.Count - 1].kolor)
                        {
                            Walka(IDGracza, i);
                            colorMatch = true;
                        }
                    }


                }
                if (colorMatch)
                    return true;
                else
                    return false;
            }

            public void Walka(int Gracz1, int Gracz2)
            {
                if (WszyscyGracze[Gracz1].OnTable.Any())
                {
                    KartyGracz1 = WszyscyGracze[Gracz1].OnTable[WszyscyGracze[Gracz1].OnTable.Count - 1].wzor + " " + WszyscyGracze[Gracz1].OnTable[WszyscyGracze[Gracz1].OnTable.Count - 1].kolor;
                }
                else
                {
                    KartyGracz1 = "null";
                }
                if (WszyscyGracze[Gracz2].OnTable.Any())
                {
                    KartyGracz2 = WszyscyGracze[Gracz2].OnTable[WszyscyGracze[Gracz2].OnTable.Count - 1].wzor + " " + WszyscyGracze[Gracz2].OnTable[WszyscyGracze[Gracz2].OnTable.Count - 1].kolor;

                }
                else
                {
                    KartyGracz2 = "null";
                }
                Console.WriteLine("JEST WALKA! Biją się karty: " + KartyGracz1 + " oraz " + KartyGracz2);
                Console.WriteLine("Jest WALKA! Kto ją wygrywa? Wpisz gracz " + (Gracz1 + 1) + " lub " + (Gracz2 + 1));
                winner = Convert.ToInt32(Console.ReadLine());


                if (winner == (Gracz1 + 1))
                {
                    WszyscyGracze[Gracz2].InHand.AddRange(WszyscyGracze[Gracz1].OnTable);
                    WszyscyGracze[Gracz2].InHand.AddRange(WszyscyGracze[Gracz2].OnTable);
                    WszyscyGracze[Gracz2].OnTable.Clear();
                    WszyscyGracze[Gracz1].OnTable.Clear();
                }
                if (winner == (Gracz2 + 1))
                {
                    WszyscyGracze[Gracz1].InHand.AddRange(WszyscyGracze[Gracz1].OnTable);
                    WszyscyGracze[Gracz1].InHand.AddRange(WszyscyGracze[Gracz2].OnTable);
                    WszyscyGracze[Gracz2].OnTable.Clear();
                    WszyscyGracze[Gracz1].OnTable.Clear();
                }
            }

        }
        public class Gracz
        {
            //public string nickname = "";
            public bool isActive = false;
            public List<Karta> InHand = new List<Karta>();
            public List<Karta> OnTable = new List<Karta>();
            public Karta OstatniaKarta = new Karta();

            public void PokazKarty(List<Karta> ZestawKart)
            {

                ZestawKart.ForEach(delegate(Karta abc)
                {
                    Console.WriteLine(abc.wzor + " " + abc.kolor);
                });
            }

        }

        class Symulacja
        {

            bool isWinner = false;
            Talia TaliaDoGry = new Talia();
            Gracze Grajacy = new Gracze();
            Karta temp = new Karta();
            int liczbaGraczy;
            string stanStolu = "";
            public List<Karta> podTotemem = new List<Karta>();
            //1. normalny tryb (porownuj wzor) 
            //2. tryb po wystapieniu karty "kolor" (porownuj po kolorze)

            public static void SymulujGre()
            {
                Symulacja a = new Symulacja();
                a.Run();
            }

            public void Run()
            {

                Console.WriteLine("Talia przed zmieszaniem");
                TaliaDoGry.UtworzTalie();
                TaliaDoGry.PokazTalie();
                Console.WriteLine("Talia po zmieszaniu");
                TaliaDoGry.ZmieszajKarty();
                TaliaDoGry.PokazTalie();

                Console.WriteLine("Ilu ma być graczy?");
                liczbaGraczy = Convert.ToInt32(Console.ReadLine());
                Console.Write(liczbaGraczy);

                for (int i = 0; i < liczbaGraczy; i++)
                {
                    Grajacy.DodajGracza();
                }

                //1 faza rozgrywki - rozdanie kart
                while ((TaliaDoGry.TaliaKart).Any())
                {
                    for (int j = 0; j < liczbaGraczy; j++)
                    {
                        for (int i = 0; i < 1; i++) { temp = TaliaDoGry.TaliaKart[i]; }
                        //Console.WriteLine("SPR1: " + temp.wzor + " " + temp.kolor);
                        Grajacy.WszyscyGracze[j].InHand.Add(temp); //dodanie 1 karty z talii
                        TaliaDoGry.TaliaKart.RemoveRange(0, 1); //usuniecie karty z talii
                        //for (int i = 0; i < 1; i++) { temp = abc.InHand[i]; }
                        //Console.WriteLine("SPR2: " + temp.wzor + " " + temp.kolor);
                        //Grajacy.WszyscyGracze[0].InHand.RemoveRange (0,1);
                        if (!(TaliaDoGry.TaliaKart).Any()) break;
                    }
                }
                for (int i = 1; i <= liczbaGraczy; i++)
                {
                    Console.WriteLine("Gracz " + i + ":");
                    Grajacy.WszyscyGracze[i - 1].PokazKarty(Grajacy.WszyscyGracze[i - 1].InHand);
                }

                //2 faza rozgrywki - gra
                while (!isWinner)
                {

                    //normalny ruch
                    for (int j = 0; j < liczbaGraczy; j++)
                    {
                        if (Grajacy.WszyscyGracze[j].InHand.Any())
                        {
                            for (int i = 0; i < 1; i++)
                            {
                                temp = Grajacy.WszyscyGracze[j].InHand[i];
                            }
                            /*if (temp.wzor == 25) {
                                Kolory (j);
                                break;
                            }
                            if (temp.wzor == 27) {
                                Glosowanie (j);
                                break;
                            }
                            */
                            Grajacy.WszyscyGracze[j].OnTable.Add(temp); //wyrzucenie 1 karty na stol
                            Grajacy.SprawdzCzyNaStoleJestSymbol(temp, j, liczbaGraczy);
                            Grajacy.WszyscyGracze[j].InHand.RemoveRange(0, 1); //usuniecie karty z reki
                        }
                        else if (Grajacy.WszyscyGracze[j].OnTable.Any())
                        {
                            Grajacy.SprawdzCzyNaStoleJestSymbol(Grajacy.WszyscyGracze[j].OnTable[Grajacy.WszyscyGracze[j].OnTable.Count - 1], j, liczbaGraczy);
                        }

                        for (int i = 1; i <= liczbaGraczy; i++)
                        {
                            stanStolu = "G" + i + ":";
                            Console.WriteLine(stanStolu);
                            Grajacy.WszyscyGracze[i - 1].PokazKarty(Grajacy.WszyscyGracze[i - 1].InHand);
                        }

                        Console.ReadLine();

                        if (!(Grajacy.WszyscyGracze[j].InHand).Any() && !(Grajacy.WszyscyGracze[j].OnTable).Any())
                        {
                            isWinner = true;
                            Console.WriteLine("Zwyciężył gracz: " + (j + 1) + ". GRATULUJĘ!");
                        }
                        if (isWinner == true)
                        {
                            break;
                        }

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
                    if (Grajacy.WszyscyGracze[i].InHand.Any())
                    {

                        for (int k = 0; k < 1; k++)
                        {
                            temp = Grajacy.WszyscyGracze[i].InHand[k];
                        }

                        if (Grajacy.SprawdzCzyNaStoleJestKolor(temp, i, liczbaGraczy)) break;

                        Grajacy.WszyscyGracze[i].OnTable.Add(temp); //wyrzucenie 1 karty na stol
                        Grajacy.WszyscyGracze[i].InHand.RemoveRange(0, 1); //usuniecie karty z reki
                    }
                    else if (Grajacy.WszyscyGracze[i].OnTable.Any())
                    {
                        if (Grajacy.SprawdzCzyNaStoleJestKolor(Grajacy.WszyscyGracze[i].OnTable[Grajacy.WszyscyGracze[i].OnTable.Count - 1], i, liczbaGraczy))
                            break;
                    }
                    for (int a = 1; a <= liczbaGraczy; a++)
                    {
                        stanStolu = "G" + a + ":";
                        Console.WriteLine(stanStolu);
                        Grajacy.WszyscyGracze[a - 1].PokazKarty(Grajacy.WszyscyGracze[a - 1].InHand);
                    }
                    Console.WriteLine();
                    if (!(Grajacy.WszyscyGracze[i].InHand).Any() && !(Grajacy.WszyscyGracze[i].OnTable).Any())
                    {
                        isWinner = true;
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
                podTotemem.AddRange(Grajacy.WszyscyGracze[j].OnTable);
                Grajacy.WszyscyGracze[j].OnTable.Clear();
                if (!(Grajacy.WszyscyGracze[j].InHand).Any() && !(Grajacy.WszyscyGracze[j].OnTable).Any())
                {
                    isWinner = true;
                    Console.WriteLine("Zwyciężył gracz: " + (j + 1) + ". GRATULUJĘ!");
                }
                //mamy zwyciezce - zatrzymaj petle glowna

            }

        }


    }
}