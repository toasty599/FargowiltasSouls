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
'Beyond a forgotten age'");
            DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "化石魔石");
            Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese,
@"受到致死伤害时会以1生命值重生并爆出几根骨头
你攻击敌人时也会扔出骨头
每根骨头会回复15点生命值
'被遗忘已久的记忆'");
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
            Item.width = 20;
            Item.height = 20;
            Item.accessory = true;
            ItemID.Sets.ItemNoGravity[Item.type] = true;
            Item.rare = ItemRarityID.Green;
            Item.value = 40000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            //player.GetModPlayer<FargoSoulsPlayer>().FossilEffect(hideVisual);
        }

        public override void AddRecipes()
        {
            CreateRecipe()

            .AddIngredient(ItemID.FossilHelm)
            .AddIngredient(ItemID.FossilShirt)
            .AddIngredient(ItemID.FossilPants)
            //fossil pick
            .AddIngredient(ItemID.BoneDagger, 300)
            .AddIngredient(ItemID.AmberStaff)
            .AddIngredient(ItemID.AntlionClaw)
            //orange phaseblade
            //snake charmers flute
            //.AddIngredient(ItemID.AmberMosquito);

            .AddTile(TileID.DemonAltar)
            .Register();
        }
    }
}
