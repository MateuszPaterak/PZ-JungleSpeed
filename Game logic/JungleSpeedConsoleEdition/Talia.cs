using System;
using System.Collections.Generic;
namespace JungleSpeedConsoleEdition
{
	public class Talia
	{
		public List<Karta>TaliaKart = new List<Karta>();
		private static Random rng = new Random(); 

		public void UtworzTalie(){
			TaliaKart.Add (new Karta () { wzor = 1, kolor = "czarny" });
			TaliaKart.Add (new Karta () { wzor = 1, kolor = "zielony" });
			TaliaKart.Add (new Karta () { wzor = 1, kolor = "niebieski" });
			TaliaKart.Add (new Karta () { wzor = 1, kolor = "czerwony" });
			TaliaKart.Add (new Karta () { wzor = 2, kolor = "czarny" });
			TaliaKart.Add (new Karta () { wzor = 2, kolor = "zielony" });
			TaliaKart.Add (new Karta () { wzor = 2, kolor = "niebieski" });
			TaliaKart.Add (new Karta () { wzor = 2, kolor = "czerwony" });
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

		public void PokazTalie(){
			TaliaKart.ForEach(delegate(Karta abc)
				{
					Console.WriteLine(abc.wzor + " " + abc.kolor);
				});
		}

		public void ZmieszajKarty(){

				int n = TaliaKart.Count;  
				while (n > 1) {  
					n--;  
					int k = rng.Next(n + 1);  
					Karta value = TaliaKart[k];  
					TaliaKart[k] = TaliaKart[n];  
					TaliaKart[n] = value;  
				}  
			}

	}
}