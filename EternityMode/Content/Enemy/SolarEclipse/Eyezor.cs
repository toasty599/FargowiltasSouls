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

namespace FargowiltasSouls.EternityMode.Content.Enemy.SolarEclipse
{
    public class Eyezor : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.Eyezor);

        public int Counter;

        public override void AI(NPC npc)
        {
            base.AI(npc);

            if (++Counter >= 8)
            {
                Counter = 0;
                int p = npc.HasPlayerTarget ? npc.target : npc.FindClosestPlayer();
                if (p != -1 && Main.player[p].active && !Main.player[p].ghost && !Main.player[p].dead && npc.Distance(Main.player[p].Center) < 600)
                {
                    Vector2 velocity = Main.player[p].Center - npc.Center;
                    velocity.Normalize();
                    velocity *= 4f;
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                        Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, velocity, ProjectileID.EyeFire, FargoSoulsUtil.ScaledProjectileDamage(npc.damage, 0.8f), 0f, Main.myPlayer);
                }
            }
        }
    }
}
