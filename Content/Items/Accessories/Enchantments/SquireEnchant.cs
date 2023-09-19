using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace FargowiltasSouls.Content.Items.Accessories.Enchantments
{
    public class SquireEnchant : BaseEnchant
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

           
        }

        protected override Color nameColor => new(148, 143, 140);

        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.rare = ItemRarityID.Yellow;
            Item.value = 150000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.DisplayToggle("SquirePanic");
            player.GetModPlayer<FargoSoulsPlayer>().SquireEnchantActive = true;
            if (!player.GetToggleValue("SquirePanic"))
                player.buffImmune[BuffID.BallistaPanic] = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ItemID.SquireGreatHelm)
            .AddIngredient(ItemID.SquirePlating)
            .AddIngredient(ItemID.SquireGreaves)
            //.AddIngredient(ItemID.SquireShield);
            .AddIngredient(ItemID.DD2BallistraTowerT2Popper)
            //rally
            //lance
            .AddIngredient(ItemID.RedPhasesaber)
            .AddIngredient(ItemID.DD2SquireDemonSword)
            //light discs

            .AddTile(TileID.CrystalBall)
            .Register();
        }
    }
}
