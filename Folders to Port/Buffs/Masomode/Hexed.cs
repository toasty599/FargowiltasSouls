using Terraria;
using Terraria.ModLoader;
using Terraria.Localization;

namespace FargowiltasSouls.Buffs.Masomode
{
    public class Hexed : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Hexed");
            Description.SetDefault("Your attacks heal enemies");
            Main.buffNoSave[Type] = true;
            canBeCleared = true;
            Main.debuff[Type] = true;
            DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "着魔");
            Description.AddTranslation((int)GameCulture.CultureName.Chinese, "你的攻击会治愈敌人");
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.GetModPlayer<FargoSoulsPlayer>().Hexed = true;
        }

        public override bool ReApply(Player player, int time, int buffIndex)
        {
            return time > 2;
        }
    }
}