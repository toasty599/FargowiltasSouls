using FargowiltasSouls.Items;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria.ModLoader;

namespace FargowiltasSouls.Patreon
{
    public abstract class PatreonModItem : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SafeModifyTooltips(List<TooltipLine> tooltips)
        {
            base.SafeModifyTooltips(tooltips);

            TooltipLine line = new TooltipLine(Mod, "tooltip", ">> Patreon Item <<");
            line.OverrideColor = Color.Orange;
            tooltips.Add(line);
        }
    }
}