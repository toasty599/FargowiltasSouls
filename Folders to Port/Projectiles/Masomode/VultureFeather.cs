using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Masomode
{
    public class VultureFeather : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Vulture Feather");
        }

        public override void SetDefaults()
        {
            projectile.CloneDefaults(ProjectileID.HarpyFeather);
            projectile.aiStyle = 1;
            projectile.hostile = true;
            projectile.friendly = false;
            projectile.timeLeft = 600;
            projectile.tileCollide = true;
            cooldownSlot = 1; // do we need this?
        }

        public override void Kill(int timeLeft)
        {
            for (int num610 = 0; num610 < 10; num610++)
            {
                Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 42, projectile.velocity.X * 0.1f, projectile.velocity.Y * 0.1f, 0, Color.SandyBrown, 1f);
            }
        }
    }
}
