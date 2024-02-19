using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace FargowiltasSouls.Common.Graphics.Particles
{
	public class ElectricSpark : Particle
	{
		public readonly bool UseBloom;
		public override bool UseAdditiveBlend => true;
		public static int FadeTime => 15;

		public ElectricSpark(Vector2 worldPosition, Vector2 velocity, Color drawColor, float scale, int lifetime, bool useBloom = true, Color? bloomColor = null)
		{
			Position = worldPosition;
			Velocity = velocity;
			DrawColor = drawColor;
			Scale = new(scale);
			MaxLifetime = lifetime;
			UseBloom = useBloom;
			bloomColor ??= Color.White;
			BloomColor = bloomColor.Value;
		}

		public override void Update()
		{
            Velocity *= 0.95f;
            Scale *= 0.95f;
            Opacity = FargoSoulsUtil.SineInOut(1f - LifetimeRatio);
            Rotation = Velocity.ToRotation() + MathHelper.PiOver2;
        }

		public override void Draw(SpriteBatch spriteBatch)
		{
			Texture2D lightningTexture = MainTexture;
            int lightningFrames = 5;

            Rectangle GetRandomLightningFrame()
            {
                int frameHeight = lightningTexture.Height / lightningFrames;
                int frame = Main.rand.Next(lightningFrames);
                return new(0, frameHeight * frame, lightningTexture.Width, frameHeight);
            }

            Rectangle lightningRect = GetRandomLightningFrame();
            Vector2 lightningOrigin = lightningRect.Size() / 2f;
            spriteBatch.Draw(lightningTexture, Position - Main.screenPosition, lightningRect, DrawColor * Opacity, Rotation, lightningOrigin, Scale, SpriteEffects.None, 0);
            spriteBatch.Draw(lightningTexture, Position - Main.screenPosition, lightningRect, DrawColor * Opacity, Rotation, lightningOrigin, Scale * new Vector2(0.45f, 1f), SpriteEffects.None, 0);

            if (UseBloom)
				spriteBatch.Draw(lightningTexture, Position - Main.screenPosition, lightningRect, BloomColor * 0.5f * Opacity, Rotation, lightningOrigin, Scale, SpriteEffects.None, 0f);
		}
	}
}