using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Microsoft.Xna.Framework;

namespace FargowiltasSouls.Common.Graphics.Particles
{
	public class FogPuff : Particle
	{
		public readonly bool UseBloom;

		public readonly float BaseOpacity = 1;
		public override int MaxFrames => 16;
		public override bool UseNonPreMultipliedBlend => true;
		public FogPuff(Vector2 worldPosition, Vector2 velocity, Color drawColor, float scale, int lifetime, float baseOpacity = 1, float rotation = 0f, float rotationSpeed = 0f, bool useBloom = true, Color? bloomColor = null)
		{
			Position = worldPosition;
			Velocity = velocity;
			DrawColor = drawColor;
			Scale = new(scale);
			MaxLifetime = lifetime;
			Rotation = rotation;
			RotationSpeed = rotationSpeed;
			UseBloom = useBloom;
			bloomColor ??= Color.White;
			BloomColor = bloomColor.Value;

			BaseOpacity = baseOpacity;

            CurrentFrame = Main.rand.Next(MaxFrames);

			Height = MainTexture.Height / MaxFrames;
			Width = MainTexture.Width;
		}
		public override void Update()
		{
			Opacity = (FadeIn ? Utils.GetLerpValue(0f, DefaultFadeTime, Timer, true) : 1f) * Utils.GetLerpValue(MaxLifetime, MaxLifetime - DefaultFadeTime, Timer, true);
			Velocity *= 0.99f;
		}
		public override void Draw(SpriteBatch spriteBatch)
		{
            Rectangle? frame = null;
            Vector2 origin = MainTexture.Size() * 0.5f;
            if (UsesFrames)
            {
                frame = new(0, CurrentFrame * Height, Width, Height);
                origin = frame.Value.Size() * 0.5f;
            }

            Vector2 screenPos = Position - Main.screenPosition;

            spriteBatch.Draw(MainTexture, screenPos, frame, DrawColor * Opacity * BaseOpacity, Rotation, origin, Scale, SpriteEffects.None, 0f);
        }
		/*
        public override void Draw(SpriteBatch spriteBatch)
		{
			int frameHeight = MainTexture.Height / 16;
			Rectangle drawRect = new(0, frameHeight * Frame, MainTexture.Width, frameHeight);

            if (UseBloom)
				spriteBatch.Draw(CommonBloomTexture, Position - Main.screenPosition, null, BloomColor with { A = 0 } * 0.5f * Opacity, 0f, CommonBloomTexture.Size() * 0.5f, Scale * 0.17f,
					SpriteEffects.None, 0f);

			spriteBatch.Draw(MainTexture, Position - Main.screenPosition, drawRect, DrawColor with { A = 0 }, Rotation, drawRect.Size() * 0.5f, Scale, SpriteEffects.None, 0f);

			if (UseBloom)
				spriteBatch.Draw(MainTexture, Position - Main.screenPosition, drawRect, BloomColor with { A = 0 } * 0.5f, Rotation, drawRect.Size() * 0.5f, Scale, SpriteEffects.None, 0f);
		}
		*/
	}
}
