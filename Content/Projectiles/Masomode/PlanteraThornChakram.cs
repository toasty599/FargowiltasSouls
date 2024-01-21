using FargowiltasSouls.Core.Systems;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Audio;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.Masomode
{
    public class PlanteraThornChakram : ModProjectile
    {
        public override string Texture => FargoSoulsUtil.VanillaTextureProjectile(ProjectileID.ThornChakram);

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 4;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }
        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.hostile = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 60 * 10;

            Projectile.scale = 1;
        }
        ref float Timer => ref Projectile.ai[2];
        //ref float OriginalVelX => ref Projectile.ai[0];
        //ref float OriginalVelY => ref Projectile.ai[1];
        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, .4f, 1.2f, .4f); //glow in the dark

            if (Projectile.localAI[0] == 0) //random rotation direction
            {
                Projectile.localAI[0] = Main.rand.NextBool() ? 1 : -1;
            }
            Projectile.rotation += 0.3f * Projectile.localAI[0];

            float MaxSpeed = Projectile.ai[0];
            if (Timer <= 20)
            {
                if (Projectile.velocity.Length() < MaxSpeed)
                    Projectile.velocity *= 1.08f;
            }
            /*
            const int turnTime = 80;
            if (Timer < turnTime)
            {
                OriginalVelX = Projectile.velocity.X;
                OriginalVelY = Projectile.velocity.Y;
            }
            const int turnDuration = 20;
            if (Timer >= turnTime && Timer <= turnTime + turnDuration)
            {
                
                Projectile.velocity.X = (float)Utils.Lerp(OriginalVelX, -OriginalVelX, (Timer - turnTime) / turnDuration);
                Projectile.velocity.Y = (float)Utils.Lerp(OriginalVelY, -OriginalVelY, (Timer - turnTime) / turnDuration);
            }
            */
            Timer++;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[Projectile.type]; i++)
            {
                Color color2 = Color.LimeGreen * 0.75f;
                color2 *= (float)(ProjectileID.Sets.TrailCacheLength[Projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[Projectile.type];
                Vector2 pos = Projectile.oldPos[i] + Projectile.Size / 2;
                float rot = Projectile.oldRot[i];
                FargoSoulsUtil.GenericProjectileDraw(Projectile, color2, drawPos: pos, rotation: rot);
            }

            FargoSoulsUtil.GenericProjectileDraw(Projectile, lightColor);
            return false;
        }
    }
}
