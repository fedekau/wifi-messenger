using System;

using Xamarin.Forms;
using Dolphins.Salaam;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;
using System.Net.Sockets;

namespace WifiMessenger
{
	public class App : Application
	{
		private SalaamService service = new SalaamService ("wifi_msg", "WifiMessenger", 15000);
		private SalaamBrowser browser = new SalaamBrowser();
		private IPAddress ip;
		private bool first;
		//ScrollView display chat messages
		private Label result = new Label {
			XAlign = TextAlignment.Center,
			Text = "Welcome to Xamarin Forms!"
		};

		// Lista de mensajes <Observable Collection> para que cuando cambie el ItemSource la listView se actualice sola
		public ObservableCollection<Mensaje> mensajes = new ObservableCollection<Mensaje>();
		// Create the ListView.
		ListView historial = new ListView
		{
			// Source of data items.
			//ItemsSource = mensajes,

			// Define template for displaying each item.
			// (Argument of DataTemplate constructor is called for 
			//      each item; it must return a Cell derivative.)

			ItemTemplate = new DataTemplate(() =>
				{
					// Create views with bindings for displaying each property.
					//Creo dos etiquetas para que dependiendo de quien lo escriba (misma ip o no) como se muestre

					//Defino etiqueta izquierda
					Label nameLabelizq = new Label();
					nameLabelizq.SetBinding(Label.TextProperty, "MensajeIzq");
					//Defino etiqueta derecha
					Label nameLabelder = new Label();
					nameLabelder.SetBinding(Label.TextProperty, "MensajeDer");
					//Defino 
					Image img = new Image { Aspect = Aspect.AspectFit };
					img.SetBinding(Image.SourceProperty, "BoxColor");
					img.SetBinding(Image.HorizontalOptionsProperty, "ubicBox");

					//Stack msj izq
					StackLayout st1= new StackLayout();
					st1.VerticalOptions=LayoutOptions.Center;
					st1.Spacing=0;
					st1.Children.Add(nameLabelizq);
					//Stack msj 
					StackLayout st2= new StackLayout();
					st2.VerticalOptions=LayoutOptions.Center;
					st2.Spacing=0;
					st2.Children.Add(nameLabelder);
					//st1.SetBinding(Layout.HorizontalOptionsProperty, "ubicIzq");
					st1.SetBinding(Layout.HorizontalOptionsProperty, "ubicDer");

					// Return an assembled ViewCell.
					return new ViewCell
					{
						View = new StackLayout
						{
							Padding = new Thickness(0, 5),
							Orientation = StackOrientation.Horizontal,
							Children = 
							{
								st1,
								img,
								st2,

							}
							}
					};

				}),

		};





		Button BotonEnviar = new Button
		{
			Text = "Enviar!",
			Font = Font.SystemFontOfSize(NamedSize.Large),
			BorderWidth = 1,
			HorizontalOptions = LayoutOptions.Center,
			//BackgroundColor = Color.Blue,

		};
		Entry entryCell = new Entry(){
			Placeholder = "Escriba su mensaje aquí"
		} ;

		public App ()
		{

			// The root page of your application

			MainPage = new ContentPage {
				Content = new StackLayout {
					VerticalOptions = LayoutOptions.Center,
					Children = {
						historial,
						entryCell,
						BotonEnviar
					},

				}

			};
			first = true;
		}

		protected override void OnStart ()
		{
			// Handle when your app starts

			service.Registered += HandleRegistered;
			service.CreationFailed += HandleCreationFailed;
			service.BroadcastFailed += HandleBroadcastFailed;
			service.Unregistered += HandleUnregistered;
			//CARGO MI IP
			ip = LocalIPAddress ();
			service.Message = "READY_TO_CHAT";

			service.Register ();

			BotonEnviar.Clicked += (sender, e) => { 
				System.Console.WriteLine (entryCell.Text);

				service.Message=entryCell.Text;
				entryCell.Text=null;
			};

			historial.ItemSelected+= (sender, e) => { 
				historial.SelectedItem=null;
			};



		}

		protected override void OnSleep ()
		{
			// Handle when your app sleeps
		}

		public IPAddress LocalIPAddress()
		{
			IPHostEntry host;
			IPAddress localIP=null;
			host = Dns.GetHostEntry(Dns.GetHostName());
			foreach (IPAddress ip in host.AddressList)
			{
				if (ip.AddressFamily == AddressFamily.InterNetwork)
				{
					localIP = ip;
					break;
				}
			}
			return localIP;
		}

		protected override void OnResume ()
		{
			// Handle when your app resumes
		}

