using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AplikacjaSerwerowa
{
    public class Talia
    {
        public List<Karta> TaliaKart = new List<Karta>();
        private static Random rng = new Random();

        public void UtworzTalie()
        {
            //100 kart 20x5
            for (int i = 1; i <=3; i++)
            {
                TaliaKart.Add(new Karta() { idKarty = 1 + ((i - 1) * 5), wzor = i, kolor = "czarny" });
                TaliaKart.Add(new Karta() { idKarty = 2 + ((i - 1) * 5), wzor = i, kolor = "biały" });
                TaliaKart.Add(new Karta() { idKarty = 3 + ((i - 1) * 5), wzor = i, kolor = "czerwony" });
                TaliaKart.Add(new Karta() { idKarty = 4 + ((i - 1) * 5), wzor = i, kolor = "zielony" });
                TaliaKart.Add(new Karta() { idKarty = 5 + ((i - 1) * 5), wzor = i, kolor = "niebieski" });
            }

            //8 kart specjalnych
            //kolor 2x
            //TaliaKart.Add(new Karta() { idKarty = 101, wzor = 21, kolor = "specjalny" });
            //TaliaKart.Add(new Karta() { idKarty = 102, wzor = 21, kolor = "specjalny" });
            //wszyscy naraz 3x
            //TaliaKart.Add(new Karta() { idKarty = 103, wzor = 22, kolor = "specjalny" });
            //TaliaKart.Add(new Karta() { idKarty = 104, wzor = 22, kolor = "specjalny" });
            //TaliaKart.Add(new Karta() { idKarty = 105, wzor = 22, kolor = "specjalny" });
            //glosowanie 3x
            //TaliaKart.Add(new Karta() { idKarty = 106, wzor = 23, kolor = "specjalny" });
            //TaliaKart.Add(new Karta() { idKarty = 107, wzor = 23, kolor = "specjalny" });
            //TaliaKart.Add(new Karta() { idKarty = 108, wzor = 23, kolor = "specjalny" });
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
}
