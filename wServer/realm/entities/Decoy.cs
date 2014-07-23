#region

using System;
using System.Collections.Generic;
using Mono.Game;
using wServer.logic;
using wServer.realm.entities.player;
using wServer.svrPackets;

#endregion

namespace wServer.realm.entities
{
    internal class Decoy : StaticObject, IPlayer
    {
        private static readonly Random rand = new Random();

        private readonly int duration;
        private readonly Player player;
        private readonly float speed;
        private Vector2 direction;
        private bool exploded;

        public Decoy(Player player, int duration, float tps)
            : base(0x0715, duration, true, true, true)
        {
            this.player = player;
            this.duration = duration;
            speed = tps;

            var history = player.TryGetHistory(100);
            if (history == null)
                direction = GetRandDirection();
            else
            {
                direction = new Vector2(player.X - history.Value.X, player.Y - history.Value.Y);
                if (direction.LengthSquared() == 0)
                    direction = GetRandDirection();
                else
                    direction.Normalize();
            }
        }

        public void Damage(int dmg, Character chr)
        {
        }

        public bool IsVisibleToEnemy()
        {
            return true;
        }

        private Vector2 GetRandDirection()
        {
            var angle = rand.NextDouble()*2*Math.PI;
            return new Vector2(
                (float) Math.Cos(angle),
                (float) Math.Sin(angle)
                );
        }

        public static Decoy DecoyRandom(Player player, int duration, float tps)
        {
            var d = new Decoy(player, duration, tps);
            d.direction = d.GetRandDirection();
            return d;
        }

        protected override void ExportStats(IDictionary<StatsType, object> stats)
        {
            stats[StatsType.Texture1] = player.Texture1;
            stats[StatsType.Texture2] = player.Texture2;
            base.ExportStats(stats);
        }

        public override void Tick(RealmTime time)
        {
            if (HP > duration/2)
            {
                BehaviorBase.ValidateAndMove(this,
                    X + direction.X*speed*time.thisTickTimes/1000,
                    Y + direction.Y*speed*time.thisTickTimes/1000
                    );
            }
            if (HP < 250 && !exploded)
            {
                exploded = true;
                Owner.BroadcastPacket(new ShowEffectPacket
                {
                    EffectType = EffectType.AreaBlast,
                    Color = new ARGB(0xffff0000),
                    TargetId = Id,
                    PosA = new Position {X = 1}
                }, null);
            }
            base.Tick(time);
        }
    }
}