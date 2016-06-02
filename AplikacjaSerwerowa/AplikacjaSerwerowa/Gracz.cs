using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;

namespace AplikacjaSerwerowa
{
    public class Gracz
    {
        public Socket _Socket;
        public string _Name;
        public int id;
        public bool isActive = false;
        public List<Karta> InHand = new List<Karta>();
        public List<Karta> OnTable = new List<Karta>();
        public Karta OstatniaKarta = new Karta();

        public Gracz(int id)
        {
            this.id = id;
        }
        public Gracz(Socket socket)
        {
                _Socket = socket;
                _Name = "Bezimienny";
        }
        public Gracz(int id, Socket socket, string nazwa)
        {
            this.id = id;
            _Socket = socket;
            _Name = nazwa;
        }
        public Gracz(int id, Socket socket)
        {
            this.id = id;
            _Socket = socket;
            _Name = "Bezimienny";
        }
        public void PokazKarty(List<Karta> ZestawKart)
        {
            ZestawKart.ForEach(delegate(Karta abc)
            {
                Console.WriteLine(abc.wzor + " " + abc.kolor);
            });
        }

    }
}
