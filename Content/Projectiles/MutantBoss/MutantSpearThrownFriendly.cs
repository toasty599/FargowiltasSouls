namespace FargowiltasSouls.Content.Projectiles.MutantBoss
{
    public class MutantSpearThrownFriendly : FargowiltasSouls.Content.Projectiles.BossWeapons.HentaiSpearThrown
    {
        public override string Texture => "FargowiltasSouls/Content/Projectiles/BossWeapons/HentaiSpear";

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.DamageType = Terraria.ModLoader.DamageClass.Default;
        }
    }
}