using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Buffs.Souls
{
    public class TitaniumDRBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Titanium Shield");
            Description.SetDefault("You have extra damage resistance");
            Main.buffNoSave[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            FargoSoulsPlayer modPlayer = player.GetModPlayer<FargoSoulsPlayer>();

            if (modPlayer.EarthForce)
            {
                player.endurance = 0.75f;
            }
            else
            {
                player.endurance = 0.95f;
            }

            //kill all shards before running out
            if (player.buffTime[buffIndex] == 2)
            {
                for (int i = 0; i < Main.maxProjectiles; i++)
                {
                    Projectile proj = Main.projectile[i];

                    if (proj.active && proj.type == ProjectileID.TitaniumStormShard && proj.owner == player.whoAmI)
                    {
                        proj.Kill();
                    }
                }

                player.AddBuff(ModContent.BuffType<TitaniumCD>(), 600);
            }
        }
    }
}
