using FargowiltasSouls.Content.Items;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria.Localization;
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

            TooltipLine line = new TooltipLine(Mod, "tooltip", Language.GetTextValue($"Mods.{Mod.Name}.Message.PatreonItem"));
            line.OverrideColor = Color.Orange;
            tooltips.Add(line);
        }
    }
}