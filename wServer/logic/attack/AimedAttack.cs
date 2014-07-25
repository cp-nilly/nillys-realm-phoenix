using System;
using System.Collections.Generic;
using wServer.realm;
using wServer.realm.entities;
using wServer.svrPackets;

namespace wServer.logic.attack
{
    internal class AimedAttack : Behavior
    {
        private static readonly Dictionary<Tuple<float, short?, int>, AimedAttack> instances =
            new Dictionary<Tuple<float, short?, int>, AimedAttack>();

        private readonly short? objType;
        private readonly int projectileIndex;
        private readonly float radius;
        private Random rand = new Random();

        private AimedAttack(float radius, short? objType, int projectileIndex)
        {
            this.radius = radius;
            this.objType = objType;
            this.projectileIndex = projectileIndex;
        }

        public static AimedAttack Instance(float radius, short? objType, int projectileIndex = 0)
        {
            var key = new Tuple<float, short?, int>(radius, objType, projectileIndex);
            AimedAttack ret;
            if (!instances.TryGetValue(key, out ret))
                ret = instances[key] = new AimedAttack(radius, objType, projectileIndex);
            return ret;
        }

        protected override bool TickCore(RealmTime time)
        {
            if (Host.Self.HasConditionEffect(ConditionEffects.Stunned)) return false;
            float dist = radius;
            Entity entity = GetNearestEntity(ref dist, objType);
            if (entity != null)
            {
                var chr = Host as Character;
                double angle = Math.Atan2(entity.Y - chr.Y, entity.X - chr.X);
                ProjectileDesc desc = chr.ObjectDesc.Projectiles[projectileIndex];

                Projectile prj = chr.CreateProjectile(
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

    internal class AimedMultiAttack : Behavior
    {
        private static readonly Dictionary<Tuple<float, short?, float, int, float, int>, AimedMultiAttack> instances =
            new Dictionary<Tuple<float, short?, float, int, float, int>, AimedMultiAttack>();

        private readonly float angle;
        private readonly int numShot;
        private readonly short? objType;
        private readonly float offset;
        private readonly int projectileIndex;
        private readonly float radius;
        private Random rand = new Random();

        private AimedMultiAttack(float radius, short? objType, float angle, int numShot, float offset,
            int projectileIndex)
        {
            this.radius = radius;
            this.objType = objType;
            this.angle = angle;
            this.numShot = numShot;
            this.offset = offset;
            this.projectileIndex = projectileIndex;
        }

        public static AimedMultiAttack Instance(float radius, short? objType, float angle, int numShot, float offset = 0,
            int projectileIndex = 0)
        {
            var key = new Tuple<float, short?, float, int, float, int>(radius, objType, angle, numShot, offset,
                projectileIndex);
            AimedMultiAttack ret;
            if (!instances.TryGetValue(key, out ret))
                ret = instances[key] = new AimedMultiAttack(radius, objType, angle, numShot, offset, projectileIndex);
            return ret;
        }

        protected override bool TickCore(RealmTime time)
        {
            if (Host.Self.HasConditionEffect(ConditionEffects.Stunned)) return false;
            int numShot = this.numShot;
            if (Host.Self.HasConditionEffect(ConditionEffects.Dazed))
                numShot = Math.Max(1, numShot/2);

            float dist = radius;
            Entity entity = GetNearestEntity(ref dist, objType);
            if (entity != null)
            {
                var chr = Host as Character;
                double startAngle = Math.Atan2(entity.Y - chr.Y, entity.X - chr.X)
                                    - angle*(numShot - 1)/2
                                    + offset;
                ProjectileDesc desc = chr.ObjectDesc.Projectiles[projectileIndex];

                byte prjId = 0;
                var prjPos = new Position {X = chr.X, Y = chr.Y};
                int dmg = chr.Random.Next(desc.MinDamage, desc.MaxDamage);
                for (int i = 0; i < numShot; i++)
                {
                    Projectile prj = chr.CreateProjectile(
                        desc, chr.ObjectType, dmg, time.tickTimes,
                        prjPos, (float) (startAngle + angle*i));
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
                    Angle = (float) startAngle,
                    Damage = (short) dmg,
                    NumShots = (byte) numShot,
                    AngleIncrement = angle,
                }, null);
                return true;
            }
            return false;
        }
    }

    internal class AimedRingAttack : Behavior
    {
        private static readonly Dictionary<Tuple<int, float, float, short?, int>, AimedRingAttack> instances =
            new Dictionary<Tuple<int, float, float, short?, int>, AimedRingAttack>();

        private readonly int count;
        private readonly short? objType;
        private readonly float offset;
        private readonly int projectileIndex;
        private readonly float radius;
        private Random rand = new Random();

        private AimedRingAttack(int count, float radius, float offset, short? objType, int projectileIndex)
        {
            this.count = count;
            this.radius = radius;
            this.offset = offset;
            this.objType = objType;
            this.projectileIndex = projectileIndex;
        }

        public static AimedRingAttack Instance(int count, float radius = 0, float offset = 0, short? objType = null,
            int projectileIndex = 0)
        {
            var key = new Tuple<int, float, float, short?, int>(count, radius, offset, objType, projectileIndex);
            AimedRingAttack ret;
            if (!instances.TryGetValue(key, out ret))
                ret = instances[key] = new AimedRingAttack(count, radius, offset, objType, projectileIndex);
            return ret;
        }

        protected override bool TickCore(RealmTime time)
        {
            if (Host.Self.HasConditionEffect(ConditionEffects.Stunned)) return false;
            float dist = radius;
            Entity entity = radius == 0 ? null : GetNearestEntity(ref dist, objType);
            if (entity != null || radius == 0)
            {
                var chr = Host as Character;
                if (chr.Owner == null) return false;
                double angle = entity == null ? offset : Math.Atan2(entity.Y - chr.Y, entity.X - chr.X) + offset;
                double angleInc = (2*Math.PI)/this.count;
                ProjectileDesc desc = chr.ObjectDesc.Projectiles[projectileIndex];

                int count = this.count;
                if (Host.Self.HasConditionEffect(ConditionEffects.Dazed))
                    count = Math.Max(1, count/2);

                byte prjId = 0;
                var prjPos = new Position {X = chr.X, Y = chr.Y};
                int dmg = chr.Random.Next(desc.MinDamage, desc.MaxDamage);
                for (int i = 0; i < count; i++)
                {
                    Projectile prj = chr.CreateProjectile(
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