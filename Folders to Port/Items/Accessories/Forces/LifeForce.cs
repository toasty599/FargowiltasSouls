using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using FargowiltasSouls.Items.Accessories.Enchantments;

namespace FargowiltasSouls.Items.Accessories.Forces
{
    public class LifeForce : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Force of Life");

            string tooltip =
$"[i:{ModContent.ItemType<PumpkinEnchant>()}] You will grow pumpkins while walking on the ground\n" +
$"[i:{ModContent.ItemType<CactusEnchant>()}] Enemies may explode into needles on death\n" +
$"[i:{ModContent.ItemType<BeeEnchant>()}] Your piercing attacks spawn bees\n" +
$"[i:{ModContent.ItemType<SpiderEnchant>()}] 30% chance for minions and sentries to crit\n" +
$"[i:{ModContent.ItemType<TurtleEnchant>()}] When standing still and not attacking, you will enter your shell\n" +
$"[i:{ModContent.ItemType<BeetleEnchant>()}] Beetles protect you from damage\n" +
$"[i:{ModContent.ItemType<BeetleEnchant>()}] Increases flight time by 50%\n" +
"'Rare is a living thing that dare disobey your will'";
            string tooltip_ch =
@"你在草地上行走时会种下南瓜
反弹100%接触伤害
敌人死亡时有几率爆裂出针刺
使友方蜜蜂或黄蜂转化为大型蜜蜂
你的仆从和哨兵现在可以造成暴击且有30%基础暴击率
站定不动时且不攻击时你会缩进壳里
甲虫会保护你，减免下次受到的伤害
延长50%飞行时间
'罕有活物敢违背你的意愿'";

            Tooltip.SetDefault(tooltip);
            DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "生命之力");
            Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, tooltip_ch);
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.accessory = true;
            ItemID.Sets.ItemNoGravity[item.type] = true;
            item.rare = ItemRarityID.Purple;
            item.value = 600000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            FargoSoulsPlayer modPlayer = player.GetModPlayer<FargoSoulsPlayer>();
            //tide hunter, yew wood, iridescent effects
            modPlayer.LifeForce = true;
            //bees ignore defense, super bees, pet
            modPlayer.BeeEffect(hideVisual);
            //minion crits and pet
            modPlayer.SpiderEffect(hideVisual);
            //defense beetle bois
            modPlayer.BeetleEffect();
            if (!modPlayer.TerrariaSoul)
                modPlayer.wingTimeModifier += .5f;
            //flame trail, pie heal, pet
            modPlayer.PumpkinEffect(hideVisual);
            //shell hide, pets
            modPlayer.TurtleEffect(hideVisual);
            //needle spray
            modPlayer.CactusEffect();
        }

        public override void AddRecipes()
        {
            CreateRecipe()

            .AddIngredient(null, "PumpkinEnchant");
            .AddIngredient(null, "BeeEnchant");
            .AddIngredient(null, "SpiderEnchant");
            .AddIngredient(null, "TurtleEnchant");
            .AddIngredient(null, "BeetleEnchant");

            recipe.AddTile(ModLoader.GetMod("Fargowiltas").TileType("CrucibleCosmosSheet"));

            recipe.SetResult(this);
            .Register();
        }
    }
}
