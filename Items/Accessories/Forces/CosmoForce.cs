using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using FargowiltasSouls.Items.Accessories.Enchantments;
using FargowiltasSouls.Items.Materials;

namespace FargowiltasSouls.Items.Accessories.Forces
{
    public class CosmoForce : BaseForce
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            DisplayName.SetDefault("Force of Cosmos");
            
            DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "宇宙之力");
            
            string tooltip =
$"[i:{ModContent.ItemType<MeteorEnchant>()}] A meteor shower initiates every few seconds while attacking\n" +
$"[i:{ModContent.ItemType<SolarEnchant>()}] Solar shield allows you to dash through enemies\n" +
$"[i:{ModContent.ItemType<SolarEnchant>()}] Attacks may inflict the Solar Flare debuff\n" +
$"[i:{ModContent.ItemType<VortexEnchant>()}] Double tap down to toggle stealth and spawn a vortex\n" +
$"[i:{ModContent.ItemType<NebulaEnchant>()}] Hurting enemies has a chance to spawn buff boosters\n" +
$"[i:{ModContent.ItemType<StardustEnchant>()}] A stardust guardian will protect you from nearby enemies\n" +
$"[i:{ModContent.ItemType<StardustEnchant>()}] Press the Freeze Key to freeze time for 5 seconds, 60 second cooldown\n" +
"'Been around since the Big Bang'";
            Tooltip.SetDefault(tooltip);

            string tooltip_ch =
@"攻击时每过几秒便会释放一次流星雨
允许你使用日耀护盾进行冲刺
攻击有几率造成耀斑减益
双击'下'键切换至隐形模式，减少敌人以你为目标的几率，但大幅降低移动速度
进入隐形状态时生成一个会吸引并伤害敌人的旋涡
伤害敌人时有几率生成强化增益
双击'下'键将你的守卫引至光标位置
按下'冻结'键后会冻结5秒时间
星尘守卫不受时间冻结影响且在此期间会获得全新的强力攻击
此效果有60秒冷却时间，冷却结束时会播放音效
'自宇宙大爆炸以来就一直存在";
            Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, tooltip_ch);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            FargoSoulsPlayer modPlayer = player.GetModPlayer<FargoSoulsPlayer>();
            //meme speed, solar flare,
            modPlayer.CosmoForce = true;

            //meteor shower
            modPlayer.MeteorEffect();
            //solar shields
            modPlayer.SolarEffect();
            //stealth, voids, pet
            modPlayer.VortexEffect(hideVisual);
            //boosters and meme speed
            modPlayer.NebulaEffect();
            //guardian and time freeze
            modPlayer.StardustEffect(Item);
        }

        public override void AddRecipes()
        {
            CreateRecipe()

            .AddIngredient(ModContent.ItemType<MeteorEnchant>())
            .AddIngredient(ModContent.ItemType<SolarEnchant>())
            .AddIngredient(ModContent.ItemType<VortexEnchant>())
            .AddIngredient(ModContent.ItemType<NebulaEnchant>())
            .AddIngredient(ModContent.ItemType<StardustEnchant>())
            .AddIngredient(ModContent.ItemType<Eridanium>(), 5)

            .AddTile(ModContent.Find<ModTile>("Fargowiltas", "CrucibleCosmosSheet"))

            
            .Register();
        }
    }
}
