using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.ModPlayers;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Accessories.Forces
{
	[AutoloadEquip(EquipType.Shield)]
    public class TerraForce : BaseForce
    {
        public override int[] Enchants => new int[]
        {
            ModContent.ItemType<CopperEnchant>(),
            ModContent.ItemType<TinEnchant>(),
            ModContent.ItemType<IronEnchant>(),
            ModContent.ItemType<LeadEnchant>(),
            ModContent.ItemType<SilverEnchant>(),
            ModContent.ItemType<TungstenEnchant>(),
            ModContent.ItemType<ObsidianEnchant>()
        };

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }

        public override void UpdateInventory(Player player)
        {
            AshWoodEnchant.PassiveEffect(player);
            IronEnchant.AddEffects(player, Item);
        }
        public override void UpdateVanity(Player player)
        {
            AshWoodEnchant.PassiveEffect(player);
            IronEnchant.AddEffects(player, Item);
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            FargoSoulsPlayer modPlayer = player.FargoSouls();
            SetActive(player);
            player.AddEffect<CopperEffect>(Item);
            player.AddEffect<TinEffect>(Item);
            IronEnchant.AddEffects(player, Item);
            player.AddEffect<LeadEffect>(Item);
            player.AddEffect<SilverEffect>(Item);
            player.AddEffect<TungstenEffect>(Item);
            player.FargoSouls().TungstenEnchantItem = Item;
            ObsidianEnchant.AddEffects(player, Item);
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            foreach (int ench in Enchants)
                recipe.AddIngredient(ench);
            recipe.AddTile(ModContent.Find<ModTile>("Fargowiltas", "CrucibleCosmosSheet"));
            recipe.Register();
        }
    }
}
