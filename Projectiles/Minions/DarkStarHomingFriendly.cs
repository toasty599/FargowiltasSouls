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
            ProjectileID.Sets.Homing[projectile.type] = true;
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

            projectile.ai[1]++;

            NPC npc = FargoSoulsUtil.NPCExists(projectile.ai[0]);
            if (npc != null)
            {
                if (projectile.ai[1] < 60)
                {
                    float rotation = projectile.velocity.ToRotation();
                    Vector2 vel = npc.Center - projectile.Center;
                    float targetAngle = vel.ToRotation();
                    projectile.velocity = new Vector2(projectile.velocity.Length(), 0f).RotatedBy(rotation.AngleLerp(targetAngle, 0.05f));
                }
            }
            else
            {
                projectile.ai[0] = FargoSoulsUtil.FindClosestHostileNPC(projectile.Center, 750);
            }

            if (projectile.ai[1] < 60)
                projectile.velocity *= 1.065f;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.immune[projectile.owner] = 6;
        }
    }
}