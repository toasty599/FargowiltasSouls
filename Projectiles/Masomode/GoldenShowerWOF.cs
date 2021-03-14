using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Masomode
{
    public class GoldenShowerWOF : ModProjectile
    {
        public override string Texture => "Terraria/Projectile_288";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Golden Shower");
        }

        public override void SetDefaults()
        {
            projectile.width = 10;
            projectile.height = 10;
            projectile.aiStyle = -1;
            projectile.alpha = 255;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.timeLeft = 300;
            projectile.hostile = true;

            projectile.extraUpdates = 2;
        }

        public override bool? CanCutTiles()
        {
            return false;
        }

        public override void AI()
        {
            if (projectile.localAI[1] == 0)
            {
                projectile.localAI[1] = 1;
                Main.PlaySound(SoundID.Item17, projectile.Center);
            }

            for (int i = 0; i < 2; i++) //vanilla dusts
            {
                for (int j = 0; j < 2; ++j)
                {
                    int d = Dust.NewDust(projectile.position, projectile.width, projectile.height, 170, 0.0f, 0.0f, 100, default, 0.75f);
                    Main.dust[d].noGravity = true;
                    Main.dust[d].velocity *= 0.1f;
                    Main.dust[d].velocity += projectile.velocity * 0.5f;
                    Main.dust[d].position -= projectile.velocity / 3 * j;
                }
                if (Main.rand.Next(8) == 0)
                {
                    int d = Dust.NewDust(projectile.position, projectile.width, projectile.height, 170, 0.0f, 0.0f, 100, default, 0.325f);
                    Main.dust[d].velocity *= 0.25f;
                    Main.dust[d].velocity += projectile.velocity * 0.5f;
                }
            }

            if (--projectile.ai[0] < 0)
                projectile.tileCollide = true;

            projectile.velocity.Y += 0.5f;
            projectile.rotation = projectile.velocity.ToRotation() + (float)Math.PI / 2f;
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.Ichor, 900);
            target.AddBuff(BuffID.OnFire, 300);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture2D13 = Main.projectileTexture[projectile.type];
            int num156 = Main.projectileTexture[projectile.type].Height / Main.projFrames[projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new Rectangle(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;
            Main.spriteBatch.Draw(texture2D13, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), projectile.GetAlpha(lightColor), projectile.rotation, origin2, projectile.scale, SpriteEffects.None, 0f);
            return false;
        }
    }
}