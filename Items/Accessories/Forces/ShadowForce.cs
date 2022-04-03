using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using FargowiltasSouls.Items.Accessories.Enchantments;

namespace FargowiltasSouls.Items.Accessories.Forces
{
    public class ShadowForce : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Shadow Force");
            
            DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "暗影之力");
            
            string tooltip = 
$"[i:{ModContent.ItemType<ShadowEnchant>()}] Four Shadow Orbs will orbit around you\n" +
$"[i:{ModContent.ItemType<AncientShadowEnchant>()}] Your attacks may inflict Darkness on enemies\n" +
$"[i:{ModContent.ItemType<NecroEnchant>()}] Slain enemies may drop a pile of bones\n" +
$"[i:{ModContent.ItemType<SpookyEnchant>()}] All of your minions gain an extra scythe attack\n" +
$"[i:{ModContent.ItemType<NinjaEnchant>()}] Throw a smoke bomb to teleport to it and gain the First Strike Buff\n" +
$"[i:{ModContent.ItemType<MonkEnchant>()}] Don't attack to gain a single use monk dash\n" +
$"[i:{ModContent.ItemType<ShinobiEnchant>()}] Dash into any walls, to teleport through them to the next opening\n" +
$"[i:{ModContent.ItemType<DarkArtistEnchant>()}] Summons a Flameburst minion that will travel to your mouse after charging\n" +
$"[i:{ModContent.ItemType<ApprenticeEnchant>()}] After attacking for 2 seconds you will be enveloped in flames\n" +
$"[i:{ModContent.ItemType<ApprenticeEnchant>()}] Switching weapons will increase the next attack's damage by 150%\n" +
"'Dark, Darker, Yet Darker'";
            Tooltip.SetDefault(tooltip);

            string tooltip_ch = @"四颗暗影珠围绕着你旋转
攻击有几率造成黑暗减益
击杀敌人时有几率爆出一摞骨头
你的召唤物获得了额外的镰刀攻击
扔出烟雾弹后会将你传送至其落点的位置并使你获得先发制人增益
不攻击一段时间后使你获得武僧冲刺增益
冲进墙壁时会直接穿过去
召唤一个爆炸烈焰哨兵，在充能完毕后会移动至光标位置
持续攻击两秒后你将被火焰包裹
切换武器后使下次攻击的伤害增加100%
大幅强化爆炸烈焰哨兵和闪电光环的效果
'Dark, Darker, Yet Darker（出自Undertale）'";
            Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, tooltip_ch);

            ItemID.Sets.ItemNoGravity[Item.type] = true;
            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.accessory = true;
            Item.rare = ItemRarityID.Purple;
            Item.value = 600000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            FargoSoulsPlayer modPlayer = player.GetModPlayer<FargoSoulsPlayer>();
            //warlock, shade, plague accessory effect for all
            modPlayer.ShadowForce = true;
            ////shoot from where you were meme, pet
            //modPlayer.DarkArtistEffect(hideVisual);
            //modPlayer.ApprenticeEffect();

            ////DG meme, pet
            //modPlayer.NecroEffect(hideVisual);
            ////shadow orbs
            //modPlayer.AncientShadowEffect();
            ////darkness debuff, pets
            //modPlayer.ShadowEffect(hideVisual);
            ////tele thru walls, pet
            //modPlayer.ShinobiEffect(hideVisual);
            ////monk dash mayhem
            //modPlayer.MonkEffect();
            ////smoke bomb nonsense, pet
            //modPlayer.NinjaEffect(hideVisual);
            ////scythe doom, pets
            //modPlayer.SpookyEffect(hideVisual);
        }

        public override void AddRecipes()
        {
            CreateRecipe()

            .AddIngredient(null, "AncientShadowEnchant")
            .AddIngredient(null, "NecroEnchant")
            .AddIngredient(null, "SpookyEnchant")
            .AddIngredient(null, "ShinobiEnchant")
            .AddIngredient(null, "DarkArtistEnchant")

            .AddTile(ModContent.Find<ModTile>("Fargowiltas", "CrucibleCosmosSheet"))

            
            .Register();
        }
    }
}
