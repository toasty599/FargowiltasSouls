using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace FargowiltasSouls.Items.Accessories.Enchantments
{
    public class FossilEnchant : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Fossil Enchantment");
            Tooltip.SetDefault(
@"If you reach zero HP you will revive with 1 HP and spawn several bones
You will also spawn a few bones on every hit
Collect the bones to heal for 15 HP each
Summons a pet Baby Dino
'Beyond a forgotten age'");
            DisplayName.AddTranslation(GameCulture.Chinese, "化石魔石");
            Tooltip.AddTranslation(GameCulture.Chinese,
@"'被遗忘的记忆'
血量为0时避免死亡, 以20点生命值重生
在复活后的几秒钟内, 免疫所有伤害, 并且可以产生骨头
召唤一只小恐龙");
        }

        public override void SafeModifyTooltips(List<TooltipLine> list)
        {
            foreach (TooltipLine tooltipLine in list)
            {
                if (tooltipLine.mod == "Terraria" && tooltipLine.Name == "ItemName")
                {
                    tooltipLine.overrideColor = new Color(140, 92, 59);
                }
            }
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.accessory = true;
            ItemID.Sets.ItemNoGravity[item.type] = true;
            item.rare = ItemRarityID.Green;
            item.value = 40000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<FargoPlayer>().FossilEffect(hideVisual);
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);

            recipe.AddIngredient(ItemID.FossilHelm);
            recipe.AddIngredient(ItemID.FossilShirt);
            recipe.AddIngredient(ItemID.FossilPants);
            //fossil pick
            recipe.AddIngredient(ItemID.BoneDagger, 300);
            recipe.AddIngredient(ItemID.AmberStaff);
            //recipe.AddIngredient(ItemID.AntlionClaw);
            //orange phaseblade
            //snake charmers flute
            recipe.AddIngredient(ItemID.AmberMosquito);

            recipe.AddTile(TileID.DemonAltar);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}