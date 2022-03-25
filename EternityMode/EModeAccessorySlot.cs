using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.EternityMode
{
    public class EModeAccessorySlot : ModAccessorySlot
    {
        public override bool IsEnabled()
        {
            return FargoSoulsWorld.EternityMode && Player.GetModPlayer<FargoSoulsPlayer>().MutantsPactSlot;
        }
    }
}
