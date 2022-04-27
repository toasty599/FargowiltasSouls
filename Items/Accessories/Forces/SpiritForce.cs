using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using FargowiltasSouls.Items.Accessories.Enchantments;

namespace FargowiltasSouls.Items.Accessories.Forces
{
    public class SpiritForce : BaseForce
    {
        public static int[] Enchants => new int[]
        {
            ModContent.ItemType<FossilEnchant>(),
            ModContent.ItemType<ForbiddenEnchant>(),
            ModContent.ItemType<HallowEnchant>(),
            ModContent.ItemType<AncientHallowEnchant>(),
            ModContent.ItemType<TikiEnchant>(),
            ModContent.ItemType<SpectreEnchant>()
        };

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            DisplayName.SetDefault("Force of Spirit");
            
            DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "心灵之力");
            
            string tooltip =
$"[i:{ModContent.ItemType<FossilEnchant>()}] If you reach zero HP you will revive with 50 HP and spawn several bones\n" +
$"[i:{ModContent.ItemType<FossilEnchant>()}] Collect the bones once they stop moving to heal for 15 HP each\n" +
$"[i:{ModContent.ItemType<ForbiddenEnchant>()}] Double tap down to call an ancient storm to the cursor location\n" +
$"[i:{ModContent.ItemType<ForbiddenEnchant>()}] Any projectiles shot through your storm gain 60% damage\n" +
$"[i:{ModContent.ItemType<AncientHallowEnchant>()}] You gain a shield that can reflect projectiles\n" +
$"[i:{ModContent.ItemType<AncientHallowEnchant>()}] Summons an Enchanted Sword familiar\n" +
$"[i:{ModContent.ItemType<AncientHallowEnchant>()}] Drastically increases minion speed\n" +
$"[i:{ModContent.ItemType<HallowEnchant>()}] Become immune after striking an enemy\n" +
$"[i:{ModContent.ItemType<TikiEnchant>()}] You can summon temporary minions and sentries after maxing out on your slots\n" +
$"[i:{ModContent.ItemType<SpectreEnchant>()}] Damage has a chance to spawn damaging and healing orbs\n" +
"'Ascend from this mortal realm'";
            Tooltip.SetDefault(tooltip);

            string tooltip_ch =
@"受到致死伤害时会以1生命值重生并爆出几根骨头
每根骨头会回复15点生命值
双击'下'键召唤远古风暴至光标位置
穿过远古风暴的弹幕会获得60%额外伤害
召唤一柄附魔剑
使你获得一面可以反弹弹幕的盾牌
在召唤栏用光后你仍可以召唤临时的哨兵和仆从
伤害敌人时有几率生成幽魂珠
攻击造成暴击时有几率生成治疗珠
'从尘世飞升'";
            Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, tooltip_ch);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            FargoSoulsPlayer modPlayer = player.GetModPlayer<FargoSoulsPlayer>();
            //spectre works for all, spirit trapper works for all
            modPlayer.SpiritForce = true;
            FossilEnchant.FossilEffect(player);
            modPlayer.ForbiddenEffect();
            HallowEnchant.HallowEffect(player);
            AncientHallowEnchant.AncientHallowEffect(player, Item);
            modPlayer.TikiEffect(hideVisual);
            modPlayer.SpectreEffect(hideVisual);
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            foreach (int ench in Enchants)
                recipe.AddIngredient(ench);
            recipe.AddTile(ModContent.Find<ModTile>("Fargowiltas", "CrucibleCosmosSheet"));
            recipe.Register();
        }
    }
}
