namespace FargowiltasSouls.Projectiles.MutantBoss
{
    public class MutantSpearThrownFriendly : BossWeapons.HentaiSpearThrown
    {
        public override string Texture => "FargowiltasSouls/Projectiles/BossWeapons/HentaiSpear";

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.DamageType = Terraria.ModLoader.DamageClass.NoScaling;
        }
    }
}