using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using FargowiltasSouls.Toggler;

namespace FargowiltasSouls.Items.Accessories.Souls
{
    [AutoloadEquip(EquipType.Wings)]
    public class FlightMasterySoul : FlightMasteryWings
    {
        protected override bool HasSupersonicSpeed => false;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Flight Mastery Soul");
            Tooltip.SetDefault(
@"Allows for infinite flight
Hold DOWN and JUMP to hover
Increases gravity
Allows the control of gravity
'Ascend'");
            DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "飞行大师之魂");
            Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, 
@"使你获得无限飞行能力
按住'下'和'跳跃'键悬停
允许你控制重力
'飞升'");

            ItemID.Sets.ItemNoGravity[Item.type] = true;
            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.accessory = true;
            Item.value = 1000000;
            Item.rare = ItemRarityID.Purple;
        }
        public override Color? GetAlpha(Color lightColor) => Color.White;
        public override void SafeModifyTooltips(List<TooltipLine> list)
        {
            foreach (TooltipLine tooltipLine in list)
            {
                if (tooltipLine.mod == "Terraria" && tooltipLine.Name == "ItemName")
                {
                    tooltipLine.overrideColor = new Color?(new Color(56, 134, 255));
                }
            }
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            FargoSoulsPlayer modPlayer = player.GetModPlayer<FargoSoulsPlayer>();
            modPlayer.FlightMasterySoul();
        }

        public override void AddRecipes()
        {
            CreateRecipe()

            .AddIngredient(ItemID.EmpressFlightBooster) //soaring insignia

            .AddIngredient(ItemID.BatWings) //bat wings
            .AddIngredient(ItemID.CreativeWings) //fledgling wings
            .AddIngredient(ItemID.FairyWings)
            .AddIngredient(ItemID.HarpyWings)
            .AddIngredient(ItemID.BoneWings)
            .AddIngredient(ItemID.FrozenWings)
            .AddIngredient(ItemID.FlameWings)
            .AddIngredient(ItemID.TatteredFairyWings)
            .AddIngredient(ItemID.FestiveWings)
            .AddIngredient(ItemID.BetsyWings)
            .AddIngredient(ItemID.FishronWings)
            .AddIngredient(ItemID.RainbowWings) //empress wings
            .AddIngredient(ItemID.LongRainbowTrailWings) //celestial starboard

            .AddIngredient(ItemID.GravityGlobe)

            .AddTile(ModContent.Find<ModTile>("Fargowiltas", "CrucibleCosmosSheet"))

            
            .Register();
        }
    }
}
