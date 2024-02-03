using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.UI.Elements
{
    // Exists to be displayed as an item icon in the Toggler UI when inflicted with Mutant's Presence.
    public class TogglerIconItem : ModItem
    {
        public override string Texture => "FargowiltasSouls/Assets/UI/SoulTogglerToggle";
    }
}
