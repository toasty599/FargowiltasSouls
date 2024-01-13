using FargowiltasSouls.Content.Projectiles.BossWeapons;
using FargowiltasSouls.Content.Projectiles.Masomode;
using FargowiltasSouls.Content.Projectiles.Minions;
using FargowiltasSouls.Content.Projectiles.Souls;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using FargowiltasSouls.Common.Utilities;
using Terraria;
using Terraria.Audio;
using Terraria.GameInput;
using Terraria.Graphics.Capture;
using Terraria.ID;
using Terraria.ModLoader;
using FargowiltasSouls.Content.Projectiles;
using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using FargowiltasSouls.Content.Buffs;
using FargowiltasSouls.Content.Buffs.Souls;
using FargowiltasSouls.Content.Buffs.Masomode;
using FargowiltasSouls.Content.Projectiles.Deathrays;
using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Content.NPCs.EternityModeNPCs;
using FargowiltasSouls.Common.Graphics.Shaders;
using FargowiltasSouls.Core.Systems;
using Fargowiltas.Common.Configs;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Toggler;
using FargowiltasSouls.Content.Items.Accessories.Souls;

namespace FargowiltasSouls.Core.ModPlayers
{
	public partial class FargoSoulsPlayer
    {

        public void GoldKey()
        {
            if (!Player.HasBuff(ModContent.BuffType<GoldenStasisBuff>()) && !Player.HasBuff(ModContent.BuffType<GoldenStasisCDBuff>()))
            {
                int duration = 300;

                if (ForceEffect<GoldEnchant>())
                {
                    duration *= 2;
                }

                Player.AddBuff(ModContent.BuffType<GoldenStasisBuff>(), duration);
                Player.AddBuff(ModContent.BuffType<GoldenStasisCDBuff>(), 3600);

                goldHP = Player.statLife;

                if (!Main.dedServ)
                    SoundEngine.PlaySound(new SoundStyle("FargowiltasSouls/Assets/Sounds/Zhonyas"), Player.Center);
            }
            //cancel it early
            else
            {
                Player.ClearBuff(ModContent.BuffType<GoldenStasisBuff>());
            }
        }

        public void GoldUpdate()
        {
            Player.immune = true;
            Player.immuneTime = 90;
            Player.hurtCooldowns[0] = 90;
            Player.hurtCooldowns[1] = 90;
            Player.stealth = 1;

            //immune to DoT
            if (Player.statLife < goldHP)
                Player.statLife = goldHP;

            if (Player.ownedProjectileCounts[ModContent.ProjectileType<GoldShellProj>()] <= 0)
                Projectile.NewProjectile(Player.GetSource_Misc(""), Player.Center.X, Player.Center.Y, 0f, 0f, ModContent.ProjectileType<GoldShellProj>(), 0, 0, Main.myPlayer);
        }

        

        public bool CanJungleJump = false;
        public bool JungleJumping = false;
        public int savedRocketTime;


        private int GetNumSentries()
        {
            int count = 0;

            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile p = Main.projectile[i];

                if (p.active && p.owner == Player.whoAmI && p.sentry)
                {
                    count++;
                }
            }

            return count;
        }




        public void TurtleEffect(bool hideVisual)
        {
            Player.DisplayToggle("Turtle");

            Player.turtleThorns = true;
            Player.thorns = 1f;

            TurtleEnchantActive = true;


            if (Player.GetToggleValue("Turtle") && !Player.HasBuff(ModContent.BuffType<BrokenShellBuff>())
                && IsStandingStill && !Player.controlUseItem && Player.whoAmI == Main.myPlayer && !noDodge)
            {
                TurtleCounter++;

                if (TurtleCounter > 20)
                {
                    Player.AddBuff(ModContent.BuffType<ShellHideBuff>(), 2);
                }
            }
            else
            {
                TurtleCounter = 0;
            }
            
            if (TurtleShellHP < 20 && !Player.HasBuff(ModContent.BuffType<BrokenShellBuff>()) && !ShellHide && ForceEffect<TurtleEnchant>())
            {
                turtleRecoverCD--;
                if (turtleRecoverCD <= 0)
                {
                    turtleRecoverCD = 240;

                    TurtleShellHP++;
                }
            }

            //Main.NewText($"shell HP: {TurtleShellHP}, counter: {TurtleCounter}, recovery: {turtleRecoverCD}");
        }
       

        public int[] ApprenticeItemCDs = new int[10];

        //saved mount stats for squire
        //public Mount.MountData OriginalSquireMountData;
        public Mount.MountData BaseSquireMountData;

        public int BaseMountType = -1;


        //public float baseMountAccel;
        //public float baseMountRunSpeed;
        //public int baseMountJumpHeight;

        //        #endregion

        //        #region souls
        

        public bool extraCarpetDuration = true;



        //        #endregion

        #region maso acc

        private readonly int[] ElectricAttacks = new int[]
        {
            ProjectileID.DeathLaser,
            ProjectileID.EyeLaser,
            ProjectileID.PinkLaser,
            ProjectileID.EyeBeam,
            ProjectileID.MartianTurretBolt,
            ProjectileID.BrainScramblerBolt,
            ProjectileID.GigaZapperSpear,
            ProjectileID.RayGunnerLaser,
            ProjectileID.SaucerLaser,
            ProjectileID.NebulaLaser,
            ProjectileID.VortexVortexLightning,
            ProjectileID.DD2LightningBugZap
        };

