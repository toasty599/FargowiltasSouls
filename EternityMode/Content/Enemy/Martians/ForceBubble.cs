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

namespace FargowiltasSouls.EternityMode.Content.Enemy.Martians
{
    public class ForceBubble : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.ForceBubble);

        public override void OnHitByAnything(NPC npc, Player player, int damage, float knockback, bool crit)
        {
            base.OnHitByAnything(npc, player, damage, knockback, crit);

            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                int Damage = Main.expertMode ? 28 : 35;

                Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, 10f * Vector2.UnitX.RotatedByRandom(MathHelper.TwoPi), ProjectileID.MartianTurretBolt, Damage, 0f, Main.myPlayer);
                
                if (Main.rand.NextBool(3))
                    Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, 10f * npc.DirectionTo(player.Center), ProjectileID.MartianTurretBolt, Damage, 0f, Main.myPlayer);
            }
        }
    }
}
