using Terraria;
using Terraria.Localization;
using Terraria.ID;
using FargowiltasSouls.Toggler;

namespace FargowiltasSouls.Items.Accessories.Masomode
{
    public class NecromanticBrew : SoulsItem
    {
        public override bool Eternity => true;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Necromantic Brew");
            Tooltip.SetDefault(@"Grants immunity to Lethargic
Summons 2 Skeletron arms to whack enemies
'The bone-growing solution of a defeated foe'");
            DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "死灵密酿");
            Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, @"被击败敌人的促进骨生长的溶液
免疫昏昏欲睡
召唤2个骷髅王手臂重击敌人
可能会吸引宝宝骷髅头");
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.accessory = true;
            item.rare = ItemRarityID.LightRed;
            item.value = Item.sellPrice(0, 3);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.buffImmune[ModContent.BuffType<Lethargic>()] = true;
            player.GetModPlayer<FargoSoulsPlayer>().NecromanticBrew = true;
            if (player.GetToggleValue("MasoSkele"))
                player.AddBuff(ModContent.BuffType<SkeletronArms>(), 2);
        }
    }
}