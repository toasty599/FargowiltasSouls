using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.BossWeapons
{
    internal class FishStickShark : ModProjectile
    {
        public override string Texture => "Terraria/Projectile_408";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Shark");
            Main.projFrames[projectile.type] = Main.projFrames[ProjectileID.MiniSharkron];
        }

        public override void SetDefaults()
        {
            projectile.CloneDefaults(ProjectileID.MiniSharkron);
            aiType = ProjectileID.MiniSharkron;
            projectile.penetrate = 2;
            projectile.timeLeft = 90;

            projectile.tileCollide = false;
            projectile.minion = false;
            projectile.ranged = true;
        }

        public override void AI()
        {
            projectile.position += projectile.velocity * .5f;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            projectile.timeLeft = 0;
        }

        public override void Kill(int timeLeft)
        {
            for (int num321 = 0; num321 < 15; num321++)
            {
                int num322 = Dust.NewDust(projectile.Center - Vector2.One * 10f, 50, 50, 5, 0f, -2f, 0, default(Color), 1f);
                Main.dust[num322].velocity /= 2f;
            }
            int num323 = 10;
            int num324 = Gore.NewGore(projectile.Center, projectile.velocity * 0.8f, 584, 1f);
            Main.gore[num324].timeLeft /= num323;
            num324 = Gore.NewGore(projectile.Center, projectile.velocity * 0.9f, 585, 1f);
            Main.gore[num324].timeLeft /= num323;
            num324 = Gore.NewGore(projectile.Center, projectile.velocity * 1f, 586, 1f);
            Main.gore[num324].timeLeft /= num323;
        }
    }
}