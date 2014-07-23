#region

using wServer.realm;
using wServer.realm.entities;
using wServer.svrPackets;

#endregion

namespace wServer.logic.attack
{
    internal class YellowDrakeAttack : Behavior
    {
        private readonly float dist;

        public YellowDrakeAttack(float dist)
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
                    Color = new ARGB(0xEEEE73),
                    TargetId = entity.Id,
                    PosA = new Position {X = 1,}
                }, null);
                entity.Owner.BroadcastPacket(new ShowEffectPacket
                {
                    EffectType = EffectType.Trail,
                    TargetId = Host.Self.Id,
                    PosA = new Position {X = entity.X, Y = entity.Y},
                    Color = new ARGB(0xEEEE73)
                }, null);
                entity.ApplyConditionEffect(new ConditionEffect
                {
                    Effect = ConditionEffectIndex.Bleeding,
                    DurationMS = 10000
                });
            }
            return false;
        }
    }
}