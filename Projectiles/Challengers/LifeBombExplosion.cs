using FargowiltasSouls.NPCs.Challengers;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Challengers
{

	public class LifeBombExplosion : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Life Bomb");
            Main.projFrames[Projectile.type] = 3;
        }
		public override void SetDefaults()
		{
			Projectile.width = 50;
			Projectile.height = 50;
			Projectile.aiStyle = 0;
			Projectile.hostile = true;
			AIType = 14;
			Projectile.penetrate = 1;
			Projectile.tileCollide = false;
			Projectile.ignoreWater = true;
			Projectile.light = 0.5f;
		}

        public override void AI()
		{
            if (++Projectile.frameCounter >= 5)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= 3)
                    Projectile.frame = 0;
            }
			Projectile.rotation += 2f;
			if (Main.rand.NextBool(6))
			{
				int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 87);
                Main.dust[d].noGravity = true;
                Main.dust[d].velocity *= 0.5f;
            }
			
            if (Projectile.ai[0] > 2400f || NPC.CountNPCS(ModContent.NPCType<LifeChallenger>()) < 1)
			{
				int d2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 87);
				Main.dust[d2].noGravity = true;
				Main.dust[d2].velocity *= 0.5f;
				Projectile.Kill();
			}
			Projectile.ai[0] += 1f;
		}
	}
}
