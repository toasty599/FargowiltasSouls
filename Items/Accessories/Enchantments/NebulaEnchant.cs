using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace FargowiltasSouls.Items.Accessories.Enchantments
{
    public class NebulaEnchant : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Nebula Enchantment");
            Tooltip.SetDefault(
@"Hurting enemies has a chance to spawn buff boosters
Buff booster stacking capped at 2
'The pillars of creation have shined upon you'");
            DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "星云魔石");
            Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, 
@"伤害敌人时有几率生成强化增益
强化增益最大堆叠上限为2
'创生之柱照耀着你'");
        }

        public override void SafeModifyTooltips(List<TooltipLine> list)
        {
            foreach (TooltipLine tooltipLine in list)
            {
                if (tooltipLine.mod == "Terraria" && tooltipLine.Name == "ItemName")
                {
                    tooltipLine.overrideColor = new Color(254, 126, 229);
                }
            }
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.accessory = true;
            ItemID.Sets.ItemNoGravity[Item.type] = true;
            Item.rare = ItemRarityID.Red;
            Item.value = 400000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<FargoSoulsPlayer>().NebulaEffect();
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ItemID.NebulaHelmet)
            .AddIngredient(ItemID.NebulaBreastplate)
            .AddIngredient(ItemID.NebulaLeggings)
            //.AddIngredient(ItemID.WingsNebula);
            .AddIngredient(ItemID.NebulaArcanum)
            .AddIngredient(ItemID.NebulaBlaze)
            //LeafBlower
            //bubble gun
            //chaarged blaster cannon
            .AddIngredient(ItemID.LunarFlareBook)

            .AddTile(TileID.LunarCraftingStation)
            .Register();
        }
    }
}
