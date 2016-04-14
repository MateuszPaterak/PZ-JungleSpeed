using System;
using System.Collections.Generic;
using System.Linq;
namespace JungleSpeedConsoleEdition
{
	public class Gracze
	{
		public List<Gracz> WszyscyGracze = new List<Gracz>();
		int winner;

		public void DodajGracza(){
			WszyscyGracze.Add (new Gracz (){ });
		}

		public void SprawdzCzyNaStoleJestSymbol(Karta Karta1, int IDGracza, int liczbaGraczy){
			for (int i=0; i<liczbaGraczy; i++) {
				if ((WszyscyGracze [i].OnTable).Any ()) {
					if (IDGracza != i && Karta1.wzor == WszyscyGracze [i].OnTable [WszyscyGracze [i].OnTable.Count - 1].wzor) {
						Walka (IDGracza, i);

					}
				}
			}

		}

		public bool SprawdzCzyNaStoleJestKolor(Karta Karta1, int IDGracza, int liczbaGraczy){
			bool colorMatch = false;
			for (int i=0; i<liczbaGraczy; i++) {
				if ((WszyscyGracze [i].OnTable).Any ()) {
					if (IDGracza != i && Karta1.kolor == WszyscyGracze [i].OnTable [WszyscyGracze [i].OnTable.Count - 1].kolor) {
						Walka (IDGracza, i);
						colorMatch = true;
					}
				}


			}
			if (colorMatch)
				return true;
			else
				return false;
		}

		public void Walka(int Gracz1, int Gracz2){
			Console.WriteLine ("JEST WALKA! Kto ją wygrywa? Wpisz gracz "+(Gracz1+1)+ " lub "+(Gracz2+1));
			winner = Convert.ToInt32(Console.ReadLine());


			if (winner == (Gracz1+1)) {
				WszyscyGracze [Gracz2].InHand.AddRange (WszyscyGracze [Gracz1].OnTable);
				WszyscyGracze [Gracz2].InHand.AddRange (WszyscyGracze [Gracz2].OnTable);
				WszyscyGracze [Gracz2].OnTable.Clear ();
				WszyscyGracze [Gracz1].OnTable.Clear ();
			}
			if (winner == (Gracz2+1)) {
				WszyscyGracze [Gracz1].InHand.AddRange (WszyscyGracze [Gracz1].OnTable);
				WszyscyGracze [Gracz1].InHand.AddRange (WszyscyGracze [Gracz2].OnTable);
				WszyscyGracze [Gracz2].OnTable.Clear ();
				WszyscyGracze [Gracz1].OnTable.Clear ();			
			}
		}

		}


}
