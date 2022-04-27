using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Buffs.Masomode
{
    public class Neurotoxin : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Neurotoxin");
            Description.SetDefault("Your body is shutting down");
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Terraria.ID.BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            FargoSoulsPlayer p = player.GetModPlayer<FargoSoulsPlayer>();

            player.poisoned = true;
            player.venom = true;
            player.slowOgreSpit = true;

            player.ClearBuff(ModContent.BuffType<Infested>());

            p.MaxInfestTime = 2;
            p.FirstInfection = false;
            p.Infested = true;
        }
    }
}