		private void HandleClientMessageChanged (object sender, SalaamClientEventArgs e){
			result.Text = "Se detecto cambio en mensaje de cliente " + e.Client.HostName;
			System.Console.WriteLine ("[Salaam Browser] "+ DateTime.Now +" Se detecto cambio en mensaje de cliente:");
			System.Console.WriteLine ("\t"+"Cliente: " + e.Client.HostName + ":" + e.Client.Port);
			System.Console.WriteLine ("\t"+"Nuevo mensaje: " + e.Client.Message);
			if (e.Client.Address.Equals(ip)) {
				Mensaje m = new Mensaje ( e.Client.Message,"", "@drawable/enviado", LayoutOptions.End, LayoutOptions.End, LayoutOptions.EndAndExpand);
				mensajes.Add (m);
				//UPDATE DE MENSAJES 
				historial.ItemsSource=null;
				historial.ItemsSource = mensajes;

				// VOY AL FINAL DE LA CONVERSACIÓN
				historial.ScrollTo(m, ScrollToPosition.End, true );
			} else {
				Mensaje m = new Mensaje ("", e.Client.Message, "@drawable/recibido", LayoutOptions.Start, LayoutOptions.Start, LayoutOptions.Start);
				mensajes.Add (m);
				//UPDATE DE MENSAJES 
				historial.ItemsSource=null;
				historial.ItemsSource = mensajes;

				// VOY AL FINAL DE LA CONVERSACIÓN
				historial.ScrollTo(m, ScrollToPosition.End, true );
			}


		}

		private void HandleClientDisappeared (object sender, SalaamClientEventArgs e){
			result.Text = "Un cliente ha desaparecido " + e.Client.HostName;
			System.Console.WriteLine ("[Salaam Browser] "+ DateTime.Now +" Un cliente ha desaparecido:");
			System.Console.WriteLine ("\t"+"Cliente: " + e.Client.HostName + ":" + e.Client.Port);
			System.Console.WriteLine ("\t"+"Mensaje: " + e.Client.Message);	
		}

		private void HandleClientAppeared (object sender, SalaamClientEventArgs e){
			result.Text= result.Text + "Un cliente ha aparecido " + e.Client.HostName;
			System.Console.WriteLine ("[Salaam Browser] "+ DateTime.Now +" Un cliente ha aparecido:");
			System.Console.WriteLine ("\t"+"Cliente: " + e.Client.HostName + ":" + e.Client.Port);
			System.Console.WriteLine ("\t"+"Mensaje: " + e.Client.Message);
			//Xamarin.listView no se actualiza bien en el primer item por eso tuve que hacer esto
			if (first) {
				Mensaje m = new Mensaje ("", "", "@drawable/transparente", LayoutOptions.Start, LayoutOptions.Start, LayoutOptions.Start);
				mensajes.Add (m);
				historial.ItemsSource = mensajes;
				first = false;
			}

		}

		private void HandleBrowserFailed (object sender, EventArgs e){
			result.Text = "El buscador ha fallado al buscar clientes..";
			System.Console.WriteLine ("[Salaam Browser] "+ DateTime.Now +" El buscador ha fallado al buscar clientes");
		}

		private void HandleStarted (object sender, EventArgs e){
			result.Text = "El buscador se ha iniciado correctamente..";
			System.Console.WriteLine ("[Salaam Browser] "+ DateTime.Now +" El buscador se ha iniciado correctamente");
			//			historial.ItemsSource=null;
		}

		private void HandleStartFailed (object sender, EventArgs e){
			result.Text = " El buscador ha fallado al iniciar..";
			System.Console.WriteLine ("[Salaam Browser] "+ DateTime.Now +" El buscador ha fallado al iniciar");
		}

		private void HandleStopped (object sender, EventArgs e){
			result.Text = "El buscador se ha detenidido correctamente..";
			System.Console.WriteLine ("[Salaam Browser] "+ DateTime.Now +" El buscador se ha detenidido correctamente");
		}

		public SalaamService getSalaamService(){
			return service;
		}

		public SalaamBrowser getSalaamBrowser(){
			return browser;
		}

		private void HandleRegistered(object sender, EventArgs e){
			result.Text = "Servicio iniciado correctamente..";
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
			result.Text = "Ocurrio un error al iniciar el servicio..";
			System.Console.WriteLine ("[Salaam Service] "+ DateTime.Now +" Ocurrio un error al iniciar el servicio");
		}

		private void HandleBroadcastFailed(object sender, EventArgs e){
			result.Text = "No se pudo enviar mensaje de descubrimiento..";
			System.Console.WriteLine ("[Salaam Service] "+ DateTime.Now +" No se pudo enviar mensaje de descubrimiento");
		}

		private void HandleUnregistered(object sender, EventArgs e){
			result.Text = "Servicio terminado correctamente..";
			System.Console.WriteLine ("[Salaam Service] "+ DateTime.Now +" Servicio terminado correctamente");
		}


	}
}

