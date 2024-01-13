using FargowiltasSouls.Core.AccessoryEffectSystem;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;

namespace FargowiltasSouls.Content.UI.Elements
{
	public class UIToggle : UIElement
    {
        public const int CheckboxTextSpace = 4;

        public static DynamicSpriteFont Font => Terraria.GameContent.FontAssets.ItemStack.Value;

        public AccessoryEffect Effect;
        public string Mod;

        public UIToggle(AccessoryEffect effect, string mod)
        {
            Effect = effect;
            Mod = mod;

            Width.Set(19, 0);
            Height.Set(21, 0);
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);
            Vector2 position = GetDimensions().Position();

            if (IsMouseHovering && Main.mouseLeft && Main.mouseLeftRelease)
            {
                Player player = Main.LocalPlayer;
                FargoSoulsPlayer modPlayer = player.FargoSouls();
                modPlayer.Toggler.Toggles[Effect].ToggleBool = !modPlayer.Toggler.Toggles[Effect].ToggleBool;

                if (Main.netMode == NetmodeID.MultiplayerClient)
                    modPlayer.SyncToggle(Effect);
            }

            spriteBatch.Draw(FargoUIManager.CheckBox.Value, position, Color.White);
            if (Main.LocalPlayer.GetToggleValue(Effect, true))
                spriteBatch.Draw(FargoUIManager.CheckMark.Value, position, Color.White);

            string text = Effect.ToggleDescription;
            position += new Vector2(Width.Pixels * Main.UIScale, 0);
            position += new Vector2(CheckboxTextSpace, 0);
            position += new Vector2(0, Font.MeasureString(text).Y * 0.175f);

            Utils.DrawBorderString(spriteBatch, text, position, Color.White);
        }
    }
}
