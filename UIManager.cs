using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.UI;
using FargowiltasSouls.UI;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;
using Terraria.ID;

namespace FargowiltasSouls
{
    public class UIManager
    {
        public UserInterface TogglerUserInterface;
        public UserInterface TogglerToggleUserInterface;
        public SoulToggler SoulToggler;
        public SoulTogglerButton SoulTogglerButton;
        private GameTime _lastUpdateUIGameTime;

        public Texture2D CheckMark;
        public Texture2D CheckBox;
        public Texture2D SoulTogglerButtonTexture;
        public Texture2D SoulTogglerButton_MouseOverTexture;

        public void LoadUI()
        {
            if (!Main.dedServ)
            {
                // Load textures
                CheckMark = ModContent.GetTexture("FargowiltasSouls/UI/Assets/CheckMark");
                CheckBox = ModContent.GetTexture("FargowiltasSouls/UI/Assets/CheckBox");
                SoulTogglerButtonTexture = ModContent.GetTexture("FargowiltasSouls/UI/Assets/SoulTogglerToggle");
                SoulTogglerButton_MouseOverTexture = ModContent.GetTexture("FargowiltasSouls/UI/Assets/SoulTogglerToggle_MouseOver");

                // Initialize UserInterfaces
                TogglerUserInterface = new UserInterface();
                TogglerToggleUserInterface = new UserInterface();

                // Activate UIs
                SoulToggler = new SoulToggler();
                SoulToggler.Activate();
                SoulTogglerButton = new SoulTogglerButton();
                SoulTogglerButton.Activate();

                TogglerToggleUserInterface.SetState(SoulTogglerButton);
            }
        }

        public void UpdateUI(GameTime gameTime)
        {
            _lastUpdateUIGameTime = gameTime;

            if (!Main.playerInventory && SoulConfig.Instance.HideTogglerWhenInventoryIsClosed)
                CloseSoulToggler();

            if (TogglerUserInterface?.CurrentState != null)
                TogglerUserInterface.Update(gameTime);
            if (TogglerToggleUserInterface?.CurrentState != null)
                TogglerToggleUserInterface.Update(gameTime);
        }

        public bool IsSoulTogglerOpen() => TogglerUserInterface?.CurrentState == null;
        public void CloseSoulToggler() => TogglerUserInterface?.SetState(null);
        public bool IsTogglerOpen() => TogglerUserInterface.CurrentState == SoulToggler;
        public void OpenToggler() => TogglerUserInterface.SetState(SoulToggler);

        public void ToggleSoulToggler()
        {
            if (IsSoulTogglerOpen())
            {
                Main.PlaySound(SoundID.MenuOpen);
                OpenToggler();
            }
            else if (IsTogglerOpen())
            {
                Main.PlaySound(SoundID.MenuClose);
                CloseSoulToggler();
            }
        }

        public void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int index = layers.FindIndex((layer) => layer.Name == "Vanilla: Cursor");
            if (index != -1)
            {
                layers.Insert(index - 1, new LegacyGameInterfaceLayer("Fargos: Soul Toggler", delegate
                {
                    if (_lastUpdateUIGameTime != null && TogglerUserInterface?.CurrentState != null)
                        TogglerUserInterface.Draw(Main.spriteBatch, _lastUpdateUIGameTime);
                    return true;
                }, InterfaceScaleType.UI));
            }

            index = layers.FindIndex((layer) => layer.Name == "Vanilla: Mouse Text");
            if (index != -1)
            {
                layers.Insert(index, new LegacyGameInterfaceLayer("Fargos: Soul Toggler Toggler", delegate
                {
                    if (_lastUpdateUIGameTime != null && TogglerToggleUserInterface?.CurrentState != null)
                        TogglerToggleUserInterface.Draw(Main.spriteBatch, _lastUpdateUIGameTime);

                    return true;
                }, InterfaceScaleType.UI));
            }
        }
    }
}
