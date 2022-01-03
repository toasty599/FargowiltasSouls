using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles
{
    public class StyxArmorScythe2 : StyxArmorScythe
    {
        public override string Texture => "FargowiltasSouls/Projectiles/StyxArmorScythe";

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            ProjectileID.Sets.Homing[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            
            projectile.penetrate = 1;

            projectile.usesLocalNPCImmunity = false;
            projectile.localNPCHitCooldown = 0;

            projectile.GetGlobalProjectile<FargoGlobalProjectile>().CanSplit = true;
            projectile.GetGlobalProjectile<FargoGlobalProjectile>().TimeFreezeImmune = false;
        }

        public override void AI()
        {
            const int baseDamage = 100;

            if (projectile.velocity == Vector2.Zero || projectile.velocity.HasNaNs())
                projectile.velocity = -Vector2.UnitY;

            Player player = Main.player[projectile.owner];
            projectile.damage = (int)(baseDamage * player.ownedProjectileCounts[projectile.type] * player.magicDamage);
            if (++projectile.ai[0] > 10)
            {
                projectile.ai[0] = 0;
                projectile.ai[1] = FargoSoulsUtil.FindClosestHostileNPC(projectile.Center, 2000);
                projectile.netUpdate = true;
            }

            if (projectile.ai[0] >= 0)
            {
                if (projectile.velocity.Length() < 24)
                    projectile.velocity *= 1.06f;
            }

            NPC npc = FargoSoulsUtil.NPCExists(projectile.ai[1]);
            if (npc != null)
            {
                double num4 = (npc.Center - projectile.Center).ToRotation() - projectile.velocity.ToRotation();
                if (num4 > Math.PI)
                    num4 -= 2.0 * Math.PI;
                if (num4 < -1.0 * Math.PI)
                    num4 += 2.0 * Math.PI;
                projectile.velocity = projectile.velocity.RotatedBy(num4 * 0.2f);
            }
            else
            {
                projectile.ai[1] = -1f;
                projectile.netUpdate = true;
            }

            projectile.direction = projectile.spriteDirection = projectile.velocity.X < 0 ? -1 : 1;
            projectile.rotation += projectile.spriteDirection * 1f;
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.NPCDeath52, projectile.Center);

            for (int i = 0; i < 40; i++)
            {
                int d = Dust.NewDust(projectile.position, projectile.width, projectile.height, 70, Scale: 3f);
                Main.dust[d].velocity *= 12f;
                Main.dust[d].noGravity = true;
            }

            projectile.timeLeft = 0;
            projectile.penetrate = -1;
            projectile.position = projectile.Center;
            projectile.width = projectile.height = 200;
            projectile.Center = projectile.position;

            if (timeLeft > 0)
            {
                projectile.Damage();
            }

            Main.PlaySound(SoundID.Item, projectile.Center, 14);

            for (int i = 0; i < 20; i++)
            {
                int dust = Dust.NewDust(projectile.position, projectile.width, projectile.height, 31, 0f, 0f, 100, default(Color), 3f);
                Main.dust[dust].velocity *= 1.4f;
            }

            for (int i = 0; i < 10; i++)
            {
                int dust = Dust.NewDust(projectile.position, projectile.width, projectile.height, 6, 0f, 0f, 100, default(Color), 3.5f);
                Main.dust[dust].noGravity = true;
                Main.dust[dust].velocity *= 7f;
                dust = Dust.NewDust(projectile.position, projectile.width, projectile.height, 6, 0f, 0f, 100, default(Color), 1.5f);
                Main.dust[dust].velocity *= 3f;
            }

            float scaleFactor9 = 0.5f;
            for (int j = 0; j < 4; j++)
            {
                int gore = Gore.NewGore(new Vector2(projectile.Center.X, projectile.Center.Y), default(Vector2), Main.rand.Next(61, 64));
                Main.gore[gore].velocity *= scaleFactor9;
                Main.gore[gore].velocity.X += 1f;
                Main.gore[gore].velocity.Y += 1f;
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.ShadowFlame, 300);
            target.AddBuff(ModContent.BuffType<Buffs.Masomode.MutantNibble>(), 300);
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

            SpriteEffects spriteEffects = projectile.spriteDirection > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[projectile.type]; i++)
            {
                Color color27 = color26;
                color27 *= (float)(ProjectileID.Sets.TrailCacheLength[projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[projectile.type];
                Vector2 value4 = projectile.oldPos[i];
                float num165 = projectile.oldRot[i];
                Main.spriteBatch.Draw(texture2D13, value4 + projectile.Size / 2f - Main.screenPosition + new Vector2(0, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color27, num165, origin2, projectile.scale, spriteEffects, 0f);
            }

            Main.spriteBatch.Draw(texture2D13, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), projectile.GetAlpha(lightColor), projectile.rotation, origin2, projectile.scale, spriteEffects, 0f);
            return false;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, 255, 0, 0) * projectile.Opacity; //yellow
        }
    }
}