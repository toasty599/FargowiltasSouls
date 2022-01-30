using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Masomode
{
    public class PlanteraCrystalLeafRing : MutantBoss.MutantCrystalLeaf
    {
        public override string Texture => "Terraria/Images/Projectile_226";

        public override void SetDefaults()
        {
            base.SetDefaults();
            projectile.scale = 1.5f;
            CooldownSlot = -1;
        }

        public override void AI()
        {
            if (++projectile.localAI[0] == 0)
            {
                for (int index1 = 0; index1 < 30; ++index1)
                {
                    int index2 = Dust.NewDust(projectile.position, projectile.width, projectile.height, Main.rand.NextBool() ? 107 : 157, 0f, 0f, 0, new Color(), 2f);
                    Main.dust[index2].noGravity = true;
                    Main.dust[index2].velocity *= 5f;
                }
            }

            Lighting.AddLight(projectile.Center, 0.1f, 0.4f, 0.2f);
            projectile.scale = (Main.mouseTextColor / 200f - 0.35f) * 0.2f + 0.95f;
            projectile.scale *= 1.5f;

            int byUUID = FargoSoulsUtil.GetByUUIDReal(projectile.owner, (int)projectile.ai[0], ModContent.ProjectileType<MutantBoss.MutantMark2>());
            if (byUUID != -1)
            {
                Vector2 offset = new Vector2(100, 0).RotatedBy(projectile.ai[1]);
                projectile.Center = Main.projectile[byUUID].Center + offset;

                projectile.localAI[1] = Math.Max(0, 150 - Main.projectile[byUUID].ai[1]) / 150; //rampup
                if (projectile.localAI[1] > 1f) //clamp it for use in draw
                    projectile.localAI[1] = 1f;
                projectile.ai[1] += 0.15f * projectile.localAI[1];
            }

            projectile.rotation = projectile.ai[1] + (float)Math.PI / 2f;
            
            if (projectile.localAI[0] > 20)
            {
                projectile.localAI[0] = 1;
                NPC plantera = FargoSoulsUtil.NPCExists(NPC.plantBoss, NPCID.Plantera);
                if (plantera != null && projectile.Distance(plantera.Center) < 1600f && Main.netMode != NetmodeID.MultiplayerClient)
                    Projectile.NewProjectile(projectile.Center, 4f * projectile.ai[1].ToRotationVector2(), ModContent.ProjectileType<CrystalLeafShot>(), projectile.damage, projectile.knockBack, projectile.owner);
            }
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.Poisoned, 300);
            target.AddBuff(mod.BuffType("Infested"), 180);
            target.AddBuff(mod.BuffType("IvyVenom"), 240);
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

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.ZoomMatrix);

            for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[projectile.type]; i++)
            {
                Color color27 = Color.White * projectile.Opacity * projectile.localAI[1];
                color27 *= (float)(ProjectileID.Sets.TrailCacheLength[projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[projectile.type];
                Vector2 value4 = projectile.oldPos[i];
                float num165 = projectile.oldRot[i];
                Main.EntitySpriteDraw(texture2D13, value4 + projectile.Size / 2f - Main.screenPosition + new Vector2(0, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color27, num165, origin2, projectile.scale, SpriteEffects.None, 0);
            }

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.ZoomMatrix);

            Main.EntitySpriteDraw(texture2D13, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), projectile.GetAlpha(lightColor), projectile.rotation, origin2, projectile.scale, SpriteEffects.None, 0);
            return false;
        }
    }
}