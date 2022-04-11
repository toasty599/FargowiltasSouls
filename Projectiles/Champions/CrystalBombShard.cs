using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Champions
{
    public class CrystalBombShard : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_90";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Crystal Shard");
        }

        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.QueenSlimeMinionBlueSpike);
            AIType = ProjectileID.QueenSlimeMinionBlueSpike;
            Projectile.scale *= 1.5f;
            Projectile.timeLeft = 30;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Projectile.velocity.X != oldVelocity.X)
                Projectile.velocity.X = -oldVelocity.X;
            if (Projectile.velocity.Y != oldVelocity.Y)
                Projectile.velocity.Y = -oldVelocity.Y;
            return false;
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            if (FargoSoulsWorld.EternityMode)
                target.AddBuff(BuffID.Chilled, 180);
            target.AddBuff(BuffID.Frostburn, 180);
        }
    }
}