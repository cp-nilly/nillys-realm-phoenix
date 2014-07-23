#region

using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq;
using System.Threading;

#endregion

namespace wServer.realm
{
    public class LogicTicker
    {
        public const int TPS = 30;
        public const int MsPT = 1000/TPS;
        public static RealmTime CurrentTime;
        private readonly ConcurrentQueue<Action<RealmTime>>[] pendings;

        public LogicTicker()
        {
            pendings = new ConcurrentQueue<Action<RealmTime>>[5];
            for (var i = 0; i < 5; i++)
                pendings[i] = new ConcurrentQueue<Action<RealmTime>>();
        }

        public void AddPendingAction(Action<RealmTime> callback)
        {
            AddPendingAction(callback, PendingPriority.Normal);
        }

        public void AddPendingAction(Action<RealmTime> callback, PendingPriority priority)
        {
            pendings[(int) priority].Enqueue(callback);
        }

        public void TickLoop()
        {
            var watch = new Stopwatch();
            long dt = 0;
            long count = 0;

            watch.Start();
            var t = new RealmTime();
            long xa = 0;
            do
            {
                var times = dt/MsPT;
                dt -= times*MsPT;
                times++;

                var b = watch.ElapsedMilliseconds;

                count += times;
                if (times > 3 && (count / (b / 1000.0)) < 30)
                  // The additional conditional operand was made to negate the "LAGGED!" spamming unless the TPS goes below 30.
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("LAGGED!| time:" + times + " dt:" + dt + " count:" + count + " time:" + b +
                                      " tps:" + count/(b/1000.0));
                    Console.ForegroundColor = ConsoleColor.White;
                }

                t.tickTimes = b;
                t.tickCount = count;
                t.thisTickCounts = (int) times;
                t.thisTickTimes = (int) (times*MsPT);
                xa += t.thisTickTimes;

                foreach (var i in pendings)
                {
                    Action<RealmTime> callback;
                    while (i.TryDequeue(out callback))
                    {
                        try
                        {
                            callback(t);
                        }
                        catch
                        {
                        }
                    }
                }
                TickWorlds1(t);

                Thread.Sleep(MsPT);
                dt += Math.Max(0, watch.ElapsedMilliseconds - b - MsPT);
            } while (true);
        }

        private void TickWorlds1(RealmTime t) //Continous simulation
        {
            CurrentTime = t;
            foreach (var i in RealmManager.Worlds.Values.Distinct())
                i.Tick(t);
        }

        private void TickWorlds2(RealmTime t) //Discrete simulation
        {
            long counter = t.thisTickTimes;
            var c = t.tickCount - t.thisTickCounts;
            var x = t.tickTimes - t.thisTickTimes;
            while (counter >= MsPT)
            {
                c++;
                x += MsPT;
                TickWorlds1(new RealmTime
                {
                    thisTickCounts = 1,
                    thisTickTimes = MsPT,
                    tickCount = c,
                    tickTimes = x
                });
                counter -= MsPT;
            }
        }
    }
}