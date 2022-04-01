using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Items.Dyes
{
    public class LifeDye : SoulsItem
    {
        //public override string Texture => "FargowiltasSouls/Items/Dyes/LifeDye";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Heavenly Dye");
        }

        public override void SetDefaults()
        {
            Item.maxStack = 99;
            Item.rare = ItemRarityID.Orange;
            Item.width = 20;
            Item.height = 20;
            Item.value = Item.sellPrice(0, 2, 50);
        }
    }

    public class WillDye : SoulsItem
    {
        //public override string Texture => "FargowiltasSouls/Items/Dyes/LifeDye";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Willpower Dye");
        }

        public override void SetDefaults()
        {
            Item.maxStack = 99;
            Item.rare = ItemRarityID.Orange;
            Item.width = 20;
            Item.height = 20;
            Item.value = Item.sellPrice(0, 2, 50);
        }
    }

    public class GaiaDye : SoulsItem
    {
        //public override string Texture => "FargowiltasSouls/Items/Dyes/LifeDye";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Gaia Dye");
        }

        public override void SetDefaults()
        {
            Item.maxStack = 99;
            Item.rare = ItemRarityID.Orange;
            Item.width = 20;
            Item.height = 20;
            Item.value = Item.sellPrice(0, 2, 50);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ItemID.BottledWater)
            .AddIngredient(ItemID.BeetleHusk)
            .AddIngredient(ItemID.ShroomiteBar)
            .AddIngredient(ItemID.SpectreBar)
            .AddIngredient(ItemID.SpookyWood)
            .AddTile(TileID.DyeVat)
            
            .Register();
        }
    }
}