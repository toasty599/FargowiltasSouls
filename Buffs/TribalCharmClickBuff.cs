using System;
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Buffs
{
    public class TribalCharmClickBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Tribally Charming");
            Description.SetDefault("Gain an instant of increased damage when you click");
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
        }
    }
}