#region

using wServer.logic.movement;
using wServer.logic.travoos;

#endregion

namespace wServer.logic
{
    partial class BehaviorDb
    {
        private static _ ZombieGame = Behav()
            .Init(0x7020, Behaves("Regular Zombie",
                IfNot.Instance(Chasing.Instance(4f, 200, 1, 0x7023), new RunBehaviors(
                    WorldEvent.Instance("dmg:2"),
                    Despawn.Instance
                    ))
                ))
            .Init(0x7021, Behaves("Tank Zombie",
                IfNot.Instance(Chasing.Instance(3f, 200, 1, 0x7023), new RunBehaviors(
                    WorldEvent.Instance("dmg:4"),
                    Despawn.Instance
                    ))
                ))
            .Init(0x7022, Behaves("Speedy Zombie",
                IfNot.Instance(Chasing.Instance(6f, 200, 1, 0x7023), new RunBehaviors(
                    WorldEvent.Instance("dmg:1"),
                    Despawn.Instance
                    ))
                ));
    }
}