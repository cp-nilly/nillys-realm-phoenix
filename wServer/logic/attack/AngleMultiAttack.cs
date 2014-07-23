#region

using System;
using System.Collections.Generic;
using wServer.realm;
using wServer.realm.entities;
using wServer.svrPackets;

#endregion

namespace wServer.logic.attack
{
    internal class AngleMultiAttack : Behavior
    {
        private static readonly Dictionary<Tuple<float, float, int, int>, AngleMultiAttack> instances =
            new Dictionary<Tuple<float, float, int, int>, AngleMultiAttack>();

        private readonly float angle;
        private readonly int numShot;
        private readonly float projAngle;
        private readonly int projectileIndex;
        private Random rand = new Random();

        private AngleMultiAttack(float angle, float projAngle, int numShot, int projectileIndex)
        {
            this.angle = angle;
            this.projAngle = projAngle;
            this.numShot = numShot;
            this.projectileIndex = projectileIndex;
        }

        public static AngleMultiAttack Instance(float angle, float projAngle, int numShot, int projectileIndex = 0)
        {
            var key = new Tuple<float, float, int, int>(angle, projAngle, numShot, projectileIndex);
            AngleMultiAttack ret;
            if (!instances.TryGetValue(key, out ret))
                ret = instances[key] = new AngleMultiAttack(angle, projAngle, numShot, projectileIndex);
            return ret;
        }

        protected override bool TickCore(RealmTime time)
        {
            if (Host.Self.HasConditionEffect(ConditionEffects.Stunned)) return false;

            var chr = Host as Character;
            var startAngle = angle - projAngle*(numShot - 1)/2;
            var desc = chr.ObjectDesc.Projectiles[projectileIndex];

            byte prjId = 0;
            var prjPos = new Position {X = chr.X, Y = chr.Y};
            var dmg = chr.Random.Next(desc.MinDamage, desc.MaxDamage);
            for (var i = 0; i < numShot; i++)
            {
                var prj = chr.CreateProjectile(
                    desc, chr.ObjectType, dmg, time.tickTimes,
                    prjPos, startAngle + projAngle*i);
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
                Angle = startAngle,
                Damage = (short) dmg,
                NumShots = (byte) numShot,
                AngleIncrement = projAngle,
            }, null);
            return true;
        }
    }
}