#region

using System;
using System.Collections.Generic;
using db.data;
using wServer.realm;
using wServer.realm.entities;
using wServer.realm.entities.player;
using wServer.svrPackets;

#endregion

namespace wServer.logic
{
    public interface IBehaviorHost
    {
        IDictionary<int, object> StateStorage { get; }
        Entity Self { get; }
        IDictionary<int, object> StateTable { get; }
    }

    public abstract class BehaviorBase
    {
        public static int MAGIC_EYE_KEY = -11;

        protected BehaviorBase()
        {
            Key = GetHashCode();
        }

        public int Key { get; set; }
        protected IBehaviorHost Host { get; set; }

        public static bool HasPlayerNearby(Entity entity)
        {
            foreach (Entity i in entity.Owner.PlayersCollision.HitTest(entity.X, entity.Y, 16))
            {
                float d = Dist(i, entity);
                if (d < 16*16)
                    return true;
            }
            return false;
        }

        public static bool HasPlayerNearby(World world, double x, double y)
        {
            foreach (Entity i in world.PlayersCollision.HitTest(x, y, 16))
            {
                double d = Dist(i.X, i.Y, x, y);
                if (d < 16*16)
                    return true;
            }
            return false;
        }

        public static float DistSqr(Entity a, Entity b)
        {
            float dx = a.X - b.X;
            float dy = a.Y - b.Y;
            return dx*dx + dy*dy;
        }

        public static float Dist(Entity a, Entity b)
        {
            return (float) Math.Sqrt(DistSqr(a, b));
        }

        public static double Dist(double x1, double y1, double x2, double y2)
        {
            return Math.Sqrt((x1 - x2)*(x1 - x2) + (y1 - y2)*(y1 - y2));
        }

        protected Entity GetNearestEntity(ref float dist, short? objType) //Null for player
        {
            if (Host.Self.Owner == null) return null;
            Entity ret = null;
            ;
            if (objType == null)
                foreach (Entity i in Host.Self.Owner.PlayersCollision.HitTest(Host.Self.X, Host.Self.Y, dist))
                {
                    if (!(i as IPlayer).IsVisibleToEnemy() &&
                        !Host.StateStorage.ContainsKey(MAGIC_EYE_KEY)) continue;
                    float d = Dist(i, Host.Self);
                    if (d < dist)
                    {
                        dist = d;
                        ret = i;
                    }
                }
            else
                foreach (Entity i in Host.Self.Owner.EnemiesCollision.HitTest(Host.Self.X, Host.Self.Y, dist))
                {
                    short any = 1;
                    if (i.ObjectType != objType.Value && objType.Value != any) continue;
                    if (i.isPet) continue;
                    float d = Dist(i, Host.Self);
                    if (d < dist)
                    {
                        dist = d;
                        ret = i;
                    }
                }
            return ret;
        }

        protected IEnumerable<Entity> GetNearestEntities(float dist, short? objType) //Null for player
        {
            if (Host.Self.Owner == null) yield break;
            if (objType == null)
                foreach (Entity i in Host.Self.Owner.PlayersCollision.HitTest(Host.Self.X, Host.Self.Y, dist))
                {
                    if (!(i as IPlayer).IsVisibleToEnemy() &&
                        !Host.StateStorage.ContainsKey(MAGIC_EYE_KEY)) continue;
                    float d = Dist(i, Host.Self);
                    if (d < dist)
                        yield return i;
                }
            else
                foreach (Entity i in Host.Self.Owner.EnemiesCollision.HitTest(Host.Self.X, Host.Self.Y, dist))
                {
                    if (i.ObjectType != objType.Value) continue;
                    float d = Dist(i, Host.Self);
                    if (d < dist)
                        yield return i;
                }
        }

        protected Entity GetNearestEntityByGroup(ref float dist, string group)
        {
            if (Host.Self.Owner == null) return null;
            Entity ret = null;
            foreach (Entity i in Host.Self.Owner.EnemiesCollision.HitTest(Host.Self.X, Host.Self.Y, dist))
            {
                if (i.ObjectDesc == null || i.ObjectDesc.Group != group) continue;
                float d = Dist(i, Host.Self);
                if (d < dist)
                {
                    dist = d;
                    ret = i;
                }
            }
            return ret;
        }

        protected Entity GetNearestEntityPet(ref float dist)
        {
            if (Host.Self.Owner == null) return null;
            Entity ret = null;
            ;
            foreach (Entity i in Host.Self.Owner.EnemiesCollision.HitTest(Host.Self.X, Host.Self.Y, dist))
            {
                if (i.isPet) continue;
                if (!(i is Enemy)) continue;
                if (!i.ObjectDesc.Enemy) continue;
                float d = Dist(i, Host.Self);
                if (d < dist)
                {
                    dist = d;
                    ret = i;
                }
            }
            return ret;
        }

        protected IEnumerable<Entity> GetNearestEntitiesByGroup(float dist, string group)
        {
            if (Host.Self.Owner == null)
                yield break;
            foreach (Entity i in Host.Self.Owner.EnemiesCollision.HitTest(Host.Self.X, Host.Self.Y, dist))
            {
                if (i.ObjectDesc == null || i.ObjectDesc.Group != group) continue;
                float d = Dist(i, Host.Self);
                if (d < dist)
                    yield return i;
            }
        }

        public static Entity GetNearestEntity(Entity entity, ref float dist, bool players,
            Predicate<Entity> predicate = null)
        {
            if (entity.Owner == null) return null;
            Entity ret = null;
            if (players)
                foreach (Entity i in entity.Owner.PlayersCollision.HitTest(entity.X, entity.Y, dist))
                {
                    if (!(i as IPlayer).IsVisibleToEnemy() ||
                        i == entity) continue;
                    float d = Dist(i, entity);
                    if (d < dist)
                    {
                        if (predicate != null && !predicate(i))
                            continue;
                        dist = d;
                        ret = i;
                    }
                }
            else
                foreach (Entity i in entity.Owner.EnemiesCollision.HitTest(entity.X, entity.Y, dist))
                {
                    if (i == entity) continue;
                    float d = Dist(i, entity);
                    if (d < dist)
                    {
                        if (predicate != null && !predicate(i))
                            continue;
                        dist = d;
                        ret = i;
                    }
                }
            return ret;
        }

