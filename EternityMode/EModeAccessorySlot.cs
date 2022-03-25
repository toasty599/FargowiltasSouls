using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.EternityMode
{
    public class EModeAccessorySlot : ModAccessorySlot
    {
        public override bool IsEnabled()
        {
            return Player.GetModPlayer<FargoSoulsPlayer>().ProofOfMastery;
        }
    }
}
