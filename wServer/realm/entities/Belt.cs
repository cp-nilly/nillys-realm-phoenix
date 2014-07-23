//#region

//using System.Collections.Generic;
//using wServer.logic;
//using wServer.realm.entities.player;
//using wServer.svrPackets;

//#endregion

//namespace wServer.realm.entities
//{
//    internal class Belt : StaticObject
//    {
//        private readonly float radius;
//        private int lifetime;
//        private int health;

//        private int p;
//        private int p2;
//        private Player player;
//        private int t;

//        public Belt(Player player, float radius, int health)
//            : base(0x0711, 5000, true, true, false)
//        {
//            this.player = player;
//            this.radius = radius;
//            this.health = health;
//        }
//        public override void Tick(RealmTime time)
//        {
//            foreach (var i in Owner.EnemiesCollision.HitTest(this.X, this.Y, radius))
//                {
                   
                        
//                }
                
//            if (t / 500 == p2)
//            {
     
//                Owner.BroadcastPacket(new ShowEffectPacket
//                {
//                    EffectType = EffectType.Trap,
//                    Color = new ARGB(0x800000),
//                    TargetId = Id,
//                    PosA = new Position { X = radius }
//                }, null);
//                p2++;
//                //Stuff
//            }
//            if (health < 0)
//            {
//                Owner.LeaveWorld(this);
//            }
//            t += time.thisTickTimes;
//            base.Tick(time);
//        }
//    }
//}