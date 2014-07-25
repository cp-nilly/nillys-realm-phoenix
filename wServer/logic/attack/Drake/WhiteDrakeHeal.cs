#region

using System;
using Mono.Game;
using wServer.realm;
using wServer.realm.entities.player;
using wServer.svrPackets;

#endregion

namespace wServer.logic.attack
{
    internal class WhiteDrakeHeal : Behavior
    {
        protected override bool TickCore(RealmTime time)
        {
            float dist = 10;
            Player entity = Host.Self.PlayerOwner;


            if (entity.HasConditionEffect(ConditionEffects.Sick) == false)
            {
                try
                {
                    float distance = Vector2.Distance(new Vector2(Host.Self.X, Host.Self.Y),
                        new Vector2(Host.Self.PlayerOwner.X, Host.Self.PlayerOwner.Y));
                    if (distance < 6)
                    {
                        int hp = entity.HP;
                        int maxHp = entity.Stats[0] + entity.Boost[0];
                        hp = Math.Min(hp + 25, maxHp);

                        if (hp != entity.HP)
                        {
                            int n = hp - entity.HP;
                            entity.HP = hp;
                            entity.UpdateCount++;
                            entity.Owner.BroadcastPacket(new ShowEffectPacket
                            {
                                EffectType = EffectType.Potion,
                                TargetId = entity.Id,
                                Color = new ARGB(0xFFFFFF)
                            }, null);
                            entity.Owner.BroadcastPacket(new ShowEffectPacket
                            {
                                EffectType = EffectType.Trail,
                                TargetId = Host.Self.Id,
                                PosA = new Position {X = entity.X, Y = entity.Y},
                                Color = new ARGB(0xFFFFFF)
                            }, null);
                            entity.Owner.BroadcastPacket(new NotificationPacket
                            {
                                ObjectId = entity.Id,
                                Text = "+" + n,
                                Color = new ARGB(0xff00ff00)
                            }, null);
                            return true;
                        }
                        entity = GetNearestEntity(ref dist, null) as Player;
                    }
                }
                catch
                {
                }
            }
            return false;
        }
    }
}

//cerpyright (c)