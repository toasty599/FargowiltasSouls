using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Buffs.Minions
{
    public class CrystalSkull : ModBuff
    {
        public override string Texture => "FargowiltasSouls/Buffs/PlaceholderBuff";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Crystal Skull");
            Description.SetDefault("The pungent eyeball will protect you");
            Main.buffNoTimeDisplay[Type] = true;
            Main.buffNoSave[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.GetModPlayer<FargoSoulsPlayer>().CrystalSkullMinion = true;
            if (player.whoAmI == Main.myPlayer && player.ownedProjectileCounts[ModContent.ProjectileType<FargowiltasSouls.Content.Projectiles.Minions.CrystalSkull>()] < 1)
                FargoSoulsUtil.NewSummonProjectile(player.GetSource_Buff(buffIndex), player.Center, Vector2.Zero, ModContent.ProjectileType<FargowiltasSouls.Content.Projectiles.Minions.CrystalSkull>(), 30, 4f, player.whoAmI);
        }
    }
}