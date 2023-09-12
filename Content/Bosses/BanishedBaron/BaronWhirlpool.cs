using FargowiltasSouls.Content.Buffs.Masomode;
using FargowiltasSouls.Core.Systems;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

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

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            if (!WorldSavingSystem.EternityMode)
            {
                return;
            }
            target.GetModPlayer<FargoSoulsPlayer>().MaxLifeReduction += 50;
            target.AddBuff(ModContent.BuffType<OceanicMaulBuff>(), 60 * 20);
            target.AddBuff(BuffID.Rabies, 60 * 10);
        }
        public override void AI()
        {
            ref float ParentID = ref Projectile.ai[0];
            ref float Number = ref Projectile.ai[1];
            ref float Timer = ref Projectile.localAI[0];
            ref float ChildID = ref Projectile.ai[2];

            Projectile.netUpdate = true; //it's choppy if this isn't done always

            if (Number == BanishedBaron.MaxWhirlpools)
            {
                NPC parent = Main.npc[(int)ParentID];
                const int BaronHeight = 132 / 2;
                if (parent.active && parent.type == ModContent.NPCType<BanishedBaron>())
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
                if (parent.active && parent.type == Type && !Animate)
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
            if (Projectile.alpha == 0)
            {
                int everyThird = (int)(Number % 3);
                if (Timer % projTime == 0 && Timer % (projTime * 3) == projTime * everyThird)
                {
                    SoundEngine.PlaySound(SoundID.Item21, Projectile.Center);
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        for (int i = -1; i < 2; i += 2)
                        {
                            Projectile.NewProjectile(Terraria.Entity.InheritSource(Projectile), Projectile.Center + Vector2.UnitX * i * Projectile.width * Main.rand.NextFloat(0.2f, 0.35f) + Vector2.UnitY * Main.rand.Next(-Projectile.height / 4, Projectile.height / 4), (Vector2.UnitX * i).RotatedBy(Main.rand.NextFloat(MathHelper.Pi / 24)), ModContent.ProjectileType<BaronWhirlpoolBolt>(), (int)(Projectile.damage * 0.8f), Projectile.knockBack, Projectile.owner, 1);
                        }
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
            bool collision = WorldSavingSystem.MasochistModeReal ? false : Collision.SolidCollision(Projectile.position, Projectile.width, Projectile.height); //no collision check in maso
            if (collision && Number > 3)
            {
                Number = 3;
            }
            if (Timer == 8 && Number > 0 && Main.netMode != NetmodeID.MultiplayerClient)
            {
                ChildID = Projectile.NewProjectile(Terraria.Entity.InheritSource(Projectile), Projectile.Center + Vector2.UnitY * Projectile.height, Vector2.Zero, Type, Projectile.damage, Projectile.knockBack, Projectile.owner, Projectile.whoAmI, Number - 1);
            }
            if (Fade)
            {
                Projectile.alpha += 17;
                if (Projectile.alpha >= 238)
                {
                    Projectile child = Main.projectile[(int)ChildID];
                    if (child != null)
                    {
                        if (child.active && child.type == Type)
                        {
                            (child.ModProjectile as BaronWhirlpool).Fade = true;
                            (child.ModProjectile as BaronWhirlpool).Animate = true;
                        }
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