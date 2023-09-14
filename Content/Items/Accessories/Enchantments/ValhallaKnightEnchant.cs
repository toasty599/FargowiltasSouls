using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace FargowiltasSouls.Content.Items.Accessories.Enchantments
{
	public class ValhallaKnightEnchant : BaseEnchant
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }

        protected override Color nameColor => new(147, 101, 30);
        
        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.rare = ItemRarityID.Yellow;
            Item.value = 250000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<FargoSoulsPlayer>().ValhallaEffect(hideVisual);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ItemID.SquireAltHead)
            .AddIngredient(ItemID.SquireAltShirt)
            .AddIngredient(ItemID.SquireAltPants)
            .AddIngredient(ItemID.VikingHelmet)
            .AddIngredient(null, "SquireEnchant")
            //.AddIngredient(ItemID.ShinyStone);
            //starlight
            //shadow lance
            .AddIngredient(ItemID.DD2SquireBetsySword)

            .AddTile(TileID.CrystalBall)
            .Register();
        }
    }
}
