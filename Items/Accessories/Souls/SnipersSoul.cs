using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using FargowiltasSouls.Toggler;

namespace FargowiltasSouls.Items.Accessories.Souls
{
    //[AutoloadEquip(EquipType.Neck)]
    public class SnipersSoul : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sniper's Soul");

            string tooltip =
@"30% increased ranged damage
20% chance to not consume ammo
15% increased ranged critical chance
Effects of Sniper Scope
'Ready, aim, fire'";
            string tooltip_ch =
@"'准备,瞄准,开火'
增加30%远程伤害
增加20%开火速度
增加15%远程暴击率
";

            Tooltip.SetDefault(tooltip);
            DisplayName.AddTranslation(GameCulture.Chinese, "神枪手之魂");
            Tooltip.AddTranslation(GameCulture.Chinese, tooltip_ch);
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.accessory = true;
            item.value = 1000000;
            item.rare = ItemRarityID.Purple;
            ItemID.Sets.ItemNoGravity[item.type] = true;
        }
        public override Color? GetAlpha(Color lightColor) => Color.White;
        public override void SafeModifyTooltips(List<TooltipLine> list)
        {
            foreach (TooltipLine tooltipLine in list)
            {
                if (tooltipLine.mod == "Terraria" && tooltipLine.Name == "ItemName")
                {
                    tooltipLine.overrideColor = new Color?(new Color(188, 253, 68));
                }
            }
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            //attack speed
            player.GetModPlayer<FargoPlayer>().RangedSoul = true;
            player.rangedDamage += .3f;
            player.rangedCrit += 15;

            //add new effects

            if (player.GetToggleValue("Sniper"))
            {
                player.scope = true;
            }
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);

            recipe.AddIngredient(null, "SharpshootersEssence");
            recipe.AddIngredient(ItemID.MagicQuiver); //molten quiver
            recipe.AddIngredient(ItemID.SniperScope); //recon scope

            recipe.AddIngredient(ItemID.DartPistol);
            recipe.AddIngredient(ItemID.Megashark);

            recipe.AddIngredient(ItemID.PulseBow);
            recipe.AddIngredient(ItemID.NailGun);
            recipe.AddIngredient(ItemID.PiranhaGun);
            recipe.AddIngredient(ItemID.SniperRifle);
            recipe.AddIngredient(ItemID.Tsunami);
            recipe.AddIngredient(ItemID.StakeLauncher);
            recipe.AddIngredient(ItemID.EldMelter);
            recipe.AddIngredient(ItemID.Xenopopper);
            recipe.AddIngredient(ItemID.FireworksLauncher); //celebration mk 2

            recipe.AddTile(ModLoader.GetMod("Fargowiltas").TileType("CrucibleCosmosSheet"));

            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}