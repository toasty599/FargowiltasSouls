using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Microsoft.Xna.Framework;

namespace FargowiltasSouls.Common.Graphics.Particles
{
	public class BigSparkle : Particle
	{
		public readonly bool UseBloom;

		public BigSparkle(Vector2 worldPosition, Vector2 velocity, Color drawColor, float scale, int lifetime, float rotation = 0f, float rotationSpeed = 0f, bool useBloom = true, Color? bloomColor = null)
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
		}

		public override void Update()
		{
			Opacity = (FadeIn ? Utils.GetLerpValue(0f, DefaultFadeTime, Timer, true) : 1f) * Utils.GetLerpValue(MaxLifetime, MaxLifetime - DefaultFadeTime, Timer, true);
			Velocity *= 0.99f;
		}

		public override void Draw(SpriteBatch spriteBatch)
		{
			if (UseBloom)
				spriteBatch.Draw(CommonBloomTexture, Position - Main.screenPosition, null, BloomColor with { A = 0 } * 0.5f * Opacity, 0f, CommonBloomTexture.Size() * 0.5f, Scale * 0.17f,
					SpriteEffects.None, 0f);

			spriteBatch.Draw(MainTexture, Position - Main.screenPosition, null, DrawColor with { A = 0 }, Rotation, MainTexture.Size() * 0.5f, Scale, SpriteEffects.None, 0f);

			if (UseBloom)
				spriteBatch.Draw(MainTexture, Position - Main.screenPosition, null, BloomColor with { A = 0 } * 0.5f, Rotation, MainTexture.Size() * 0.5f, Scale, SpriteEffects.None, 0f);
		}
	}
}
