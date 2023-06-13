using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Buffs.Masomode
{
    public class LoosePocketsBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Loose Pockets");
            Description.SetDefault("An item might be taken from you");
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
        }
    }
}
