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
                                Cooldown.Instance(6000, new SimpleTaunt("Happy Thanksgiving, Friends! Only {HP} more hitpoints to go!"))
                            )
                        )
                    ),
#endregion
  
#region Loot                        
                    loot: new LootBehavior(LootDef.Empty,
                        Tuple.Create(100, new LootDef(0, 5, 0, 8,
                            Tuple.Create(0.05, (ILoot)new ItemLoot("America Sword")),
                            Tuple.Create(0.05, (ILoot)new ItemLoot("America Armor")),
                            Tuple.Create(0.05, (ILoot)new ItemLoot("America Shield")),
                            Tuple.Create(0.05, (ILoot)new ItemLoot("America Ring")),

                            Tuple.Create(0.5, (ILoot)new StatPotionLoot(StatPotion.Att)),
                            Tuple.Create(0.5, (ILoot)new StatPotionLoot(StatPotion.Wis)),
                            Tuple.Create(0.5, (ILoot)new StatPotionLoot(StatPotion.Vit)),
                            Tuple.Create(0.5, (ILoot)new StatPotionLoot(StatPotion.Spd))
                    )))
#endregion
            ));

    }
}