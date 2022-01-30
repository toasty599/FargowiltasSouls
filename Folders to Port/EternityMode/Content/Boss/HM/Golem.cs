using Fargowiltas.Items.Summons.VanillaCopy;
using FargowiltasSouls.Buffs.Masomode;
using FargowiltasSouls.EternityMode.Net;
using FargowiltasSouls.EternityMode.Net.Strategies;
using FargowiltasSouls.EternityMode.NPCMatching;
using FargowiltasSouls.Items.Accessories.Masomode;
using FargowiltasSouls.NPCs;
using FargowiltasSouls.Projectiles.Deathrays;
using FargowiltasSouls.Projectiles.Masomode;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.EternityMode.Content.Boss.HM
{
    public abstract class GolemPart : EModeNPCBehaviour
    {
        public int HealPerSecond;
        public int HealCounter;

        protected GolemPart(int heal)
        {
            HealPerSecond = heal;
        }

        public override void SetDefaults(NPC npc)
        {
            base.SetDefaults(npc);

            npc.trapImmune = true;
            npc.buffImmune[BuffID.Ichor] = true;
            npc.buffImmune[BuffID.Poisoned] = true;
            npc.buffImmune[BuffID.Suffocation] = true;
            npc.buffImmune[ModContent.BuffType<ClippedWings>()] = true;
        }

        public override bool PreAI(NPC npc)
        {
            if (!FargoSoulsWorld.SwarmActive && !npc.dontTakeDamage && HealPerSecond != 0)
            {
                npc.life += HealPerSecond / 60; //healing stuff
                if (npc.life > npc.lifeMax)
                    npc.life = npc.lifeMax;
                
                if (++HealCounter >= 75)
                {
                    HealCounter = Main.rand.Next(30);
                    CombatText.NewText(npc.Hitbox, CombatText.HealLife, HealPerSecond);
                }
            }

            return base.PreAI(npc);
        }

        public override void OnHitPlayer(NPC npc, Player target, int damage, bool crit)
        {
            base.OnHitPlayer(npc, target, damage, crit);

            target.AddBuff(BuffID.BrokenArmor, 600);
            target.AddBuff(ModContent.BuffType<Defenseless>(), 600);
            target.AddBuff(BuffID.WitheredArmor, 600);
        }

        public override void LoadSprites(NPC npc, bool recolor)
        {
            base.LoadSprites(npc, recolor);

            LoadNPCSprite(recolor, npc.type);
        }
    }

    public class Golem : GolemPart
    {
        public Golem() : base(180) { }

        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.Golem);

        public int StompAttackCounter;
        public int SpikyBallTimer;

        public bool DoStompBehaviour;
        public bool HaveBoostedJumpHeight;
        public bool IsInTemple;

        public bool DroppedSummon;

        public override Dictionary<Ref<object>, CompoundStrategy> GetNetInfo() =>
            new Dictionary<Ref<object>, CompoundStrategy> {
                { new Ref<object>(StompAttackCounter), IntStrategies.CompoundStrategy },
                { new Ref<object>(SpikyBallTimer), IntStrategies.CompoundStrategy },

                { new Ref<object>(DoStompBehaviour), BoolStrategies.CompoundStrategy },
                { new Ref<object>(HaveBoostedJumpHeight), BoolStrategies.CompoundStrategy },
                { new Ref<object>(IsInTemple), BoolStrategies.CompoundStrategy },
            };

        public override void SetDefaults(NPC npc)
        {
            base.SetDefaults(npc);

            npc.lifeMax *= 5;
        }

        public override void AI(NPC npc)
        {
            NPC.golemBoss = npc.whoAmI;

            if (FargoSoulsWorld.SwarmActive)
                return;

            /*if (npc.ai[0] == 0f && npc.velocity.Y == 0f) //manipulating golem jump ai
            {
                if (npc.ai[1] > 0f)
                {
                    npc.ai[1] += 5f; //count up to initiate jump faster
                }
                else
                {
                    float threshold = -2f - (float)Math.Round(18f * npc.life / npc.lifeMax);

                    if (npc.ai[1] < threshold) //jump activates at npc.ai[1] == -1
                        npc.ai[1] = threshold;
                }
            }*/

            if (Main.LocalPlayer.active && Main.LocalPlayer.Distance(npc.Center) < 2000)
                Main.LocalPlayer.AddBuff(ModContent.BuffType<LowGround>(), 2);

            HealPerSecond = FargoSoulsWorld.MasochistModeReal ? 240 : 180;
            if (!IsInTemple) //temple enrage, more horiz move and fast jumps
            {
                HealPerSecond *= 2;
                npc.position.X += npc.velocity.X / 2f;
                if (npc.velocity.Y < 0)
                {
                    npc.position.Y += npc.velocity.Y * 0.5f;
                    if (npc.velocity.Y > -2)
                        npc.velocity.Y = 20;
                }
            }

            if (npc.velocity.Y < 0) //jumping up
            {
                if (!HaveBoostedJumpHeight)
                {
                    HaveBoostedJumpHeight = true;
                    npc.velocity.Y *= 1.25f;

                    if (!IsInTemple) //temple enrage
                    {
                        if (Main.player[npc.target].Center.Y < npc.Center.Y - 16 * 30)
                            npc.velocity.Y *= 1.5f;
                    }
                }
            }
            else
            {
                HaveBoostedJumpHeight = false;
            }

            if (DoStompBehaviour)
            {
                if (npc.velocity.Y == 0f)
                {
                    DoStompBehaviour = false;
                    IsInTemple = Main.tile[(int)npc.Center.X / 16, (int)npc.Center.Y / 16] != null &&
                        Main.tile[(int)npc.Center.X / 16, (int)npc.Center.Y / 16].wall == WallID.LihzahrdBrickUnsafe;

                    if (Main.netMode != NetmodeID.MultiplayerClient) //landing attacks
                    {
                        if (IsInTemple) //in temple
                        {
                            StompAttackCounter++;
                            if (StompAttackCounter == 1) //plant geysers
                            {
                                if (FargoSoulsWorld.MasochistModeReal)
                                    StompAttackCounter++;

                                Vector2 spawnPos = new Vector2(npc.position.X, npc.Center.Y); //floor geysers
                                spawnPos.X -= npc.width * 7;
                                for (int i = 0; i < 6; i++)
                                {
                                    int tilePosX = (int)spawnPos.X / 16 + npc.width * i * 3 / 16;
                                    int tilePosY = (int)spawnPos.Y / 16;// + 1;

                                    Projectile.NewProjectile(tilePosX * 16 + 8, tilePosY * 16 + 8, 0f, 0f, ModContent.ProjectileType<GolemGeyser2>(), npc.damage / 4, 0f, Main.myPlayer, npc.whoAmI);
                                }

                                spawnPos = npc.Center;
                                for (int i = -3; i <= 3; i++) //ceiling geysers
                                {
                                    int tilePosX = (int)spawnPos.X / 16 + npc.width * i * 3 / 16;
                                    int tilePosY = (int)spawnPos.Y / 16;// + 1;

                                    Projectile.NewProjectile(tilePosX * 16 + 8, tilePosY * 16 + 8, 0f, 0f, ModContent.ProjectileType<GolemGeyser>(), npc.damage / 4, 0f, Main.myPlayer, npc.whoAmI);
                                }
                            }
                            else if (StompAttackCounter == 2) //empty jump
                            {

                            }
                            else if (StompAttackCounter == 3) //rocks fall
                            {
                                if (FargoSoulsWorld.MasochistModeReal)
                                    StompAttackCounter = 0;

                                if (npc.HasPlayerTarget)
                                {
                                    for (int i = -2; i <= 2; i++)
                                    {
                                        int tilePosX = (int)Main.player[npc.target].Center.X / 16;
                                        int tilePosY = (int)Main.player[npc.target].Center.Y / 16;// + 1;
                                        tilePosX += 4 * i;

                                        if (Main.tile[tilePosX, tilePosY] == null)
                                            Main.tile[tilePosX, tilePosY] = new Tile();

                                        //first move up through solid tiles
                                        while (Main.tile[tilePosX, tilePosY].nactive() && Main.tileSolid[Main.tile[tilePosX, tilePosY].type])
                                        {
                                            tilePosY--;
                                            if (Main.tile[tilePosX, tilePosY] == null)
                                                Main.tile[tilePosX, tilePosY] = new Tile();
                                        }
                                        //then move up through air until next ceiling reached
                                        while (!(Main.tile[tilePosX, tilePosY].nactive() && Main.tileSolid[Main.tile[tilePosX, tilePosY].type]))
                                        {
                                            tilePosY--;
                                            if (Main.tile[tilePosX, tilePosY] == null)
                                                Main.tile[tilePosX, tilePosY] = new Tile();
                                        }

                                        Vector2 spawn = new Vector2(tilePosX * 16 + 8, tilePosY * 16 + 8);
                                        Projectile.NewProjectile(spawn, Vector2.Zero, ModContent.ProjectileType<GolemBoulder>(), npc.damage / 4, 0f, Main.myPlayer);
                                    }
                                }
                            }
                            else //empty jump
                            {
                                StompAttackCounter = 0;
                            }
                        }
                        else //outside temple
                        {
                            Vector2 spawnPos = new Vector2(npc.position.X, npc.Center.Y);
                            spawnPos.X -= npc.width * 7;
                            for (int i = 0; i < 6; i++)
                            {
                                int tilePosX = (int)spawnPos.X / 16 + npc.width * i * 3 / 16;
                                int tilePosY = (int)spawnPos.Y / 16;// + 1;

                                if (Main.tile[tilePosX, tilePosY] == null)
                                    Main.tile[tilePosX, tilePosY] = new Tile();

                                while (!(Main.tile[tilePosX, tilePosY].nactive() && Main.tileSolid[(int)Main.tile[tilePosX, tilePosY].type]))
                                {
                                    tilePosY++;
                                    if (Main.tile[tilePosX, tilePosY] == null)
                                        Main.tile[tilePosX, tilePosY] = new Tile();
                                }

                                if (npc.HasPlayerTarget && Main.player[npc.target].position.Y > tilePosY * 16)
                                {
                                    Projectile.NewProjectile(tilePosX * 16 + 8, tilePosY * 16 + 8, 6.3f, 6.3f,
                                        ProjectileID.FlamesTrap, npc.damage / 4, 0f, Main.myPlayer);
                                    Projectile.NewProjectile(tilePosX * 16 + 8, tilePosY * 16 + 8, -6.3f, 6.3f,
                                        ProjectileID.FlamesTrap, npc.damage / 4, 0f, Main.myPlayer);
                                }

                                Projectile.NewProjectile(tilePosX * 16 + 8, tilePosY * 16 + 8, 0f, -8f, ProjectileID.GeyserTrap, npc.damage / 4, 0f, Main.myPlayer);

                                Projectile.NewProjectile(tilePosX * 16 + 8, tilePosY * 16 + 8 - 640, 0f, -8f, ProjectileID.GeyserTrap, npc.damage / 4, 0f, Main.myPlayer);
                                Projectile.NewProjectile(tilePosX * 16 + 8, tilePosY * 16 + 8 - 640, 0f, 8f, ProjectileID.GeyserTrap, npc.damage / 4, 0f, Main.myPlayer);
                            }
                            if (npc.HasPlayerTarget)
                            {
                                for (int i = -3; i <= 3; i++)
                                {
                                    int tilePosX = (int)Main.player[npc.target].Center.X / 16;
                                    int tilePosY = (int)Main.player[npc.target].Center.Y / 16;// + 1;
                                    tilePosX += 10 * i;

                                    if (Main.tile[tilePosX, tilePosY] == null)
                                        Main.tile[tilePosX, tilePosY] = new Tile();

                                    for (int j = 0; j < 30; j++)
                                    {
                                        if (Main.tile[tilePosX, tilePosY].nactive() && Main.tileSolid[Main.tile[tilePosX, tilePosY].type])
                                            break;
                                        tilePosY--;
                                        if (Main.tile[tilePosX, tilePosY] == null)
                                            Main.tile[tilePosX, tilePosY] = new Tile();
                                    }

                                    Vector2 spawn = new Vector2(tilePosX * 16 + 8, tilePosY * 16 + 8);
                                    Projectile.NewProjectile(spawn, Vector2.Zero, ModContent.ProjectileType<GolemBoulder>(), npc.damage / 4, 0f, Main.myPlayer);
                                }
                            }
                        }

                        //golem's anti-air fireball spray (whenever he lands while player is below)
                        /*if (npc.HasPlayerTarget && Main.player[npc.target].position.Y > npc.position.Y + npc.height)
                        {
                            float gravity = 0.2f; //shoot down
                            const float time = 60f;
                            Vector2 distance = Main.player[npc.target].Center - npc.Center;
                            distance += Main.player[npc.target].velocity * 45f;
                            distance.X = distance.X / time;
                            distance.Y = distance.Y / time - 0.5f * gravity * time;
                            if (Math.Sign(distance.Y) != Math.Sign(gravity))
                                distance.Y = 0f; //cannot arc shots to hit someone on the same elevation
                            int max = masobool3 ? 1 : 3;
                            for (int i = -max; i <= max; i++)
                            {
                                Projectile.NewProjectile(npc.Center.X, npc.Center.Y, distance.X + i * 1.5f, distance.Y,
                                    ModContent.ProjectileType<GolemFireball>(), npc.damage / 5, 0f, Main.myPlayer, gravity, 0);
                            }
                        }*/
                    }
                }
            }
            else if (npc.velocity.Y > 0)
            {
                DoStompBehaviour = true;
            }

            if (++SpikyBallTimer >= 900) //spray spiky balls
            {
                if (Main.tile[(int)npc.Center.X / 16, (int)npc.Center.Y / 16] != null && //in temple
                    Main.tile[(int)npc.Center.X / 16, (int)npc.Center.Y / 16].wall == WallID.LihzahrdBrickUnsafe)
                {
                    if (npc.velocity.Y > 0) //only when falling, implicitly assume at peak of a jump
                    {
                        SpikyBallTimer = FargoSoulsWorld.MasochistModeReal ? 600 : 0;
                        for (int i = 0; i < 8; i++)
                        {
                            Projectile.NewProjectile(npc.position.X + Main.rand.Next(npc.width), npc.position.Y + Main.rand.Next(npc.height),
                                  Main.rand.NextFloat(-0.3f, 0.3f), Main.rand.NextFloat(-10, -6), ModContent.ProjectileType<GolemSpikyBall>(), npc.damage / 4, 0f, Main.myPlayer);
                        }
                    }
                }
                else //outside temple
                {
                    SpikyBallTimer = 600; //do it more often
                    for (int i = 0; i < 16; i++)
                    {
                        Projectile.NewProjectile(npc.position.X + Main.rand.Next(npc.width), npc.position.Y + Main.rand.Next(npc.height),
                              Main.rand.NextFloat(-1f, 1f), Main.rand.Next(-20, -9), ModContent.ProjectileType<GolemSpikyBall>(), npc.damage / 4, 0f, Main.myPlayer);
                    }
                }
            }

            /*Counter2++;
            if (Counter2 > 240) //golem's anti-air fireball spray (when player is above)
            {
                Counter2 = 0;
                if (npc.HasPlayerTarget && Main.player[npc.target].position.Y < npc.position.Y
                    && Main.netMode != NetmodeID.MultiplayerClient) //shoutouts to arterius
                {
                    bool inTemple = Main.tile[(int)npc.Center.X / 16, (int)npc.Center.Y / 16] != null && //in temple
                        Main.tile[(int)npc.Center.X / 16, (int)npc.Center.Y / 16].wall == WallID.LihzahrdBrickUnsafe;

                    float gravity = -0.2f; //normally floats up
                    //if (Main.player[npc.target].position.Y > npc.position.Y + npc.height) gravity *= -1f; //aim down if player below golem
                    const float time = 60f;
                    Vector2 distance = Main.player[npc.target].Center - npc.Center;
                    distance += Main.player[npc.target].velocity * 45f;
                    distance.X = distance.X / time;
                    distance.Y = distance.Y / time - 0.5f * gravity * time;
                    if (Math.Sign(distance.Y) != Math.Sign(gravity))
                        distance.Y = 0f; //cannot arc shots to hit someone on the same elevation
                    int max = inTemple ? 1 : 3;
                    for (int i = -max; i <= max; i++)
                    {
                        Projectile.NewProjectile(npc.Center.X, npc.Center.Y, distance.X + i, distance.Y,
                            ModContent.ProjectileType<GolemFireball>(), npc.damage / 5, 0f, Main.myPlayer, gravity, 0);
                    }
                }
            }*/

            EModeUtils.DropSummon(npc, ModContent.ItemType<LihzahrdPowerCell2>(), NPC.downedGolemBoss, ref DroppedSummon, NPC.downedPlantBoss);
        }

        public override bool StrikeNPC(NPC npc, ref double damage, int defense, ref float knockback, int hitDirection, ref bool crit)
        {
            damage *= 0.9;

            return base.StrikeNPC(npc, ref damage, defense, ref knockback, hitDirection, ref crit);
        }

        public override void NPCLoot(NPC npc)
        {
            base.NPCLoot(npc);

            npc.DropItemInstanced(npc.position, npc.Size, ModContent.ItemType<LihzahrdTreasureBox>());
            npc.DropItemInstanced(npc.position, npc.Size, ItemID.GoldenCrate, 5);
        }

        public override void LoadSprites(NPC npc, bool recolor)
        {
            base.LoadSprites(npc, recolor);
            
            for (int i = 1; i <= 3; i++)
                Main.golemTexture[i] = LoadSprite(recolor, $"GolemLights{i}");
        }
    }

    public class GolemFist : GolemPart
    {
        public GolemFist() : base(9999) { }

        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchTypeRange(NPCID.GolemFistLeft, NPCID.GolemFistRight);

        public int FistAttackRateSlowdownTimer;

        public bool DoAttackOnFistImpact;

        public override Dictionary<Ref<object>, CompoundStrategy> GetNetInfo() =>
            new Dictionary<Ref<object>, CompoundStrategy> {
                { new Ref<object>(FistAttackRateSlowdownTimer), IntStrategies.CompoundStrategy },

                { new Ref<object>(DoAttackOnFistImpact), BoolStrategies.CompoundStrategy },
            };

        public override void SetDefaults(NPC npc)
        {
            base.SetDefaults(npc);

            npc.scale += 0.5f;
        }

        public override void AI(NPC npc)
        {
            if (FargoSoulsWorld.SwarmActive)
                return;

            if (npc.HasValidTarget && Framing.GetTileSafely(Main.player[npc.target].Center).wall == WallID.LihzahrdBrickUnsafe)
            {
                if (npc.ai[0] == 1) //on the tick it shoots out, reset counter
                {
                    FistAttackRateSlowdownTimer = 0;
                }
                else
                {
                    if (++FistAttackRateSlowdownTimer < 90) //this basically tracks total time since punch started
                    {
                        npc.ai[1] = 0; //don't allow attacking until counter finishes counting up
                    }
                }

                if (npc.velocity.Length() > 10)
                    npc.position -= Vector2.Normalize(npc.velocity) * (npc.velocity.Length() - 10);
            }

            if (npc.ai[0] == 0f && DoAttackOnFistImpact)
            {
                DoAttackOnFistImpact = false;
                if (Framing.GetTileSafely(Main.player[npc.target].Center).wall != WallID.LihzahrdBrickUnsafe || FargoSoulsWorld.MasochistModeReal)
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                        Projectile.NewProjectile(npc.Center, Vector2.Zero, ModContent.ProjectileType<MoonLordSunBlast>(), npc.damage / 4, 0f, Main.myPlayer);
                }
            }
            DoAttackOnFistImpact = npc.ai[0] != 0f;

            NPC golem = FargoSoulsUtil.NPCExists(NPC.golemBoss, NPCID.Golem);
            if (golem != null)
            {
                if (npc.ai[0] == 0 && npc.Distance(golem.Center) < golem.width * 1.5f) //when attached to body
                    npc.position += golem.velocity; //stick to body better, dont get left behind during jumps
            }
        }

        public override void OnHitByProjectile(NPC npc, Projectile projectile, int damage, float knockback, bool crit)
        {
            base.OnHitByProjectile(npc, projectile, damage, knockback, crit);

            if (projectile.maxPenetrate != 1 && FargoSoulsUtil.CanDeleteProjectile(projectile))
                projectile.timeLeft = 0;
        }
    }

    public class GolemHead : GolemPart
    {
        public GolemHead() : base(180) { }

        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchTypeRange(NPCID.GolemHead, NPCID.GolemHeadFree);

        public int DeathrayAITimer;
        public int DeathraySweepTargetHeight;

        public float SuppressedAi1;
        public float SuppressedAi2;

        public bool ShootDeathray;
        public bool SweepToLeft;
        public bool IsInTemple;

        public override Dictionary<Ref<object>, CompoundStrategy> GetNetInfo() =>
            new Dictionary<Ref<object>, CompoundStrategy> {
                { new Ref<object>(DeathrayAITimer), IntStrategies.CompoundStrategy },
                { new Ref<object>(DeathraySweepTargetHeight), IntStrategies.CompoundStrategy },

                { new Ref<object>(SuppressedAi1), FloatStrategies.CompoundStrategy },
                { new Ref<object>(SuppressedAi2), FloatStrategies.CompoundStrategy },

                { new Ref<object>(ShootDeathray), BoolStrategies.CompoundStrategy },
                { new Ref<object>(SweepToLeft), BoolStrategies.CompoundStrategy },
                { new Ref<object>(IsInTemple), BoolStrategies.CompoundStrategy },
            };

        public override void SetDefaults(NPC npc)
        {
            base.SetDefaults(npc);

            DeathrayAITimer = 540;
        }

        public override bool CanHitPlayer(NPC npc, Player target, ref int CooldownSlot)
        {
            return base.CanHitPlayer(npc, target, ref CooldownSlot) && npc.type != NPCID.GolemHeadFree;
        }

        public override bool PreAI(NPC npc)
        {
            bool result = base.PreAI(npc);

            if (FargoSoulsWorld.SwarmActive)
                return result;

            NPC golem = FargoSoulsUtil.NPCExists(NPC.golemBoss, NPCID.Golem);

            if (npc.type == NPCID.GolemHead)
            {
                npc.dontTakeDamage = false;

                if (golem != null)
                    npc.position += golem.velocity;
            }
            else //detatched head
            {
                if (!ShootDeathray) //default mode
                {
                    npc.position += npc.velocity * 0.25f;
                    npc.position.Y += npc.velocity.Y * 0.25f;

                    if (!npc.noTileCollide && npc.HasValidTarget && Collision.SolidCollision(npc.position, npc.width, npc.height)) //unstick from walls
                        npc.position += npc.DirectionTo(Main.player[npc.target].Center) * 4;

                    //disable attacks when nearby
                    if (npc.HasValidTarget && npc.Distance(Main.player[npc.target].Center) < 350 && !FargoSoulsWorld.MasochistModeReal)
                    {
                        if (SuppressedAi1 < npc.ai[1])
                            SuppressedAi1 = npc.ai[1];
                        npc.ai[1] = 0f;

                        if (SuppressedAi2 < npc.ai[2])
                            SuppressedAi2 = npc.ai[2];
                        npc.ai[2] = 0f;
                    }
                    else
                    {
                        if (npc.ai[1] < SuppressedAi1)
                            npc.ai[1] = SuppressedAi1;
                        SuppressedAi1 = 0;

                        if (npc.ai[2] < SuppressedAi2)
                            npc.ai[2] = SuppressedAi2;
                        SuppressedAi2 = 0;
                    }

                    if (++DeathrayAITimer > 540)
                    {
                        DeathrayAITimer = 0;
                        DeathraySweepTargetHeight = 0;
                        ShootDeathray = true;
                        IsInTemple = Framing.GetTileSafely(npc.Center).wall == WallID.LihzahrdBrickUnsafe; //is in temple
                        npc.netUpdate = true;
                        NetSync(npc);
                    }
                }
                else //deathray time
                {
                    if (golem == null)
                    {
                        npc.StrikeNPCNoInteraction(npc.lifeMax, 0f, 0); //die if golem is dead
                        return false;
                    }

                    npc.noTileCollide = true;

                    const int fireTime = 120;

                    npc.localAI[0] = DeathrayAITimer > fireTime ? 1f : 0f; //mouth animations

                    if (++DeathrayAITimer < fireTime) //move to above golem
                    {
                        if (DeathrayAITimer == 1)
                            SoundEngine.PlaySound(SoundID.Roar, npc.Center, 0);

                        Vector2 target = golem.Center;
                        target.Y -= 250;
                        if (target.Y > DeathraySweepTargetHeight) //stores lowest remembered golem position
                            DeathraySweepTargetHeight = (int)target.Y;
                        target.Y = DeathraySweepTargetHeight;
                        if (npc.HasPlayerTarget && Main.player[npc.target].position.Y < target.Y)
                            target.Y = Main.player[npc.target].position.Y;
                        /*if (masobool2) //in temple
                        {
                            target.Y -= 250;
                            if (target.Y > Counter2) //counter2 stores lowest remembered golem position
                                Counter2 = (int)target.Y;
                            target.Y = Counter2;
                        }
                        else if (npc.HasPlayerTarget)
                        {
                            target.Y = Main.player[npc.target].Center.Y - 250;
                        }*/
                        npc.velocity = (target - npc.Center) / 30;
                    }
                    else if (DeathrayAITimer == fireTime) //fire deathray
                    {
                        npc.velocity = Vector2.Zero;
                        if (npc.HasPlayerTarget) //stores if player is on head's left at this moment
                            SweepToLeft = Main.player[npc.target].Center.X < npc.Center.X;
                        npc.netUpdate = true;
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                            Projectile.NewProjectile(npc.Center, Vector2.UnitY, ModContent.ProjectileType<PhantasmalDeathrayGolem>(), npc.damage / 4, 0f, Main.myPlayer, 0f, npc.whoAmI);
                    }
                    else if (DeathrayAITimer < fireTime + 20)
                    {
                        //do nothing
                    }
                    else if (DeathrayAITimer < fireTime + 150)
                    {
                        npc.velocity.X += SweepToLeft ? -.15f : .15f;

                        Tile tile = Framing.GetTileSafely(npc.Center); //stop if reached a wall, but only 1sec after started firing
                        if (DeathrayAITimer > fireTime + 60 && (tile.nactive() && tile.type == TileID.LihzahrdBrick && tile.wall == WallID.LihzahrdBrickUnsafe)
                            || (IsInTemple && tile.wall != WallID.LihzahrdBrickUnsafe)) //i.e. started in temple but has left temple, then stop
                        {
                            npc.velocity = Vector2.Zero;
                            npc.netUpdate = true;
                            DeathrayAITimer = 0;
                            DeathraySweepTargetHeight = 0;
                            ShootDeathray = false;
                        }
                    }
                    else
                    {
                        npc.velocity = Vector2.Zero;
                        npc.netUpdate = true;
                        DeathrayAITimer = 0;
                        DeathraySweepTargetHeight = 0;
                        ShootDeathray = false;
                    }

                    if (!FargoSoulsWorld.MasochistModeReal)
                    {
                        const float geyserTiming = 100;
                        if (DeathrayAITimer % geyserTiming == geyserTiming - 5)
                        {
                            Vector2 spawnPos = golem.Center;
                            float offset = DeathrayAITimer % (geyserTiming * 2) == geyserTiming - 5 ? 0 : 0.5f;
                            for (int i = -3; i <= 3; i++) //ceiling geysers
                            {
                                int tilePosX = (int)(spawnPos.X / 16 + golem.width * (i + offset) * 3 / 16);
                                int tilePosY = (int)spawnPos.Y / 16;// + 1;

                                int type = IsInTemple ? ModContent.ProjectileType<GolemGeyser>() : ModContent.ProjectileType<GolemGeyser2>();
                                Projectile.NewProjectile(tilePosX * 16 + 8, tilePosY * 16 + 8, 0f, 0f, type, golem.damage / 4, 0f, Main.myPlayer, golem.whoAmI);
                            }
                        }

                        if (IsInTemple) //nerf golem movement during deathray dash, provided we're in temple
                        {
                            if (golem.HasValidTarget)
                            {
                                //golem.velocity.X = 0f;

                                if (golem.ai[0] == 0f && golem.velocity.Y == 0f && golem.ai[1] > 1f) //if golem is standing on ground and preparing to jump, stall it
                                    golem.ai[1] = 1f;

                                golem.GetEModeNPCMod<Golem>().DoStompBehaviour = false; //disable stomp attacks
                            }
                        }
                    }

                    if (!ShootDeathray && Main.netMode != NetmodeID.MultiplayerClient) //spray lasers after dash
                    {
                        int max = IsInTemple ? 6 : 10;
                        int speed = IsInTemple ? 6 : -12; //down in temple, up outside it
                        for (int i = -max; i <= max; i++)
                        {
                            int p = Projectile.NewProjectile(npc.Center, speed * Vector2.UnitY.RotatedBy(Math.PI / 2 / max * i),
                                ModContent.ProjectileType<EyeBeam2>(), npc.damage / 4, 0f, Main.myPlayer);
                            if (p != Main.maxProjectiles)
                                Main.projectile[p].timeLeft = 1200;
                        }
                    }

                    if (npc.netUpdate)
                    {
                        npc.netUpdate = false;
                        if (Main.netMode == NetmodeID.Server)
                            NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, npc.whoAmI);
                        NetSync(npc);
                    }
                    return false;
                }
            }

            return result;
        }
    }
}
