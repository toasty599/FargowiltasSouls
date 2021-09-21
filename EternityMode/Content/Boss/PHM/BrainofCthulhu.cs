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

            npc.lifeMax = (int)(npc.lifeMax * 1.25);
            npc.scale += 0.25f;
            npc.buffImmune[BuffID.Ichor] = true;
        }

        public override void AI(NPC npc)
        {
            base.AI(npc);

            EModeGlobalNPC.brainBoss = npc.whoAmI;

            if (FargoSoulsWorld.SwarmActive)
                return;

            if (!npc.HasValidTarget || npc.Distance(Main.player[npc.target].Center) > 3000)
            {
                npc.velocity.Y += 0.75f;
                if (npc.timeLeft > 120)
                    npc.timeLeft = 120;
            }

            if (npc.alpha == 0)
            {
                npc.damage = npc.defDamage;
            }
            else
            {
                npc.damage = 0;
                if (npc.ai[0] != -2 && npc.HasPlayerTarget && npc.Distance(Main.player[npc.target].Center) < 300) //stay at a minimum distance
                {
                    npc.Center = Main.player[npc.target].Center + Main.player[npc.target].DirectionTo(npc.Center) * 300;
                }
            }

            if (!npc.dontTakeDamage) //vulnerable
            {
                if (npc.buffType[0] != 0) //constant debuff cleanse
                {
                    npc.buffImmune[npc.buffType[0]] = true;
                    npc.DelBuff(0);
                }

                if (!EnteredPhase2) //spawn illusions
                {
                    EnteredPhase2 = true;

                    if (Main.netMode != NetmodeID.MultiplayerClient)
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

                        Vector2 spawnPos = Main.player[npc.target].Center;
                        spawnPos.X += offset.X;
                        spawnPos.Y += offset.Y;
                        Projectile.NewProjectile(spawnPos, new Vector2(0, -4), ModContent.ProjectileType<BrainofConfusion>(), 0, 0, Main.myPlayer);
                        for (int i = -max; i <= max; i++)
                            Projectile.NewProjectile(spawnPos, 0.2f * Main.player[npc.target].DirectionFrom(spawnPos).RotatedBy(MathHelper.ToRadians(degree) * i), ModContent.ProjectileType<DestroyerLaser>(), npc.damage / 4, 0f, Main.myPlayer);

                        spawnPos = Main.player[npc.target].Center;
                        spawnPos.X += offset.X;
                        spawnPos.Y -= offset.Y;
                        Projectile.NewProjectile(spawnPos, new Vector2(0, -4), ModContent.ProjectileType<BrainofConfusion>(), 0, 0, Main.myPlayer);
                        for (int i = -max; i <= max; i++)
                            Projectile.NewProjectile(spawnPos, 0.2f * Main.player[npc.target].DirectionFrom(spawnPos).RotatedBy(MathHelper.ToRadians(degree) * i), ModContent.ProjectileType<DestroyerLaser>(), npc.damage / 4, 0f, Main.myPlayer);

                        spawnPos = Main.player[npc.target].Center;
                        spawnPos.X -= offset.X;
                        spawnPos.Y += offset.Y;
                        Projectile.NewProjectile(spawnPos, new Vector2(0, -4), ModContent.ProjectileType<BrainofConfusion>(), 0, 0, Main.myPlayer);
                        for (int i = -max; i <= max; i++)
                            Projectile.NewProjectile(spawnPos, 0.2f * Main.player[npc.target].DirectionFrom(spawnPos).RotatedBy(MathHelper.ToRadians(degree) * i), ModContent.ProjectileType<DestroyerLaser>(), npc.damage / 4, 0f, Main.myPlayer);

                        spawnPos = Main.player[npc.target].Center;
                        spawnPos.X -= offset.X;
                        spawnPos.Y -= offset.Y;
                        Projectile.NewProjectile(spawnPos, new Vector2(0, -4), ModContent.ProjectileType<BrainofConfusion>(), 0, 0, Main.myPlayer);
                        for (int i = -max; i <= max; i++)
                            Projectile.NewProjectile(spawnPos, 0.2f * Main.player[npc.target].DirectionFrom(spawnPos).RotatedBy(MathHelper.ToRadians(degree) * i), ModContent.ProjectileType<DestroyerLaser>(), npc.damage / 4, 0f, Main.myPlayer);
                    }
                };

                if (--ConfusionTimer < 0)
                {
                    ConfusionTimer = 300;

                    npc.netUpdate = true;
                    NetSync(npc);

                    if (Main.LocalPlayer.HasBuff(BuffID.Confused))
                    {
                        Main.PlaySound(SoundID.ForceRoar, (int)npc.Center.X, (int)npc.Center.Y, -1, 1f, 0f);
                        TelegraphConfusion(npc.Center);
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
                else if (ConfusionTimer == 240) //inflict confusion after telegraph
                {
                    npc.netUpdate = true;
                    NetSync(npc);

                    if (npc.Distance(Main.LocalPlayer.Center) < 3000 && !Main.LocalPlayer.HasBuff(BuffID.Confused))
                    {
                        Main.LocalPlayer.AddBuff(BuffID.Confused, Main.expertMode && Main.expertDebuffTime > 1 ? 150 + 5 : 300 + 10);
                    }

                    LaserSpread();
                }

                if (--IllusionTimer < 0)
                {
                    IllusionTimer = Main.rand.Next(5, 15);
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Vector2 spawn = Main.player[npc.target].Center + Main.rand.NextVector2CircularEdge(1200f, 1200f);
                        Vector2 speed = Main.player[npc.target].Center + Main.rand.NextVector2Circular(-600f, 600f) - spawn;
                        speed = Vector2.Normalize(speed) * Main.rand.NextFloat(12f, 48f);
                        Projectile.NewProjectile(spawn, speed, ModContent.ProjectileType<BrainIllusionProj>(), 0, 0f, Main.myPlayer, npc.whoAmI);
                    }
                }
            }

            EModeUtils.DropSummon(npc, ModContent.ItemType<GoreySpine>(), NPC.downedBoss2, ref DroppedSummon);
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

            if (IchorAttackTimer % 60 == 0)
            {
                IchorAttackTimer = Math.Min(IchorAttackTimer, NPC.CountNPCS(npc.type));
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
