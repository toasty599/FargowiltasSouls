using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using FargowiltasSouls.Toggler;

namespace FargowiltasSouls.Items.Accessories.Enchantments
{
    public class TitaniumEnchant : BaseEnchant
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            DisplayName.SetDefault("Titanium Enchantment");
            Tooltip.SetDefault(
@"Attacking generates a defensive barrier of titanium shards
''");

            DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "钛金魔石");
            Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese,
@"攻击敌人后会使你无敌一小段时间
'Hit me with your best shot'（某歌曲名）");
        }

        protected override Color nameColor => new Color(130, 140, 136);

        public override void SetDefaults()
        {
            base.SetDefaults();
            
            Item.rare = ItemRarityID.Pink;
            Item.value = 100000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            TitaniumEffect(player);
        }

        public static void TitaniumEffect(Player player)
        {
            if (player.GetToggleValue("Titanium"))
            {
                player.onHitTitaniumStorm = true;
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddRecipeGroup("FargowiltasSouls:AnyTitaHead")
                .AddIngredient(ItemID.TitaniumBreastplate)
                .AddIngredient(ItemID.TitaniumLeggings)
            //.AddIngredient(ItemID.TitaniumDrill);
            .AddIngredient(ItemID.TitaniumSword)
            .AddIngredient(ItemID.Rockfish)

                .AddTile(TileID.CrystalBall)
                .Register();
        }
    }
}
