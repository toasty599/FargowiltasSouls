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
    public class DrManFly : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.DrManFly);

        public override void OnKill(NPC npc)
        {
            base.OnKill(npc);

            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                for (int i = 0; i < 10; i++)
                    Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, new Vector2(Main.rand.Next(-10, 11), Main.rand.Next(-10, 11)),
                        ProjectileID.DrManFlyFlask, FargoSoulsUtil.ScaledProjectileDamage(npc.damage, 4f / 2), 1f, Main.myPlayer);
            }
        }
    }
}
