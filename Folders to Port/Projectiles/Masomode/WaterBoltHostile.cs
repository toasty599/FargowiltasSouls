using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Masomode
{
    public class WaterBoltHostile : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_27";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Water Bolt");
        }

        public override void SetDefaults()
        {
            projectile.CloneDefaults(ProjectileID.WaterBolt);
            AIType = ProjectileID.WaterBolt;
            projectile.friendly = false;
            projectile.magic = false;
            projectile.hostile = true;
            projectile.timeLeft = 300;
            projectile.alpha = 255;
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.Wet, 1200);
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Terraria.Audio.SoundEngine.PlaySound(SoundID.Item10, projectile.position);
            if (projectile.velocity.X != projectile.oldVelocity.X)
                projectile.velocity.X = -projectile.oldVelocity.X;
            if (projectile.velocity.Y != projectile.oldVelocity.Y)
                projectile.velocity.Y = -projectile.oldVelocity.Y;
            return false;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
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
