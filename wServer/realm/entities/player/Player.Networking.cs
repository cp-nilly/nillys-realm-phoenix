#region

using System;
using System.Collections.Generic;

#endregion

namespace wServer.realm.entities.player
{
    partial class Player
    {
        private readonly Queue<Tuple<Packet, Predicate<Player>>> pendingPackets =
            new Queue<Tuple<Packet, Predicate<Player>>>();

        /*
        void Flush()
        {
            foreach (var i in Owner.Players.Values)
                foreach (var j in pendingPackets)
                    if (j.Item2(i))
                        i.client.SendPacket(j.Item1);
            pendingPackets.Clear();
        }
        */

        private void BroadcastSync(Packet packet) //sync at Move
        {
            BroadcastSync(packet, _ => true);
        }

        private void BroadcastSync(Packet packet, Predicate<Player> cond)
        {
            pendingPackets.Enqueue(Tuple.Create(packet, cond));
        }

        private void BroadcastSync(IEnumerable<Packet> packets)
        {
            foreach (var i in packets)
                BroadcastSync(i, _ => true);
        }

        private void BroadcastSync(IEnumerable<Packet> packets, Predicate<Player> cond)
        {
            foreach (var i in packets)
                BroadcastSync(i, cond);
        }
    }
}