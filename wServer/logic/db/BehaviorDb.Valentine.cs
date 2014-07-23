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
        static _ Valentine = Behav()
            .Init(0x3700, Behaves("Oryx The Loving God",
    new RunBehaviors(
        SmoothWandering.Instance(0.4f, 1),
                IsEntityPresent.Instance(30, null), Once.Instance(new SetKey(-1, 1)),
                 //create key 1
                //Run keys:

            //behaviors to run in key 1
               IfEqual.Instance(-1, 1,
                    new QueuedBehavior(
                          Once.Instance(SetConditionEffect.Instance(ConditionEffectIndex.Invulnerable)),
                          Cooldown.Instance(2000, new SimpleTaunt("Oh look, Mortals. It must be that time again")),
                          Cooldown.Instance(2000, new SimpleTaunt("You raid me for my children.")),
                          Cooldown.Instance(2000, new SimpleTaunt("But not this year.")),
                          Cooldown.Instance(2000, new SimpleTaunt("I will rip your hearts out now")),
                          Cooldown.Instance(2000, new SimpleTaunt("ONE BY ONE")),
                          Cooldown.Instance(2000, new SimpleTaunt("NOW GIVE ME YOUR HEARTS")),
                          UnsetConditionEffect.Instance(ConditionEffectIndex.Invulnerable),
                          new SetKey(-1, 2)
                          )),
                //key 2
                IfEqual.Instance(-1, 2,
                      new RunBehaviors(
                        MaintainDist.Instance(1, 5, 15, null),                       
                        Cooldown.Instance(1000, MultiAttack.Instance(25, 45 * (float)Math.PI / 180, 8, 0, projectileIndex: 0)),
                        Cooldown.Instance(1000, MultiAttack.Instance(25, 45 * (float)Math.PI / 180, 5, 0, projectileIndex: 0)),
                        Cooldown.Instance(1000, MultiAttack.Instance(25, 45 * (float)Math.PI / 180, 3, 0, projectileIndex: 1)),
                        Cooldown.Instance(1000, MultiAttack.Instance(25, 45 * (float)Math.PI / 180, 2, 0, projectileIndex: 2)),
                        Cooldown.Instance(4500, RingAttack.Instance(30, projectileIndex: 2)),
                        Chasing.Instance(3, 25, 2, null)
                        )),

                       
                        

                                    If.Instance(HpLesserPercent.Instance(0.5f, new SetKey(-1, 3)),
                                    IfEqual.Instance(-1, 3, new RunBehaviors(
                                     new QueuedBehavior(
                                     Once.Instance(SetConditionEffect.Instance(ConditionEffectIndex.Invulnerable)),
                                     Timed.Instance(7000, Flashing.Instance(200, 0xf389E13)),
                                     Cooldown.Instance(1000,new SimpleTaunt("You")),
                                     Cooldown.Instance(1000,new SimpleTaunt("WONT")),
                                     Cooldown.Instance(1000,new SimpleTaunt("KILL ME!")),
                                     Cooldown.Instance(1000, RingAttack.Instance(30, projectileIndex: 2)),
                                     Cooldown.Instance(1000, RingAttack.Instance(30, projectileIndex: 2)),
                                     Once.Instance(UnsetConditionEffect.Instance(ConditionEffectIndex.Invulnerable)),
                                     new SetKey(-1, 4)
                                     )))),

                                     
                                     IfEqual.Instance(-1, 4, 
                                     new RunBehaviors(
                                     Chasing.Instance(3, 25, 2, null),
                                     Cooldown.Instance(2000, MultiAttack.Instance(25, 45 * (float)Math.PI / 180, 8, 0, projectileIndex: 0)),
                                     Cooldown.Instance(2000, MultiAttack.Instance(25, 45 * (float)Math.PI / 180, 3, 0, projectileIndex: 0)),
                                     Cooldown.Instance(2000, MultiAttack.Instance(25, 45 * (float)Math.PI / 180, 3, 0, projectileIndex: 1)),
                                     Cooldown.Instance(2000, MultiAttack.Instance(25, 45 * (float)Math.PI / 180, 2, 0, projectileIndex: 2))
                                        ))),




 //Close Keys


                  loot: new LootBehavior(LootDef.Empty,
                        Tuple.Create(100, new LootDef(0, 5, 0, 15,
                         //Insert your special items Here 
                         //   Tuple.Create(0.008, (ILoot)new ItemLoot("??????")),                 
                         //   Tuple.Create(0.008, (ILoot)new ItemLoot("??????")),
                         //   Tuple.Create(0.008, (ILoot)new ItemLoot("??????")),
                         //   Tuple.Create(0.008, (ILoot)new ItemLoot("??????")),
                         //   Tuple.Create(0.008, (ILoot)new ItemLoot("??????")),
                            Tuple.Create(0.07, (ILoot)new TierLoot(5, ItemType.Ability)),
                            Tuple.Create(0.05, (ILoot)new TierLoot(6, ItemType.Ability)),
                            Tuple.Create(0.07, (ILoot)new TierLoot(11, ItemType.Armor)),
                            Tuple.Create(0.06, (ILoot)new TierLoot(12, ItemType.Armor)),
                            Tuple.Create(0.05, (ILoot)new TierLoot(10, ItemType.Armor)),
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
                            Tuple.Create(1.00, (ILoot)new ItemLoot("Valentine Generator"))
                            ))),

                     condBehaviors: new ConditionalBehavior[] {
                    new OnDeath(Once.Instance(SetConditionEffect.Instance(ConditionEffectIndex.Invulnerable))),
                    new OnDeath(new SimpleTaunt("I will return"))}

                    ));











                         
    }
}