        protected int CountEntity(float dist, short? objType)
        {
            if (Host.Self.Owner == null) return 0;
            int ret = 0;
            if (objType == null)
                foreach (Entity i in Host.Self.Owner.PlayersCollision.HitTest(Host.Self.X, Host.Self.Y, dist))
                {
                    if (!(i as IPlayer).IsVisibleToEnemy()) continue;
                    float d = Dist(i, Host.Self);
                    if (d < dist)
                        ret++;
                }
            else
                foreach (Entity i in Host.Self.Owner.EnemiesCollision.HitTest(Host.Self.X, Host.Self.Y, dist))
                {
                    if (i.ObjectType != objType.Value) continue;
                    float d = Dist(i, Host.Self);
                    if (d < dist)
                        ret++;
                }
            return ret;
        }

        protected int CountEntity(float dist, string group)
        {
            if (Host.Self.Owner == null) return 0;
            int ret = 0;
            foreach (Entity i in Host.Self.Owner.EnemiesCollision.HitTest(Host.Self.X, Host.Self.Y, dist))
            {
                if (i.ObjectDesc == null || i.ObjectDesc.Group != group) continue;
                float d = Dist(i, Host.Self);
                if (d < dist)
                    ret++;
            }
            return ret;
        }

        protected float GetSpeedMultiplier(Entity entity)
        {
            float ret = 1;
            if (entity.HasConditionEffect(ConditionEffects.Speedy))
                ret *= 2f;
            if (entity.HasConditionEffect(ConditionEffects.Slowed))
                ret *= 0.5f;
            if (entity.HasConditionEffect(ConditionEffects.Paralyzed) ||
                entity.HasConditionEffect(ConditionEffects.Stasis))
                ret = 0;
            return ret;
        }

        protected IEnumerable<Entity> HitTestPlayer(float x, float y)
        {
            if (Host.Self.Owner == null) yield break;
            foreach (Entity i in Host.Self.Owner.PlayersCollision.HitTest(x, y))
            {
                double xSide = (i.X - x);
                double ySide = (i.Y - y);
                if (xSide*xSide <= 1 && ySide*ySide <= 1)
                    yield return i;
            }
        }

        protected IEnumerable<Entity> HitTestEnemy(float x, float y)
        {
            if (Host.Self.Owner == null) yield break;
            foreach (Entity i in Host.Self.Owner.EnemiesCollision.HitTest(x, y))
            {
                double xSide = (i.X - x);
                double ySide = (i.Y - y);
                if (xSide*xSide <= 1 && ySide*ySide <= 1)
                    yield return i;
            }
        }

        protected bool Validate(float x, float y)
        {
            if (Host.Self.Owner == null ||
                Host.Self.HasConditionEffect(ConditionEffects.Paralyzed)) return false;
            if (x < 0 || x >= Host.Self.Owner.Map.Width ||
                y < 0 || y >= Host.Self.Owner.Map.Height)
                return false;
            if (Host.Self.ObjectDesc.Flying &&
                Host.Self.Owner.Obstacles[(int) x, (int) y] != 2) return true;

            WmapTile tile = Host.Self.Owner.Map[(int) x, (int) y];
            short objId = tile.ObjType;
            if (objId != 0 &&
                XmlDatas.ObjectDescs[objId].OccupySquare)
                return false;
            if (Host is Enemy && (Host as Enemy).Terrain != WmapTerrain.None &&
                (Host as Enemy).Terrain != tile.Terrain &&
                (Host as Enemy).Terrain == Host.Self.Owner.Map[(int) Host.Self.X, (int) Host.Self.Y].Terrain)
                return false;

            if (Host.Self.Owner.Obstacles[(int) x, (int) y] != 0)
                return false;
            return true;
        }

        protected bool ValidateAndMove(float x, float y)
        {
            if (Host.Self.Owner == null ||
                Host.Self.HasConditionEffect(ConditionEffects.Paralyzed)) return false;
            if (Validate(x, y))
                Host.Self.Move(x, y);
            else if (Validate(Host.Self.X, y))
                Host.Self.Move(Host.Self.X, y);
            else if (Validate(x, Host.Self.Y))
                Host.Self.Move(x, Host.Self.Y);
            else
                return false;
            return true;
        }

        public static bool Validate(Entity entity, float x, float y)
        {
            if (entity.Owner == null ||
                entity.HasConditionEffect(ConditionEffects.Paralyzed)) return false;
            if (x < 0 || x >= entity.Owner.Map.Width ||
                y < 0 || y >= entity.Owner.Map.Height)
                return false;
            if (entity.ObjectDesc.Flying &&
                entity.Owner.Obstacles[(int) x, (int) y] != 2) return true;

            short objId = entity.Owner.Map[(int) x, (int) y].ObjType;
            if (objId != 0 &&
                XmlDatas.ObjectDescs[objId].OccupySquare)
                return false;

            if (entity.Owner.Obstacles[(int) x, (int) y] != 0)
                return false;
            return true;
        }

        public static bool ValidateAndMove(Entity entity, float x, float y)
        {
            if (entity.Owner == null ||
                entity.HasConditionEffect(ConditionEffects.Paralyzed)) return false;
            if (Validate(entity, x, y))
                entity.Move(x, y);
            else if (Validate(entity, entity.X, y))
                entity.Move(entity.X, y);
            else if (Validate(entity, x, entity.Y))
                entity.Move(x, entity.Y);
            else
                return false;
            return true;
        }

        protected void AOE(World world, float radius, short? objType, Action<Entity> callback) //Null for player
        {
            if (objType == null)
                foreach (Entity i in world.PlayersCollision.HitTest(Host.Self.X, Host.Self.Y, radius))
                {
                    float d = Dist(i, Host.Self);
                    if (d < radius)
                        callback(i);
                }
            else
                foreach (Entity i in world.EnemiesCollision.HitTest(Host.Self.X, Host.Self.Y, radius))
                {
                    if (i.ObjectType != objType.Value) continue;
                    float d = Dist(i, Host.Self);
                    if (d < radius)
                        callback(i);
                }
        }

        public static void AOE(World world, Entity self, float radius, bool players, Action<Entity> callback)
            //Null for player
        {
            if (players)
                foreach (Entity i in world.PlayersCollision.HitTest(self.X, self.Y, radius))
                {
                    float d = Dist(i, self);
                    if (d < radius)
                        callback(i);
                }
            else
                foreach (Entity i in world.EnemiesCollision.HitTest(self.X, self.Y, radius))
                {
                    if (!(i is Enemy)) continue;
                    float d = Dist(i, self);
                    if (d < radius)
                        callback(i);
                }
        }

        public static void AOE(World world, Position pos, float radius, bool players, Action<Entity> callback)
            //Null for player
        {
            if (players)
                foreach (Entity i in world.PlayersCollision.HitTest(pos.X, pos.Y, radius))
                {
                    double d = Dist(i.X, i.Y, pos.X, pos.Y);
                    if (d < radius)
                        callback(i);
                }
            else
                foreach (Entity i in world.EnemiesCollision.HitTest(pos.X, pos.Y, radius))
                {
                    if (!(i is Enemy)) continue;
                    double d = Dist(i.X, i.Y, pos.X, pos.Y);
                    if (d < radius)
                        callback(i);
                }
        }

