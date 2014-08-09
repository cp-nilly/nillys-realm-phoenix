using System;
using System.Globalization;
using System.IO;
using System.Net;
using System.Threading;
using common;
using log4net;
using log4net.Config;

namespace server
{
    internal class Program
    {
        private static readonly ILog Log = LogManager.GetLogger("Server");
        internal static SimpleSettings Settings;
        private static HttpListener _listener;

        private static void Main(string[] args)
        {
            XmlConfigurator.ConfigureAndWatch(new FileInfo("log4net.config"));

            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            Thread.CurrentThread.Name = "Entry";

            Settings = new SimpleSettings("server");

            Database.Init(Settings.GetValue<string>("db_host", "nillysrealm.com"),
                          Settings.GetValue<string>("db_port", "3306"),
                          Settings.GetValue<string>("db_name", "rotmg"),
                          Settings.GetValue<string>("db_user", ""),
                          Settings.GetValue<string>("db_pass", "botmaker"));

            var port = Settings.GetValue<int>("port", "8888");

            _listener = new HttpListener();
            _listener.Prefixes.Add("http://*:" + port + "/");
            _listener.Start();

            _listener.BeginGetContext(ListenerCallback, null);
            Log.Info("Listening at port " + port + "...");

            Console.CancelKeyPress += delegate
            {
                Log.Info("Terminating...");
                _listener.Stop();
                Settings.Dispose();
                Environment.Exit(0);
            };

            while (true)
            {
                Thread.Sleep(500);
            }
        }

        private static void ListenerCallback(IAsyncResult ar)
        {
            if (!_listener.IsListening) return;
            var context = _listener.EndGetContext(ar);
            _listener.BeginGetContext(ListenerCallback, null);
            ProcessRequest(context);
        }

        private static void ProcessRequest(HttpListenerContext context)
        {
            try
            {
                Log.InfoFormat("Dispatching request '{0}'@{1}",
                    context.Request.Url.LocalPath, context.Request.RemoteEndPoint);
                RequestHandler handler;

                if (!RequestHandlers.Handlers.TryGetValue(context.Request.Url.LocalPath, out handler))
                {
                    context.Response.StatusCode = 400;
                    context.Response.StatusDescription = "Bad request";
                    using (var wtr = new StreamWriter(context.Response.OutputStream))
                        wtr.Write("<h1>Bad request</h1>");
                }
                else
                    handler.HandleRequest(context);
            }
            catch (Exception e)
            {
                using (var wtr = new StreamWriter(context.Response.OutputStream))
                    wtr.Write("<Error>Internal Server Error</Error>");
                Log.Error("Error when dispatching request", e);
            }

            context.Response.Close();
        }
    }
}