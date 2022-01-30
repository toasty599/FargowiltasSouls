using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.DeviBoss
{
    public class DeviLightBall : Masomode.LightBall
    {
        public override string Texture => "FargowiltasSouls/Projectiles/Masomode/LightBall";

        public override void SetDefaults()
        {
            base.SetDefaults();

            projectile.penetrate = -1;
            projectile.timeLeft = 270;
        }

        public override void Kill(int timeLeft)
        {
            base.Kill(timeLeft);

            Terraria.Audio.SoundEngine.PlaySound(SoundID.Item10, projectile.position);
            for (int index1 = 0; index1 < 10; ++index1)
            {
                int index2 = Dust.NewDust(projectile.position, projectile.width, projectile.height, 246, -projectile.velocity.X * 0.2f, -projectile.velocity.Y * 0.2f, 100, new Color(), 2f);
                Main.dust[index2].noGravity = true;
                Main.dust[index2].velocity *= 2f;
                int index3 = Dust.NewDust(projectile.position, projectile.width, projectile.height, 246, -projectile.velocity.X * 0.2f, -projectile.velocity.Y * 0.2f, 100, new Color(), 1f);
                Main.dust[index3].velocity *= 2f;
            }

            if (projectile.ai[0] == 0f && Main.netMode != NetmodeID.MultiplayerClient)
            {
                Projectile.NewProjectile(projectile.Center, Vector2.Normalize(projectile.velocity), ModContent.ProjectileType<Deathrays.DeviLightBeam>(),
                    projectile.damage, projectile.knockBack, projectile.owner, MathHelper.ToRadians(60f / 60) * Math.Sign(projectile.ai[1]));
            }
        }
    }
}