        public static void AOEPet(World world, Position pos, float radius, Action<Entity> callback) //Null for player
        {
            foreach (Entity i in world.EnemiesCollision.HitTest(pos.X, pos.Y, radius))
            {
                if (i.isPet) continue;
                if (!(i is Enemy)) continue;
                double d = Dist(i.X, i.Y, pos.X, pos.Y);
                if (d < radius)
                    callback(i);
            }
        }
    }

    public abstract class Behavior : BehaviorBase
    {
        public bool Tick(IBehaviorHost host, RealmTime time)
        {
            Host = host;
            try
            {
                return TickCore(time);
            }
            catch (Exception e)
            {
                Console.Out.WriteLine(e);
                return true;
            }
        }

        protected abstract bool TickCore(RealmTime time);
    }

    [Flags]
    public enum BehaviorCondition
    {
        OnSpawn = 1,
        OnHit = 2,
        OnDeath = 4,
        OnChat = 5,
        Other = 8,
    }

    public abstract class ConditionalBehavior : BehaviorBase
    {
        public abstract BehaviorCondition Condition { get; }

        public bool ConditionMeet(IBehaviorHost host)
        {
            Host = host;
            return ConditionMeetCore();
        }

        protected virtual bool ConditionMeetCore()
        {
            return false;
        }

        public void Behave(BehaviorCondition cond, IBehaviorHost host, RealmTime? time, object state)
        {
            Host = host;
            BehaveCore(cond, time, state);
        }

        public void Behave(BehaviorCondition cond, IBehaviorHost host, RealmTime? time, object state, string msg)
        {
            Host = host;
            BehaveCore(cond, time, state, msg);
        }

        public void Behave(BehaviorCondition cond, IBehaviorHost host, RealmTime? time, object state, string msg,
            Player player)
        {
            Host = host;
            BehaveCore(cond, time, state, msg, player);
        }

        protected virtual void BehaveCore(BehaviorCondition cond, RealmTime? time, object state)
        {
        }

        protected virtual void BehaveCore(BehaviorCondition cond, RealmTime? time, object state, string msg)
        {
        }

        protected virtual void BehaveCore(BehaviorCondition cond, RealmTime? time, object state, string msg,
            Player player)
        {
        }

        protected void Taunt(string txt)
        {
            Host.Self.Owner.BroadcastPacket(new TextPacket
            {
                Name = "#" + (Host.Self.ObjectDesc.DisplayId ?? Host.Self.ObjectDesc.ObjectId),
                ObjectId = Host.Self.Id,
                Stars = -1,
                BubbleTime = 5,
                Recipient = "",
                Text = txt,
                CleanText = ""
            }, null);
        }
    }

    internal class NullBehavior : Behavior
    {
        public static readonly NullBehavior Instance = new NullBehavior();

        private NullBehavior()
        {
        }

        protected override bool TickCore(RealmTime time)
        {
            return true;
        }
    }

    internal class RunBehaviors : Behavior
    {
        private readonly Behavior[] behavs;

        public RunBehaviors(params Behavior[] behaviors)
        {
            behavs = behaviors;
        }

        protected override bool TickCore(RealmTime time)
        {
            foreach (Behavior i in behavs)
                i.Tick(Host, time);
            return true;
        }
    }

    internal class QueuedBehavior : Behavior
    {
        private readonly Behavior[] behavs;

        public QueuedBehavior(params Behavior[] behaviors)
        {
            behavs = behaviors;
        }

        protected override bool TickCore(RealmTime time)
        {
            int idx = 0;
            object obj;
            if (Host.StateStorage.TryGetValue(Key, out obj))
                idx = (int) obj;
            else
                idx = 0;

            bool repeat = false;
            int c = 0;
            bool ret = false;
            do
            {
                repeat = false;
                if (behavs[idx].Tick(Host, time))
                {
                    repeat = true;
                    idx++;
                    if (idx == behavs.Length)
                    {
                        idx = 0;
                        ret = true;
                    }
                }
                c++;
            } while (repeat && c < 2);
            Host.StateStorage[Key] = idx;

            return ret;
        }
    }

    internal class SetKey : Behavior
    {
        private readonly int key;
        private readonly int val;

        public SetKey(int key, int val)
        {
            this.key = key;
            this.val = val;
        }

        protected override bool TickCore(RealmTime time)
        {
            Host.StateStorage[key] = val;
            Host.StateTable.Clear();
            return true;
        }
    }

    internal class RemoveKey : Behavior
    {
        private readonly int key;

        public RemoveKey(Behavior behav)
        {
            key = behav.Key;
        }

        public RemoveKey(int key)
        {
            this.key = key;
        }

        protected override bool TickCore(RealmTime time)
        {
            Host.StateStorage.Remove(key);
            return true;
        }
    }

    internal class Cooldown : Behavior
    {
        private static readonly Dictionary<Tuple<int, Behavior>, Cooldown> instances =
            new Dictionary<Tuple<int, Behavior>, Cooldown>();

        private readonly Behavior behav;
        private readonly float cooldown;
        private readonly Random rand = new Random();

        private Cooldown(int cooldown, Behavior behav)
        {
            this.cooldown = cooldown;
            this.behav = behav;
        }

        public static Cooldown Instance(int cooldown, Behavior behav = null)
        {
            var key = new Tuple<int, Behavior>(cooldown, behav);
            Cooldown ret;
            if (!instances.TryGetValue(key, out ret))
                ret = instances[key] = new Cooldown(cooldown, behav);
            return ret;
        }

        protected override bool TickCore(RealmTime time)
        {
            int remainingTick;
            object o;
            if (!Host.StateStorage.TryGetValue(Key, out o))

                remainingTick = rand.Next(0, (int) cooldown);
            else
            {
                remainingTick = (int) o;

                if (Host.Self.HasConditionEffect(ConditionEffects.Berserk))
                {
                    remainingTick = (int) cooldown/2;
                }
                if (Host.Self.HasConditionEffect(ConditionEffects.Dazed))
                {
                    remainingTick = (int) cooldown*2;
                }
            }

            remainingTick -= time.thisTickTimes;
            bool ret;
            if (remainingTick <= 0)
            {
                if (behav != null)
                    behav.Tick(Host, time);
                remainingTick = rand.Next((int) (cooldown*0.95), (int) (cooldown*1.05));
                ret = true;
            }
            else
                ret = false;
            Host.StateStorage[Key] = remainingTick;
            return ret;
        }
    }

