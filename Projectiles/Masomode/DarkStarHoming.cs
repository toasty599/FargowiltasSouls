using Microsoft.Xna.Framework;
using Terraria;

namespace FargowiltasSouls.Projectiles.Masomode
{
    public class DarkStarHoming : DarkStar
    {
        public override string Texture => "Terraria/Projectile_12";

        public override void AI()
        {
            base.AI();

            if (projectile.ai[1] == 0)
            {
                bool playerExists = projectile.ai[0] > -1 && projectile.ai[0] < Main.maxPlayers;
                bool proceed = false;

                if (playerExists)
                {
                    float rotation = projectile.velocity.ToRotation();
                    Vector2 vel = Main.player[(int)projectile.ai[0]].Center - projectile.Center;
                    float targetAngle = vel.ToRotation();
                    projectile.velocity = new Vector2(projectile.velocity.Length(), 0f).RotatedBy(rotation.AngleLerp(targetAngle, 0.2f));
                    if (vel.Length() < 300 || !Main.player[(int)projectile.ai[0]].active || Main.player[(int)projectile.ai[0]].dead || Main.player[(int)projectile.ai[0]].ghost)
                        proceed = true;
                }

                if (!playerExists || proceed)
                {
                    projectile.ai[1] = 1;
                    projectile.netUpdate = true;
                    projectile.timeLeft = 180;
                    projectile.velocity.Normalize();
                }
            }
            else if (projectile.ai[1] > 0)
            {
                if (projectile.ai[0] > -1 && projectile.ai[0] < Main.maxPlayers) //homing mode
                {
                    if (++projectile.ai[1] < 90)
                    {
                        projectile.velocity *= 1.03f;

                        float rotation = projectile.velocity.ToRotation();
                        Vector2 vel = Main.player[(int)projectile.ai[0]].Center - projectile.Center;
                        float targetAngle = vel.ToRotation();
                        projectile.velocity = new Vector2(projectile.velocity.Length(), 0f).RotatedBy(rotation.AngleLerp(targetAngle, 0.035f));
                    }
                }
                else //straight accel mode
                {
                    if (++projectile.ai[1] < 75)
                        projectile.velocity *= 1.06f;
                }
            }
            else //ai1 below 0 rn
            {
                projectile.ai[1]++;
            }
        }
    }
}