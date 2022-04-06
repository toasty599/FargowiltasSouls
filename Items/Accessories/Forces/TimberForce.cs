using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using FargowiltasSouls.Items.Accessories.Enchantments;

namespace FargowiltasSouls.Items.Accessories.Forces
{
    public class TimberForce : BaseForce
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            DisplayName.SetDefault("Force of Timber");

            Tooltip.SetDefault(
$"[i:{ModContent.ItemType<WoodEnchant>()}] Turns certain critters into weapons\n" +
$"[i:{ModContent.ItemType<BorealWoodEnchant>()}] Attacks will periodically be accompanied by several snowballs\n" +
$"[i:{ModContent.ItemType<RichMahoganyEnchant>()}] All grappling hooks shoot, pull, and retract 2.5x as fast\n" +
$"[i:{ModContent.ItemType<EbonwoodEnchant>()}] You have an aura of Shadowflame, Cursed Flames, and Bleeding\n" +
$"[i:{ModContent.ItemType<PalmWoodEnchant>()}] Double tap down to spawn a palm tree sentry that throws nuts at enemies\n" +
$"[i:{ModContent.ItemType<PearlwoodEnchant>()}] Projectiles may spawn a star when they hit something\n" +
"'Extremely rigid'");
            DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "森林之力");
            Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese,
@"小动物在释放出去1秒后会爆炸
50%几率不消耗弹药
攻击时定期释放雪球
所有钩爪的抛出速度、牵引速度和回收速度x2.5
一圈暗影焰、诅咒焰和流血光环环绕着你
双击'下'键会召唤一个会向敌人扔橡子的棕榈树哨兵
弹幕在击中敌人或物块时有几率生成一颗星星
'很刚'");
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            FargoSoulsPlayer modPlayer = player.GetModPlayer<FargoSoulsPlayer>();
            modPlayer.WoodForce = true;
            WoodEnchant.WoodEffect(player);
            BorealWoodEnchant.BorealEffect(player);
            modPlayer.MahoganyEnchantActive = true;
            EbonwoodEnchant.EbonwoodEffect(player);
            ShadewoodEnchant.ShadewoodEffect(player);
            PalmWoodEnchant.PalmEffect(player);
            PearlwoodEnchant.PearlwoodEffect(player);
        }

        public override void AddRecipes()
        {
            CreateRecipe()

            .AddIngredient(ModContent.ItemType<WoodEnchant>())
            .AddIngredient(ModContent.ItemType<BorealWoodEnchant>())
            .AddIngredient(ModContent.ItemType<RichMahoganyEnchant>())
            .AddIngredient(ModContent.ItemType<EbonwoodEnchant>())
            .AddIngredient(ModContent.ItemType<ShadewoodEnchant>())
            .AddIngredient(ModContent.ItemType<PalmWoodEnchant>())
            .AddIngredient(ModContent.ItemType<PearlwoodEnchant>())

            .AddTile(ModContent.Find<ModTile>("Fargowiltas", "CrucibleCosmosSheet"))

            
            .Register();
        }
    }
}
