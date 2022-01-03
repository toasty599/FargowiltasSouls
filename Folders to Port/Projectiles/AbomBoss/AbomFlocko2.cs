using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace FargowiltasSouls.Projectiles.AbomBoss
{
    public class AbomFlocko2 : AbomFlocko
    {
        public override bool CanDamage()
        {
            return false;
        }

        public override void AI()
        {
            if (projectile.ai[0] < 0 || projectile.ai[0] >= Main.maxPlayers)
            {
                projectile.Kill();
                return;
            }

            Player player = Main.player[(int)projectile.ai[0]];

            Vector2 target = player.Center;
            target.X += 700 * projectile.ai[1];

            Vector2 distance = target - projectile.Center;
            float length = distance.Length();
            if (length > 100f)
            {
                distance /= 8f;
                projectile.velocity = (projectile.velocity * 23f + distance) / 24f;
            }
            else
            {
                if (projectile.velocity.Length() < 12f)
                    projectile.velocity *= 1.05f;
            }

            if (++projectile.localAI[0] > 90 && ++projectile.localAI[1] > 60) //fire frost wave
            {
                projectile.localAI[1] = 0f;
                Main.PlaySound(SoundID.Item120, projectile.position);
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Vector2 vel = projectile.DirectionTo(player.Center) * 7f;
                    for (int i = -1; i <= 1; i++)
                        Projectile.NewProjectile(projectile.Center, vel.RotatedBy(MathHelper.ToRadians(10) * i), mod.ProjectileType("AbomFrostWave"), projectile.damage, projectile.knockBack, projectile.owner);
                }
            }

            projectile.rotation += projectile.velocity.Length() / 12f * (projectile.velocity.X > 0 ? -0.2f : 0.2f);
            if (++projectile.frameCounter > 3)
            {
                if (++projectile.frame >= 6)
                    projectile.frame = 0;
                projectile.frameCounter = 0;
            }
        }
    }
}