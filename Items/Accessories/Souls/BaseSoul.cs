using FargowiltasSouls.Utilities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using FargowiltasSouls.Projectiles;

namespace FargowiltasSouls.Items.Accessories.Souls
{
    public abstract class BaseSoul : SoulsItem
    {
        protected virtual Color? nameColor => null;

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            ItemID.Sets.ItemNoGravity[Type] = true;
            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SafeModifyTooltips(List<TooltipLine> tooltips)
        {
            base.SafeModifyTooltips(tooltips);

            if (nameColor != null && tooltips.TryFindTooltipLine("ItemName", out TooltipLine itemNameLine))
                itemNameLine.overrideColor = nameColor;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.width = 20;
            Item.height = 20;
            Item.accessory = true;
            Item.value = 1000000;
            Item.rare = ItemRarityID.Purple;
        }

        public override Color? GetAlpha(Color lightColor) => Color.White;
    }
}
