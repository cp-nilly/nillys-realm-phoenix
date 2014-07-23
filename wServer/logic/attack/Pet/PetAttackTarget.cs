#region

using System;
using System.Collections.Generic;
using wServer.realm;
using wServer.realm.entities;
using wServer.realm.entities.player;
using wServer.svrPackets;

#endregion

namespace wServer.logic.attack
{
    internal class PetAttackTarget : Behavior
    {
        private static readonly Dictionary<Tuple<float, int, int>, PetAttackTarget> instances =
            new Dictionary<Tuple<float, int, int>, PetAttackTarget>();

        private readonly int numshot;
        private readonly int projectileIndex;
        private float radius;
        private Random rand = new Random();

        private PetAttackTarget(float radius, int projectileIndex, int numshot)
        {
            this.projectileIndex = projectileIndex;
            this.radius = radius;
            this.numshot = numshot;
        }

        public static PetAttackTarget Instance(float radius, int numshot, int projectileIndex = 0)
        {
            var key = new Tuple<float, int, int>(radius, projectileIndex, numshot);
            PetAttackTarget ret;
            if (!instances.TryGetValue(key, out ret))
                ret = instances[key] = new PetAttackTarget(radius, projectileIndex, numshot);
            return ret;
        }

        protected override bool TickCore(RealmTime time)
        {
            var targetlocation = Player.targetlink;
            var chr = Host as Character;
            var arcGap = 11.25f*Math.PI/180;
            var startAngle = Math.Atan2(targetlocation.Y - chr.Y, targetlocation.X - chr.X) - (numshot - 1)/2*arcGap;
            var desc = chr.ObjectDesc.Projectiles[projectileIndex];
            byte prjId = 0;
            var prjPos = new Position {X = chr.X, Y = chr.Y};
            var dmg = chr.Random.Next(desc.MinDamage, desc.MaxDamage);
            for (var i = 0; i < numshot; i++)
            {
                var prj = chr.CreateProjectile(
                    desc, chr.ObjectType, dmg, time.tickTimes,
                    prjPos, (float) (startAngle + arcGap*i));
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
                NumShots = (byte) numshot,
                AngleIncrement = 11.25f*(float) Math.PI/180,
            }, null);
            return true;
        }
    }
}