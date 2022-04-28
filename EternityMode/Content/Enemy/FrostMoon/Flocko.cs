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

namespace FargowiltasSouls.EternityMode.Content.Enemy.FrostMoon
{
    public class Flocko : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.Flocko);

        public override void OnKill(NPC npc)
        {
            base.OnKill(npc);

            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                for (int i = 0; i < 20; i++)
                {
                    Vector2 speed = new Vector2(Main.rand.Next(-1000, 1001), Main.rand.Next(-1000, 1001));
                    speed.Normalize();
                    speed *= Main.rand.NextFloat(9f);
                    Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center + 4 * speed, speed, ProjectileID.FrostShard, FargoSoulsUtil.ScaledProjectileDamage(npc.damage), 0f, Main.myPlayer);
                }
            }
        }

        public override void OnHitPlayer(NPC npc, Player target, int damage, bool crit)
        {
            base.OnHitPlayer(npc, target, damage, crit);

            target.AddBuff(ModContent.BuffType<Hypothermia>(), 300);
            target.AddBuff(BuffID.Frostburn, 180);
        }
    }
}
