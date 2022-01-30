using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Buffs.Masomode
{
    public class ChaosLife : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Chaos Life");
            Description.SetDefault("Max life reduced");
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
            Terraria.ID.BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.GetModPlayer<FargoSoulsPlayer>().OceanicMaul = true;
            if (player.buffTime[buffIndex] < 30 && FargoSoulsUtil.AnyBossAlive())
                player.buffTime[buffIndex] = 30;
        }
    }
}