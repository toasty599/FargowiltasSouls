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

namespace FargowiltasSouls.EternityMode.Content.Enemy.Mushroom
{
    public class MushroomEnemies : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchTypeRange(
            NPCID.FungiBulb,
            NPCID.GiantFungiBulb,
            NPCID.AnomuraFungus,
            NPCID.MushiLadybug,
            NPCID.SporeBat,
            NPCID.ZombieMushroom,
            NPCID.ZombieMushroomHat,
            NPCID.FungoFish
        );

        public override void OnKill(NPC npc)
        {
            base.OnKill(npc);

            if (Main.netMode != NetmodeID.MultiplayerClient && Main.hardMode)
            {
                for (int i = 0; i < 10; i++)
                {
                    FargoSoulsUtil.NewNPCEasy(npc.GetSpawnSourceForProjectileNPC(), npc.Center, NPCID.FungiSpore,
                        velocity: 0.5f * new Vector2(Main.rand.NextFloat(-5, 5), Main.rand.NextFloat(-5, 5)));
                }
            }
        }

        public override void OnHitPlayer(NPC npc, Player target, int damage, bool crit)
        {
            base.OnHitPlayer(npc, target, damage, crit);

            target.AddBuff(BuffID.Poisoned, 300);
        }

        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            base.ModifyNPCLoot(npc, npcLoot);

            EModeUtils.EModeDrop(npcLoot, ItemDropRule.Common(ItemID.GlowingMushroom, 1, 1, 5));
            EModeUtils.EModeDrop(npcLoot, ItemDropRule.Common(ItemID.MushroomGrassSeeds, 5));
            EModeUtils.EModeDrop(npcLoot, ItemDropRule.Common(ItemID.TruffleWorm, 20));
        }
    }
}
