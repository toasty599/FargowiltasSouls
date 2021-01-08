using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;

namespace FargowiltasSouls.UI
{
    public class UISoulToggler : UIState
    {
        public const int BackWidth = 400;
        public const int BackHeight = 600;

        public UIDragablePanel BackPanel;

        public override void OnInitialize()
        {
            BackPanel = new UIDragablePanel();
            BackPanel.Left.Set(Main.screenWidth / 2f - BackHeight / 2, 0f);
            BackPanel.Top.Set(Main.screenHeight / 2f - BackHeight / 2, 0f);
            BackPanel.Width.Set(BackWidth, 0f);
            BackPanel.Height.Set(BackHeight, 0f);
            Append(BackPanel);
            base.OnInitialize();
        }
    }
}
