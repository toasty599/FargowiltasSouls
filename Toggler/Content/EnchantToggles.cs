using FargowiltasSouls.Items.Accessories.Forces;
using Terraria.ModLoader;

namespace FargowiltasSouls.Toggler.Content
{
    public class EnchantToggles : ToggleCollection
    {
        public override string Mod => "Terraria";
        public override string SortCatagory => "Enchantments";
        public override int Priority => 1;

        public int WoodHeader = ModContent.ItemType<TimberForce>();
        public string Boreal;
        public string Ebon;
        public string Shade;
        public string Mahogany;
        public string Palm;
        public string Pearl;
    }
}
