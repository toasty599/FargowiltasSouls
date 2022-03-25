using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;

namespace FargowiltasSouls.Items.Misc
{
    public class ProofOfMastery : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Proof of Mastery");
            Tooltip.SetDefault(@"Permanently increases the number of accessory slots
Only usable after Demon Heart");
            DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "天篆");
            Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese,
@"永久增加饰品栏
只能在使用恶魔之心之后使用");
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.rare = ItemRarityID.Purple;
            Item.maxStack = 99;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.useAnimation = 30;
            Item.useTime = 30;
            Item.consumable = true;
            Item.UseSound = SoundID.Item123;
            Item.value = Item.sellPrice(0, 15);
        }

        public override bool CanUseItem(Player player)
        {
            return !player.GetModPlayer<FargoSoulsPlayer>().ProofOfMastery;
        }

        public override bool? UseItem(Player player)
        {
            if (player.itemAnimation > 0 && player.itemTime == 0)
            {
                player.GetModPlayer<FargoSoulsPlayer>().ProofOfMastery = true;
            }
            return true;
        }

        public override void SafeModifyTooltips(List<TooltipLine> list)
        {
            foreach (TooltipLine line2 in list)
            {
                if (line2.mod == "Terraria" && line2.Name == "ItemName")
                {
                    line2.overrideColor = new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB);
                }
            }
        }

        /*public override void AddRecipes()
        {
            CreateRecipe()

            .AddIngredient(ModContent.\1Type<\2>\(\), 15)
            .AddIngredient(ItemID.LunarBar, 10)
            .AddIngredient(ItemID.FragmentNebula, 25)
            .AddIngredient(ItemID.FragmentSolar, 25)
            .AddIngredient(ItemID.FragmentStardust, 25)
            .AddIngredient(ItemID.FragmentVortex, 25)

            .AddTile(mod, "CrucibleCosmosSheet")

            
            .Register();
        }*/
    }
}