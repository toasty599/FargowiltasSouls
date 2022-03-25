using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace FargowiltasSouls.Items
{
    public class MasochistReal : SoulsItem
    {
        public override bool IsLoadingEnabled(Mod mod) => false;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Forgotten Gift");
            Tooltip.SetDefault("[c/00ff00:World must be in Eternity Mode!]\nToggles Masochist Mode");
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.maxStack = 1;
            Item.rare = ItemRarityID.Blue;
            Item.useAnimation = 30;
            Item.useTime = 30;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.consumable = false;
        }

        public override bool? UseItem(Player player)
        {
            if (!FargoSoulsWorld.EternityMode)
            {
                FargoSoulsUtil.PrintText("World must be in Eternity Mode!", new Color(255, 51, 153));
                return true;
            }

            if (!FargoSoulsUtil.AnyBossAlive())
            {
                FargoSoulsWorld.MasochistModeReal = !FargoSoulsWorld.MasochistModeReal;

                Terraria.Audio.SoundEngine.PlaySound(SoundID.Roar, (int)player.position.X, (int)player.position.Y, 0);

                FargoSoulsUtil.PrintText(FargoSoulsWorld.MasochistModeReal ? "The difficulty got real!" : "The difficulty got fake!", new Color(255, 51, 153));

                if (Main.netMode == NetmodeID.Server)
                    NetMessage.SendData(MessageID.WorldData); //sync world
            }
            return true;
        }
    }
}