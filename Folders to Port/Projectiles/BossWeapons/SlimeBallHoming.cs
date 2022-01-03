using Terraria;

namespace FargowiltasSouls.Projectiles.BossWeapons
{
    public class SlimeBallHoming : SlimeBall
    {
        public override string Texture => "FargowiltasSouls/Projectiles/BossWeapons/SlimeBall";

        int bounce;

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

                if (projectile.owner == Main.myPlayer)
                {
                    projectile.velocity = projectile.DirectionTo(Main.MouseWorld) * projectile.velocity.Length();
                    projectile.netUpdate = true;
                }
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            base.OnHitNPC(target, damage, knockback, crit);
            target.immune[projectile.owner] = 9;
        }
    }
}