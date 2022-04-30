using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;

namespace FargowiltasSouls.UI
{
    public class SoulTogglerButton : UIState
    {
        public UIImage Icon;
        public UIHoverTextImageButton IconHighlight;
        public UIOncomingMutant OncomingMutant;

        public override void OnActivate()
        {
            Icon = new UIImage(FargowiltasSouls.UserInterfaceManager.SoulTogglerButtonTexture);
            Icon.Left.Set(570, 0); //Icon.Left.Set(26, 0);
            Icon.Top.Set(275, 0); //Icon.Top.Set(300, 0);
            Append(Icon);

            IconHighlight = new UIHoverTextImageButton(FargowiltasSouls.UserInterfaceManager.SoulTogglerButton_MouseOverTexture, "Configure Accessory Effects");
            IconHighlight.Left.Set(-2, 0);
            IconHighlight.Top.Set(-2, 0);
            IconHighlight.SetVisibility(1f, 0);
            IconHighlight.OnClick += IconHighlight_OnClick;
            Icon.Append(IconHighlight);

            OncomingMutant = new UIOncomingMutant(FargowiltasSouls.UserInterfaceManager.OncomingMutantTexture.Value, FargowiltasSouls.UserInterfaceManager.OncomingMutantAuraTexture.Value, "Eternity Mode is enabled", "Masochist Mode is enabled");
            OncomingMutant.Left.Set(610, 0);
            OncomingMutant.Top.Set(250, 0);
            Append(OncomingMutant);

            base.OnActivate();
        }

        private void IconHighlight_OnClick(UIMouseEvent evt, UIElement listeningElement)
        {
            if (!Main.playerInventory)
            {
                return;
            }

            FargowiltasSouls.UserInterfaceManager.ToggleSoulToggler();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (Main.playerInventory)
                base.Draw(spriteBatch);
        }
    }
}
