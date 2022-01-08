using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using System.Collections.Generic;
using FargowiltasSouls.Toggler;
//using FargowiltasSouls.Projectiles.Minions;

namespace FargowiltasSouls.Items.Accessories.Enchantments
{
    public class SilverEnchant : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Silver Enchantment");
            
            DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "银魔石");
            
            string tooltip =
@"Summons a sword familiar that scales with minion damage
Drastically increases minion speed
Certain minion attacks do reduced damage to compensate for increased speed
'Have you power enough to wield me?'";
            Tooltip.SetDefault(tooltip);

            string tooltip_ch =
@"召唤一柄剑，剑的伤害取决于你的召唤伤害
'你有足够的力量驾驭我吗？'";
            Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, tooltip_ch);

        }

        public override void SafeModifyTooltips(List<TooltipLine> list)
        {
            foreach (TooltipLine tooltipLine in list)
            {
                if (tooltipLine.mod == "Terraria" && tooltipLine.Name == "ItemName")
                {
                    tooltipLine.overrideColor = new Color(180, 180, 204);
                }
            }
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.accessory = true;
            ItemID.Sets.ItemNoGravity[Item.type] = true;
            Item.rare = ItemRarityID.Blue;
            Item.value = 30000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            FargoSoulsPlayer modPlayer = player.GetModPlayer<FargoSoulsPlayer>();
            //modPlayer.SilverEnchant = true;
            //modPlayer.AddMinion(player.GetToggleValue("Silver"), ModContent.ProjectileType<SilverSword>(), (int)(20 * player.minionDamage), 0f);
        }

        public override void AddRecipes()
        {
            CreateRecipe()

            .AddIngredient(ItemID.SilverHelmet)
            .AddIngredient(ItemID.SilverChainmail)
            .AddIngredient(ItemID.SilverGreaves)
            .AddIngredient(ItemID.SilverBroadsword)
            //recipe.AddIngredient(ItemID.SilverBow);
            .AddIngredient(ItemID.SapphireStaff)
            .AddIngredient(ItemID.BluePhaseblade)
            //leather whip
            //recipe.AddIngredient(ItemID.TreeNymphButterfly);
            //roasted duck

            .AddTile(TileID.DemonAltar)
            .Register();
        }
    }
}
