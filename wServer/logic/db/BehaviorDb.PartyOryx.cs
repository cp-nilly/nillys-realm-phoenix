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
        private static _ Oryxb = Behav()
            .Init(0x7001, Behaves("Oryx the Birthday God",
                new RunBehaviors(
                    new State("idle",
                        Cooldown.Instance(700,
                            MultiAttack.Instance(25, 45*(float) Math.PI/180, 8, 0, projectileIndex: 0)),
                        Cooldown.Instance(6000, new SimpleTaunt("YOU WILL NOT HAVE MY PRESENTS!!"))
                        )
                    ),
                HpLesserPercent.Instance(0.2f,
                    new RunBehaviors(
                        Chasing.Instance(3, 25, 2, null),
                        Cooldown.Instance(2200,
                            MultiAttack.Instance(25, 45*(float) Math.PI/180, 8, 0, projectileIndex: 0)),
                        Once.Instance(new SimpleTaunt("WHY CAN'T I HAVE A PARTY IN PEACE?!?!?")),
                        Cooldown.Instance(8000,
                            Once.Instance(UnsetConditionEffect.Instance(ConditionEffectIndex.Invulnerable)))
                        )
                    ),
                new RunBehaviors(
                    new State("dance",
                        Once.Instance(new SimpleTaunt("I HAAAAATE CAKE!")),
                        new QueuedBehavior(
                            Cooldown.Instance(500, RingAttack.Instance(5, 0, 2, projectileIndex: 0)),
                            Cooldown.Instance(500, RingAttack.Instance(6, 0, 2, 1)),
                            Cooldown.Instance(500, RingAttack.Instance(6, 0, 2, 2)),
                            Cooldown.Instance(500, RingAttack.Instance(6, 0, 2, 3))
                            )
                        )), new LootBehavior(LootDef.Empty,
                            Tuple.Create(100, new LootDef(0, 5, 0, 10,
                                Tuple.Create(0.20, (ILoot) new ItemLoot("Slice of Cake")),
                                Tuple.Create(0.20, (ILoot) new ItemLoot("Large Cake")),
                                Tuple.Create(0.20, (ILoot) new ItemLoot("Large Chocolate Cake"))
                                ))), new ConditionalBehavior[]
                                {
                                    new ChatEvent(
                                        SetState.Instance("dance")
                                        ).SetChats("Happy birthday Oryx! I heard you like cake!"),
                                    new OnDeath(new RunBehaviors(
                                        Once.Instance(TossEnemy.Instance(24*(float) Math.PI/180, 9, 0x702a)),
                                        Once.Instance(TossEnemy.Instance(48*(float) Math.PI/180, 9, 0x702b)),
                                        Once.Instance(TossEnemy.Instance(72*(float) Math.PI/180, 9, 0x702c)),
                                        Once.Instance(TossEnemy.Instance(96*(float) Math.PI/180, 9, 0x702a)),
                                        Once.Instance(TossEnemy.Instance(120*(float) Math.PI/180, 9, 0x702b)),
                                        Once.Instance(TossEnemy.Instance(144*(float) Math.PI/180, 9, 0x702c)),
                                        Once.Instance(TossEnemy.Instance(168*(float) Math.PI/180, 9, 0x702a)),
                                        Once.Instance(TossEnemy.Instance(192*(float) Math.PI/180, 9, 0x702b)),
                                        Once.Instance(TossEnemy.Instance(216*(float) Math.PI/180, 9, 0x702c)),
                                        Once.Instance(TossEnemy.Instance(240*(float) Math.PI/180, 9, 0x702a)),
                                        Once.Instance(TossEnemy.Instance(264*(float) Math.PI/180, 9, 0x702b)),
                                        Once.Instance(TossEnemy.Instance(288*(float) Math.PI/180, 9, 0x702c)),
                                        Once.Instance(TossEnemy.Instance(312*(float) Math.PI/180, 9, 0x702a)),
                                        Once.Instance(TossEnemy.Instance(336*(float) Math.PI/180, 9, 0x702b)),
                                        Once.Instance(TossEnemy.Instance(360*(float) Math.PI/180, 9, 0x702c))
                                        ))
                                }
                ))
            .Init(0x702a, Behaves("Red Balloon",
                new RunBehaviors(SimpleWandering.Instance(1, 1f)),
                loot: new LootBehavior(LootDef.Empty,
                    Tuple.Create(100, new LootDef(0, 5, 0, 10,
                        Tuple.Create(0.010, (ILoot) new ItemLoot("Party Balloons")),
                        Tuple.Create(0.010, (ILoot) new ItemLoot("Birthday Suit")),
                        Tuple.Create(0.009, (ILoot) new ItemLoot("Disco Lights")),
                        Tuple.Create(0.010, (ILoot) new ItemLoot("Party Hat"))
                        )))
                ))
            .Init(0x702b, Behaves("Blue Balloon",
                new RunBehaviors(SimpleWandering.Instance(1, 1f)),
                loot: new LootBehavior(LootDef.Empty,
                    Tuple.Create(100, new LootDef(0, 5, 0, 10,
                        Tuple.Create(0.010, (ILoot) new ItemLoot("Party Balloons")),
                        Tuple.Create(0.010, (ILoot) new ItemLoot("Birthday Suit")),
                        Tuple.Create(0.009, (ILoot) new ItemLoot("Disco Lights")),
                        Tuple.Create(0.010, (ILoot) new ItemLoot("Party Hat"))
                        )))
                ))
            .Init(0x702c, Behaves("Green Balloon",
                new RunBehaviors(SimpleWandering.Instance(1, 1f)),
                loot: new LootBehavior(LootDef.Empty,
                    Tuple.Create(100, new LootDef(0, 5, 0, 10,
                        Tuple.Create(0.010, (ILoot) new ItemLoot("Party Balloons")),
                        Tuple.Create(0.010, (ILoot) new ItemLoot("Birthday Suit")),
                        Tuple.Create(0.009, (ILoot) new ItemLoot("Disco Lights")),
                        Tuple.Create(0.010, (ILoot) new ItemLoot("Party Hat"))
                        )))
                ));
    }
}