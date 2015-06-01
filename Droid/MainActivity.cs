using System;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Support.V4.App;
using Android.Graphics;
using Android.Media;


namespace WifiMessenger.Droid
{
	[Activity (Label = "Wifi Messenger", Icon = "@drawable/monkey", LaunchMode = Android.Content.PM.LaunchMode.SingleInstance, MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
	public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsApplicationActivity
	{	
		private static readonly int ButtonClickNotificationId = 1000;
		public static MainActivity activity;
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			global::Xamarin.Forms.Forms.Init (this, bundle);

			LoadApplication (new App ());
			activity = this;

		}

		public void CrearNotificacion(string mensaje)
		{	

			Intent resultIntent = new Intent(this, typeof(MainActivity));
			Android.Support.V4.App.TaskStackBuilder stackBuilder = Android.Support.V4.App.TaskStackBuilder.Create(this);
			stackBuilder.AddParentStack(this);
			stackBuilder.AddNextIntent(resultIntent);
			PendingIntent pendingIntent = PendingIntent.GetActivity(this, 0, resultIntent, PendingIntentFlags.CancelCurrent);
			// Build the notification
			NotificationCompat.Builder builder = new NotificationCompat.Builder (this)
				.SetAutoCancel (true) // dismiss the notification from the notification area when the user clicks on it
				.SetContentIntent (pendingIntent) // start up this activity when the user clicks the intent.
				.SetContentTitle ("Nuevo Mensaje") // Set the title
				.SetSmallIcon (Resource.Drawable.monkey) // This is the icon to display
				.SetContentText (String.Format (mensaje)) // the message to display.
				.SetPriority (NotificationCompat.PriorityHigh)
				.SetSound (RingtoneManager.GetDefaultUri (RingtoneType.Notification));
			// Finally publish the notification
			NotificationManager notificationManager = (NotificationManager)GetSystemService(Context.NotificationService);
			notificationManager.Notify(ButtonClickNotificationId, builder.Build());

		}
	}
}

