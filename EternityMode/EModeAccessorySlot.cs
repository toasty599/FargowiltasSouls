using FargowiltasSouls.Core.Systems;
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.EternityMode
{
    public class EModeAccessorySlot : ModAccessorySlot
    {
        public override bool IsEnabled()
        {
            return WorldSavingSystem.EternityMode && Player.GetModPlayer<FargoSoulsPlayer>().MutantsPactSlot;
        }
    }
}
