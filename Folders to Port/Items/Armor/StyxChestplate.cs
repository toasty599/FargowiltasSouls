using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Items.Armor
{
    [AutoloadEquip(EquipType.Body)]
    public class StyxChestplate : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Styx Chestplate");
            Tooltip.SetDefault(@"15% increased damage
10% increased critical strike chance
Reduces damage taken by 10%
Increases your life regeneration");
        }

        public override void SetDefaults()
        {
            item.width = 18;
            item.height = 18;
            item.rare = ItemRarityID.Purple;
            item.value = Item.sellPrice(0, 25);
            item.defense = 35;
        }

        public override void UpdateEquip(Player player)
        {
            player.GetModPlayer<FargoSoulsPlayer>().AllDamageUp(0.15f);
            player.GetModPlayer<FargoSoulsPlayer>().AllCritUp(10);
            player.endurance += 0.1f;
            player.lifeRegen += 4;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ItemID.SoulofMight, 15);
            .AddIngredient(ItemID.LunarBar, 5);
            .AddIngredient(ModContent.ItemType<Misc.AbomEnergy>(), 10);
            recipe.AddTile(ModLoader.GetMod("Fargowiltas").TileType("CrucibleCosmosSheet"));
            recipe.SetResult(this);
            .Register();
        }
    }
}