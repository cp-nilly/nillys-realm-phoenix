using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using wServer.realm;
using common;
using wServer.networking;
using log4net;

namespace wServer
{
    class Server
    {
        static ILog log = LogManager.GetLogger(typeof(Server));

        public Socket Socket { get; private set; }
        public Server(int port)
        {
            Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        public void Start()
        {
            log.Info("Starting server...");
            Socket.Bind(new IPEndPoint(IPAddress.Any, 2050));
            Socket.Listen(0xff);
            Socket.BeginAccept(Listen, null);
        }

        void Listen(IAsyncResult ar)
        {
            if (!Socket.IsBound) return;
            var cliSkt = Socket.EndAccept(ar);
            Socket.BeginAccept(Listen, null);
            if (cliSkt != null)
            {
                var client = new ClientProcessor(cliSkt);
                client.BeginProcess();
            }
        }

        public void Stop()
        {
            log.Info("Stoping server...");
            foreach (var i in RealmManager.Clients.Values.ToArray())
                i.Disconnect();
            Socket.Close();
        }
    }
}
