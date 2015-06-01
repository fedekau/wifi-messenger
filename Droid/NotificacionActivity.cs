using Android.OS;
using Android.App;
namespace WifiMessenger.Droid
{	[Activity(Label = "Notification Activity", MainLauncher = true
)]		

	public class NotificationActivity :  global::Xamarin.Forms.Platform.Android.FormsApplicationActivity
	{
		protected override void OnCreate(Bundle bundle)
		{
			base.OnCreate(bundle);

			Xamarin.Forms.Forms.Init(this, bundle);

			LoadApplication (new App ());
		}
	}
}

