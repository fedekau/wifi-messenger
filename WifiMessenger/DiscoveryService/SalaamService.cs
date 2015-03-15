using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Timers;

namespace Dolphins.Salaam
{
    /// <summary>
    /// This class is used to register SalaamService. 
    /// </summary>
    /// <remarks>SalaamService registers a unique service that SalaamBrowser clients will find it immediately.</remarks>
    public class SalaamService : IDisposable
    {
        /// <summary>
        /// The default SalaamService port.
        /// </summary>
        private const int port = 54183;

        private const int defaultFrequency = 45;

        private readonly string hostname;

        private readonly Timer timer;

        private IPEndPoint ipEndPoint;

        private UdpClient udpClient;

        private double frequency;

        /// <summary>
        /// Initializes a new instance of the <see cref="SalaamService"/> class.
        /// </summary>
        /// <param name="serviceType">Type of the service.</param>
        /// <param name="name">The name of the application.</param>
        /// <param name="port">The port at which the mail application is listening.</param>
        public SalaamService(string serviceType, string name, int port)
        {
            if (serviceType.Contains(";"))
            {
                throw new ArgumentException("Semicolon character is not allowed in ServiceType argument.");
            }

            if (name.Contains(";"))
            {
                throw new ArgumentException("Semicolon character is not allowed in Name argument.");
            }

            ServiceType = serviceType;

            Name = name;

            Port = port;

            try
            {
                udpClient = new UdpClient {EnableBroadcast = true, ExclusiveAddressUse = false};

                BroadcastAddress = IPAddress.Broadcast;

                timer = new Timer
                            {
                                AutoReset = true
                            };


                Frequency = defaultFrequency;

                timer.Elapsed += OnTimerElapsed;

                timer.Start();

                timer.Enabled = false;

				IPHostEntry host;
				string localIP = "?";
				host = Dns.GetHostEntry(Dns.GetHostName());
				foreach (IPAddress ip in host.AddressList)
				{
					if ((ip.AddressFamily == AddressFamily.InterNetwork)&&(!ip.ToString ().Equals ("127.0.0.1")))
					{
						localIP = ip.ToString();
					}
				}

				hostname = localIP;

                //hostname = Dns.GetHostName();

                Message = "";
            }
            catch
            {
                if (CreationFailed != null)
                {
                    CreationFailed(this, new EventArgs());
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="SalaamService"/> is enabled.
        /// </summary>
        /// <value><c>true</c> if enabled; otherwise, <c>false</c>.</value>
        public bool Enabled
        {
            get { return timer.Enabled; }
            set { timer.Enabled = value; }
        }

        /// <summary>
        /// Gets or sets the frequency in minute.
        /// </summary>
        /// <value>The frequency.</value>
        public double Frequency
        {
            get
            {
                return frequency;
            } 
            set
            {
                frequency = value;
            
                timer.Interval = (1000*60)/frequency;
            }
        }

        /// <summary>
        /// Gets or sets the broadcast address.
        /// </summary>
        /// <value>The broadcast address.</value>
        /// <remarks>The default IPAddress is 255.255.255.255 which broadcasts to all the networks.</remarks>
        public IPAddress BroadcastAddress { get; set; }

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        /// <value>The message.</value>
        /// <remarks>This is a custom message for your application.</remarks>
        public string Message { get; set; }

        /// <summary>
        /// Gets the application name.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the type of the service.
        /// </summary>
        /// <remarks>The service type is a custom string meant to be used to identify the SalaamService. The universal form of this string is preferred to be <c>_applicationProtocolName</c>.<c>_networkProtocolName</c>. e.g. _teleporter._tcp </remarks>
        public string ServiceType { get; private set; }

        /// <summary>
        /// Gets the application port.
        /// </summary>
        public int Port { get; private set; }

        /// <summary>
        /// Occurs when the SalaamService fails to create the service.
        /// </summary>
        public event EventHandler CreationFailed;

        /// <summary>
        /// Occurs when the SalaamService fails to broadcast on the network.
        /// </summary>
        public event EventHandler BroadcastFailed;

        /// <summary>
        /// Occurs when the service is successfully registered.
        /// </summary>
        public event EventHandler Registered;

        /// <summary>
        /// Occurs when the service is successfully unregistered.
        /// </summary>
        public event EventHandler Unregistered;

        private void OnTimerElapsed(object sender, ElapsedEventArgs e)
        {
            string broadcastMessage = string.Format("{0};{1};{2};{3};{4};", hostname, ServiceType, Name, Port, Message);

            broadcastMessage = broadcastMessage.Length + ";" + broadcastMessage;

            broadcastMessage = "Salaam:" + Convert.ToBase64String(Encoding.UTF8.GetBytes(broadcastMessage));

            byte[] messageBytes = Encoding.UTF8.GetBytes(broadcastMessage);

            try
            {
                udpClient.Send(messageBytes, messageBytes.Length, ipEndPoint);
            }
            catch
            {
                if (BroadcastFailed != null)
                {
                    BroadcastFailed(this, new EventArgs());
                }
            }
        }

        ~SalaamService()
        {
            Dispose();
        }

        /// <summary>
        /// Registers the SalaamService.
        /// </summary>
        public void Register()
        {
            ipEndPoint = new IPEndPoint(BroadcastAddress, port);

            timer.Enabled = true;

            OnTimerElapsed(null, null);

            if (Registered != null)
            {
                Registered(this, new EventArgs());
            }
        }

        /// <summary>
        /// Unregisters the SalaamService.
        /// </summary>
        public void Unregister()
        {
            timer.Enabled = false;

            if (Unregistered != null)
            {
                Unregistered(this, new EventArgs());
            }

            SendDisappearanceMessage();
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            SendDisappearanceMessage();

            try
            {
                timer.Enabled = false;
            }
            catch
            {
            }

            try
            {
                timer.Dispose();
            }
            catch
            {
            }

            try
            {
                udpClient.Client.Shutdown(SocketShutdown.Both);
            }
            catch
            {
            }

            try
            {
                udpClient.Close();
            }
            catch
            {
            }

            try
            {
                udpClient = null;
            }
            catch
            {
            }
        }

        /// <summary>
        /// Sends the disappearance message.
        /// </summary>
        private void SendDisappearanceMessage()
        {
            try
            {
                string broadcastMessage = string.Format("{0};{1};{2};{3};{4};<EOS>", hostname, ServiceType, Name, Port, Message);

                broadcastMessage = broadcastMessage.Length + ";" + broadcastMessage;

                broadcastMessage = "Salaam:" + Convert.ToBase64String(Encoding.UTF8.GetBytes(broadcastMessage));

                byte[] messageBytes = Encoding.UTF8.GetBytes(broadcastMessage);

                try
                {
                    udpClient.Send(messageBytes, messageBytes.Length, ipEndPoint);
                }
                catch
                {
                }
            }
            catch
            {
            }
        }
    }
}