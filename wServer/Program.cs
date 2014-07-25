#region

using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using wServer.realm;
using wServer.svrPackets;

#endregion

namespace wServer
{
    internal static class Program
    {
        private static Socket svrSkt;

        private static void HostPolicyServer()
        {
            try
            {
                var listener = new TcpListener(IPAddress.Any, 843);
                listener.Start();
                listener.BeginAcceptTcpClient(ServePolicyFile, listener);
            }
            catch
            {
            }
        }

        private static void Main(string[] args)
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            svrSkt = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            svrSkt.Bind(new IPEndPoint(IPAddress.Any, 2050));
            svrSkt.Listen(0xff);
            svrSkt.BeginAccept(Listen, null);
            Console.CancelKeyPress += (sender, e) =>
            {
                Console.WriteLine(@"Saving Please Wait...");
                svrSkt.Close();
                foreach (ClientProcessor i in RealmManager.Clients.Values.ToArray())
                {
                    try
                    {
                        //i.Player.SaveToCharacter();
                        //i.Save();
                        i.Disconnect();
                    }
                    catch
                    {
                    }
                }
                Console.WriteLine(@"Closing...");
                Thread.Sleep(500);
                Environment.Exit(0);
            };

            Console.WriteLine(@"Listening at port 2050...");

            //new Thread(AutoBroadCastNews).Start();
            //new Thread(AutoSave).Start();

            HostPolicyServer();

            RealmManager.CoreTickLoop(); //Never returns
        }

        private static void AutoSave()
        {
            foreach (ClientProcessor i in RealmManager.Clients.Values.ToArray())
            {
                i.Player.SaveToCharacter();
                i.Save();
            }
            Thread.Sleep(3000);
        }

        private static void AutoBroadCastNews()
        {
            string[] news = File.ReadAllLines(@"news.txt");
            while (true)
            {
                foreach (ClientProcessor i in RealmManager.Clients.Values.ToArray())
                {
                    i.SendPacket(new TextPacket
                    {
                        Name = "@*NEWS*",
                        Stars = -1,
                        BubbleTime = 0,
                        Recipient = "",
                        Text = news[new Random().Next(news.Length)],
                        CleanText = news[new Random().Next(news.Length)]
                    });
                }
                Thread.Sleep(3*(60*1000));
            }
        }

        private static void ServePolicyFile(IAsyncResult ar)
        {
            TcpClient cli = (ar.AsyncState as TcpListener).EndAcceptTcpClient(ar);
            (ar.AsyncState as TcpListener).BeginAcceptTcpClient(ServePolicyFile, ar.AsyncState);
            try
            {
                NetworkStream s = cli.GetStream();
                var rdr = new NReader(s);
                var wtr = new NWriter(s);
                if (rdr.ReadNullTerminatedString() == "<policy-file-request/>")
                {
                    wtr.WriteNullTerminatedString(@"<cross-domain-policy>
     <allow-access-from domain=""*"" to-ports=""*"" />
</cross-domain-policy>");
                    wtr.Write((byte) '\r');
                    wtr.Write((byte) '\n');
                }
                cli.Close();
            }
            catch
            {
            }
        }

        private static void Listen(IAsyncResult ar)
        {
            Socket skt = null;
            try
            {
                skt = svrSkt.EndAccept(ar);
            }
            catch (ObjectDisposedException)
            {
            }
            try
            {
                svrSkt.BeginAccept(Listen, null);
            }
            catch (ObjectDisposedException)
            {
            }
            if (skt != null)
            {
                var psr = new ClientProcessor(skt);
                psr.BeginProcess();
            }
        }
    }
}