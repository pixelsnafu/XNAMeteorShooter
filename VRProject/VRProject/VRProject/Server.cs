using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace VRProject
{
    class Server
    {
        /// <summary>
        /// Determine if the server is running or not
        /// </summary>
        public Boolean Running { get; set; }

        public List<IPEndPoint> Clients;

        /// <summary>
        /// Delegate type for functions to be executed upon received data from a connection
        /// </summary>
        /// <param name="data">The byte array contained in the packet</param>
        public delegate void ReceiveFunction(byte[] data);

        /// <summary>
        /// The delegate function which will be executed
        /// </summary>
        public ReceiveFunction ReceiveFunc { get; set; }

        /// <summary>
        /// The UdpClient to handle the socketing
        /// </summary>
        private UdpClient Client;

        /// <summary>
        /// The port the server is listening on
        /// </summary>
        private int port;

        #region Constructors
        /// <summary>
        /// Creates a server which, when started, will listen on the specified port
        /// </summary>
        /// <param name="port">The port to listen on</param>
        public Server(int port)
        {
            Running = false;
            this.port = port;
            ReceiveFunc = null;
            Clients = new List<IPEndPoint>();
        }

        /// <summary>
        /// Creates a server which, when started, will listen on the specified port and
        /// will use the specified ReceiveFunction to handle incoming data
        /// </summary>
        /// <param name="port">The port to listen on</param>
        /// <param name="receiveFunc">Handler function to process incoming data</param>
        public Server(int port, ReceiveFunction receiveFunc)
        {
            Running = false;
            this.port = port;
            ReceiveFunc = receiveFunc;
            Clients = new List<IPEndPoint>();
        }
        #endregion

        /// <summary>
        /// Starts the server listening on the specified port
        /// </summary>
        public void Start()
        {
            Running = true;
            Client = new UdpClient(port);
            
            try
            {
                Client.BeginReceive(new AsyncCallback(receive), null);
                Log("Server is running...");
            }
            catch (Exception e)
            {
                Log(e.ToString());
            }

        }

        /// <summary>
        /// Stops the server
        /// </summary>
        public void Stop()
        {
            Running = false;
            Log("Server is shut down...");
            Client.Close();
        }

        /// <summary>
        /// Callback function which is called when a packet is received by the server.
        /// 
        /// Called the delegate ReceiveFunction to process the actual data
        /// </summary>
        /// <param name="res">The IAsyncResult</param>
        private void receive(IAsyncResult res)
        {
            try
            {
                IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, 8000);
                byte[] data = Client.EndReceive(res, ref RemoteIpEndPoint);
                if (!Clients.Contains(RemoteIpEndPoint))
                {
                    Clients.Add(RemoteIpEndPoint);
                }
                Client.BeginReceive(new AsyncCallback(receive), null);

                if (ReceiveFunc != null)
                    ReceiveFunc(data);

                //Packet packet = new Packet(data);
            }
            catch (ObjectDisposedException ex)
            {
            }
        }

        private void send(IPEndPoint client, Packet packet)
        {
            Client.BeginSend(packet.Data, packet.Data.Length, client, null, null);
        }

        public void NotifyClients()
        {
            foreach (var client in Clients)
            {
                Packet message = new Packet();
                message.AddString("Hi everyone!");
                send(client, message);
            }
        }

        /// <summary>
        /// Write to the log
        /// </summary>
        /// <param name="message"></param>
        public void Log(String message)
        {
            Console.WriteLine(message);
        }
    }
}
