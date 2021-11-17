using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Terraria;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;

namespace FargowiltasSouls.Patreon.Purified
{
    public class PrimeMinionProj : ModProjectile
    {
        private Vector2 mousePos;
        public override void SetStaticDefaults()
        {
            Main.projFrames[projectile.type] = 7;
            ProjectileID.Sets.MinionTargettingFeature[projectile.type] = true;
        }
        public override void SetDefaults()
        {
            projectile.width = 34;
            projectile.height = 38;
            projectile.aiStyle = -1;
            projectile.penetrate = -1;
            projectile.friendly = true;
            projectile.minion = true;
            projectile.netImportant = true;
            //projectile.minionSlots = 1;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.timeLeft = 18000;
            projectile.GetGlobalProjectile<Projectiles.FargoGlobalProjectile>().CanSplit = false;
            ProjectileID.Sets.Homing[projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[base.projectile.type] = true;
        }
        public override void AI()
        {
            Player player = Main.player[projectile.owner];
            if (player.whoAmI == Main.myPlayer)
            {
                mousePos = Main.MouseWorld;
            }
            PatreonPlayer patronPlayer = player.GetModPlayer<PatreonPlayer>();
            if (player.dead) patronPlayer.PrimeMinion = false;
            if (patronPlayer.PrimeMinion) projectile.timeLeft = 2;
           // projectile.alpha = 0;

            projectile.frameCounter++;
            if (projectile.frameCounter >= 7)
            {
                projectile.frameCounter = 0;
                projectile.frame = (projectile.frame + 1) % 7;
                Main.NewText(projectile.DirectionTo(mousePos).ToString());
            }
            if (projectile.Distance(mousePos) > 50)
            {
                //projectile.velocity = projectile.DirectionTo(Main.MouseWorld) * 8;
                projectile.velocity = Vector2.Lerp(projectile.velocity, (projectile.DirectionTo(mousePos) * 10), 0.05f);
            }
            else if (projectile.Distance(mousePos) > 10)
            {
                projectile.velocity = Vector2.Zero;
            }
            //
            //projectile.velocity = Vector2.Lerp(projectile.velocity, (projectile.DirectionTo(mousePos) * 20), 0.05f);
            //projectile.alpha = (int)(Math.Cos(projectile.ai[0] * MathHelper.TwoPi / 180) * 122.5 + 122.5);
            // Main.NewText(projectile.DirectionTo(Main.MouseWorld).ToString());
            /*projectile.ai[0]++;
            if (projectile.ai[0] == 180)
            {

                projectile.Center = Main.MouseWorld;
                //projectile.velocity = projectile.DirectionTo(Main.MouseWorld) * 8;
                projectile.netUpdate = true;
                projectile.ai[0] = 0;
            }*/
        }
    }
}
    