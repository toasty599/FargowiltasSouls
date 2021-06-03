using FargowiltasSouls.Toggler;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Souls
{
    public class NecroGrave : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Necro Grave");
        }

        public override void SetDefaults()
        {
            projectile.width = 48;
            projectile.height = 32;
            projectile.aiStyle = -1;
            projectile.tileCollide = true;
            projectile.ignoreWater = true;
            projectile.penetrate = -1;
            projectile.timeLeft = 1800;
        }

        public override bool CanDamage()
        {
            return false;
        }

        public override void AI()
        {
            Player player = Main.player[projectile.owner];

            Lighting.AddLight(projectile.Center, 1f, 1f, 1f);

            if (projectile.localAI[0] == 0)
            {
                projectile.localAI[0] = 1;
                Main.PlaySound(SoundID.Item2, projectile.Center);
            }

            projectile.velocity.Y = projectile.velocity.Y + 0.2f;
            if (projectile.velocity.Y > 16f)
            {
                projectile.velocity.Y = 16f;
            }

            if (player.Hitbox.Intersects(projectile.Hitbox))
            {
                if (player.GetModPlayer<FargoPlayer>().NecroEnchant && player.GetToggleValue("Necro"))
                    Projectile.NewProjectile(projectile.Center, new Vector2(0, -20), ModContent.ProjectileType<DungeonGuardianNecro>(), (int)projectile.ai[0], 1, projectile.owner);

                //dust ring
                int num1 = 36;
                for (int index1 = 0; index1 < num1; ++index1)
                {
                    Vector2 vector2_1 = (Vector2.Normalize(projectile.velocity) * new Vector2((float)projectile.width / 2f, (float)projectile.height) * 0.75f).RotatedBy((double)(index1 - (num1 / 2 - 1)) * 6.28318548202515 / (double)num1, new Vector2()) + projectile.Center;
                    Vector2 vector2_2 = vector2_1 - projectile.Center;
                    int index2 = Dust.NewDust(vector2_1 + vector2_2, 0, 0, DustID.Blood, vector2_2.X * 2f, vector2_2.Y * 2f, 100, new Color(), 1.4f);
                    Main.dust[index2].noGravity = true;
                    Main.dust[index2].noLight = true;
                    Main.dust[index2].velocity = vector2_2;
                }

                projectile.Kill();
            }
        }

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough)
        {
            fallThrough = false;
            return true;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            projectile.position += projectile.velocity;
            projectile.velocity = Vector2.Zero;
            return false;
        }
    }
}
