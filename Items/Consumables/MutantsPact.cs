using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Items.Consumables
{
    public class MutantsPact : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Mutant's Pact");
            Tooltip.SetDefault("Permanently increases the number of accessory slots");
            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.rare = ItemRarityID.Purple;
            Item.maxStack = 99;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.useAnimation = 30;
            Item.useTime = 30;
            Item.consumable = true;
            Item.UseSound = SoundID.Item123;
            Item.value = Item.sellPrice(0, 15);
        }

        public override bool CanUseItem(Player player)
        {
            return !player.GetModPlayer<FargoSoulsPlayer>().MutantsPactSlot;
        }

        public override bool? UseItem(Player player)
        {
            if (player.itemAnimation > 0 && player.itemTime == 0)
            {
                player.GetModPlayer<FargoSoulsPlayer>().MutantsPactSlot = true;

                Terraria.Audio.SoundEngine.PlaySound(SoundID.Roar, player.Center, 0);
                if (!Main.dedServ)
                    Terraria.Audio.SoundEngine.PlaySound(SoundLoader.GetLegacySoundSlot(FargowiltasSouls.Instance, "Sounds/Thunder"), player.Center);
            }
            return true;
        }

        public override void SafeModifyTooltips(List<TooltipLine> list)
        {
            foreach (TooltipLine line2 in list)
            {
                if (line2.Mod == "Terraria" && line2.Name == "ItemName")
                {
                    line2.OverrideColor = new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB);
                }
            }
        }
    }
}