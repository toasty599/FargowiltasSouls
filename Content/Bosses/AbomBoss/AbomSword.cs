using FargowiltasSouls.Content.Projectiles.Deathrays;
using FargowiltasSouls.Core.Systems;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Bosses.AbomBoss
{
	public class AbomSword : AbomSpecialDeathray
    {
        public AbomSword() : base(300) { }

        public int counter;
        public bool spawnedHandle;

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            // DisplayName.SetDefault("Styx Gazer Blade");
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.FargoSouls().DeletionImmuneRank = 2;
            Projectile.extraUpdates = 1;
            Projectile.netImportant = true;
        }
        public override void AI()
        {
            base.AI();

            Vector2? vector78 = null;
            if (Projectile.velocity.HasNaNs() || Projectile.velocity == Vector2.Zero)
            {
                Projectile.velocity = -Vector2.UnitY;
            }
            NPC abom = FargoSoulsUtil.NPCExists(Projectile.ai[1], ModContent.NPCType<AbomBoss>());
            if (abom == null)
            {
                Projectile.Kill();
                return;
            }
            else
            {
                Projectile.Center = abom.Center;
            }
            if (Projectile.velocity.HasNaNs() || Projectile.velocity == Vector2.Zero)
            {
                Projectile.velocity = -Vector2.UnitY;
            }
            if (Projectile.localAI[0] == 0f)
            {
                SoundEngine.PlaySound(SoundID.Zombie104, Projectile.Center);
            }
            float num801 = 1f;
            Projectile.localAI[0] += 1f;
            if (Projectile.localAI[0] >= maxTime)
            {
                Projectile.Kill();
                return;
            }
            Projectile.scale = (float)Math.Sin(Projectile.localAI[0] * 3.14159274f / maxTime) * num801 * 6f;
            if (Projectile.scale > num801)
            {
                Projectile.scale = num801;
            }
            float num804 = Projectile.velocity.ToRotation();
            if ((abom.velocity != Vector2.Zero || abom.ai[0] == 19) && abom.ai[0] != 20)
                num804 += Projectile.ai[0] / Projectile.MaxUpdates;
            Projectile.rotation = num804 - 1.57079637f;
            Projectile.velocity = num804.ToRotationVector2();
            float num805 = 3f;
            float num806 = Projectile.width;
            Vector2 samplingPoint = Projectile.Center;
            if (vector78.HasValue)
            {
                samplingPoint = vector78.Value;
            }
            float[] array3 = new float[(int)num805];
            //Collision.LaserScan(samplingPoint, Projectile.velocity, num806 * Projectile.scale, 3000f, array3);
            for (int i = 0; i < array3.Length; i++)
                array3[i] = 3000f;
            float num807 = 0f;
            int num3;
            for (int num808 = 0; num808 < array3.Length; num808 = num3 + 1)
            {
                num807 += array3[num808];
                num3 = num808;
            }
            num807 /= num805;
            float amount = 0.5f;
            Projectile.localAI[1] = MathHelper.Lerp(Projectile.localAI[1], num807, amount);
            if (Projectile.localAI[0] % 2 == 0)
            {
                /*Vector2 vector79 = Projectile.Center + Projectile.velocity * (Projectile.localAI[1] - 14f);
                for (int num809 = 0; num809 < 2; num809 = num3 + 1)
                {
                    float num810 = Projectile.velocity.ToRotation() + ((Main.rand.Next(2) == 1) ? -1f : 1f) * 1.57079637f;
                    float num811 = (float)Main.rand.NextDouble() * 2f + 2f;
                    Vector2 vector80 = new Vector2((float)Math.Cos((double)num810) * num811, (float)Math.Sin((double)num810) * num811);
                    int num812 = Dust.NewDust(vector79, 0, 0, 244, vector80.X, vector80.Y, 0, default(Color), 1f);
                    Main.dust[num812].noGravity = true;
                    Main.dust[num812].scale = 1.7f;
                    num3 = num809;
                }
                if (Main.rand.NextBool(5))
                {
                    Vector2 value29 = Projectile.velocity.RotatedBy(1.5707963705062866, default(Vector2)) * ((float)Main.rand.NextDouble() - 0.5f) * (float)Projectile.width;
                    int num813 = Dust.NewDust(vector79 + value29 - Vector2.One * 4f, 8, 8, 244, 0f, 0f, 100, default(Color), 1.5f);
                    Dust dust = Main.dust[num813];
                    dust.velocity *= 0.5f;
                    Main.dust[num813].velocity.Y = -Math.Abs(Main.dust[num813].velocity.Y);
                }*/
                //DelegateMethods.v3_1 = new Vector3(0.3f, 0.65f, 0.7f);
                //Utils.PlotTileLine(Projectile.Center, Projectile.Center + Projectile.velocity * Projectile.localAI[1], (float)Projectile.width * Projectile.scale, new Utils.PerLinePoint(DelegateMethods.CastLight));

                if (abom.velocity != Vector2.Zero && --counter < 0)
                {
                    counter = 5;
                    if (FargoSoulsUtil.HostCheck) //spawn bonus projs
                    {
                        Vector2 spawnPos = Projectile.Center;
                        Vector2 vel = Projectile.velocity.RotatedBy(Math.PI / 2 * Math.Sign(Projectile.ai[0]));
                        const int max = 15;
                        for (int i = 1; i <= max; i++)
                        {
                            spawnPos += Projectile.velocity * 3000f / max;
                            Projectile.NewProjectile(Terraria.Entity.InheritSource(Projectile), spawnPos, vel, ModContent.ProjectileType<AbomSickle2>(), Projectile.damage, 0f, Projectile.owner);
                        }
                    }
                }

                for (int i = 0; i < 2; i++)
                {
                    int d = Dust.NewDust(Projectile.position + Projectile.velocity * Main.rand.NextFloat(3000), Projectile.width, Projectile.height, DustID.GemTopaz, 0f, 0f, 0, Color.White, 6f);
                    Main.dust[d].noGravity = true;
                    Main.dust[d].velocity *= 4f;
                }
            }

            if (!spawnedHandle)
            {
                spawnedHandle = true;
                if (FargoSoulsUtil.HostCheck)
                {
                    Projectile.NewProjectile(Terraria.Entity.InheritSource(Projectile), Projectile.Center, Projectile.velocity, ModContent.ProjectileType<AbomSwordHandle>(), Projectile.damage, Projectile.knockBack, Projectile.owner, (float)Math.PI / 2, Projectile.identity);
                    Projectile.NewProjectile(Terraria.Entity.InheritSource(Projectile), Projectile.Center, Projectile.velocity, ModContent.ProjectileType<AbomSwordHandle>(), Projectile.damage, Projectile.knockBack, Projectile.owner, -(float)Math.PI / 2, Projectile.identity);
                }
            }
        }
        //attempted visual rework. does some fire shit. idk
        /*
        const int FireTextures = 1000;
        Vector2[] FirePositions = new Vector2[FireTextures];
        int[] FireFrames = new int[FireTextures];
        float[] FireRotations = new float[FireTextures];
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D fireTexture = ModContent.Request<Texture2D>("FargowiltasSouls/Content/Bosses/AbomBoss/AbomSwordFire", AssetRequestMode.ImmediateLoad).Value;
            const int FrameCount = 5;

            float rayLength = Projectile.localAI[1];
            float rayWidth = 22f * Projectile.scale * hitboxModifier / 2;
            int travelFrames = 80;

            Vector2 endPos = Projectile.Center + Projectile.velocity * rayLength;
            int allowedNewFires = FireTextures / travelFrames;
            int newFires = 0;
            for (int i = 0; i < FireTextures; i++)
            {
                bool newFire = FirePositions[i].X > rayLength || FirePositions[i].X == 0 || FireFrames[i] == 0;
                
                if (newFire)
                {
                    if (newFires >= allowedNewFires)
                        continue;

                    FireFrames[i] = Main.rand.Next(FrameCount) + 1;
                    float fireSpawnWidth = (rayWidth / 2) - fireTexture.Width / 3;
                    FirePositions[i].X = Main.rand.NextFloat(0, rayLength / travelFrames) / 2;
                    FirePositions[i].Y = Main.rand.NextFloat(-fireSpawnWidth, fireSpawnWidth) / 2;
                    FireRotations[i] = Main.rand.NextFloat(MathHelper.TwoPi);
                    newFires++;
                }

                FirePositions[i].X += rayLength * (1f / travelFrames);
                int rotationDirection = (rayWidth / 2) > FirePositions[i].Y ? 1 : -1;
                FireRotations[i] += rotationDirection * MathHelper.TwoPi / 20f;

                Color color;
                float lengthProgress = FirePositions[i].X / rayLength;
                float widthProgress = Math.Abs(FirePositions[i].Y - (rayWidth / 2)) / rayWidth;

                const float lerp1 = 0.2f;
                const float lerp2 = 0.5f;
                if (lengthProgress < lerp1)
                {
                    float Ylerp = lengthProgress / lerp1;
                    color = Color.Lerp(Color.White, Color.Orange, Ylerp);
                }
                else if (lengthProgress < lerp2)
                {
                    float Ylerp = (lengthProgress - lerp1) / lerp2;
                    color = Color.Lerp(Color.Orange, Color.Red, Ylerp);
                }
                else
                {
                    float Ylerp = (lengthProgress - lerp1 - lerp2) / 1f;
                    color = Color.Lerp(Color.Red, Color.DarkRed, Ylerp);
                }
                float Xlerp = widthProgress / 3f;
                color = Color.Lerp(color, Color.DarkRed, Xlerp);

                Vector2 dir = Vector2.Normalize(Projectile.velocity);
                Vector2 drawPos = Projectile.Center - Main.screenPosition + FirePositions[i].RotatedBy(dir.ToRotation());
                int frameHeight = fireTexture.Height / FrameCount;
                Rectangle rectangle = new(0, (FireFrames[i] - 1) * frameHeight, fireTexture.Width, frameHeight);
                Main.EntitySpriteDraw(fireTexture, drawPos, rectangle, color, FireRotations[i], fireTexture.Size() / 2, Projectile.scale, SpriteEffects.None);

            }
            return false;
        }
        */
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.velocity.X = target.Center.X < Main.npc[(int)Projectile.ai[1]].Center.X ? -15f : 15f;
            target.velocity.Y = -10f;

            Projectile.NewProjectile(Terraria.Entity.InheritSource(Projectile), target.Center + Main.rand.NextVector2Circular(100, 100), Vector2.Zero, ModContent.ProjectileType<AbomBlast>(), 0, 0f, Projectile.owner);

            if (WorldSavingSystem.EternityMode)
            {
                target.AddBuff(ModContent.BuffType<Buffs.Boss.AbomFangBuff>(), 300);
                target.AddBuff(BuffID.Burning, 180);
            }
            target.AddBuff(BuffID.WitheredArmor, 600);
            target.AddBuff(BuffID.WitheredWeapon, 600);
        }
    }
}