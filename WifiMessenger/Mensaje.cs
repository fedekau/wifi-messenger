using System;
using Xamarin.Forms;
namespace WifiMessenger
{
	public class Mensaje
	{
		public Mensaje(string msjIzq, string msjDer,String boxColor, LayoutOptions ubic, LayoutOptions u1, LayoutOptions u2)
		{
			this.MensajeIzq = msjIzq;
			this.MensajeDer= msjDer;
			this.BoxColor = boxColor;
			this.ubicBox = ubic;
			this.ubicIzq = u1;
			this.ubicDer = u2;
		}

		public string MensajeIzq { private set; get; }
		public string MensajeDer { private set; get; }
		public LayoutOptions ubicBox { private set; get; }
		public LayoutOptions ubicIzq { private set; get; }
		public LayoutOptions ubicDer { private set; get; }
		public String BoxColor { private set; get; }
	};
}

