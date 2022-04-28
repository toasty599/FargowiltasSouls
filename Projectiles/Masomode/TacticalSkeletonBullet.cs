using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Masomode
{
    public class TacticalSkeletonBullet : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Meteor Shot");
        }

        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.MeteorShot);
            AIType = ProjectileID.MeteorShot;
            Projectile.DamageType = DamageClass.Default;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.penetrate = 3;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Projectile.penetrate > 1)
            {
                Collision.HitTiles(Projectile.position, Projectile.velocity, Projectile.width, Projectile.height);
                Terraria.Audio.SoundEngine.PlaySound(SoundID.Item10, Projectile.position);
                Projectile.penetrate--;

                if (Projectile.velocity.X != Projectile.oldVelocity.X)
                    Projectile.velocity.X = -Projectile.oldVelocity.X;
                if (Projectile.velocity.Y != Projectile.oldVelocity.Y)
                    Projectile.velocity.Y = -Projectile.oldVelocity.Y;

                return false;
            }
            return true;
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.OnFire, 360);
            target.AddBuff(BuffID.Burning, 180);
        }
    }
}