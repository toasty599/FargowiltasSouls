using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.UI;
using Terraria.GameInput;
using Terraria.GameContent.UI.Elements;

namespace FargowiltasSouls.UI
{
    public class UISearchBar : UIElement
    {
        public const int CharacterLimit = 16;
        public const string HintText = "Search...";

        public bool IsEmpty => string.IsNullOrEmpty(Input);

        public UIText TextDisplayer;
        public UIPanel BackPanel;
        public string Input;
        public bool Focused;
        public int CursorBlinkTimer;

        public delegate void TextChangeDelegate(string oldText, string currentText);
        public event TextChangeDelegate OnTextChange;

        public UISearchBar(int width, int height)
        {
            Width.Set(width, 0f);
            Height.Set(height, 0f);

            BackPanel = new UIPanel();
            BackPanel.Width.Set(width, 0f);
            BackPanel.Height.Set(height, 0f);
            BackPanel.BackgroundColor = new Color(22, 25, 55);
            BackPanel.PaddingLeft = BackPanel.PaddingRight = BackPanel.PaddingTop = BackPanel.PaddingBottom = 0;
            Append(BackPanel);

            TextDisplayer = new UIText(HintText);
            TextDisplayer.Top.Set(6, 0);
            TextDisplayer.Left.Set(6, 0);
            BackPanel.Append(TextDisplayer);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (Main.mouseLeft && Main.mouseLeftRelease)
            {
                if (ContainsPoint(Main.MouseScreen))
                    Focused = !Focused;
                else
                    Focused = false;

                Main.NewText(Focused);
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            PlayerInput.WritingText = Focused;
            if (Focused)
            {
                Main.instance.HandleIME();
                
                string newInput = Main.GetInputText(Input);
                if (newInput != Input)
                {
                    OnTextChange?.Invoke(Input, newInput);
                    Input = newInput;
                }
            }

            string drawInput;
            Color drawColor = Color.White;

            if (!string.IsNullOrEmpty(Input))
            {
                drawInput = Input;

                CursorBlinkTimer++;
                if (CursorBlinkTimer / 20 % 2f < 0.5f)
                {
                    drawInput += "|";
                }
            }
            else
            {
                drawColor = Color.DarkGray;
                drawInput = HintText;
            }

            TextDisplayer = new UIText(drawInput);
            TextDisplayer.TextColor = drawColor;
        }
    }
}
