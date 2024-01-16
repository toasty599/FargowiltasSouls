using FargowiltasSouls.Content.Bosses.VanillaEternity;
using FargowiltasSouls.Content.Projectiles.Masomode;
using FargowiltasSouls.Core.Globals;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Content.Items.Accessories.Expert;
using Microsoft.Xna.Framework;

namespace FargowiltasSouls.Content.Projectiles.Minions
{
    public class MinosPrime : ModProjectile
    {

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Lunar Cultist");
            //Main.projFrames[Projectile.type] = 12;
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 9;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
            //ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.netImportant = true;
            Projectile.width = 84;
            Projectile.height = 98;
            Projectile.timeLeft *= 5;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.minion = true;
            Projectile.DamageType = DamageClass.Generic;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
        }
        public ref float Timer => ref Projectile.ai[0];
        public float Wobble = 0;

        public override void AI()
        {
            Main.projFrames[Type] = 1;
            Player player = Main.player[Projectile.owner];
            if (player.active && !player.dead && player.HasEffect<PrimeSoulEffect>())
                Projectile.timeLeft = 2;

            Wobble = 20f * (float)Math.Sin(MathHelper.TwoPi * (Timer / 60f));
            Movement(player);
            Main.NewText(player.FargoSouls().PrimeSoulItemCount);
        }
        public void Movement(Player player)
        {
            Vector2 mousePos = Main.MouseWorld;
            int offset = Math.Sign(mousePos.X - player.Center.X);
            Projectile.spriteDirection = Projectile.direction = -offset;
            offset *= 225;
            Vector2 idlePosition = mousePos + Vector2.UnitX * offset;
            Vector2 toIdlePosition = idlePosition - Projectile.Center;
            float distance = toIdlePosition.Length();
            float speed = 38f;
            float inertia = 15f;
            toIdlePosition.Normalize();
            toIdlePosition *= speed;
            Projectile.velocity = (Projectile.velocity * (inertia - 1f) + toIdlePosition) / inertia;
            if (distance == 0)
                Projectile.velocity = Vector2.Zero;
            if (distance < Projectile.velocity.Length())
                Projectile.velocity = Vector2.Normalize(Projectile.velocity) * distance;
            if (Projectile.velocity == Vector2.Zero && distance > 10)
            {
                Projectile.velocity.X = -0.15f;
                Projectile.velocity.Y = -0.05f;
            }
        }

        public override bool? CanCutTiles()
        {
            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            int num156 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;

            Color color26 = lightColor;
            color26 = Projectile.GetAlpha(color26);

            //Texture2D texture2D14 = ModContent.Request<Texture2D>("FargowiltasSouls/Content/Projectiles/Minions/LunarCultistTrail", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[Projectile.type]; i += 3)
            {
                Color color27 = color26;
                color27 *= (float)(ProjectileID.Sets.TrailCacheLength[Projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[Projectile.type];
                Vector2 value4 = Projectile.oldPos[i];
                float num165 = Projectile.oldRot[i];
                Main.EntitySpriteDraw(texture2D13, value4 + Projectile.Size / 2f - Main.screenPosition + new Vector2(0, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color27, num165, origin2, Projectile.scale, Projectile.spriteDirection > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0);
            }

            Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Projectile.GetAlpha(lightColor), Projectile.rotation, origin2, Projectile.scale, Projectile.spriteDirection > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0);
            return false;
        }
    }
}
