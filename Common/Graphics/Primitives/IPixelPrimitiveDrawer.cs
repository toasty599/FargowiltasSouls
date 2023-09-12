using Microsoft.Xna.Framework.Graphics;

namespace FargowiltasSouls.Common.Graphics.Primitives
{
    public interface IPixelPrimitiveDrawer
    {
        /// <summary>
        /// Whether <see cref="DrawPixelPrimitives(SpriteBatch)"/> is rendered over projectiles or before npcs.
        /// </summary>
        public bool RenderOverProjectiles => false;

        /// <summary>
        /// Draw primitives you wish to become pixelated here.
        /// </summary>
        public void DrawPixelPrimitives(SpriteBatch spriteBatch);
    }
}