    internal class RandomDelay : Behavior
    {
        private static readonly Dictionary<Tuple<int, int>, RandomDelay> instances =
            new Dictionary<Tuple<int, int>, RandomDelay>();

        private readonly int max;
        private readonly int min;
        private readonly Random rand = new Random();

        private RandomDelay(int min, int max)
        {
            this.min = min;
            this.max = max;
        }

        public static RandomDelay Instance(int min, int max)
        {
            var key = new Tuple<int, int>(min, max);
            RandomDelay ret;
            if (!instances.TryGetValue(key, out ret))
                ret = instances[key] = new RandomDelay(min, max);
            return ret;
        }

        protected override bool TickCore(RealmTime time)
        {
            int remainingTick;
            object o;
            if (!Host.StateStorage.TryGetValue(Key, out o))
                remainingTick = rand.Next(min, max);
            else
                remainingTick = (int) o;

            remainingTick -= time.thisTickTimes;
            bool ret;
            if (remainingTick <= 0)
            {
                remainingTick = rand.Next(min, max);
                ret = true;
            }
            else
                ret = false;
            Host.StateStorage[Key] = remainingTick;
            return ret;
        }
    }

    internal class RandomDelay2 : Behavior
    {
        private static readonly Dictionary<Tuple<int, int, Behavior>, RandomDelay2> instances =
            new Dictionary<Tuple<int, int, Behavior>, RandomDelay2>();

        private readonly Behavior behav;
        private readonly int max;
        private readonly int min;
        private readonly Random rand = new Random();

        private RandomDelay2(int min, int max, Behavior behav)
        {
            this.min = min;
            this.max = max;
            this.behav = behav;
        }

        public static RandomDelay2 Instance(int min, int max, Behavior behav)
        {
            var key = new Tuple<int, int, Behavior>(min, max, behav);
            RandomDelay2 ret;
            if (!instances.TryGetValue(key, out ret))
                ret = instances[key] = new RandomDelay2(min, max, behav);
            return ret;
        }

        protected override bool TickCore(RealmTime time)
        {
            int remainingTick;
            object o;
            if (!Host.StateStorage.TryGetValue(Key, out o))
                remainingTick = rand.Next(min, max);
            else
                remainingTick = (int) o;

            remainingTick -= time.thisTickTimes;
            bool ret;
            if (remainingTick <= 0)
            {
                if (behav != null)
                    behav.Tick(Host, time);
                remainingTick = rand.Next(min, max);
                ret = true;
            }
            else
                ret = false;
            Host.StateStorage[Key] = remainingTick;
            return ret;
        }
    }

    internal class CooldownExact : Behavior
    {
        private static readonly Dictionary<Tuple<int, Behavior>, CooldownExact> instances =
            new Dictionary<Tuple<int, Behavior>, CooldownExact>();

        private readonly Behavior behav;
        private readonly int cooldown;
        private Random rand = new Random();

        private CooldownExact(int cooldown, Behavior behav)
        {
            this.cooldown = cooldown;
            this.behav = behav;
        }

        public static CooldownExact Instance(int cooldown, Behavior behav = null)
        {
            var key = new Tuple<int, Behavior>(cooldown, behav);
            CooldownExact ret;
            if (!instances.TryGetValue(key, out ret))
                ret = instances[key] = new CooldownExact(cooldown, behav);
            return ret;
        }

        protected override bool TickCore(RealmTime time)
        {
            int remainingTick;
            object o;
            if (!Host.StateStorage.TryGetValue(Key, out o))
                remainingTick = cooldown;
            else
                remainingTick = (int) o;

            remainingTick -= time.thisTickTimes;
            bool ret;
            if (remainingTick <= 0)
            {
                if (behav != null)
                    behav.Tick(Host, time);
                remainingTick = cooldown;
                ret = true;
            }
            else
                ret = false;
            Host.StateStorage[Key] = remainingTick;
            return ret;
        }
    }

    internal class Rand : Behavior
    {
        private static readonly Dictionary<int, Rand> instances = new Dictionary<int, Rand>();
        private readonly Behavior[] behavs;
        private readonly Random rand = new Random();

        private Rand(params Behavior[] behavs)
        {
            this.behavs = behavs;
        }

        public static Rand Instance(params Behavior[] behavs)
        {
            int key = behavs.Length;
            foreach (Behavior i in behavs)
                key = key*23 + i.GetHashCode();
            Rand ret;
            if (!instances.TryGetValue(key, out ret))
                ret = instances[key] = new Rand(behavs);
            return ret;
        }

        protected override bool TickCore(RealmTime time)
        {
            return behavs[rand.Next(0, behavs.Length)].Tick(Host, time);
        }
    }

    internal class If : Behavior
    {
        private static readonly Dictionary<Tuple<Behavior, Behavior, Behavior>, If> instances =
            new Dictionary<Tuple<Behavior, Behavior, Behavior>, If>();

        private readonly Behavior cond;
        private readonly Behavior no;
        private readonly Behavior result;

        private If(Behavior cond, Behavior result, Behavior no)
        {
            this.cond = cond;
            this.result = result;
            this.no = no;
        }

        public static If Instance(Behavior cond, Behavior result, Behavior no = null)
        {
            var key = new Tuple<Behavior, Behavior, Behavior>(cond, result, no);
            If ret;
            if (!instances.TryGetValue(key, out ret))
                ret = instances[key] = new If(cond, result, no);
            return ret;
        }

        protected override bool TickCore(RealmTime time)
        {
            if (cond.Tick(Host, time))
                return result.Tick(Host, time);
            if (no != null)
                return no.Tick(Host, time);
            return false;
        }
    }

    internal class IfNot : Behavior
    {
        private static readonly Dictionary<Tuple<Behavior, Behavior>, IfNot> instances =
            new Dictionary<Tuple<Behavior, Behavior>, IfNot>();

        private readonly Behavior cond;
        private readonly Behavior result;

        private IfNot(Behavior cond, Behavior result)
        {
            this.cond = cond;
            this.result = result;
        }

        public static IfNot Instance(Behavior cond, Behavior result)
        {
            var key = new Tuple<Behavior, Behavior>(cond, result);
            IfNot ret;
            if (!instances.TryGetValue(key, out ret))
                ret = instances[key] = new IfNot(cond, result);
            return ret;
        }

        protected override bool TickCore(RealmTime time)
        {
            if (!cond.Tick(Host, time))
                return result.Tick(Host, time);
            return false;
        }
    }

    internal class IfExist : Behavior
    {
        private static readonly Dictionary<Tuple<int, Behavior, Behavior>, IfExist> instances =
            new Dictionary<Tuple<int, Behavior, Behavior>, IfExist>();

