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
            Projectile.penetrate = 2;
        }

        public override void AI()
        {
            base.AI();

            if (bounce == 0)
            {
                bounce = Projectile.timeLeft - Main.rand.Next(150);
            }

            if (Projectile.timeLeft == bounce)
            {
                bounce = 0;

                if (Projectile.owner == Main.myPlayer)
                {
                    Projectile.velocity = Projectile.DirectionTo(Main.MouseWorld) * Projectile.velocity.Length();
                    Projectile.netUpdate = true;
                }
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            base.OnHitNPC(target, damage, knockback, crit);
            target.immune[Projectile.owner] = 9;
        }
    }
}