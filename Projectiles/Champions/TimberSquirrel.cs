using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Champions
{
    internal class TimberSquirrel : ModProjectile
    {
        public int Counter = 1;

        public override string Texture => "FargowiltasSouls/Items/Weapons/Misc/TophatSquirrelWeapon";

        public override void SetDefaults()
        {
            projectile.width = 19;
            projectile.height = 19;
            projectile.hostile = true;
            projectile.scale = 1f;
            projectile.timeLeft = 120;
            projectile.penetrate = -1;
            projectile.GetGlobalProjectile<FargoGlobalProjectile>().ImmuneToDeletion = true;
        }

        public override bool CanDamage()
        {
            return false;
        }

        public override void AI()
        {
            projectile.rotation += 0.2f;

            if (Counter >= 45)
                projectile.scale += .1f;

            Counter++;
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