using Terraria;
using Terraria.ModLoader;
using Terraria.Localization;

namespace FargowiltasSouls.Buffs.Souls
{
    public class Revived : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Revived");
            Description.SetDefault("Revived recently");
            Main.buffNoSave[Type] = true;
            canBeCleared = false;
            Main.debuff[Type] = true;
            DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "已复活");
            Description.AddTranslation((int)GameCulture.CultureName.Chinese, "最近经历过复活");
        }
    }
}