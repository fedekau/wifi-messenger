using System;

using Xamarin.Forms;
using Dolphins.Salaam;
using System.Collections.ObjectModel;

namespace WifiMessenger
{
	public class App : Application
	{
		private SalaamService service = new SalaamService ("wifi_msg", "WifiMessenger", 15000);
		private SalaamBrowser browser = new SalaamBrowser();

		public App ()
		{
			// The root page of your application

			MainPage = new ContentPage {
				Content = new StackLayout {
					VerticalOptions = LayoutOptions.Center,
					Children = {
						new Label {
							XAlign = TextAlignment.Center,
							Text = "Welcome to Xamarin Forms!"
						},
					}

				}
			};
		}

		protected override void OnStart ()
		{
			// Handle when your app starts


			service.Registered += HandleRegistered;
			service.CreationFailed += HandleCreationFailed;
			service.BroadcastFailed += HandleBroadcastFailed;
			service.Unregistered += HandleUnregistered;

			service.Message = "READY_TO_CHAT";

			service.Register ();

		}

		protected override void OnSleep ()
		{
			// Handle when your app sleeps
		}

		protected override void OnResume ()
		{
			// Handle when your app resumes
		}

		private void HandleClientMessageChanged (object sender, SalaamClientEventArgs e){
			System.Console.WriteLine ("[Salaam Browser] "+ DateTime.Now +" Se detecto cambio en mensaje de cliente:");
			System.Console.WriteLine ("\t"+"Cliente: " + e.Client.HostName + ":" + e.Client.Port);
			System.Console.WriteLine ("\t"+"Nuevo mensaje: " + e.Client.Message);	
		}

		private void HandleClientDisappeared (object sender, SalaamClientEventArgs e){
			System.Console.WriteLine ("[Salaam Browser] "+ DateTime.Now +" Un cliente ha desaparecido:");
			System.Console.WriteLine ("\t"+"Cliente: " + e.Client.HostName + ":" + e.Client.Port);
			System.Console.WriteLine ("\t"+"Mensaje: " + e.Client.Message);	
		}

		private void HandleClientAppeared (object sender, SalaamClientEventArgs e){
			System.Console.WriteLine ("[Salaam Browser] "+ DateTime.Now +" Un cliente ha aparecido:");
			System.Console.WriteLine ("\t"+"Cliente: " + e.Client.HostName + ":" + e.Client.Port);
			System.Console.WriteLine ("\t"+"Mensaje: " + e.Client.Message);	
		}

		private void HandleBrowserFailed (object sender, EventArgs e){
			System.Console.WriteLine ("[Salaam Browser] "+ DateTime.Now +" El buscador ha fallado al buscar clientes");
		}

		private void HandleStarted (object sender, EventArgs e){
			System.Console.WriteLine ("[Salaam Browser] "+ DateTime.Now +" El buscador se ha iniciado correctamente");
		}

		private void HandleStartFailed (object sender, EventArgs e){
			System.Console.WriteLine ("[Salaam Browser] "+ DateTime.Now +" El buscador ha fallado al iniciar");
		}

		private void HandleStopped (object sender, EventArgs e){
			System.Console.WriteLine ("[Salaam Browser] "+ DateTime.Now +" El buscador se ha detenidido correctamente");
		}

		public SalaamService getSalaamService(){
			return service;
		}

		public SalaamBrowser getSalaamBrowser(){
			return browser;
		}

		private void HandleRegistered(object sender, EventArgs e){
			System.Console.WriteLine ("[Salaam Service] "+ DateTime.Now +" Servicio iniciado correctamente");

			browser.Started += HandleStarted;
			browser.StartFailed += HandleStartFailed;
			browser.Stopped += HandleStopped;
			browser.BrowserFailed += HandleBrowserFailed;
			browser.ClientAppeared += HandleClientAppeared;
			browser.ClientDisappeared+=HandleClientDisappeared;
			browser.ClientMessageChanged += HandleClientMessageChanged;

			//Esto en un futuro deberia ser false, se usa en true para "ver mas dispositivos"
			//Ya que soy pobre y tengo solo un cel como la gente, y uno viejo que tenia tirado ahi
			browser.ReceiveFromLocalMachine = true;

			browser.Start ("wifi_msg");
		}
			
		private void HandleCreationFailed(object sender, EventArgs e){
			System.Console.WriteLine ("[Salaam Service] "+ DateTime.Now +" Ocurrio un error al iniciar el servicio");
		}

		private void HandleBroadcastFailed(object sender, EventArgs e){
			System.Console.WriteLine ("[Salaam Service] "+ DateTime.Now +" No se pudo enviar mensaje de descubrimiento");
		}

		private void HandleUnregistered(object sender, EventArgs e){
			System.Console.WriteLine ("[Salaam Service] "+ DateTime.Now +" Servicio terminado correctamente");
		}
	}
}

