using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.AbomBoss
{
    public class AbomSickleSplit1 : AbomSickle
    {
        public override string Texture => "FargowiltasSouls/Projectiles/AbomBoss/AbomSickle";

        public override void SetDefaults()
        {
            base.SetDefaults();
            projectile.timeLeft = 90;
        }

        public override void AI()
        {
            if (projectile.localAI[0] == 0)
            {
                projectile.localAI[0] = 1;
                SoundEngine.PlaySound(SoundID.Item8, projectile.Center);
            }
            projectile.rotation += 0.8f;
            /*for (int i = 0; i < 6; i++)
            {
                Vector2 offset = new Vector2(0, -20).RotatedBy(projectile.rotation);
                offset = offset.RotatedByRandom(MathHelper.Pi / 6);
                int d = Dust.NewDust(projectile.Center, 0, 0, 87, 0f, 0f, 150);
                Main.dust[d].position += offset;
                float velrando = Main.rand.Next(20, 31) / 10;
                Main.dust[d].velocity = projectile.velocity / velrando;
                Main.dust[d].noGravity = true;
            }*/
        }

        public override void Kill(int timeLeft)
        {
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                for (int i = 0; i < 8; i++)
                {
                    Vector2 vel = Vector2.Normalize(projectile.velocity).RotatedBy(Math.PI / 4 * i);
                    Projectile.NewProjectile(projectile.Center, vel, ModContent.ProjectileType<AbomSickleSplit2>(), projectile.damage, 0f, projectile.owner);
                }
            }
        }
    }
}