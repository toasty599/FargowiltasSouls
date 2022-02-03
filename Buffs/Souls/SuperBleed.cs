using FargowiltasSouls.NPCs;
using Terraria;
using Terraria.ModLoader;
using Terraria.Localization;

namespace FargowiltasSouls.Buffs.Souls
{
    public class SuperBleed : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Blood Geyser");
            //Description.SetDefault("Spewing blood in self defense");
            Main.buffNoSave[Type] = true;
            DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "大出血");
            //Description.AddTranslation((int)GameCulture.CultureName.Chinese, "出于自卫而喷发出的血");
        }

        public override string Texture => "FargowiltasSouls/Buffs/PlaceholderBuff";

        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.GetGlobalNPC<FargoSoulsGlobalNPC>().SBleed = true;
        }
    }
}