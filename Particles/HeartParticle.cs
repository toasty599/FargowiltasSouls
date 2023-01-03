using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace FargowiltasSouls.Particles
{
    // THESE DO NOT CURRENTLY WORK, DO NOT USE.
    public class HeartParticle : Particle
    {
        public Color Color;
        public Color InitialColor;
        public float ShrinkTime = 20;

        public override string TexturePath => "FargowiltasSouls/Particles/HeartParticle";

        public override bool UseAdditiveDrawing => true;

        public HeartParticle(Vector2 position, Vector2 velocity, Color color, int lifetime, float scale, float opacity)
        {
            Position = position;
            Velocity = velocity;    
            Color = InitialColor = color;
            Lifetime = TimeLeft = lifetime;
            Scale = scale;
            Opacity = opacity;
        }

        public override void AI()
        {
            Scale *= 0.95f;
            Color = Color.Lerp(InitialColor, Color.Transparent, LifetimeInterpolant);
            Velocity *= 0.95f;
            Rotation = Velocity.ToRotation();
        }

        public override void Draw(SpriteBatch spriteBatch, ref Color lightColor)
        {
            spriteBatch.Draw(Texture, Position, null, Color, Rotation, Origin, Scale, SpriteEffects.None, 0f);
        }
    }
}