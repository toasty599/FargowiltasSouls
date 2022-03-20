using FargowiltasSouls.Buffs.Masomode;
using FargowiltasSouls.EternityMode.Net;
using FargowiltasSouls.EternityMode.Net.Strategies;
using FargowiltasSouls.EternityMode.NPCMatching;
using FargowiltasSouls.NPCs;
using FargowiltasSouls.Projectiles;
using FargowiltasSouls.Projectiles.Masomode;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.EternityMode.Content.Enemy.Night
{
    public class Wraith : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.Wraith);

        public override void AI(NPC npc)
        {
            base.AI(npc);

            EModeGlobalNPC.Aura(npc, 80, BuffID.Obstructed, false, 199);

            npc.aiStyle = 5;
        }

        public override void OnHitPlayer(NPC npc, Player target, int damage, bool crit)
        {
            base.OnHitPlayer(npc, target, damage, crit);

            target.AddBuff(ModContent.BuffType<LivingWasteland>(), 600);
            if (target.whoAmI == Main.myPlayer && !target.GetModPlayer<FargoSoulsPlayer>().SecurityWallet)
            {
                bool IsSoul(int type)
                {
                    return type == ItemID.SoulofFlight || type == ItemID.SoulofFright || type == ItemID.SoulofLight || type == ItemID.SoulofMight || type == ItemID.SoulofNight || type == ItemID.SoulofSight;
                };

                bool stolen = false;
                if (IsSoul(Main.mouseItem.type) && EModeGlobalNPC.StealFromInventory(target, ref Main.mouseItem))
                {
                    stolen = true;
                }
                else
                {
                    for (int j = 0; j < target.inventory.Length; j++)
                    {
                        Item item = target.inventory[j];

                        if (IsSoul(item.type))
                        {
                            if (EModeGlobalNPC.StealFromInventory(target, ref target.inventory[j]))
                                stolen = true;
                            break;
                        }
                    }
                }

                if (stolen)
                {
                    Main.NewText("An item was stolen from you!", new Color(255, 50, 50));
                    CombatText.NewText(target.Hitbox, new Color(255, 50, 50), "An item was stolen from you!", true);
                }
            }
        }
    }
}
