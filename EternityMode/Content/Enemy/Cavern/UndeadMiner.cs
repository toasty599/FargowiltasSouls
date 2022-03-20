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
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.EternityMode.Content.Enemy.Cavern
{
    public class UndeadMiner : Shooters
    {
        public UndeadMiner() : base(90, ProjectileID.BombSkeletronPrime, 10f, 0.7f, -1, 800, 0) { }

        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.UndeadMiner);

        public override void OnHitPlayer(NPC npc, Player target, int damage, bool crit)
        {
            base.OnHitPlayer(npc, target, damage, crit);

            target.AddBuff(ModContent.BuffType<Lethargic>(), 600);
            target.AddBuff(BuffID.Blackout, 300);
            target.AddBuff(BuffID.NoBuilding, 300);
            if (target.whoAmI == Main.myPlayer && !target.GetModPlayer<FargoSoulsPlayer>().SecurityWallet)
            {
                bool stolen = false;
                for (int i = 0; i < 59; i++)
                {
                    if (target.inventory[i].pick != 0 || target.inventory[i].hammer != 0 || target.inventory[i].axe != 0)
                    {
                        if (EModeGlobalNPC.StealFromInventory(target, ref target.inventory[i]))
                            stolen = true;
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
