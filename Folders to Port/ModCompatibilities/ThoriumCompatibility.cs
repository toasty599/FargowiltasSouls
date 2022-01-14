using Terraria;
using Terraria.ModLoader;
using ThoriumMod.Items.MeleeItems;
using ThoriumMod.Tiles;

namespace FargowiltasSouls.ModCompatibilities
{
    public sealed class ThoriumCompatibility : ModCompatibility
    {
        public ThoriumCompatibility(Mod callerMod) : base(callerMod, nameof(ThoriumMod))
        {
        }


        protected override void AddRecipes()
        {
            int 
                //foldedMetal = ModContent.ItemType<FoldedMetal>(),
                arcaneArmorFabricator = ModContent.TileType<ArcaneArmorFabricator>();


            /*ModRecipe recipe = new ModRecipe(CallerMod);

            .AddIngredient(foldedMetal);
            recipe.AddTile(arcaneArmorFabricator);
            
            recipe.SetResult(ModContent.ItemType<SteelArrow>());
            .Register();



            recipe = new ModRecipe(CallerMod);

            .AddIngredient(foldedMetal);
            recipe.AddTile(arcaneArmorFabricator);

            recipe.SetResult(ModContent.ItemType<SteelAxe>());
            .Register();



            recipe = new ModRecipe(CallerMod);

            .AddIngredient(foldedMetal);
            recipe.AddTile(arcaneArmorFabricator);

            recipe.SetResult(ModContent.ItemType<SteelBattleAxe>(), 10);
            .Register();



            recipe = new ModRecipe(CallerMod);

            .AddIngredient(foldedMetal);
            recipe.AddTile(arcaneArmorFabricator);

            recipe.SetResult(ModContent.ItemType<SteelBlade>());
            .Register();



            recipe = new ModRecipe(CallerMod);

            .AddIngredient(foldedMetal);
            recipe.AddTile(arcaneArmorFabricator);

            recipe.SetResult(ModContent.ItemType<SteelBow>());
            .Register();



            recipe = new ModRecipe(CallerMod);

            .AddIngredient(foldedMetal);
            recipe.AddTile(arcaneArmorFabricator);

            recipe.SetResult(ModContent.ItemType<SteelChestplate>());
            .Register();



            recipe = new ModRecipe(CallerMod);

            .AddIngredient(foldedMetal);
            recipe.AddTile(arcaneArmorFabricator);

            recipe.SetResult(ModContent.ItemType<SteelGreaves>());
            .Register();



            recipe = new ModRecipe(CallerMod);

            .AddIngredient(foldedMetal);
            recipe.AddTile(arcaneArmorFabricator);

            recipe.SetResult(ModContent.ItemType<SteelHelmet>());
            .Register();



            recipe = new ModRecipe(CallerMod);

            .AddIngredient(foldedMetal);
            recipe.AddTile(arcaneArmorFabricator);

            recipe.SetResult(ModContent.ItemType<SteelMallet>());
            .Register();



            recipe = new ModRecipe(CallerMod);

            .AddIngredient(foldedMetal);
            recipe.AddTile(arcaneArmorFabricator);

            recipe.SetResult(ModContent.ItemType<SteelPickaxe>());
            .Register();*/
        }

        protected override void AddRecipeGroups()
        {
            RecipeGroup group = new RecipeGroup(() => Lang.misc[37] + " Combination Yoyo", ModContent.ItemType<Nocturnal>(), ModContent.ItemType<Sanguine>());
            RecipeGroup.RegisterGroup("FargowiltasSouls:AnyThoriumYoyo", group);
        }
    }
}