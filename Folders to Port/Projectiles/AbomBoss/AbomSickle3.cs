using Microsoft.Xna.Framework;
using System.Linq;
using Terraria;
using Terraria.ID;

namespace FargowiltasSouls.Projectiles.AbomBoss
{
    public class AbomSickle3 : AbomSickle
    {
        public override string Texture => "FargowiltasSouls/Projectiles/AbomBoss/AbomSickle";

        public override void SetDefaults()
        {
            base.SetDefaults();
            //projectile.timeLeft = 3600;
        }

        public override void AI()
        {
            if (projectile.localAI[0] == 0)
            {
                projectile.localAI[0] = projectile.Center.X;
                projectile.localAI[1] = projectile.Center.Y;
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

            if (projectile.ai[1] == 0)
            {
                Player target = FargoSoulsUtil.PlayerExists(projectile.ai[0]);
                if (target != null)
                {
                    Vector2 spawnPoint = new Vector2(projectile.localAI[0], projectile.localAI[1]);
                    if (projectile.Distance(spawnPoint) > target.Distance(spawnPoint) - 160)// && Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        projectile.ai[1] = 1;
                        projectile.velocity.Normalize();
                        projectile.timeLeft = 300;
                        projectile.netUpdate = true;

                        /*foreach (Projectile p in Main.projectile.Where(p => p.active && p.hostile && p.type == projectile.type && p.ai[1] == 0))
                        {
                            p.ai[1] = 1;
                            p.velocity.Normalize();
                            p.timeLeft = 300;
                            p.netUpdate = true;
                        }*/
                    }
                }
            }
            else
            {
                if (++projectile.ai[1] < 60)
                    projectile.velocity *= 1.065f;
            }
        }
    }
}