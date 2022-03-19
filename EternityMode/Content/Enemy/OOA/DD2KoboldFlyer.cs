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

namespace FargowiltasSouls.EternityMode.Content.Enemy.OOA
{
    public class DD2KoboldFlyer : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchTypeRange(
            NPCID.DD2KoboldFlyerT2,
            NPCID.DD2KoboldFlyerT3
        );

        public int Counter;

        public override void AI(NPC npc)
        {
            base.AI(npc);

            if (++Counter > 60)
            {
                Counter = 0;
                if (Main.netMode != NetmodeID.MultiplayerClient)
                    Projectile.NewProjectile(npc.GetSpawnSource_ForProjectile(), npc.Center, new Vector2(Main.rand.NextFloat(-2f, 2f), -5f),
                        ModContent.ProjectileType<GoblinSpikyBall>(), npc.damage / 5, 0f, Main.myPlayer);
            }
        }

        public override void OnHitPlayer(NPC npc, Player target, int damage, bool crit)
        {
            base.OnHitPlayer(npc, target, damage, crit);

            target.AddBuff(ModContent.BuffType<Fused>(), 1800);
        }
    }
}
