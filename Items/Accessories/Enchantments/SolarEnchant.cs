using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace FargowiltasSouls.Items.Accessories.Enchantments
{
    public class SolarEnchant : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Solar Enchantment");
            Tooltip.SetDefault(
@"Solar shield allows you to dash through enemies
Solar shield is not depleted on hit, but has reduced damage reduction
Attacks may inflict the Solar Flare debuff
'Too hot to handle'");
            DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "日耀魔石");
            Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, 
@"允许你使用日耀护盾进行冲刺
日耀护盾在击中敌人时不会被消耗，但会降低其伤害减免效果
攻击有几率造成耀斑减益
'烫手魔石'");
        }

        public override void SafeModifyTooltips(List<TooltipLine> list)
        {
            foreach (TooltipLine tooltipLine in list)
            {
                if (tooltipLine.mod == "Terraria" && tooltipLine.Name == "ItemName")
                {
                    tooltipLine.overrideColor = new Color(254, 158, 35);
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
            FargoSoulsPlayer modPlayer = player.GetModPlayer<FargoSoulsPlayer>();
            //solar shields
            //modPlayer.SolarEffect();
            //flare debuff
            //modPlayer.SolarEnchant = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ItemID.SolarFlareHelmet)
            .AddIngredient(ItemID.SolarFlareBreastplate)
            .AddIngredient(ItemID.SolarFlareLeggings)
            //solar wings
            .AddIngredient(ItemID.HelFire)
            //golem fist
            //xmas tree sword
            //.AddIngredient(ItemID.SolarEruption);
            .AddIngredient(ItemID.DayBreak)
            .AddIngredient(ItemID.StarWrath) //terrarian

            .AddTile(TileID.LunarCraftingStation)
            .Register();
            
        }
    }
}
