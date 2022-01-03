using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace FargowiltasSouls.Projectiles.Masomode
{
    public class BetsyFury2 : BetsyFury
    {
        public override string Texture => "Terraria/Projectile_709";

        public override void SetDefaults()
        {
            base.SetDefaults();
            projectile.timeLeft = 180;
        }

        public override void AI()
        {
            if (projectile.localAI[0] == 0)
            {
                Main.PlayTrackedSound(SoundID.DD2_SkyDragonsFuryShot, projectile.Center);
            }

            if (++projectile.localAI[0] < 120)
            {
                projectile.velocity *= 1.033f;
                float rotation = projectile.velocity.ToRotation();
                Vector2 vel = Main.player[(int)projectile.ai[0]].Center - projectile.Center;
                float targetAngle = vel.ToRotation();
                projectile.velocity = new Vector2(projectile.velocity.Length(), 0f).RotatedBy(rotation.AngleLerp(targetAngle, 0.02f));
            }

            projectile.alpha = projectile.alpha - 30;
            if (projectile.alpha < 0)
                projectile.alpha = 0;

            if (++projectile.frameCounter >= 3)
            {
                projectile.frameCounter = 0;
                if (++projectile.frame >= 3)
                    projectile.frame = 0;
            }
            Lighting.AddLight((int)projectile.Center.X / 16, (int)projectile.Center.Y / 16, 0.4f, 0.85f, 0.9f);

            projectile.rotation = projectile.velocity.ToRotation();
        }

        public override void Kill(int timeLeft)
        {
            int num1 = 3;
            int num2 = 10;
            
            for (int index1 = 0; index1 < num1; ++index1)
            {
                int index2 = Dust.NewDust(projectile.position, projectile.width, projectile.height, 31, 0.0f, 0.0f, 100, default, 1.5f);
                Main.dust[index2].position = new Vector2(projectile.width / 2, 0.0f).RotatedBy(6.28318548202515 * Main.rand.NextDouble()) * (float)Main.rand.NextDouble() + projectile.Center;
            }
            for (int index1 = 0; index1 < num2; ++index1)
            {
                int index2 = Dust.NewDust(projectile.position, projectile.width, projectile.height, 226, 0.0f, 0.0f, 0, default, 1.5f);
                Main.dust[index2].position = new Vector2(projectile.width / 2, 0.0f).RotatedBy(6.28318548202515 * Main.rand.NextDouble()) * (float)Main.rand.NextDouble() + projectile.Center;
                Main.dust[index2].noGravity = true;
            }
            
            Main.PlayTrackedSound(SoundID.DD2_SkyDragonsFuryCircle, projectile.Center);
        }
    }
}