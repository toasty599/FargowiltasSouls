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

namespace FargowiltasSouls.EternityMode.Content.Enemy.SkyAndRain
{
    public class AngryNimbus : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.AngryNimbus);

        public int Counter;

        public override void AI(NPC npc)
        {
            base.AI(npc);

            if (++Counter >= 360)
            {
                Counter = 0;

                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Projectile.NewProjectile(npc.GetSource_FromThis(), new Vector2(npc.Center.X + 100, npc.Center.Y), Vector2.Zero, ProjectileID.VortexVortexLightning, 0, 1, Main.myPlayer, 0, 1);
                    Projectile.NewProjectile(npc.GetSource_FromThis(), new Vector2(npc.Center.X - 100, npc.Center.Y), Vector2.Zero, ProjectileID.VortexVortexLightning, 0, 1, Main.myPlayer, 0, 1);
                }
            }
        }
    }
}
