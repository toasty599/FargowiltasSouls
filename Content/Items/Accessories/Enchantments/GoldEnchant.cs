using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace FargowiltasSouls.Content.Items.Accessories.Enchantments
{
	public class GoldEnchant : BaseEnchant
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
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
            FargoSoulsPlayer modPlayer = player.FargoSouls();
            modPlayer.GoldEffect(hideVisual);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ItemID.GoldHelmet)
            .AddIngredient(ItemID.GoldChainmail)
            .AddIngredient(ItemID.GoldGreaves)
            .AddIngredient(ItemID.GoldCrown)
            .AddIngredient(ItemID.GoldBunny)
            .AddIngredient(ItemID.SquirrelGold)
            .AddIngredient(ItemID.GoldGoldfish)

            .AddTile(TileID.DemonAltar)
            .Register();
        }
    }
}
