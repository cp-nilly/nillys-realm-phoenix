#region

using System;
using System.Collections.Generic;
using wServer.realm;
using wServer.realm.entities;
using wServer.svrPackets;

#endregion

namespace wServer.logic.attack
{
    internal class AngleAttack : Behavior
    {
        private static readonly Dictionary<Tuple<float, int>, AngleAttack> instances =
            new Dictionary<Tuple<float, int>, AngleAttack>();

        private readonly float angle;
        private readonly int projectileIndex;
        private Random rand = new Random();

        private AngleAttack(float angle, int projectileIndex)
        {
            this.angle = angle;
            this.projectileIndex = projectileIndex;
        }

        public static AngleAttack Instance(float angle, int projectileIndex = 0)
        {
            var key = new Tuple<float, int>(angle, projectileIndex);
            AngleAttack ret;
            if (!instances.TryGetValue(key, out ret))
                ret = instances[key] = new AngleAttack(angle, projectileIndex);
            return ret;
        }

        protected override bool TickCore(RealmTime time)
        {
            if (Host.Self.HasConditionEffect(ConditionEffects.Stunned)) return false;

            var chr = Host as Character;
            if (chr.Owner == null) return false;
            var desc = chr.ObjectDesc.Projectiles[projectileIndex];

            var prj = chr.CreateProjectile(
                desc, chr.ObjectType, chr.Random.Next(desc.MinDamage, desc.MaxDamage),
                time.tickTimes, new Position {X = chr.X, Y = chr.Y}, angle);
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
    }
}