        private readonly int key;
        private readonly Behavior no;
        private readonly Behavior result;

        private IfExist(int key, Behavior result, Behavior no)
        {
            this.key = key;
            this.result = result;
            this.no = no;
        }

        public static IfExist Instance(int key, Behavior result, Behavior no = null)
        {
            var k = new Tuple<int, Behavior, Behavior>(key, result, no);
            IfExist ret;
            if (!instances.TryGetValue(k, out ret))
                ret = instances[k] = new IfExist(key, result, no);
            return ret;
        }

        protected override bool TickCore(RealmTime time)
        {
            if (Host.StateStorage.ContainsKey(key))
                return result.Tick(Host, time);
            if (no != null)
                return no.Tick(Host, time);
            return false;
        }
    }

    internal class IfEqual : Behavior
    {
        private static readonly Dictionary<Tuple<int, int, Behavior, Behavior>, IfEqual> instances =
            new Dictionary<Tuple<int, int, Behavior, Behavior>, IfEqual>();

        private readonly int key;
        private readonly Behavior no;
        private readonly Behavior result;
        private readonly int value;

        private IfEqual(int key, int value, Behavior result, Behavior no)
        {
            this.key = key;
            this.value = value;
            this.result = result;
            this.no = no;
        }

        public static IfEqual Instance(int key, int value, Behavior result, Behavior no = null)
        {
            var k = new Tuple<int, int, Behavior, Behavior>(key, value, result, no);
            IfEqual ret;
            if (!instances.TryGetValue(k, out ret))
                ret = instances[k] = new IfEqual(key, value, result, no);
            return ret;
        }

        protected override bool TickCore(RealmTime time)
        {
            object obj;
            int val;
            if (Host.StateStorage.TryGetValue(key, out obj))
                val = (int) obj;
            else
                return false;
            if (val == value)
                return result.Tick(Host, time);
            if (no != null)
                return no.Tick(Host, time);
            return false;
        }
    }

    internal class IfLesser : Behavior
    {
        private static readonly Dictionary<Tuple<int, int, Behavior, Behavior>, IfLesser> instances =
            new Dictionary<Tuple<int, int, Behavior, Behavior>, IfLesser>();

        private readonly int key;
        private readonly Behavior no;
        private readonly Behavior result;
        private readonly int value;

        private IfLesser(int key, int value, Behavior result, Behavior no)
        {
            this.key = key;
            this.value = value;
            this.result = result;
            this.no = no;
        }

        public static IfLesser Instance(int key, int value, Behavior result, Behavior no = null)
        {
            var k = new Tuple<int, int, Behavior, Behavior>(key, value, result, no);
            IfLesser ret;
            if (!instances.TryGetValue(k, out ret))
                ret = instances[k] = new IfLesser(key, value, result, no);
            return ret;
        }

        protected override bool TickCore(RealmTime time)
        {
            object obj;
            int val;
            if (Host.StateStorage.TryGetValue(key, out obj))
                val = (int) obj;
            else
                return false;
            if (val < value)
                return result.Tick(Host, time);
            if (no != null)
                return no.Tick(Host, time);
            return false;
        }
    }

    internal class IfBetween : Behavior
    {
        private static readonly Dictionary<Tuple<int, int, int, Behavior, Behavior>, IfBetween> instances =
            new Dictionary<Tuple<int, int, int, Behavior, Behavior>, IfBetween>();

        private readonly int key;
        private readonly int maxValue;
        private readonly int minValue;
        private readonly Behavior no;
        private readonly Behavior result;

        private IfBetween(int key, int minValue, int maxValue, Behavior result, Behavior no)
        {
            this.key = key;
            this.minValue = minValue;
            this.maxValue = maxValue;
            this.result = result;
            this.no = no;
        }

        public static IfBetween Instance(int key, int minValue, int maxValue, Behavior result, Behavior no = null)
        {
            var k = new Tuple<int, int, int, Behavior, Behavior>(key, minValue, maxValue, result, no);
            IfBetween ret;
            if (!instances.TryGetValue(k, out ret))
                ret = instances[k] = new IfBetween(key, minValue, maxValue, result, no);
            return ret;
        }

        protected override bool TickCore(RealmTime time)
        {
            object obj;
            int val;
            if (Host.StateStorage.TryGetValue(key, out obj))
                val = (int) obj;
            else
                return false;
            if (val < maxValue & val > minValue)
                return result.Tick(Host, time);
            if (no != null)
                return no.Tick(Host, time);
            return false;
        }
    }

    internal class IfGreater : Behavior
    {
        private static readonly Dictionary<Tuple<int, int, Behavior, Behavior>, IfGreater> instances =
            new Dictionary<Tuple<int, int, Behavior, Behavior>, IfGreater>();

        private readonly int key;
        private readonly Behavior no;
        private readonly Behavior result;
        private readonly int value;

        private IfGreater(int key, int value, Behavior result, Behavior no)
        {
            this.key = key;
            this.value = value;
            this.result = result;
            this.no = no;
        }

        public static IfGreater Instance(int key, int value, Behavior result, Behavior no = null)
        {
            var k = new Tuple<int, int, Behavior, Behavior>(key, value, result, no);
            IfGreater ret;
            if (!instances.TryGetValue(k, out ret))
                ret = instances[k] = new IfGreater(key, value, result, no);
            return ret;
        }

        protected override bool TickCore(RealmTime time)
        {
            object obj;
            int val;
            if (Host.StateStorage.TryGetValue(key, out obj))
                val = (int) obj;
            else
                return false;
            if (val > value)
                return result.Tick(Host, time);
            if (no != null)
                return no.Tick(Host, time);
            return false;
        }
    }

    internal class HpLesser : Behavior
    {
        private static readonly Dictionary<Tuple<int, Behavior, Behavior>, HpLesser> instances =
            new Dictionary<Tuple<int, Behavior, Behavior>, HpLesser>();

        private readonly Behavior no;
        private readonly Behavior result;
        private readonly int threshold;

        private HpLesser(int threshold, Behavior result, Behavior no)
        {
            this.threshold = threshold;
            this.result = result;
            this.no = no;
        }

        public static HpLesser Instance(int threshold, Behavior result, Behavior no = null)
        {
            var k = new Tuple<int, Behavior, Behavior>(threshold, result, no);
            HpLesser ret;
            if (!instances.TryGetValue(k, out ret))
                ret = instances[k] = new HpLesser(threshold, result, no);
            return ret;
        }

