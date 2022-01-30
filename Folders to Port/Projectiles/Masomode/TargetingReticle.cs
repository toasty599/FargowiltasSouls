using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Masomode
{
    public class TargetingReticle : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Targeting Reticle");
        }

        public override void SetDefaults()
        {
            projectile.width = 110;
            projectile.height = 110;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.aiStyle = -1;
            projectile.penetrate = -1;
            projectile.hostile = true;
            projectile.alpha = 255;
            projectile.timeLeft = 90;
            //CooldownSlot = 1;
        }

        public override bool? CanDamage()
        {
            return false;
        }

        public override void AI()
        {
            NPC npc = FargoSoulsUtil.NPCExists(projectile.ai[0]);
            if (npc != null && npc.HasPlayerTarget)
            {
                projectile.alpha -= 4;
                if (projectile.alpha < 0) //fade in
                    projectile.alpha = 0;

                int modifier = Math.Min(60, 90 - projectile.timeLeft);

                projectile.scale = 4f - 3f / 60f * modifier; //start big, shrink down

                projectile.Center = npc.Center;
                projectile.velocity = Main.player[npc.target].Center - projectile.Center;
                projectile.velocity = projectile.velocity / 60 * modifier; //move from npc to player
                projectile.rotation = (float)Math.PI * 2 / 30 * modifier * Math.Sign(projectile.velocity.X);
            }
            else
            {
                projectile.Kill();
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, 255, 255, 128) * (1f - projectile.alpha / 255f);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            int num156 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new Rectangle(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;
            Main.EntitySpriteDraw(texture2D13, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), projectile.GetAlpha(lightColor), projectile.rotation, origin2, projectile.scale, SpriteEffects.None, 0);
            return false;
        }
    }
}