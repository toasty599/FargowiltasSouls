using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace FargowiltasSouls.Projectiles.DeviBoss
{
    public class DeviLightBall2 : DeviLightBall
    {
        public override string Texture => "FargowiltasSouls/Projectiles/Masomode/LightBall";

        public override void SetDefaults()
        {
            base.SetDefaults();

            projectile.timeLeft = 600;
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.Item10, projectile.position);
            for (int index1 = 0; index1 < 10; ++index1)
            {
                int index2 = Dust.NewDust(projectile.position, projectile.width, projectile.height, 246, -projectile.velocity.X * 0.2f, -projectile.velocity.Y * 0.2f, 100, new Color(), 2f);
                Main.dust[index2].noGravity = true;
                Main.dust[index2].velocity *= 2f;
                int index3 = Dust.NewDust(projectile.position, projectile.width, projectile.height, 246, -projectile.velocity.X * 0.2f, -projectile.velocity.Y * 0.2f, 100, new Color(), 1f);
                Main.dust[index3].velocity *= 2f;
            }
        }
    }
}