        public float GroundStickDR(Projectile proj, ref Player.HurtModifiers modifiers)
        {
            float dr = 0;
            if (!Player.GetToggleValue("MasoLightning"))
                return dr;

            bool electricAttack = false;

            if (proj.ModProjectile == null)
            {
                if (proj.aiStyle == ProjAIStyleID.MartianDeathRay
                    || proj.aiStyle == ProjAIStyleID.ThickLaser
                    || proj.aiStyle == ProjAIStyleID.LightningOrb
                    || ElectricAttacks.Contains(proj.type))
                {
                    electricAttack = true;
                }
            }
            else if (proj.ModProjectile is BaseDeathray)
            {
                electricAttack = true;
            }
            else
            {
                string name = proj.ModProjectile.Name.ToLower();
                if (name.Contains("lightning") || name.Contains("electr") || name.Contains("thunder") || name.Contains("laser"))
                    electricAttack = true;
            }

            if (electricAttack && Player.whoAmI == Main.myPlayer && !Player.HasBuff(ModContent.BuffType<SuperchargedBuff>()))
            {
                dr = 0.5f;

                Player.AddBuff(ModContent.BuffType<SuperchargedBuff>(), 60 * 30);

                foreach (Projectile p in Main.projectile.Where(p => p.active && p.minion && p.owner == Player.whoAmI
                    && (p.type == ModContent.ProjectileType<Probe1>() || p.type == ModContent.ProjectileType<Probe2>())))
                {
                    p.ai[1] = 180;
                    p.netUpdate = true;
                }

                SoundEngine.PlaySound(SoundID.NPCDeath6, Player.Center);
                SoundEngine.PlaySound(SoundID.Item92, Player.Center);
                SoundEngine.PlaySound(SoundID.Item14, Player.Center);

                for (int i = 0; i < 20; i++)
                {
                    int dust = Dust.NewDust(Player.position, Player.width, Player.height, DustID.Vortex, 0f, 0f, 100, default, 3f);
                    Main.dust[dust].velocity *= 1.4f;
                }

                for (int i = 0; i < 10; i++)
                {
                    int dust = Dust.NewDust(Player.position, Player.width, Player.height, DustID.Torch, 0f, 0f, 100, default, 3.5f);
                    Main.dust[dust].noGravity = true;
                    Main.dust[dust].velocity *= 7f;
                    dust = Dust.NewDust(Player.position, Player.width, Player.height, DustID.Torch, 0f, 0f, 100, default, 1.5f);
                    Main.dust[dust].velocity *= 3f;
                }

                for (int index1 = 0; index1 < 30; ++index1)
                {
                    int index2 = Dust.NewDust(Player.position, Player.width, Player.height, DustID.Vortex, 0f, 0f, 100, new Color(), 2f);
                    Main.dust[index2].noGravity = true;
                    Main.dust[index2].velocity *= 21f;
                    Main.dust[index2].noLight = true;
                    int index3 = Dust.NewDust(Player.position, Player.width, Player.height, DustID.Vortex, 0f, 0f, 100, new Color(), 1f);
                    Main.dust[index3].velocity *= 12f;
                    Main.dust[index3].noGravity = true;
                    Main.dust[index3].noLight = true;
                }

                for (int i = 0; i < 20; i++)
                {
                    int d = Dust.NewDust(Player.position, Player.width, Player.height, DustID.Vortex, 0f, 0f, 100, default, Main.rand.NextFloat(2f, 5f));
                    if (Main.rand.NextBool(3))
                        Main.dust[d].noGravity = true;
                    Main.dust[d].velocity *= Main.rand.NextFloat(12f, 18f);
                    Main.dust[d].position = Player.Center;
                }
            }
            return dr;
        }


        public void GuttedHeartEffect()
        {
            if (Player.whoAmI != Main.myPlayer)
                return;

            GuttedHeartCD--;

            if (Player.velocity == Vector2.Zero && Player.itemAnimation == 0)
                GuttedHeartCD--;

            if (GuttedHeartCD <= 0)
            {
                int cd = (int)Math.Round(Utils.Lerp(10 * 60, 15 * 60, (float)Player.statLife / Player.statLifeMax2));
                GuttedHeartCD = cd;
                if (Player.GetToggleValue("MasoBrain"))
                {
                    int count = 0;
                    for (int i = 0; i < Main.maxNPCs; i++)
                    {
                        if (Main.npc[i].active && Main.npc[i].type == ModContent.NPCType<CreeperGutted>() && Main.npc[i].ai[0] == Player.whoAmI)
                            count++;
                    }
                    if (count < 5)
                    {
                        int multiplier = 1;
                        if (PureHeart)
                            multiplier = 2;
                        if (MasochistSoul)
                            multiplier = 5;
                        if (Main.netMode == NetmodeID.SinglePlayer)
                        {
                            int n = NPC.NewNPC(NPC.GetBossSpawnSource(Player.whoAmI), (int)Player.Center.X, (int)Player.Center.Y, ModContent.NPCType<CreeperGutted>(), 0, Player.whoAmI, 0f, multiplier);
                            if (n != Main.maxNPCs)
                                Main.npc[n].velocity = Vector2.UnitX.RotatedByRandom(2 * Math.PI) * 8;
                        }
                        else if (Main.netMode == NetmodeID.MultiplayerClient)
                        {
                            var netMessage = Mod.GetPacket();
                            netMessage.Write((byte)FargowiltasSouls.PacketID.RequestGuttedCreeper);
                            netMessage.Write((byte)Player.whoAmI);
                            netMessage.Write((byte)multiplier);
                            netMessage.Send();
                        }
                    }
                    else
                    {
                        int lowestHealth = -1;
                        for (int i = 0; i < Main.maxNPCs; i++)
                        {
                            if (Main.npc[i].active && Main.npc[i].type == ModContent.NPCType<CreeperGutted>() && Main.npc[i].ai[0] == Player.whoAmI)
                            {
                                if (lowestHealth < 0)
                                    lowestHealth = i;
                                else if (Main.npc[i].life < Main.npc[lowestHealth].life)
                                    lowestHealth = i;
                            }
                        }
                        if (Main.npc[lowestHealth].life < Main.npc[lowestHealth].lifeMax)
                        {
                            if (Main.netMode == NetmodeID.SinglePlayer)
                            {
                                int damage = Main.npc[lowestHealth].lifeMax - Main.npc[lowestHealth].life;
                                Main.npc[lowestHealth].life = Main.npc[lowestHealth].lifeMax;
                                CombatText.NewText(Main.npc[lowestHealth].Hitbox, CombatText.HealLife, damage);
                            }
                            else if (Main.netMode == NetmodeID.MultiplayerClient)
                            {
                                var netMessage = Mod.GetPacket();
                                netMessage.Write((byte)FargowiltasSouls.PacketID.RequestCreeperHeal);
                                netMessage.Write((byte)Player.whoAmI);
                                netMessage.Write((byte)lowestHealth);
                                netMessage.Send();
                            }
                        }
                    }
                }
            }
        }

