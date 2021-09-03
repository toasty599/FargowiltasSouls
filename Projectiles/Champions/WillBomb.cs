using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using FargowiltasSouls.Buffs.Masomode;
using FargowiltasSouls.NPCs;

namespace FargowiltasSouls.Projectiles.Champions
{
    public class WillBomb : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Will Bomb");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            projectile.width = 50;
            projectile.height = 50;
            projectile.aiStyle = -1;
            projectile.hostile = true;
            projectile.timeLeft = 40;
            projectile.tileCollide = false;
            projectile.penetrate = -1;

            cooldownSlot = 1;

            projectile.GetGlobalProjectile<FargoGlobalProjectile>().ImmuneToGuttedHeart = true;
        }

        public override bool CanDamage()
        {
            return false;
        }

        public override void AI()
        {
            Vector2 reduction = Vector2.Normalize(projectile.velocity) * projectile.ai[0];
            projectile.velocity -= reduction;

            projectile.rotation += projectile.velocity.Length() * 0.03f * Math.Sign(projectile.velocity.X);
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            if (FargoSoulsWorld.MasochistMode)
            {
                target.AddBuff(ModContent.BuffType<Defenseless>(), 300);
                target.AddBuff(ModContent.BuffType<Midas>(), 300);
            }
            target.AddBuff(BuffID.Bleeding, 300);
        }

        private void SpawnSphereRing(int max, float speed, int damage, float rotationModifier)
        {
            if (Main.netMode == NetmodeID.MultiplayerClient) return;
            float rotation = 2f * (float)Math.PI / max;
            Vector2 vel = Vector2.UnitY * speed;
            int type = ModContent.ProjectileType<WillTyphoon>();
            for (int i = 0; i < max; i++)
            {
                vel = vel.RotatedBy(rotation);
                Projectile.NewProjectile(projectile.Center, vel, type, damage, 0f, Main.myPlayer, rotationModifier, speed);
            }
            Main.PlaySound(SoundID.Item84, projectile.Center);
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.Item92, projectile.Center);

            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                if (FargoSoulsWorld.MasochistMode)
                    Projectile.NewProjectile(projectile.Center, Vector2.Zero, ModContent.ProjectileType<WillRitual>(), projectile.damage, 0f, Main.myPlayer, 0f, projectile.ai[1]);

                if (FargoSoulsUtil.BossIsAlive(ref EModeGlobalNPC.championBoss, ModContent.NPCType<NPCs.Champions.WillChampion>())
                    && Main.npc[EModeGlobalNPC.championBoss].ai[0] > -1)
                {
                    if (Main.npc[EModeGlobalNPC.championBoss].localAI[2] == 1)
                    {
                        SpawnSphereRing(8, 8f, projectile.damage, 2f);
                        SpawnSphereRing(8, 8f, projectile.damage, -2f);
                    }

                    if (Main.npc[EModeGlobalNPC.championBoss].localAI[3] == 1)
                    {
                        SpawnSphereRing(8, 8f, projectile.damage, 0.5f);
                        SpawnSphereRing(8, 8f, projectile.damage, -0.5f);
                    }
                }

                for (int i = 0; i < 4; i++)
                {
                    Projectile.NewProjectile(projectile.Center, Vector2.UnitX.RotatedBy(Math.PI / 4 * 2 * i + Math.PI / 4),
                        ModContent.ProjectileType<WillDeathray>(), projectile.damage, 0f, Main.myPlayer, 0f, projectile.ai[1]);
                }
            }

            projectile.position = projectile.Center;
            projectile.width = 250;
            projectile.height = 250;
            projectile.Center = projectile.position;

            Main.PlaySound(SoundID.Item, (int)projectile.Center.X, (int)projectile.Center.Y, 14);

            for (int num615 = 0; num615 < 45; num615++)
            {
                int num616 = Dust.NewDust(projectile.position, projectile.width, projectile.height, 31, 0f, 0f, 100, default(Color), 1.5f);
                Main.dust[num616].velocity *= 1.4f;
            }

            for (int num617 = 0; num617 < 30; num617++)
            {
                int num618 = Dust.NewDust(projectile.position, projectile.width, projectile.height, 6, 0f, 0f, 100, default(Color), 3.5f);
                Main.dust[num618].noGravity = true;
                Main.dust[num618].velocity *= 7f;
                num618 = Dust.NewDust(projectile.position, projectile.width, projectile.height, 6, 0f, 0f, 100, default(Color), 1.5f);
                Main.dust[num618].velocity *= 3f;
            }

            for (int num619 = 0; num619 < 3; num619++)
            {
                float scaleFactor9 = 0.4f;
                if (num619 == 1) scaleFactor9 = 0.8f;
                int num620 = Gore.NewGore(projectile.Center, default(Vector2), Main.rand.Next(61, 64));
                Main.gore[num620].velocity *= scaleFactor9;
                Gore gore97 = Main.gore[num620];
                gore97.velocity.X = gore97.velocity.X + 1f;
                Gore gore98 = Main.gore[num620];
                gore98.velocity.Y = gore98.velocity.Y + 1f;
                num620 = Gore.NewGore(projectile.Center, default(Vector2), Main.rand.Next(61, 64));
                Main.gore[num620].velocity *= scaleFactor9;
                Gore gore99 = Main.gore[num620];
                gore99.velocity.X = gore99.velocity.X - 1f;
                Gore gore100 = Main.gore[num620];
                gore100.velocity.Y = gore100.velocity.Y + 1f;
                num620 = Gore.NewGore(projectile.Center, default(Vector2), Main.rand.Next(61, 64));
                Main.gore[num620].velocity *= scaleFactor9;
                Gore gore101 = Main.gore[num620];
                gore101.velocity.X = gore101.velocity.X + 1f;
                Gore gore102 = Main.gore[num620];
                gore102.velocity.Y = gore102.velocity.Y - 1f;
                num620 = Gore.NewGore(projectile.Center, default(Vector2), Main.rand.Next(61, 64));
                Main.gore[num620].velocity *= scaleFactor9;
                Gore gore103 = Main.gore[num620];
                gore103.velocity.X = gore103.velocity.X - 1f;
                Gore gore104 = Main.gore[num620];
                gore104.velocity.Y = gore104.velocity.Y - 1f;
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

            SpriteEffects effects = projectile.spriteDirection < 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[projectile.type]; i++)
            {
                Color color27 = Color.White * projectile.Opacity * 0.75f * 0.5f;
                color27 *= (float)(ProjectileID.Sets.TrailCacheLength[projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[projectile.type];
                Vector2 value4 = projectile.oldPos[i];
                float num165 = projectile.oldRot[i];
                Main.spriteBatch.Draw(texture2D13, value4 + projectile.Size / 2f - Main.screenPosition + new Vector2(0, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color27, num165, origin2, projectile.scale, effects, 0f);
            }

            Main.spriteBatch.Draw(texture2D13, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), projectile.GetAlpha(lightColor), projectile.rotation, origin2, projectile.scale, effects, 0f);
            return false;
        }
    }
}