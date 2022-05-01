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

namespace FargowiltasSouls.EternityMode.Content.Enemy.Dungeon
{
    public class DarkCaster : DungeonTeleporters
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.DarkCaster);

        public int AttackTimer;

        public override void AI(NPC npc)
        {
            base.AI(npc);

            if (++AttackTimer > 300)
            {
                AttackTimer = 0;
                for (int i = 0; i < 5; i++) //spray water bolts
                {
                    Terraria.Audio.SoundEngine.PlaySound(SoundID.Item21, npc.Center);
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        int p = Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, Main.rand.NextVector2CircularEdge(-4.5f, 4.5f), ProjectileID.WaterBolt, FargoSoulsUtil.ScaledProjectileDamage(npc.damage), 0f, Main.myPlayer);
                        if (p != Main.maxProjectiles)
                            Main.projectile[p].timeLeft = Main.rand.Next(180, 360);
                    }
                }
            }
        }

        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            base.ModifyNPCLoot(npc, npcLoot);

            FargoSoulsUtil.EModeDrop(npcLoot, ItemDropRule.Common(ItemID.WaterBolt, 50));
        }
    }
}
