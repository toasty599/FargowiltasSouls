using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.MutantBoss
{
    public class MutantSpearAim : ModProjectile
    {
        public override string Texture => "FargowiltasSouls/Projectiles/BossWeapons/HentaiSpear";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("The Penetrator");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            projectile.width = 30;
            projectile.height = 30;
            projectile.aiStyle = -1;
            projectile.hostile = true;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.scale = 1.3f;
            projectile.alpha = 0;
            projectile.timeLeft = 60;
            cooldownSlot = 1;
            projectile.GetGlobalProjectile<FargoGlobalProjectile>().TimeFreezeImmune = true;
            projectile.GetGlobalProjectile<FargoGlobalProjectile>().ImmuneToMutantBomb = true;
        }

        /* -1: direct, green, 3sec for rapid p2 toss
         * 0: direct, green, 1sec (?)
         * 1: direct, green, 0.5sec for rapid p2 toss
         * 2: predictice, blue, 1sec for p2 destroyer throw
         * 3: predictive, blue, 1.5sec for p1
         * 4: predictive, blue, 2sec with no more tracking after a bit for p2 slow dash finisher (maybe red?)
         * 5: predictive, NONE, 1sec for p2 slow dash
         */

        public override void AI()
        {
            //basically: ai1 > 1 means predictive and blue glow, otherwise direct aim and green glow

            NPC mutant = Main.npc[(int)projectile.ai[0]];
            if (mutant.active && mutant.type == mod.NPCType("MutantBoss"))
            {
                projectile.Center = mutant.Center;
                if (projectile.ai[1] > 1)
                {
                    if (!(projectile.ai[1] == 4 && projectile.timeLeft < System.Math.Abs(projectile.localAI[1]) + 5))
                        projectile.rotation = mutant.DirectionTo(Main.player[mutant.target].Center + Main.player[mutant.target].velocity * 30).ToRotation() + MathHelper.ToRadians(135f);
                }
                else
                {
                    projectile.rotation = mutant.DirectionTo(Main.player[mutant.target].Center).ToRotation() + MathHelper.ToRadians(135f);
                }
            }
            else
            {
                projectile.Kill();
            }

            if (projectile.localAI[0] == 0) //modifying timeleft for mp sync, localAI1 changed to adjust the rampup on the glow tell
            {
                projectile.localAI[0] = 1;

                if (projectile.ai[1] == -1) //extra long startup on p2 direct throw
                {
                    projectile.timeLeft += 120;
                    projectile.localAI[1] = -120;
                }
                else if (projectile.ai[1] == 1) //p2 direct throw rapid fire
                {
                    projectile.timeLeft -= 30;
                    projectile.localAI[1] = 30;
                }
                else if (projectile.ai[1] == 3) //p1 predictive throw
                {
                    projectile.timeLeft += 30;
                    projectile.localAI[1] = -30;
                }
                else if (projectile.ai[1] == 4)
                {
                    projectile.timeLeft += 40;
                    projectile.localAI[1] = -40;
                }
            }
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            Projectile.NewProjectile(target.Center + Main.rand.NextVector2Circular(100, 100), Vector2.Zero, mod.ProjectileType("PhantasmalBlast"), 0, 0f, projectile.owner);
            if (FargoSoulsWorld.MasochistMode)
            {
                target.GetModPlayer<FargoPlayer>().MaxLifeReduction += 100;
                target.AddBuff(mod.BuffType("OceanicMaul"), 5400);
                target.AddBuff(mod.BuffType("MutantFang"), 180);
            }
            target.AddBuff(mod.BuffType("CurseoftheMoon"), 600);
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

            for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[projectile.type]; i++)
            {
                Color color27 = color26;
                color27 *= (float)(ProjectileID.Sets.TrailCacheLength[projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[projectile.type];
                Vector2 value4 = projectile.oldPos[i];
                float num165 = projectile.oldRot[i];
                Main.spriteBatch.Draw(texture2D13, value4 + projectile.Size / 2f - Main.screenPosition + new Vector2(0, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color27, num165, origin2, projectile.scale, SpriteEffects.None, 0f);
            }

            Main.spriteBatch.Draw(texture2D13, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), projectile.GetAlpha(lightColor), projectile.rotation, origin2, projectile.scale, SpriteEffects.None, 0f);

            if (projectile.ai[1] != 5)
            {
                Texture2D glow = mod.GetTexture("Projectiles/MutantBoss/MutantSpearAimGlow");
                float modifier = projectile.timeLeft / (60f - projectile.localAI[1]);
                Color glowColor = new Color(51, 255, 191, 210);
                if (projectile.ai[1] > 1)
                    glowColor = new Color(0, 0, 255, 210);
                //if (projectile.ai[1] == 4) glowColor = new Color(255, 0, 0, 210);
                glowColor *= 1f - modifier;
                float glowScale = projectile.scale * 6f * modifier;
                Main.spriteBatch.Draw(glow, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), glowColor, 0, origin2, glowScale, SpriteEffects.None, 0f);
            }
            return false;
        }
    }
}