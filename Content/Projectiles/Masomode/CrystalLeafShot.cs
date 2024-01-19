using FargowiltasSouls.Content.Buffs.Masomode;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.Masomode
{
    public class CrystalLeafShot : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_227";

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Crystal Leaf");
        }

        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 900;
            Projectile.aiStyle = 43;
            AIType = ProjectileID.CrystalLeafShot;
            Projectile.penetrate = -1;
        }

        public override void AI()
        {
            if (!Collision.SolidCollision(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height))
                Lighting.AddLight(Projectile.Center + Projectile.velocity, 0.1f, 0.4f, 0.2f);
            if (Projectile.timeLeft < 900 - 120)
                Projectile.tileCollide = true;

            const int netWindow = 10; // extra leniency time to let variables net sync
            const float redirectTime = 60f + netWindow;
            if (Projectile.ai[1] > 0) // redirect
            {
                if (Projectile.ai[1] > netWindow)
                {

                    Player player = Main.player[(int)Projectile.ai[2]];
                    if (player.Alive())
                    {
                        Vector2 LV = Projectile.velocity;
                        Vector2 PV = Projectile.DirectionTo(player.Center);
                        float anglediff = FargoSoulsUtil.RotationDifference(LV, PV);
                        //change rotation towards target
                        Projectile.velocity = Projectile.velocity.RotatedBy(Math.Sign(anglediff) * Math.Min(Math.Abs(anglediff),  MathHelper.Pi / redirectTime));
                        Projectile.rotation = Projectile.velocity.ToRotation();

                        /*
                        float angledif = FargoSoulsUtil.RotationDifference(Projectile.rotation.ToRotationVector2(), Projectile.DirectionTo(player.Center));
                        float amt = MathHelper.Min(Math.Abs(angledif), MathHelper.Pi / redirectTime);
                        Projectile.rotation += amt * Math.Sign(angledif);
                        Projectile.velocity = Projectile.rotation.ToRotationVector2() * Projectile.velocity
                        */
                    }
                    Projectile.position -= Projectile.velocity * (Projectile.ai[1] / redirectTime);
                }
                Projectile.ai[1]++;
                if (Projectile.ai[1] > redirectTime)
                {
                    Projectile.ai[1] = 0;
                    Projectile.netUpdate = true;
                }
            }
        }

        public override void ModifyHitPlayer(Player target, ref Player.HurtModifiers modifiers)
        {
            if (target.hurtCooldowns[1] == 0)
            {
                target.AddBuff(ModContent.BuffType<IvyVenomBuff>(), 240);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            int num156 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;

            SpriteEffects spriteEffects = Projectile.spriteDirection < 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Color color26 = lightColor;
            color26 = Projectile.GetAlpha(color26);

            float scale = Projectile.scale * 1.5f;

            Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color26, Projectile.rotation, origin2, scale, spriteEffects, 0);

            color26.A = 150;
            Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color26, Projectile.rotation, origin2, scale, spriteEffects, 0);
            return false;
        }
    }
}