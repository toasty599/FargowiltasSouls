using Fargowiltas.Items.Summons;
using FargowiltasSouls.EternityMode.Net;
using FargowiltasSouls.EternityMode.Net.Strategies;
using FargowiltasSouls.EternityMode.NPCMatching;
using FargowiltasSouls.Items.Accessories.Masomode;
using FargowiltasSouls.NPCs;
using FargowiltasSouls.NPCs.EternityMode;
using FargowiltasSouls.Projectiles;
using FargowiltasSouls.Projectiles.Masomode;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.EternityMode.Content.Boss.PHM
{
    public class BrainofCthulhu : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.BrainofCthulhu);

        public int ConfusionTimer;
        public int IllusionTimer;

        public bool EnteredPhase2;
        
        public bool DroppedSummon;

        public override Dictionary<Ref<object>, CompoundStrategy> GetNetInfo() =>
            new Dictionary<Ref<object>, CompoundStrategy> {
                { new Ref<object>(ConfusionTimer), IntStrategies.CompoundStrategy },
                { new Ref<object>(IllusionTimer), IntStrategies.CompoundStrategy },

                { new Ref<object>(EnteredPhase2), BoolStrategies.CompoundStrategy },
            };

        public override void SetDefaults(NPC npc)
        {
            base.SetDefaults(npc);

            //npc.lifeMax = (int)(npc.lifeMax * 1.25);
            npc.scale += 0.25f;
            npc.buffImmune[BuffID.Ichor] = true;
        }

        public override bool CanHitPlayer(NPC npc, Player target, ref int cooldownSlot)
        {
            return npc.alpha == 0;
        }

        public override void AI(NPC npc)
        {
            EModeGlobalNPC.brainBoss = npc.whoAmI;

            if (FargoSoulsWorld.SwarmActive)
                return;

            if (!npc.HasValidTarget || npc.Distance(Main.player[npc.target].Center) > 3000)
            {
                npc.velocity.Y += 0.75f;
                if (npc.timeLeft > 120)
                    npc.timeLeft = 120;
            }

            if (npc.alpha > 0 && npc.ai[0] == -3 && npc.HasValidTarget) //stay at a minimum distance
            {
                const float safeRange = 360;
                /*Vector2 stayAwayFromHere = Main.player[npc.target].Center + Main.player[npc.target].velocity * 30f;
                if (npc.Distance(stayAwayFromHere) < safeRange)
                    npc.Center = stayAwayFromHere + npc.DirectionFrom(stayAwayFromHere) * safeRange;*/
                Vector2 stayAwayFromHere = Main.player[npc.target].Center;
                if (npc.Distance(stayAwayFromHere) < safeRange)
                    npc.Center = stayAwayFromHere + npc.DirectionFrom(stayAwayFromHere) * safeRange;
            }

            if (EnteredPhase2)
            {
                if (npc.buffType[0] != 0) //constant debuff cleanse
                {
                    npc.buffImmune[npc.buffType[0]] = true;
                    npc.DelBuff(0);
                }

                void TelegraphConfusion(Vector2 spawn)
                {
                    Projectile.NewProjectile(spawn, Vector2.Zero, ModContent.ProjectileType<GlowRingHollow>(), 0, 0f, Main.myPlayer, 8, 180);
                    Projectile.NewProjectile(spawn, Vector2.Zero, ModContent.ProjectileType<GlowRingHollow>(), 0, 0f, Main.myPlayer, 8, 200);
                    Projectile.NewProjectile(spawn, Vector2.Zero, ModContent.ProjectileType<GlowRingHollow>(), 0, 0f, Main.myPlayer, 8, 220);
                };

                void LaserSpread()
                {
                    if (npc.HasValidTarget && Main.netMode != NetmodeID.MultiplayerClient) //laser spreads from each illusion
                    {
                        Vector2 offset = npc.Center - Main.player[npc.target].Center;

                        const int max = 3;
                        const int degree = 3;
                        int laserDamage = npc.damage / 3;

                        Vector2 spawnPos = Main.player[npc.target].Center;
                        spawnPos.X += offset.X;
                        spawnPos.Y += offset.Y;
                        Projectile.NewProjectile(spawnPos, new Vector2(0, -4), ModContent.ProjectileType<BrainofConfusion>(), 0, 0, Main.myPlayer);
                        for (int i = -max; i <= max; i++)
                            Projectile.NewProjectile(spawnPos, 0.2f * Main.player[npc.target].DirectionFrom(spawnPos).RotatedBy(MathHelper.ToRadians(degree) * i), ModContent.ProjectileType<DestroyerLaser>(), laserDamage, 0f, Main.myPlayer);

                        spawnPos = Main.player[npc.target].Center;
                        spawnPos.X += offset.X;
                        spawnPos.Y -= offset.Y;
                        Projectile.NewProjectile(spawnPos, new Vector2(0, -4), ModContent.ProjectileType<BrainofConfusion>(), 0, 0, Main.myPlayer);
                        for (int i = -max; i <= max; i++)
                            Projectile.NewProjectile(spawnPos, 0.2f * Main.player[npc.target].DirectionFrom(spawnPos).RotatedBy(MathHelper.ToRadians(degree) * i), ModContent.ProjectileType<DestroyerLaser>(), laserDamage, 0f, Main.myPlayer);

                        spawnPos = Main.player[npc.target].Center;
                        spawnPos.X -= offset.X;
                        spawnPos.Y += offset.Y;
                        Projectile.NewProjectile(spawnPos, new Vector2(0, -4), ModContent.ProjectileType<BrainofConfusion>(), 0, 0, Main.myPlayer);
                        for (int i = -max; i <= max; i++)
                            Projectile.NewProjectile(spawnPos, 0.2f * Main.player[npc.target].DirectionFrom(spawnPos).RotatedBy(MathHelper.ToRadians(degree) * i), ModContent.ProjectileType<DestroyerLaser>(), laserDamage, 0f, Main.myPlayer);

                        spawnPos = Main.player[npc.target].Center;
                        spawnPos.X -= offset.X;
                        spawnPos.Y -= offset.Y;
                        Projectile.NewProjectile(spawnPos, new Vector2(0, -4), ModContent.ProjectileType<BrainofConfusion>(), 0, 0, Main.myPlayer);
                        for (int i = -max; i <= max; i++)
                            Projectile.NewProjectile(spawnPos, 0.2f * Main.player[npc.target].DirectionFrom(spawnPos).RotatedBy(MathHelper.ToRadians(degree) * i), ModContent.ProjectileType<DestroyerLaser>(), laserDamage, 0f, Main.myPlayer);
                    }
                };

                if (--ConfusionTimer < 0)
                {
                    ConfusionTimer = 300;

                    npc.netUpdate = true;
                    NetSync(npc);

                    if (Main.player[npc.target].HasBuff(BuffID.Confused))
                    {
                        Main.PlaySound(SoundID.ForceRoar, (int)npc.Center.X, (int)npc.Center.Y, -1, 1f, 0f);
                        TelegraphConfusion(npc.Center);

                        IllusionTimer = 120 + 90;

                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            int type = ModContent.ProjectileType<BrainIllusionProj>(); //make illusions attack
                            foreach (Projectile p in Main.projectile.Where(p => p.active && p.type == type && p.ai[0] == npc.whoAmI && p.ai[1] == 0f))
                            {
                                if (p.Distance(Main.player[npc.target].Center) < 1000)
                                {
                                    //p.ai[1] = 1f;
                                    //p.netUpdate = true;

                                    int n = NPC.NewNPC((int)p.Center.X, (int)p.Center.Y, ModContent.NPCType<BrainIllusionAttack>(), npc.whoAmI, npc.whoAmI, p.alpha);
                                    if (n != Main.maxNPCs)
                                        NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, n);
                                }
                                p.Kill();
                            }
                        }
                    }
                    else
                    {
                        Main.PlaySound(SoundID.Roar, (int)npc.Center.X, (int)npc.Center.Y, 0);

                        Vector2 offset = npc.Center - Main.player[npc.target].Center;

                        Vector2 spawnPos = Main.player[npc.target].Center;
                        spawnPos.X += offset.X;
                        spawnPos.Y += offset.Y;
                        TelegraphConfusion(spawnPos);

                        spawnPos = Main.player[npc.target].Center;
                        spawnPos.X += offset.X;
                        spawnPos.Y -= offset.Y;
                        TelegraphConfusion(spawnPos);

                        spawnPos = Main.player[npc.target].Center;
                        spawnPos.X -= offset.X;
                        spawnPos.Y += offset.Y;
                        TelegraphConfusion(spawnPos);

                        spawnPos = Main.player[npc.target].Center;
                        spawnPos.X -= offset.X;
                        spawnPos.Y -= offset.Y;
                        TelegraphConfusion(spawnPos);
                    }
                }
                else if (ConfusionTimer == 240)
                {
                    npc.netUpdate = true;
                    NetSync(npc);

                    if (npc.Distance(Main.LocalPlayer.Center) < 3000 && !Main.LocalPlayer.HasBuff(BuffID.Confused)) //inflict confusion
                    {
                        Main.LocalPlayer.AddBuff(BuffID.Confused, Main.expertMode && Main.expertDebuffTime > 1 ? 150 + 5 : 300 + 10);

                        LaserSpread();
                    }
                }

                if (--IllusionTimer < 0) //spawn illusions
                {
                    IllusionTimer = Main.rand.Next(5, 11);
                    if (npc.life > npc.lifeMax / 2)
                        IllusionTimer += 5;
                    if (npc.life < npc.lifeMax / 10)
                        IllusionTimer -= 2;

                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Vector2 spawn = Main.player[npc.target].Center + Main.rand.NextVector2CircularEdge(1200f, 1200f);
                        Vector2 speed = Main.player[npc.target].Center + Main.player[npc.target].velocity * 45f + Main.rand.NextVector2Circular(-600f, 600f) - spawn;
                        speed = Vector2.Normalize(speed) * Main.rand.NextFloat(12f, 48f);
                        Projectile.NewProjectile(spawn, speed, ModContent.ProjectileType<BrainIllusionProj>(), npc.damage / 3, 0f, Main.myPlayer, npc.whoAmI);
                    }
                }

                if (IllusionTimer > 60)
                {
                    if (npc.ai[0] == -1f && npc.localAI[1] < 80) //force a tp
                    {
                        npc.localAI[1] = 80f;
                    }
                    if (npc.ai[0] == -3f && npc.ai[3] > 200) //stay invis
                    {
                        npc.dontTakeDamage = true;
                        npc.ai[0] = -3f;
                        npc.ai[3] = 255;
                        npc.alpha = 255;
                    }
                }
            }
            else if (!npc.dontTakeDamage)
            {
                EnteredPhase2 = true;

                if (Main.netMode != NetmodeID.MultiplayerClient) //spawn illusions
                {
                    bool recolor = SoulConfig.Instance.BossRecolors && FargoSoulsWorld.MasochistMode;
                    int type = recolor ? ModContent.NPCType<BrainIllusion2>() : ModContent.NPCType<BrainIllusion>();
                    int n = NPC.NewNPC((int)npc.Center.X, (int)npc.Center.Y, type, npc.whoAmI, npc.whoAmI, -1, 1);
                    if (n != Main.maxNPCs && Main.netMode == NetmodeID.Server)
                        NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, n);
                    n = NPC.NewNPC((int)npc.Center.X, (int)npc.Center.Y, type, npc.whoAmI, npc.whoAmI, 1, -1);
                    if (n != Main.maxNPCs && Main.netMode == NetmodeID.Server)
                        NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, n);
                    n = NPC.NewNPC((int)npc.Center.X, (int)npc.Center.Y, type, npc.whoAmI, npc.whoAmI, 1, 1);
                    if (n != Main.maxNPCs && Main.netMode == NetmodeID.Server)
                        NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, n);
                    n = NPC.NewNPC((int)npc.Center.X, (int)npc.Center.Y, ModContent.NPCType<BrainClone>(), npc.whoAmI);
                    if (n != Main.maxNPCs && Main.netMode == NetmodeID.Server)
                        NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, n);

                    for (int i = 0; i < Main.maxProjectiles; i++) //clear old golden showers
                    {
                        if (Main.projectile[i].active && Main.projectile[i].type == ModContent.ProjectileType<GoldenShowerHoming>())
                            Main.projectile[i].Kill();
                    }
                }
            }

            EModeUtils.DropSummon(npc, ModContent.ItemType<GoreySpine>(), NPC.downedBoss2, ref DroppedSummon);

            npc.defense = 0;
        }

        public override bool StrikeNPC(NPC npc, ref double damage, int defense, ref float knockback, int hitDirection, ref bool crit)
        {
            if (npc.life > 0)
                damage *= Math.Sqrt((double)npc.life / npc.lifeMax);

            return base.StrikeNPC(npc, ref damage, defense, ref knockback, hitDirection, ref crit);
        }

        public override void NPCLoot(NPC npc)
        {
            base.NPCLoot(npc);

            npc.DropItemInstanced(npc.position, npc.Size, ItemID.CrimsonFishingCrate, 5);
            npc.DropItemInstanced(npc.position, npc.Size, ModContent.ItemType<GuttedHeart>());

            //to make up for no loot creepers
            Item.NewItem(npc.Hitbox, ItemID.TissueSample, 60);
            Item.NewItem(npc.Hitbox, ItemID.CrimtaneOre, 200);
        }

        public override void OnHitPlayer(NPC npc, Player target, int damage, bool crit)
        {
            base.OnHitPlayer(npc, target, damage, crit);

            target.AddBuff(BuffID.Poisoned, 120);
            target.AddBuff(BuffID.Darkness, 120);
            target.AddBuff(BuffID.Bleeding, 120);
            target.AddBuff(BuffID.Slow, 120);
            target.AddBuff(BuffID.Weak, 120);
            target.AddBuff(BuffID.BrokenArmor, 120);
        }

        public override void LoadSprites(NPC npc, bool recolor)
        {
            base.LoadSprites(npc, recolor);

            LoadNPCSprite(recolor, npc.type);
            LoadBossHeadSprite(recolor, 23);
            LoadGoreRange(recolor, 392, 402);
        }
    }

    public class Creeper : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.Creeper);

        public int IchorAttackTimer;

        public override Dictionary<Ref<object>, CompoundStrategy> GetNetInfo() =>
            new Dictionary<Ref<object>, CompoundStrategy> {
                { new Ref<object>(IchorAttackTimer), IntStrategies.CompoundStrategy },
            };

        public override void SetDefaults(NPC npc)
        {
            base.SetDefaults(npc);

            npc.lifeMax = (int)(npc.lifeMax * 1.25);
            npc.buffImmune[BuffID.Ichor] = true;

            IchorAttackTimer = Main.rand.Next(60 * NPC.CountNPCS(NPCID.Creeper)) + Main.rand.Next(-60, 61);
        }

        public override void AI(NPC npc)
        {
            base.AI(npc);

            if (--IchorAttackTimer < 0)
            {
                IchorAttackTimer = 60 * NPC.CountNPCS(NPCID.Creeper) - 30;
                if (IchorAttackTimer >= 60)
                    IchorAttackTimer += Main.rand.Next(-30, 31);

                if (npc.HasPlayerTarget && Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Projectile.NewProjectile(npc.Center, 10f * npc.DirectionFrom(Main.player[npc.target].Center).RotatedByRandom(Math.PI),
                        ModContent.ProjectileType<GoldenShowerHoming>(), npc.damage / 4, 0f, Main.myPlayer, npc.target, -60f);
                }

                npc.netUpdate = true;
                NetSync(npc);
            }

            if (IchorAttackTimer % 60 == 0) //update timer periodically for if player suddenly kills a lot of creepers at once
            {
                IchorAttackTimer = Math.Min(IchorAttackTimer, 60 * NPC.CountNPCS(npc.type));
            }
        }

        public override void OnHitPlayer(NPC npc, Player target, int damage, bool crit)
        {
            base.OnHitPlayer(npc, target, damage, crit);

            target.AddBuff(BuffID.Poisoned, 120);
            target.AddBuff(BuffID.Darkness, 120);
            target.AddBuff(BuffID.Bleeding, 120);
            target.AddBuff(BuffID.Slow, 120);
            target.AddBuff(BuffID.Weak, 120);
            target.AddBuff(BuffID.BrokenArmor, 120);
        }

        public override bool CheckDead(NPC npc)
        {
            Main.PlaySound(npc.DeathSound, npc.Center);
            npc.active = false;
            return false;
        }

        public override void LoadSprites(NPC npc, bool recolor)
        {
            base.LoadSprites(npc, recolor);

            LoadNPCSprite(recolor, npc.type);
        }
    }
}
