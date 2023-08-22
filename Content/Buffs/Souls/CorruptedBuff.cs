using FargowiltasSouls.Core.Globals;
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Buffs.Souls
{
    public class CorruptedBuff : ModBuff
    {

        public override string Texture => "FargowiltasSouls/Content/Buffs/PlaceholderBuff";

        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.buffTime[buffIndex] = 60 * 60;
            npc.GetGlobalNPC<FargoSoulsGlobalNPC>().Corrupted = true;
        }
    }
}