using System;
using System.Collections.Generic;
namespace JungleSpeedConsoleEdition
{
	public class Gracz
	{
		//public string nickname = "";
		public bool isActive =  false;
		public List<Karta> InHand = new List<Karta>();
		public List<Karta> OnTable = new List<Karta>();
		public Karta OstatniaKarta = new Karta();

		public void PokazKarty(List<Karta>ZestawKart){

			ZestawKart.ForEach(delegate(Karta abc)
				{
					Console.WriteLine(abc.wzor + " " + abc.kolor);
				});
		}
				
	}
}