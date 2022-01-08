using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace FargowiltasSouls.Items.Accessories.Enchantments
{
    public class NecroEnchant : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Necro Enchantment");
            Tooltip.SetDefault(
@"Slain enemies may drop a pile of bones
Touch a pile of bones to spawn a friendly Dungeon Guardian
Damage scales with the defeated enemy's max HP
Bosses will drop bones every 10% of their HP lost
'Welcome to the bone zone'");
            DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "死灵魔石");
            Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese,
@"击杀敌人时有几率爆出一摞骨头
拾取骨头时有几率生成一个地牢守卫
地牢守卫的伤害取决于被击杀的敌人的最大生命值
Boss每损失10%生命值便会掉落骨头
'欢迎来到骸骨领域'");
        }

        public override void SafeModifyTooltips(List<TooltipLine> list)
        {
            foreach (TooltipLine tooltipLine in list)
            {
                if (tooltipLine.mod == "Terraria" && tooltipLine.Name == "ItemName")
                {
                    tooltipLine.overrideColor = new Color(86, 86, 67);
                }
            }
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.accessory = true;
            ItemID.Sets.ItemNoGravity[Item.type] = true;
            Item.rare = ItemRarityID.Orange;
            Item.value = 50000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            //player.GetModPlayer<FargoSoulsPlayer>().NecroEffect(hideVisual);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ItemID.NecroHelmet)
            .AddIngredient(ItemID.NecroBreastplate)
            .AddIngredient(ItemID.NecroGreaves)
            .AddIngredient(ItemID.BoneGlove)
            .AddIngredient(ItemID.BookofSkulls) //spinal tap?
            //quad barrel shotgun
            //maggot
            .AddIngredient(ItemID.TheGuardiansGaze)
            //recipe.AddIngredient(ItemID.BoneKey);

            .AddTile(TileID.DemonAltar)
            .Register();
        }
    }
}
