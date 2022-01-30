using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Buffs.Masomode
{
    public class Recovering : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Recovering");
            Description.SetDefault("The Nurse cannot heal you again yet");
            Main.debuff[Type] = true;
            BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
        }
    }
}