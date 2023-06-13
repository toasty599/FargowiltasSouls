using FargowiltasSouls.Buffs.Boss;
using FargowiltasSouls.Buffs.Masomode;
using FargowiltasSouls.Content.NPCs;
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Masomode
{
    public class MoonLordMoonBlast : MoonLordSunBlast
    {
        public override string Texture => "Terraria/Images/Projectile_645";

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            DisplayName.SetDefault("Moon Blast");
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.AddBuff(ModContent.BuffType<CurseoftheMoon>(), 360);
            if (FargoSoulsUtil.BossIsAlive(ref EModeGlobalNPC.mutantBoss, ModContent.NPCType<NPCs.MutantBoss.MutantBoss>()))
            {
                target.GetModPlayer<FargoSoulsPlayer>().MaxLifeReduction += 100;
                target.AddBuff(ModContent.BuffType<OceanicMaul>(), 5400);
                target.AddBuff(ModContent.BuffType<MutantFang>(), 180);
            }
        }
    }
}

