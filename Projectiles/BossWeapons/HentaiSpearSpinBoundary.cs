using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using FargowiltasSouls.Buffs.Masomode;

namespace FargowiltasSouls.Projectiles.BossWeapons
{
    public class HentaiSpearSpinBoundary : HentaiSpearSpin
    {
        public override string Texture => "FargowiltasSouls/Projectiles/BossWeapons/HentaiSpear";

        public override void SetDefaults()
        {
            base.SetDefaults();
            projectile.melee = false;
            projectile.ranged = true;
        }

        public override void AI()
        {
            //dust!
            int dustId = Dust.NewDust(projectile.position, projectile.width, projectile.height, 15, projectile.velocity.X * 0.2f,
                projectile.velocity.Y * 0.2f, 100, default(Color), 2f);
            Main.dust[dustId].noGravity = true;
            int dustId3 = Dust.NewDust(projectile.position, projectile.width, projectile.height, 15, projectile.velocity.X * 0.2f,
                projectile.velocity.Y * 0.2f, 100, default(Color), 2f);
            Main.dust[dustId3].noGravity = true;

            Player player = Main.player[projectile.owner];
            if (projectile.owner == Main.myPlayer && (!player.controlUseTile || player.altFunctionUse != 2 || (player.controlUp && player.controlDown)))
            {
                projectile.Kill();
                return;
            }

            if (player.dead || !player.active)
            {
                projectile.Kill();
                return;
            }

            player.velocity *= 0.9f; //move slower while holding it

            Vector2 ownerMountedCenter = player.RotatedRelativePoint(player.MountedCenter);
            projectile.direction = player.direction;
            player.heldProj = projectile.whoAmI;
            player.itemTime = 2; //15;
            player.itemAnimation = 2; //15;
            //player.itemAnimationMax = 15;
            projectile.Center = ownerMountedCenter;
            projectile.timeLeft = 2;

            projectile.rotation = (float)Math.Atan2(projectile.velocity.Y, projectile.velocity.X);
            projectile.rotation += (float)Math.PI / 6.85f * player.direction;
            projectile.ai[0] += MathHelper.Pi / 45;
            projectile.velocity = projectile.rotation.ToRotationVector2();
            projectile.position -= projectile.velocity;
            player.itemRotation = projectile.rotation;
            player.itemRotation = MathHelper.WrapAngle(player.itemRotation);

            if (++projectile.localAI[0] > 2)
            {
                Main.PlaySound(SoundID.Item12, projectile.Center);
                projectile.localAI[0] = 0;
                projectile.localAI[1] += (float)Math.PI / 4 / 360 * ++projectile.ai[1] * player.direction;
                if (projectile.localAI[1] > (float)Math.PI)
                    projectile.localAI[1] -= (float)Math.PI * 2;
                if (projectile.owner == Main.myPlayer)
                {
                    for (int i = 0; i < 6; i++)
                    {
                        Projectile.NewProjectile(player.Center, new Vector2(0, -9f).RotatedBy(projectile.localAI[1] + Math.PI / 3 * i),
                            ModContent.ProjectileType<PhantasmalEyeBoundary>(), projectile.damage, projectile.knockBack / 2, projectile.owner);
                    }
                }
            }
        }
    }
}