        protected override bool TickCore(RealmTime time)
        {
            var enemy = Host as Enemy;
            if (enemy.HP < threshold)
                return result.Tick(Host, time);
            if (no != null)
                return no.Tick(Host, time);
            return false;
        }
    }

    internal class DamageLesserEqual : Behavior
    {
        private static readonly Dictionary<Tuple<int, Behavior, Behavior>, DamageLesserEqual> instances =
            new Dictionary<Tuple<int, Behavior, Behavior>, DamageLesserEqual>();

        private readonly Behavior no;
        private readonly Behavior result;
        private readonly int threshold;

        private DamageLesserEqual(int threshold, Behavior result, Behavior no)
        {
            this.threshold = threshold;
            this.result = result;
            this.no = no;
        }

        public static DamageLesserEqual Instance(int threshold, Behavior result, Behavior no = null)
        {
            var k = new Tuple<int, Behavior, Behavior>(threshold, result, no);
            DamageLesserEqual ret;
            if (!instances.TryGetValue(k, out ret))
                ret = instances[k] = new DamageLesserEqual(threshold, result, no);
            return ret;
        }

        protected override bool TickCore(RealmTime time)
        {
            var enemy = Host as Enemy;
            if (enemy.ObjectDesc.MaxHp - enemy.HP <= threshold)
                return result.Tick(Host, time);
            if (no != null)
                return no.Tick(Host, time);
            return false;
        }
    }

    internal class HpGreaterEqual : Behavior
    {
        private static readonly Dictionary<Tuple<int, Behavior, Behavior>, HpGreaterEqual> instances =
            new Dictionary<Tuple<int, Behavior, Behavior>, HpGreaterEqual>();

        private readonly Behavior no;
        private readonly Behavior result;
        private readonly int threshold;

        private HpGreaterEqual(int threshold, Behavior result, Behavior no)
        {
            this.threshold = threshold;
            this.result = result;
            this.no = no;
        }

        public static HpGreaterEqual Instance(int threshold, Behavior result, Behavior no = null)
        {
            var k = new Tuple<int, Behavior, Behavior>(threshold, result, no);
            HpGreaterEqual ret;
            if (!instances.TryGetValue(k, out ret))
                ret = instances[k] = new HpGreaterEqual(threshold, result, no);
            return ret;
        }

        protected override bool TickCore(RealmTime time)
        {
            var enemy = Host as Enemy;
            if (enemy.HP >= threshold)
                return result.Tick(Host, time);
            if (no != null)
                return no.Tick(Host, time);
            return false;
        }
    }

    internal class HpLesserCond : ConditionalBehavior
    {
        private static readonly Dictionary<Tuple<int, Behavior, Behavior>, HpLesserCond> instances =
            new Dictionary<Tuple<int, Behavior, Behavior>, HpLesserCond>();

        private readonly Behavior no;
        private readonly Behavior result;
        private readonly int threshold;

        private HpLesserCond(int threshold, Behavior result, Behavior no)
        {
            this.threshold = threshold;
            this.result = result;
            this.no = no;
        }

        public override BehaviorCondition Condition
        {
            get { return BehaviorCondition.Other | BehaviorCondition.OnHit; }
        }

        public static HpLesserCond Instance(int threshold, Behavior result, Behavior no = null)
        {
            var k = new Tuple<int, Behavior, Behavior>(threshold, result, no);
            HpLesserCond ret;
            if (!instances.TryGetValue(k, out ret))
                ret = instances[k] = new HpLesserCond(threshold, result, no);
            return ret;
        }

        protected override bool ConditionMeetCore()
        {
            return true;
        }

        protected override void BehaveCore(BehaviorCondition cond, RealmTime? time, object state)
        {
            var enemy = Host as Enemy;
            if (enemy.HP < threshold)
                result.Tick(Host, time.Value);
            else if (no != null)
                no.Tick(Host, time.Value);
        }
    }

    internal class HpLesserPercent : Behavior
    {
        private static readonly Dictionary<Tuple<float, Behavior, Behavior>, HpLesserPercent> instances =
            new Dictionary<Tuple<float, Behavior, Behavior>, HpLesserPercent>();

        private readonly Behavior no;
        private readonly Behavior result;
        private readonly float threshold;

        private HpLesserPercent(float threshold, Behavior result, Behavior no)
        {
            this.threshold = threshold;
            this.result = result;
            this.no = no;
        }

        public static HpLesserPercent Instance(float threshold, Behavior result, Behavior no = null)
        {
            var k = new Tuple<float, Behavior, Behavior>(threshold, result, no);
            HpLesserPercent ret;
            if (!instances.TryGetValue(k, out ret))
                ret = instances[k] = new HpLesserPercent(threshold, result, no);
            return ret;
        }

        protected override bool TickCore(RealmTime time)
        {
            var enemy = Host as Enemy;
            if (enemy.HP < threshold*enemy.ObjectDesc.MaxHp)
                return result.Tick(Host, time);
            if (no != null)
                return no.Tick(Host, time);
            return false;
        }
    }

    internal class Not : Behavior
    {
        private static readonly Dictionary<Behavior, Not> instances = new Dictionary<Behavior, Not>();
        private readonly Behavior x;

        private Not(Behavior x)
        {
            this.x = x;
        }

        public static Not Instance(Behavior x)
        {
            Not ret;
            if (!instances.TryGetValue(x, out ret))
                ret = instances[x] = new Not(x);
            return ret;
        }

        protected override bool TickCore(RealmTime time)
        {
            return !x.Tick(Host, time);
        }
    }

    internal class And : Behavior
    {
        private static readonly Dictionary<Tuple<Behavior, Behavior>, And> instances =
            new Dictionary<Tuple<Behavior, Behavior>, And>();

        private readonly Behavior x;
        private readonly Behavior y;

        private And(Behavior x, Behavior y)
        {
            this.x = x;
            this.y = y;
        }

        public static And Instance(Behavior x, Behavior y)
        {
            Tuple<Behavior, Behavior> key = Tuple.Create(x, y);
            And ret;
            if (!instances.TryGetValue(key, out ret))
                ret = instances[key] = new And(x, y);
            return ret;
        }

        protected override bool TickCore(RealmTime time)
        {
            return x.Tick(Host, time) && y.Tick(Host, time);
        }
    }

    internal class Or : Behavior
    {
        private static readonly Dictionary<Tuple<Behavior, Behavior>, Or> instances =
            new Dictionary<Tuple<Behavior, Behavior>, Or>();

        private readonly Behavior x;
        private readonly Behavior y;

        private Or(Behavior x, Behavior y)
        {
            this.x = x;
            this.y = y;
        }

