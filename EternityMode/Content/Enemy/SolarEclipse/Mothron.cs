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
    public class Mothron : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.Mothron);

        public int StingerCounter;
        public int LaserCounter;

        public override void SetDefaults(NPC npc)
        {
            base.SetDefaults(npc);

            npc.knockBackResist *= 0.1f;
        }

        public override void OnSpawn(NPC npc)
        {
            base.OnSpawn(npc);

            FargoSoulsUtil.NewNPCEasy(npc.GetSpawnSourceForNPCFromNPCAI(), npc.Center + 100 * Vector2.UnitX, NPCID.MothronSpawn);
            FargoSoulsUtil.NewNPCEasy(npc.GetSpawnSourceForNPCFromNPCAI(), npc.Center - 100 * Vector2.UnitX, NPCID.MothronSpawn);
        }

        public override void AI(NPC npc)
        {
            base.AI(npc);

            if (--StingerCounter < 0)
            {
                StingerCounter = 20 + (int)(100f * npc.life / npc.lifeMax);
                if (npc.HasPlayerTarget && Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Vector2 spawnPos = npc.Center + new Vector2(30f * -npc.direction, 30f);
                    Vector2 vel = Main.player[npc.target].Center - spawnPos
                        + new Vector2(Main.rand.Next(-80, 81), Main.rand.Next(-40, 41));
                    vel.Normalize();
                    vel *= 10f;
                    Projectile.NewProjectile(npc.GetSpawnSource_ForProjectile(), spawnPos, vel, ProjectileID.Stinger, npc.defDamage / 8, 0f, Main.myPlayer);
                }
            }

            if (--LaserCounter < 0)
            {
                LaserCounter = 60 + (int)(120f * npc.life / npc.lifeMax);
                if (npc.HasPlayerTarget && Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Vector2 spawnPos = npc.Center;
                    spawnPos.X += 45f * npc.direction;
                    Vector2 vel = Vector2.Normalize(Main.player[npc.target].Center - spawnPos) * 9f;
                    Projectile.NewProjectile(npc.GetSpawnSource_ForProjectile(), spawnPos, vel, ProjectileID.EyeLaser, npc.defDamage / 5, 0f, Main.myPlayer);
                }
            }

            npc.defense = npc.defDefense;
            npc.reflectsProjectiles = npc.ai[0] >= 4f;
            if (npc.reflectsProjectiles)
            {
                npc.defense *= 5;
                if (npc.buffType[0] != 0)
                    npc.DelBuff(0);
                int d = Dust.NewDust(npc.position, npc.width, npc.height, 228, npc.velocity.X * .4f, npc.velocity.Y * .4f, 0, Color.White, 3f);
                Main.dust[d].velocity *= 6f;
                Main.dust[d].noGravity = true;
            }
        }

        public override void OnHitPlayer(NPC npc, Player target, int damage, bool crit)
        {
            base.OnHitPlayer(npc, target, damage, crit);

            target.AddBuff(BuffID.Rabies, 3600);
            target.GetModPlayer<FargoSoulsPlayer>().AddBuffNoStack(ModContent.BuffType<Stunned>(), 60);
        }
    }
}
