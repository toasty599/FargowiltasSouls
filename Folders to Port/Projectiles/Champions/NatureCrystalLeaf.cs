using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;

namespace FargowiltasSouls.Projectiles.Champions
{
    public class NatureCrystalLeaf : MutantBoss.MutantCrystalLeaf
    {
        public override string Texture => "Terraria/Images/Projectile_226";

        public override void SetDefaults()
        {
            base.SetDefaults();
            projectile.timeLeft = 300;
            projectile.penetrate = -1;
            projectile.GetGlobalProjectile<FargoSoulsGlobalProjectile>().DeletionImmuneRank = 1;
        }

        public override void AI()
        {
            if (projectile.localAI[0] == 0)
            {
                projectile.localAI[0] = 1;
                for (int index1 = 0; index1 < 30; ++index1)
                {
                    int index2 = Dust.NewDust(projectile.position, projectile.width, projectile.height, 157, 0f, 0f, 0, new Color(), 2f);
                    Main.dust[index2].noGravity = true;
                    Main.dust[index2].velocity *= 5f;
                }
            }

            Lighting.AddLight(projectile.Center, 0.1f, 0.4f, 0.2f);
            projectile.scale = (Main.mouseTextColor / 200f - 0.35f) * 0.2f + 0.95f;
            projectile.scale *= 2;

            int ai0 = (int)projectile.ai[0];
            Vector2 offset = new Vector2(125, 0).RotatedBy(projectile.ai[1]);
            projectile.Center = Main.npc[ai0].Center + offset;
            projectile.ai[1] += 0.09f;
            projectile.rotation = projectile.ai[1] + (float)Math.PI / 2f;
        }

        public override void Kill(int timeLeft)
        {
            for (int index1 = 0; index1 < 30; ++index1)
            {
                int index2 = Dust.NewDust(projectile.position, projectile.width, projectile.height, 157, 0f, 0f, 0, new Color(), 2f);
                Main.dust[index2].noGravity = true;
                Main.dust[index2].velocity *= 5f;
            }
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.Poisoned, 300);
            if (FargoSoulsWorld.EternityMode)
                target.AddBuff(ModContent.BuffType<Infested>(), 300);
        }
    }
}