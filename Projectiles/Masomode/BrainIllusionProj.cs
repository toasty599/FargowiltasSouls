using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Localization;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Masomode
{
    public class BrainIllusionProj : ModProjectile
    {
        public override string Texture => "Terraria/NPC_266";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Brain of Cthulhu");
            DisplayName.AddTranslation(GameCulture.Chinese, "克苏鲁之脑");
            Main.projFrames[projectile.type] = Main.npcFrameCount[NPCID.BrainofCthulhu];
        }

        public override void SetDefaults()
        {
            projectile.width = 160;
            projectile.height = 110;
            projectile.aiStyle = -1;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.timeLeft = 240;
            projectile.penetrate = -1;

            projectile.scale += 0.25f;
        }

        public override void AI()
        {
            NPC npc = FargoSoulsUtil.NPCExists(projectile.ai[0], NPCID.BrainofCthulhu);
            if (npc == null)
            {
                projectile.Kill();
                return;
            }

            if (projectile.localAI[0] == 0)
            {
                projectile.localAI[0] = 1;
                Main.projectileTexture[projectile.type] = Main.npcTexture[NPCID.BrainofCthulhu];
            }

            if (++projectile.frameCounter > 6)
            {
                projectile.frameCounter = 0;
                projectile.frame++;
            }

            if (projectile.frame < 4 || projectile.frame > 7)
                projectile.frame = 4;

            projectile.alpha = (int)(255f * npc.life / npc.lifeMax);
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