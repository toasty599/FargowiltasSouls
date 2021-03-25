using FargowiltasSouls.Items.Accessories.Forces;
using Terraria.ModLoader;

namespace FargowiltasSouls.Toggler.Content
{
    public class EnchantToggles : ToggleCollection
    {
        public override string Mod => "Terraria";
        public override string SortCatagory => "Enchantments";
        public override int Priority => 0;

        public int WoodHeader = ModContent.ItemType<TimberForce>();
        public string Boreal;
        public string Ebon;
        public string Shade;
        public string Mahogany;
        public string Palm;
        public string Pearl;

        public int EarthHeader = ModContent.ItemType<EarthForce>();
        public string Adamantite;
        public string Cobalt;
        public string AncientCobalt;
        public string Mythril;
        public string Orichalcum;
        public string Palladium;
        public string PalladiumOrb;
        public string Titanium;
    }
}
