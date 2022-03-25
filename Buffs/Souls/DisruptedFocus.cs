using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Buffs.Souls
{
    public class DisruptedFocus : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Disrupted Focus");
            Description.SetDefault("Weapon speed reduced");
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
            
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.GetModPlayer<FargoSoulsPlayer>().DisruptedFocus = true;
        }
    }
}