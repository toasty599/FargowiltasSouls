using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace FargowiltasSouls.Content.Items.Accessories.Enchantments
{
	public class ForbiddenEnchant : BaseEnchant
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            // DisplayName.SetDefault("Forbidden Enchantment");

            // Tooltip.SetDefault(tooltip);
        }

        protected override Color nameColor => new(231, 178, 28);
        

        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.rare = ItemRarityID.Pink;
            Item.value = 150000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.FargoSouls().ForbiddenEffect();
        }

        public static void ActivateForbiddenStorm(Player player)
        {
            FargoSoulsPlayer modPlayer = player.FargoSouls();
            if (modPlayer.ForbiddenEnchantActive)
            {
                if (modPlayer.CanSummonForbiddenStorm)
                {
                    modPlayer.CommandForbiddenStorm();
                    modPlayer.CanSummonForbiddenStorm = false;
                }
            }
        }
        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ItemID.AncientBattleArmorHat)
            .AddIngredient(ItemID.AncientBattleArmorShirt)
            .AddIngredient(ItemID.AncientBattleArmorPants)
            //sun mask/moon mask
            .AddIngredient(ItemID.DjinnsCurse)
            .AddIngredient(ItemID.SpiritFlame)
            .AddIngredient(ItemID.SkyFracture)
            //sky fracture
            //.AddIngredient(ItemID.RainbowRod);

            //recipe.AddRecipeGroup("FargowiltasSouls:AnyScorpion");
            //fennec fox pet

            .AddTile(TileID.CrystalBall)
            .Register();
        }
    }
}
