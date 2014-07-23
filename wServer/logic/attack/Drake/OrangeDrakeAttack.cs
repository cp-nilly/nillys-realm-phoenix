#region

using wServer.realm;
using wServer.realm.entities;
using wServer.svrPackets;

#endregion

namespace wServer.logic.attack
{
    internal class OrangeDrakeAttack : Behavior
    {
        private readonly float dist;

        public OrangeDrakeAttack(float dist)
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
                    Color = new ARGB(0xEB8931),
                    TargetId = entity.Id,
                    PosA = new Position {X = 1,}
                }, null);
                entity.Owner.BroadcastPacket(new ShowEffectPacket
                {
                    EffectType = EffectType.Trail,
                    TargetId = Host.Self.Id,
                    PosA = new Position {X = entity.X, Y = entity.Y},
                    Color = new ARGB(0xEB8931)
                }, null);
                entity.ApplyConditionEffect(new ConditionEffect
                {
                    Effect = ConditionEffectIndex.Stasis,
                    DurationMS = 5000
                });
            }
            return false;
        }
    }
}