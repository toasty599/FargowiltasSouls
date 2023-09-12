using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace FargowiltasSouls.Common.Graphics.Particles
{
	public class SmallSparkle : Particle
	{
		public readonly bool UseBloom;

		public static int FadeTime => 15;

		public SmallSparkle(Vector2 worldPosition, Vector2 velocity, Color drawColor, float scale, int lifetime, float rotation = 0f, float rotationSpeed = 0f, bool useBloom = true, Color? bloomColor = null)
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
			Opacity = (FadeIn ? Utils.GetLerpValue(0f, FadeTime, Timer, true) : 1f) * Utils.GetLerpValue(MaxLifetime, MaxLifetime - FadeTime, Timer, true);
			Velocity *= 0.99f;
		}

		public override void Draw(SpriteBatch spriteBatch)
		{
			if (UseBloom)
				spriteBatch.Draw(CommonBloomTexture, Position - Main.screenPosition, null, BloomColor with { A = 0 } * 0.5f * Opacity, 0f, CommonBloomTexture.Size() * 0.5f, Scale * 0.08f,
					SpriteEffects.None, 0f);

			spriteBatch.Draw(MainTexture, Position - Main.screenPosition, null, DrawColor with { A = 0 } * Opacity, Rotation, MainTexture.Size() * 0.5f, Scale, SpriteEffects.None, 0f);

			if (UseBloom)
				spriteBatch.Draw(MainTexture, Position - Main.screenPosition, null, BloomColor with { A = 0 } * 0.5f * Opacity, Rotation, MainTexture.Size() * 0.5f, Scale, SpriteEffects.None, 0f);
		}
	}
}