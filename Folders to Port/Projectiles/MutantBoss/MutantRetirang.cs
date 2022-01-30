using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.MutantBoss
{
    public class MutantRetirang : ModProjectile
    {
        public override string Texture => "FargowiltasSouls/Projectiles/BossWeapons/Retirang";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Retirang");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 12;
            ProjectileID.Sets.TrailingMode[projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            projectile.width = 50;
            projectile.height = 50;
            projectile.scale = 1.5f;
            projectile.penetrate = -1;
            projectile.hostile = true;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.aiStyle = -1;
            CooldownSlot = 1;
        }

        public override void AI()
        {
            if (++projectile.localAI[0] > projectile.ai[1])
                projectile.Kill();
            if (projectile.localAI[1] == 0)
                projectile.localAI[1] = projectile.velocity.Length();

            Vector2 acceleration = Vector2.Normalize(projectile.velocity).RotatedBy(Math.PI / 2) * projectile.ai[0];
            projectile.velocity = Vector2.Normalize(projectile.velocity) * projectile.localAI[1] + acceleration;

            projectile.rotation += 1f * Math.Sign(projectile.ai[0]);
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.Ichor, 120);
            if (FargoSoulsWorld.EternityMode)
                target.AddBuff(mod.BuffType("MutantFang"), 180);
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

            int add = 150; Color glowColor = new Color(add + Main.DiscoR / 3, add + Main.DiscoG / 3, add + Main.DiscoB / 3, 0);
            for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[projectile.type]; i += 2)
            {
                Color color27 = glowColor * 0.9f;
                color27 *= (float)(ProjectileID.Sets.TrailCacheLength[projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[projectile.type];
                Vector2 value4 = projectile.oldPos[i];
                float num165 = projectile.oldRot[i];
                Main.EntitySpriteDraw(texture2D13, value4 + projectile.Size / 2f - Main.screenPosition + new Vector2(0, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color27, num165, origin2, projectile.scale, SpriteEffects.None, 0);
            }

            Main.EntitySpriteDraw(texture2D13, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color26, projectile.rotation, origin2, projectile.scale, SpriteEffects.None, 0);
            return false;
        }
    }
}