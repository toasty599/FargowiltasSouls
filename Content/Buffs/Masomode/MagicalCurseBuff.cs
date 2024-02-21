using FargowiltasSouls.Core.Globals;
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Buffs.Masomode
{
    public class MagicalCurseBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.FargoSouls().MagicalCurse = true;
        }
    }
}