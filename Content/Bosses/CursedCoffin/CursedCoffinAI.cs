using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Audio;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.DataStructures;
using FargowiltasSouls.Common.Graphics.Particles;
using static tModPorter.ProgressUpdate;
using System.Drawing;
using Terraria.WorldBuilding;
using FargowiltasSouls.Core.Systems;
using FargowiltasSouls.Content.Patreon.DanielTheRobot;
using FargowiltasSouls.Content.Bosses.AbomBoss;
using FargowiltasSouls.Content.Buffs.Masomode;

namespace FargowiltasSouls.Content.Bosses.CursedCoffin
{
    public partial class CursedCoffin : ModNPC
    {
        #region Variables

        public static readonly SoundStyle PhaseTransitionSFX = new("FargowiltasSouls/Assets/Sounds/CoffinPhaseTransition");
        public static readonly SoundStyle SlamSFX = new("FargowiltasSouls/Assets/Sounds/CoffinSlam") { PitchVariance = 0.3f };
        public static readonly SoundStyle SpiritDroneSFX = new("FargowiltasSouls/Assets/Sounds/CoffinSpiritDrone") { MaxInstances = 1, SoundLimitBehavior = SoundLimitBehavior.IgnoreNew, Volume = 0.2f };
        public static readonly SoundStyle BigShotSFX = new("FargowiltasSouls/Assets/Sounds/CoffinBigShot") { Volume = 0.6f, PitchVariance = 0.3f };
        public static readonly SoundStyle ShotSFX = new("FargowiltasSouls/Assets/Sounds/CoffinShot") { Volume = 0.3f, PitchVariance = 0.3f };
        public static readonly SoundStyle SoulShotSFX = new("FargowiltasSouls/Assets/Sounds/CoffinSoulShot") { Volume = 0.3f, PitchVariance = 0.3f};
        public static readonly SoundStyle HandChargeSFX = new("FargowiltasSouls/Assets/Sounds/CoffinHandCharge");

        public enum StateEnum
        {
            Opening,
            PhaseTransition,
            StunPunish,
            HoveringForSlam,
            SlamWShockwave,
            WavyShotCircle,
            WavyShotFlight,
            GrabbyHands,
            RandomStuff,
        }

        private readonly List<StateEnum> P1Attacks = new()
        {
            StateEnum.HoveringForSlam,
            StateEnum.WavyShotCircle,
            StateEnum.WavyShotFlight,
            StateEnum.GrabbyHands,
            
        };
        private readonly List<StateEnum> P2Attacks = new()
        {
            StateEnum.HoveringForSlam,
            StateEnum.WavyShotCircle,
            StateEnum.WavyShotFlight,
            StateEnum.GrabbyHands,
            StateEnum.RandomStuff,
        };
        private List<int> availablestates = new();

        public Player Player => Main.player[NPC.target];

