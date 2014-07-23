using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using wServer.realm;
using wServer.logic.attack;
using wServer.logic.movement;
using wServer.logic.loot;
using wServer.logic.taunt;
using wServer.logic.cond;

namespace wServer.logic
{
    partial class BehaviorDb
    {
        static _ FrozenDungeon = Behav()
            .Init(0x757b, Behaves("Ice Lich",
            new RunBehaviors(
            Once.Instance(new SetKey(-1, 1)),

            IfEqual.Instance(-1, 1,
                 new RunBehaviors(
                    CooldownExact.Instance(4000, RingAttack.Instance(8, 0, 0, 0)),
                    Once.Instance(new SimpleTaunt("You have killed my loyal minions and you have broken into my Ice Palace! Prepare to die!")),
                    new QueuedBehavior(
                        Cooldown.Instance(8000),
                        new SetKey(-1, 2)
                        ))),

                    IfEqual.Instance(-1, 2,
                   new RunBehaviors(
                        Chasing.Instance(7, 20, 3, null),
                        SetConditionEffect.Instance(ConditionEffectIndex.Armored),
                        Cooldown.Instance(1500, MultiAttack.Instance(20, 10 * (float)Math.PI / 180, 4, 0, projectileIndex: 0)),
                        HpLesserPercent.Instance(0.5f, UnsetConditionEffect.Instance(ConditionEffectIndex.Armored)),
                        new QueuedBehavior(HpLesserPercent.Instance(0.8f, new SetKey(-1, 3)))
                          )),

                        IfEqual.Instance(-1, 3,
                        new RunBehaviors(
                            SetConditionEffect.Instance(ConditionEffectIndex.Invincible),
                            SetConditionEffect.Instance(ConditionEffectIndex.Paralyzed),
                            Once.Instance(new SimpleTaunt("You may have hurt me, but now you will die!")),
                            CooldownExact.Instance(2000, new SimpleTaunt("Minions, kill them!")),
                            Cooldown.Instance(3000, UnsetConditionEffect.Instance(ConditionEffectIndex.Invincible)),
                            Cooldown.Instance(3000, UnsetConditionEffect.Instance(ConditionEffectIndex.Paralyzed)),
                            new QueuedBehavior(Cooldown.Instance(3000, new SetKey(-1, 4)))
                            )),

                            IfEqual.Instance(-1, 4,
                            new RunBehaviors(
                                SetConditionEffect.Instance(ConditionEffectIndex.Invincible),
                                SetConditionEffect.Instance(ConditionEffectIndex.Paralyzed),
                                SpawnMinion.Instance(0x196a, 2, 10, 1000, 1000),
                                Cooldown.Instance(20000, UnsetConditionEffect.Instance(ConditionEffectIndex.Invincible)),
                                Cooldown.Instance(20000, UnsetConditionEffect.Instance(ConditionEffectIndex.Paralyzed)),
                                new QueuedBehavior(Cooldown.Instance(20000, new SetKey(-1, 5)))
                            )
                            ),

                            IfEqual.Instance(-1, 5,
                            new RunBehaviors(
                                Chasing.Instance(12, 20, 1, null),
                                Cooldown.Instance(1000, RingAttack.Instance(16, 0, 0, 1)),
                                new QueuedBehavior(Cooldown.Instance(10000, new SetKey(-1, 6)))
                                )
                            ),
                            IfEqual.Instance(-1, 6,
                            new RunBehaviors(
                                Cooldown.Instance(500, RingAttack.Instance(15, 0, 0, 2)),
                                new QueuedBehavior(HpLesserPercent.Instance(0.3f, new SetKey(-1, 7)))
                                )
                                ),

                                IfEqual.Instance(-1, 7,
                                new RunBehaviors(
                                    Chasing.Instance(12, 20, 1, null),
                                    Once.Instance(new SimpleTaunt("You Mongrals! You May have hurt me, but now im mad!")),
                                    Cooldown.Instance(750, MultiAttack.Instance(20, 15 * (float)Math.PI / 180, 8, 0, 0)),
                                    new QueuedBehavior(HpLesserPercent.Instance(0.07f, new SetKey(-1, 8)))
                                    )
                                    ),

                                    IfEqual.Instance(-1, 8,
                                    new RunBehaviors(
                                        Once.Instance(new SimpleTaunt("I have failed my kingdom, you fools do not know what you just did!"))
                                        )
                                        )

                        )
                        ))

            .Init(0x197a, Behaves("Ice Skeleton Warrior",
            new RunBehaviors(
                Chasing.Instance(10, 20, 1, null),
                Cooldown.Instance(750, MultiAttack.Instance(25, 0, 1, 0, 0))
                ),
                loot: new LootBehavior(LootDef.Empty,
                    Tuple.Create(800, new LootDef(0, 5, 1, 2,
                        Tuple.Create(0.01, (ILoot)new TierLoot(8, ItemType.Weapon)),
                        Tuple.Create(0.01, (ILoot)new TierLoot(9, ItemType.Armor)),
                        Tuple.Create(0.01, (ILoot)new TierLoot(4, ItemType.Ability))
                )))
                ))




            .Init(0x196a, Behaves("Ice Skeleton Archer",
            new RunBehaviors(
                SimpleWandering.Instance(3, 4),
                Cooldown.Instance(2200, MultiAttack.Instance(25, 10 * (float)Math.PI / 180, 3, 0, projectileIndex: 0))
                ),
                loot: new LootBehavior(LootDef.Empty,
                        Tuple.Create(800, new LootDef(0, 5, 1, 2,
                            Tuple.Create(0.01, (ILoot)new TierLoot(8, ItemType.Weapon)),
                            Tuple.Create(0.01, (ILoot)new TierLoot(9, ItemType.Armor)),
                            Tuple.Create(0.01, (ILoot)new TierLoot(4, ItemType.Ability))
                            )))));

    }
}

