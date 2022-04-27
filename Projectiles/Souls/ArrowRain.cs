using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Souls
{
    public class ArrowRain : ModProjectile
    {
        private bool launchArrow = true;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Arrow Rain");
        }

        public override void SetDefaults()
        {
            Projectile.width = 1;
            Projectile.height = 1;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 500;
        }

        public override bool? CanDamage()
        {
            return false;
        }

        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];

            //follow the cursor and double fire rate with red riding
            if (owner.GetModPlayer<FargoSoulsPlayer>().RedEnchantActive)
            {
                Projectile.Center = new Vector2(Main.MouseWorld.X, Main.MouseWorld.Y - 400);
                //launchArrow = true;
            }

            //delay
            if (Projectile.timeLeft > 470)
            {
                return;
            }

            if (launchArrow)
            {
                Vector2 position = new Vector2(Projectile.Center.X + Main.rand.Next(-100, 100), Projectile.Center.Y + Main.rand.Next(-75, 75));

                float direction = Projectile.ai[1];
                //Vector2 velocity;

                Vector2 mouse = Main.MouseWorld;
                //Vector2 pos = new Vector2(mouse.X - player.direction * 100, mouse.Y - 800);
                Vector2 velocity = Vector2.Normalize(mouse - Projectile.position) * 25;


                //if (direction == 1)
                //{
                //    velocity = new Vector2(Main.rand.NextFloat(0, 2), Main.rand.NextFloat(20, 25));
                //}
                //else
                //{
                //    velocity = new Vector2(Main.rand.NextFloat(-2, 0), Main.rand.NextFloat(20, 25));
                //}

                int p = Projectile.NewProjectile(Projectile.GetSource_FromThis(), position, velocity, (int)Projectile.ai[0], Projectile.damage, 0, Projectile.owner);
                Main.projectile[p].noDropItem = true;

                launchArrow = false;
            }
            else
            {
                launchArrow = true;
            }
        }
    }
}