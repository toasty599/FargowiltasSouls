using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Microsoft.Xna.Framework;
using Terraria.ModLoader;

namespace FargowiltasSouls.Common.Graphics.Particles
{
	public class Bubble : Particle
	{
		public readonly bool UseBloom;

		public readonly float BaseOpacity = 1;
		public override int MaxFrames => 2;
		public override bool UseNonPreMultipliedBlend => true;
		public Bubble(Vector2 worldPosition, Vector2 velocity, float scale, int lifetime, float baseOpacity = 1, float rotation = 0f, float rotationSpeed = 0f)
		{
			Position = worldPosition;
			Velocity = velocity;
			DrawColor = Color.White;
			Scale = new(scale);
			MaxLifetime = lifetime;
			Rotation = rotation;
			RotationSpeed = rotationSpeed;
			UseBloom = false;

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
	}
}
