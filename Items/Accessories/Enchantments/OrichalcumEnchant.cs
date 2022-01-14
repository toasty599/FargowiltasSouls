using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace FargowiltasSouls.Items.Accessories.Enchantments
{
    public class OrichalcumEnchant : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Orichalcum Enchantment");
            Tooltip.SetDefault(
@"Flower petals will cause extra damage to your target and inflict Orichalcum Poison
Damaging debuffs deal 3x damage
'Nature blesses you'");
            DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "山铜魔石");
            Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese,
@"花瓣将落到被你攻击的敌人的身上以造成额外伤害和山铜中毒减益
伤害性减益造成的伤害x3
'自然祝福着你'");
        }

        public override void SafeModifyTooltips(List<TooltipLine> list)
        {
            foreach (TooltipLine tooltipLine in list)
            {
                if (tooltipLine.mod == "Terraria" && tooltipLine.Name == "ItemName")
                {
                    tooltipLine.overrideColor = new Color(235, 50, 145);
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
            Item.value = 100000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            //player.GetModPlayer<FargoSoulsPlayer>().OrichalcumEffect();
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddRecipeGroup("FargowiltasSouls:AnyOriHead")
            .AddIngredient(ItemID.OrichalcumBreastplate)
            .AddIngredient(ItemID.OrichalcumLeggings)
            //.AddIngredient(ItemID.OrichalcumWaraxe);
            //ori sword
            //flare gun
            .AddIngredient(ItemID.FlowerofFire)
            .AddIngredient(ItemID.FlowerofFrost)
            .AddIngredient(ItemID.CursedFlames)
            //flamethrower

            .AddTile(TileID.CrystalBall)
            .Register();
        }
    }
}
