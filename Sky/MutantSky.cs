using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.ModLoader;
using FargowiltasSouls.NPCs;
using FargowiltasSouls.NPCs.MutantBoss;

namespace FargowiltasSouls.Sky
{
    public class MutantSky : CustomSky
    {
        private bool isActive = false;
        private float intensity = 0f;
        private float lifeIntensity = 0f;
        private float blackLerp = 0f;
        private int delay = 0;
        private int[] xPos = new int[50];
        private int[] yPos = new int[50];

        public override void Update(GameTime gameTime)
        {
            const float increment = 0.01f;
            if (FargoSoulsUtil.BossIsAlive(ref EModeGlobalNPC.mutantBoss, ModContent.NPCType<MutantBoss>())
                && (Main.npc[EModeGlobalNPC.mutantBoss].ai[0] < 0 || Main.npc[EModeGlobalNPC.mutantBoss].ai[0] >= 10))
            {
                lifeIntensity = 1f - (float)Main.npc[EModeGlobalNPC.mutantBoss].life / Main.npc[EModeGlobalNPC.mutantBoss].lifeMax;
                /*if (!FargoSoulsWorld.MasochistMode)
                {
                    lifeIntensity -= 0.5f;
                    if (lifeIntensity < 0)
                        lifeIntensity = 0;
                }*/

                if (Main.npc[EModeGlobalNPC.mutantBoss].ai[0] == 10) //smash to black
                {
                    intensity += increment * 3;

                    blackLerp += increment * 3;
                    if (blackLerp > 0.75f)
                        blackLerp = 0.75f;
                }
                else
                {
                    intensity += increment;

                    blackLerp -= increment;
                    if (blackLerp < 0)
                        blackLerp = 0;
                }

                if (intensity > 1f)
                    intensity = 1f;
            }
            else
            {
                lifeIntensity -= increment;
                if (lifeIntensity < 0f)
                    lifeIntensity = 0f;

                blackLerp -= increment;
                if (blackLerp < 0)
                    blackLerp = 0;

                intensity -= increment;
                if (intensity < 0f)
                {
                    intensity = 0f;
                    lifeIntensity = 0f;
                    blackLerp = 0f;
                    delay = 0;
                    Deactivate();
                }
            }
        }

        public override void Draw(SpriteBatch spriteBatch, float minDepth, float maxDepth)
        {
            if (maxDepth >= 0 && minDepth < 0)
            {
                Color color = Color.Lerp(new Color(180, 180, 180), Color.Black, blackLerp);

                spriteBatch.Draw(ModContent.GetTexture("FargowiltasSouls/Sky/MutantSky"),
                    new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), color * (intensity * 0.5f + System.Math.Max(lifeIntensity, blackLerp) * 0.5f));

                if (--delay < 0)
                {
                    delay = Main.rand.Next(5 + (int)(85f * (1f - lifeIntensity)));
                    for (int i = 0; i < 50; i++) //update positions
                    {
                        xPos[i] = Main.rand.Next(Main.screenWidth);
                        yPos[i] = Main.rand.Next(Main.screenHeight);
                    }
                }

                for (int i = 0; i < 50; i++) //static on screen
                {
                    int width = Main.rand.Next(3, 251);
                    spriteBatch.Draw(ModContent.GetTexture("FargowiltasSouls/Sky/MutantStatic"),
                    new Rectangle(xPos[i] - width / 2, yPos[i], width, 3),
                    color * lifeIntensity * 0.75f);
                }
            }
        }

        public override float GetCloudAlpha()
        {
            return 1f - intensity;
        }

        public override void Activate(Vector2 position, params object[] args)
        {
            isActive = true;
        }

        public override void Deactivate(params object[] args)
        {
            isActive = false;
        }

        public override void Reset()
        {
            isActive = false;
        }

        public override bool IsActive()
        {
            return isActive;
        }

        public override Color OnTileColor(Color inColor)
        {
            return new Color(Vector4.Lerp(new Vector4(0.6f, 0.9f, 1f, 1f), inColor.ToVector4(), 1f - intensity));
        }
    }
}