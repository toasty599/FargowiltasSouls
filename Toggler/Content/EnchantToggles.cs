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

        public int TerraHeader = ModContent.ItemType<TerraForce>();
        public string Copper;
        public string IronS;
        public string IronM;
        public string CthulhuShield;
        public string Tin;
        public string Tungsten;
        public string TungstenProj;
        public string Obsidian;

        public int WillHeader = ModContent.ItemType<WillForce>();
        public bool Gladiator;
        public bool Gold;
        public bool Huntress;
        public bool Valhalla;
        public bool SquirePanic;

        public int LifeHeader = ModContent.ItemType<LifeForce>();
        public bool Bee;
        public bool Beetle;
        public bool Cactus;
        public bool Pumpkin;
        public bool Spider;
        public bool Turtle;

        public int NatureHeader = ModContent.ItemType<NatureForce>();
        public bool Chlorophyte;
        public bool ChlorophyteFlower;
        public bool Crimson;
        public bool Frost;
        public bool Snow;
        public bool Jungle;
        public bool Cordage;
        public bool Molten;
        public bool MoltenE;
        public bool Rain;
        public bool Shroomite;
        public bool ShroomiteShroom;

        public int ShadowHeader = ModContent.ItemType<ShadowForce>();
        public bool DarkArtist;
        public bool Apprentice;
        public bool Necro;
        public bool AncientShadow;
        public bool Shadow;
        public bool Monk;
        public bool Shinobi;
        public bool ShinobiTabi;
        public bool ShinobiClimbing;
        public bool Spooky;

        public int SpiritHeader = ModContent.ItemType<SpiritForce>();
        public bool Forbidden;
        public bool Hallowed;
        public bool HallowS;
        public bool Silver;
        public bool Spectre;
        public bool Tiki;

        public int CosmoHeader = ModContent.ItemType<CosmoForce>();
        public bool Meteor;
        public bool Nebula;
        public bool Solar;
        public bool SolarFlare;
        public bool Stardust;
        public bool VortexS;
        public bool VortexV;
    }
}
