#region

using System.Collections.Generic;
using wServer.cliPackets;
using wServer.svrPackets;

#endregion

namespace wServer.realm.entities.player
{
    public partial class Player
    {
        private const int PING_PERIOD = 1000;
        private const int DC_THRESOLD = 15000;

        private int updateLastSeen;

        public WorldTimer PongDCTimer { get; private set; }

        private bool KeepAlive(RealmTime time)
        {
            return true;
        }

        internal void Pong(int time, PongPacket pkt)
        {
            updateLastSeen++;

            if (Owner.Timers.Contains(PongDCTimer))
                Owner.Timers.Remove(PongDCTimer);

            Owner.Timers.Add(PongDCTimer = new WorldTimer(DC_THRESOLD, (w, t) =>
            {
                SendError("Lost connection to server.");
                Client.Disconnect();
            }));
        }
    }
}