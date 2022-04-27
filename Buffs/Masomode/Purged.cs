using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Buffs.Masomode
{
    public class Purged : ModBuff
    {
        public override string Texture => "FargowiltasSouls/Buffs/PlaceholderDebuff";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Purged");
            Description.SetDefault("Dodge effects don't work");
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;
            BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.GetModPlayer<FargoSoulsPlayer>().noDodge = true;
        }
    }
}