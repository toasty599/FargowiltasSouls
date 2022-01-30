using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Items.Armor
{
    [AutoloadEquip(EquipType.Body)]
    public class GaiaPlate : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Gaia Plate");
            Tooltip.SetDefault(@"10% increased damage
5% increased critical strike chance
Reduces damage taken by 10%
Increases your life regeneration");
        }

        public override void SetDefaults()
        {
            item.width = 18;
            item.height = 18;
            item.rare = ItemRarityID.Yellow;
            item.value = Item.sellPrice(0, 6);
            item.defense = 20;
        }

        public override void UpdateEquip(Player player)
        {
            player.GetModPlayer<FargoSoulsPlayer>().AllDamageUp(0.1f);
            player.GetModPlayer<FargoSoulsPlayer>().AllCritUp(5);
            player.endurance += 0.1f;
            player.lifeRegen += 2;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ItemID.BeetleHusk, 6)
            .AddIngredient(ItemID.ShroomiteBar, 9)
            .AddIngredient(ItemID.SpectreBar, 9)
            .AddIngredient(ItemID.SpookyWood, 150)
            .AddTile(TileID.LunarCraftingStation)
            
            .Register();
        }
    }
}