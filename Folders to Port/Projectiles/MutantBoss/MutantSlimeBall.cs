using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.MutantBoss
{
    public class MutantSlimeBall : ModProjectile
    {
        public override string Texture => "FargowiltasSouls/Projectiles/BossWeapons/SlimeBall";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Slime Rain");
            Main.projFrames[projectile.type] = 4;
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 5;
            ProjectileID.Sets.TrailingMode[projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            projectile.width = 14;
            projectile.height = 14;
            projectile.aiStyle = -1;
            //projectile.aiStyle = 14;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.hostile = true;
            CooldownSlot = 1;
            projectile.scale = 1.5f;
            projectile.alpha = 50;

            projectile.extraUpdates = 1;
            projectile.timeLeft = 90 * (projectile.extraUpdates + 1);
        }
        
        public override void AI()
        {
            projectile.rotation = projectile.velocity.ToRotation() - (float)Math.PI / 2;
            if (projectile.localAI[0] == 0) //choose a texture to use
            {
                projectile.localAI[0] += Main.rand.Next(1, 4);
            }

            if (projectile.timeLeft % projectile.MaxUpdates == 0)
            {
                //if (projectile.timeLeft < 45 * projectile.MaxUpdates) projectile.velocity *= 1.015f;

                if (++projectile.frameCounter >= 6)
                {
                    projectile.frameCounter = 0;
                    if (++projectile.frame >= Main.projFrames[projectile.type])
                        projectile.frame = 0;
                }
            }
        }

        public override void Kill(int timeleft)
        {
            for (int i = 0; i < 20; i++)
            {
                int num469 = Dust.NewDust(new Vector2(projectile.Center.X, projectile.Center.Y), projectile.width, projectile.height, 59, -projectile.velocity.X * 0.2f,
                    -projectile.velocity.Y * 0.2f, 100, default(Color), 2f);
                Main.dust[num469].noGravity = true;
                Main.dust[num469].velocity *= 2f;
                num469 = Dust.NewDust(new Vector2(projectile.Center.X, projectile.Center.Y), projectile.width, projectile.height, 59, -projectile.velocity.X * 0.2f,
                    -projectile.velocity.Y * 0.2f, 100);
                Main.dust[num469].velocity *= 2f;
            }
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.Slimed, 240);
            if (FargoSoulsWorld.EternityMode)
                target.AddBuff(ModContent.BuffType<MutantFang>(), 180);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture2D13 = FargowiltasSouls.Instance.Assets.Request<Texture2D>("Projectiles/MutantBoss/MutantSlimeBall_" + projectile.localAI[0].ToString(), ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            int num156 = texture2D13.Height / Main.projFrames[projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new Rectangle(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;

            Color color26 = lightColor;
            color26 = projectile.GetAlpha(color26);

            for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[projectile.type]; i++)
            {
                Color color27 = color26 * 0.5f;
                color27 *= (float)(ProjectileID.Sets.TrailCacheLength[projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[projectile.type];
                Vector2 value4 = projectile.oldPos[i];
                float num165 = projectile.oldRot[i];
                Main.EntitySpriteDraw(texture2D13, value4 + projectile.Size / 2f - Main.screenPosition + new Vector2(0, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color27, num165, origin2, projectile.scale, SpriteEffects.None, 0);
            }

            Main.EntitySpriteDraw(texture2D13, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), projectile.GetAlpha(lightColor), projectile.rotation, origin2, projectile.scale, SpriteEffects.None, 0);
            return false;
        }
    }
}