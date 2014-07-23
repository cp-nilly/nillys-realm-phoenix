#region

using System.Xml.Linq;
using wServer.realm.entities.player;
using wServer.svrPackets;

#endregion

namespace wServer.realm.entities
{
    public class Wall : StaticObject
    {
        public Wall(short objType, XElement node)
            : base(objType, GetHP(node), true, false, true)
        {
        }


        public override bool HitByProjectile(Projectile projectile, RealmTime time)
        {
            if (Vulnerable && projectile.ProjectileOwner is Player)
            {
                var dmg = (int) StatsManager.GetDefenseDamage(this, projectile.Damage, ObjectDesc.Defense);
                HP -= dmg;
                Owner.BroadcastPacket(new DamagePacket
                {
                    TargetId = Id,
                    Effects = 0,
                    Damage = (ushort) dmg,
                    Killed = !CheckHP(),
                    BulletId = projectile.ProjectileId,
                    ObjectId = projectile.ProjectileOwner.Self.Id
                }, null);
            }
            return true;
        }
    }
}