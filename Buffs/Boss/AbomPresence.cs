using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Buffs.Boss
{
    public class AbomPresence : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Abominable Presence");
            Description.SetDefault("Defense, damage reduction, and life regen reduced; Moon Leech effect");
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
            longerExpertDebuff = false;
            canBeCleared = false;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.GetModPlayer<FargoPlayer>().noDodge = true;
            player.GetModPlayer<FargoPlayer>().noSupersonic = true;
            player.moonLeech = true;
            player.bleed = true;

            player.statDefense -= 20;
            player.endurance -= 0.2f;
        }
    }
}