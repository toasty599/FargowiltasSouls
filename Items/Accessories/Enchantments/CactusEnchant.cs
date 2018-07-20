using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Items.Accessories.Enchantments
{
	public class CactusEnchant : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Cactus Enchantment");
			Tooltip.SetDefault(
@"'' 
50% of contact damage is reflected
Getting hit by a projectile causes a needle spray
Needles scale with melee damage");
		}

		public override void SetDefaults()
		{
			item.width = 20;
			item.height = 20;
			item.accessory = true;			
			ItemID.Sets.ItemNoGravity[item.type] = true;
			item.rare = 0; 
			item.value = 10000;
		}
		
		public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<FargoPlayer>(mod).CactusEnchant = true;
            player.thorns = .5f;
        }
		
		public override void AddRecipes()
		{
            ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.CactusHelmet);
			recipe.AddIngredient(ItemID.CactusBreastplate);
			recipe.AddIngredient(ItemID.CactusLeggings);
			recipe.AddIngredient(ItemID.CactusSword);
			recipe.AddIngredient(ItemID.PinkPricklyPear);
			recipe.AddTile(TileID.DemonAltar);
            recipe.SetResult(this);
            recipe.AddRecipe();
		}
	}
}
		
