using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.JungleMimic
{
    public class MahogunyLeafProjectile : ModProjectile
	{
		public override void SetStaticDefaults(){
			Main.projFrames[projectile.type] = 4;
		}
        public override void SetDefaults()
        {
             aiType = 14;
            projectile.width = 5;
            projectile.height = 9;
            projectile.friendly = true;
             projectile.hostile = false;
             projectile.ranged = true;
             projectile.penetrate = 1;
             projectile.ignoreWater = false;
             projectile.tileCollide = true;
        }
        public override void AI()
        {
            projectile.rotation = projectile.velocity.ToRotation() + MathHelper.PiOver2;
            if (++projectile.frameCounter >= 5)
            {
	            projectile.frameCounter = 0;
	            projectile.frame = ++projectile.frame % Main.projFrames[projectile.type];
            }
            if (Main.rand.NextBool(3))
                Dust.NewDust(projectile.position, projectile.width, projectile.height, DustID.GrassBlades, projectile.velocity.X * 0.25f, projectile.velocity.Y * 0.25f, 150, default(Color), 0.7f);
        }
        public override void Kill(int timeLeft)
        {
            Collision.HitTiles(projectile.position + projectile.velocity, projectile.velocity, projectile.width, projectile.height);
            Main.PlaySound(SoundID.Grass, projectile.position);
            for (int d = 0; d < 35; d++)
            {
                Dust.NewDust(projectile.position, projectile.width, projectile.height, mod.DustType("Leaf_Dust"), projectile.velocity.X * 0.25f, projectile.velocity.Y * 0.25f, 150, default(Color), 0.7f);
            }
        }
    }
}
