using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Minions
{
    public class SuperFlocko : ModProjectile
    {
        public override string Texture => "Terraria/Images/NPC_352";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Super Flocko");
            Main.projFrames[Projectile.type] = 6;
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
            //ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.netImportant = true;
            Projectile.width = 54;
            Projectile.height = 54;
            Projectile.timeLeft *= 5;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.minion = true;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            if (player.active && !player.dead && player.GetModPlayer<FargoSoulsPlayer>().SuperFlocko)
                Projectile.timeLeft = 2;

            NPC minionAttackTargetNpc = Projectile.OwnerMinionAttackTargetNPC;
            if (minionAttackTargetNpc != null && Projectile.ai[0] != minionAttackTargetNpc.whoAmI && minionAttackTargetNpc.CanBeChasedBy(Projectile))
            {
                Projectile.ai[0] = minionAttackTargetNpc.whoAmI;
                Projectile.netUpdate = true;
            }

            if (Projectile.ai[0] >= 0 && Projectile.ai[0] < Main.maxNPCs) //has target
            {
                NPC npc = Main.npc[(int)Projectile.ai[0]];

                if (npc.CanBeChasedBy(Projectile))
                {
                    Vector2 distance = npc.Center - Projectile.Center;
                    float length = distance.Length();
                    if (length > 100f)
                    {
                        distance /= 8f;
                        Projectile.velocity = (Projectile.velocity * 23f + distance) / 24f;
                    }
                    else
                    {
                        if (Projectile.velocity.Length() < 12f)
                            Projectile.velocity *= 1.05f;
                    }

                    Projectile.localAI[0]++;
                    if (Projectile.localAI[0] > 45)
                    {
                        Projectile.localAI[0] = 0f;
                        if (Projectile.owner == Main.myPlayer)
                        {
                            Vector2 vel = distance;
                            vel.Normalize();
                            vel *= 9f;
                            FargoSoulsUtil.NewSummonProjectile(Projectile.GetSource_FromThis(), Projectile.Center, vel, ModContent.ProjectileType<FrostWave>(),
                                Projectile.originalDamage, Projectile.knockBack, Projectile.owner);
                        }
                    }

                    Projectile.localAI[1]++;
                    if (Projectile.localAI[1] > 6)
                    {
                        Projectile.localAI[1] = 0f;
                        if (Projectile.owner == Main.myPlayer)
                        {
                            Vector2 speed = new Vector2(Main.rand.Next(-1000, 1001), Main.rand.Next(-1000, 1001));
                            speed.Normalize();
                            speed *= 12f;
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                                FargoSoulsUtil.NewSummonProjectile(Projectile.GetSource_FromThis(), Projectile.Center + speed * 4f, speed, ModContent.ProjectileType<FrostShard>(),
                                    Projectile.originalDamage / 2, Projectile.knockBack / 2, Projectile.owner);
                        }
                    }
                }
                else //forget target
                {
                    Projectile.ai[0] = -1f;
                    Projectile.netUpdate = true;
                }
            }
            else //no target
            {
                Vector2 distance = player.Center - Projectile.Center;
                float length = distance.Length();
                if (length > 1500f)
                {
                    Projectile.Center = player.Center;
                    Projectile.velocity = Vector2.UnitX.RotatedByRandom(2 * Math.PI) * 12f;
                }
                else if (length > 100f)
                {
                    distance /= 12f;
                    Projectile.velocity = (Projectile.velocity * 23f + distance) / 24f;
                }
                else
                {
                    if (Projectile.velocity.Length() < 12f)
                        Projectile.velocity *= 1.05f;
                }

                Projectile.localAI[1]++;
                if (Projectile.localAI[1] > 6f)
                {
                    Projectile.localAI[1] = 0f;
                    Projectile.ai[0] = FargoSoulsUtil.FindClosestHostileNPCPrioritizingMinionFocus(Projectile, 1000f);
                    if (Projectile.ai[0] != -1)
                        Projectile.netUpdate = true;
                }
            }

            //Projectile.rotation = Projectile.velocity.ToRotation();
            Projectile.rotation += Projectile.velocity.Length() / 12f * (Projectile.velocity.X > 0 ? -0.2f : 0.2f);
            if (++Projectile.frameCounter > 3)
            {
                if (++Projectile.frame >= 6)
                    Projectile.frame = 0;
                Projectile.frameCounter = 0;
            }
        }

        public override bool? CanCutTiles()
        {
            return false;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.Frostburn, 360);
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, 255, 255, 200) * Projectile.Opacity;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            int num156 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new Rectangle(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;
            Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Projectile.GetAlpha(lightColor), Projectile.rotation, origin2, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
    }
}