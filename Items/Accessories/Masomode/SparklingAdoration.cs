using FargowiltasSouls.Toggler;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;

namespace FargowiltasSouls.Items.Accessories.Masomode
{
    public class SparklingAdoration : SoulsItem
    {
        public override bool Eternity => true;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sparkling Adoration");
            Tooltip.SetDefault(@"Grants immunity to Lovestruck and Fake Hearts
Graze projectiles to gain up to 25% increased critical damage
Critical damage bonus decreases over time and is fully lost on hit
Your attacks periodically summon life-draining hearts
'With all of your emotion!'");
            Main.RegisterItemAnimation(item.type, new DrawAnimationVertical(4, 11));
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
            player.buffImmune[BuffID.Lovestruck] = true;
            player.buffImmune[mod.BuffType("Lovestruck")] = true;

            if (player.GetToggleValue("MasoGraze", false))
                player.GetModPlayer<FargoPlayer>().Graze = true;

            if (player.GetToggleValue("MasoDevianttHearts"))
                player.GetModPlayer<FargoPlayer>().DevianttHearts = true;
        }
    }
}