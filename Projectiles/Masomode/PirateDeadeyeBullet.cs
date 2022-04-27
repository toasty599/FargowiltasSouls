using FargowiltasSouls.Buffs.Masomode;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Masomode
{
    public class PirateDeadeyeBullet : ModProjectile
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
            Projectile.penetrate = 6; //used for bouncing
            Projectile.timeLeft = 600;
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.AddBuff(ModContent.BuffType<Midas>(), 600);
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Projectile.penetrate-- > 1)
            {
                Collision.HitTiles(Projectile.position, Projectile.velocity, Projectile.width, Projectile.height);
                Terraria.Audio.SoundEngine.PlaySound(SoundID.Item10, Projectile.position);

                if (Projectile.velocity.X != Projectile.oldVelocity.X)
                    Projectile.velocity.X = -Projectile.oldVelocity.X;
                if (Projectile.velocity.Y != Projectile.oldVelocity.Y)
                    Projectile.velocity.Y = -Projectile.oldVelocity.Y;

                return false;
            }
            return true;
        }
    }
}