using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace FargowiltasSouls.Buffs
{
    public class BerserkerInstallCD : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Berserker Cooldown");
            Description.SetDefault("You cannot go berserk yet");
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;
            Terraria.ID.BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
        }
    }
}