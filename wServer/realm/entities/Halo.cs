#region

using System.Collections.Generic;
using wServer.logic;
using wServer.realm.entities.player;
using wServer.svrPackets;

#endregion

namespace wServer.realm.entities
{
    internal class Halo : StaticObject
    {
        private readonly int amount;
        private readonly float radius;
        private int lifetime;

        private int p;
        private int p2;
        private Player player;
        private int t;

        public Halo(Player player, float radius, int amount, int lifetime)
            : base(0x0711, lifetime, true, true, false)
        {
            this.player = player;
            this.radius = radius;
            this.amount = amount;
            this.lifetime = lifetime;
        }

        public override void Tick(RealmTime time)
        {
            if (t/500 == p2)
            {
                Owner.BroadcastPacket(new ShowEffectPacket
                {
                    EffectType = EffectType.Trap,
                    Color = new ARGB(0xffd700),
                    TargetId = Id,
                    PosA = new Position {X = radius}
                }, null);
                p2++;
                //Stuff
            }
            if (t/2000 == p)
            {
                var pkts = new List<Packet>();
                BehaviorBase.AOE(Owner, this, radius, true,
                    player => { Player.ActivateHealHp(player as Player, amount, pkts); });
                pkts.Add(new ShowEffectPacket
                {
                    EffectType = EffectType.AreaBlast,
                    TargetId = Id,
                    Color = new ARGB(0xffd700),
                    PosA = new Position {X = radius}
                });
                Owner.BroadcastPackets(pkts, null);
                p++;
            }
            t += time.thisTickTimes;
            base.Tick(time);
        }
    }
}