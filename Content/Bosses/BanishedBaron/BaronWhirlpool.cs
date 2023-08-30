using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Color = Microsoft.Xna.Framework.Color;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace FargowiltasSouls.Content.Bosses.BanishedBaron
{

    public class BaronWhirlpool : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Banished Baron's Mine");
            Main.projFrames[Type] = 16;
        }
        public override void SetDefaults()
        {
            Projectile.width = 186;
            Projectile.height = 48;
            Projectile.aiStyle = -1;
            Projectile.hostile = true;
            Projectile.penetrate = 1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.scale = 1f;
            Projectile.light = 1;
            Projectile.alpha = 255;
        }
        public bool Fade;
        public bool Animate;
        public Projectile Child;
        public override void AI()
        {
            ref float ParentID = ref Projectile.ai[0];
            ref float Number = ref Projectile.ai[1];
            ref float Timer = ref Projectile.localAI[0];

            if (Number == BanishedBaron.MaxWhirlpools)
            {
                NPC parent = Main.npc[(int)ParentID];
                const int BaronHeight = 132 / 2;
                if (parent != null && parent.type == ModContent.NPCType<BanishedBaron>())
                {
                    Projectile.Center = parent.Center + (Vector2.UnitY * ((Projectile.height / 2) + BaronHeight));
                }
                if (Timer > 60 * 4)
                {
                    Fade = true;
                }
                Animate = true;
            }
            else
            {
                Projectile parent = Main.projectile[(int)ParentID];
                if (parent != null && parent.type == Type && !Animate)
                {
                    Projectile.Center = parent.Center + Vector2.UnitY * Projectile.height;
                    int frame = Main.projectile[(int)ParentID].frame - 1;
                    if (frame < 0 || frame >= Main.projFrames[Type])
                    {
                        frame = Main.projFrames[Type] - 1;
                    }
                    Projectile.frame = frame; //makes it look very good and connected
                }
                else
                {
                    Animate = true;
                    Fade = true;
                }
            }
            const int projTime = 50;
            if (Projectile.alpha == 0 && Main.netMode != NetmodeID.MultiplayerClient)
            {
                int everyThird = (int)(Number % 3);
                if (Timer % projTime == 0 && Timer % (projTime * 3) == projTime * everyThird)
                {
                    for (int i = -1; i < 2; i += 2)
                    {
                        Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center + Vector2.UnitX * i * Projectile.width * Main.rand.NextFloat(0.2f, 0.35f) + Vector2.UnitY * Main.rand.Next(-Projectile.height / 4, Projectile.height / 4), (Vector2.UnitX * i).RotatedBy(Main.rand.NextFloat(MathHelper.Pi / 24)), ModContent.ProjectileType<BaronWhirlpoolBolt>(), (int)(Projectile.damage * 0.8f), Projectile.knockBack, Projectile.owner, 1);
                    }
                }
            }
            if (Timer < 20)
            {
                Projectile.alpha -= 17;
                if (Projectile.alpha <= 0)
                {
                    Projectile.alpha = 0;
                }
            }
            bool collision = Collision.SolidCollision(Projectile.position, Projectile.width, Projectile.height);
            if (Timer == 8 && Number > 0 && !collision)
            {
                Child = Projectile.NewProjectileDirect(Projectile.InheritSource(Projectile), Projectile.Center + Vector2.UnitY * Projectile.height, Vector2.Zero, Type, Projectile.damage, Projectile.knockBack, Projectile.owner, Projectile.whoAmI, Number - 1);
            }
            if (Fade)
            {
                Projectile.alpha += 17;
                if (Projectile.alpha >= 238)
                {
                    if (Child != null)
                    {
                        (Child.ModProjectile as BaronWhirlpool).Fade = true;
                        (Child.ModProjectile as BaronWhirlpool).Animate = true;
                    }
                    Projectile.Kill();
                }
            }

            if (Animate)
            {
                if (++Projectile.frameCounter > 2)
                {
                    if (++Projectile.frame >= Main.projFrames[Type])
                    {
                        Projectile.frame = 0;
                    }
                    Projectile.frameCounter = 0;
                }
            }
            Timer++;

        }
    }
}