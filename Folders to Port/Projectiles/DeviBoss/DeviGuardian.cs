using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using FargowiltasSouls.NPCs;

namespace FargowiltasSouls.Projectiles.DeviBoss
{
    public class DeviGuardian : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_197";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Baby Guardian");
        }

        public override void SetDefaults()
        {
            projectile.width = 42;
            projectile.height = 42;
            projectile.penetrate = -1;
            projectile.hostile = true;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.aiStyle = -1;
            CooldownSlot = 1;

            projectile.timeLeft = 600;
            projectile.hide = true;
        }

        public override void AI()
        {
            if (projectile.localAI[0] == 0)
            {
                projectile.localAI[0] = 1;
                projectile.rotation = Main.rand.NextFloat(0, 2 * (float)Math.PI);
                projectile.hide = false;

                Terraria.Audio.SoundEngine.PlaySound(SoundID.Item21, projectile.Center);

                for (int i = 0; i < 50; i++)
                {
                    Vector2 pos = new Vector2(projectile.Center.X + Main.rand.Next(-20, 20), projectile.Center.Y + Main.rand.Next(-20, 20));
                    int dust = Dust.NewDust(pos, projectile.width, projectile.height, DustID.Blood, 0, 0, 100, default(Color), 2f);
                    Main.dust[dust].noGravity = true;
                }
            }

            projectile.direction = projectile.velocity.X < 0 ? -1 : 1;
            projectile.rotation += projectile.direction * .3f;
        }

        public override void Kill(int timeLeft)
        {
            for (int i = 0; i < 50; i++)
            {
                Vector2 pos = new Vector2(projectile.Center.X + Main.rand.Next(-20, 20), projectile.Center.Y + Main.rand.Next(-20, 20));
                int dust = Dust.NewDust(pos, projectile.width, projectile.height, DustID.Blood, 0, 0, 100, default(Color), 2f);
                Main.dust[dust].noGravity = true;
            }
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.AddBuff(ModContent.BuffType<Defenseless>(), 300);
            target.AddBuff(ModContent.BuffType<Lethargic>(), 300);
            if (FargoSoulsUtil.BossIsAlive(ref EModeGlobalNPC.guardBoss, NPCID.DungeonGuardian))
                target.AddBuff(ModContent.BuffType<MarkedForDeath>(), 300);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            int num156 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new Rectangle(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;
            Main.EntitySpriteDraw(texture2D13, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), projectile.GetAlpha(lightColor), projectile.rotation, origin2, projectile.scale, SpriteEffects.None, 0);
            return false;
        }
    }
}