        public static Or Instance(Behavior x, Behavior y)
        {
            Tuple<Behavior, Behavior> key = Tuple.Create(x, y);
            Or ret;
            if (!instances.TryGetValue(key, out ret))
                ret = instances[key] = new Or(x, y);
            return ret;
        }

        protected override bool TickCore(RealmTime time)
        {
            return x.Tick(Host, time) || y.Tick(Host, time);
        }
    }

    internal class False : Behavior
    {
        private static readonly Dictionary<Behavior, False> instances = new Dictionary<Behavior, False>();
        private readonly Behavior x;

        private False(Behavior x)
        {
            this.x = x;
        }

        public static False Instance(Behavior x)
        {
            False ret;
            if (!instances.TryGetValue(x, out ret))
                ret = instances[x] = new False(x);
            return ret;
        }

        protected override bool TickCore(RealmTime time)
        {
            x.Tick(Host, time);
            return false;
        }
    }

    internal class True : Behavior
    {
        private static readonly Dictionary<Behavior, True> instances = new Dictionary<Behavior, True>();
        private readonly Behavior x;

        private True(Behavior x)
        {
            this.x = x;
        }

        public static True Instance(Behavior x)
        {
            True ret;
            if (!instances.TryGetValue(x, out ret))
                ret = instances[x] = new True(x);
            return ret;
        }

        protected override bool TickCore(RealmTime time)
        {
            x.Tick(Host, time);
            return true;
        }
    }

    internal class Once : Behavior
    {
        private static readonly Dictionary<Behavior, Once> instances = new Dictionary<Behavior, Once>();
        private readonly Behavior x;

        private Once(Behavior x)
        {
            this.x = x;
        }

        public static Once Instance(Behavior x)
        {
            Once ret;
            if (!instances.TryGetValue(x, out ret))
                ret = instances[x] = new Once(x);
            return ret;
        }

        protected override bool TickCore(RealmTime time)
        {
            if (!Host.StateStorage.ContainsKey(Key))
            {
                x.Tick(Host, time);
                Host.StateStorage.Add(Key, true);
                return true;
            }
            return false;
        }
    }

    internal class Timed : Behavior
    {
        private static readonly Dictionary<Tuple<int, Behavior>, Timed> instances =
            new Dictionary<Tuple<int, Behavior>, Timed>();

        private readonly Behavior behav;
        private readonly int time;

        private Timed(int time, Behavior behav)
        {
            this.time = time;
            this.behav = behav;
        }

        public static Timed Instance(int time, Behavior behav)
        {
            var key = new Tuple<int, Behavior>(time, behav);
            Timed ret;
            if (!instances.TryGetValue(key, out ret))
                ret = instances[key] = new Timed(time, behav);
            return ret;
        }

        protected override bool TickCore(RealmTime time)
        {
            int remainingTick;
            object o;
            if (!Host.StateStorage.TryGetValue(Key, out o))
                remainingTick = this.time;
            else
                remainingTick = (int) o;

            remainingTick -= time.thisTickTimes;
            bool ret = behav.Tick(Host, time);
            if (remainingTick <= 0)
            {
                remainingTick = this.time;
                ret = true;
            }
            else if (ret)
                remainingTick = this.time;
            Host.StateStorage[Key] = remainingTick;
            return ret;
        }
    }

    internal class Despawn : Behavior
    {
        public static readonly Despawn Instance = new Despawn();

        private Despawn()
        {
        }

        protected override bool TickCore(RealmTime time)
        {
            Host.Self.Owner.LeaveWorld(Host.Self);
            return true;
        }
    }

    internal class Die : Behavior
    {
        public static readonly Die Instance = new Die();

        private Die()
        {
        }

        protected override bool TickCore(RealmTime time)
        {
            var enemy = Host as Enemy;
            enemy.DamageCounter.Death();
            foreach (ConditionalBehavior i in enemy.CondBehaviors)
                if ((i.Condition & BehaviorCondition.OnDeath) != 0)
                    i.Behave(BehaviorCondition.OnDeath, Host, null, enemy.DamageCounter);
            try
            {
                enemy.Owner.LeaveWorld(enemy);
            }
            catch
            {
                Console.ForegroundColor = ConsoleColor.DarkBlue;
                Console.Out.WriteLine("Crash halted! - Nonexistent entity tried to die!");
                Console.ForegroundColor = ConsoleColor.White;
            }
            return true;
        }
    }

    internal class IsEntityPresent : Behavior
    {
        private static readonly Dictionary<Tuple<float, short?>, IsEntityPresent> instances =
            new Dictionary<Tuple<float, short?>, IsEntityPresent>();

        private readonly short? objType;
        private readonly float radius;
        private Random rand = new Random();

        private IsEntityPresent(float radius, short? objType)
        {
            this.radius = radius;
            this.objType = objType;
        }

        public static IsEntityPresent Instance(float radius, short? objType)
        {
            var key = new Tuple<float, short?>(radius, objType);
            IsEntityPresent ret;
            if (!instances.TryGetValue(key, out ret))
                ret = instances[key] = new IsEntityPresent(radius, objType);
            return ret;
        }

        protected override bool TickCore(RealmTime time)
        {
            float dist = radius;
            return GetNearestEntity(ref dist, objType) != null;
        }
    }

    internal class IsEntityNotPresent : Behavior
    {
        private static readonly Dictionary<Tuple<float, short?>, IsEntityNotPresent> instances =
            new Dictionary<Tuple<float, short?>, IsEntityNotPresent>();

        private readonly short? objType;
        private readonly float radius;
        private Random rand = new Random();

        private IsEntityNotPresent(float radius, short? objType)
        {
            this.radius = radius;
            this.objType = objType;
        }

        public static IsEntityNotPresent Instance(float radius, short? objType)
        {
            var key = new Tuple<float, short?>(radius, objType);
            IsEntityNotPresent ret;
            if (!instances.TryGetValue(key, out ret))
                ret = instances[key] = new IsEntityNotPresent(radius, objType);
            return ret;
        }

        protected override bool TickCore(RealmTime time)
        {
            float dist = radius;
            return GetNearestEntity(ref dist, objType) == null;
        }
    }


    internal class EntityLesserThan : Behavior
    {
        private static readonly Dictionary<Tuple<float, int, short?>, EntityLesserThan> instances =
            new Dictionary<Tuple<float, int, short?>, EntityLesserThan>();

        private readonly int count;
        private readonly short? objType;
        private readonly float radius;
        private Random rand = new Random();

        private EntityLesserThan(float radius, int count, short? objType)
        {
            this.radius = radius;
            this.count = count;
            this.objType = objType;
        }

