#region

using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using common;
using log4net;
using log4net.Config;
using wServer.networking;
using wServer.realm;
using wServer.svrPackets;

#endregion

namespace wServer
{
    internal static class Program
    {
        internal static SimpleSettings Settings;

        static ILog log = LogManager.GetLogger("Server");

        private static void Main(string[] args)
        {
            XmlConfigurator.ConfigureAndWatch(new FileInfo("log4net.config"));

            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            Thread.CurrentThread.Name = "Entry";

            Settings = new SimpleSettings("wServer");

            var manager = new RealmManager();

            manager.Initialize();
            manager.Run();

            var server = new Server(Settings.GetValue<int>("port", "2050"));
            var policy = new PolicyServer();

            policy.Start();
            server.Start();
            log.Info("Server initialized.");

            Console.CancelKeyPress += delegate
            {
                log.Info("Terminating...");
                server.Stop();
                policy.Stop();
                Settings.Dispose();
                log.Info("Server terminated.");
                Thread.Sleep(500);
                Environment.Exit(0);
            };

            while (true)
            {
                Thread.Sleep(500);
            }
        }
    }
}