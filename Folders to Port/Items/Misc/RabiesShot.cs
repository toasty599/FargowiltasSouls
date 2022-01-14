using Terraria;
using Terraria.ID;
using Terraria.Localization;

namespace FargowiltasSouls.Items.Misc
{
    public class RabiesShot : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Rabies Shot");
            Tooltip.SetDefault("Cures Feral Bite");
            DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "狂犬疫苗");
            Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, "治愈野性咬噬");
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.maxStack = 30;
            item.rare = ItemRarityID.Orange;
            item.useStyle = ItemUseStyleID.EatingUsing;
            item.useAnimation = 17;
            item.useTime = 17;
            item.consumable = true;
            item.UseSound = SoundID.Item3;
            item.value = Item.sellPrice(0, 0, 4, 0);
        }

        public override bool UseItem(Player player)
        {
            if (player.itemAnimation > 0 && player.itemTime == 0)
            {
                player.ClearBuff(BuffID.Rabies);
            }
            return true;
        }
    }
}