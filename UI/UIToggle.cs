using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.UI;
using FargowiltasSouls.Toggler;
using ReLogic.Graphics;
using Terraria.UI.Chat;
using Terraria.Localization;

namespace FargowiltasSouls.UI
{
    public class UIToggle : UIElement
    {
        public const int CheckboxTextSpace = 4;

        public DynamicSpriteFont Font => Main.fontItemStack;

        public string Key;
        public string Text;

        public UIToggle(string key)
        {
            Key = key;
            Text = Language.GetTextValue($"Mods.FargowiltasSouls.{Key}Config");

            OnClick += UIToggle_OnClick;
            Width.Set(19, 0f);
            Height.Set(21, 0f);
        }

        private void UIToggle_OnClick(UIMouseEvent evt, UIElement listeningElement)
        {
            Main.LocalPlayer.SetToggleValue(Key, !Main.LocalPlayer.GetToggleValue(Key, false, false));
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
            Vector2 position = GetDimensions().Position();

            spriteBatch.Draw(Fargowiltas.UserInterfaceManager.CheckBox, position, Color.White);
            if (Main.LocalPlayer.GetToggleValue(Key, false, false))
                spriteBatch.Draw(Fargowiltas.UserInterfaceManager.CheckMark, position, Color.White);

            position += new Vector2(Width.Pixels * Main.UIScale, 0);
            position += new Vector2(CheckboxTextSpace, 0);
            position += new Vector2(0, Font.MeasureString(Text).Y * 0.175f);
            ChatManager.DrawColorCodedStringWithShadow(spriteBatch, Font, Text, position, Color.White, 0f, Vector2.Zero, Vector2.One);
        }        
    }
}
