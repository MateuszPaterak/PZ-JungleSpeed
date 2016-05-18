/*TODO w grze finalnej
 * 1. Stwórz 104 karty (z kartami wszyscy naraz, kolory, glosowanie włącznie). [na sam koniec, do testow niewygodne]
 * 2. Realizacja mini-gry papier-kamień-nożyce. [dodatkowo]
 * 3. Możliwość popełnienia błędu przy pojawieniu się karty (kolejka wtedy wraca do osoby, która się pomyliła). [końcowka]
 * 4. Możliwość bicia się 3+ osób np. po wystąpieniu karty "wszyscy naraz". [końcówka]
 * 5. Pozmieniać nazwy klas, zmiennych na "normalne" (m. in. stos kart zakrytych/odkrytych). [dodatkowo]
 * 6. Zrob cos takiego, zeby najpierw wczytywalo na "parastół", a po wcisnieciu "start gry" tworzylo graczy. [dodatkowo]
 * 7. Losuj graczy przy ustalaniu miejsca przy stole. [dodatkowo]
 * 8. Usunięcie pojedynczych zapętleń. (!!!)
 * 9. Dopisać info o tym, kto ma turę. (!!!)
 * 10.Zrzucaj stan gry do pliku txt (dziennik). [dodatkowo]
 * 11.Zaimplementuj zbieranie kart spod totemu. [końcówka]
 * 12.Zaimplementuj kartę "wszyscy naraz". [obsługa czasu, sprawdzanie pojedynków].
 * 13.Zwycięzca pojedynku 1 vs 2+ może zadecydować komu oddać większość kart. [dodatkowo]
 * 14.Zlikwidować kilka przypadkowych "zdarzeń". [końcówka]
 * 15.Blokować zdarzenia, w których jakiś gracz nie może brać udziału i karać, gdy się pomyli. (!!!)
 * */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AplikacjaSerwerowa
{
    public class LogikaGry
    {

        bool isWinner = false;
        public int zwyciezca;
        Talia TaliaDoGry = new Talia();
        Gracze Grajacy = new Gracze();
        Karta temp = new Karta();
        int liczbaGraczy;
        string stanStolu = "";
        public List<Karta> podTotemem = new List<Karta>();
        //1. normalny tryb (porownuj wzor) 
        //2. tryb po wystapieniu karty "kolor" (porownuj po kolorze)

        /*public static void SymulujGre()
        {
            LogikaGry a = new LogikaGry();
            a.Run();
        }
        */

        public LogikaGry(Pokoj glownyPokoj)
        {

        }

        public int ZwrocIdGracza(int a)
        {
            /*
            for(var i=0; i<?){
            }
            Grajacy.WszyscyGracze[i - 1]
             */
            return a;
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
            liczbaGraczy = Form1.Pokoj1.WszyscyGracze.Count();
            Console.Write(liczbaGraczy + "\n");

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
                    Grajacy.WszyscyGracze[j].InHand.Add(temp); //dodanie 1 karty z talii
                    TaliaDoGry.TaliaKart.RemoveRange(0, 1); //usuniecie karty z talii
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
                    ZwrocIdGracza(j);
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
                        Grajacy.SprawdzCzyNaStoleJestSymbol(temp, j);
                        Grajacy.WszyscyGracze[j].InHand.RemoveRange(0, 1); //usuniecie karty z reki
                    }
                    else if (Grajacy.WszyscyGracze[j].OnTable.Any())
                    {
                        Grajacy.SprawdzCzyNaStoleJestSymbol(Grajacy.WszyscyGracze[j].OnTable[Grajacy.WszyscyGracze[j].OnTable.Count - 1], j);
                    }

                    for (int i = 1; i <= liczbaGraczy; i++)
                    {
                        stanStolu = "G" + i + ":";
                        Console.WriteLine(stanStolu);
                        Grajacy.WszyscyGracze[i - 1].PokazKarty(Grajacy.WszyscyGracze[i - 1].InHand);
                    }

                    if (!(Grajacy.WszyscyGracze[j].InHand).Any() && !(Grajacy.WszyscyGracze[j].OnTable).Any())
                    {
                        isWinner = true;
                        Console.WriteLine("Zwyciężył gracz: " + (j + 1) + ". GRATULUJĘ!");
                    }
                    if (isWinner == true)
                    {
                        break;
                    }
                    //mamy zwyciezce - zatrzymaj petle glowna
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
            podTotemem.AddRange(Grajacy.WszyscyGracze[j].OnTable);
            Grajacy.WszyscyGracze[j].OnTable.Clear();
            if (!(Grajacy.WszyscyGracze[j].InHand).Any() && !(Grajacy.WszyscyGracze[j].OnTable).Any())
            {
                isWinner = true;
                Console.WriteLine("Zwyciężył gracz: " + (j + 1) + ". GRATULUJĘ!");
            }

        }

    }
}
