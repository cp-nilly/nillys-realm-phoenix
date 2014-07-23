#region

using wServer.realm;
using wServer.realm.entities;
using wServer.svrPackets;

#endregion

namespace wServer.logic.attack
{
    internal class GreenDrakeAttack : Behavior
    {
        private readonly float dist;

        public GreenDrakeAttack(float dist)
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
                    Color = new ARGB(0x48D747),
                    TargetId = entity.Id,
                    PosA = new Position {X = 1,}
                }, null);
                entity.Owner.BroadcastPacket(new ShowEffectPacket
                {
                    EffectType = EffectType.Trail,
                    TargetId = Host.Self.Id,
                    PosA = new Position {X = entity.X, Y = entity.Y},
                    Color = new ARGB(0x48D747)
                }, null);
                entity.ApplyConditionEffect(new ConditionEffect
                {
                    Effect = ConditionEffectIndex.Slowed,
                    DurationMS = 10000
                });
            }
            return false;
        }
    }
}