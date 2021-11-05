using Fargowiltas.Items.Summons.Mutant;
using FargowiltasSouls.EternityMode.Net;
using FargowiltasSouls.EternityMode.Net.Strategies;
using FargowiltasSouls.EternityMode.NPCMatching;
using FargowiltasSouls.Buffs.Masomode;
using FargowiltasSouls.Items.Accessories.Masomode;
using FargowiltasSouls.NPCs;
using FargowiltasSouls.Projectiles;
using FargowiltasSouls.Projectiles.Deathrays;
using FargowiltasSouls.Projectiles.Masomode;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using FargowiltasSouls.Items.Misc;
using Fargowiltas.Items.Misc;

namespace FargowiltasSouls.EternityMode.Content.Boss.PHM
{
    public class WallofFlesh : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.WallofFlesh);

        public int WorldEvilAttackCycleTimer = 600;
        public int ChainBarrageTimer;

        public bool UseCorruptAttack;
        public bool InPhase2;
        public bool InDesperationPhase;

        public bool DroppedSummon;

        public override Dictionary<Ref<object>, CompoundStrategy> GetNetInfo() =>
            new Dictionary<Ref<object>, CompoundStrategy> {
                { new Ref<object>(WorldEvilAttackCycleTimer), IntStrategies.CompoundStrategy },
                { new Ref<object>(ChainBarrageTimer), IntStrategies.CompoundStrategy },

                { new Ref<object>(UseCorruptAttack), BoolStrategies.CompoundStrategy },
                { new Ref<object>(InPhase2), BoolStrategies.CompoundStrategy },
                { new Ref<object>(InDesperationPhase), BoolStrategies.CompoundStrategy },
            };

        public override void SetDefaults(NPC npc)
        {
            base.SetDefaults(npc);

            npc.lifeMax = (int)(npc.lifeMax * 1.5);
            npc.defense = 0;
            npc.HitSound = SoundID.NPCHit41;
            npc.buffImmune[BuffID.OnFire] = true;
        }

        public override void AI(NPC npc)
        {
            base.AI(npc);

            EModeGlobalNPC.wallBoss = npc.whoAmI;

            if (FargoSoulsWorld.SwarmActive)
                return;

            if (npc.ai[3] == 0f) //when spawned in, make one eye invul
            {
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    if (Main.npc[i].active && Main.npc[i].type == NPCID.WallofFleshEye && Main.npc[i].realLife == npc.whoAmI)
                    {
                        Main.npc[i].ai[2] = -1f;
                        Main.npc[i].netUpdate = true;

                        npc.ai[3] = 1f;
                        npc.netUpdate = true;
                        break;
                    }
                }
            }

            if (InPhase2) //phase 2
            {
                if (++WorldEvilAttackCycleTimer > 600)
                {
                    WorldEvilAttackCycleTimer = 0;
                    UseCorruptAttack = !UseCorruptAttack;

                    npc.netUpdate = true;
                    NetSync(npc);
                }
                else if (WorldEvilAttackCycleTimer > 600 - 120) //telegraph for special attacks
                {
                    int type = !UseCorruptAttack ? 75 : 170;
                    int speed = !UseCorruptAttack ? 10 : 4;
                    float scale = !UseCorruptAttack ? 6f : 5f;
                    int d = Dust.NewDust(npc.Center + Vector2.UnitX * Math.Sign(npc.velocity.X) * 32f, 0, 0, type, speed * Math.Sign(npc.velocity.X), 0, 100, Color.White, scale);
                    Main.dust[d].velocity *= 12f;
                    Main.dust[d].noGravity = true;
                }
                else if (WorldEvilAttackCycleTimer < 240) //special attacks
                {
                    if (UseCorruptAttack) //cursed inferno attack
                    {
                        if (WorldEvilAttackCycleTimer == 10 && Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            Projectile.NewProjectile(npc.Center, Vector2.UnitY, ModContent.ProjectileType<CursedDeathrayWOFS>(), 0, 0f, Main.myPlayer, npc.direction, npc.whoAmI);
                        }

                        if (WorldEvilAttackCycleTimer % 4 == 0)
                        {
                            float xDistance = (2500f - 1800f * WorldEvilAttackCycleTimer / 240f) * Math.Sign(npc.velocity.X);
                            Vector2 spawnPos = new Vector2(npc.Center.X + xDistance, npc.Center.Y);

                            Main.PlaySound(SoundID.Item34, spawnPos);

                            const int offsetY = 800;
                            const int speed = 14;
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                Projectile.NewProjectile(spawnPos + Vector2.UnitY * offsetY, Vector2.UnitY * -speed, ModContent.ProjectileType<CursedFlamethrower>(), npc.damage / 4, 0f, Main.myPlayer);
                                Projectile.NewProjectile(spawnPos + Vector2.UnitY * offsetY / 2, Vector2.UnitY * speed, ModContent.ProjectileType<CursedFlamethrower>(), npc.damage / 4, 0f, Main.myPlayer);
                                Projectile.NewProjectile(spawnPos + Vector2.UnitY * -offsetY / 2, Vector2.UnitY * -speed, ModContent.ProjectileType<CursedFlamethrower>(), npc.damage / 4, 0f, Main.myPlayer);
                                Projectile.NewProjectile(spawnPos + Vector2.UnitY * -offsetY, Vector2.UnitY * speed, ModContent.ProjectileType<CursedFlamethrower>(), npc.damage / 4, 0f, Main.myPlayer);
                            }
                        }
                    }
                    else //ichor attack
                    {
                        if (WorldEvilAttackCycleTimer % 8 == 0)
                        {
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                for (int i = 0; i < 8; i++)
                                {
                                    Vector2 target = npc.Center;
                                    target.X += Math.Sign(npc.velocity.X) * 1000f * WorldEvilAttackCycleTimer / 240f; //gradually targets further and further
                                    target.X += Main.rand.NextFloat(-100, 100);
                                    target.Y += Main.rand.NextFloat(-450, 450);

                                    const float gravity = 0.5f;
                                    float time = 60f;
                                    Vector2 distance = target - npc.Center;
                                    distance.X = distance.X / time;
                                    distance.Y = distance.Y / time - 0.5f * gravity * time;

                                    Projectile.NewProjectile(npc.Center + Vector2.UnitX * Math.Sign(npc.velocity.X) * 32f, distance,
                                        ModContent.ProjectileType<GoldenShowerWOF>(), npc.damage / 4, 0f, Main.myPlayer, time);
                                }
                            }
                        }
                    }
                }
            }
            else if (npc.life < npc.lifeMax * .75) //enter phase 2
            {
                InPhase2 = true;
                npc.netUpdate = true;
                NetSync(npc);

                if (!Main.dedServ)
                {
                    Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Monster94"),
                        npc.HasValidTarget && Main.player[npc.target].ZoneUnderworldHeight ? Main.player[npc.target].Center : npc.Center);

                    if (Main.LocalPlayer.active)
                        Main.LocalPlayer.GetModPlayer<FargoPlayer>().Screenshake = 90;
                }
            }

            if (npc.ai[3] == 2) //phase 3
            {
                if (InDesperationPhase)
                {
                    //ChainBarrageTimer -= 0.5f; //increment faster

                    if (WorldEvilAttackCycleTimer % 2 == 1) //always make sure its even in here
                        WorldEvilAttackCycleTimer--;
                }

                int floor = 240 - (InDesperationPhase ? 120 : 0);
                int ceiling = 600 - 180 - (InDesperationPhase ? 120 : 0);

                if (WorldEvilAttackCycleTimer >= floor && WorldEvilAttackCycleTimer <= ceiling)
                {
                    if (--ChainBarrageTimer < 0)
                    {
                        ChainBarrageTimer = 80;
                        if (npc.HasValidTarget && Main.player[npc.target].ZoneUnderworldHeight)
                        {
                            if (Main.netMode != NetmodeID.MultiplayerClient) //spawn reticles for chain barrages
                            {
                                Vector2 spawnPos = Main.player[npc.target].Center;

                                float offset = 1000f * (ceiling - WorldEvilAttackCycleTimer) / (ceiling - floor); //progress further as attack continues
                                spawnPos.X += Math.Sign(npc.velocity.X) * offset;
                                spawnPos.Y += Main.rand.NextFloat(-100, 100);

                                if (spawnPos.Y / 16 < Main.maxTilesY - 200) //clamp so it stays in hell
                                    spawnPos.Y = (Main.maxTilesY - 200) * 16;
                                if (spawnPos.Y / 16 >= Main.maxTilesY)
                                    spawnPos.Y = Main.maxTilesY * 16 - 16;
                                Projectile.NewProjectile(spawnPos, Vector2.Zero, ModContent.ProjectileType<WOFReticle>(), npc.damage / 6, 0f, Main.myPlayer);
                            }
                        }
                    }
                }
                else
                {
                    ChainBarrageTimer = 0;
                }
            }
            else if (npc.ai[3] == 1 && npc.life < npc.lifeMax * .5) //enter phase 3
            {
                npc.ai[3] = 2;
                npc.netUpdate = true;
                NetSync(npc);

                if (!Main.dedServ)
                {
                    Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Monster94"),
                        npc.HasValidTarget && Main.player[npc.target].ZoneUnderworldHeight ? Main.player[npc.target].Center : npc.Center);

                    if (Main.LocalPlayer.active)
                        Main.LocalPlayer.GetModPlayer<FargoPlayer>().Screenshake = 90;
                }
            }

            if (npc.life < npc.lifeMax / 10) //final phase
            {
                WorldEvilAttackCycleTimer++;

                if (!InDesperationPhase)
                {
                    InDesperationPhase = true;

                    //temporarily stop eyes from attacking during the transition to avoid accidental insta-lasers
                    for (int i = 0; i < Main.maxNPCs; i++)
                    {
                        if (Main.npc[i].active && Main.npc[i].type == NPCID.WallofFleshEye && Main.npc[i].realLife == npc.whoAmI)
                        {
                            Main.npc[i].GetEModeNPCMod<WallofFleshEye>().PreventAttacks = 60;
                        }
                    }

                    npc.netUpdate = true;
                    NetSync(npc);
                    
                    if (!Main.dedServ)
                    {
                        Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Monster5").WithVolume(1.5f),
                            npc.HasValidTarget && Main.player[npc.target].ZoneUnderworldHeight ? Main.player[npc.target].Center : npc.Center);

                        if (Main.LocalPlayer.active)
                            Main.LocalPlayer.GetModPlayer<FargoPlayer>().Screenshake = 180;
                    }
                }
            }

            const float maxSpeed = 3.5f; //don't let wof move faster than this normally
            if (npc.HasPlayerTarget && (Main.player[npc.target].dead || Vector2.Distance(npc.Center, Main.player[npc.target].Center) > 3000))
            {
                npc.TargetClosest(true);
                if (Main.player[npc.target].dead || Vector2.Distance(npc.Center, Main.player[npc.target].Center) > 3000)
                {
                    npc.position.X += 60 * Math.Sign(npc.velocity.X); //move faster to despawn
                }
                else if (Math.Abs(npc.velocity.X) > maxSpeed)
                {
                    npc.position.X -= (Math.Abs(npc.velocity.X) - maxSpeed) * Math.Sign(npc.velocity.X);
                }
            }
            else if (Math.Abs(npc.velocity.X) > maxSpeed)
            {
                npc.position.X -= (Math.Abs(npc.velocity.X) - maxSpeed) * Math.Sign(npc.velocity.X);
            }
            
            if (Main.LocalPlayer.active & !Main.LocalPlayer.dead && !Main.LocalPlayer.ghost && Main.LocalPlayer.ZoneUnderworldHeight)
            {
                float velX = npc.velocity.X;
                if (velX > maxSpeed)
                    velX = maxSpeed;
                else if (velX < -maxSpeed)
                    velX = -maxSpeed;

                for (int i = 0; i < 10; i++) //dust
                {
                    Vector2 dustPos = new Vector2(2000 * npc.direction, 0f).RotatedBy(Math.PI / 3 * (-0.5 + Main.rand.NextDouble()));
                    int d = Dust.NewDust(npc.Center + dustPos, 0, 0, DustID.Fire);
                    Main.dust[d].scale += 1f;
                    Main.dust[d].velocity.X = velX;
                    Main.dust[d].velocity.Y = npc.velocity.Y;
                    Main.dust[d].noGravity = true;
                    Main.dust[d].noLight = true;
                }

                if (++npc.localAI[1] > 15f)
                {
                    npc.localAI[1] = 0f; //tongue the player if they're 2000-2800 units away
                    if (Math.Abs(2400 - npc.Distance(Main.LocalPlayer.Center)) < 400)
                    {
                        if (!Main.LocalPlayer.tongued)
                            Main.PlaySound(SoundID.ForceRoar, Main.LocalPlayer.Center, -1); //eoc roar
                        Main.LocalPlayer.AddBuff(BuffID.TheTongue, 10);
                    }
                }
            }

            EModeUtils.DropSummon(npc, ModContent.ItemType<FleshyDoll>(), Main.hardMode, ref DroppedSummon);
        }

        public override void NPCLoot(NPC npc)
        {
            base.NPCLoot(npc);

            npc.DropItemInstanced(npc.position, npc.Size, ModContent.ItemType<PungentEyeball>());
            npc.DropItemInstanced(npc.position, npc.Size, ItemID.HallowedFishingCrate, 5);
            npc.DropItemInstanced(npc.position, npc.Size, ModContent.ItemType<ShadowCrate>(), 5);

            if (!Main.LocalPlayer.GetModPlayer<FargoPlayer>().MutantsDiscountCard)
                npc.DropItemInstanced(npc.position, npc.Size, ModContent.ItemType<MutantsDiscountCard>());
        }

        public override void OnHitPlayer(NPC npc, Player target, int damage, bool crit)
        {
            base.OnHitPlayer(npc, target, damage, crit);

            target.AddBuff(BuffID.Burning, 300);
        }

        public override bool StrikeNPC(NPC npc, ref double damage, int defense, ref float knockback, int hitDirection, ref bool crit)
        {
            damage /= 3;
            return base.StrikeNPC(npc, ref damage, defense, ref knockback, hitDirection, ref crit);
        }

        public override void LoadSprites(NPC npc, bool recolor)
        {
            base.LoadSprites(npc, recolor);

            LoadNPCSprite(recolor, npc.type);
            LoadBossHeadSprite(recolor, 22);
            LoadGoreRange(recolor, 132, 142);

            Main.chain12Texture = LoadSprite(recolor, "Chain12");
            Main.wofTexture = LoadSprite(recolor, "WallOfFlesh");
        }
    }

    public class WallofFleshEye : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.WallofFleshEye);

        public int PreventAttacks;

        public bool RepeatingAI;
        public bool HasTelegraphedNormalLasers;

        public override Dictionary<Ref<object>, CompoundStrategy> GetNetInfo() =>
            new Dictionary<Ref<object>, CompoundStrategy> {
                { new Ref<object>(PreventAttacks), IntStrategies.CompoundStrategy },

                { new Ref<object>(RepeatingAI), BoolStrategies.CompoundStrategy },
                { new Ref<object>(HasTelegraphedNormalLasers), BoolStrategies.CompoundStrategy },
            };

        public override void SetDefaults(NPC npc)
        {
            base.SetDefaults(npc);

            npc.lifeMax = (int)(npc.lifeMax * 1.5);
            npc.buffImmune[BuffID.OnFire] = true;
            npc.buffImmune[ModContent.BuffType<ClippedWings>()] = true;
        }

        public override bool PreAI(NPC npc)
        {
            NPC mouth = FargoSoulsUtil.NPCExists(npc.realLife, NPCID.WallofFlesh);
            if (FargoSoulsWorld.SwarmActive || RepeatingAI || mouth == null)
                return true;

            if (PreventAttacks > 0)
                PreventAttacks--;

            float maxTime = 540f;

            if (mouth.GetEModeNPCMod<WallofFlesh>().InDesperationPhase)
            {
                if (npc.ai[1] < maxTime - 180) //dont lower this if it's already telegraphing laser
                    maxTime = 240f;

                npc.localAI[1] = -1f; //no more regular lasers
                npc.localAI[2] = 0f;
            }

            if (++npc.ai[1] >= maxTime)
            {
                npc.ai[1] = 0f;
                if (npc.ai[2] == 0f)
                    npc.ai[2] = 1f;
                else
                    npc.ai[2] *= -1f;

                if (npc.ai[2] > 0) //FIRE LASER
                {
                    Vector2 speed = Vector2.UnitX.RotatedBy(npc.ai[3]);
                    if (Main.netMode != NetmodeID.MultiplayerClient && PreventAttacks <= 0)
                        Projectile.NewProjectile(npc.Center, speed, ModContent.ProjectileType<PhantasmalDeathrayWOF>(), npc.damage / 4, 0f, Main.myPlayer, 0, npc.whoAmI);
                }
                else //ring dust to denote i am vulnerable now
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                        Projectile.NewProjectile(npc.Center, Vector2.Zero, ModContent.ProjectileType<GlowRing>(), 0, 0f, Main.myPlayer, npc.whoAmI, npc.type);
                }

                npc.netUpdate = true;
                NetSync(npc);
            }

            if (npc.ai[2] >= 0f)
            {
                npc.alpha = 175;
                npc.dontTakeDamage = true;

                if (npc.ai[1] <= 90) //still firing laser rn
                {
                    RepeatingAI = true;
                    npc.AI();
                    RepeatingAI = false;

                    npc.localAI[1] = -1f;
                    npc.localAI[2] = 0f;

                    npc.rotation = npc.ai[3];
                    return false;
                }
                else
                {
                    npc.ai[2] = 1;
                }
            }
            else
            {
                npc.alpha = 0;
                npc.dontTakeDamage = false;

                if (npc.ai[1] == maxTime - 3 * 5 && Main.netMode != NetmodeID.MultiplayerClient)
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient && PreventAttacks <= 0)
                    {
                        float ai0 = (npc.realLife != -1 && Main.npc[npc.realLife].velocity.X > 0) ? 1f : 0f;
                        Projectile.NewProjectile(npc.Center, Vector2.Zero, ModContent.ProjectileType<WOFBlast>(), 0, 0f, Main.myPlayer, ai0, npc.whoAmI);
                    }
                }

                if (npc.ai[1] > maxTime - 180f)
                {
                    if (Main.rand.Next(4) < 3) //dust telegraphs switch
                    {
                        int dust = Dust.NewDust(npc.position - new Vector2(2f, 2f), npc.width + 4, npc.height + 4, 88, npc.velocity.X * 0.4f, npc.velocity.Y * 0.4f, 114, default(Color), 3.5f);
                        Main.dust[dust].noGravity = true;
                        Main.dust[dust].velocity *= 1.8f;
                        Main.dust[dust].velocity.Y -= 0.5f;
                        if (Main.rand.Next(4) == 0)
                        {
                            Main.dust[dust].noGravity = false;
                            Main.dust[dust].scale *= 0.5f;
                        }
                    }

                    float stopTime = maxTime - 90f;
                    if (npc.ai[1] == stopTime) //shoot warning dust in phase 2
                    {
                        int t = npc.HasPlayerTarget ? npc.target : npc.FindClosestPlayer();
                        if (t != -1)
                        {
                            if (npc.Distance(Main.player[t].Center) < 3000)
                                Main.PlaySound(SoundID.Roar, (int)Main.player[t].position.X, (int)Main.player[t].position.Y, 0);
                            npc.ai[2] = -2f;
                            npc.ai[3] = (npc.Center - Main.player[t].Center).ToRotation();
                            if (npc.realLife != -1 && Main.npc[npc.realLife].velocity.X > 0)
                                npc.ai[3] += (float)Math.PI;
                            
                            Vector2 speed = Vector2.UnitX.RotatedBy(npc.ai[3]);
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                                Projectile.NewProjectile(npc.Center, speed, ModContent.ProjectileType<PhantasmalDeathrayWOFS>(), 0, 0f, Main.myPlayer, 0, npc.whoAmI);
                        }

                        npc.netUpdate = true;
                        NetSync(npc);
                    }
                    else if (npc.ai[1] > stopTime)
                    {
                        HasTelegraphedNormalLasers = false;

                        RepeatingAI = true;
                        npc.AI();
                        RepeatingAI = false;

                        npc.localAI[1] = -1f;
                        npc.localAI[2] = 0f;

                        npc.rotation = npc.ai[3];
                        return false;
                    }
                }
            }
            
            //dont fire during mouth's special attacks (this is at bottom to override others)
            if ((mouth.GetEModeNPCMod<WallofFlesh>().InPhase2 && mouth.GetEModeNPCMod<WallofFlesh>().WorldEvilAttackCycleTimer < 240) || mouth.GetEModeNPCMod<WallofFlesh>().InDesperationPhase)
            {
                npc.localAI[1] = -90f;
                npc.localAI[2] = 0f;

                HasTelegraphedNormalLasers = false;
            }

            if (npc.localAI[2] > 1) //has shot at least one laser
            {
                HasTelegraphedNormalLasers = false;
            }
            else if (npc.localAI[1] >= 0f && !HasTelegraphedNormalLasers && npc.HasValidTarget) //telegraph for imminent laser
            {
                HasTelegraphedNormalLasers = true;
                if (Main.netMode != NetmodeID.MultiplayerClient)
                    Projectile.NewProjectile(npc.Center, Vector2.Zero, ModContent.ProjectileType<GlowRing>(), 0, 0f, Main.myPlayer, npc.whoAmI, -22);
            }

            return true;
        }

        public override void OnHitPlayer(NPC npc, Player target, int damage, bool crit)
        {
            base.OnHitPlayer(npc, target, damage, crit);

            target.AddBuff(BuffID.Burning, 300);
        }

        public override void LoadSprites(NPC npc, bool recolor)
        {
            base.LoadSprites(npc, recolor);

            LoadNPCSprite(recolor, npc.type);
        }
    }

    public class Hungry : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchTypeRange(NPCID.TheHungry, NPCID.TheHungryII);

        public override void SetDefaults(NPC npc)
        {
            base.SetDefaults(npc);

            npc.buffImmune[BuffID.OnFire] = true;
        }

        public override void AI(NPC npc)
        {
            base.AI(npc);

            if (FargoSoulsWorld.SwarmActive)
                return;

            NPC wall = FargoSoulsUtil.NPCExists(EModeGlobalNPC.wallBoss, NPCID.WallofFlesh);
            if (npc.HasValidTarget && npc.Distance(Main.player[npc.target].Center) < 200 && wall != null && wall.GetEModeNPCMod<WallofFlesh>().UseCorruptAttack && wall.GetEModeNPCMod<WallofFlesh>().WorldEvilAttackCycleTimer < 240)
            {
                //snap away from player if too close during wof cursed flame wall
                npc.position += (Main.player[npc.target].position - Main.player[npc.target].oldPosition) / 3;

                Vector2 vel = Main.player[npc.target].Center - npc.Center;
                vel += 200f * Main.player[npc.target].DirectionTo(npc.Center);
                npc.velocity = vel / 15;
            }
        }

        public override void OnHitPlayer(NPC npc, Player target, int damage, bool crit)
        {
            base.OnHitPlayer(npc, target, damage, crit);

            target.AddBuff(BuffID.OnFire, 300);
        }

        public override void LoadSprites(NPC npc, bool recolor)
        {
            base.LoadSprites(npc, recolor);

            LoadNPCSprite(recolor, npc.type);
        }
    }

    public class Leech : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchTypeRange(NPCID.LeechBody, NPCID.LeechHead, NPCID.LeechTail);

        public override void SetDefaults(NPC npc)
        {
            base.SetDefaults(npc);

            npc.buffImmune[BuffID.OnFire] = true;
        }

        public override void OnHitPlayer(NPC npc, Player target, int damage, bool crit)
        {
            base.OnHitPlayer(npc, target, damage, crit);

            target.AddBuff(BuffID.OnFire, 300);
            target.AddBuff(BuffID.Bleeding, 300);
            target.AddBuff(BuffID.Rabies, 600);
        }

        public override void LoadSprites(NPC npc, bool recolor)
        {
            base.LoadSprites(npc, recolor);

            LoadNPCSprite(recolor, npc.type);
        }
    }
}
