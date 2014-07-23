#region

using wServer.realm;
using wServer.realm.entities;
using wServer.svrPackets;

#endregion

namespace wServer.logic.attack
{
    internal class PurpleDrakeAttack : Behavior
    {
        private readonly float dist;

        public PurpleDrakeAttack(float dist)
        {
            this.dist = dist;
        }

        protected override bool TickCore(RealmTime time)
        {
            var radius = dist;
            var entity = GetNearestEntityPet(ref radius) as Enemy;

            if (entity != null)
            {
                entity.Owner.BroadcastPacket(new ShowEffectPacket
                {
                    EffectType = EffectType.AreaBlast,
                    Color = new ARGB(0x3E3A78),
                    TargetId = entity.Id,
                    PosA = new Position {X = 1,}
                }, null);
                entity.Owner.BroadcastPacket(new ShowEffectPacket
                {
                    EffectType = EffectType.Trail,
                    TargetId = Host.Self.Id,
                    PosA = new Position {X = entity.X, Y = entity.Y},
                    Color = new ARGB(0x3E3A78)
                }, null);
                entity.Damage(Host.Self.PlayerOwner, time, 35, false, new ConditionEffect[] {});
            }
            return false;
        }
    }
}