#region

using System;
using System.Collections.Generic;
using wServer.realm.entities;
using wServer.realm.entities.player;
using wServer.realm.worlds;

#endregion

namespace wServer.logic
{
    public class DamageCounter
    {
        private readonly Enemy enemy;
        private readonly WeakDictionary<Player, int> hitters = new WeakDictionary<Player, int>();

        public DamageCounter(Enemy enemy)
        {
            this.enemy = enemy;
        }

        public Enemy Host
        {
            get { return enemy; }
        }

        public Projectile LastProjectile { get; private set; }
        public Player LastHitter { get; private set; }

        public DamageCounter Corpse { get; set; }
        public DamageCounter Parent { get; set; }

        public void HitBy(Player player, Projectile projectile, int dmg)
        {
            int totalDmg;
            if (!hitters.TryGetValue(player, out totalDmg))
                totalDmg = 0;
            totalDmg += dmg;
            hitters[player] = totalDmg;

            LastProjectile = projectile;
            LastHitter = player;

            player.FameCounter.Hit(projectile, enemy);
        }

        public Tuple<Player, int>[] GetPlayerData()
        {
            if (Parent != null)
                return Parent.GetPlayerData();
            var dat = new List<Tuple<Player, int>>();
            foreach (var i in hitters)
            {
                if (i.Key.Owner == null) continue;
                dat.Add(new Tuple<Player, int>(i.Key, i.Value));
            }
            return dat.ToArray();
        }

        public void Death()
        {
            if (Corpse != null)
            {
                Corpse.Parent = this;
                return;
            }

            var eligiblePlayers = new List<Tuple<Player, int>>();
            var totalDamage = 0;
            var totalPlayer = 0;
            var enemy = (Parent ?? this).enemy;
            foreach (var i in (Parent ?? this).hitters)
            {
                if (i.Key.Owner == null) continue;
                totalDamage += i.Value;
                totalPlayer++;
                eligiblePlayers.Add(new Tuple<Player, int>(i.Key, i.Value));
            }
            if (totalPlayer != 0)
            {
                var totalExp = totalPlayer*(enemy.ObjectDesc.MaxHp/10f)*(enemy.ObjectDesc.ExpMultiplier ?? 1);
                var lowerLimit = totalExp/totalPlayer*0.1f;
                var lvUps = 0;
                foreach (var i in eligiblePlayers)
                {
                    var playerXp = totalExp*i.Item2/totalDamage;

                    var upperLimit = i.Item1.ExperienceGoal*0.1f;
                    if (i.Item1.Quest == enemy)
                        upperLimit = i.Item1.ExperienceGoal*0.5f;

                    if (playerXp < lowerLimit) playerXp = lowerLimit;
                    if (playerXp > upperLimit) playerXp = upperLimit;

                    var killer = (Parent ?? this).LastHitter == i.Item1;
                    if (i.Item1.EnemyKilled(
                        enemy,
                        (int) playerXp,
                        killer) && !killer)
                        lvUps++;
                }
                (Parent ?? this).LastHitter.FameCounter.LevelUpAssist(lvUps);
            }

            if (enemy.Owner is GameWorld)
                (enemy.Owner as GameWorld).EnemyKilled(enemy, (Parent ?? this).LastHitter);
        }
    }
}