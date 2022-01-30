using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.DeviBoss
{
    public class DeviRainHeart2 : DeviRainHeart
    {
        public override string Texture => "FargowiltasSouls/Projectiles/Masomode/FakeHeart";

        public override void Kill(int timeLeft)
        {
            base.Kill(timeLeft);

            NPC npc = FargoSoulsUtil.NPCExists(projectile.ai[1], ModContent.NPCType<NPCs.DeviBoss.DeviBoss>());
            if (npc != null)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Projectile.NewProjectile(projectile.Center, -Vector2.UnitY, ModContent.ProjectileType<DeviDeathray>(), projectile.damage, projectile.knockBack, projectile.owner);
                    if (Main.player[npc.target].Center.Y > projectile.Center.Y)
                        Projectile.NewProjectile(projectile.Center, Vector2.UnitY, ModContent.ProjectileType<DeviDeathray>(), projectile.damage, projectile.knockBack, projectile.owner);
                }
            }
        }
    }
}