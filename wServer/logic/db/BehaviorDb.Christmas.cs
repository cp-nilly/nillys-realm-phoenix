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
        private static _ ChristmasOryx = Behav()
            .Init(0x269e, Behaves("Oryx the Christmas God",
                new RunBehaviors(
                    new State("idle",
                        Cooldown.Instance(1000, MultiAttack.Instance(25, 10 * (float)Math.PI / 180, 3, 0, 1)),
                        Cooldown.Instance(8000, Once.Instance(RingAttack.Instance(30, projectileIndex: 0))),
                        Cooldown.Instance(1000, InfiniteSpiralAttack.Instance(25, 5, 5, 1)),
                        Cooldown.Instance(6000, new SimpleTaunt("Merry christmas! Eat my {HP} Candy Canes!"))
                        )
                    ),
                new RunBehaviors(
                    new State("idle",
                        HpGreaterEqual.Instance(15000,
                            new RunBehaviors(
                                MaintainDist.Instance(1, 5, 15, null),
                                Cooldown.Instance(3600,
                                    MultiAttack.Instance(25, 45 * (float)Math.PI / 180, 8, 0, projectileIndex: 0))
                                )
                            ),
                        HpLesserPercent.Instance(0.2f,
                            new RunBehaviors(
                                Chasing.Instance(3, 25, 2, null),
                                Cooldown.Instance(2200,
                                    MultiAttack.Instance(25, 45 * (float)Math.PI / 180, 8, 0, projectileIndex: 0)),
                                Once.Instance(SetConditionEffect.Instance(ConditionEffectIndex.Invulnerable)),
                                Once.Instance(new SimpleTaunt("I must protect the Sleigh!")),
                                Cooldown.Instance(8000,
                                    Once.Instance(UnsetConditionEffect.Instance(ConditionEffectIndex.Invulnerable))),
                                Cooldown.Instance(8000, Once.Instance(RingAttack.Instance(30, 0, 2, 0))),
                                Cooldown.Instance(8000, Once.Instance(RingAttack.Instance(30, projectileIndex: 0)))
                                )
                            )
                        )
                    ),
                  loot: new LootBehavior(LootDef.Empty,
                        Tuple.Create(100, new LootDef(0, 5, 0, 15,
                            Tuple.Create(0.0095, (ILoot)new ItemLoot("Christmas Blade")),
                            Tuple.Create(0.0095, (ILoot)new ItemLoot("Tree Topper Star")),
                            Tuple.Create(0.0095, (ILoot)new ItemLoot("Reindeer Skin Armor")),
                            Tuple.Create(0.0095, (ILoot)new ItemLoot("Santa Hat")),
                            Tuple.Create(0.0095, (ILoot)new ItemLoot("Christmas Miracle")),
                            Tuple.Create(0.07, (ILoot)new TierLoot(5, ItemType.Ability)),
                            Tuple.Create(0.05, (ILoot)new TierLoot(6, ItemType.Ability)),
                            Tuple.Create(0.07, (ILoot)new TierLoot(11, ItemType.Armor)),
                            Tuple.Create(0.06, (ILoot)new TierLoot(12, ItemType.Armor)),
                            Tuple.Create(0.05, (ILoot)new TierLoot(13, ItemType.Armor)),
                            Tuple.Create(0.07, (ILoot)new TierLoot(10, ItemType.Weapon)),
                            Tuple.Create(0.06, (ILoot)new TierLoot(11, ItemType.Weapon)),
                            Tuple.Create(0.05, (ILoot)new TierLoot(12, ItemType.Weapon)),
                            Tuple.Create(0.05, (ILoot)new TierLoot(5, ItemType.Ring)),
                            Tuple.Create(0.4, (ILoot)new StatPotionLoot(StatPotion.Att)),
                            Tuple.Create(0.4, (ILoot)new StatPotionLoot(StatPotion.Wis)),
                            Tuple.Create(0.4, (ILoot)new StatPotionLoot(StatPotion.Vit)),
                            Tuple.Create(0.4, (ILoot)new StatPotionLoot(StatPotion.Spd))
                            )),
                        Tuple.Create(100, new LootDef(0, 5, 0, 15,
                            Tuple.Create(1.00, (ILoot)new ItemLoot("Christmas Popper"))
                            )))
                ));
                }}
