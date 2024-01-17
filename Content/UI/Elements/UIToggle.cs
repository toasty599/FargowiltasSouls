using FargowiltasSouls.Assets.UI;
using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using FargowiltasSouls.Content.Items.Accessories.Essences;
using FargowiltasSouls.Content.Items.Accessories.Expert;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.ModPlayers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Graphics;
using System.Linq;
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
            Player player = Main.LocalPlayer;
            FargoSoulsPlayer modPlayer = player.FargoSouls();

            if (IsMouseHovering && Main.mouseLeft && Main.mouseLeftRelease)
            {
                modPlayer.Toggler.Toggles[Effect].ToggleBool = !modPlayer.Toggler.Toggles[Effect].ToggleBool;

                if (Main.netMode == NetmodeID.MultiplayerClient)
                    modPlayer.SyncToggle(Effect);
            }

            bool disabledByMinos = (Effect.MinionEffect || Effect.ExtraAttackEffect) && modPlayer.PrimeSoulActive;
            bool disabledByPresence = modPlayer.MutantPresence && !Effect.IgnoresMutantPresence;

            spriteBatch.Draw(FargoUIManager.CheckBox.Value, position, Color.White);

            if (disabledByMinos)
                spriteBatch.Draw(FargoUIManager.Cross.Value, position, Color.Cyan);
            else if (disabledByPresence)
                spriteBatch.Draw(FargoUIManager.Cross.Value, position, Color.Gray);
            else if (Main.LocalPlayer.GetToggleValue(Effect, true))
                spriteBatch.Draw(FargoUIManager.CheckMark.Value, position, Color.White);

            string text = Effect.ToggleDescription;
            position += new Vector2(Width.Pixels * Main.UIScale, 0);
            position += new Vector2(CheckboxTextSpace, 0);
            position += new Vector2(0, Font.MeasureString(text).Y * 0.175f);
            Color color = Color.White;
            if (Effect.ToggleItemType > 0)
            {
                Item item = ContentSamples.ItemsByType[Effect.ToggleItemType];
                if (item.ModItem != null)
                {
                    if (item.ModItem is BaseEnchant enchant)
                        color = enchant.nameColor;
                    else if (item.ModItem is BaseEssence essence)
                        color = essence.nameColor;
                }

            }
            if (disabledByMinos)
            {
                color = Color.Cyan * 0.5f;
                text += $" [i:{ModContent.ItemType<PrimeSoul>()}]";
            }
            else if (disabledByPresence)
            {
                color = Color.Gray * 0.5f;
                text += $" [i:{ModContent.ItemType<OncomingMutantItem>()}]";
            }
            Utils.DrawBorderString(spriteBatch, text, position, color);
        }
    }
}
