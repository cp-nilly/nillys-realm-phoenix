#region

using System;
using wServer.logic.attack;
using wServer.logic.movement;
using wServer.logic.taunt;

#endregion

namespace wServer.logic
{
    partial class BehaviorDb
    {
        private static _ Madlab = Behav()
            .Init(0x0976, Behaves("Dr Terrible",
                new RunBehaviors(
                    Cooldown.Instance(3000, TossEnemy.Instance(0f, 4f, 0x0978)),
                    SimpleWandering.Instance(2, 2)
                    ),
                Cooldown.Instance(1000,
                    Rand.Instance(
                        new RandomTaunt(0.001, "baa"),
                        new RandomTaunt(0.001, "baa baa")
                        )
                    )
                ))
            .Init(0x5e1c, Behaves("Horrific Creation",
                new RunBehaviors(
                    Cooldown.Instance(1000, MultiAttack.Instance(25, 10*(float) Math.PI/180, 4, 0, 1)),
                    SimpleWandering.Instance(2, 2)
                    ),
                Cooldown.Instance(1000,
                    Rand.Instance(
                        new RandomTaunt(0.001, "baa"),
                        new RandomTaunt(0.001, "baa baa")
                        )
                    )
                ));
    }
}