using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace FargowiltasSouls.Items.Accessories.Enchantments
{
    public class NinjaEnchant : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ninja Enchantment");
            Tooltip.SetDefault(
@"Use Ninja hotkey to throw a smoke bomb, use it again to teleport to it and gain the First Strike Buff
Using the Rod of Discord will also grant this buff
First Strike ensures your next attack is a crit dealing 3x damage
'Now you see me, now you don’t'");
            DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "忍者魔石");
            Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, 
@"按下'忍者秘技'键后会扔出一颗烟雾弹，再次按下'忍者秘技'键时会将你传送至其落点的位置并使你获得先发制人增益
使用混沌传送杖也会获得先发制人增益
先发制人增益会使你下次攻击必定暴击且造成3倍伤害
'你现在能看到我了，诶，你又看不到我了'");
        }

        public override void SafeModifyTooltips(List<TooltipLine> list)
        {
            foreach (TooltipLine tooltipLine in list)
            {
                if (tooltipLine.mod == "Terraria" && tooltipLine.Name == "ItemName")
                {
                    tooltipLine.overrideColor = new Color(48, 49, 52);
                }
            }
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.accessory = true;
            ItemID.Sets.ItemNoGravity[Item.type] = true;
            Item.rare = ItemRarityID.Green;
            Item.value = 30000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            //player.GetModPlayer<FargoSoulsPlayer>().NinjaEffect(hideVisual);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ItemID.NinjaHood)
            .AddIngredient(ItemID.NinjaShirt)
            .AddIngredient(ItemID.NinjaPants)
            //chain knife
            .AddIngredient(ItemID.Katana)
            .AddIngredient(ItemID.Shuriken, 300)
            //throwing knives
            .AddIngredient(ItemID.SmokeBomb, 50)
            //recipe.AddIngredient(ItemID.SlimeHook);

            .AddTile(TileID.DemonAltar)
            .Register();
        }
    }
}
