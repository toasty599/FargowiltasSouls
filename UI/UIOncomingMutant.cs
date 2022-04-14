using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Terraria.GameContent.UI.Elements;
using System.Collections.Generic;
using System.Linq;
using Terraria.UI;
using Terraria;
using Terraria.GameInput;

namespace FargowiltasSouls.UI
{
    public class UIOncomingMutant : UIPanel
    {
        // Stores the offset from the top left of the UIPanel while dragging.
        private Vector2 offset;
        public bool dragging;
        public Texture2D Texture;
        public string TextEMode;
        public string TextMaso;

        public UIOncomingMutant(Texture2D tex, string textEMode, string textMaso)
        {
            Texture = tex;
            TextEMode = textEMode;
            TextMaso = textMaso;

            Width.Set(24, 0);
            Height.Set(26, 0);
        }

        private void DragStart(Vector2 pos)
        {
            offset = new Vector2(pos.X - Left.Pixels, pos.Y - Top.Pixels);
            dragging = true;
        }

        private void DragEnd(Vector2 pos)
        {
            Vector2 end = pos - offset;
            dragging = false;

            Left.Set(end.X, 0);
            Top.Set(end.Y, 0);
            Recalculate();

            StayInBounds();

            SoulConfig.Instance.OncomingMutantX = end.X;
            SoulConfig.Instance.OncomingMutantY = end.Y;
            SoulConfig.Instance.OnChanged();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime); // don't remove.

            if (!FargoSoulsWorld.EternityMode)
                return;

            // Checking ContainsPoint and then setting mouseInterface to true is very common. This causes clicks on this UIElement to not cause the player to use current items. 
            if (ContainsPoint(Main.MouseScreen))
            {
                Main.LocalPlayer.mouseInterface = true;
            }

            if (!dragging && ContainsPoint(Main.MouseScreen) && Main.mouseLeft && PlayerInput.MouseInfoOld.LeftButton == ButtonState.Released)
            {
                DragStart(Main.MouseScreen);
            }
            else if (dragging && !Main.mouseLeft)
            {
                DragEnd(Main.MouseScreen);
            }

            if (dragging)
            {
                Left.Set(Main.mouseX - offset.X, 0); // Main.MouseScreen.X and Main.mouseX are the same.
                Top.Set(Main.mouseY - offset.Y, 0);
                Recalculate();
            }
            else
            {
                Left.Set(SoulConfig.Instance.OncomingMutantX, 0);
                Top.Set(SoulConfig.Instance.OncomingMutantY, 0);
                Recalculate();
            }

            StayInBounds();
        }

        private void StayInBounds()
        {
            // Here we check if the DragableUIPanel is outside the Parent UIElement rectangle. 
            var parentSpace = Parent.GetDimensions().ToRectangle();
            if (!GetDimensions().ToRectangle().Intersects(parentSpace))
            {
                Left.Pixels = Utils.Clamp(Left.Pixels, 0, parentSpace.Right - Width.Pixels);
                Top.Pixels = Utils.Clamp(Top.Pixels, 0, parentSpace.Bottom - Height.Pixels);
                // Recalculate forces the UI system to do the positioning math again.
                Recalculate();
            }
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            if (!FargoSoulsWorld.EternityMode)
                return;

            base.DrawSelf(spriteBatch);

            CalculatedStyle style = GetDimensions();
            // Logic
            if (IsMouseHovering && !dragging)
            {
                Vector2 textPosition = style.Position() + new Vector2(0, style.Height + 8);
                Utils.DrawBorderString(
                    spriteBatch, 
                    FargoSoulsWorld.MasochistModeReal ? TextMaso : TextEMode, 
                    textPosition,
                    FargoSoulsWorld.MasochistModeReal ? new Color(51, 255, 191) : Color.White);
            }

            // Drawing
            Vector2 position = style.Position();
            position += new Vector2(2);
            Rectangle frame = Texture.Bounds;
            spriteBatch.Draw(Texture, position, frame, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0);
        }
    }
}
