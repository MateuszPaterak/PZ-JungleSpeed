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
        public Socket _Socket { get; set; }
        public string _Name { get; set; }
        public int id;
        public bool isActive = false;
        public List<Karta> InHand = new List<Karta>();
        public List<Karta> OnTable = new List<Karta>();
        public Karta OstatniaKarta = new Karta();
        private int p;

        public Gracz(int id)
        {
            this.id = id;
        }
        public Gracz(Socket socket)
        {
                this._Socket = socket;
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
