using System;
using System.Collections.Generic;
using System.Linq;

/*TODO w grze finalnej
 * 1. Stwórz 104 karty (z kartami wszyscy naraz, kolory, glosowanie włącznie).
 * 2. Realizacja mini-gry papier-kamień-nożyce.
 * 3. Możliwość popełnienia błędu przy pojawieniu się karty (kolejka wtedy wraca do osoby, która się pomyliła).
 * 4. Możliwość bicia się 3+ osób np. po wystąpieniu karty "wszyscy naraz".
 * 5. Pozmieniać nazwy klas, zmiennych na "normalne" (m. in. stos kart zakrytych/odkrytych).
 * 6. Zrob cos takiego, zeby najpierw wczytywalo na "parastół", a po wcisnieciu "start gry" tworzylo graczy.
 * 7. Losuj graczy przy ustalaniu miejsca przy stole.
 * 8. Usunięcie pojedynczych zapętleń.
 * 9. Dopisać info o tym, kto ma turę.
 * 10.Zaimplementuj ładnie graficznie (!!!). Tabelka?
 * 11.Zaimplementuj zbieranie kart spod totemu.
 * 12.Zaimplementuj kartę "wszyscy naraz". [obsługa czasu, sprawdzanie pojedynków].
 * 13.Zwycięzca pojedynku 1 vs 2+ może zadecydować komu oddać większość kart.
 * 14.Zlikwidować kilka przypadkowyc "zdarzeń".
 * */

namespace JungleSpeedConsoleEdition
{

	class MainClass
	{

		bool isWinner = false;
		Talia TaliaDoGry = new Talia();
		Gracze Grajacy = new Gracze ();
		Karta temp = new Karta();
		int liczbaGraczy;
		string stanStolu = "";
		public List<Karta> podTotemem = new List<Karta>();
		//1. normalny tryb (porownuj wzor) 
		//2. tryb po wystapieniu karty "kolor" (porownuj po kolorze)

		public static void Main()
		{
			MainClass a = new MainClass ();
			a.Run ();
		}

