using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Buffs.Masomode
{
    public class Anticoagulation : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Anticoagulation");
            Description.SetDefault("Losing life, shed blood when hurt, enemies will drink it and grow stronger");
            Main.debuff[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.bleed = true;
            player.GetModPlayer<FargoSoulsPlayer>().Anticoagulation = true;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.GetGlobalNPC<NPCs.FargoSoulsGlobalNPC>().Anticoagulation = true;
        }
    }
}