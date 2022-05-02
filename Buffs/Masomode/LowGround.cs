using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace FargowiltasSouls.Buffs.Masomode
{
    public class LowGround : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Low Ground");
            Description.SetDefault("Cannot stand on platforms or liquids");
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;
            DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "低地");
            Description.AddTranslation((int)GameCulture.CultureName.Chinese, "不能站在平台或液体上");
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.GetModPlayer<FargoSoulsPlayer>().LowGround = true;

            if (player.mount.Active)
                player.mount.Dismount(player);

            for (int i = -2; i <= 2; i++)
            {
                Vector2 pos = player.Center;
                pos.X += i * 16;
                pos.Y += player.height / 2;
                if (player.mount.Active)
                    pos.Y += player.mount.HeightBoost;
                pos.Y += 8;

                Tile tile = Framing.GetTileSafely((int)(pos.X / 16), (int)(pos.Y / 16));
                if (tile.TileType == TileID.Platforms || tile.TileType == TileID.PlanterBox)
                    tile.IsActuated = true;
            }
        }
    }
}
