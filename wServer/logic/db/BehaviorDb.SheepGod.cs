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
        private static _ SheepGod = Behav()
            .Init(0x997, Behaves("Sheep God",
                HpGreaterEqual.Instance(50000,
                    new RunBehaviors(
                        Cooldown.Instance(500, RingAttack.Instance(10, 0, 5, projectileIndex: 0)),
                        Cooldown.Instance(500, RingAttack.Instance(8, 0, 5, 1)),
                        Cooldown.Instance(1000,
                            If.Instance(
                                EntityGroupLesserThan.Instance(20, 4, "Bigsheep"),
                                Rand.Instance(
                                    TossEnemy.Instance(0, 5, 0x996)
                                    )
                                )
                            )
                        )),
                If.Instance(
                    And.Instance(HpLesser.Instance(50000, NullBehavior.Instance),
                        HpGreaterEqual.Instance(25000, NullBehavior.Instance)),
                    new RunBehaviors(
                        Cooldown.Instance(15000, RingAttack.Instance(20, 0, 5, 2)),
                        Cooldown.Instance(1000, ThrowAttack.Instance(4, 10, 125)),
                        Cooldown.Instance(1000, MultiAttack.Instance(25, 10*(float) Math.PI/180, 3, 0, 3)),
                        Cooldown.Instance(1000,
                            If.Instance(
                                EntityGroupLesserThan.Instance(20, 4, "Bigsheep"),
                                Rand.Instance(
                                    TossEnemy.Instance(0, 5, 0x996)
                                    ))))),
                HpLesser.Instance(25000, new RunBehaviors(
                    Flashing.Instance(5000, 0xffff3333),
                    Cooldown.Instance(15000, RingAttack.Instance(30, 0, 5, 2)),
                    Cooldown.Instance(500, ThrowAttack.Instance(4, 10, 100)),
                    Cooldown.Instance(500, RingAttack.Instance(10, 0, 5, projectileIndex: 0)),
                    Cooldown.Instance(500, RingAttack.Instance(8, 0, 5, 1)),
                    OrderAllEntity.Instance(20, 0x996, new Transmute(0x0993)
                        ),
                    OrderAllEntity.Instance(20, 0x995, new Transmute(0x0994)
                        ))), new LootBehavior(LootDef.Empty,
                            Tuple.Create(100, new LootDef(0, 3, 0, 8,
                                Tuple.Create(0.001, (ILoot) new ItemLoot("Orb of Conflict")),
                                Tuple.Create(0.01, (ILoot) new TierLoot(11, ItemType.Weapon)),
                                Tuple.Create(0.01, (ILoot) new TierLoot(11, ItemType.Armor)),
                                Tuple.Create(0.01, (ILoot) new TierLoot(5, ItemType.Ring)),
                                Tuple.Create(0.02, (ILoot) new TierLoot(10, ItemType.Weapon)),
                                Tuple.Create(0.02, (ILoot) new TierLoot(10, ItemType.Armor)),
                                Tuple.Create(0.03, (ILoot) new TierLoot(9, ItemType.Weapon)),
                                Tuple.Create(0.03, (ILoot) new TierLoot(5, ItemType.Ability)),
                                Tuple.Create(0.03, (ILoot) new TierLoot(9, ItemType.Armor)),
                                Tuple.Create(1.0, (ILoot) new StatPotionsLoot(1, 2)),
                                Tuple.Create(0.05, (ILoot) new TierLoot(4, ItemType.Ring)),
                                Tuple.Create(0.1, (ILoot) new TierLoot(4, ItemType.Ability)),
                                Tuple.Create(0.1, (ILoot) new TierLoot(8, ItemType.Armor)),
                                Tuple.Create(0.2, (ILoot) new TierLoot(8, ItemType.Weapon)),
                                Tuple.Create(0.2, (ILoot) new TierLoot(7, ItemType.Armor)),
                                Tuple.Create(0.2, (ILoot) new TierLoot(3, ItemType.Ring))
                                ))
                            )
                ))
            .Init(0x996, Behaves("Giant Sheep",
                new RunBehaviors(
                    StrictCircling.Instance(5, 3, 0x997),
                    Cooldown.Instance(5000, SimpleAttack.Instance(5, projectileIndex: 0)),
                    Cooldown.Instance(1000, RingAttack.Instance(8, 0, 0, 1)),
                    Cooldown.Instance(100,
                        If.Instance(
                            EntityGroupLesserThan.Instance(3, 5, "Smallsheep"),
                            SpawnMinionImmediate.Instance(0x995, 1, 1, 1))
                        )
                    ),
                loot: new LootBehavior(
                    new LootDef(0, 1, 0, 8,
                        Tuple.Create(0.01, (ILoot) PotionLoot.Instance)
                        ))
                )
            )
            .Init(0x995, Behaves("Evil Sheep",
                new RunBehaviors(
                    StrictCircling.Instance(1, 5, 0x996),
                    Cooldown.Instance(3000, SimpleAttack.Instance(5, projectileIndex: 0))
                    ),
                loot: new LootBehavior(
                    new LootDef(0, 1, 0, 8,
                        Tuple.Create(0.01, (ILoot) PotionLoot.Instance)
                        ))
                )
            )
            .Init(0x994, Behaves("Enraged Evil Sheep",
                new RunBehaviors(
                    Chasing.Instance(8, 12, 1, null),
                    Flashing.Instance(500, 0xffff3333),
                    Cooldown.Instance(2000, MultiAttack.Instance(25, 10*(float) Math.PI/180, 1, 0, projectileIndex: 0))
                    ),
                loot: new LootBehavior(
                    new LootDef(0, 1, 0, 8,
                        Tuple.Create(0.01, (ILoot) PotionLoot.Instance)))))
            .Init(0x993, Behaves("Enraged Giant Sheep",
                new RunBehaviors(
                    Chasing.Instance(6, 12, 1, null),
                    Flashing.Instance(500, 0xffff3333),
                    Cooldown.Instance(5000, MultiAttack.Instance(25, 10*(float) Math.PI/180, 1, 0, projectileIndex: 0)
                        ))))
            ;
    }
}