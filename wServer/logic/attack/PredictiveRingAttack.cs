#region

using System;
using System.Collections.Generic;
using wServer.realm;
using wServer.realm.entities;
using wServer.svrPackets;

#endregion

namespace wServer.logic.attack
{
    internal class PredictiveRingAttack : Behavior
    {
        private static readonly Dictionary<Tuple<int, float, float, int>, PredictiveRingAttack> instances =
            new Dictionary<Tuple<int, float, float, int>, PredictiveRingAttack>();

        private readonly int count;
        private readonly int projectileIndex;
        private readonly float radius;
        private float predictFactor;
        private Random rand = new Random();

        private PredictiveRingAttack(int count, float predictFactor, float radius, int projectileIndex)
        {
            this.count = count;
            this.predictFactor = predictFactor;
            this.radius = radius;
            this.projectileIndex = projectileIndex;
        }

        public static PredictiveRingAttack Instance(int count, float predictFactor, float radius = 0,
            int projectileIndex = 0)
        {
            var key = new Tuple<int, float, float, int>(count, predictFactor, radius, projectileIndex);
            PredictiveRingAttack ret;
            if (!instances.TryGetValue(key, out ret))
                ret = instances[key] = new PredictiveRingAttack(count, predictFactor, radius, projectileIndex);
            return ret;
        }

        private double Predict(Entity entity, ProjectileDesc desc)
        {
            var history = entity.TryGetHistory(100);
            if (history == null)
                return 0;

            var originalAngle = Math.Atan2(history.Value.Y - Host.Self.Y, history.Value.X - Host.Self.X);
            var newAngle = Math.Atan2(entity.Y - Host.Self.Y, entity.X - Host.Self.X);


            var bulletSpeed = desc.Speed/100f;
            var dist = Dist(entity, Host.Self);
            var angularVelo = (newAngle - originalAngle)/(100/1000f);
            return angularVelo*bulletSpeed;
        }

        protected override bool TickCore(RealmTime time)
        {
            if (Host.Self.HasConditionEffect(ConditionEffects.Stunned)) return false;
            var dist = radius;
            var entity = radius == 0 ? null : GetNearestEntity(ref dist, null);
            if (entity != null || radius == 0)
            {
                var chr = Host as Character;
                if (chr.Owner == null) return false;
                var desc = chr.ObjectDesc.Projectiles[projectileIndex];
                var angle = entity == null ? 0 : Math.Atan2(entity.Y - chr.Y, entity.X - chr.X);
                angle = Predict(entity, desc);
                var angleInc = (2*Math.PI)/this.count;

                var count = this.count;
                if (Host.Self.HasConditionEffect(ConditionEffects.Dazed))
                    count = Math.Max(1, count/2);

                byte prjId = 0;
                var prjPos = new Position {X = chr.X, Y = chr.Y};
                var dmg = chr.Random.Next(desc.MinDamage, desc.MaxDamage);
                for (var i = 0; i < count; i++)
                {
                    var prj = chr.CreateProjectile(
                        desc, chr.ObjectType, dmg, time.tickTimes,
                        prjPos, (float) (angle + angleInc*i));
                    chr.Owner.EnterWorld(prj);
                    if (i == 0)
                        prjId = prj.ProjectileId;
                }
                chr.Owner.BroadcastPacket(new MultiShootPacket
                {
                    BulletId = prjId,
                    OwnerId = Host.Self.Id,
                    BulletType = (byte) desc.BulletType,
                    Position = prjPos,
                    Angle = (float) angle,
                    Damage = (short) dmg,
                    NumShots = (byte) count,
                    AngleIncrement = (float) angleInc,
                }, null);
                return true;
            }
            return false;
        }
    }
}