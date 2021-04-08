using FargowiltasSouls.Items.Accessories.Souls;
using Terraria.ModLoader;

namespace FargowiltasSouls.Toggler.Content
{
    public class SoulToggles : ToggleCollection
    {
        public override string Mod => "Terraria";
        public override string SortCatagory => "Souls";
        public override int Priority => 1;

        public int SoulHeader = ModContent.ItemType<UniverseSoul>();
        public string Melee;
        public string MagmaStone;
        public string YoyoBag;
        public string Sniper;
        public string Universe;

        //public int WorldShaperSoul = ModContent.ItemType<WorldShaperSoul>();
        public string MiningHunt;
        public string MiningDanger;
        public string MiningSpelunk;
        public string MiningShine;
        public string Builder;

        //public int ColossusSoul = ModContent.ItemType<ColossusSoul>();
        public string DefenseSpore;
        public string DefenseStar;
        public string DefenseBee;
        public string DefensePanic;

        //public int SupersonicSoul = ModContent.ItemType<SupersonicSoul>();
        public string RunSpeed;
        public string Momentum;
        public string Supersonic;
        // Supersonic speed multiplier
        public string SupersonicJumps;
        public string SupersonicRocketBoots;
        public string SupersonicCarpet;
        public string CthulhuShield;

        //public int TrawlersSoul = ModContent.ItemType<TrawlerSoul>();
        public string Trawler;

        //public int SoulOfEternity = ModContent.ItemType<EternitySoul>();
        public string Eternity;
    }
}
