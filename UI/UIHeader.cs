using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.UI;

namespace FargowiltasSouls.UI
{
    // Currently broken
    public class UIHeader : UIElement
    {
        public const int TextureTextPadding = 8;
        public const int TextureBarPadding = 4;
        public const int ItemTextureDimensions = 32;

        string Text;
        int Item;

        public UIHeader(string text, int item)
        {
            Text = text;
            Item = item;
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);

            CalculatedStyle dimensions = base.GetDimensions();
            Vector2 position = new Vector2(dimensions.X, dimensions.Y) + new Vector2(8);
            spriteBatch.Draw(Main.magicPixel, new Rectangle((int)dimensions.X + 10, (int)dimensions.Y + (int)dimensions.Height - 2, (int)dimensions.Width - 20, 1), Color.LightGray);
            Utils.DrawBorderString(spriteBatch, Text, position, Color.White);
        }
    }
}
