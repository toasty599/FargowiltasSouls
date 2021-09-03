using Terraria;
using Terraria.ID;

namespace FargowiltasSouls.Projectiles.BossWeapons
{
    public class PhantasmalSphere2 : PhantasmalSphere
    {
        public override string Texture => "Terraria/Projectile_454";

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 6;
            ProjectileID.Sets.Homing[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            projectile.timeLeft = 90;
            projectile.penetrate = -1;
        }

        public override void AI()
        {
            //dust!
            /*int dustId = Dust.NewDust(projectile.position, projectile.width / 2, projectile.height + 5, 56, projectile.velocity.X * 0.2f,
                projectile.velocity.Y * 0.2f, 100, default(Color), .5f);
            Main.dust[dustId].noGravity = true;
            int dustId3 = Dust.NewDust(projectile.position, projectile.width / 2, projectile.height + 5, 56, projectile.velocity.X * 0.2f,
                projectile.velocity.Y * 0.2f, 100, default(Color), .5f);
            Main.dust[dustId3].noGravity = true;*/

            if (++projectile.localAI[0] == 20)
            {
                //projectile.localAI[0] = 0;

                int foundTarget = HomeOnTarget();
                if (foundTarget == -1)
                {
                    projectile.Kill();
                }
                else
                {
                    NPC n = Main.npc[foundTarget];
                    projectile.velocity = projectile.DirectionTo(n.Center + n.velocity * Main.rand.NextFloat(60)) * 32f;
                }
            }
            
            if (projectile.alpha > 0)
            {
                projectile.alpha -= 20;
                if (projectile.alpha < 0)
                    projectile.alpha = 0;
            }
            projectile.scale = 1f - projectile.alpha / 255f;
            if (++projectile.frameCounter >= 6)
            {
                projectile.frameCounter = 0;
                if (++projectile.frame > 1)
                    projectile.frame = 0;
            }
        }

        private int HomeOnTarget()
        {
            const bool homingCanAimAtWetEnemies = true;
            const float homingMaximumRangeInPixels = 1500;

            int selectedTarget = -1;
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC n = Main.npc[i];
                if (n.CanBeChasedBy(projectile) && (!n.wet || homingCanAimAtWetEnemies))
                {
                    float distance = projectile.Distance(n.Center);
                    if (distance <= homingMaximumRangeInPixels &&
                        (
                            selectedTarget == -1 || //there is no selected target
                            projectile.Distance(Main.npc[selectedTarget].Center) > distance) //or we are closer to this target than the already selected target
                    )
                        selectedTarget = i;
                }
            }

            return selectedTarget;
        }
    }
}