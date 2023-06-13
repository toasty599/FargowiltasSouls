using FargowiltasSouls.Content.NPCs;
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Buffs.Souls
{
    public class MoltenAmplifyBuff : ModBuff
    {
        public override string Texture => "FargowiltasSouls/Content/BuffsPlaceholderDebuff";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Molten Amplify");
            Main.buffNoSave[Type] = true;
            Main.debuff[Type] = true;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.GetGlobalNPC<FargoSoulsGlobalNPC>().MoltenAmplify = true;
        }
    }
}