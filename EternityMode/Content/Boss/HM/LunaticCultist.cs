using Fargowiltas.Items.Summons.Mutant;
using FargowiltasSouls.EternityMode.Net;
using FargowiltasSouls.EternityMode.Net.Strategies;
using FargowiltasSouls.EternityMode.NPCMatching;
using FargowiltasSouls.Buffs.Masomode;
using FargowiltasSouls.Items.Accessories.Masomode;
using FargowiltasSouls.Items.Misc;
using FargowiltasSouls.NPCs;
using FargowiltasSouls.Projectiles;
using FargowiltasSouls.Projectiles.Masomode;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.EternityMode.Content.Boss.HM
{
    public class LunaticCultist : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.CultistBoss);

        public float RitualRotation;

        public int MagicDamageCounter;
        public int MeleeDamageCounter;
        public int RangedDamageCounter;
        public int MinionDamageCounter;

        public bool EnteredPhase2;
        public bool DroppedSummon;

        public override Dictionary<Ref<object>, CompoundStrategy> GetNetInfo() =>
            new Dictionary<Ref<object>, CompoundStrategy> {
                { new Ref<object>(RitualRotation), FloatStrategies.CompoundStrategy },

                { new Ref<object>(MeleeDamageCounter), IntStrategies.CompoundStrategy },
                { new Ref<object>(RangedDamageCounter), IntStrategies.CompoundStrategy },
                { new Ref<object>(MagicDamageCounter), IntStrategies.CompoundStrategy },
                { new Ref<object>(MinionDamageCounter), IntStrategies.CompoundStrategy },

                { new Ref<object>(EnteredPhase2), BoolStrategies.CompoundStrategy },
            };

        public override void SetDefaults(NPC npc)
        {
            base.SetDefaults(npc);

            npc.lifeMax = (int)(npc.lifeMax * 1.5);
            npc.buffImmune[BuffID.Suffocation] = true;
        }

        public override void AI(NPC npc)
        {
            base.AI(npc);

            EModeGlobalNPC.cultBoss = npc.whoAmI;

            if (FargoSoulsWorld.SwarmActive)
                return;

            if (npc.ai[3] == -1f)
            {
                if (Fargowiltas.Instance.MasomodeEXLoaded && npc.ai[1] >= 120f && npc.ai[1] < 419f) //skip summoning ritual LMAO
                {
                    npc.ai[1] = 419f;
                    npc.netUpdate = true;
                }

                if (npc.ai[0] == 5)
                {
                    if (npc.ai[1] == 1f)
                    {
                        RitualRotation = Main.rand.Next(360);
                        npc.netUpdate = true;
                        NetSync(npc);
                    }

                    if (npc.ai[1] > 30f && npc.ai[1] < 330f)
                    {
                        RitualRotation += 2f - Math.Min(1f, 2f * npc.life / npc.lifeMax); //always at least 1, begins scaling below 50% life
                        npc.Center = Main.player[npc.target].Center + 180f * Vector2.UnitX.RotatedBy(MathHelper.ToRadians(RitualRotation));
                    }
                }
            }
            else
            {
                //if (npc.ai[3] == 0) npc.damage = 0;

                //about to begin moving for ritual: 0 39 0 12
                //begin transit for ritual: 1 34 0 12
                //pause just before ritual: 0 0 0 13
                //ritual: 5 0 0 -1

                if (!EnteredPhase2 && npc.life < npc.lifeMax / 2) //p2 transition, force a ritual immediately
                {
                    EnteredPhase2 = true;
                    npc.ai[0] = 5;
                    npc.ai[1] = 0;
                    npc.ai[2] = 0;
                    npc.ai[3] = -1;
                    Main.PlaySound(SoundID.Roar, npc.Center, 0);

                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        npc.netUpdate = true;
                        NetSync(npc);
                    }
                }

                int damage = Math.Max(npc.damage, 75); //necessary because calameme
                switch ((int)npc.ai[0])
                {
                    case -1:
                        if (npc.ai[1] == 419f) //always start fight with a ritual
                        {
                            npc.ai[0] = 0f;
                            npc.ai[1] = 0f;
                            npc.ai[3] = 11f;
                            npc.netUpdate = true;
                        }
                        break;

                    case 2: //ice mist, frost wave support
                        if (EnteredPhase2)
                        {
                            if (npc.ai[1] < 60 && npc.ai[1] % 4 == 3)
                            {
                                int spacing = 14 - (int)(npc.ai[1] - 3) / 4; //start far and get closer
                                for (int j = -1; j <= 1; j += 2) //from above and below
                                {
                                    for (int i = -1; i <= 1; i += 2) //waves beside you
                                    {
                                        if (i == 0)
                                            continue;
                                        Vector2 spawnPos = Main.player[npc.target].Center;
                                        spawnPos.X += Math.Sign(i) * 150 * 2 + i * 120 * spacing;
                                        spawnPos.Y -= (700 + Math.Abs(i) * 50) * j;
                                        float speed = 8 + spacing * 0.8f;
                                        Projectile.NewProjectile(spawnPos, Vector2.UnitY * speed * j, ProjectileID.FrostWave, damage / 3, 0f, Main.myPlayer);
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (npc.ai[1] == 45f && Main.netMode != NetmodeID.MultiplayerClient) //single wave
                            {
                                for (int i = 0; i < Main.maxNPCs; i++)
                                {
                                    if (Main.npc[i].active && Main.npc[i].type == NPCID.CultistBossClone)
                                    {
                                        Vector2 distance = Main.player[npc.target].Center - Main.npc[i].Center;
                                        distance.Normalize();
                                        distance *= Main.rand.NextFloat(6f, 9f);
                                        distance = distance.RotatedByRandom(Math.PI / 24);
                                        Projectile.NewProjectile(Main.npc[i].Center, distance,
                                            ProjectileID.FrostWave, damage / 3, 0f, Main.myPlayer);
                                    }
                                }
                            }
                        }
                        break;

                    case 3:
                        if (npc.ai[1] == 3f && Main.netMode != NetmodeID.MultiplayerClient) //fireballs
                        {
                            if (EnteredPhase2) //solar goop support
                            {
                                int max = NPC.CountNPCS(NPCID.CultistBossClone) * 2 + 6;

                                Vector2 baseOffset = npc.DirectionTo(Main.player[npc.target].Center);
                                const float spawnOffset = 1200f;
                                const float speed = 7f;
                                const float ai0 = spawnOffset / speed;
                                for (int i = 0; i < max; i++)
                                {
                                    Projectile.NewProjectile(Main.player[npc.target].Center + spawnOffset * baseOffset.RotatedBy(2 * Math.PI / max * i),
                                        -speed * baseOffset.RotatedBy(2 * Math.PI / max * i), ModContent.ProjectileType<CultistFireball>(),
                                        damage / 3, 0f, Main.myPlayer, ai0);
                                }

                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                    Projectile.NewProjectile(npc.Center, Vector2.Zero, ModContent.ProjectileType<GlowRing>(), 0, 0f, Main.myPlayer, npc.whoAmI, npc.type);
                            }
                            else //fireball ring
                            {
                                for (int i = 0; i < Main.maxNPCs; i++)
                                {
                                    if (Main.npc[i].active && Main.npc[i].type == NPCID.CultistBossClone)
                                    {
                                        int n = NPC.NewNPC((int)Main.npc[i].Center.X, (int)Main.npc[i].Center.Y, NPCID.SolarGoop);
                                        if (n < 200)
                                        {
                                            Main.npc[n].velocity.X = Main.rand.Next(-10, 11);
                                            Main.npc[n].velocity.Y = Main.rand.Next(-15, -4);
                                            if (Main.netMode == NetmodeID.Server)
                                                NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, n);
                                        }
                                    }
                                }
                            }
                        }
                        break;

                    case 4:
                        if (npc.ai[1] == 19f && npc.HasPlayerTarget && Main.netMode != NetmodeID.MultiplayerClient) //lightning orb
                        {
                            int cultistCount = 1;
                            for (int i = 0; i < Main.maxNPCs; i++)
                            {
                                if (Main.npc[i].active && Main.npc[i].type == NPCID.CultistBossClone)
                                {
                                    if (EnteredPhase2) //vortex lightning
                                    {
                                        Projectile.NewProjectile(Main.npc[i].Center, Main.rand.NextVector2Square(-15, 15), ModContent.ProjectileType<CultistVortex>(),
                                          damage / 15 * 6, 0, Main.myPlayer, 0f, cultistCount);
                                        cultistCount++;
                                    }
                                    else //aimed lightning
                                    {
                                        Vector2 dir = Main.player[npc.target].Center - Main.npc[i].Center;
                                        float ai1New = Main.rand.Next(100);
                                        Vector2 vel = Vector2.Normalize(dir.RotatedByRandom(Math.PI / 4)) * 6f;
                                        Projectile.NewProjectile(Main.npc[i].Center, vel, ModContent.ProjectileType<HostileLightning>(),
                                            damage / 15 * 6, 0, Main.myPlayer, dir.ToRotation(), ai1New);
                                    }
                                }
                            }
                        }
                        break;

                    case 7:
                        if (npc.ai[1] == 3f && Main.netMode != NetmodeID.MultiplayerClient) //ancient light, jellyfish support
                        {
                            for (int i = 0; i < Main.maxProjectiles; i++)
                            {
                                if (Main.projectile[i].active && Main.projectile[i].type == ModContent.ProjectileType<CultistRitual>())
                                {
                                    Projectile.NewProjectile(new Vector2(Main.projectile[i].Center.X, Main.player[npc.target].Center.Y - 700),
                                        Vector2.Zero, ModContent.ProjectileType<StardustRain>(), damage / 3, 0f, Main.myPlayer);
                                }
                            }
                        }
                        break;

                    case 8:
                        if (npc.ai[1] == 3f) //ancient doom, nebula sphere support
                        {
                            int t = npc.HasPlayerTarget ? npc.target : npc.FindClosestPlayer();
                            if (t != -1 && Main.player[t].active)
                            {
                                for (int i = 0; i < Main.maxNPCs; i++)
                                {
                                    if (Main.npc[i].active && Main.npc[i].type == NPCID.CultistBossClone)
                                        Projectile.NewProjectile(Main.npc[i].Center, Vector2.Zero, ProjectileID.NebulaSphere, damage / 15 * 6, 0f, Main.myPlayer);
                                }
                            }
                        }
                        break;

                    default:
                        break;
                }
            }

            npc.defense = npc.defDefense; //prevent vanilla p2 from lowering defense!
            Lighting.AddLight(npc.Center, 1f, 1f, 1f);
            
            EModeUtils.DropSummon(npc, ModContent.ItemType<CultistSummon>(), NPC.downedAncientCultist, ref DroppedSummon, NPC.downedGolemBoss);
        }

        public override bool CanHitPlayer(NPC npc, Player target, ref int cooldownSlot)
        {
            return false;
        }

        private void IncrementDamageCounters(bool melee, bool ranged, bool magic, bool minion, int damage)
        {
            if (melee)// || thrown)
                MeleeDamageCounter += damage;
            else if (ranged)
                RangedDamageCounter += damage;
            else if (magic)
                MagicDamageCounter += damage;
            else if (minion)
                MinionDamageCounter += damage;
        }

        public override void OnHitByItem(NPC npc, Player player, Item item, int damage, float knockback, bool crit)
        {
            base.OnHitByItem(npc, player, item, damage, knockback, crit);

            IncrementDamageCounters(item.melee, item.ranged, item.magic, item.summon, damage);
        }

        public override void OnHitByProjectile(NPC npc, Projectile projectile, int damage, float knockback, bool crit)
        {
            base.OnHitByProjectile(npc, projectile, damage, knockback, crit);

            IncrementDamageCounters(projectile.melee, projectile.ranged, projectile.magic, FargoSoulsUtil.IsMinionDamage(projectile), damage);
        }

        public override void NPCLoot(NPC npc)
        {
            base.NPCLoot(npc);

            npc.DropItemInstanced(npc.position, npc.Size, ModContent.ItemType<CelestialRune>());
            if (Main.player[Main.myPlayer].extraAccessorySlots == 1 || Main.netMode != NetmodeID.SinglePlayer)
                npc.DropItemInstanced(npc.position, npc.Size, ModContent.ItemType<CelestialSeal>());
        }

        public override void LoadSprites(NPC npc, bool recolor)
        {
            base.LoadSprites(npc, recolor);

            LoadNPCSprite(recolor, npc.type);
            LoadBossHeadSprite(recolor, 24);
            LoadBossHeadSprite(recolor, 31);
            LoadGoreRange(recolor, 902, 903);
        }
    }

    public class LunaticCultistClone : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.CultistBossClone);

        public int TotalCultistCount;
        public int MyRitualPosition;

        public override Dictionary<Ref<object>, CompoundStrategy> GetNetInfo() =>
            new Dictionary<Ref<object>, CompoundStrategy> {
                { new Ref<object>(TotalCultistCount), IntStrategies.CompoundStrategy },
                { new Ref<object>(MyRitualPosition), IntStrategies.CompoundStrategy },
            };

        public override void SetDefaults(NPC npc)
        {
            base.SetDefaults(npc);

            npc.buffImmune[ModContent.BuffType<ClippedWings>()] = true;
            npc.buffImmune[BuffID.Suffocation] = true;
        }

        public override void AI(NPC npc)
        {
            base.AI(npc);

            NPC cultist = FargoSoulsUtil.NPCExists(npc.ai[3], NPCID.CultistBoss);
            if (cultist != null)
            {
                //during ritual
                if (cultist.ai[3] == -1 && cultist.ai[0] == 5) //&& cultist.ai[2] > -1 && cultist.ai[2] < Main.maxProjectiles)
                {
                    if (npc.alpha > 0)
                    {
                        TotalCultistCount = 1; //accounts for cultist boss
                        for (int i = 0; i < Main.maxNPCs; i++)
                        {
                            if (Main.npc[i].active && Main.npc[i].type == npc.type && Main.npc[i].ai[3] == npc.ai[3])
                            {
                                if (i == npc.whoAmI)
                                    MyRitualPosition = TotalCultistCount; //stores which one this is
                                TotalCultistCount++; //stores total number of cultists
                            }
                        }
                    }

                    if (cultist.ai[1] > 30f && cultist.ai[1] < 330f)
                    {
                        Vector2 offset = cultist.Center - Main.player[cultist.target].Center;
                        npc.Center = Main.player[cultist.target].Center + offset.RotatedBy(2 * Math.PI / TotalCultistCount * MyRitualPosition);
                        Lighting.AddLight(npc.Center, 1f, 1f, 1f);
                    }

                    /*int ritual = (int)cultist.ai[2]; //rotate around ritual
                    if (Main.projectile[ritual].active && Main.projectile[ritual].type == ProjectileID.CultistRitual)
                    {
                        Vector2 offset = cultist.Center - Main.projectile[ritual].Center;
                        npc.Center = Main.projectile[ritual].Center + offset.RotatedBy(2 * Math.PI / Counter[0] * Counter[1]);
                    }*/
                }
                else
                {
                    if (cultist.ai[3] == 0) //be visible always
                        npc.alpha = 0;
                }

                Lighting.AddLight(npc.Center, 1f, 1f, 1f);
            }
        }

        public override void HitEffect(NPC npc, int hitDirection, double damage)
        {
            base.HitEffect(npc, hitDirection, damage);

            if (!FargoSoulsWorld.SwarmActive)
            {
                NPC cultist = FargoSoulsUtil.NPCExists(npc.ai[3], NPCID.CultistBoss);
                if (cultist != null && NPC.CountNPCS(npc.type) < TotalCultistCount) //yes, this allows spawning two clones
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        int n = NPC.NewNPC((int)npc.Center.X, (int)npc.Center.Y, NPCID.CultistBossClone, 0, npc.ai[0], npc.ai[1], npc.ai[2], npc.ai[3], npc.target);
                        if (n != Main.maxNPCs && Main.netMode == NetmodeID.Server)
                            NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, n);
                    }
                }
            }
        }

        public override void LoadSprites(NPC npc, bool recolor)
        {
            base.LoadSprites(npc, recolor);

            LoadNPCSprite(recolor, npc.type);
        }
    }
}
