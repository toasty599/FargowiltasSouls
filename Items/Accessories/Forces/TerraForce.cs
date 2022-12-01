using FargowiltasSouls.Items.Accessories.Enchantments;
using FargowiltasSouls.Toggler;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace FargowiltasSouls.Items.Accessories.Forces
{
    [AutoloadEquip(EquipType.Shield)]
    public class TerraForce : BaseForce
    {
        public static int[] Enchants => new int[]
        {
            ModContent.ItemType<CopperEnchant>(),
            ModContent.ItemType<TinEnchant>(),
            ModContent.ItemType<IronEnchant>(),
            ModContent.ItemType<LeadEnchant>(),
            ModContent.ItemType<SilverEnchant>(),
            ModContent.ItemType<TungstenEnchant>(),
            ModContent.ItemType<ObsidianEnchant>()
        };

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            DisplayName.SetDefault("Terra Force");

            string tooltip =
$"[i:{ModContent.ItemType<CopperEnchant>()}][i:{ModContent.ItemType<ObsidianEnchant>()}][i:{ModContent.ItemType<LeadEnchant>()}] Attacks spawn lightning, explosions, and inflict Lead Poisoning\n" +
$"[i:{ModContent.ItemType<TinEnchant>()}] Sets Tin crit chance to 10%, increasing up to double your current crit chance or 50%\n" +
$"[i:{ModContent.ItemType<IronEnchant>()}] You attract items from further away, 33% chance to not consume items at an Anvil\n" +
$"[i:{ModContent.ItemType<LeadEnchant>()}] You take 60% less from damage over time\n" +
$"[i:{ModContent.ItemType<SilverEnchant>()}] Right Click to guard with your shield\n" +
$"[i:{ModContent.ItemType<TungstenEnchant>()}] 300% increased sword size and projectile size every quarter second\n" +
$"[i:{ModContent.ItemType<ObsidianEnchant>()}] Grants immunity to fire and lava\n" +
"'The land lends its strength'";
            Tooltip.SetDefault(tooltip);

            string tooltip_ch =
@"[i:{0}] 攻击有几率释放闪电攻击敌人
[i:{1}] 将你的基础暴击率设为10%
[i:{1}] 每次暴击时都会增加5%暴击率，增加的暴击率的最大值为你当前最大暴击率数值x2
[i:{1}] 被击中后会降低暴击率
[i:{2}] 右键进行盾牌格挡
[i:{2}] 在受伤前格挡以格挡此次攻击
[i:{2}] 扩大你的拾取范围
[i:{3}] 攻击有几率造成铅中毒减益
[i:{4}] 剑的大小增加300%
[i:{4}] 每过0.25秒便会有一个随机弹幕的尺寸翻倍
[i:{5}] 使你免疫火块与熔岩
[i:{5}] 你的攻击会引发爆炸
[i:{5}] 鞭子的范围增加50%
“大地赐予它力量”";
            Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, string.Format(tooltip_ch, Enchants[0], Enchants[1], Enchants[2], Enchants[3], Enchants[4], Enchants[5]));

            string tooltip_pt =
@"[i:{0}][i:{6}][i:{4}] Ataques invocam relâmpagos, explosões e infligem Envenenamento por chumbo
[i:{1}] Define a chance de acerto crítico do Estanho em 10%, aumentando até o dobro da sua chance de acerto crítico ou 50%
[i:{2}] Você atrai itens de mais longe, 33% de chance de não consumir itens em uma Bigorna
[i:{3}] Você sofre 60% menos dano de efeitos negativos
[i:{4}] Clique com o botão direito para defender com seu escudo
[i:{5}] 300% de aumento no tamanho das espadas e de projéteis a cada quarto de segundo
[i:{6}] Oferece imunidade a fogo e lava
'A terra empresta sua força'";
            Tooltip.AddTranslation((int)GameCulture.CultureName.Portuguese, string.Format(tooltip_pt, Enchants[0], Enchants[1], Enchants[2], Enchants[3], Enchants[4], Enchants[5], Enchants[6]));
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            FargoSoulsPlayer modPlayer = player.GetModPlayer<FargoSoulsPlayer>();
            modPlayer.TerraForce = true;
            CopperEnchant.CopperEffect(player, Item);
            TinEnchant.TinEffect(player, Item);
            modPlayer.IronEnchantItem = Item;
            LeadEnchant.LeadEffect(player, Item);
            if (player.GetToggleValue("SilverS"))
            {
                //shield
                modPlayer.SilverEnchantItem = Item;
            }
            TungstenEnchant.TungstenEffect(player, Item);
            ObsidianEnchant.ObsidianEffect(player, Item);
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
