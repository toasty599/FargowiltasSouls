using FargowiltasSouls.Buffs.Masomode;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Deathrays
{
    public class DeviBigDeathray : BaseDeathray
    {
        public override string Texture => "FargowiltasSouls/Projectiles/Deathrays/DeviDeathray";

        public PrimDrawer LaserDrawer { get; private set; } = null;

        public PrimDrawer RingDrawer { get; private set; } = null;

        public static List<string> RingTexturePaths => new()
        {
            "Ring1",
            "Ring2",
            "Ring3",
            "Ring4"
        };

        public DeviBigDeathray() : base(4000) { }

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            DisplayName.SetDefault("Love Ray");
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.GetGlobalProjectile<FargoSoulsGlobalProjectile>().DeletionImmuneRank = 2;
        }

        public override void AI()
        {
            if (!Main.dedServ && Main.LocalPlayer.active)
                Main.LocalPlayer.GetModPlayer<FargoSoulsPlayer>().Screenshake = 2;

            Vector2? vector78 = null;
            if (Projectile.velocity.HasNaNs() || Projectile.velocity == Vector2.Zero)
            {
                Projectile.velocity = -Vector2.UnitY;
            }
            NPC npc = FargoSoulsUtil.NPCExists(Projectile.ai[1], ModContent.NPCType<NPCs.DeviBoss.DeviBoss>());
            if (npc != null)
            {
                Projectile.Center = npc.Center + Projectile.velocity * 300 + Main.rand.NextVector2Circular(20, 20);
            }
            else
            {
                Projectile.Kill();
                return;
            }
            if (Projectile.velocity.HasNaNs() || Projectile.velocity == Vector2.Zero)
            {
                Projectile.velocity = -Vector2.UnitY;
            }
            if (Projectile.localAI[0] == 0f)
            {
                SoundEngine.PlaySound(SoundID.Zombie104, Projectile.Center);
            }
            float num801 = 17f;
            Projectile.localAI[0] += 1f;
            if (Projectile.localAI[0] >= maxTime)
            {
                Projectile.Kill();
                return;
            }
            Projectile.scale = (float)Math.Sin(Projectile.localAI[0] * 3.14159274f / maxTime) * 5f * num801;
            if (Projectile.scale > num801)
                Projectile.scale = num801;
            float num804 = Projectile.velocity.ToRotation();
            num804 += Projectile.ai[0];
            Projectile.rotation = num804 - 1.57079637f;
            Projectile.velocity = num804.ToRotationVector2();
            float num805 = 3f;
            float num806 = (float)Projectile.width;
            Vector2 samplingPoint = Projectile.Center;
            if (vector78.HasValue)
            {
                samplingPoint = vector78.Value;
            }
            float[] array3 = new float[(int)num805];
            //Collision.LaserScan(samplingPoint, Projectile.velocity, num806 * Projectile.scale, 3000f, array3);
            for (int i = 0; i < array3.Length; i++)
                array3[i] = 3000f;
            float num807 = 0f;
            int num3;
            for (int num808 = 0; num808 < array3.Length; num808 = num3 + 1)
            {
                num807 += array3[num808];
                num3 = num808;
            }
            num807 /= num805;
            float amount = 0.5f;
            Projectile.localAI[1] = MathHelper.Lerp(Projectile.localAI[1], num807, amount);
            Vector2 vector79 = Projectile.Center + Projectile.velocity * (Projectile.localAI[1] - 14f);
            for (int num809 = 0; num809 < 2; num809 = num3 + 1)
            {
                float num810 = Projectile.velocity.ToRotation() + ((Main.rand.Next(2) == 1) ? -1f : 1f) * 1.57079637f;
                float num811 = (float)Main.rand.NextDouble() * 2f + 2f;
                Vector2 vector80 = new Vector2((float)Math.Cos((double)num810) * num811, (float)Math.Sin((double)num810) * num811);
                int num812 = Dust.NewDust(vector79, 0, 0, 244, vector80.X, vector80.Y, 0, default(Color), 1f);
                Main.dust[num812].noGravity = true;
                Main.dust[num812].scale = 1.7f;
                num3 = num809;
            }
            if (Main.rand.NextBool(5))
            {
                Vector2 value29 = Projectile.velocity.RotatedBy(1.5707963705062866, default(Vector2)) * ((float)Main.rand.NextDouble() - 0.5f) * (float)Projectile.width;
                int num813 = Dust.NewDust(vector79 + value29 - Vector2.One * 4f, 8, 8, 244, 0f, 0f, 100, default(Color), 1.5f);
                Dust dust = Main.dust[num813];
                dust.velocity *= 0.5f;
                Main.dust[num813].velocity.Y = -Math.Abs(Main.dust[num813].velocity.Y);
            }
            //DelegateMethods.v3_1 = new Vector3(0.3f, 0.65f, 0.7f);
            //Utils.PlotTileLine(Projectile.Center, Projectile.Center + Projectile.velocity * Projectile.localAI[1], (float)Projectile.width * Projectile.scale, new Utils.PerLinePoint(DelegateMethods.CastLight));

            const int increment = 100;
            for (int i = 0; i < array3[0]; i += increment)
            {
                if (Main.rand.Next(3) != 0)
                    continue;
                float offset = i + Main.rand.NextFloat(-increment, increment);
                if (offset < 0)
                    offset = 0;
                if (offset > array3[0])
                    offset = array3[0];
                int d = Dust.NewDust(Projectile.position + Projectile.velocity * offset,
                    Projectile.width, Projectile.height, 86, 0f, 0f, 0, default, Main.rand.NextFloat(4f, 8f));
                Main.dust[d].noGravity = true;
                Main.dust[d].velocity += Projectile.velocity * 0.5f;
                Main.dust[d].velocity *= Main.rand.NextFloat(12f, 24f);
            }
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.Ichor, 2); //lots of defense down stack to make damage calc consistent
            target.AddBuff(BuffID.WitheredArmor, 2);
            target.AddBuff(BuffID.BrokenArmor, 2);
            //target.AddBuff(ModContent.BuffType<Rotting>(), 2);
            //target.AddBuff(ModContent.BuffType<MutantNibble>(), 2);
            //target.AddBuff(ModContent.BuffType<Stunned>(), 2);
            //target.AddBuff(ModContent.BuffType<CurseoftheMoon>(), 2);
            target.AddBuff(ModContent.BuffType<Lovestruck>(), 360);
            target.AddBuff(ModContent.BuffType<Defenseless>(), 1800);

            target.velocity.X = 0;
            target.velocity.Y = -0.4f;
        }

        public float WidthFunction(float trailInterpolant)
        {
            float baseWidth = Projectile.scale * Projectile.width;

            return baseWidth * 0.7f;
            float sine = (float)(0.5 * (1 + Math.Sin(Main.GlobalTimeWrappedHourly - trailInterpolant) / 2));
            float cosine = (float)(0.5 * (1 + Math.Cos(Main.GlobalTimeWrappedHourly - trailInterpolant)));

            //float y = (float)Math.Abs(sine + cosine);

            //float y = (float)Math.Abs(Math.Pow(Math.E, -2 * trailInterpolant) * Math.Cos( trailInterpolant));

            //float y = (float)Math.Abs(Math.Sin(trailInterpolant + 4) - Math.Sin(trailInterpolant / 2 + 2));

            //float y = (float)(4 / (3 + Math.Pow((4 * trailInterpolant - 5 * Main.GlobalTimeWrappedHourly), 2)));

            float y = (float)Math.Sin(10 * Main.GlobalTimeWrappedHourly - 6 * trailInterpolant);

            float localInterpolant = (y + (1 - trailInterpolant)) / 2;
            return MathHelper.Lerp(baseWidth * 0.7f, baseWidth * 1.3f, localInterpolant);
        }
        public static Color[] DeviColors => new Color[] { new(216, 108, 224), new(232, 140, 240), new(224, 16, 216), new(240, 220, 240)};
        public Color ColorFunction(float trailInterpolant)
        {
            float time = (float)(0.5 * (1 + Math.Sin(1.5f * Main.GlobalTimeWrappedHourly % 1)));
            float localInterpolant = (time + (1 - trailInterpolant)) / 2;
            List<Color> colors = new();
            List<Color> colorsToAdd = new() { Color.Red, Color.Green, Color.Blue };
            for (int i = 0; i < 3; i++)
                colors.AddRange(colorsToAdd);

            Color[] colorArray = colors.ToArray();
            int currentColorIndex = (int)(localInterpolant * (colorArray.Length));
            Color val = colorArray[currentColorIndex];
            Color nextColor = colorArray[(currentColorIndex + 1) % colorArray.Length];
            return Color.Lerp(val, nextColor, localInterpolant * colorArray.Length % 1f) * 2;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            // Get the laser end position.
            Vector2 laserEnd = Projectile.Center + Projectile.velocity.SafeNormalize(Vector2.UnitY) * drawDistance;

            // Create 8 points that span across the draw distance from the projectile center.

            // This allows the drawing to be pushed back, which is needed due to the shader fading in at the start to avoid
            // sharp lines.
            Vector2 initialDrawPoint = Projectile.Center - Projectile.velocity * 300f;
            Vector2[] baseDrawPoints = new Vector2[8];
            for (int i = 0; i < baseDrawPoints.Length; i++)
                baseDrawPoints[i] = Vector2.Lerp(initialDrawPoint, laserEnd, i / (float)(baseDrawPoints.Length - 1f));

          

            #region MainLaser

            LaserDrawer ??= new PrimDrawer(WidthFunction, ColorFunction, GameShaders.Misc["FargowiltasSouls:DeviBigDeathray"]);

            // Set shader parameters. This one takes two lots of fademaps and colors for two different overlayed textures.

            // Initial fademap
            GameShaders.Misc["FargowiltasSouls:DeviBigDeathray"].UseColor(Color.White);
            // GameShaders.Misc["FargoswiltasSouls:MutantDeathray"].UseImage1(); cannot be used due to only accepting vanilla paths.
            Asset<Texture2D> fademap = ModContent.Request<Texture2D>("FargowiltasSouls/ExtraTextures/DevBackStreak");
            GameShaders.Misc["FargowiltasSouls:DeviBigDeathray"].SetShaderTexture(fademap);

            // Secondary fademap
            // GameShaders.Misc["FargoswiltasSouls:MutantDeathray"].UseImage1(); cannot be used due to only accepting vanilla paths.
            Asset<Texture2D> fademap2 = ModContent.Request<Texture2D>("FargowiltasSouls/ExtraTextures/DevInnerStreak");
            GameShaders.Misc["FargowiltasSouls:DeviBigDeathray"].SetShaderTexture2(fademap2);
            LaserDrawer.DrawPrims(baseDrawPoints.ToList(), -Main.screenPosition, 40);
            #endregion

            #region Rings

            RingDrawer ??= new PrimDrawer(RingWidthFunction, RingColorFunction, GameShaders.Misc["FargowiltasSouls:DeviRing"]);

            Vector2 velocity = Projectile.velocity.SafeNormalize(Vector2.UnitY);
            velocity = velocity.RotatedBy(MathHelper.PiOver2) * 1250;

            Vector2 currentLaserPosition;
            Asset<Texture2D> ringText;
            int iterator = 0;
            // We want to create a ring on every point on the trail.
            for (int i = 1; i <= baseDrawPoints.Length; i += 2)
            {
                // Get the current position, and rotate it to face out.
                currentLaserPosition = baseDrawPoints[i];
                // Get the velocity from the projectile velocity, rotated.

                // Move the current position back by half the velocity, so we start drawing at the edge.
                // For some FUCKING reason, 0.5 doesnt center them properly here..
                float velocityScaler = MathHelper.Lerp(1f, 0.8f, (float)i / baseDrawPoints.Length);
                velocity *= velocityScaler;
                currentLaserPosition -= velocity * 0.48f;

                Vector2[] ringDrawPoints = new Vector2[4];
                for (int j = 0; j < ringDrawPoints.Length; j++)
                    ringDrawPoints[j] = Vector2.Lerp(currentLaserPosition, currentLaserPosition + velocity, j / (float)(ringDrawPoints.Length - 1f));

                GameShaders.Misc["FargowiltasSouls:DeviRing"].UseColor(new Color(216, 108, 224));

                ringText = ModContent.Request<Texture2D>("FargowiltasSouls/ExtraTextures/" + RingTexturePaths[iterator]);
                GameShaders.Misc["FargowiltasSouls:DeviRing"].SetShaderTexture(ringText);
                GameShaders.Misc["FargowiltasSouls:DeviRing"].Shader.Parameters["stretchAmount"].SetValue(1f);

                float scrollSpeed = MathHelper.Lerp(0.3f,0.5f, 1 - i / ((baseDrawPoints.Length / 2) - 1));
                GameShaders.Misc["FargowiltasSouls:DeviRing"].Shader.Parameters["scrollSpeed"].SetValue(scrollSpeed);

                RingDrawer.DrawPrims(ringDrawPoints.ToList(), -Main.screenPosition, 10);
                iterator++;
            }
            #endregion

            // Draw a big glow above the start of the laser, to help mask the intial fade in due to the immense width.
            Texture2D glowTexture = ModContent.Request<Texture2D>("FargowiltasSouls/Projectiles/GlowRing").Value;
            Vector2 glowDrawPosition = Projectile.Center - Projectile.velocity * 320f;
            //Main.EntitySpriteDraw(glowTexture, glowDrawPosition - Main.screenPosition, null, Color.LavenderBlush, Projectile.rotation, glowTexture.Size() * 0.5f, Projectile.scale * 0.3f, SpriteEffects.None, 0);

            return false;
        }

        public float RingWidthFunction(float trailInterpolant)
        {
            return Projectile.scale * 5;
        }
        public Color RingColorFunction(float trailInterpolant)
        {
            float time = (float)(0.5 * (1 + Math.Sin(Main.GlobalTimeWrappedHourly - trailInterpolant) / 2));
            float localInterpolant = (time + (1 - trailInterpolant)) / 2;
            List<Color> colors = new();
            List<Color> colorsToAdd = new() { Color.Yellow, Color.Blue};
            for (int i = 0; i < 2; i++)
                colors.AddRange(colorsToAdd);

            Color[] colorArray = colors.ToArray();
            int currentColorIndex = (int)(localInterpolant * (colorArray.Length));
            Color val = colorArray[currentColorIndex];
            Color nextColor = colorArray[(currentColorIndex + 1) % colorArray.Length];
            return Color.Lerp(val, nextColor, localInterpolant * colorArray.Length % 1f) * 4;
        }
    }
}