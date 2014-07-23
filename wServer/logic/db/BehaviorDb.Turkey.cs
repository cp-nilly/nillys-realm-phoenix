using System.Collections.Generic;
using System;
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
        static _ Turkey = Behav()
            .Init(0x2685, Behaves("Turkey God",
                    new RunBehaviors(
                        new RunBehaviors(
                            Once.Instance(SetConditionEffect.Instance(ConditionEffectIndex.Invulnerable)), //Sets Invulnerable once at the start of it's behaviors.
                            If.Instance(IsEntityPresent.Instance(15, null), Once.Instance(new SetKey(-1, 1))) //if player is within 15 tiles, start behaviors (Once.Instance so it doesn't do it over and over).
                        ),
        #region Taunts
 IfEqual.Instance(-1, 1,
                            new RunBehaviors(
                                Flashing.Instance(250, 0xffff0000), //Basic Flashing
                                new QueuedBehavior( //All the taunts
                                    new SimpleTaunt("A long time ago. Pilgrims ate their feast."),
                                    CooldownExact.Instance(5000),
                                    new SimpleTaunt("It's called Thanksgiving, for a reason."),
                                    CooldownExact.Instance(5000),
                                    new SimpleTaunt("You give me food."),
                                    CooldownExact.Instance(5000),
                                    new SimpleTaunt("You give me pain."),
                                    CooldownExact.Instance(5000),
                                    new SimpleTaunt("Now I give you both."),
                                    CooldownExact.Instance(2000),
                                    UnsetConditionEffect.Instance(ConditionEffectIndex.Invulnerable), //Unset invulnerable before the setkey is done.
                                    new SimpleTaunt("Prepare to enjoy your dinner!"),
                                    new SetKey(-1, 2) //Go into battle after all taunts
                                    )
                                )
                            ),
        #endregion

        #region Battle
 IfEqual.Instance(-1, 2,
                            new RunBehaviors(
                                Cooldown.Instance(1000, MultiAttack.Instance(25, 10 * (float)Math.PI / 180, 3, 0, projectileIndex: 1)),
                                Cooldown.Instance(500, MultiAttack.Instance(25, 10 * (float)Math.PI / 180, 5, 0, projectileIndex: 2)),
                                Cooldown.Instance(1000, MultiAttack.Instance(25, 10 * (float)Math.PI / 180, 3, 0, projectileIndex: 3)),
                                Cooldown.Instance(500, MultiAttack.Instance(25, 10 * (float)Math.PI / 180, 4, 0, projectileIndex: 5)),
                                Cooldown.Instance(1000, MultiAttack.Instance(25, 10 * (float)Math.PI / 180, 3, 0, projectileIndex: 6)),
                                Cooldown.Instance(8000, Once.Instance(RingAttack.Instance(40, 0, 2, projectileIndex: 7))),
                                Cooldown.Instance(8000, Once.Instance(RingAttack.Instance(45, projectileIndex: 8))),
                                SpawnMinion.Instance(0x2686, 2, 9, 12000, 12000),
                                Once.Instance(SpawnMinionImmediate.Instance(0x2687, 7, 2, 3)),
                    new RunBehaviors(
                        new State("idle",
                        HpGreaterEqual.Instance(15000,
                            new RunBehaviors(
                                MaintainDist.Instance(1, 5, 15, null),
                                Cooldown.Instance(3600, MultiAttack.Instance(25, 45 * (float)Math.PI / 180, 10, 0, projectileIndex: 1)),
                                Cooldown.Instance(2000, PredictiveMultiAttack.Instance(25, 10 * (float)Math.PI / 180, 3, 5, projectileIndex: 1)),
                                SpawnMinion.Instance(0x2686, 2, 9, 12000, 12000),
                                Once.Instance(SpawnMinionImmediate.Instance(0x2687, 7, 2, 3)),
                                Cooldown.Instance(8000, Once.Instance(RingAttack.Instance(40, 0, 2, projectileIndex: 7))),
                                Cooldown.Instance(8000, Once.Instance(RingAttack.Instance(45, projectileIndex: 8)))
                            )
                        ),
                        HpLesserPercent.Instance(0.2f,
                            new RunBehaviors(
                                Chasing.Instance(3, 25, 2, null),
                                Cooldown.Instance(2200, MultiAttack.Instance(25, 45 * (float)Math.PI / 180, 8, 0, projectileIndex: 1)),
                                Once.Instance(SetConditionEffect.Instance(ConditionEffectIndex.Invulnerable)),
                                Cooldown.Instance(8000, Once.Instance(UnsetConditionEffect.Instance(ConditionEffectIndex.Invulnerable))),
                                Cooldown.Instance(8000, Once.Instance(RingAttack.Instance(40, 0, 2, projectileIndex: 7))),
                                Cooldown.Instance(8000, Once.Instance(RingAttack.Instance(45, projectileIndex: 8)))
                            )
                        )
                        )
                    ),
                                Cooldown.Instance(20000, new SimpleTaunt("Happy Thanksgiving, Eat my meat! Only {HP} more mutton chunks for you to eat!"))
                            )
                        )
                    ),
        #endregion

        #region Loot
 loot: new LootBehavior(LootDef.Empty,
                        Tuple.Create(100, new LootDef(0, 5, 0, 8,
                            Tuple.Create(0.07, (ILoot)new TierLoot(6, ItemType.Ability)),
                            Tuple.Create(0.07, (ILoot)new TierLoot(13, ItemType.Armor)),
                            Tuple.Create(0.07, (ILoot)new TierLoot(12, ItemType.Weapon)),
                            Tuple.Create(0.2, (ILoot)new TierLoot(5, ItemType.Ring)),
                            Tuple.Create(0.01, (ILoot)new ItemLoot("America Sword")),
                            Tuple.Create(0.01, (ILoot)new ItemLoot("America Armor")),
                            Tuple.Create(0.01, (ILoot)new ItemLoot("America Shield")),
                            Tuple.Create(0.01, (ILoot)new ItemLoot("America Ring")),
                            Tuple.Create(0.05, (ILoot)new ItemLoot("Turkey Leg of Doom")),
                            Tuple.Create(0.2, (ILoot)new StatPotionLoot(StatPotion.Att)),
                            Tuple.Create(0.2, (ILoot)new StatPotionLoot(StatPotion.Wis)),
                            Tuple.Create(0.2, (ILoot)new StatPotionLoot(StatPotion.Vit)),
                            Tuple.Create(0.2, (ILoot)new StatPotionLoot(StatPotion.Spd))
                    )))
        #endregion
                
))
    .Init(0x2687, Behaves("Turkey God Minion",
            new RunBehaviors(
                     Chasing.Instance(12, 10, 1, null),
                     Once.Instance(new SimpleTaunt("Gobble, Gobble")
                        )
                        ),
                        loot: new LootBehavior(LootDef.Empty,
                        Tuple.Create(360, new LootDef(0, 2, 0, 2,
                        Tuple.Create(0.09, (ILoot)HpPotionLoot.Instance),
                        Tuple.Create(0.09, (ILoot)MpPotionLoot.Instance)
                    )))
                    ))
             .Init(0x2686, Behaves("Pilgrim",
             new RunBehaviors(
                 Chasing.Instance(12, 10, 1, null),
                 Cooldown.Instance(400, MultiAttack.Instance(25, 10 * (float)Math.PI / 180, 1, 0, projectileIndex: 0)),
                 Once.Instance(new SimpleTaunt("This is our Turkey! Stay away from our feast!")
                 ))));

    }

}
