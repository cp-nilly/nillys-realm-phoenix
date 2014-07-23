#region

using System;
using System.Collections.Concurrent;
using System.Threading;
using log4net;

#endregion

namespace wServer.realm
{
    public class NetworkTicker //Sync network processing
    {
        private static readonly ConcurrentQueue<Tuple<ClientProcessor, Packet>> pendings =
            new ConcurrentQueue<Tuple<ClientProcessor, Packet>>();

        private readonly AutoResetEvent handle = new AutoResetEvent(false);

        private ILog log = LogManager.GetLogger(typeof (NetworkTicker));

        //public void AddPendingAction(ClientProcessor client, Action<RealmTime> callback)
        public void AddPendingPacket(ClientProcessor client, Packet packet)
        {
            pendings.Enqueue(new Tuple<ClientProcessor, Packet>(client, packet));
            //Action<RealmTime>>(client, callback));
            handle.Set();
        }


        public void TickLoop()
        {
            do
            {
                foreach (var i in RealmManager.Clients)
                    if (i.Value.Stage == ProtocalStage.Disconnected)
                    {
                        ClientProcessor psr;
                        RealmManager.Clients.TryRemove(i.Key, out psr);
                    }

                handle.WaitOne();

                Tuple<ClientProcessor, Packet> work; //Action<RealmTime>> work;
                while (pendings.TryDequeue(out work))
                {
                    if (work.Item1.Stage == ProtocalStage.Disconnected)
                    {
                        ClientProcessor psr;
                        try
                        {
                            RealmManager.Clients.TryRemove(work.Item1.Account.AccountId, out psr);
                        }
                        catch
                        {
                        }
                        continue;
                    }
                    try
                    {
                        work.Item1.ProcessPacket(work.Item2);
                        //work.Item2(LogicTicker.CurrentTime);
                    }
                    catch
                    {
                    }
                }
            } while (true);
        }
    }
}