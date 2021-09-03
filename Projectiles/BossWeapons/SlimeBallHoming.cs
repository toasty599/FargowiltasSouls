using Terraria;
using Terraria.ID;

namespace FargowiltasSouls.Projectiles.BossWeapons
{
    public class SlimeBallHoming : SlimeBall
    {
        public override string Texture => "FargowiltasSouls/Projectiles/BossWeapons/SlimeBall";

        int bounce;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Slime Ball");
            ProjectileID.Sets.Homing[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            projectile.penetrate = 2;
        }

        public override void AI()
        {
            base.AI();

            if (bounce == 0)
            {
                bounce = projectile.timeLeft - Main.rand.Next(150);
            }

            if (projectile.timeLeft == bounce)
            {
                bounce = 0;

                //projectile.localAI[0] = 0;

                if (projectile.owner == Main.myPlayer)
                {
                    projectile.velocity = projectile.DirectionTo(Main.MouseWorld) * projectile.velocity.Length();
                }
                
                /*int foundTarget = HomeOnTarget();
                if (foundTarget != -1)
                {
                    NPC n = Main.npc[foundTarget];
                    projectile.velocity = projectile.DirectionTo(n.Center) * projectile.velocity.Length();
                }*/
            }
        }

        private int HomeOnTarget()
        {
            const bool homingCanAimAtWetEnemies = true;
            const float homingMaximumRangeInPixels = 1000;

            int selectedTarget = -1;
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC n = Main.npc[i];
                if (n.CanBeChasedBy(projectile) && (!n.wet || homingCanAimAtWetEnemies) && Collision.CanHitLine(projectile.Center, 0, 0, n.Center, 0, 0))
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

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            base.OnHitNPC(target, damage, knockback, crit);
            target.immune[projectile.owner] = 9;
        }
    }
}