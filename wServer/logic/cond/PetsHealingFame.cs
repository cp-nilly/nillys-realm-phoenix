#region

using System;
using wServer.realm;
using wServer.realm.entities;
using wServer.realm.entities.player;
using wServer.svrPackets;

#endregion

namespace wServer.logic.cond
{
    internal class PetsHealingFame : Behavior
    {
        protected override bool TickCore(RealmTime time)
        {
            float dist = 10;
            var entity = GetNearestEntity(ref dist, null) as Player;

            while (entity == Host.Self.PlayerOwner)
            {
                while (entity != null)
                {
                    var fame = entity.Fame;
                    fame = Math.Min(fame + new Random().Next(30, 70), 400000);
                    if (fame != entity.Fame)
                    {
                        var n = fame - entity.Fame;
                        entity.Fame = fame;
                        entity.UpdateCount++;
                        entity.Owner.BroadcastPacket(new ShowEffectPacket
                        {
                            EffectType = EffectType.Potion,
                            TargetId = entity.Id,
                            Color = new ARGB(0xFFFF6600)
                        }, null);
                        entity.Owner.BroadcastPacket(new ShowEffectPacket
                        {
                            EffectType = EffectType.Trail,
                            TargetId = Host.Self.Id,
                            PosA = new Position {X = entity.X, Y = entity.Y},
                            Color = new ARGB(0xFFFF6600)
                        }, null);
                        entity.Owner.BroadcastPacket(new NotificationPacket
                        {
                            ObjectId = entity.Id,
                            Text = "+" + n,
                            Color = new ARGB(0xFFFF6600)
                        }, null);

                        return true;
                    }

                    entity = GetNearestEntity(ref dist, null) as Player;
                }
            }
            return false;
        }
    }
}