using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;

namespace FargowiltasSouls.Items.Accessories.Souls
{
    //[AutoloadEquip(EquipType.Back)]
    public class TrawlerSoul : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Trawler Soul");
            
            DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "捕鱼之魂");
            
            string tooltip =
@"Increases fishing skill substantially
All fishing rods will have 10 extra lures
You catch fish almost instantly
Permanent Sonar and Crate Buffs
Effects of Angler Tackle Bag and Spore Sac 
Effects of Pink Horseshoe Balloon and Arctic Diving Gear
'The fish catch themselves'";
            Tooltip.SetDefault(tooltip);

            string tooltip_ch =
@"大幅增加渔力
钓竿会额外扔出10根鱼线
你几乎能立刻就钓到鱼
拥有声呐和宝匣效果
拥有渔夫渔具袋和狍子囊效果
拥有粉马掌气球和北极潜水装备效果
'愿者上钩'";
            Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, tooltip_ch);

            ItemID.Sets.ItemNoGravity[Item.type] = true;
            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.accessory = true;
            Item.value = 750000;
            Item.rare = ItemRarityID.Purple;
        }
        public override Color? GetAlpha(Color lightColor) => Color.White;
        public override void SafeModifyTooltips(List<TooltipLine> list)
        {
            foreach (TooltipLine tooltipLine in list)
            {
                if (tooltipLine.mod == "Terraria" && tooltipLine.Name == "ItemName")
                {
                    tooltipLine.overrideColor = new Color?(new Color(0, 238, 125));
                }
            }
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            FargoSoulsPlayer modPlayer = player.GetModPlayer<FargoSoulsPlayer>();
            modPlayer.TrawlerSoul(Item, hideVisual);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(null, "AnglerEnchantment")
            //inner tube
            .AddIngredient(ItemID.BalloonHorseshoeSharkron)
            .AddIngredient(ItemID.ArcticDivingGear)
            //frog gear
            //volatile gel
            .AddIngredient(ItemID.SporeSac)

            //engineer rod
            .AddIngredient(ItemID.SittingDucksFishingRod)
            //hotline fishing
            .AddIngredient(ItemID.GoldenFishingRod)
            .AddIngredient(ItemID.GoldenCarp)
            .AddIngredient(ItemID.ReaverShark)
            .AddIngredient(ItemID.Bladetongue)
            .AddIngredient(ItemID.ObsidianSwordfish)
            .AddIngredient(ItemID.FuzzyCarrot)
            .AddIngredient(ItemID.HardySaddle)
            //.AddIngredient(ItemID.ZephyrFish);

            .AddTile(ModContent.Find<ModTile>("Fargowiltas", "CrucibleCosmosSheet"))

            
            .Register();
        }
    }
}
