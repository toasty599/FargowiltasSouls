using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace FargowiltasSouls.Common.Graphics.Particles
{
	public class SparkParticle : Particle
	{
		public readonly bool UseBloom;

		public override bool UseAdditiveBlend => true;

		public SparkParticle(Vector2 worldPosition, Vector2 velocity, Color drawColor, float scale, int lifetime, bool useBloom = true, Color? bloomColor = null)
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
			// Shrink, fade, and slow over time.
			Velocity *= 0.95f;
			Scale *= 0.95f;
			Opacity = FargoSoulsUtil.SineInOut(1f - LifetimeRatio);
			Rotation = Velocity.ToRotation() + MathHelper.PiOver2;
		}

		public override void Draw(SpriteBatch spriteBatch)
		{
			Vector2 scale = new Vector2(0.5f, 1.6f) * Scale;
			spriteBatch.Draw(MainTexture, Position - Main.screenPosition, null, DrawColor, Rotation, MainTexture.Size() * 0.5f, scale, 0, 0f);
			spriteBatch.Draw(MainTexture, Position - Main.screenPosition, null, DrawColor, Rotation, MainTexture.Size() * 0.5f, scale * new Vector2(0.45f, 1f), 0, 0f);

			if (UseBloom)
				spriteBatch.Draw(MainTexture, Position - Main.screenPosition, null, BloomColor * 0.5f, Rotation, MainTexture.Size() * 0.5f, scale, 0, 0f);
		}
	}
}
