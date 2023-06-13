namespace FargowiltasSouls.Content.Projectiles.Champions
{
    public class SpiritCrossBoneReverse : SpiritCrossBone
    {
        public override string Texture => "Terraria/Images/Projectile_532";

        public override void AI()
        {
            base.AI();
            Projectile.velocity.Y -= 0.2f * 2; //reverse gravity
        }
    }
}