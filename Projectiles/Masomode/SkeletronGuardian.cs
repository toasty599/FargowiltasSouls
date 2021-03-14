using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Masomode
{
    public class SkeletronGuardian : ModProjectile
    {
        public override string Texture => "Terraria/Projectile_197";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Baby Guardian");
        }

        public override void SetDefaults()
        {
            projectile.width = 42;
            projectile.height = 42;
            projectile.penetrate = -1;
            projectile.hostile = true;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.aiStyle = -1;
            //cooldownSlot = 1;

            projectile.timeLeft = 360;
            projectile.hide = true;

            projectile.light = 0.5f;
        }

        public override void AI()
        {
            if (projectile.localAI[0] == 0)
            {
                projectile.localAI[0] = 1;
                projectile.rotation = Main.rand.NextFloat(0, 2 * (float)Math.PI);
                projectile.hide = false;

                Main.PlaySound(SoundID.Item21, projectile.Center);

                for (int i = 0; i < 50; i++)
                {
                    Vector2 pos = new Vector2(projectile.Center.X + Main.rand.Next(-20, 20), projectile.Center.Y + Main.rand.Next(-20, 20));
                    int dust = Dust.NewDust(pos, projectile.width, projectile.height, DustID.Blood, 0, 0, 100, default(Color), 2f);
                    Main.dust[dust].noGravity = true;
                }
            }

            if (projectile.ai[0] == 0)
            {
                projectile.velocity -= new Vector2(projectile.ai[1], 0).RotatedBy(projectile.velocity.ToRotation());

                if (projectile.velocity.Length() < 1)
                {
                    int p = Player.FindClosest(projectile.Center, 0, 0);
                    if (p != -1)
                    {
                        projectile.velocity = projectile.DirectionTo(Main.player[p].Center);
                        projectile.ai[0] = 1f;
                        projectile.ai[1] = p; //now used for tracking player
                        projectile.netUpdate = true;

                        Main.PlaySound(SoundID.Item1, projectile.Center);
                    }
                    else
                    {
                        projectile.Kill();
                    }
                }
            }
            else //weak homing
            {
                if (++projectile.localAI[0] < 45)
                    projectile.velocity *= 1.08f;

                if (projectile.localAI[0] < 65)
                {
                    float rotation = projectile.velocity.ToRotation();
                    Vector2 vel = Main.player[(int)projectile.ai[1]].Center - projectile.Center;
                    float targetAngle = vel.ToRotation();
                    projectile.velocity = new Vector2(projectile.velocity.Length(), 0f).RotatedBy(rotation.AngleLerp(targetAngle, 0.065f));
                }
            }

            projectile.direction = projectile.velocity.X < 0 ? -1 : 1;
            projectile.rotation += projectile.direction * .3f;
        }

        public override void Kill(int timeLeft)
        {
            for (int i = 0; i < 50; i++)
            {
                Vector2 pos = new Vector2(projectile.Center.X + Main.rand.Next(-20, 20), projectile.Center.Y + Main.rand.Next(-20, 20));
                int dust = Dust.NewDust(pos, projectile.width, projectile.height, DustID.Blood, 0, 0, 100, default(Color), 2f);
                Main.dust[dust].noGravity = true;
            }
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.AddBuff(mod.BuffType("Defenseless"), 300);
            target.AddBuff(mod.BuffType("Lethargic"), 300);
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