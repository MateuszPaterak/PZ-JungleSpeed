using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AplikacjaSerwerowa
{
    public class Pokoj
    {
        public int id;
        public string nazwa;

        public Pokoj(int id, string nazwa)
        {
            this.id = id;
            this.nazwa = nazwa;            
        }
    }
}
