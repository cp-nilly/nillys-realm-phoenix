using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using log4net;
using common;

namespace wServer.networking
{
    class PolicyServer
    {
        static ILog log = LogManager.GetLogger(typeof(PolicyServer));

        TcpListener listener;
        public PolicyServer()
        {
            listener = new TcpListener(IPAddress.Any, 843);
        }

        static void ServePolicyFile(IAsyncResult ar)
        {
            TcpClient cli = (ar.AsyncState as TcpListener).EndAcceptTcpClient(ar);
            (ar.AsyncState as TcpListener).BeginAcceptTcpClient(ServePolicyFile, ar.AsyncState);
            try
            {
                var s = cli.GetStream();
                NReader rdr = new NReader(s);
                NWriter wtr = new NWriter(s);
                if (rdr.ReadNullTerminatedString() == "<policy-file-request/>")
                {
                    wtr.WriteNullTerminatedString(@"<cross-domain-policy>
     <allow-access-from domain=""*"" to-ports=""*"" />
</cross-domain-policy>");
                    wtr.Write((byte)'\r');
                    wtr.Write((byte)'\n');
                }
                cli.Close();
            }
            catch { }
        }

        bool started = false;
        public void Start()
        {
            log.Info("Starting policy server...");
            try
            {
                listener.Start();
                listener.BeginAcceptTcpClient(ServePolicyFile, listener);
                started = true;
            }
            catch
            {
                log.Warn("Could not start Socket Policy Server, is port 843 occupied?");
                started = false;
            }
        }

        public void Stop()
        {
            if (started)
            {
                log.Warn("Stopping policy server...");
                listener.Stop();
            }
        }
    }
}
