#region

using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using log4net;

#endregion

namespace wServer.realm
{
    public class NetworkTicker //Sync network processing
    {
        private static readonly ConcurrentQueue<Tuple<ClientProcessor, Packet>> Pendings =
            new ConcurrentQueue<Tuple<ClientProcessor, Packet>>();

        private readonly AutoResetEvent _handle = new AutoResetEvent(false);

        private ILog log = LogManager.GetLogger(typeof (NetworkTicker));

        //public void AddPendingAction(ClientProcessor client, Action<RealmTime> callback)
        public void AddPendingPacket(ClientProcessor client, Packet packet)
        {
            Pendings.Enqueue(new Tuple<ClientProcessor, Packet>(client, packet));
            //Action<RealmTime>>(client, callback));
            _handle.Set();
        }

        public void TickLoop()
        {
            do
            {
                _handle.WaitOne();

                foreach (var i in RealmManager.Clients.Where(
                    i => i.Value.Stage == ProtocalStage.Disconnected))
                {
                    ClientProcessor dummyPsr;
                    RealmManager.Clients.TryRemove(i.Key, out dummyPsr);
                }

                Tuple<ClientProcessor, Packet> work; //Action<RealmTime>> work;
                while (Pendings.TryDequeue(out work))
                {
                    try
                    {
                        work.Item1.ProcessPacket((work.Item2));
                    }
                    catch
                    {
                    }
                }
            } while (true);
        }
    }
}