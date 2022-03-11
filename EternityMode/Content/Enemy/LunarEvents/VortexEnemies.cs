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
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.EternityMode.Content.Enemy.LunarEvents
{
    public class VortexEnemies : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchTypeRange(
            NPCID.VortexLarva,
            NPCID.VortexHornet,
            NPCID.VortexHornetQueen,
            NPCID.VortexSoldier,
            NPCID.VortexRifleman
        );

        public override void SetDefaults(NPC npc)
        {
            base.SetDefaults(npc);
            npc.buffImmune[BuffID.Suffocation] = true;
        }

        public override void OnHitPlayer(NPC npc, Player target, int damage, bool crit)
        {
            base.OnHitPlayer(npc, target, damage, crit);

            target.AddBuff(ModContent.BuffType<LightningRod>(), 300);
        }
    }

    public class VortexHornetQueen : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.VortexHornetQueen);

        public int Counter;

        public override void AI(NPC npc)
        {
            base.AI(npc);

            if (++Counter >= 240)
            {
                Counter = Main.rand.Next(30);
                if (Main.netMode != NetmodeID.MultiplayerClient)
                    Projectile.NewProjectile(npc.GetSpawnSource_ForProjectile(), npc.Center, Vector2.Zero, ModContent.ProjectileType<LightningVortexHostile>(), npc.damage / 4, 0f, Main.myPlayer);
            }
        }
    }

    public class VortexRifleman : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.VortexRifleman);

        public override void AI(NPC npc)
        {
            base.AI(npc);

            //default: if (npc.localAI[2] >= 360f + Main.rand.Next(360) && etc)
            if (npc.localAI[2] >= 180f + Main.rand.Next(180) && npc.Distance(Main.player[npc.target].Center) < 400f && Math.Abs(npc.DirectionTo(Main.player[npc.target].Center).Y) < 0.5f && Collision.CanHitLine(npc.Center, 0, 0, Main.player[npc.target].Center, 0, 0))
            {
                npc.localAI[2] = 0f;
                Vector2 vector2_1 = npc.Center;
                vector2_1.X += npc.direction * 30f;
                vector2_1.Y += 2f;

                Vector2 vec = npc.DirectionTo(Main.player[npc.target].Center) * 7f;
                if (vec.HasNaNs())
                    vec = new Vector2(npc.direction * 8f, 0);

                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    int Damage = Main.expertMode ? 50 : 75;
                    for (int index = 0; index < 4; ++index)
                    {
                        Vector2 vector2_2 = vec + Utils.RandomVector2(Main.rand, -0.8f, 0.8f);
                        Projectile.NewProjectile(npc.GetSpawnSource_ForProjectile(), vector2_1.X, vector2_1.Y, vector2_2.X, vector2_2.Y, ModContent.ProjectileType<StormDiverBullet>(), Damage, 1f, Main.myPlayer);
                    }
                }

                Terraria.Audio.SoundEngine.PlaySound(SoundID.Item36, npc.Center);
            }
        }
    }
}
