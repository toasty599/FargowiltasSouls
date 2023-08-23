using FargowiltasSouls.Content.Buffs.Masomode;
using FargowiltasSouls.Core.NPCMatching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;

namespace FargowiltasSouls.Content.NPCs.EternityModeNPCs.VanillaEnemies.LunarEvents.Vortex
{
    public class LunarTowerVortex : LunarTowers
    {
        public override int ShieldStrength
        {
            get => NPC.ShieldStrengthTowerVortex;
            set => NPC.ShieldStrengthTowerVortex = value;
        }

        public override NPCMatcher CreateMatcher() =>
            new NPCMatcher().MatchType(NPCID.LunarTowerVortex);

        public LunarTowerVortex() : base(ModContent.BuffType<JammedBuff>(), DustID.Vortex) { }
        public override void SetDefaults(NPC npc)
        {
            base.SetDefaults(npc);

            npc.lifeMax = (int)Math.Round(npc.lifeMax * 4f);
            npc.damage = (int)Math.Round(npc.damage * 0.6f);
        }
        private enum Attacks
        {
            Idle,
            VortexVortex,
            LightningBall,

        }
        public override List<int> RandomAttacks => new List<int>() //these are randomly chosen attacks in p1
        {
            (int)Attacks.LightningBall
        };
        public override void ShieldsDownAI(NPC npc)
        {
            if (++AttackTimer > 360) //triggers "shield going down" animation
            {
                AttackTimer = 0;
                npc.ai[3] = 1f;
                npc.netUpdate = true;
                NetSync(npc);
            }

            npc.reflectsProjectiles = npc.ai[3] != 0f;
            if (npc.reflectsProjectiles) //dust
            {
                for (int i = 0; i < 20; i++)
                {
                    Vector2 offset = new();
                    double angle = Main.rand.NextDouble() * 2d * Math.PI;
                    offset.X += (float)(Math.Sin(angle) * npc.height / 2);
                    offset.Y += (float)(Math.Cos(angle) * npc.height / 2);
                    Dust dust = Main.dust[Dust.NewDust(
                        npc.Center + offset - new Vector2(4, 4), 0, 0,
                        DustID.Vortex, 0, 0, 100, Color.White, 1f
                        )];
                    dust.noGravity = true;
                }
            }

            //if (++Counter[2] > 240)
            //{
            //    Counter[2] = 0;
            //    npc.TargetClosest(false);
            //    if (npc.HasPlayerTarget && Main.netMode != NetmodeID.MultiplayerClient)
            //    {
            //        Vector2 speed = Main.player[npc.target].Center + Main.player[npc.target].velocity * 15f - npc.Center;
            //        speed.Normalize();
            //        speed *= 4f;
            //        Projectile.NewProjectile(npc.Center, speed, ProjectileID.CultistBossLightningOrb, 30, 0f, Main.myPlayer);
            //    }
            //}
        }
    }
}
