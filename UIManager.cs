using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.UI;
using FargowiltasSouls.UI;

namespace FargowiltasSouls
{
    public class UIManager
    {
        public UserInterface UserInterface;
        public UISoulToggler SoulToggler;
        private GameTime _lastUpdateUIGameTime;

        public void LoadUI()
        {
            if (!Main.dedServ)
            {
                UserInterface = new UserInterface();

                SoulToggler = new UISoulToggler();
                SoulToggler.Activate();
            }
        }

        public void UpdateUI(GameTime gameTime)
        {
            _lastUpdateUIGameTime = gameTime;
            if (UserInterface?.CurrentState != null)
            {
                UserInterface.Update(gameTime);
            }
        }

        public bool IsInterfaceClosed() => UserInterface?.CurrentState == null;
        public void CloseInterface() => UserInterface?.SetState(null);
        public bool IsTogglerOpen() => UserInterface.CurrentState == SoulToggler;
        public void OpenToggler() => UserInterface.SetState(SoulToggler);

        public void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int index = layers.FindIndex((layer) => layer.Name == "Vanilla: Cursor");
            if (index != -1)
            {
                layers.Insert(index - 1, new LegacyGameInterfaceLayer("Fargos: Soul Toggler", delegate
                {
                    if (_lastUpdateUIGameTime != null && UserInterface?.CurrentState != null)
                        UserInterface.Draw(Main.spriteBatch, _lastUpdateUIGameTime);

                    return true;
                }, InterfaceScaleType.UI));
            }
        }
    }
}
