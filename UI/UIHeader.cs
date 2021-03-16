using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.UI;

namespace FargowiltasSouls.UI
{
    public class UIHeader : UIElement
    {
        public const int TextureTextPadding = 8;
        public const int TextureBarPadding = 4;
        public const int ItemTextureDimensions = 32;

        string Text;
        int Item;

        public UIHeader(string text, int item, (int width, int height) dimensions)
        {
            Text = text;
            Item = item;
            Width.Set(dimensions.width, 0f);
            Height.Set(dimensions.height, 0f);
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);

            CalculatedStyle dimensions = GetDimensions();
            Vector2 position = new Vector2(dimensions.X, dimensions.Y);
            // I honestly forget why I did half of these calculations at this point
            spriteBatch.Draw(Main.magicPixel, new Rectangle((int)position.X + 1, /*Y*/ (int)dimensions.Y + 20 - 1 + 0, (int)dimensions.Width - 2, 1), Color.Black);
            spriteBatch.Draw(Main.magicPixel, new Rectangle((int)position.X + 1, /*Y*/ (int)dimensions.Y + 20 - 1 + 1, (int)dimensions.Width - 2, 1), Color.LightGray);
            spriteBatch.Draw(Main.magicPixel, new Rectangle((int)position.X + 1, /*Y*/ (int)dimensions.Y + 20 - 1 + 2, (int)dimensions.Width - 2, 1), Color.Gray);
            spriteBatch.Draw(Main.magicPixel, new Rectangle((int)position.X + 1, /*Y*/ (int)dimensions.Y + 20 - 1 + 3, (int)dimensions.Width - 2, 1), Color.Black);

            // "Caps" at the end of the line
            spriteBatch.Draw(Main.magicPixel, new Rectangle((int)position.X, /*Y*/ (int)dimensions.Y + 20 - 1 + 1, 1, 2), Color.Black);
            spriteBatch.Draw(Main.magicPixel, new Rectangle((int)position.X + (int)dimensions.Width - 1, /*Y*/ (int)dimensions.Y + 20 - 1 + 1, 1, 2), Color.Black);

            Utils.DrawBorderString(spriteBatch, $"[i:{Item}] {Text}", position, Color.White);

            //Texture2D itemTexture = Main.itemTexture[Item];
            //spriteBatch.Draw(itemTexture, new Rectangle((int)position.X, (int)position.Y + 2, 16, 16), Color.White);
        }
    }
}
