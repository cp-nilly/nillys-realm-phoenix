#region

using System;
using System.Diagnostics;
using db.data;
using wServer.cliPackets;
using wServer.svrPackets;

#endregion

namespace wServer.realm.entities.player
{
    public partial class Player
    {
        public void PlayerShoot(RealmTime time, PlayerShootPacket pkt)
        {
            if (Owner == null)
            {
                Console.WriteLine("[playerShoot:" + nName + "] Owner is null.");
                return;
            }

            Item item = XmlDatas.ItemDescs[pkt.ContainerType];
            if (item.ObjectType != Inventory[0].ObjectType && item.ObjectType != Inventory[1].ObjectType) return;

            if (item.DualShooting)
            {
                double arcGap1 = item.ArcGap1*Math.PI/180;
                double arcGap2 = item.ArcGap2*Math.PI/180;
                //var startAngle1 = Math.Atan2(target.Y - Y, target.X - X) - (item.NumProjectiles1 - 1) / 2 * arcGap1;
                //var startAngle2 = Math.Atan2(target.Y - Y, target.X - X) - (item.NumProjectiles2 - 1) / 2 * arcGap2;
                double startAngle1 = pkt.Angle - (item.NumProjectiles1 - 1)/2*arcGap1;
                double startAngle2 = pkt.Angle - (item.NumProjectiles2 - 1)/2*arcGap2;
                ProjectileDesc prjDesc1 = item.Projectiles[0];
                ProjectileDesc prjDesc2 = item.Projectiles[1]; //Assume only two

                for (int i = 0; i < item.NumProjectiles1; i++)
                {
                    Projectile proj = CreateProjectile(prjDesc1, item.ObjectType,
                        (int) statsMgr.GetAttackDamage(prjDesc1.MinDamage, prjDesc1.MaxDamage),
                        time.tickTimes, new Position {X = X, Y = Y}, (float) (startAngle1 + arcGap1*i));
                    Owner.EnterWorld(proj);
                    Owner.BroadcastPacket(new AllyShootPacket
                    {
                        OwnerId = Id,
                        Angle = (float) (startAngle1 + arcGap1*i),
                        ContainerType = pkt.ContainerType,
                        BulletId = 0
                    }, this);
                    fames.Shoot(proj);
                }

                for (int h = 0; h < item.NumProjectiles2; h++)
                {
                    Projectile proj = CreateProjectile(prjDesc2, item.ObjectType,
                        (int) statsMgr.GetAttackDamage(prjDesc2.MinDamage, prjDesc2.MaxDamage),
                        time.tickTimes, new Position {X = X, Y = Y}, (float) (startAngle2 + arcGap2*h));
                    Owner.EnterWorld(proj);
                    Owner.BroadcastPacket(new AllyShootPacket
                    {
                        OwnerId = Id,
                        Angle = (float) (startAngle2 + arcGap2*h),
                        ContainerType = pkt.ContainerType,
                        BulletId = 1
                    }, this);
                    fames.Shoot(proj);
                }
            }
            else
            {
                ProjectileDesc prjDesc = item.Projectiles[0]; //Assume only one
                projectileId = pkt.BulletId;
                Projectile prj = CreateProjectile(prjDesc, item.ObjectType,
                    0,
                    pkt.Time, pkt.Position, pkt.Angle);
                Owner.EnterWorld(prj);
                Owner.BroadcastPacket(new AllyShootPacket
                {
                    OwnerId = Id,
                    Angle = pkt.Angle,
                    ContainerType = pkt.ContainerType,
                    BulletId = pkt.BulletId
                }, this);
                fames.Shoot(prj);
            }
        }

        public void EnemyHit(RealmTime time, EnemyHitPacket pkt)
        {
            try
            {
                Entity entity = Owner.GetEntity(pkt.TargetId);
                Projectile prj = (this as IProjectileOwner).Projectiles[pkt.BulletId];
                prj.Damage = (int) statsMgr.GetAttackDamage(prj.Descriptor.MinDamage, prj.Descriptor.MaxDamage);
                prj.ForceHit(entity, time);
                if (pkt.Killed && !(entity is Wall))
                {
                    psr.SendPacket(new UpdatePacket
                    {
                        Tiles = new UpdatePacket.TileData[0],
                        NewObjects = new[] {entity.ToDefinition()},
                        RemovedObjectIds = new[] {pkt.TargetId}
                    });
                    _clientEntities.Remove(entity);
                }
            }
            catch
            {
            }
        }

        public void OtherHit(RealmTime time, OtherHitPacket pkt)
        {
        }

        public void SquareHit(RealmTime time, SquareHitPacket pkt)
        {
        }

        public void PlayerHit(RealmTime time, PlayerHitPacket pkt)
        {
        }

        public void ShootAck(RealmTime time, ShootAckPacket pkt)
        {
            //Console.WriteLine("ACK! " + Objects.type2desc[Owner.GetEntity(pkt.ObjectId).ObjectType].Attributes["id"].Value);
        }
    }
}