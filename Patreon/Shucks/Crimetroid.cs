using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Patreon.Shucks
{
    public class Crimetroid : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[projectile.type] = 4;
            Main.projPet[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.netImportant = true;
            projectile.width = 20;
            projectile.height = 20;
            projectile.aiStyle = -1;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.timeLeft *= 5;
        }
        
        public override void AI()
        {
            Player player = Main.player[projectile.owner];
            PatreonPlayer modPlayer = player.GetModPlayer<PatreonPlayer>();

            if (!player.active || player.dead || player.ghost)
            {
                modPlayer.Crimetroid = false;
            }
            
            if (modPlayer.Crimetroid)
            {
                projectile.timeLeft = 2;
            }

            if (++projectile.frameCounter > 6)
            {
                projectile.frameCounter = 0;
                if (++projectile.frame >= Main.projFrames[projectile.type])
                    projectile.frame = 0;
            }

            float dist = projectile.Distance(player.Top);
            if (dist > 400)
                projectile.tileCollide = false;
            if (dist > 2000)
                projectile.Center = player.Center;

            Vector2 focus = player.Top;
            if (dist < 60)
            {
                projectile.localAI[0] = 0;

                focus = projectile.Center + projectile.velocity;
                if (focus == projectile.Center)
                    focus.Y -= 1;
                if (!projectile.tileCollide)
                    projectile.tileCollide = Collision.CanHitLine(projectile.position, projectile.width, projectile.height, player.position, player.width, player.height);
            }
            else if (projectile.localAI[0] == 0)
            {
                projectile.localAI[0] = MathHelper.ToRadians(Main.rand.NextFloat(45));
                if (Main.rand.NextBool())
                    projectile.localAI[0] *= -1;
            }

            projectile.localAI[0] *= 0.99f;

            float maxSpeed = 8f;
            if (projectile.localAI[1] > 0)
            {
                projectile.localAI[1]--;
                maxSpeed *= 0.5f;
            }
            if (!player.velocity.HasNaNs() && player.velocity != Vector2.Zero)
                maxSpeed += player.velocity.Length() / 2;

            Vector2 change = projectile.DirectionTo(focus) * maxSpeed;
            if (projectile.localAI[0] != 0)
            {
                change = change.RotatedBy(projectile.localAI[0]);
            }
            const float increment = 22;
            projectile.velocity = (projectile.velocity * (increment - 1) + change) / increment;

            projectile.rotation = projectile.velocity.X * .05f;
            if (Math.Abs(projectile.rotation) > MathHelper.ToRadians(75))
                projectile.rotation = MathHelper.ToRadians(75) * Math.Sign(projectile.rotation);
        }

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough)
        {
            fallThrough = true;
            return base.TileCollideStyle(ref width, ref height, ref fallThrough);
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            projectile.localAI[1] = 15;
            if (projectile.velocity.X != oldVelocity.X)
                projectile.velocity.X = -oldVelocity.X * 0.75f;
            if (projectile.velocity.Y != oldVelocity.Y)
                projectile.velocity.Y = -oldVelocity.Y * 0.75f;
            return false;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            if (projectile.owner == Main.myPlayer)
            {
                projectile.vampireHeal(1, projectile.Center);
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture2D13 = Main.projectileTexture[projectile.type];
            int num156 = Main.projectileTexture[projectile.type].Height / Main.projFrames[projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new Rectangle(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;

            Color color26 = lightColor;
            color26 = projectile.GetAlpha(color26);

            SpriteEffects effects = projectile.spriteDirection > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Main.spriteBatch.Draw(texture2D13, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color26, projectile.rotation, origin2, projectile.scale, effects, 0f);


            Texture2D texture2D14 = ModContent.GetTexture(Texture + "Jelly");
            const float jellyOpacity = 0.5f;
            Main.spriteBatch.Draw(texture2D14, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color26 * jellyOpacity, projectile.rotation, origin2, projectile.scale, effects, 0f);
            return false;
        }
    }
}