using Terraria.ModLoader;

namespace FargowiltasSouls.Items.Placeables
{
    public class MutantStatueGift : MutantStatue
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            DisplayName.SetDefault("Mutant Statue (Gift)");
        }

        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.createTile = ModContent.TileType<Tiles.MutantStatueGift>();
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<MutantStatue>())
                .AddIngredient(ModContent.ItemType<Masochist>())
                .Register();
        }
    }
}