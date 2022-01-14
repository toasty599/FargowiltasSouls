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
            projectile.extraUpdates = 0;
            projectile.width = 15;
            projectile.height = 17;
            projectile.friendly = true;
            //projectile.penetrate = -1;
            Projectile.DamageType = DamageClass.Magic;
            projectile.scale = 0.5f;
            projectile.timeLeft = 100;
        }

        public override void AI()
        {
            projectile.rotation += 0.2f;
            projectile.scale += .02f;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            projectile.velocity = oldVelocity;
            return true;
        }

        public override void Kill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.NPCDeath52, projectile.Center);

            if (projectile.owner == Main.myPlayer)
            {
                int proj2 = ModContent.ProjectileType<TopHatSquirrelLaser>();

                FargoSoulsUtil.XWay(16, projectile.Center, proj2, projectile.velocity.Length() * 2f, projectile.damage * 4, projectile.knockBack);

                for (int i = 0; i < 50; i++)
                {
                    Vector2 pos = projectile.Center + Vector2.Normalize(projectile.velocity) * Main.rand.NextFloat(600, 1800) +
                        Vector2.Normalize(projectile.velocity.RotatedBy(MathHelper.Pi / 2)) * Main.rand.NextFloat(-900, 900);

                    Projectile.NewProjectile(pos, -projectile.velocity * Main.rand.NextFloat(2f, 3f), proj2,
                        projectile.damage * 4, projectile.knockBack, Main.myPlayer);
                }
            }
        }
    }
}