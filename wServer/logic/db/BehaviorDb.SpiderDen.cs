#region

using System;
using wServer.logic.attack;
using wServer.logic.loot;
using wServer.logic.movement;

#endregion

namespace wServer.logic
{
    partial class BehaviorDb
    {
        private static _ SpiderDen = Behav()
            .Init(0x0936, Behaves("Arachna the Spider Queen",
                new RunBehaviors(
                    Cooldown.Instance(5000, ReturnSpawn.Instance(3)),
                    If.Instance(IsEntityPresent.Instance(10, null), Once.Instance(new SetKey(-1, 1))),
                    IfEqual.Instance(-1, 1,
                        new QueuedBehavior(
                            Once.Instance(SetConditionEffect.Instance(ConditionEffectIndex.Invulnerable)),
                            Once.Instance(TossEnemy.Instance(0*(float) Math.PI/180, 6, 0x093d)),
                            Once.Instance(TossEnemy.Instance(120*(float) Math.PI/180, 6, 0x093e)),
                            Once.Instance(TossEnemy.Instance(240*(float) Math.PI/180, 6, 0x093f)),
                            Once.Instance(TossEnemy.Instance(0*(float) Math.PI/180, 10, 0x0937)),
                            Once.Instance(TossEnemy.Instance(60*(float) Math.PI/180, 10, 0x0938)),
                            Once.Instance(TossEnemy.Instance(120*(float) Math.PI/180, 10, 0x0939)),
                            Once.Instance(TossEnemy.Instance(180*(float) Math.PI/180, 10, 0x093a)),
                            Once.Instance(TossEnemy.Instance(240*(float) Math.PI/180, 10, 0x093b)),
                            Once.Instance(TossEnemy.Instance(300*(float) Math.PI/180, 10, 0x093c)),
                            Cooldown.Instance(3000),
                            Once.Instance(UnsetConditionEffect.Instance(ConditionEffectIndex.Invulnerable)),
                            new SetKey(-1, 2),
                            Cooldown.Instance(5000)
                            )
                        ),
                    IfEqual.Instance(-1, 2,
                        new RunBehaviors(
                            SimpleWandering.Instance(4, 3f),
                            Cooldown.Instance(3000, RingAttack.Instance(12, 0, 0, projectileIndex: 0)),
                            Cooldown.Instance(1000, SimpleAttack.Instance(10, projectileIndex: 0)),
                            Cooldown.Instance(2000, SimpleAttack.Instance(10, 1))
                            )
                        )
                    ),
                loot: new LootBehavior(LootDef.Empty,
                    Tuple.Create(100, new LootDef(0, 5, 0, 10,
                        Tuple.Create(0.2, (ILoot) new ItemLoot("Poison Fang Dagger")),
                        Tuple.Create(0.8, (ILoot) new ItemLoot("Golden Dagger"))
                        )))))
            .Init(0x0937, Behaves("Arachna Web Spoke 1",
                new RunBehaviors(
                    Once.Instance(SetConditionEffect.Instance(ConditionEffectIndex.Invulnerable)),
                    Cooldown.Instance(150, AngleAttack.Instance(180*(float) Math.PI/180, projectileIndex: 0)),
                    Cooldown.Instance(150, AngleAttack.Instance(120*(float) Math.PI/180, projectileIndex: 0)),
                    Cooldown.Instance(150, AngleAttack.Instance(240*(float) Math.PI/180, projectileIndex: 0))
                    )
                ))
            .Init(0x0938, Behaves("Arachna Web Spoke 2",
                new RunBehaviors(
                    Once.Instance(SetConditionEffect.Instance(ConditionEffectIndex.Invulnerable)),
                    Cooldown.Instance(150, AngleAttack.Instance(240*(float) Math.PI/180, projectileIndex: 0)),
                    Cooldown.Instance(150, AngleAttack.Instance(180*(float) Math.PI/180, projectileIndex: 0)),
                    Cooldown.Instance(150, AngleAttack.Instance(300*(float) Math.PI/180, projectileIndex: 0))
                    )
                ))
            .Init(0x0939, Behaves("Arachna Web Spoke 3",
                new RunBehaviors(
                    Once.Instance(SetConditionEffect.Instance(ConditionEffectIndex.Invulnerable)),
                    Cooldown.Instance(150, AngleAttack.Instance(300*(float) Math.PI/180, projectileIndex: 0)),
                    Cooldown.Instance(150, AngleAttack.Instance(240*(float) Math.PI/180, projectileIndex: 0)),
                    Cooldown.Instance(150, AngleAttack.Instance(0*(float) Math.PI/180, projectileIndex: 0))
                    )
                ))
            .Init(0x093a, Behaves("Arachna Web Spoke 4",
                new RunBehaviors(
                    Once.Instance(SetConditionEffect.Instance(ConditionEffectIndex.Invulnerable)),
                    Cooldown.Instance(150, AngleAttack.Instance(0*(float) Math.PI/180, projectileIndex: 0)),
                    Cooldown.Instance(150, AngleAttack.Instance(60*(float) Math.PI/180, projectileIndex: 0)),
                    Cooldown.Instance(150, AngleAttack.Instance(300*(float) Math.PI/180, projectileIndex: 0))
                    )
                ))
            .Init(0x093b, Behaves("Arachna Web Spoke 5",
                new RunBehaviors(
                    Once.Instance(SetConditionEffect.Instance(ConditionEffectIndex.Invulnerable)),
                    Cooldown.Instance(150, AngleAttack.Instance(60*(float) Math.PI/180, projectileIndex: 0)),
                    Cooldown.Instance(150, AngleAttack.Instance(0*(float) Math.PI/180, projectileIndex: 0)),
                    Cooldown.Instance(150, AngleAttack.Instance(120*(float) Math.PI/180, projectileIndex: 0))
                    )
                ))
            .Init(0x093c, Behaves("Arachna Web Spoke 6",
                new RunBehaviors(
                    Once.Instance(SetConditionEffect.Instance(ConditionEffectIndex.Invulnerable)),
                    Cooldown.Instance(150, AngleAttack.Instance(120*(float) Math.PI/180, projectileIndex: 0)),
                    Cooldown.Instance(150, AngleAttack.Instance(60*(float) Math.PI/180, projectileIndex: 0)),
                    Cooldown.Instance(150, AngleAttack.Instance(180*(float) Math.PI/180, projectileIndex: 0))
                    )
                ))
            .Init(0x93d, Behaves("Arachna Web Spoke 7",
                new RunBehaviors(
                    Once.Instance(SetConditionEffect.Instance(ConditionEffectIndex.Invulnerable)),
                    Cooldown.Instance(150, AngleAttack.Instance(180*(float) Math.PI/180, projectileIndex: 0)),
                    Cooldown.Instance(150, AngleAttack.Instance(120*(float) Math.PI/180, projectileIndex: 0)),
                    Cooldown.Instance(150, AngleAttack.Instance(240*(float) Math.PI/180, projectileIndex: 0))
                    )
                ))
            .Init(0x093e, Behaves("Arachna Web Spoke 8",
                new RunBehaviors(
                    Once.Instance(SetConditionEffect.Instance(ConditionEffectIndex.Invulnerable)),
                    Cooldown.Instance(150, AngleAttack.Instance(360*(float) Math.PI/180, projectileIndex: 0)),
                    Cooldown.Instance(150, AngleAttack.Instance(240*(float) Math.PI/180, projectileIndex: 0)),
                    Cooldown.Instance(150, AngleAttack.Instance(300*(float) Math.PI/180, projectileIndex: 0))
                    )))
            .Init(0x093f, Behaves("Arachna Web Spoke 9",
                new RunBehaviors(
                    Once.Instance(SetConditionEffect.Instance(ConditionEffectIndex.Invulnerable)),
                    Cooldown.Instance(150, AngleAttack.Instance(0*(float) Math.PI/180, projectileIndex: 0)),
                    Cooldown.Instance(150, AngleAttack.Instance(60*(float) Math.PI/180, projectileIndex: 0)),
                    Cooldown.Instance(150, AngleAttack.Instance(120*(float) Math.PI/180, projectileIndex: 0))
                    )))
            .Init(0x021c, Behaves("Green Den Spider Hatchling",
                new RunBehaviors(
                    SimpleWandering.Instance(2, 2f),
                    Cooldown.Instance(1000, SimpleAttack.Instance(10, projectileIndex: 0))
                    )))
            .Init(0x021d, Behaves("Black Den Spider",
                new RunBehaviors(
                    SimpleWandering.Instance(4, 3f),
                    Cooldown.Instance(1000, SimpleAttack.Instance(10, projectileIndex: 0))
                    )))
            .Init(0x021e, Behaves("Red Spotted Den Spider",
                new RunBehaviors(
                    SimpleWandering.Instance(4, 3f),
                    Cooldown.Instance(1000, SimpleAttack.Instance(10, projectileIndex: 0))
                    ),
                loot: new LootBehavior(LootDef.Empty,
                    Tuple.Create(100, new LootDef(0, 5, 0, 10,
                        Tuple.Create(0.2, (ILoot) new ItemLoot("Healing Ichor"))
                        ))))
            )
            .Init(0x021f, Behaves("Black Spotted Den Spider",
                new RunBehaviors(
                    SimpleWandering.Instance(4, 2f),
                    Cooldown.Instance(1000, SimpleAttack.Instance(10, projectileIndex: 0))
                    ),
                loot: new LootBehavior(LootDef.Empty,
                    Tuple.Create(100, new LootDef(0, 5, 0, 10,
                        Tuple.Create(0.2, (ILoot) new ItemLoot("Healing Ichor"))
                        ))))
            )
            .Init(0x0220, Behaves("Brown Den Spider",
                new RunBehaviors(
                    SimpleWandering.Instance(4, 3f),
                    Cooldown.Instance(1000, MultiAttack.Instance(10, 0, 3, 0, projectileIndex: 0))
                    ),
                loot: new LootBehavior(LootDef.Empty,
                    Tuple.Create(100, new LootDef(0, 5, 0, 10,
                        Tuple.Create(0.6, (ILoot) new ItemLoot("Healing Ichor"))
                        ))))
            )
            .Init(0x021b, Behaves("Spider Egg Sac",
                condBehaviors: new ConditionalBehavior[]
                {
                    new DeathTransmute(0x021c, 3, 5)
                }
                ));
    }
}