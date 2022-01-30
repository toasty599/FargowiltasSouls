using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.MutantBoss
{
    public class MutantScythe1 : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Mutant Sickle");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 3;
            ProjectileID.Sets.TrailingMode[projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            projectile.width = 30;
            projectile.height = 30;
            projectile.alpha = 0;
            projectile.hostile = true;
            projectile.timeLeft = 600;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.aiStyle = -1;
            CooldownSlot = 1;
            
            projectile.hide = true;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(projectile.timeLeft);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            projectile.timeLeft = reader.ReadInt32();
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            behindProjectiles.Add(index);
        }

        public override void AI()
        {
            /*if (projectile.localAI[0] == 0)
            {
                projectile.localAI[0] = 1;
                SoundEngine.PlaySound(SoundID.Item8, projectile.Center);
            }*/
            if (projectile.rotation == 0)
                projectile.rotation = Main.rand.NextFloat(MathHelper.TwoPi);

            float modifier = (180f - projectile.timeLeft + 90) / 180f; //2f - projectile.timeLeft / 240f;
            if (modifier < 0)
                modifier = 0;
            if (modifier > 1)
                modifier = 1;
            projectile.rotation += 0.1f + 0.7f * modifier;
            //projectile.alpha = (int)(127f * (1f - modifier));

            if (projectile.timeLeft < 180) //240)
            {
                if (projectile.velocity == Vector2.Zero)
                {
                    projectile.velocity = projectile.ai[1].ToRotationVector2();
                    projectile.netUpdate = true;
                }
                projectile.velocity *= 1f + projectile.ai[0];
            }
            /*for (int i = 0; i < 6; i++)
            {
                Vector2 offset = new Vector2(0, -20).RotatedBy(projectile.rotation);
                offset = offset.RotatedByRandom(MathHelper.Pi / 6);
                int d = Dust.NewDust(projectile.Center, 0, 0, 229, 0f, 0f, 150);
                Main.dust[d].position += offset;
                float velrando = Main.rand.Next(20, 31) / 10;
                Main.dust[d].velocity = projectile.velocity / velrando;
                Main.dust[d].noGravity = true;
            }*/
        }

        public override void PostAI()
        {
            /*if (projectile.GetGlobalProjectile<FargoSoulsGlobalProjectile>().GrazeCD == 6)
                projectile.GetGlobalProjectile<FargoSoulsGlobalProjectile>().GrazeCD = 60;
            else if (projectile.GetGlobalProjectile<FargoSoulsGlobalProjectile>().GrazeCD == 7)
                projectile.GetGlobalProjectile<FargoSoulsGlobalProjectile>().GrazeCD = 5;*/
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White * projectile.Opacity;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            int num156 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new Rectangle(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;

            Color color26 = lightColor;
            color26 = projectile.GetAlpha(color26);

            for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[projectile.type]; i++)
            {
                Color color27 = color26;
                color27 *= (float)(ProjectileID.Sets.TrailCacheLength[projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[projectile.type];
                Vector2 value4 = projectile.oldPos[i];
                float num165 = projectile.oldRot[i];
                Main.EntitySpriteDraw(texture2D13, value4 + projectile.Size / 2f - Main.screenPosition + new Vector2(0, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color27, num165, origin2, projectile.scale, SpriteEffects.None, 0);
            }

            Main.EntitySpriteDraw(texture2D13, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), projectile.GetAlpha(lightColor), projectile.rotation, origin2, projectile.scale, SpriteEffects.None, 0);
            return false;
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            if (FargoSoulsWorld.EternityMode)
                target.AddBuff(mod.BuffType("MutantFang"), 180);
            target.AddBuff(BuffID.Bleeding, 600);
        }
    }
}