        #endregion
        #region AI
        public override void OnSpawn(IEntitySource source)
        {
            Targeting();
            Player player = Main.player[NPC.target];
            if (player.Alive())
            {
                NPC.position = player.Center + new Vector2(0, -700) - NPC.Size / 2;
                LockVector1 = player.Top - Vector2.UnitY * 50;
                NPC.velocity = new Vector2(0, 0.25f);
            }
        }
        public override bool? CanFallThroughPlatforms() => NPC.noTileCollide || (Player.Top.Y > NPC.Bottom.Y + 30) ? true : null;
        public override void AI()
        {
            //Defaults
            NPC.defense = NPC.defDefense;
            if (PhaseTwo)
                NPC.defense += 15;
            NPC.rotation = 0;
            NPC.noTileCollide = true;

            if (!Targeting())
                return;
            NPC.timeLeft = 60;

            if (Player.HasBuff<StunnedBuff>() && State != (float)StateEnum.StunPunish && !Main.projectile.Any(p => p.TypeAlive<CoffinHand>()))
            {
                Timer = 0;
                AI2 = 0;
                AI3 = 0;
                State = (float)StateEnum.StunPunish;
            }
            //Normal looping attack AI
            if (Attacking) //Phases and random attack choosing
            {
                switch ((StateEnum)State) //Attack Choices
                {
                    case StateEnum.Opening:
                        Opening();
                        break;
                    case StateEnum.PhaseTransition:
                        PhaseTransition();
                        break;
                    case StateEnum.StunPunish:
                        StunPunish();
                        break;
                    case StateEnum.HoveringForSlam:
                        HoveringForSlam();
                        break;
                    case StateEnum.SlamWShockwave:
                        SlamWShockwave();
                        break;
                    case StateEnum.WavyShotCircle:
                        WavyShotCircle();
                        break;
                    case StateEnum.WavyShotFlight:
                        WavyShotFlight();
                        break;
                    case StateEnum.GrabbyHands:
                        GrabbyHands();
                        break;
                    case StateEnum.RandomStuff:
                        RandomStuff();
                        break;
                    default:
                        StateReset();
                        break;
                }
            }
            Timer++;
        }
        #endregion
        #region States
        public void Opening()
        {
            if (Timer >= 0)
            {
                ExtraTrail = true;
                NPC.velocity.Y *= 1.04f;
                if (NPC.Center.Y >= LockVector1.Y || Timer > 60 * 2)
                {
                    NPC.noTileCollide = false;
                    if (NPC.velocity.Y <= 1) //when you hit tile
                    {
                        SoundEngine.PlaySound(SoundID.Item14, NPC.Center);
                        SoundEngine.PlaySound(SlamSFX, NPC.Center);
                        //dust explosion
                        ExtraTrail = false;
                        Timer = -60;
                        //shockwaves
                        if (FargoSoulsUtil.HostCheck)
                        {
                            for (int i = -1; i <= 1; i += 2)
                            {
                                Vector2 vel = Vector2.UnitX * i * 3;
                                Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Bottom, vel, ModContent.ProjectileType<CoffinSlamShockwave>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage, 0.1f), 1f, Main.myPlayer);
                            }
                        }
                    }
                }
                if (NPC.Center.Y >= LockVector1.Y + 800) //only go so far
                {
                    NPC.velocity = Vector2.Zero;
                }
            }
            if (Timer == -1)
                StateReset();
        }
        public void PhaseTransition()
        {
            HoverSound();

            const int TransTime = 120;
            NPC.velocity = -Vector2.UnitY * 5 * (1-(Timer / TransTime));
            NPC.rotation = Main.rand.NextFloat(MathF.Tau * 0.06f * (Timer / TransTime));
            SoundEngine.PlaySound(SpiritDroneSFX, NPC.Center);
            if (Timer >= 60)
            {
                SoundEngine.PlaySound(PhaseTransitionSFX, NPC.Center);
                PhaseTwo = true;
                NPC.netUpdate = true;
                if (FargoSoulsUtil.HostCheck)
                {
                    Vector2 maskCenter = MaskCenter();
                    NPC.NewNPC(NPC.GetSource_FromAI(), (int)maskCenter.X, (int)maskCenter.Y, ModContent.NPCType<CursedSpirit>(), ai0: NPC.whoAmI);
                }
                NPC.velocity = Vector2.UnitY * 0.1f;
                State = (float)StateEnum.SlamWShockwave;
                LockVector1 = Player.Top - Vector2.UnitY * 250;
                AI2 = 2; // only bounce once
                Timer = 0;
            }
                
        }
        public void StunPunish()
        {
            NPC.velocity *= 0.95f;
            if (Timer < 20)
            {
                if (++NPC.frameCounter % 4 == 3)
                    if (Frame < Main.npcFrameCount[Type] - 1)
                        Frame++;
            }
            else if (Timer == 20)
            {
                SoundEngine.PlaySound(ShotSFX, NPC.Center);
                if (FargoSoulsUtil.HostCheck)
                {
                    Vector2 dir = NPC.rotation.ToRotationVector2();
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, dir * 4, ModContent.ProjectileType<CoffinHand>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage, 0.1f), 1f, Main.myPlayer, NPC.whoAmI, 22);
                }
            }
            else
            {
                if (++NPC.frameCounter % 60 == 59)
                    Frame--;
                if (Frame <= 0)
                {
                    NPC.frameCounter = 0;
                    Frame = 0;
                    StateReset();
                }
                    
            }
        }
        public void HoveringForSlam()
        {
            const float WaveAmpX = 200;
            const float WaveAmpY = 30;
            const float XHalfPeriod = 60 * 1.5f;
            const float YHalfPeriod = 60 * 0.75f;
            ref float XThetaOffset = ref AI2;
            ref float RandomTimer = ref AI3;

            HoverSound();

            if (Timer == 1)
            {
                float xOffset = Utils.Clamp(NPC.Center.X - Player.Center.X, -WaveAmpX, WaveAmpX);
                XThetaOffset = MathF.Asin(xOffset / WaveAmpX);
                RandomTimer = Main.rand.Next(160, 220);
            }
            
            if (Timer < RandomTimer && Timer >= 0)
            {
                NPC.noTileCollide = true;
                float desiredX = WaveAmpX * MathF.Sin(XThetaOffset + MathF.PI * (Timer / XHalfPeriod));
                float desiredY = -350 + WaveAmpY * MathF.Sin(MathF.PI * (Timer / YHalfPeriod));
                Vector2 desiredPos = Player.Center + desiredX * Vector2.UnitX + desiredY * Vector2.UnitY;
                Movement(desiredPos, 0.1f, 10, 5, 0.08f, 20);
            }
            else if (Timer == RandomTimer)
            {
                NPC.velocity.Y = -5;
                NPC.velocity.X /= 2;
                LockVector1 = Player.Top - Vector2.UnitY * 250;
                XThetaOffset = 0;
                State = (float)StateEnum.SlamWShockwave;
                Timer = 0;
            }
        }
        public void SlamWShockwave()
        {
            ref float Counter = ref AI2;

            if (Timer >= 0)
            {
                //if (Timer < 15) // no funny double hits from weird terrain
                  //  NPC.noTileCollide = true;

                NPC.velocity.X *= 0.97f;
                if (WorldSavingSystem.EternityMode)
                    NPC.velocity.X += Math.Sign(Player.Center.X - NPC.Center.X) * 0.2f;
                if (NPC.velocity.Y >= 0 && Counter == 0)
                {
                    Counter = 1;
                }
                if (NPC.velocity.Y == 0 && Counter > 0) //when you hit tile
                {
                    SoundEngine.PlaySound(SoundID.Item14, NPC.Center);
                    SoundEngine.PlaySound(SlamSFX, NPC.Center);
                    ExtraTrail = false;

                    //shockwaves
                    if (FargoSoulsUtil.HostCheck)
                    {
                        for (int i = -1; i <= 1; i += 2)
                        {
                            Vector2 vel = Vector2.UnitX * i * 3;
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Bottom, vel, ModContent.ProjectileType<CoffinSlamShockwave>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage, 0.1f), 1f, Main.myPlayer);
                        }
                    }
                    if (WorldSavingSystem.EternityMode && Counter < 2)
                    {
                        Counter = 2;
                        Timer = 0;
                        NPC.velocity.Y = -8;
                    }
                    else
                    {
                        int endlag = WorldSavingSystem.MasochistModeReal ? 80 : WorldSavingSystem.EternityMode ? 100 : 120;
                        Timer = -endlag;
                        NPC.velocity.X = 0;
                    }
                    return;
                }
                NPC.velocity.Y += 0.175f;
                if (NPC.velocity.Y > 0)
                    NPC.velocity.Y += 0.32f;
                ExtraTrail = true;

                NPC.noTileCollide = false;

                if (NPC.Center.Y >= LockVector1.Y + 1000) //only go so far
                {
                    NPC.velocity = Vector2.Zero;
                }
            }
            if (Timer == -1)
            {
                StateReset();
            }
        }
        public void WavyShotCircle()
        {
            int TelegraphTime = WorldSavingSystem.MasochistModeReal ? 60 : 70;
            float progress = 1 -(Timer / TelegraphTime);
            Vector2 maskCenter = MaskCenter();


            if (Timer < TelegraphTime)
            {
                Vector2 sparkDir = Vector2.UnitX.RotatedByRandom(MathHelper.TwoPi);
                float sparkDistance = (120 * progress) * Main.rand.NextFloat(0.6f, 1.3f);
                Vector2 sparkCenter = maskCenter + sparkDir * sparkDistance * 2;
                float sparkTime = 15;
                Vector2 sparkVel = (maskCenter - sparkCenter) / sparkTime;
                float sparkScale = 2f - progress * 1.2f;
                Particle spark = new SparkParticle(sparkCenter, sparkVel, GlowColor, sparkScale, (int)sparkTime);
                spark.Spawn();
            }
            else if (Timer == TelegraphTime)
            {
                SoundEngine.PlaySound(BigShotSFX, maskCenter);
                int shots = Main.expertMode ? WorldSavingSystem.EternityMode ? WorldSavingSystem.MasochistModeReal ? 12 : 10 : 8 : 6;
                if (FargoSoulsUtil.HostCheck)
                {
                    float baseRot = Main.rand.NextFloat(MathF.Tau);
                    for (int i = 0; i < shots; i++)
                    {
                        float rot = baseRot + MathF.Tau * ((float)i / shots);
                        Vector2 vel = rot.ToRotationVector2() * 4;
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), maskCenter, vel, ModContent.ProjectileType<CoffinWaveShot>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 1f, Main.myPlayer);
                    }
                }
                if (WorldSavingSystem.MasochistModeReal || !PhaseTwo)
                    StateReset();
            }
            else if (Timer > TelegraphTime + (WorldSavingSystem.MasochistModeReal || AI3 < 1 ? 20 : 50)) // + endlag
            {
                if (PhaseTwo && AI3 < 1 && WorldSavingSystem.EternityMode)
                {
                    AI3 = 1;
                    Timer = 0;
                    return;
                }
                StateReset();
            }

        }
        public void WavyShotFlight()
        {
            /*
            int dir = Math.Sign(Player.Center.X - NPC.Center.X);
            NPC.direction = dir == 0 ? 1 : -dir;
            */
            HoverSound();
            NPC.noTileCollide = true;

            ref float RotDir = ref AI2;

            if (Timer == 1)
            {
                RotDir = Main.rand.NextBool() ? 1 : -1;
            }

            const float RotationSpeed = MathF.Tau * 0.18f;
            Vector2 offset = Player.DirectionTo(NPC.Center);

            offset = offset.RotatedBy(RotDir * RotationSpeed) * 350;

            Vector2 desiredPos = Player.Center + offset;
            Movement(desiredPos, 0.1f, 10, 5, 0.08f, 20);

            const int soulStart = 30;
            const int soulEnd = 200;
            /*
            if (Timer > soulStart && Timer < soulEnd)
            {
                Frame = 1;
                float prog = Utils.Clamp(Timer / soulStart, 0, 1);
                NPC.rotation = Vector2.Lerp(NPC.rotation.ToRotationVector2(), NPC.DirectionTo(Player.Center), prog).ToRotation();
            }
            */
            if (Timer == soulEnd / 2 && WorldSavingSystem.EternityMode)
                RotDir *= -1;

            if (Timer > soulStart + 15 && Timer < soulEnd)
            {
                const int ShotTime = 32;

                float progress = (Timer % ShotTime) / ShotTime;
                Vector2 maskCenter = MaskCenter();

                Vector2 sparkDir = Vector2.UnitX.RotatedByRandom(MathHelper.TwoPi);
                float sparkDistance = (120 * progress) * Main.rand.NextFloat(0.6f, 1.3f);
                Vector2 sparkCenter = maskCenter + sparkDir * sparkDistance * 2;
                float sparkTime = 15;
                Vector2 sparkVel = (maskCenter - sparkCenter) / sparkTime;
                float sparkScale = 2f - progress * 1.2f;
                sparkScale /= 2;
                Particle spark = new SparkParticle(sparkCenter, sparkVel, GlowColor, sparkScale, (int)sparkTime);
                spark.Spawn();

                if (Timer % ShotTime == 0)
                {
                    SoundEngine.PlaySound(ShotSFX, NPC.Center);
                    if (FargoSoulsUtil.HostCheck)
                    {
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), maskCenter, maskCenter.DirectionTo(Player.Center) * 4, ModContent.ProjectileType<CoffinWaveShot>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 1f, Main.myPlayer, 1);
                        //if (WorldSavingSystem.EternityMode)
                            //Projectile.NewProjectile(NPC.GetSource_FromThis(), maskCenter, maskCenter.DirectionTo(Player.Center).RotatedBy(RotDir * MathF.Tau * 0.1) * 4, ModContent.ProjectileType<CoffinWaveShot>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 1f, Main.myPlayer, 1);
                    }
                }
            }
            /*
            if (Timer > soulEnd)
            {
                float prog = Utils.Clamp((Timer - soulEnd) / 20, 0, 1);
                NPC.rotation = Vector2.Lerp(NPC.rotation.ToRotationVector2(), Vector2.UnitX, prog).ToRotation();
            }
            */
            if (Timer > soulEnd + 20)
            {
                if (NPC.Center.Y > Player.Center.Y) // only end if above player
                {
                    return;
                }
                NPC.velocity.Y -= 3f;
                Frame = 0;
                State = (float)StateEnum.SlamWShockwave;
                LockVector1 = Player.Top - Vector2.UnitY * 250;
                AI2 = 2; // only bounce once
                Timer = 0;
            }
        }
        public void GrabbyHands()
        {
            NPC.noTileCollide = true;
            HoverSound();
            Vector2 offset = -Vector2.UnitY * 300 + Vector2.UnitX * Math.Sign(NPC.Center.X - Player.Center.X) * 200;
            Vector2 desiredPos = Player.Center + offset;
            Movement(desiredPos, 0.1f, 10, 5, 0.08f, 20);

            if (Timer == 2)
            {
                AI3 = Main.rand.Next(90, 120); // time for hands to grab
                NPC.netUpdate = true;
            }
            if (Timer > 2 && Timer == AI3)
            {
                
                foreach (Projectile hand in Main.projectile.Where(p => p.TypeAlive<CoffinHand>() && p.ai[0] == NPC.whoAmI && p.ai[1] == 1))
                {
                    SoundEngine.PlaySound(HandChargeSFX, hand.Center);
                    hand.ai[1] = 2;
                    hand.netUpdate = true;
                }
            }
            if (Timer < 40)
            {
                if (++NPC.frameCounter % 4 == 3)
                    if (Frame < Main.npcFrameCount[Type] - 1)
                        Frame++;
            }
            else if (Timer == 40)
            {
                SoundEngine.PlaySound(ShotSFX, NPC.Center);
                if (FargoSoulsUtil.HostCheck)
                {
                    Vector2 dir = NPC.rotation.ToRotationVector2();
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, dir * 4, ModContent.ProjectileType<CoffinHand>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage, 0.1f), 1f, Main.myPlayer, NPC.whoAmI, 1, 1);
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, dir * 4, ModContent.ProjectileType<CoffinHand>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage, 0.1f), 1f, Main.myPlayer, NPC.whoAmI, 1, -1);
                    if (WorldSavingSystem.EternityMode)
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, (NPC.rotation + MathHelper.PiOver2).ToRotationVector2() * 4, ModContent.ProjectileType<CoffinHand>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage, 0.5f), 1f, Main.myPlayer, NPC.whoAmI, 1, Main.rand.NextBool() ? 1.5f : -1.5f);
                }
            }
            else
            {
                if (++NPC.frameCounter % 60 == 59 && Frame > 0)
                    Frame--;
                if (Frame <= 0 && Timer > AI3 + 10)
                {
                    NPC.frameCounter = 0;
                    Frame = 0;
                    NPC.TargetClosest(false);
                    Timer = 0;
                    AI2 = 0;
                    AI3 = 0;
                    if (Main.rand.NextBool())
                    {
                        State = (float)StateEnum.SlamWShockwave;
                        NPC.noTileCollide = true;
                        LockVector1 = Player.Top - Vector2.UnitY * 250;
                        NPC.velocity.Y -= 5;
                        NPC.velocity.X /= 2;
                    }
                    else
                    {
                        State = (float)StateEnum.WavyShotFlight;
                        LastAttackChoice = (int)State;
                    }
                    NPC.netUpdate = true;
                }

            }
        }
        public void RandomStuff()
        {
            ref float RandomProj = ref AI3;
            NPC.noTileCollide = true;
            if (Timer < 35)
            {
                HoverSound();
                Vector2 offset = Vector2.UnitX * Math.Sign(NPC.Center.X - Player.Center.X) * 250;
                Vector2 desiredPos = Player.Center + offset;
                Movement(desiredPos, 0.1f, 10, 5, 0.08f, 20);

                Vector2 desiredRot = Vector2.UnitX * Math.Sign(Player.Center.X - NPC.Center.X) - Vector2.UnitY;
                NPC.rotation = Vector2.Lerp(NPC.rotation.ToRotationVector2(), desiredRot, Timer / 35).ToRotation();
            }
            else
            {
                NPC.velocity *= 0.95f;
            }
            if (Timer < 350)
            {
                if (Timer % 20 == 0)
                {
                    RandomProj = Main.rand.Next(3) switch
                    {
                        1 => 5,
                        2 => 6,
                        _ => Main.rand.Next(5)
                    };
                    NPC.netUpdate = true;
                }
                if (Timer % 20 == 19)
                {
                    SoundStyle sound = RandomProj switch
                    {
                        5 => SoundID.Item106,
                        6 => SoundID.NPCHit2,
                        _ => SoundID.Item101
                    };
                    SoundEngine.PlaySound(sound, NPC.Center);
                    if (FargoSoulsUtil.HostCheck)
                    {
                        float gravity = CoffinRandomStuff.Gravity(RandomProj);
                        // we want the proj to end up at player x position in t frames
                        // we also want proj to end up at player's y position position, in an arc
                        // vX * t = xdif -> vX = xdif / t
                        // ydif = a*t^2 / 2 - vY * t  -> vY = a*t / 2 - ydif/t
                        // we also want 45 degree angle, so vX = vY
                        // xdif / t = a*t/2 - ydif/t -> t = sqrt(2 * (xdif+ydif) / a), vX = vY = xdif / t

                        // this is scuffed rn, vertically. fix it. tweak until it works
                        float xDif = Player.Center.X - NPC.Center.X;
                        float yDif = Player.Center.Y - NPC.Center.Y;
                        float travelTime = MathF.Sqrt(2 * (Math.Abs(xDif) + yDif) / gravity);
                        Vector2 vel = Vector2.UnitX * xDif / travelTime - Vector2.UnitY * Math.Abs(xDif) / travelTime;
                        vel *= Main.rand.NextFloat(0.9f, 1.3f);

                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, vel, ModContent.ProjectileType<CoffinRandomStuff>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 1f, Main.myPlayer, RandomProj);
                    }
                }
            }
            else
            {
                if (Timer < 400)
                {
                    NPC.rotation = Vector2.Lerp(NPC.rotation.ToRotationVector2(), 0f.ToRotationVector2(), (Timer - 350) / 50).ToRotation();
                }
                else
                {
                    NPC.rotation = 0;
                    StateReset();
                }
            }
        }
        #endregion
        #region Help Methods
        void HoverSound() => SoundEngine.PlaySound(SoundID.Item24 with { MaxInstances = 1, SoundLimitBehavior = SoundLimitBehavior.IgnoreNew, Pitch = -0.5f, Volume = 10f}, NPC.Center);

        void Movement(Vector2 pos, float accel = 0.03f, float maxSpeed = 20, float lowspeed = 5, float decel = 0.03f, float slowdown = 30)
        {
            if (NPC.Distance(pos) > slowdown)
            {
                NPC.velocity = Vector2.Lerp(NPC.velocity, (pos - NPC.Center).SafeNormalize(Vector2.Zero) * maxSpeed, accel);
            }
            else
            {
                NPC.velocity = Vector2.Lerp(NPC.velocity, (pos - NPC.Center).SafeNormalize(Vector2.Zero) * lowspeed, decel);
            }
        }
        public bool Targeting()
        {
            Player player = Main.player[NPC.target];
            //Targeting
            if (!player.active || player.dead || player.ghost || NPC.Distance(player.Center) > 2400)
            {
                NPC.TargetClosest(false);
                player = Main.player[NPC.target];
                if (!player.active || player.dead || player.ghost || NPC.Distance(player.Center) > 2400)
                {
                    if (NPC.timeLeft > 60)
                        NPC.timeLeft = 60;
                    NPC.velocity.Y -= 0.4f;
                    return false;
                }
            }
            return true;
        }
        public void StateReset()
        {
            NPC.TargetClosest(false);
            RandomizeState();
            Timer = 0;
            AI2 = 0;
            AI3 = 0;
        }
        public void RandomizeState() //it's done this way so it cycles between attacks in a random order: for increased variety
        {
            NPC.netUpdate = true;
            int index;
            if (availablestates.Count < 1)
            {
                availablestates.Clear();
                if (PhaseTwo)
                {
                    foreach (StateEnum state in P2Attacks)
                        availablestates.Add((int)state);
                }
                else
                {
                    foreach (StateEnum state in P1Attacks)
                        availablestates.Add((int)state);
                }
                availablestates.Remove(LastAttackChoice);
            }
            if (FargoSoulsUtil.HostCheck)
            {
                if (!PhaseTwo && NPC.GetLifePercent() <= 0.8f)
                {
                    State = (float)StateEnum.PhaseTransition;
                    availablestates.Clear();
                    return;
                }
                index = Main.rand.Next(availablestates.Count);
                State = LastAttackChoice = availablestates[index];
                availablestates.RemoveAt(index);
            }
            
        }
        #endregion
    }
}
