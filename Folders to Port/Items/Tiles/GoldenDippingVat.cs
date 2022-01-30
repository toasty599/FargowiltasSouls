using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;

namespace FargowiltasSouls.Items.Tiles
{
    public class GoldenDippingVat : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Golden Dipping Vat");
            Tooltip.SetDefault("Used to craft Gold Critters");
            DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "黄金浸渍缸");
            Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, "用来制作黄金动物");
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.maxStack = 99;
            item.useTurn = true;
            item.autoReuse = true;
            item.rare = ItemRarityID.Yellow;
            item.value = Item.sellPrice(0, 10);
            item.useAnimation = 15;
            item.useTime = 15;
            item.useStyle = ItemUseStyleID.Swing;
            item.consumable = true;
            item.createTile = ModContent.TileType<GoldenDippingVatSheet>();
        }

        public override void AddRecipes()
        {
            AddCritter(ItemID.Bird, ItemID.GoldBird);
            AddCritter(ItemID.Bunny, ItemID.GoldBunny);
            AddCritter(ItemID.Frog, ItemID.GoldFrog);
            AddCritter(ItemID.Grasshopper, ItemID.GoldGrasshopper);
            AddCritter(ItemID.Mouse, ItemID.GoldMouse);
            //AddCritter(ItemID.Squirrel, ItemID.SquirrelGold);
            AddCritter(ItemID.Worm, ItemID.GoldWorm);

            CreateRecipe()
            recipe.AddRecipeGroup("FargowiltasSouls:AnyButterfly");
            .AddIngredient(ItemID.GoldDust, 100)
            .AddTile(ModContent.TileType<GoldenDippingVatSheet>())
            recipe.SetResult(ItemID.GoldButterfly);
            .Register();

            recipe = new ModRecipe(mod);
            recipe.AddRecipeGroup("FargowiltasSouls:AnySquirrel");
            .AddIngredient(ItemID.GoldDust, 100)
            .AddTile(ModContent.TileType<GoldenDippingVatSheet>())
            recipe.SetResult(ItemID.SquirrelGold);
            .Register();

            recipe = new ModRecipe(mod);
            recipe.AddRecipeGroup("FargowiltasSouls:AnyCommonFish");
            .AddIngredient(ItemID.GoldDust, 100)
            .AddTile(ModContent.TileType<GoldenDippingVatSheet>())
            recipe.SetResult(ItemID.GoldenCarp);
            .Register();
        }

        private void AddCritter(int critterID, int goldCritterID)
        {
            CreateRecipe()
            .AddIngredient(critterID)
            .AddIngredient(ItemID.GoldDust, 100)
            .AddTile(ModContent.TileType<GoldenDippingVatSheet>())
            recipe.SetResult(goldCritterID);
            .Register();
        }
    }
}