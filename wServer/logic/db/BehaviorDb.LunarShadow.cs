#region

using System;
using wServer.logic.attack;
using wServer.logic.loot;
using wServer.logic.movement;
using wServer.logic.taunt;

#endregion

namespace wServer.logic
{
    partial class BehaviorDb
    {
        private static _ ShadowMage = Behav()
            .Init(0x3704, Behaves("The Shadow Mage",
                new RunBehaviors(
                    Once.Instance(new SetKey(-1, 1)),
                
                    IfEqual.Instance(-1, 1,
                        new RunBehaviors(
                            HpLesser.Instance(2500000, new SetKey(-1, 2))
                            )
                        ),
                
                    IfEqual.Instance(-1, 2,
                        new RunBehaviors(
                            new QueuedBehavior(
                                Once.Instance(SetConditionEffect.Instance(ConditionEffectIndex.StunImmume)),
                                Once.Instance(SetConditionEffect.Instance(ConditionEffectIndex.Invincible)),
                                Cooldown.Instance(1000),
                                new SimpleTaunt("Oh? You wish to battle me?"),
                                Cooldown.Instance(4000),
                                new SimpleTaunt("Hmph, very well"),
                                Cooldown.Instance(4000),
                                new SimpleTaunt("However"),
                                Cooldown.Instance(4000),
                                new SimpleTaunt("I dislike one-sided fights"),
                                Cooldown.Instance(4000),
                                new SimpleTaunt("I shall give you this advantage"),
                                new BuffAll(),
                                Cooldown.Instance(4000),
                                new SimpleTaunt("Now, it is time for you to be eliminated"),
                                Cooldown.Instance(4000),
                                new SimpleTaunt("Farewell"),
                                Cooldown.Instance(2000),
                                UnsetConditionEffect.Instance(ConditionEffectIndex.Invincible),
                                new SetKey(-1, 3)
                                )
                            )
                        ),
                    IfEqual.Instance(-1, 3,
                        new RunBehaviors(
                            new RunBehaviors(
                                Cooldown.Instance(500, SimpleAttack.Instance(20, 0)),
                                Chasing.Instance(15, 10, 1, null)
                                ),
                            new QueuedBehavior(
                                new SimpleTaunt("Surely, you don't think you'll beat me, do you?"),
                                Cooldown.Instance(10000),
                                new SimpleTaunt("Now that I have decreased your numbers, it's time to unleash a little more of my powers"),
                                new SetKey(-1, 4)
                                )
                            )
                        ),
                    IfEqual.Instance(-1, 4,
                        new RunBehaviors(
                            new RunBehaviors(
                                InfiniteSpiralAttack.Instance(100, 4, 15, 1),
                                InfiniteSpiralAttack.Instance(100, 4, 30, 1),
                                Cooldown.Instance(500, MultiAttack.Instance(30, 90 * (float)Math.PI / 180, 30, 0, 0))
                                
                                ),
                            new QueuedBehavior(
                                Cooldown.Instance(3000),
                                new SimpleTaunt("Boring..."),
                                Cooldown.Instance(5000),
                                new SimpleTaunt("Even with your advantage, you are weak"),
                                Cooldown.Instance(30000),
                                new SimpleTaunt("I shall do away with you now"),
                                Cooldown.Instance(2000),
                                new SetKey(-1, 5)
                                )
                            )
                        ),
                    IfEqual.Instance(-1, 5,
                        new RunBehaviors(
                            new QueuedBehavior(
                                Cooldown.Instance(5000),
                                new SimpleTaunt("There is no more need for your existence"),
                                Cooldown.Instance(5000),
                                new SimpleTaunt("I will wipe you off the face of this realm"),
                                Cooldown.Instance(5000),
                                new SimpleTaunt("Finishing Move..."),
                                Cooldown.Instance(3000),
                                new ParalyzeAll(),
                                new SimpleTaunt("Elimination Void!"),
                                new SetKey(-1, 6)
                                )
                            )
                        ),
                    IfEqual.Instance(-1, 6,
                        new RunBehaviors(
                            Cooldown.Instance(500, new DamageAll())
                            )
                        )
                    )
            ));
                
    }
}