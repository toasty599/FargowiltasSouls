using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;

namespace FargowiltasSouls.Items.Accessories.Masomode
{
    [AutoloadEquip(EquipType.Shoes)]
    public class AeolusBoots : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Aeolus Boots");
            Tooltip.SetDefault(@"Allows flight, super fast running, and extra mobility on ice
8% increased movement speed
Allows the holder to double jump
Increases jump height and negates fall damage
'Run like the wind'");
            DisplayName.AddTranslation(GameCulture.Chinese, "埃俄罗斯之靴");
            Tooltip.AddTranslation(GameCulture.Chinese, @"'风一般地奔跑'
允许飞行，超快速奔跑，冰上额外机动力
+8%移动速度
允许持有者二段跳
增加跳跃高度，免疫摔伤");
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.accessory = true;
            item.rare = ItemRarityID.Yellow;
            item.value = Item.sellPrice(0, 10);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.accRunSpeed = 6.75f;
            player.rocketBoots = 3;
            player.moveSpeed += 0.08f;
            player.iceSkate = true;
            if (SoulConfig.Instance.GetValue(SoulConfig.Instance.AeolusJump))
                player.doubleJumpFart = true;
            player.jumpBoost = true;
            player.noFallDmg = true;

            //add effects
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);

            recipe.AddIngredient(ItemID.FrostsparkBoots); //terraspark
            //amphibian
            //fairy boots
            //dunerider
            recipe.AddIngredient(ItemID.BalloonHorseshoeFart);
            recipe.AddIngredient(mod.ItemType("EurusSock"));
            recipe.AddIngredient(mod.ItemType("DeviatingEnergy"), 10);

            recipe.AddTile(TileID.TinkerersWorkbench);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}