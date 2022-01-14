using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Items.Tiles
{
    public class MutantStatue : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Mutant Statue");
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.maxStack = 99;
            item.useTurn = true;
            item.autoReuse = true;
            item.rare = ItemRarityID.Blue;
            item.useAnimation = 15;
            item.useTime = 15;
            item.useStyle = ItemUseStyleID.Swing;
            item.consumable = true;
            item.createTile = mod.TileType("MutantStatue");
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ItemID.StoneBlock, 50);
            .AddIngredient(ModLoader.GetMod("Fargowiltas").ItemType("Mutant"));

            recipe.AddTile(TileID.HeavyWorkBench);
            recipe.SetResult(this);
            .Register();
        }
    }
}