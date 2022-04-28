using Terraria;
using Terraria.ModLoader;
using Terraria.Localization;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria.ID;
using FargowiltasSouls.Items;
using Terraria.Chat;

namespace FargowiltasSouls.Patreon.LaBonez
{
    public class PiranhaPlantVoodooDoll : PatreonModItem
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            DisplayName.SetDefault("Piranha Plant Voodoo Doll");
            Tooltip.SetDefault(
@"Toggle that will grant all enemies the ability to inflict random debuffs
'In loving memory of Masochist mode EX. I always hated you.'");
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.maxStack = 1;
            Item.rare = 1;
            Item.useAnimation = 30;
            Item.useTime = 30;
            Item.useStyle = 4;
            Item.consumable = false;
        }

        public override bool? UseItem(Player player)
        {
            PatreonPlayer patreonPlayer = player.GetModPlayer<PatreonPlayer>();
            patreonPlayer.PiranhaPlantMode = !patreonPlayer.PiranhaPlantMode;

            FargoSoulsUtil.PrintLocalization(patreonPlayer.PiranhaPlantMode ? $"Mods.{Mod.Name}.Message.{Name}On" : $"Mods.{Mod.Name}.Message.{Name}Off", 175, 75, 255);
            if (Main.netMode == NetmodeID.Server)
                NetMessage.SendData(MessageID.WorldData); //sync world

            Terraria.Audio.SoundEngine.PlaySound(SoundID.Roar, (int)player.position.X, (int)player.position.Y, 0);

            return true;
        }
    }
}