        public static EntityLesserThan Instance(float radius, int count, short? objType)
        {
            var key = new Tuple<float, int, short?>(radius, count, objType);
            EntityLesserThan ret;
            if (!instances.TryGetValue(key, out ret))
                ret = instances[key] = new EntityLesserThan(radius, count, objType);
            return ret;
        }

        protected override bool TickCore(RealmTime time)
        {
            return CountEntity(radius, objType) < count;
        }
    }

    internal class EntityGroupLesserThan : Behavior
    {
        private static readonly Dictionary<Tuple<float, int, string>, EntityGroupLesserThan> instances =
            new Dictionary<Tuple<float, int, string>, EntityGroupLesserThan>();

        private readonly int count;
        private readonly string group;
        private readonly float radius;
        private Random rand = new Random();

        private EntityGroupLesserThan(float radius, int count, string group)
        {
            this.radius = radius;
            this.count = count;
            this.group = group;
        }

        public static EntityGroupLesserThan Instance(float radius, int count, string group)
        {
            var key = new Tuple<float, int, string>(radius, count, group);
            EntityGroupLesserThan ret;
            if (!instances.TryGetValue(key, out ret))
                ret = instances[key] = new EntityGroupLesserThan(radius, count, group);
            return ret;
        }

        protected override bool TickCore(RealmTime time)
        {
            return CountEntity(radius, group) < count;
        }
    }

    internal class WithinSpawn : Behavior
    {
        private static readonly Dictionary<float, WithinSpawn> instances = new Dictionary<float, WithinSpawn>();
        private readonly float radius;
        private Random rand = new Random();

        private WithinSpawn(float radius)
        {
            this.radius = radius;
        }

        public static WithinSpawn Instance(float radius)
        {
            WithinSpawn ret;
            if (!instances.TryGetValue(radius, out ret))
                ret = instances[radius] = new WithinSpawn(radius);
            return ret;
        }

        protected override bool TickCore(RealmTime time)
        {
            float dist = radius;
            return Dist(Host.Self.X, Host.Self.Y,
                (Host.Self as Enemy).SpawnPoint.X,
                (Host.Self as Enemy).SpawnPoint.Y) < radius;
        }
    }

    internal class MagicEye : Behavior
    {
        public static readonly MagicEye Instance = new MagicEye();

        private MagicEye()
        {
        }

        protected override bool TickCore(RealmTime time)
        {
            Host.StateStorage[MAGIC_EYE_KEY] = this;
            return true;
        }
    }

    internal class CheckConditionEffects : Behavior
    {
        private static readonly Dictionary<ConditionEffects[], CheckConditionEffects> instances =
            new Dictionary<ConditionEffects[], CheckConditionEffects>();

        private readonly ConditionEffects[] effects;

        private CheckConditionEffects(params ConditionEffects[] effects)
        {
            this.effects = effects;
        }

        public static CheckConditionEffects Instance(ConditionEffects[] effects)
        {
            CheckConditionEffects ret;
            if (!instances.TryGetValue(effects, out ret))
                ret = instances[effects] = new CheckConditionEffects(effects);
            return ret;
        }

        protected override bool TickCore(RealmTime time)
        {
            int effLength = 0;
            foreach (ConditionEffects i in effects)
                if (Host.Self.HasConditionEffect(i))
                    effLength++;
            return effLength == effects.Length;
        }
    }

    internal class PlaySound : Behavior
    {
        private static readonly Dictionary<int, PlaySound> instances = new Dictionary<int, PlaySound>();
        private readonly int soundIndex;

        private PlaySound(int soundIndex)
        {
            this.soundIndex = soundIndex;
        }

        public static PlaySound Instance(int soundIndex = 0)
        {
            PlaySound ret;
            if (!instances.TryGetValue(soundIndex, out ret))
                ret = instances[soundIndex] = new PlaySound(soundIndex);
            return ret;
        }

        protected override bool TickCore(RealmTime time)
        {
            Host.Self.Owner.BroadcastPacket(new PlaySoundPacket
            {
                OwnerId = Host.Self.Id,
                SoundId = (byte) soundIndex
            }, null);
            return true;
        }
    }

    internal class IfTag : Behavior
    {
        private readonly Behavior[] behaves;
        private readonly string tag;

        public IfTag(string tag, params Behavior[] behaves)
        {
            this.tag = tag;
            this.behaves = behaves;
        }

        protected override bool TickCore(RealmTime time)
        {
            Entity desc = Host.Self;
            if (desc.Tags.Count > 0)
            {
                if (desc.Tags.ContainsTag(tag))
                {
                    foreach (Behavior i in behaves)
                        i.Tick(Host, time);
                    return true;
                }
            }
            return false;
        }
    }

    internal class IfValue : Behavior
    {
        private readonly Behavior[] behaves;
        private readonly string tag;
        private readonly string value;

        public IfValue(string tag, string value, params Behavior[] behaves)
        {
            this.tag = tag;
            this.value = value;
            this.behaves = behaves;
        }

        protected override bool TickCore(RealmTime time)
        {
            Entity desc = Host.Self;
            if (desc.Tags.Count > 0)
            {
                if (desc.Tags.TagValue(tag, value) != null)
                {
                    foreach (Behavior i in behaves)
                        i.Tick(Host, time);
                    return true;
                }
            }
            return false;
        }
    }

    public class GetTag
    {
        private readonly short objType;

        public GetTag(int objType)
        {
            this.objType = (short) objType;
        }

        public string Get(string name, string value, string def)
        {
            try
            {
                string ret = XmlDatas.ObjectDescs[objType].Tags.TagValue(name, value);
                if (ret != null)
                    return ret;
                return def;
            }
            catch
            {
                return def;
            }
        }

        public int GetInt(string name, string value, int def)
        {
            try
            {
                string ret = XmlDatas.ObjectDescs[objType].Tags.TagValue(name, value);
                if (ret != null)
                    return Convert.ToInt32(ret);
                return def;
            }
            catch
            {
                return def;
            }
        }
    }

    internal class Switch : Behavior
    {
        private readonly Behavior[] behaves;

        public Switch(params Behavior[] behaves)
        {
            this.behaves = behaves;
        }

        protected override bool TickCore(RealmTime time)
        {
            foreach (Behavior i in behaves)
            {
                if (!i.Tick(Host, time))
                    continue;
                break;
            }
            return true;
        }
    }

    internal class PlayMusic : Behavior
    {
        private readonly string music;

        public PlayMusic(string name = "default")
        {
            music = name;
        }

        protected override bool TickCore(RealmTime time)
        {
            Host.Self.Owner.BroadcastPacket(
                new SwitchMusicPacket {Music = music == "default" ? Host.Self.Owner.GetMusic(new wRandom()) : music},
                null);
            return true;
        }
    }
}