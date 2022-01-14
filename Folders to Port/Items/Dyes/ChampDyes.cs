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
            item.maxStack = 99;
            item.rare = ItemRarityID.Orange;
            item.width = 20;
            item.height = 20;
            item.value = Item.sellPrice(0, 2, 50);
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
            item.maxStack = 99;
            item.rare = ItemRarityID.Orange;
            item.width = 20;
            item.height = 20;
            item.value = Item.sellPrice(0, 2, 50);
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
            item.maxStack = 99;
            item.rare = ItemRarityID.Orange;
            item.width = 20;
            item.height = 20;
            item.value = Item.sellPrice(0, 2, 50);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ItemID.BottledWater);
            .AddIngredient(ItemID.BeetleHusk);
            .AddIngredient(ItemID.ShroomiteBar);
            .AddIngredient(ItemID.SpectreBar);
            .AddIngredient(ItemID.SpookyWood);
            recipe.AddTile(TileID.DyeVat);
            recipe.SetResult(this);
            .Register();
        }
    }
}