using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace FargowiltasSouls.Projectiles.Minions
{
    public class DarkStarHomingFriendly : Masomode.DarkStar
    {
        public override string Texture => "Terraria/Projectile_12";

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            ProjectileID.Sets.MinionShot[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            projectile.timeLeft = 180;
            projectile.hostile = false;
            projectile.friendly = true;
            projectile.minion = true;
            projectile.penetrate = -1;
        }

        public override void AI()
        {
            base.AI();

            int ai0 = (int)projectile.ai[0];
            if (ai0 > -1 && ai0 < Main.maxNPCs && Main.npc[ai0].active && Main.npc[ai0].CanBeChasedBy())
            {
                if (++projectile.ai[1] < 60)
                {
                    float rotation = projectile.velocity.ToRotation();
                    Vector2 vel = Main.npc[ai0].Center - projectile.Center;
                    float targetAngle = vel.ToRotation();
                    projectile.velocity = new Vector2(projectile.velocity.Length(), 0f).RotatedBy(rotation.AngleLerp(targetAngle, 0.05f));
                }
            }
            else
            {
                float maxRange = 750f;
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    if (Main.npc[i].CanBeChasedBy(projectile) && Collision.CanHit(projectile.Center, 0, 0, Main.npc[i].Center, 0, 0))
                    {
                        if (projectile.Distance(Main.npc[i].Center) <= maxRange)
                        {
                            maxRange = projectile.Distance(Main.npc[i].Center);
                            projectile.ai[0] = i;
                        }
                    }
                }
            }

            if(projectile.ai[0] < 60)
                projectile.velocity *= 1.065f;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.immune[projectile.owner] = 6;
        }
    }
}