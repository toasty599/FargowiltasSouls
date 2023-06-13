using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Buffs.Souls
{
    public class BrokenShell : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Broken Shell");
            Description.SetDefault("You cannot enter your shell yet");
            Main.buffNoSave[Type] = true;
            Main.debuff[Type] = true;
            Terraria.ID.BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
        }
    }
}