using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AplikacjaSerwerowa
{
    public class Gracze
    {
        public List<Gracz> WszyscyGracze = new List<Gracz>();
        public int winner;
        string KartyGracz1 = "";
        string KartyGracz2 = "";
        public int loser;
        public string typ = "";

        public void SprawdzCzyNaStoleJestSymbol(Karta Karta1, int IDGracza, int liczbaGraczy)
        {
            for (int i = 0; i < WszyscyGracze.Count; i++)
            {
                if ((WszyscyGracze[i].OnTable).Any())
                {
                    if (IDGracza != i && Karta1.wzor == WszyscyGracze[i].OnTable[WszyscyGracze[i].OnTable.Count - 1].wzor)
                    {
                        Walka(IDGracza, i);

                    }
                    else
                    {
                        Pomylka(IDGracza, liczbaGraczy);
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

            //zwyciezca gracz siegajacy po totem
            winner = Gracz1;
            loser = Gracz2;

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

            typ = "WAL";
        }

        //pomylka skutkuje tym, ze gracz "ciagnie" wszystkie karty ze stolu
        public void Pomylka(int Gracz1, int liczbaGraczy)
        {
            if (WszyscyGracze[Gracz1].OnTable.Any())
            {
                for (int i = 0; i < liczbaGraczy; i++) { 
                    WszyscyGracze[Gracz1].InHand.AddRange(WszyscyGracze[i].OnTable);
                    WszyscyGracze[i].OnTable.Clear();
                    loser = Gracz1;
                    typ = "POM";
                }
                Console.WriteLine("Wszystkie karty ze stołu zdobyte przez: " + Gracz1);
            }
        }

    }
}
