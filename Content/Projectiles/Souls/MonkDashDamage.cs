using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.Souls
{
    public class MonkDashDamage : ModProjectile
    {

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 2;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;

        }
        public override void SetDefaults()
        {
            Player player = Main.player[Projectile.owner];

            Projectile.DamageType = DamageClass.Generic;
            Projectile.friendly = true;
            Projectile.width = player.width;
            Projectile.height = player.height;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 30;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 30;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            if (player == null || !player.active || player.dead)
            {
                Projectile.Kill();
            }
            Projectile.Center = player.Center;
            
        }

        public override string Texture => "FargowiltasSouls/Content/Projectiles/Empty";

        /*public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float collisionPoint = 0f;
            if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.position, Projectile.oldPos[1], Projectile.width, ref collisionPoint))
            {
                return true;
            }
            return false;
        }*/
        
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.damage = (int)(Math.Round(Projectile.damage * 0.8));
            //CHANGE THIS SOUND EFFECT!!!!!!!!!!!!!!!!!!!!!! TOO MUCH RAINBOW GUN
            SoundEngine.PlaySound(SoundID.Item68, target.Center);

        }

    }
}
