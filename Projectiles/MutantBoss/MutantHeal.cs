using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.MutantBoss
{
    public class MutantHeal : ModProjectile
    {
        public override string Texture => "FargowiltasSouls/Projectiles/Champions/SpiritSpirit";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Mutant Heal");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 20;
            ProjectileID.Sets.TrailingMode[projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            projectile.width = 12;
            projectile.height = 12;
            projectile.aiStyle = -1;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.timeLeft = 360;
            //projectile.hostile = true;
            projectile.scale = 0.8f;
        }

        public override bool CanDamage()
        {
            return false;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(projectile.localAI[0]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            projectile.localAI[0] = reader.ReadSingle();
        }

        public override void AI()
        {
            if (projectile.ai[0] == 0)
            {
                if (projectile.localAI[0] == 0)
                {
                    projectile.localAI[0] = Main.rand.NextFloat(MathHelper.ToRadians(1f)) * (Main.rand.NextBool() ? 1 : -1);
                    projectile.netUpdate = true;
                }

                projectile.velocity = Vector2.Normalize(projectile.velocity).RotatedBy(projectile.localAI[0]) * (projectile.velocity.Length() - projectile.ai[1]);

                if (projectile.velocity.Length() < 1f)
                {
                    projectile.ai[0] = 1;
                    //projectile.velocity = projectile.velocity.RotatedByRandom(MathHelper.TwoPi);
                }
            }
            else if (projectile.ai[0] == 1)
            {
                for (int i = 0; i < 2; i++) //make up for real spectre bolt having 3 extraUpdates
                {
                    projectile.position += projectile.velocity;

                    Vector2 change = Vector2.Normalize(projectile.velocity) * 5f;
                    projectile.velocity = (projectile.velocity * 29f + change).RotatedBy(projectile.localAI[0] * 3) / 30f;
                }

                if (projectile.velocity.Length() > 4.5f)
                {
                    projectile.ai[0] = 2;
                }
            }
            else
            {
                projectile.extraUpdates = 1;

                Player player = Main.player[projectile.owner];

                if (projectile.Distance(player.Center) < 5f) //die and feed player
                {
                    if (player.whoAmI == Main.myPlayer)
                    {
                        player.ClearBuff(mod.BuffType("MutantFang"));
                        player.statLife += projectile.damage;
                        player.HealEffect(projectile.damage);
                        if (player.statLife > player.statLifeMax2)
                            player.statLife = player.statLifeMax2;
                        projectile.Kill();
                    }
                }
                else
                {
                    for (int i = 0; i < 2; i++) //make up for real spectre bolt having 3 extraUpdates
                    {
                        Vector2 change = projectile.DirectionTo(player.Center) * 5f;
                        projectile.velocity = (projectile.velocity * 29f + change) / 30f;
                    }
                }

                for (int i = 0; i < 3; i++) //make up for real spectre bolt having 3 extraUpdates
                {
                    projectile.position += projectile.velocity;
                }
            }
        }

        public override void Kill(int timeLeft)
        {
            for (int i = 0; i < 5; i++)
            {
                int d = Dust.NewDust(projectile.position, projectile.width, projectile.height, 229, 0f, 0f, 0, default(Color), 1.5f);
                Main.dust[d].noGravity = true;
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(51, 255, 191, 210) * projectile.Opacity * 0.8f;
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

            for (float i = 0; i < ProjectileID.Sets.TrailCacheLength[projectile.type]; i += 0.2f)
            {
                Player player = Main.player[projectile.owner];
                Texture2D glow = texture2D13; //mod.GetTexture("Projectiles/BossWeapons/HentaiSpearSpinGlow");
                Color color27 = color26; //Color.Lerp(new Color(255, 255, 0, 210), Color.Transparent, 0.4f);
                color27 *= (float)(ProjectileID.Sets.TrailCacheLength[projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[projectile.type];
                float scale = projectile.scale;
                scale *= (float)(ProjectileID.Sets.TrailCacheLength[projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[projectile.type];
                int max0 = (int)i - 1;//Math.Max((int)i - 1, 0);
                if (max0 < 0)
                    continue;
                Vector2 center = Vector2.Lerp(projectile.oldPos[(int)i], projectile.oldPos[max0], 1 - i % 1);
                float smoothtrail = i % 1 * MathHelper.Pi / 6.85f;

                center += projectile.Size / 2;

                Main.spriteBatch.Draw(
                    glow,
                    center - Main.screenPosition + new Vector2(0, projectile.gfxOffY),
                    null,
                    color27,
                    projectile.rotation,
                    glow.Size() / 2,
                    scale,
                    SpriteEffects.None,
                    0f);
            }
            return false;
        }
    }
}