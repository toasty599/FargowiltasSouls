using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Accessories.Forces
{
	public class WillForce : BaseForce
    {
        public override void SetStaticDefaults()
        {
            Enchants[Type] = new int[]
            {
                ModContent.ItemType<GoldEnchant>(),
                ModContent.ItemType<PlatinumEnchant>(),
                ModContent.ItemType<GladiatorEnchant>(),
                ModContent.ItemType<RedRidingEnchant>(),
                ModContent.ItemType<ValhallaKnightEnchant>()
            };
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            SetActive(player);
            FargoSoulsPlayer modPlayer = player.FargoSouls();
            player.AddEffect<GoldEffect>(Item);
            player.AddEffect<GoldToPiggy>(Item);
            modPlayer.PlatinumEffectActive = true;
            player.AddEffect<GladiatorBanner>(Item);
            player.AddEffect<GladiatorSpears>(Item);
            player.AddEffect<RedRidingEffect>(Item);
            player.AddEffect<HuntressEffect>(Item);
            modPlayer.ValhallaEnchantActive = true;
            SquireEnchant.SquireEffect(player, Item);
            
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            foreach (int ench in Enchants[Type])
                recipe.AddIngredient(ench);
            recipe.AddTile(ModContent.Find<ModTile>("Fargowiltas", "CrucibleCosmosSheet"));
            recipe.Register();
        }
    }
}
