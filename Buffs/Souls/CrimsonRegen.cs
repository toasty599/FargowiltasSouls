using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Buffs.Souls
{
    public class CrimsonRegen : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Crimson Regen");
            Description.SetDefault("You are regenning your last wound");
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.buffTime[buffIndex] = 2;
            player.GetModPlayer<FargoSoulsPlayer>().CrimsonRegen = true;
        }
    }
}