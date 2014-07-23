#region

using System;
using wServer.logic.loot;
using wServer.logic.taunt;

#endregion

namespace wServer.logic
{
    partial class BehaviorDb
    {
        private static _ DavyJonesLocker = Behav()
            .Init(0x0e37, Behaves("Ghost Ship",
                new RunBehaviors(
                    Once.Instance(new SimpleTaunt("I love Davy!"))
                    )
                ))
            .Init(0x0e41, Behaves("Yellow Key",
                SetConditionEffect.Instance(ConditionEffectIndex.Invincible),
                If.Instance(IsEntityPresent.Instance(1, null),
                    Despawn.Instance
                    )
                ))
            .Init(0x0e3f, Behaves("Green Key",
                SetConditionEffect.Instance(ConditionEffectIndex.Invincible),
                If.Instance(IsEntityPresent.Instance(1, null),
                    Despawn.Instance
                    )
                ))
            .Init(0x0e40, Behaves("Red Key",
                SetConditionEffect.Instance(ConditionEffectIndex.Invincible),
                If.Instance(IsEntityPresent.Instance(1, null),
                    Despawn.Instance
                    )
                ))
            .Init(0x0e3e, Behaves("Purple Key",
                SetConditionEffect.Instance(ConditionEffectIndex.Invincible),
                If.Instance(IsEntityPresent.Instance(1, null),
                    Despawn.Instance
                    )
                ))
            .Init(0x0e36, Behaves("Ghost Lanturn On",
                SetConditionEffect.Instance(ConditionEffectIndex.Invincible),
                new QueuedBehavior(
                    Cooldown.Instance(20000),
                    new Transmute(0x0e35, 1, 1)
                    )
                ))
            .Init(0x0e35, Behaves("Ghost Lanturn Off",
                condBehaviors: new ConditionalBehavior[]
                {
                    new DeathTransmute(0x0e36)
                }
                ))
            .Init(0x0e32, Behaves("Davy Jones",
                new RunBehaviors(
                    Once.Instance(SetSize.Instance(0)),
                    If.Instance(IsEntityPresent.Instance(6, null), Once.Instance(new SetKey(-1, 1))),
                    IfEqual.Instance(-1, 1,
                        new RunBehaviors(
                            new QueuedBehavior(
                                SetAltTexture.Instance(4),
                                CooldownExact.Instance(125),
                                SetSize.Instance(25),
                                CooldownExact.Instance(125),
                                SetSize.Instance(50),
                                CooldownExact.Instance(125),
                                SetSize.Instance(75),
                                CooldownExact.Instance(125),
                                SetSize.Instance(100),
                                new SetKey(-1, 2)
                                ))),
                    IfEqual.Instance(-1, 2,
                        new RunBehaviors(
                            If.Instance(IsEntityNotPresent.Instance(100, 0x0e35),
                                new SetKey(-1, 3)
                                ),
                            If.Instance(IsEntityPresent.Instance(100, 0x0e35),
                                new SetKey(-1, 4)
                                )
                            ))),
                loot: new LootBehavior(LootDef.Empty,
                    Tuple.Create(100, new LootDef(0, 5, 0, 10,
                        Tuple.Create(0.01, (ILoot) new ItemLoot("Spirit Dagger")),
                        Tuple.Create(0.01, (ILoot) new ItemLoot("Spectral Cloth Armor")),
                        Tuple.Create(0.01, (ILoot) new ItemLoot("Ghostly Prism")),
                        Tuple.Create(0.99, (ILoot) new ItemLoot("Potion of Wisdom")),
                        Tuple.Create(0.01, (ILoot) new ItemLoot("Captain's Ring"))
                        ))
                    ))
            );
    }
}