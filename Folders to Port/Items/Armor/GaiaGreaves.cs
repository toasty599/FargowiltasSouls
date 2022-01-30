using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Items.Armor
{
    [AutoloadEquip(EquipType.Legs)]
    public class GaiaGreaves : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Gaia Greaves");
            Tooltip.SetDefault(@"10% increased damage
5% increased critical strike chance
10% increased movement speed");
        }

        public override void SetDefaults()
        {
            item.width = 18;
            item.height = 18;
            item.rare = ItemRarityID.Yellow;
            item.value = Item.sellPrice(0, 5);
            item.defense = 15;
        }

        public override void UpdateEquip(Player player)
        {
            player.GetModPlayer<FargoSoulsPlayer>().AllDamageUp(0.1f);
            player.GetModPlayer<FargoSoulsPlayer>().AllCritUp(5);
            player.moveSpeed += 0.1f;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ItemID.BeetleHusk, 3)
            .AddIngredient(ItemID.ShroomiteBar, 6)
            .AddIngredient(ItemID.SpectreBar, 6)
            .AddIngredient(ItemID.SpookyWood, 100)
            .AddTile(TileID.LunarCraftingStation)
            
            .Register();
        }
    }
}