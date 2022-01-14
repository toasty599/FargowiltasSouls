using Terraria;
using Terraria.ModLoader;
using Terraria.Localization;

namespace FargowiltasSouls.Buffs.Masomode
{
    public class Shadowflame : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Shadowflame");
            Description.SetDefault("Losing life");
            DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "暗影烈焰");
            Description.AddTranslation((int)GameCulture.CultureName.Chinese, "流失生命");
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
            canBeCleared = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.GetModPlayer<FargoSoulsPlayer>().Shadowflame = true;
        }
    }
}