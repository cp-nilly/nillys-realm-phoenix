#region

using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

#endregion

namespace wServer.realm
{
    public class SpatialStorage
    {
        private const int SCALE_FACTOR = 16;

        private readonly ConcurrentDictionary<int, ConcurrentDictionary<int, Entity>> store =
            new ConcurrentDictionary<int, ConcurrentDictionary<int, Entity>>();

        private int HashPosition(double x, double y)
        {
            var ix = (int) x/SCALE_FACTOR;
            var iy = (int) y/SCALE_FACTOR;
            return (ix << 16) | iy;
        }

        public void Insert(Entity entity)
        {
            var hash = HashPosition(entity.X, entity.Y);
            var bucket = store.GetOrAdd(hash, _ => new ConcurrentDictionary<int, Entity>());
            bucket[entity.Id] = entity;
        }

        public void Remove(Entity entity)
        {
            var hash = HashPosition(entity.X, entity.Y);
            var bucket = store[hash];
            bucket.TryRemove(entity.Id, out entity);
        }

        public void Move(Entity entity, double x, double y)
        {
            var hash = HashPosition(entity.X, entity.Y);
            var bucket = store.GetOrAdd(hash, _ => new ConcurrentDictionary<int, Entity>());
            Entity dummy;
            bucket.TryRemove(entity.Id, out dummy);

            hash = HashPosition(x, y);
            bucket = store.GetOrAdd(hash, _ => new ConcurrentDictionary<int, Entity>());
            bucket[entity.Id] = entity;
        }

        public IEnumerable<Entity> HitTest(Position pos, float radius)
        {
            return HitTest(pos.X, pos.Y, radius);
        }

        public IEnumerable<Entity> HitTest(double _x, double _y, float radius)
        {
            var xl = (int) (_x - radius)/SCALE_FACTOR;
            var xh = (int) (_x + radius)/SCALE_FACTOR;
            var yl = (int) (_y - radius)/SCALE_FACTOR;
            var yh = (int) (_y + radius)/SCALE_FACTOR;
            for (var x = xl; x <= xh; x++)
                for (var y = yl; y <= yh; y++)
                {
                    ConcurrentDictionary<int, Entity> bucket;
                    if (store.TryGetValue((x << 16) | y, out bucket))
                        foreach (var i in bucket) yield return i.Value;
                }
        }

        public IEnumerable<Entity> HitTest(double _x, double _y)
        {
            var x = (int) _x/SCALE_FACTOR;
            var y = (int) _y/SCALE_FACTOR;
            ConcurrentDictionary<int, Entity> bucket;
            if (store.TryGetValue((x << 16) | y, out bucket))
                return bucket.Values;
            return Enumerable.Empty<Entity>();
        }
    }
}