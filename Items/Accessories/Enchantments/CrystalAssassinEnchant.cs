using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria.Localization;

namespace FargowiltasSouls.Items.Accessories.Enchantments
{
    public class CrystalAssassinEnchant : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Crystal Assassin Enchantment");
            
            DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "水晶刺客魔石");
            
            string tooltip =
@"Effects of Volatile Gel
''";
            Tooltip.SetDefault(tooltip);
            string tooltip_ch =
@"拥有挥发明胶效果
''";
            Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, tooltip_ch);
        }

        public override void SafeModifyTooltips(List<TooltipLine> list)
        {
            foreach (TooltipLine tooltipLine in list)
            {
                if (tooltipLine.mod == "Terraria" && tooltipLine.Name == "ItemName")
                {
                    tooltipLine.overrideColor = new Color(231, 178, 28); //change e
                }
            }
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.accessory = true;
            ItemID.Sets.ItemNoGravity[Item.type] = true;
            Item.rare = ItemRarityID.Pink;
            Item.value = 150000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            //player.GetModPlayer<FargoSoulsPlayer>().ForbiddenEffect(); //effect tele on party girl bathwater, when tele slashes through enemies
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ItemID.AncientBattleArmorHat)//head
            .AddIngredient(ItemID.AncientBattleArmorShirt) //body
            .AddIngredient(ItemID.AncientBattleArmorPants) //legs
            //ninja enchant
            //volatile gel
            //magic dagger
            //flying knife
            //party gitl bathwater
            //hook of dissonance
            //qs mount

            .AddTile(TileID.CrystalBall)
            .Register();
        }
    }
}
