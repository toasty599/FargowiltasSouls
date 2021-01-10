using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.UI;
using FargowiltasSouls.Toggler;
using ReLogic.Graphics;
using Terraria.Localization;
using Terraria.GameContent.UI.Elements;

namespace FargowiltasSouls.UI
{
    public class UIToggle : UIElement
    {
        public const int CheckboxTextSpace = 4;

        public DynamicSpriteFont Font => Main.fontItemStack;

        public UIText Name;
        public string Key;

        public UIToggle(string key)
        {
            Key = key;

            Width.Set(19, 0f);
            Height.Set(21, 0f);

            Name = new UIText(Language.GetTextValue($"Mods.FargowiltasSouls.{Key}Config"));
            Vector2 position = GetDimensions().Position();
            position += new Vector2(Width.Pixels * Main.UIScale, 0);
            position += new Vector2(CheckboxTextSpace, 0);
            position += new Vector2(0, Font.MeasureString(Name.Text).Y * 0.175f);
            Name.Left.Set(position.X, 0f);
            Name.Top.Set(position.Y, 0f);
            Append(Name);

            OnClick += UIToggle_OnClick;
        }

        private void UIToggle_OnClick(UIMouseEvent evt, UIElement listeningElement)
        {
            Main.LocalPlayer.SetToggleValue(Key, !Main.LocalPlayer.GetToggleValue(Key, false, false));
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);
            Vector2 position = GetDimensions().Position();

            spriteBatch.Draw(Fargowiltas.UserInterfaceManager.CheckBox, position, Color.White);
            if (Main.LocalPlayer.GetToggleValue(Key, false, false))
                spriteBatch.Draw(Fargowiltas.UserInterfaceManager.CheckMark, position, Color.White);
        }        
    }
}