        public void GuttedHeartNurseHeal()
        {
            if (Player.whoAmI == Main.myPlayer && Player.GetToggleValue("MasoBrain"))
            {
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    NPC npc = Main.npc[i];

                    if (npc.type == ModContent.NPCType<CreeperGutted>() && npc.ai[0] == Player.whoAmI)
                    {
                        int heal = npc.lifeMax - npc.life;

                        if (Main.netMode == NetmodeID.SinglePlayer)
                        {
                            if (heal > 0)
                            {
                                npc.HealEffect(heal);
                                npc.life = npc.lifeMax;
                            }
                        }
                        else if (Main.netMode == NetmodeID.MultiplayerClient)
                        {
                            var netMessage = Mod.GetPacket();
                            netMessage.Write((byte)FargowiltasSouls.PacketID.RequestCreeperHeal);
                            netMessage.Write((byte)Player.whoAmI);
                            netMessage.Write((byte)i);
                            netMessage.Send();
                        }
                    }
                }
            }
        }

        public void WretchedPouchEffect()
        {
            if (!Player.GetToggleValue("MasoPouch"))
                return;

            if (!Player.controlUseItem && !Player.controlUseTile && WeaponUseTimer <= 6) //remove extra 6 added to the timer, makes it a lot less awkward
                return;

            if (Player.HeldItem.IsAir || Player.HeldItem.damage <= 0 || Player.HeldItem.pick > 0 || Player.HeldItem.axe > 0 || Player.HeldItem.hammer > 0)
                return;

            Player.AddBuff(ModContent.BuffType<WretchedHexBuff>(), 2);

            int d = Dust.NewDust(Player.position, Player.width, Player.height, DustID.Shadowflame, Player.velocity.X * 0.4f, Player.velocity.Y * 0.4f, 0, new Color(), 3f);
            Main.dust[d].noGravity = true;
            Main.dust[d].velocity *= 5f;

            Player.GetDamage(DamageClass.Generic) += 1.20f;
            Player.endurance -= 0.20f;
            Player.velocity *= 0.875f;

            if (--WretchedPouchCD <= 0)
            {
                WretchedPouchCD = 25;

                if (Player.whoAmI == Main.myPlayer)
                {
                    Vector2 vel = Main.rand.NextVector2Unit();
                    
                    NPC target = Main.npc.FirstOrDefault(n => n.active && n.Distance(Player.Center) < 360 && n.CanBeChasedBy() && Collision.CanHit(Player.position, Player.width, Player.height, n.position, n.width, n.height));
                    if (target != null)
                        vel = Player.DirectionTo(target.Center);

                    vel *= 8f;

                    SoundEngine.PlaySound(SoundID.Item103, Player.Center);

                    int dam = 40;
                    if (MasochistSoul)
                        dam *= 3;
                    dam = (int)(dam * Player.ActualClassDamage(DamageClass.Magic));

                    void ShootTentacle(Vector2 baseVel, float variance, int aiMin, int aiMax)
                    {
                        Vector2 speed = baseVel.RotatedBy(variance * (Main.rand.NextDouble() - 0.5));
                        float ai0 = Main.rand.Next(aiMin, aiMax) * (1f / 1000f);
                        if (Main.rand.NextBool())
                            ai0 *= -1f;
                        float ai1 = Main.rand.Next(aiMin, aiMax) * (1f / 1000f);
                        if (Main.rand.NextBool())
                            ai1 *= -1f;
                        Projectile.NewProjectile(Player.GetSource_Accessory(WretchedPouchItem), Player.Center, speed, ModContent.ProjectileType<ShadowflameTentacle>(), dam, 4f, Player.whoAmI, ai0, ai1);
                    };

                    int max = target == null ? 3 : 6;
                    float rotationOffset = MathHelper.TwoPi / max;
                    if (target != null)
                    {
                        for (int i = 0; i < max / 2; i++) //shoot right at them
                            ShootTentacle(vel, rotationOffset, 60, 90);
                    }
                    for (int i = 0; i < max; i++) //shoot everywhere
                        ShootTentacle(vel.RotatedBy(rotationOffset * i), rotationOffset, 30, 50);
                }
            }
        }

        public void OnLandingEffects()
        {
            

            //Player.justJumped use this tbh
            if (SlimyShieldFalling) //landing
            {
                if (Player.velocity.Y < 0f)
                    SlimyShieldFalling = false;

                if (Player.velocity.Y == 0f)
                {
                    SlimyShieldFalling = false;
                    if (Player.whoAmI == Main.myPlayer && Player.gravDir > 0)
                    {
                        if (SlimyShieldItem != null && Player.GetToggleValue("MasoSlime"))
                        {
                            SoundEngine.PlaySound(SoundID.Item21, Player.Center);
                            Vector2 mouse = Main.MouseWorld;
                            int damage = 8;
                            if (SupremeDeathbringerFairy)
                                damage = 16;
                            if (MasochistSoul)
                                damage = 80;
                            damage = (int)(damage * Player.ActualClassDamage(DamageClass.Melee));
                            for (int i = 0; i < 3; i++)
                            {
                                Vector2 spawn = new(mouse.X + Main.rand.Next(-200, 201), mouse.Y - Main.rand.Next(600, 901));
                                if (Collision.CanHitLine(mouse, 0, 0, spawn, 0, 0))
                                {
                                    Vector2 speed = mouse - spawn;
                                    speed.Normalize();
                                    speed *= 10f;
                                    Projectile.NewProjectile(Player.GetSource_Accessory(SlimyShieldItem, "SlimyShield"), spawn, speed, ModContent.ProjectileType<SlimeBall>(), damage, 1f, Main.myPlayer);
                                }
                            }
                        }

                        if (GelicWingsItem != null && Player.GetToggleValue("MasoQueen"))
                        {
                            int dam = 60; //deliberately no scaling
                            for (int j = -1; j <= 1; j += 2)
                            {
                                Vector2 baseVel = Vector2.UnitX.RotatedBy(MathHelper.ToRadians(-10 * j));
                                const int max = 8;
                                for (int i = 0; i < max; i++)
                                {
                                    Vector2 vel = Main.rand.NextFloat(5f, 10f) * j * baseVel.RotatedBy(-MathHelper.PiOver4 * 0.8f / max * i * j);
                                    Projectile.NewProjectile(Player.GetSource_Accessory(GelicWingsItem), Player.Bottom - Vector2.UnitY * 8, vel, ModContent.ProjectileType<GelicWingSpike>(), dam, 5f, Main.myPlayer);
                                }
                            }
                        }
                    }
                }
            }
            else if (Player.velocity.Y > 3f)
            {
                SlimyShieldFalling = true;
            }
        }

        public void AgitatingLensEffect()
        {
            if (AgitatingLensCD++ > 15)
            {
                AgitatingLensCD = 0;
                if ((Math.Abs(Player.velocity.X) >= 5 || Math.Abs(Player.velocity.Y) >= 5) && Player.whoAmI == Main.myPlayer && Player.GetToggleValue("MasoEye"))
                {
                    int damage = 18;
                    if (SupremeDeathbringerFairy)
                        damage *= 2;
                    if (MasochistSoul)
                        damage *= 2;
                    damage = (int)(damage * Player.ActualClassDamage(DamageClass.Magic));
                    Projectile.NewProjectile(Player.GetSource_Accessory(AgitatingLensItem), Player.Center, Player.velocity * 0.1f, ModContent.ProjectileType<BloodScytheFriendly>(), damage, 5f, Player.whoAmI);
                }
            }
        }

        public void DarkenedHeartAttack(Projectile projectile)
        {
            if (DarkenedHeartCD <= 0)
            {
                DarkenedHeartCD = 60;

                if (Player.GetToggleValue("MasoEater") && (projectile == null || projectile.type != ProjectileID.TinyEater))
                {
                    SoundEngine.PlaySound(SoundID.NPCHit1, Player.Center);
                    for (int index1 = 0; index1 < 20; ++index1)
                    {
                        int index2 = Dust.NewDust(Player.position, Player.width, Player.height, DustID.ScourgeOfTheCorruptor, 0.0f, 0.0f, 0, new Color(), 1f);
                        Dust dust = Main.dust[index2];
                        dust.scale *= 1.1f;
                        Main.dust[index2].noGravity = true;
                    }
                    for (int index1 = 0; index1 < 30; ++index1)
                    {
                        int index2 = Dust.NewDust(Player.position, Player.width, Player.height, DustID.ScourgeOfTheCorruptor, 0.0f, 0.0f, 0, new Color(), 1f);
                        Dust dust1 = Main.dust[index2];
                        dust1.velocity *= 2.5f;
                        Dust dust2 = Main.dust[index2];
                        dust2.scale *= 0.8f;
                        Main.dust[index2].noGravity = true;
                    }
                    int num = 2;
                    if (Main.rand.NextBool(3))
                        ++num;
                    if (Main.rand.NextBool(6))
                        ++num;
                    if (Main.rand.NextBool(9))
                        ++num;
                    int dam = PureHeart ? 45 : 18;
                    if (MasochistSoul)
                        dam *= 2;
                    for (int index = 0; index < num; ++index)
                        Projectile.NewProjectile(Player.GetSource_Accessory(DarkenedHeartItem), Player.Center.X, Player.Center.Y, Main.rand.Next(-35, 36) * 0.02f * 10f,
                            Main.rand.Next(-35, 36) * 0.02f * 10f, ProjectileID.TinyEater, (int)(dam * Player.ActualClassDamage(DamageClass.Melee)), 1.75f, Player.whoAmI);
                }
            }
        }

        public void FrigidGemstoneKey()
        {
            if (FrigidGemstoneCD > 0)
                return;

            if (!Player.CheckMana(6, true))
                return;

            FrigidGemstoneCD = 10;
            Player.manaRegenDelay = Math.Max(Player.manaRegenDelay, 30);

            SoundEngine.PlaySound(SoundID.Item28, Player.Center);

            Projectile.NewProjectile(Player.GetSource_Accessory(FrigidGemstoneItem),
                Player.Center, 12f * Player.DirectionTo(Main.MouseWorld), ProjectileID.IceBlock,
                (int)(14 * Player.ActualClassDamage(DamageClass.Magic)), 2f,
                Player.whoAmI, Player.tileTargetX, Player.tileTargetY);
        }

        public void CelestialRuneSupportAttack(int damage, DamageClass damageType)
        {
            Vector2 position = Player.Center;
            Vector2 velocity = Vector2.Normalize(Main.MouseWorld - position);

            if (damageType.CountsAsClass(DamageClass.Melee)) //fireball
            {
                SoundEngine.PlaySound(SoundID.Item34, position);
                for (int i = 0; i < 3; i++)
                {
                    Projectile.NewProjectile(Player.GetSource_Accessory(CelestialRuneItem), position, velocity.RotatedByRandom(Math.PI / 6) * Main.rand.NextFloat(6f, 10f),
                        ModContent.ProjectileType<CelestialRuneFireball>(), (int)(50f * Player.ActualClassDamage(DamageClass.Melee)), 9f, Player.whoAmI);
                }
            }
            if (damageType.CountsAsClass(DamageClass.Ranged)) //lightning
            {
                for (int i = -1; i <= 1; i++)
                {
                    float ai1 = Main.rand.Next(100);
                    Vector2 vel = Vector2.Normalize(velocity.RotatedByRandom(Math.PI / 4)).RotatedBy(MathHelper.ToRadians(5) * i) * 7f;
                    Projectile.NewProjectile(Player.GetSource_Accessory(CelestialRuneItem), position, vel, ModContent.ProjectileType<CelestialRuneLightningArc>(),
                        (int)(50f * Player.ActualClassDamage(DamageClass.Ranged)), 1f, Player.whoAmI, velocity.ToRotation(), ai1);
                }
            }
            if (damageType.CountsAsClass(DamageClass.Magic)) //ice mist
            {
                Projectile.NewProjectile(Player.GetSource_Accessory(CelestialRuneItem), position, velocity * 4.25f, ModContent.ProjectileType<CelestialRuneIceMist>(), (int)(50f * Player.ActualClassDamage(DamageClass.Magic)), 4f, Player.whoAmI);
            }
            if (damageType.CountsAsClass(DamageClass.Summon)) //ancient vision
            {
                FargoSoulsUtil.NewSummonProjectile(Player.GetSource_Accessory(CelestialRuneItem), position, velocity * 16f, ModContent.ProjectileType<CelestialRuneAncientVision>(), 50, 3f, Player.whoAmI);
            }
        }

        public void SpecialDashKey()
        {
            if (SpecialDashCD <= 0)
            {
                SpecialDashCD = 60 * 5;

                if (Player.whoAmI == Main.myPlayer)
                {
                    Player.RemoveAllGrapplingHooks();

                    /*Player.controlLeft = false;
                    Player.controlRight = false;
                    Player.controlJump = false;
                    Player.controlDown = false;*/
                    Player.controlUseItem = false;
                    Player.controlUseTile = false;
                    Player.controlHook = false;
                    Player.controlMount = false;

                    Player.itemAnimation = 0;
                    Player.itemTime = 0;
                    Player.reuseDelay = 0;

                    if (BetsysHeartItem != null)
                    {
                        Vector2 vel = Player.DirectionTo(Main.MouseWorld) * (MasochistHeart ? 25 : 20);
                        Projectile.NewProjectile(Player.GetSource_Accessory(BetsysHeartItem), Player.Center, vel, ModContent.ProjectileType<Content.Projectiles.BetsyDash>(), (int)(100 * Player.ActualClassDamage(DamageClass.Melee)), 6f, Player.whoAmI);

                        Player.immune = true;
                        Player.immuneTime = Math.Max(Player.immuneTime, 2);
                        Player.hurtCooldowns[0] = Math.Max(Player.hurtCooldowns[0], 2);
                        Player.hurtCooldowns[1] = Math.Max(Player.hurtCooldowns[1], 2);

                        //immune to all debuffs
                        foreach (int debuff in FargowiltasSouls.DebuffIDs)
                        {
                            if (!Player.HasBuff(debuff))
                            {
                                Player.buffImmune[debuff] = true;
                            }
                        }
                    }
                    else if (QueenStingerItem != null)
                    {
                        SpecialDashCD += 60;

                        Vector2 vel = Player.DirectionTo(Main.MouseWorld) * 20;
                        Projectile.NewProjectile(Player.GetSource_Accessory(QueenStingerItem), Player.Center, vel, ModContent.ProjectileType<BeeDash>(), (int)(44 * Player.ActualClassDamage(DamageClass.Melee)), 6f, Player.whoAmI);
                    }

                    Player.AddBuff(ModContent.BuffType<BetsyDashBuff>(), 20);
                }
            }
        }

        public void MagicalBulbKey()
        {
            if (Player.HasBuff(ModContent.BuffType<MagicalCleanseCDBuff>()))
                return;

            bool cleansed = false;

            int max = Player.buffType.Length;
            for (int i = 0; i < max; i++)
            {
                int timeLeft = Player.buffTime[i];
                if (timeLeft <= 0)
                    continue;

                int buffType = Player.buffType[i];
                if (buffType <= 0)
                    continue;

                if (timeLeft > 5
                    && Main.debuff[buffType]
                    && !Main.buffNoTimeDisplay[buffType]
                    && !BuffID.Sets.NurseCannotRemoveDebuff[buffType])
                {
                    Player.DelBuff(i);

                    cleansed = true;

                    i--;
                    max--; //just in case, to prevent being stuck here forever
                }
            }

            if (cleansed)
            {
                Player.AddBuff(ModContent.BuffType<MagicalCleanseCDBuff>(), 60 * 120);

                SoundEngine.PlaySound(SoundID.Item4, Player.Center);

                for (int index1 = 0; index1 < 50; ++index1)
                {
                    int index2 = Dust.NewDust(Player.position, Player.width, Player.height, Main.rand.NextBool() ? 107 : 157, 0f, 0f, 0, new Color(), 3f);
                    Main.dust[index2].noGravity = true;
                    Main.dust[index2].velocity *= 8f;
                }
            }
        }

        public void BombKey()
        {
            if (MutantEyeItem != null && MutantEyeCD <= 0)
            {
                MutantEyeCD = 3600;

                if (!Main.dedServ && Main.LocalPlayer.active)
                    Main.LocalPlayer.FargoSouls().Screenshake = 30;

                const int invulTime = 90;
                Player.immune = true;
                Player.immuneTime = invulTime;
                Player.hurtCooldowns[0] = invulTime;
                Player.hurtCooldowns[1] = invulTime;

                SoundEngine.PlaySound(SoundID.Item84, Player.Center);

                const int max = 100; //make some indicator dusts
                for (int i = 0; i < max; i++)
                {
                    Vector2 vector6 = Vector2.UnitY * 30f;
                    vector6 = vector6.RotatedBy((i - (max / 2 - 1)) * 6.28318548f / max) + Main.LocalPlayer.Center;
                    Vector2 vector7 = vector6 - Main.LocalPlayer.Center;
                    int d = Dust.NewDust(vector6 + vector7, 0, 0, DustID.Vortex, 0f, 0f, 0, default, 3f);
                    Main.dust[d].noGravity = true;
                    Main.dust[d].velocity = vector7;
                }

                for (int i = 0; i < 50; i++) //make some indicator dusts
                {
                    int d = Dust.NewDust(Player.position, Player.width, Player.height, DustID.Vortex, 0f, 0f, 0, default, 2.5f);
                    Main.dust[d].noGravity = true;
                    Main.dust[d].noLight = true;
                    Main.dust[d].velocity *= 24f;
                }

                FargoSoulsUtil.ClearHostileProjectiles(1);

                void SpawnSphereRing(int ringMax, float speed, int damage2, float rotationModifier)
                {
                    float rotation = 2f * (float)Math.PI / ringMax;
                    Vector2 vel = Vector2.UnitY * speed;
                    int type = ModContent.ProjectileType<PhantasmalSphereRing>();
                    for (int i = 0; i < ringMax; i++)
                    {
                        vel = vel.RotatedBy(rotation);
                        Projectile.NewProjectile(Player.GetSource_Accessory(MutantEyeItem), Player.Center, vel, type, damage2, 3f, Main.myPlayer, rotationModifier * Player.direction, speed);
                    }
                }

                int damage = (int)(1700 * Player.ActualClassDamage(DamageClass.Magic));
                SpawnSphereRing(24, 12f, damage, -1f);
                SpawnSphereRing(24, 12f, damage, 1f);
            }
            else if (CirnoGraze)
            {
                Projectile cirnoBomb = Main.projectile.FirstOrDefault(p => p.active && p.friendly && p.owner == Player.whoAmI && p.type == ModContent.ProjectileType<CirnoBomb>());
                if (cirnoBomb != null)
                {
                    cirnoBomb.ai[0] = 1;
                    cirnoBomb.netUpdate = true;

                    CirnoGrazeCounter = 0;
                }
            }
        }

        public void DebuffInstallKey()
        {
            if (AgitatingLensItem != null
                && Player.GetToggleValue("MasoEyeInstall", false)
                && Player.controlUp && Player.controlDown)
            {
				if (!Player.HasBuff(ModContent.BuffType<BerserkerInstallBuff>())
					&& !Player.HasBuff(ModContent.BuffType<BerserkerInstallCDBuff>()))
				{
					SoundEngine.PlaySound(SoundID.Item119, Player.Center);

					Player.AddBuff(ModContent.BuffType<BerserkerInstallBuff>(), 7 * 60 + 30); //7.5sec

					for (int i = 0; i < 60; i++)
					{
						int index2 = Dust.NewDust(Player.position, Player.width, Player.height, DustID.RedTorch, 0f, 0f, 0, default, 3f);
						Main.dust[index2].noGravity = true;
						Main.dust[index2].velocity *= 9;
					}
				}
            }
			else if (FusedLens && Player.GetToggleValue("FusedLensInstall", false))
            {
                int buffType = ModContent.BuffType<TwinsInstallBuff>();
                if (Player.HasBuff(buffType))
                {
                    Player.ClearBuff(buffType);
                }
                else
                {
                    SoundEngine.PlaySound(SoundID.Item119, Player.Center);

                    Player.AddBuff(ModContent.BuffType<TwinsInstallBuff>(), 60);

                    int max = 60;
                    for (int i = 0; i < max; i++)
                    {
                        float scale = 3f;
                        int index2 = Dust.NewDust(Player.position, Player.width, Player.height, Main.rand.NextBool() ? 90 : 89, 0f, 0f, 0, new Color(), scale);
                        Main.dust[index2].noGravity = true;
                        Main.dust[index2].velocity *= scale * 3;
                    }
                }
            }
        }

        public void AmmoCycleKey()
        {
            SoundEngine.PlaySound(SoundID.Unlock, Player.Center);

            for (int i = 54; i <= 56; i++)
            {
                int j = i + 1;

                (Player.inventory[j], Player.inventory[i]) = (Player.inventory[i], Player.inventory[j]);
            }
        }

        int lihzahrdFallCD;
        public void LihzahrdTreasureBoxUpdate()
        {
            if (lihzahrdFallCD > 0)
                lihzahrdFallCD--;

            if (Player.gravDir > 0 && Player.GetToggleValue("MasoGolem"))
            {
                if (lihzahrdFallCD <= 0 && !Player.mount.Active && Player.controlDown && Player.releaseDown && !Player.controlJump && Player.doubleTapCardinalTimer[0] > 0 && Player.doubleTapCardinalTimer[0] != 15)
                {
                    if (Player.velocity.Y != 0f)
                    {
                        if (Player.velocity.Y < 15f)
                        {
                            Player.velocity.Y = 15f;
                        }

                        if (GroundPound <= 0)
                        {
                            GroundPound = 1;
                        }
                    }
                }

                if (GroundPound > 0)
                {
                    lihzahrdFallCD = 60;

                    if (Player.velocity.Y == 0f && Player.controlDown)
                    {
                        Vector2 vec = Collision.TileCollision(Player.position, 15f * Vector2.UnitY, Player.width, Player.height, true, true, (int)Player.gravDir);
                        if (vec != Vector2.Zero)
                        {
                            Player.position += vec;
                            Player.velocity.Y = 15f;
                        }
                    }

                    if (Player.velocity.Y < 0f || Player.mount.Active || (Player.controlJump && !Player.controlDown))
                    {
                        GroundPound = 0;
                    }
                    else if (Player.velocity.Y == 0f && Player.oldVelocity.Y == 0f)
                    {
                        int x = (int)Player.Center.X / 16;
                        int y = (int)(Player.position.Y + Player.height + 8) / 16;
                        if (/*GroundPound > 15 && */x >= 0 && x < Main.maxTilesX && y >= 0 && y < Main.maxTilesY
                            && Main.tile[x, y] != null && Main.tile[x, y].HasUnactuatedTile && Main.tileSolid[Main.tile[x, y].TileType])
                        {
                            GroundPound = 0;

                            if (Player.GetToggleValue("MasoBoulder"))
                            {
                                if (!Main.dedServ)
                                    Main.LocalPlayer.FargoSouls().Screenshake = 60;

                                if (Player.whoAmI == Main.myPlayer)
                                {
                                    //explosion
                                    Projectile.NewProjectile(Player.GetSource_Accessory(LihzahrdTreasureBoxItem), Player.Center, Vector2.Zero, ModContent.ProjectileType<MoonLordSunBlast>(), 0, 0f, Player.whoAmI);
                                    int p = Projectile.NewProjectile(Player.GetSource_Accessory(LihzahrdTreasureBoxItem), Player.Center, Vector2.Zero, ModContent.ProjectileType<Explosion>(), (int)(200 * Player.ActualClassDamage(DamageClass.Melee)), 9f, Player.whoAmI);
                                    if (p != Main.maxProjectiles)
                                        Main.projectile[p].DamageType = DamageClass.Melee;

                                    //boulders
                                    int dam = 50;
                                    if (MasochistSoul)
                                        dam *= 3;
                                    for (int i = -5; i <= 5; i += 2)
                                    {
                                        Projectile.NewProjectile(Player.GetSource_Accessory(LihzahrdTreasureBoxItem), Player.Center, -10f * Vector2.UnitY.RotatedBy(MathHelper.PiOver2 / 6 * i),
                                            ModContent.ProjectileType<LihzahrdBoulderFriendly>(), (int)(dam * Player.ActualClassDamage(DamageClass.Melee)), 7.5f, Player.whoAmI);
                                    }

                                    //geysers
                                    int baseDamage = (int)(50 * Player.ActualClassDamage(DamageClass.Melee));
                                    if (MasochistSoul)
                                        baseDamage *= 3;
                                    Projectile.NewProjectile(Player.GetSource_Accessory(LihzahrdTreasureBoxItem), Player.Center, Vector2.Zero, ModContent.ProjectileType<ExplosionSmall>(), baseDamage * 2, 12f, Player.whoAmI);
                                    y -= 2;
                                    for (int i = -3; i <= 3; i++)
                                    {
                                        if (i == 0)
                                            continue;
                                        int tilePosX = x + 16 * i;
                                        int tilePosY = y;
                                        if (Main.tile[tilePosX, tilePosY] != null && tilePosX >= 0 && tilePosX < Main.maxTilesX)
                                        {
                                            while (Main.tile[tilePosX, tilePosY] != null && tilePosY >= 0 && tilePosY < Main.maxTilesY
                                                && !(Main.tile[tilePosX, tilePosY].HasUnactuatedTile && Main.tileSolid[Main.tile[tilePosX, tilePosY].TileType]))
                                            {
                                                tilePosY++;
                                            }
                                            Projectile.NewProjectile(Player.GetSource_Accessory(LihzahrdTreasureBoxItem), tilePosX * 16 + 8, tilePosY * 16 + 8, 0f, -8f, ModContent.ProjectileType<GeyserFriendly>(), baseDamage, 8f, Player.whoAmI);
                                        }
                                    }
                                }
                            }
                        }

                    }
                    else
                    {
                        if (Player.controlDown && Player.velocity.Y < 15f)
                        {
                            Player.velocity.Y = 15f;
                        }

                        Player.maxFallSpeed = 15f;
                        GroundPound++;

                        for (int i = 0; i < 5; i++)
                        {
                            int d = Dust.NewDust(Player.position, Player.width, Player.height, DustID.Torch, Scale: 1.5f);
                            Main.dust[d].noGravity = true;
                            Main.dust[d].velocity *= 0.2f;
                        }
                    }
                }
            }
        }

        public void AbomWandUpdate()
        {
            if (AbominableWandRevived) //has been revived already
            {
                if (Player.statLife >= Player.statLifeMax2) //can revive again
                {
                    AbominableWandRevived = false;

                    SoundEngine.PlaySound(SoundID.Item28, Player.Center);

                    FargoSoulsUtil.DustRing(Player.Center, 50, 87, 8f, default, 2f);

                    for (int i = 0; i < 30; i++)
                    {
                        int d = Dust.NewDust(Player.position, Player.width, Player.height, DustID.GemTopaz, 0f, 0f, 0, default, 2.5f);
                        Main.dust[d].noGravity = true;
                        Main.dust[d].velocity *= 8f;
                    }
                }
                else //cannot currently revive
                {
                    Player.AddBuff(ModContent.BuffType<AbomCooldownBuff>(), 2);
                }
            }
        }

        public void DreadParryCounter()
        {
            SoundEngine.PlaySound(SoundID.Item119, Player.Center);

            if (Player.whoAmI == Main.myPlayer)
            {
                int projDamage = FargoSoulsUtil.HighestDamageTypeScaling(Player, 1000);

                const int max = 20;
                for (int i = 0; i < max; i++)
                {
                    void SharpTears(Vector2 pos, Vector2 vel)
                    {
                        int p = Projectile.NewProjectile(Player.GetSource_Accessory(DreadShellItem), pos, vel, ProjectileID.SharpTears, projDamage, 12f, Player.whoAmI, 0f, Main.rand.NextFloat(0.5f, 1f));
                        if (p != Main.maxProjectiles)
                        {
                            Main.projectile[p].DamageType = DamageClass.Default;
                            Main.projectile[p].usesLocalNPCImmunity = false;
                            Main.projectile[p].usesIDStaticNPCImmunity = true;
                            Main.projectile[p].idStaticNPCHitCooldown = 60;
                            Main.projectile[p].FargoSouls().noInteractionWithNPCImmunityFrames = true;
                            Main.projectile[p].FargoSouls().DeletionImmuneRank = 1;
                        }
                    }

                    SharpTears(Player.Center, 16f * Vector2.UnitX.RotatedBy(MathHelper.TwoPi / max * (i + Main.rand.NextFloat(-0.5f, 0.5f))));

                    for (int k = 0; k < 6; k++)
                    {
                        Vector2 spawnPos = Player.Bottom;
                        spawnPos.X += Main.rand.NextFloat(-256, 256);

                        bool foundTile = false;

                        for (int j = 0; j < 40; j++)
                        {
                            Tile tile = Framing.GetTileSafely(spawnPos);
                            if (tile.HasUnactuatedTile && (Main.tileSolid[tile.TileType] || Main.tileSolidTop[tile.TileType]))
                            {
                                foundTile = true;
                                break;
                            }
                            spawnPos.Y += 16;
                        }

                        if (!foundTile)
                            spawnPos.Y = Player.Bottom.Y + Main.rand.NextFloat(-64, 64);

                        for (int j = 0; j < 40; j++)
                        {
                            Tile tile = Framing.GetTileSafely(spawnPos);
                            if (tile.HasUnactuatedTile && (Main.tileSolid[tile.TileType] || Main.tileSolidTop[tile.TileType]))
                                spawnPos.Y -= 16;
                            else
                                break;
                        }

                        if (!Collision.SolidCollision(spawnPos, 0, 0))
                        {
                            spawnPos.Y += 16;
                            SharpTears(spawnPos, 16f * -Vector2.UnitY.RotatedByRandom(MathHelper.ToRadians(30)));
                            break;
                        }
                    }
                }
            }
        }

        public void PumpkingsCapeCounter(int damage)
        {
            SoundEngine.PlaySound(SoundID.Item62, Player.Center);

            if (Player.whoAmI == Main.myPlayer)
            {
                int heal = GetHealMultiplier(damage);
                Player.statLife += heal;
                if (Player.statLife > Player.statLifeMax2)
                    Player.statLife = Player.statLifeMax2;
                Player.HealEffect(heal);

                int counterDamage = damage;

                for (int i = 0; i < 30; i++)
                {
                    Vector2 vel = Vector2.UnitX.RotatedByRandom(MathHelper.TwoPi);
                    vel *= Main.rand.NextFloat(12f, 24f);
                    FargoSoulsUtil.NewSummonProjectile(Player.GetSource_Accessory(PumpkingsCapeItem), Player.Center, vel, ModContent.ProjectileType<SpookyScythe>(), counterDamage, 6f, Player.whoAmI);
                }
            }
        }

        public void DeerSinewEffect()
        {
            Player.DisplayToggle("DeerSinewDash");
            if (DeerSinewFreezeCD > 0)
                DeerSinewFreezeCD--;

            if (!Player.GetToggleValue("DeerSinewDash", false) || HasDash || Player.mount.Active || Player.whoAmI != Main.myPlayer)
                return;

            HasDash = true;
            FargoDash = DashManager.DashType.DeerSinew;
            DeerSinewNerf = true;

            if (IsDashingTimer > 0)
            {
                for (int i = 0; i < 3; i++)
                {
                    int d = Dust.NewDust(Player.position, Player.width, Player.height, DustID.GemSapphire, Scale: 1.25f);
                    Main.dust[d].noGravity = true;
                    Main.dust[d].velocity *= 0.2f;
                }
            }
        }
        public void DeerSinewDash(int dir)
        {
            float dashSpeed = 12f;
            dashCD = 60;
            Player.dashDelay = dashCD;
            if (IsDashingTimer < 15)
                IsDashingTimer = 15;
            Player.velocity.X = dir * dashSpeed;
            if (Main.netMode == NetmodeID.MultiplayerClient)
                NetMessage.SendData(MessageID.PlayerControls, number: Player.whoAmI);
        }
        public float DeerSinewCritNerf()
        {
            float ratio = Math.Min(Player.velocity.Length() / 16f, 1f);
            return MathHelper.Lerp(1f, 0.75f, ratio);
        }

        #endregion maso acc

        // TODO: rework this because we can only get final damage in hurtinfo hooks
        public void TryParryAttack(ref Player.HurtInfo hurtInfo)
        {
            bool silverEffect = Player.HasEffect<SilverEffect>();
            if (GuardRaised && shieldTimer > 0 && !Player.immune)
            {
                Player.immune = true;
                int invul = Player.longInvince ? 90 : 60;
                int extrashieldCD = 40;

                int damageBlockCap = 100;
                const int higherCap = 200;

                if (DreadShellItem != null || PumpkingsCapeItem != null)
                {
                    damageBlockCap = higherCap;

                    invul += 60;

                    extrashieldCD = LONG_SHIELD_COOLDOWN;
                    if (silverEffect)
                        extrashieldCD = (LONG_SHIELD_COOLDOWN + BASE_SHIELD_COOLDOWN) / 2;
                }
                else if (silverEffect)
                {
                    extrashieldCD = BASE_SHIELD_COOLDOWN;
                }

                if (silverEffect)
                {
                    if (ForceEffect<SilverEnchant>())
                    {
                        damageBlockCap = higherCap;
                        Player.AddBuff(BuffID.ParryDamageBuff, 300);
                    }

                    Projectile.NewProjectile(Player.GetSource_Misc(""), Player.Center, Vector2.Zero, ModContent.ProjectileType<IronParry>(), 0, 0f, Main.myPlayer);
                }

                int damageBlocked = Math.Min(damageBlockCap, hurtInfo.Damage);
                var newDamage = hurtInfo.Damage - damageBlocked;
                if (newDamage < 1)
                {
                    hurtInfo.Null();
                }
                else
                {
                    hurtInfo.Damage = newDamage;
                }

                if (DreadShellItem != null)
                {
                    DreadParryCounter();
                }

                if (PumpkingsCapeItem != null)
                {
                    PumpkingsCapeCounter(damageBlocked);
                }

                Player.immuneTime = invul;
                Player.hurtCooldowns[0] = invul;
                Player.hurtCooldowns[1] = invul;
                ParryDebuffImmuneTime = invul;
                shieldCD = invul + extrashieldCD;

                foreach (int debuff in FargowiltasSouls.DebuffIDs) //immune to all debuffs
                {
                    if (!Player.HasBuff(debuff))
                        Player.buffImmune[debuff] = true;
                }
            }
        }

        private const int BASE_PARRY_WINDOW = 20;
        private const int BASE_SHIELD_COOLDOWN = 100;
        private const int HARD_PARRY_WINDOW = 10;
        private const int LONG_SHIELD_COOLDOWN = 360;

        void RaisedShieldEffects()
        {
            bool silverEffect = Player.HasEffect<SilverEffect>();
            if (DreadShellItem != null)
            {
                if (!MasochistSoul)
                    DreadShellVulnerabilityTimer = 60;
            }

            if (PumpkingsCapeItem != null) //strong aura effect
            {
                const float distance = 300f;
                for (int i = 0; i < Main.maxNPCs; i++)
                    if (Main.npc[i].active && !Main.npc[i].friendly && Main.npc[i].Distance(Player.Center) < distance)
                        Main.npc[i].AddBuff(ModContent.BuffType<RottingBuff>(), 600);

                for (int i = 0; i < 20; i++)
                {
                    Vector2 offset = new();
                    double angle = Main.rand.NextDouble() * 2d * Math.PI;
                    offset.X += (float)(Math.Sin(angle) * distance);
                    offset.Y += (float)(Math.Cos(angle) * distance);
                    Dust dust = Main.dust[Dust.NewDust(Player.Center + offset - new Vector2(4, 4), 0, 0, DustID.Ice_Pink, 0, 0, 100, Color.White, 1f)];
                    dust.velocity = Player.velocity;
                    if (Main.rand.NextBool(3))
                        dust.velocity += Vector2.Normalize(offset) * -5f;
                    dust.noGravity = true;
                }
            }

            if ((DreadShellItem != null || PumpkingsCapeItem != null) && !silverEffect)
            {
                Player.velocity.X *= 0.85f;
                if (Player.velocity.Y < 0)
                    Player.velocity.Y *= 0.85f;
            }

            int cooldown = BASE_SHIELD_COOLDOWN;
            if (DreadShellItem != null || PumpkingsCapeItem != null)
            {
                cooldown = LONG_SHIELD_COOLDOWN;
                if (silverEffect)
                    cooldown = (LONG_SHIELD_COOLDOWN + BASE_SHIELD_COOLDOWN) / 2;
            }
            else if (silverEffect)
            {
                cooldown = BASE_SHIELD_COOLDOWN;
            }

            if (shieldCD < cooldown)
                shieldCD = cooldown;
        }

        public void UpdateShield()
        {
            bool silverEffect = Player.HasEffect<SilverEffect>();
            GuardRaised = false;

            //no need when player has brand of inferno
            if ((!silverEffect && DreadShellItem == null && PumpkingsCapeItem == null) ||
                Player.inventory[Player.selectedItem].type == ItemID.DD2SquireDemonSword || Player.inventory[Player.selectedItem].type == ItemID.BouncingShield)
            {
                shieldTimer = 0;
                wasHoldingShield = false;
                return;
            }

            Player.shieldRaised = Player.selectedItem != 58 && Player.controlUseTile && !Player.tileInteractionHappened && Player.releaseUseItem
                && !Player.controlUseItem && !Player.mouseInterface && !CaptureManager.Instance.Active && !Main.HoveringOverAnNPC
                && !Main.SmartInteractShowingGenuine &&
                Player.itemAnimation == 0 && Player.itemTime == 0 && Player.reuseDelay == 0 && PlayerInput.Triggers.Current.MouseRight;

            if (Player.shieldRaised)
            {
                GuardRaised = true;

                for (int i = 3; i < 8 + Player.extraAccessorySlots; i++)
                {
                    if (Player.shield == -1 && Player.armor[i].shieldSlot != -1)
                        Player.shield = Player.armor[i].shieldSlot;
                }

                if (shieldTimer > 0)
                    shieldTimer--;

                if (!wasHoldingShield)
                {
                    wasHoldingShield = true;

                    if (shieldCD == 0) //if cooldown over, enable parry
                    {
                        if (DreadShellItem != null || PumpkingsCapeItem != null)
                        {
                            shieldTimer = HARD_PARRY_WINDOW;
                            if (silverEffect)
                                shieldTimer += (BASE_PARRY_WINDOW - HARD_PARRY_WINDOW) / 2;
                        }
                        else if (silverEffect)
                        {
                            shieldTimer = BASE_PARRY_WINDOW;
                        }
                    }

                    Player.itemAnimation = 0;
                    Player.itemTime = 0;
                    Player.reuseDelay = 0;
                }
                else //doing this so that on the first tick, these things DO NOT run
                {
                    RaisedShieldEffects();
                }

                if (shieldTimer == 1) //parry window over
                {
                    SoundEngine.PlaySound(SoundID.Item27, Player.Center);

                    List<int> dusts = new();
                    if (DreadShellItem != null)
                        dusts.Add(DustID.LifeDrain);
                    if (PumpkingsCapeItem != null)
                        dusts.Add(87);
                    if (silverEffect)
                        dusts.Add(66);

                    if (dusts.Count > 0)
                    {
                        for (int i = 0; i < 20; i++)
                        {
                            int d = Dust.NewDust(Player.position, Player.width, Player.height, Main.rand.Next(dusts), 0, 0, 0, default, 1.5f);
                            Main.dust[d].noGravity = true;
                            Main.dust[d].velocity *= 3f;
                        }
                    }
                }
            }
            else
            {
                shieldTimer = 0;

                if (wasHoldingShield)
                {
                    wasHoldingShield = false;

                    Player.shield_parry_cooldown = 0; //prevent that annoying tick noise
                    //Player.shieldParryTimeLeft = 0;
                    //ironShieldTimer = 0;
                }

                if (shieldCD == 1) //cooldown over
                {
                    SoundEngine.PlaySound(SoundID.Item28, Player.Center); //make a sound for refresh

                    List<int> dusts = new();
                    if (DreadShellItem != null)
                        dusts.Add(DustID.LifeDrain);
                    if (PumpkingsCapeItem != null)
                        dusts.Add(87);
                    if (silverEffect)
                        dusts.Add(66);
                    
                    if (dusts.Count > 0)
                    {
                        for (int i = 0; i < 30; i++)
                        {
                            int d = Dust.NewDust(Player.position, Player.width, Player.height, Main.rand.Next(dusts), 0, 0, 0, default, 2.5f);
                            Main.dust[d].noGravity = true;
                            Main.dust[d].velocity *= 6f;
                        }
                    }
                }

                if (shieldCD > 0)
                    shieldCD--;
            }

            //Main.NewText($"{ironShieldCD}, {ironShieldTimer}");
        }

        
    }
}
