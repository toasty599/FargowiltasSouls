using Microsoft.Xna.Framework;
using Terraria;

namespace FargowiltasSouls.Projectiles.Masomode
{
    public class DarkStarDestroyer : DarkStar
    {
        public override string Texture => "Terraria/Images/Projectile_12";

        public override void AI()
        {
            if (projectile.localAI[1] == 0)
                projectile.localAI[1] = projectile.velocity.Length() / System.Math.Abs(projectile.ai[1]);

            base.AI();

            if (++projectile.ai[1] > 0)
            {
                if (projectile.ai[1] > 30)
                    projectile.velocity *= 1.06f;
                else if (projectile.ai[1] == 30)
                {
                    projectile.velocity = projectile.ai[0].ToRotationVector2();
                    projectile.timeLeft = 180;
                    projectile.netUpdate = true;
                }
                else
                    projectile.velocity = Vector2.Lerp(projectile.velocity, projectile.ai[0].ToRotationVector2(), 0.1f);
            }
            else
            {
                projectile.velocity = Vector2.Normalize(projectile.velocity) * (projectile.velocity.Length() - projectile.localAI[1]);
            }
        }
    }
}