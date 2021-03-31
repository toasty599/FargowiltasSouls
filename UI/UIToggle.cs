using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.UI;
using FargowiltasSouls.Toggler;
using ReLogic.Graphics;
using Terraria.Localization;

namespace FargowiltasSouls.UI
{
    public class UIToggle : UIElement
    {
        public const int CheckboxTextSpace = 4;

        public DynamicSpriteFont Font => Main.fontItemStack;

        public string Key;

        public UIToggle(string key)
        {
            Key = key;

            Width.Set(19, 0f);
            Height.Set(21, 0f);
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);
            Vector2 position = GetDimensions().Position();

            if (IsMouseHovering && Main.mouseLeft && Main.mouseLeftRelease)
            {
                Main.LocalPlayer.SetToggleValue(Key, !Main.LocalPlayer.GetToggleValue(Key, false));
            }

            spriteBatch.Draw(Fargowiltas.UserInterfaceManager.CheckBox, position, Color.White);
            if (Main.LocalPlayer.GetToggleValue(Key, false))
                spriteBatch.Draw(Fargowiltas.UserInterfaceManager.CheckMark, position, Color.White);

            string text = Language.GetTextValue($"Mods.FargowiltasSouls.{Key}Config");
            position += new Vector2(Width.Pixels * Main.UIScale, 0);
            position += new Vector2(CheckboxTextSpace, 0);
            position += new Vector2(0, Font.MeasureString(text).Y * 0.175f);

            Utils.DrawBorderString(spriteBatch, text, position, Color.White);
        }        
    }
}
