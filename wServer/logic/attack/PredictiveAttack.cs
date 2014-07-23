#region

using System;
using System.Collections.Generic;
using wServer.realm;
using wServer.realm.entities;
using wServer.svrPackets;

#endregion

namespace wServer.logic.attack
{
    internal class PredictiveAttack : Behavior
    {
        private static readonly Dictionary<Tuple<float, float, int>, PredictiveAttack> instances =
            new Dictionary<Tuple<float, float, int>, PredictiveAttack>();

        private readonly int projectileIndex;

        private readonly float radius;
        private float predictFactor;
        private Random rand = new Random();

        private PredictiveAttack(float radius, float predictFactor, int projectileIndex)
        {
            this.radius = radius;
            this.predictFactor = predictFactor;
            this.projectileIndex = projectileIndex;
        }

        public static PredictiveAttack Instance(float radius, float predictFactor, int projectileIndex = 0)
        {
            var key = new Tuple<float, float, int>(radius, predictFactor, projectileIndex);
            PredictiveAttack ret;
            if (!instances.TryGetValue(key, out ret))
                ret = instances[key] = new PredictiveAttack(radius, predictFactor, projectileIndex);
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
            var entity = GetNearestEntity(ref dist, null);
            if (entity != null)
            {
                var chr = Host as Character;
                var desc = chr.ObjectDesc.Projectiles[projectileIndex];
                var angle = Math.Atan2(entity.Y - chr.Y, entity.X - chr.X) + Predict(entity, desc);

                var prj = chr.CreateProjectile(
                    desc, chr.ObjectType, chr.Random.Next(desc.MinDamage, desc.MaxDamage),
                    time.tickTimes, new Position {X = chr.X, Y = chr.Y}, (float) angle);
                chr.Owner.EnterWorld(prj);
                if (projectileIndex == 0) //(false)
                    chr.Owner.BroadcastPacket(new ShootPacket
                    {
                        BulletId = prj.ProjectileId,
                        OwnerId = Host.Self.Id,
                        ContainerType = Host.Self.ObjectType,
                        Position = prj.BeginPos,
                        Angle = prj.Angle,
                        Damage = (short) prj.Damage
                    }, null);
                else
                    chr.Owner.BroadcastPacket(new MultiShootPacket
                    {
                        BulletId = prj.ProjectileId,
                        OwnerId = Host.Self.Id,
                        Position = prj.BeginPos,
                        Angle = prj.Angle,
                        Damage = (short) prj.Damage,
                        BulletType = (byte) (desc.BulletType),
                        AngleIncrement = 0,
                        NumShots = 1,
                    }, null);
                return true;
            }
            return false;
        }
    }
}