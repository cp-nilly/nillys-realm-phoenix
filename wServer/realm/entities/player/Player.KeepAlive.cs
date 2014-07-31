#region

using System.Collections.Generic;
using wServer.cliPackets;
using wServer.svrPackets;

#endregion

namespace wServer.realm.entities.player
{
    public partial class Player
    {
        private readonly Queue<long> ts = new Queue<long>();
        private long lastPong = -1;
        private int? lastTime;

        private bool sentPing;
        private long tickMapping;

        private bool KeepAlive(RealmTime time)
        {
            if (lastPong == -1) lastPong = time.tickTimes - 1500;
            if (time.tickTimes - lastPong > 1500 && !sentPing)
            {
                sentPing = true;
                ts.Enqueue(time.tickTimes);
                psr.SendPacket(new PingPacket());
            }
            else if (time.tickTimes - lastPong > 3000)
            {
                return false;
            }
            return true;
        }

        public void Pong(PongPacket pkt)
        {
            if (lastTime != null && (pkt.Time - lastTime.Value > 3000 || pkt.Time - lastTime.Value < 0))
#pragma warning disable 642
                ; //psr.Disconnect();
#pragma warning restore 642
            else
                lastTime = pkt.Time;
            tickMapping = ts.Dequeue() - pkt.Time;
            lastPong = pkt.Time + tickMapping;
            sentPing = false;
        }
    }
}