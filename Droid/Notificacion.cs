
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using WifiMessenger.Droid;
using Xamarin.Forms;
using Android.Support.V4.App;


[assembly: Xamarin.Forms.Dependency (typeof (Notificacion))]
namespace WifiMessenger.Droid
{
	public class Notificacion : Java.Lang.Object, INotificacion
	{
		#region INotificacion implementation

		public void Notificar (string mensaje)
		{
			MainActivity.activity.CrearNotificacion(mensaje);
		}	
		#endregion

		public Notificacion ()
		{
		}
	}
}