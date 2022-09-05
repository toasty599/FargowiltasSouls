using FargowiltasSouls.Buffs.Masomode;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace FargowiltasSouls.Buffs
{
    public class BerserkerInstall : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Berserker Install");
            Description.SetDefault("'Get them! Berserker!'");
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;
            Terraria.ID.BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.GetModPlayer<FargoSoulsPlayer>().AttackSpeed += 0.50f;
            player.GetModPlayer<FargoSoulsPlayer>().Berserked = true;
            player.moveSpeed += 0.30f;
            player.endurance -= 0.30f;

            player.hasMagiluminescence = true;
            player.noKnockback = true;
            
            player.GetDamage(DamageClass.Generic) += 0.30f;
            player.GetCritChance(DamageClass.Generic) += 30;
            player.statDefense -= 30;

            if (!player.controlLeft && !player.controlRight)
            {
                if (player.velocity.X > 0)
                    player.controlRight = true;
                else if (player.velocity.X < 0)
                    player.controlLeft = true;
                else if (player.direction > 0)
                    player.controlRight = true;
                else
                    player.controlLeft = true;
            }

            if (player.buffTime[buffIndex] > 2)
                player.GetModPlayer<FargoSoulsPlayer>().NoMomentum = true;

            if (player.buffTime[buffIndex] == 2)
            {
                //player.AddBuff(ModContent.BuffType<BerserkerInstallCD>(), 420);
                player.AddBuff(ModContent.BuffType<Stunned>(), 120);
            }
        }
    }
}