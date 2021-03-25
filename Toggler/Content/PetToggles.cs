using Terraria.ID;

namespace FargowiltasSouls.Toggler.Content
{
    public class PetToggles : ToggleCollection
    {
        public override string Mod => "Terraria";
        public override string SortCatagory => "Pets";
        public override int Priority => 2;

        public int PetHeader = ItemID.MagicLantern;
        public string PetLantern;
    }
}
