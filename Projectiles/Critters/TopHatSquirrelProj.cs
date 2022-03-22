using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles
{
    internal class TopHatSquirrelProj : ModProjectile
    {
        public int Counter = 1;

        public override string Texture => "FargowiltasSouls/Items/Weapons/Misc/TophatSquirrelWeapon";

        public override void SetDefaults()
        {
            Projectile.extraUpdates = 0;
            Projectile.width = 15;
            Projectile.height = 17;
            Projectile.friendly = true;
            //Projectile.penetrate = -1;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.scale = 0.5f;
            Projectile.timeLeft = 100;
        }

        public override void AI()
        {
            Projectile.rotation += 0.2f;
            Projectile.scale += .02f;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.velocity = oldVelocity;
            return true;
        }

        public override void Kill(int timeLeft)
        {
            Terraria.Audio.SoundEngine.PlaySound(SoundID.NPCDeath52, Projectile.Center);

            if (Projectile.owner == Main.myPlayer)
            {
                int proj2 = ModContent.ProjectileType<TopHatSquirrelLaser>();

                FargoSoulsUtil.XWay(16, Projectile.GetProjectileSource_FromThis(), Projectile.Center, proj2, Projectile.velocity.Length() * 2f, Projectile.damage * 4, Projectile.knockBack);

                for (int i = 0; i < 50; i++)
                {
                    Vector2 pos = Projectile.Center + Vector2.Normalize(Projectile.velocity) * Main.rand.NextFloat(600, 1800) +
                        Vector2.Normalize(Projectile.velocity.RotatedBy(MathHelper.Pi / 2)) * Main.rand.NextFloat(-900, 900);

                    Projectile.NewProjectile(Projectile.GetProjectileSource_FromThis(), pos, -Projectile.velocity * Main.rand.NextFloat(2f, 3f), proj2,
                        Projectile.damage * 4, Projectile.knockBack, Main.myPlayer);
                }
            }
        }
    }
}