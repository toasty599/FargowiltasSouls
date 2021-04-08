using FargowiltasSouls.Toggler;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Items.Accessories.Enchantments
{
    public class WizardEnchant : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Wizard Enchantment");
            Tooltip.SetDefault(
@"Enhances the power of all other equipped Enchantments
Summons a pet Black Cat
'I'm a what?'");
        }

        public override void SafeModifyTooltips(List<TooltipLine> list)
        {
            foreach (TooltipLine tooltipLine in list)
            {
                if (tooltipLine.mod == "Terraria" && tooltipLine.Name == "ItemName")
                {
                    tooltipLine.overrideColor = new Color(50, 80, 193);
                }
            }
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.accessory = true;
            ItemID.Sets.ItemNoGravity[item.type] = true;
            item.rare = ItemRarityID.LightRed;
            item.value = 100000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            //player.GetModPlayer<FargoPlayer>().WizardEnchant = true;
            player.GetModPlayer<FargoPlayer>().AddPet(player.GetToggleValue("PetBlackCat"), hideVisual, BuffID.BlackCat, ProjectileID.BlackCat);
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);

            recipe.AddIngredient(ItemID.WizardHat);
            //recipe.AddIngredient(ItemID.AmethystRobe);
            //recipe.AddIngredient(ItemID.TopazRobe);
            //recipe.AddIngredient(ItemID.EmeraldRobe);
            recipe.AddIngredient(ItemID.SapphireRobe);
            recipe.AddIngredient(ItemID.RubyRobe);
            recipe.AddIngredient(ItemID.DiamondRobe);
            //amber robe
            recipe.AddIngredient(ItemID.IceRod);
            recipe.AddIngredient(ItemID.RareEnchantment);
            recipe.AddIngredient(ItemID.UnluckyYarn);

            recipe.AddTile(TileID.CrystalBall);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}