		public void Run(){
		
			Console.WriteLine ("Talia przed zmieszaniem");
			TaliaDoGry.UtworzTalie();
			TaliaDoGry.PokazTalie();
			Console.WriteLine ("Talia po zmieszaniu");
			TaliaDoGry.ZmieszajKarty();
			TaliaDoGry.PokazTalie();

			Console.WriteLine ("Ilu ma być graczy?");
			liczbaGraczy = Convert.ToInt32(Console.ReadLine());
			Console.Write (liczbaGraczy);

			for (int i = 0; i < liczbaGraczy; i++) {
				Grajacy.DodajGracza ();
			}
				
			//1 faza rozgrywki - rozdanie kart
			while ((TaliaDoGry.TaliaKart).Any()) {
				for(int j=0; j<liczbaGraczy; j++)
					{   
						for (int i = 0; i < 1; i++) { temp = TaliaDoGry.TaliaKart[i]; }
						//Console.WriteLine("SPR1: " + temp.wzor + " " + temp.kolor);
						Grajacy.WszyscyGracze[j].InHand.Add(temp); //dodanie 1 karty z talii
						TaliaDoGry.TaliaKart.RemoveRange (0,1); //usuniecie karty z talii
						//for (int i = 0; i < 1; i++) { temp = abc.InHand[i]; }
						//Console.WriteLine("SPR2: " + temp.wzor + " " + temp.kolor);
						//Grajacy.WszyscyGracze[0].InHand.RemoveRange (0,1);
					if(!(TaliaDoGry.TaliaKart).Any()) break;
					}
			}
			for (int i = 1; i <= liczbaGraczy; i++) {
				Console.WriteLine("Gracz "+i+":");
				Grajacy.WszyscyGracze[i-1].PokazKarty(Grajacy.WszyscyGracze[i-1].InHand);
			}

			//2 faza rozgrywki - gra
			while (!isWinner) {
				
				//normalny ruch
				for(int j=0; j<liczbaGraczy; j++)
				{   
					if (Grajacy.WszyscyGracze[j].InHand.Any()){
						for (int i = 0; i < 1; i++) {
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
						Grajacy.SprawdzCzyNaStoleJestSymbol (temp, j, liczbaGraczy);
						Grajacy.WszyscyGracze[j].OnTable.Add(temp); //wyrzucenie 1 karty na stol
						Grajacy.WszyscyGracze[j].InHand.RemoveRange (0,1); //usuniecie karty z reki
					}
					else if (Grajacy.WszyscyGracze[j].OnTable.Any()){
						Grajacy.SprawdzCzyNaStoleJestSymbol(Grajacy.WszyscyGracze[j].OnTable[Grajacy.WszyscyGracze[j].OnTable.Count-1], j, liczbaGraczy);
					}

					for (int i = 1; i <= liczbaGraczy; i++) {
						stanStolu = "G"+i+":";
						Console.WriteLine (stanStolu);
						Grajacy.WszyscyGracze[i-1].PokazKarty(Grajacy.WszyscyGracze[i-1].InHand);
					}
					Console.WriteLine ();
					if (!(Grajacy.WszyscyGracze [j].InHand).Any () && !(Grajacy.WszyscyGracze [j].OnTable).Any ()) {
						isWinner = true;
						Console.WriteLine ("Zwyciężył gracz: " + (j+1) + ". GRATULUJĘ!");
					}
					if (isWinner == true) {
						break;
					}

				}
					
			}

		}
			
		public void Kolory(int graczWyrzucajacyKolor){
			int i = 1;
			for(int j=0; j<liczbaGraczy; j++)
			{   
				i = j + graczWyrzucajacyKolor;
				if (i >= liczbaGraczy)	i = 0;
				if (Grajacy.WszyscyGracze[i].InHand.Any()){
					
					for (int k = 0; k < 1; k++) {
						temp = Grajacy.WszyscyGracze[i].InHand[k];
					}

					if(Grajacy.SprawdzCzyNaStoleJestKolor (temp, i, liczbaGraczy)) break;

					Grajacy.WszyscyGracze[i].OnTable.Add(temp); //wyrzucenie 1 karty na stol
					Grajacy.WszyscyGracze[i].InHand.RemoveRange (0,1); //usuniecie karty z reki
				}
				else if (Grajacy.WszyscyGracze[i].OnTable.Any()){
					if (Grajacy.SprawdzCzyNaStoleJestKolor (Grajacy.WszyscyGracze [i].OnTable [Grajacy.WszyscyGracze [i].OnTable.Count - 1], i, liczbaGraczy))
						break;
				}
				for (int a = 1; a <= liczbaGraczy; a++) {
					stanStolu = "G"+a+":";
					Console.WriteLine (stanStolu);
					Grajacy.WszyscyGracze[a-1].PokazKarty(Grajacy.WszyscyGracze[a-1].InHand);
				}
				Console.WriteLine ();
				if (!(Grajacy.WszyscyGracze [i].InHand).Any () && !(Grajacy.WszyscyGracze [i].OnTable).Any ()) {
					isWinner = true;
					Console.WriteLine ("Zwyciężył gracz: " + (i+1) + ". GRATULUJĘ!");
				}
				if (isWinner == true) {
					break;
				}

			}
		}

		public void Glosowanie(int j){
		
			Console.WriteLine ("Ktory z graczy ma oddac karty pod totem?");
			podTotemem.AddRange (Grajacy.WszyscyGracze [j].OnTable);
			Grajacy.WszyscyGracze [j].OnTable.Clear ();
			if (!(Grajacy.WszyscyGracze [j].InHand).Any () && !(Grajacy.WszyscyGracze [j].OnTable).Any ()) {
				isWinner = true;
				Console.WriteLine ("Zwyciężył gracz: " + (j+1) + ". GRATULUJĘ!");
			}
			//mamy zwyciezce - zatrzymaj petle glowna
		
		}

	}
}