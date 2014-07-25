#region

using System;
using wServer.realm;
using wServer.realm.entities.player;
using wServer.svrPackets;

#endregion

namespace wServer.logic.cond
{
    internal class NexusHealMp : Behavior
    {
        protected override bool TickCore(RealmTime time)
        {
            float dist = 5;
            var entity = GetNearestEntity(ref dist, null) as Player;
            while (entity != null)
            {
                if (entity.HasConditionEffect(ConditionEffects.Quiet)) return false;
                int mp = entity.MP;
                int maxMp = entity.Stats[1] + entity.Boost[1];
                mp = Math.Min(mp + 100, maxMp);
                if (mp != entity.MP)
                {
                    int n = mp - entity.MP;
                    entity.MP = mp;
                    entity.UpdateCount++;
                    entity.Owner.BroadcastPacket(new ShowEffectPacket
                    {
                        EffectType = EffectType.Potion,
                        TargetId = entity.Id,
                        Color = new ARGB(0xffffffff)
                    }, null);
                    entity.Owner.BroadcastPacket(new ShowEffectPacket
                    {
                        EffectType = EffectType.Trail,
                        TargetId = Host.Self.Id,
                        PosA = new Position {X = entity.X, Y = entity.Y},
                        Color = new ARGB(0xffffffff)
                    }, null);
                    entity.Owner.BroadcastPacket(new NotificationPacket
                    {
                        ObjectId = entity.Id,
                        Text = "+" + n,
                        Color = new ARGB(0xff9000ff)
                    }, null);

                    return true;
                }
                entity = GetNearestEntity(ref dist, null) as Player;
            }
            return false;
        }
    }
}