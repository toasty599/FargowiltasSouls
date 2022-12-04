using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Deathrays
{
    public abstract class BaseDeathray : ModProjectile
    {
        protected float maxTime;
        protected readonly float transparency; //THIS IS A 0 TO 1 PERCENTAGE, NOT AN ALPHA
        protected readonly float hitboxModifier;
        protected readonly int grazeCD;

        //by default, real hitbox is slightly more than the "white" of a vanilla ray
        //remember that the value passed into function is total width, i.e. on each side the distance is only half the width
        protected readonly int drawDistance;

        protected BaseDeathray(float maxTime, float transparency = 0f, float hitboxModifier = 1f, int drawDistance = 2400, int grazeCD = 15)
        {
            this.maxTime = maxTime;
            this.transparency = transparency;
            this.hitboxModifier = hitboxModifier;
            this.drawDistance = drawDistance;
            this.grazeCD = grazeCD;
        }

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            ProjectileID.Sets.DrawScreenCheckFluff[Projectile.type] = drawDistance;
        }

        public override void SetDefaults() //MAKE SURE YOU CALL BASE.SETDEFAULTS IF OVERRIDING
        {
            Projectile.width = 48;
            Projectile.height = 48;
            Projectile.hostile = true;
            Projectile.alpha = 255;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 3600;

            CooldownSlot = 1; //not in warning line, test?

            Projectile.GetGlobalProjectile<FargoSoulsGlobalProjectile>().GrazeCheck =
                Projectile =>
                {
                    float num6 = 0f;
                    if (CanDamage() != false && Collision.CheckAABBvLineCollision(Main.LocalPlayer.Hitbox.TopLeft(), Main.LocalPlayer.Hitbox.Size(), Projectile.Center,
                        Projectile.Center + Projectile.velocity * Projectile.localAI[1], 22f * Projectile.scale + Main.LocalPlayer.GetModPlayer<FargoSoulsPlayer>().GrazeRadius * 2f + Player.defaultHeight, ref num6))
                    {
                        return true;
                    }
                    return false;
                };

            Projectile.hide = true; //fixes weird issues on spawn with scaling
            Projectile.GetGlobalProjectile<FargoSoulsGlobalProjectile>().DeletionImmuneRank = 1;
        }

        public override void PostAI()
        {
            if (Projectile.hide)
            {
                Projectile.hide = false;
                if (Projectile.friendly)
                    Projectile.GetGlobalProjectile<FargoSoulsGlobalProjectile>().DeletionImmuneRank = 2;
            }

            if (Projectile.GetGlobalProjectile<FargoSoulsGlobalProjectile>().GrazeCD > grazeCD)
                Projectile.GetGlobalProjectile<FargoSoulsGlobalProjectile>().GrazeCD = grazeCD;
        }

        public override Color? GetAlpha(Color lightColor) => new Color(255, 255, 255, 0) * 0.95f;

        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.velocity == Vector2.Zero)
            {
                return false;
            }
            Texture2D rayBeg = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            Texture2D rayMid = ModContent.Request<Texture2D>($"{Texture}2", AssetRequestMode.ImmediateLoad).Value;
            Texture2D rayEnd = ModContent.Request<Texture2D>($"{Texture}3", AssetRequestMode.ImmediateLoad).Value;
            float num223 = Projectile.localAI[1];
            Color color44 = Projectile.GetAlpha(lightColor);
            color44 = Color.Lerp(color44, Color.Transparent, transparency);
            Main.EntitySpriteDraw(rayBeg, Projectile.Center - Main.screenPosition, null, color44, Projectile.rotation, rayBeg.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);
            num223 -= (float)(rayBeg.Height / 2 + rayEnd.Height) * Projectile.scale;
            Vector2 drawPos = Projectile.Center;
            drawPos += Projectile.velocity * Projectile.scale * (float)rayBeg.Height / 2f;
            if (num223 > 0f)
            {
                float num224 = 0f;
                Rectangle rectangle7 = new Rectangle(0, 0, rayMid.Width, rayMid.Height);
                while (num224 + 1f < num223)
                {
                    if (num223 - num224 < (float)rectangle7.Height)
                    {
                        rectangle7.Height = (int)(num223 - num224);
                    }
                    Main.EntitySpriteDraw(rayMid, drawPos - Main.screenPosition, new Microsoft.Xna.Framework.Rectangle?(rectangle7), color44, Projectile.rotation, new Vector2((float)(rectangle7.Width / 2), 0f), Projectile.scale, SpriteEffects.None, 0);
                    num224 += (float)rectangle7.Height * Projectile.scale;
                    drawPos += Projectile.velocity * (float)rectangle7.Height * Projectile.scale;
                    rectangle7.Y += 16;
                    if (rectangle7.Y + rectangle7.Height > rayMid.Height)
                    {
                        rectangle7.Y = 0;
                    }
                }
            }
            Main.EntitySpriteDraw(rayEnd, drawPos - Main.screenPosition, null, color44, Projectile.rotation, rayEnd.Frame(1, 1, 0, 0).Top(), Projectile.scale, SpriteEffects.None, 0);
            return false;
        }

        public override void CutTiles()
        {
            DelegateMethods.tilecut_0 = TileCuttingContext.AttackProjectile;
            Vector2 unit = Projectile.velocity;
            Utils.PlotTileLine(Projectile.Center, Projectile.Center + unit * Projectile.localAI[1], Projectile.width * Projectile.scale, DelegateMethods.CutTiles);
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (projHitbox.Intersects(targetHitbox))
            {
                return true;
            }
            float num6 = 0f;
            if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, Projectile.Center + Projectile.velocity * Projectile.localAI[1], 22f * Projectile.scale * hitboxModifier, ref num6))
            {
                return true;
            }
            return false;
        }
    }
}
