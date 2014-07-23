#region

using System.Collections.Generic;
using wServer.logic;
using wServer.realm.entities.player;
using wServer.svrPackets;

#endregion

namespace wServer.realm.entities
{
    public class Enemy : Character
    {
        private readonly DamageCounter counter;
        private readonly bool stat;
        private float bleeding;
        private Position? pos;

        public Enemy(short objType)
            : base(objType, new wRandom())
        {
            stat = ObjectDesc.MaxHp == 0;
            counter = new DamageCounter(this);
        }

        public DamageCounter DamageCounter
        {
            get { return counter; }
        }

        public WmapTerrain Terrain { get; set; }

        public int AltTextureIndex { get; set; }

        public Position SpawnPoint
        {
            get { return pos ?? new Position {X = X, Y = Y}; }
        }

        protected override void ExportStats(IDictionary<StatsType, object> stats)
        {
            stats[StatsType.AltTextureIndex] = AltTextureIndex;
            stats[StatsType.HP] = HP;
            base.ExportStats(stats);
        }

        protected override void ImportStats(StatsType stats, object val)
        {
            if (stats == StatsType.AltTextureIndex) AltTextureIndex = (int) val;
            else if (stats == StatsType.HP) HP = (int) val;
            base.ImportStats(stats, val);
        }

        public override void Init(World owner)
        {
            base.Init(owner);
            if (ObjectDesc.StasisImmune)
                ApplyConditionEffect(new ConditionEffect
                {
                    Effect = ConditionEffectIndex.StasisImmune,
                    DurationMS = -1
                });
            foreach (var i in CondBehaviors)
                if ((i.Condition & BehaviorCondition.OnSpawn) != 0)
                    i.Behave(BehaviorCondition.OnSpawn, this, null, null);
        }

        public int Damage(Player from, RealmTime time, int dmg, bool noDef, params ConditionEffect[] effs)
        {
            if (stat) return 0;
            if (HasConditionEffect(ConditionEffects.Invincible))
                return 0;
            if (!HasConditionEffect(ConditionEffects.Paused) &&
                !HasConditionEffect(ConditionEffects.Stasis))
            {
                var def = ObjectDesc.Defense;
                if (noDef)
                    def = 0;
                dmg = (int) StatsManager.GetDefenseDamage(this, dmg, def);
                var effDmg = dmg;
                if (effDmg > HP)
                    effDmg = HP;
                if (!HasConditionEffect(ConditionEffects.Invulnerable))
                    HP -= dmg;
                ApplyConditionEffect(effs);
                Owner.BroadcastPacket(new DamagePacket
                {
                    TargetId = Id,
                    Effects = 0,
                    Damage = (ushort) dmg,
                    Killed = HP < 0,
                    BulletId = 0,
                    ObjectId = from.Id
                }, null);

                foreach (var i in CondBehaviors)
                    if ((i.Condition & BehaviorCondition.OnHit) != 0)
                        i.Behave(BehaviorCondition.OnHit, this, time, null);
                counter.HitBy(from, null, dmg);

                if (HP < 0)
                {
                    foreach (var i in CondBehaviors)
                        if ((i.Condition & BehaviorCondition.OnDeath) != 0)
                            i.Behave(BehaviorCondition.OnDeath, this, time, counter);
                    counter.Death();
                    if (Owner != null)
                        Owner.LeaveWorld(this);
                }

                UpdateCount++;
                return effDmg;
            }
            return 0;
        }

        public override bool HitByProjectile(Projectile projectile, RealmTime time)
        {
            if (stat) return false;
            if (HasConditionEffect(ConditionEffects.Invincible))
                return false;

            var player = projectile.ProjectileOwner is Player;
            var pet = projectile.ProjectileOwner.Self.isPet;

            if ((player || pet) &&
                !HasConditionEffect(ConditionEffects.Paused) &&
                !HasConditionEffect(ConditionEffects.Stasis))
            {
                var plr = pet ? projectile.ProjectileOwner.Self.PlayerOwner : projectile.ProjectileOwner as Player;

                var def = ObjectDesc.Defense;
                if (projectile.Descriptor.ArmorPiercing)
                    def = 0;
                var dmg = (int) StatsManager.GetDefenseDamage(this, projectile.Damage, def);
                if (!HasConditionEffect(ConditionEffects.Invulnerable))
                    HP -= dmg;
                ApplyConditionEffect(projectile.Descriptor.Effects);
                Owner.BroadcastPacket(new DamagePacket
                {
                    TargetId = Id,
                    Effects = projectile.ConditionEffects,
                    Damage = (ushort) dmg,
                    Killed = HP < 0,
                    BulletId = projectile.ProjectileId,
                    ObjectId = projectile.ProjectileOwner.Self.Id
                }, pet ? null : plr);

                foreach (var i in CondBehaviors)
                    if ((i.Condition & BehaviorCondition.OnHit) != 0)
                        i.Behave(BehaviorCondition.OnHit, this, time, projectile);
                counter.HitBy(plr, projectile, dmg);

                if (HP < 0)
                {
                    foreach (var i in CondBehaviors)
                        if ((i.Condition & BehaviorCondition.OnDeath) != 0)
                            i.Behave(BehaviorCondition.OnDeath, this, time, counter);
                    counter.Death();
                    if (Owner != null)
                        Owner.LeaveWorld(this);
                }
                UpdateCount++;
                return true;
            }
            return false;
        }

        public override void Tick(RealmTime time)
        {
            if (pos == null)
                pos = new Position {X = X, Y = Y};

            if (!stat && HasConditionEffect(ConditionEffects.Bleeding))
            {
                if (bleeding > 1)
                {
                    HP -= (int) bleeding;
                    bleeding -= (int) bleeding;
                    UpdateCount++;
                }
                bleeding += 28*(time.thisTickTimes/1000f);
            }
            base.Tick(time);
        }
    }
}