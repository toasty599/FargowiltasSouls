namespace FargowiltasSouls.Projectiles.Champions
{
    public class SpiritCrossBoneReverse : SpiritCrossBone
    {
        public override string Texture => "Terraria/Projectile_532";

        public override void AI()
        {
            base.AI();
            projectile.velocity.Y -= 0.2f * 2; //reverse gravity
        }
    }
}