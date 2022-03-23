using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.MutantBoss
{
    public class MutantGlowything : ModProjectile
    {

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            DisplayName.SetDefault("Retiray telegraph");
        }

        public override void SetDefaults()
        {
            projectile.width = 18;
            projectile.height = 18;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.aiStyle = -1;
            projectile.scale = 0.5f;
            projectile.alpha = 0;
            CooldownSlot = 1;
        }

        Vector2 spawnPoint;

        float scalefactor;
        public override void AI()
        {
            projectile.rotation = projectile.ai[0];

            if (spawnPoint == Vector2.Zero)
                spawnPoint = projectile.Center;
            projectile.Center = spawnPoint + Vector2.UnitX.RotatedBy(projectile.ai[0]) * 96 * projectile.scale;

            if(projectile.scale < 4f) //grow over time
            {
                projectile.scale += 0.2f;
            }
            else //if full size, start fading away
            {
                projectile.scale = 4f;
                projectile.alpha += 10;
            }
            if(projectile.alpha > 255) //die if fully faded away
            {
                projectile.Kill();
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D glow = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            int rect1 = glow.Height;
            int rect2 = 0;
            Rectangle glowrectangle = new Rectangle(0, rect2, glow.Width, rect1);
            Vector2 gloworigin2 = glowrectangle.Size() / 2f;
            Color glowcolor = new Color(255, 0, 0, 0);

            float scale = projectile.scale;
            Main.EntitySpriteDraw(glow, projectile.Center + projectile.Size / 2f - Main.screenPosition + new Vector2(0, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(glowrectangle), projectile.GetAlpha(glowcolor),
                projectile.rotation, gloworigin2, scale * 2, SpriteEffects.None, 0);


            return false;
        }
    }
}