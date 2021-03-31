using FargowiltasSouls.Toggler;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.UI;

namespace FargowiltasSouls.UI
{
    public class UIPresetButton : UIElement
    {
        public Texture2D Texture;
        public Action<ToggleBackend> ApplyPreset;
        public string Text;

        public UIPresetButton(Texture2D tex, Action<ToggleBackend> preset, string text)
        {
            Texture = tex;
            ApplyPreset = preset;
            Text = text;

            Width.Set(20, 0f);
            Height.Set(20, 0f);
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);

            CalculatedStyle style = GetDimensions();
            bool hovered = false;
            // Logic
            if (IsMouseHovering)
            {
                Vector2 textPosition = style.Position() + new Vector2(0, style.Height + 8);
                Utils.DrawBorderString(spriteBatch, Text, textPosition, Color.White);

                hovered = true;

                if (Main.mouseLeft && Main.mouseLeftRelease)
                {
                    ApplyPreset(Main.LocalPlayer.GetModPlayer<FargoPlayer>().Toggler);
                }
            }

            // Drawing
            Texture2D outlineTexture = Fargowiltas.UserInterfaceManager.PresetButtonOutline;
            Vector2 position = style.Position();
            spriteBatch.Draw(outlineTexture, position, null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);

            position += new Vector2(2);
            Rectangle frame = new Rectangle(0, 0, 16, 16);
            if (hovered)
                frame.X += 16;
            spriteBatch.Draw(Texture, position, frame, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
        }
    }
}
