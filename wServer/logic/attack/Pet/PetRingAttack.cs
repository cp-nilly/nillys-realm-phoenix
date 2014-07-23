#region

using System;
using System.Collections.Generic;
using wServer.realm;
using wServer.realm.entities;
using wServer.svrPackets;

#endregion

namespace wServer.logic.attack
{
    internal class PetRingAttack : Behavior
    {
        private static readonly Dictionary<Tuple<int, float, float, int>, PetRingAttack> instances =
            new Dictionary<Tuple<int, float, float, int>, PetRingAttack>();

        private readonly int count;
        private readonly float offset;
        private readonly int projectileIndex;
        private readonly float radius;
        private Random rand = new Random();

        private PetRingAttack(int count, float radius, float offset, int projectileIndex)
        {
            this.count = count;
            this.radius = radius;
            this.offset = offset;
            this.projectileIndex = projectileIndex;
        }

        public static PetRingAttack Instance(int count, float radius = 0, float offset = 0, int projectileIndex = 0)
        {
            var key = new Tuple<int, float, float, int>(count, radius, offset, projectileIndex);
            PetRingAttack ret;
            if (!instances.TryGetValue(key, out ret))
                ret = instances[key] = new PetRingAttack(count, radius, offset, projectileIndex);
            return ret;
        }

        protected override bool TickCore(RealmTime time)
        {
            if (Host.Self.HasConditionEffect(ConditionEffects.Stunned)) return false;
            var dist = radius;
            var entity = radius == 0 ? null : GetNearestEntityPet(ref dist);
            if (entity != null || radius == 0)
            {
                var chr = Host as Character;
                if (chr.Owner == null) return false;
                var angle = entity == null ? offset : Math.Atan2(entity.Y - chr.Y, entity.X - chr.X) + offset;
                var angleInc = (2*Math.PI)/this.count;
                var desc = chr.ObjectDesc.Projectiles[projectileIndex];

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