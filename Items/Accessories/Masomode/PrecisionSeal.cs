using FargowiltasSouls.Buffs.Masomode;
using FargowiltasSouls.Toggler;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Items.Accessories.Masomode
{
    public class PrecisionSeal : SoulsItem
    {
        public override string Texture => "FargowiltasSouls/Items/Placeholder";

        public override bool Eternity => true;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Precision Seal");
            Tooltip.SetDefault(@"Grants immunity to Smite
Reduces your hurtbox size for projectiles
Hold the Precision Seal key to disable dashes and double jumps
'Dodge so close you can almost taste it'");

            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.accessory = true;
            Item.rare = ItemRarityID.Red;
            Item.value = Item.sellPrice(0, 8);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            FargoSoulsPlayer modPlayer = player.GetModPlayer<FargoSoulsPlayer>();

            player.buffImmune[ModContent.BuffType<Smite>()] = true;
            modPlayer.PrecisionSeal = true;
            if (player.GetToggleValue("PrecisionSealHurtbox", false))
                modPlayer.PrecisionSealHurtbox = true;
        }
    }
}