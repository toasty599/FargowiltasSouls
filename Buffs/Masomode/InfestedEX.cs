using Terraria;
using Terraria.ModLoader;
using Terraria.Localization;

namespace FargowiltasSouls.Buffs.Masomode
{
    public class InfestedEX : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Infested EX");
            Description.SetDefault("This can only get worse");
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "感染");
            Description.AddTranslation((int)GameCulture.CultureName.Chinese, "这只会变得更糟");
        }

        public override void Update(Player player, ref int buffIndex)
        {
            FargoSoulsPlayer p = player.GetModPlayer<FargoSoulsPlayer>();

            player.ClearBuff(ModContent.BuffType<Infested>());

            p.MaxInfestTime = 2;
            p.FirstInfection = false;
            p.Infested = true;
        }
    }
}