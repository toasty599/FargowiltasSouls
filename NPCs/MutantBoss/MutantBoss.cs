using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

using FargowiltasSouls.Buffs.Boss;
using FargowiltasSouls.Buffs.Masomode;
using FargowiltasSouls.Buffs.Souls;
using FargowiltasSouls.Projectiles.MutantBoss;
using FargowiltasSouls.Items.Summons;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using FargowiltasSouls.Items.Misc;
using FargowiltasSouls.Items.Placeables;
using FargowiltasSouls.ItemDropRules.Conditions;
using FargowiltasSouls.Items.Pets;

namespace FargowiltasSouls.NPCs.MutantBoss
{
    [AutoloadBossHead]
    public class MutantBoss : ModNPC
    {
        public bool playerInvulTriggered;
        public int ritualProj, spriteProj, ringProj;
        private bool droppedSummon = false;
        
        public Queue<float> attackHistory = new Queue<float>();
        public int attackCount;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Mutant");
            DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "突变体");
            Main.npcFrameCount[NPC.type] = 4;
            NPCID.Sets.BossBestiaryPriority.Add(NPC.type);
            NPCID.Sets.DebuffImmunitySets.Add(NPC.type, new Terraria.DataStructures.NPCDebuffImmunityData
            {
                SpecificallyImmuneTo = new int[]
                {
                    BuffID.Confused,
                    BuffID.Chilled,
                    BuffID.OnFire,
                    BuffID.Suffocation,
                    ModContent.BuffType<Lethargic>(),
                    ModContent.BuffType<ClippedWings>(),
                    ModContent.BuffType<MutantNibble>(),
                    ModContent.BuffType<OceanicMaul>(),
                    ModContent.BuffType<LightningRod>(),
                    ModContent.BuffType<Sadism>(),
                    ModContent.BuffType<GodEater>(),
                    ModContent.BuffType<TimeFrozen>()
                }
            });
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Sky,
                new FlavorTextBestiaryInfoElement($"Mods.FargowiltasSouls.Bestiary.{Name}")
            });
        }

        public override void SetDefaults()
        {
            NPC.width = 120;//34;
            NPC.height = 120;//50;
            NPC.damage = 444;
            NPC.defense = 255;
            NPC.value = Item.buyPrice(2);
            NPC.lifeMax = Main.expertMode ? 7000000 : 3500000;
            if (FargoSoulsWorld.MasochistModeReal)
                NPC.lifeMax = 7700000;
            NPC.HitSound = SoundID.NPCHit57;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.npcSlots = 50f;
            NPC.knockBackResist = 0f;
            NPC.boss = true;
            NPC.lavaImmune = true;
            NPC.aiStyle = -1;
            NPC.netAlways = true;
            NPC.timeLeft = NPC.activeTime * 30;
            if (FargoSoulsWorld.AngryMutant)// || Fargowiltas.Instance.CalamityLoaded)
            {
                NPC.lifeMax = 177000000;
                NPC.damage = (int)(NPC.damage * 4);
                NPC.defense *= 2;
                //if (FargowiltasSouls.Instance.CalamityLoaded)
                //{
                //    NPC.buffImmune[ModLoader.GetMod("CalamityMod").BuffType("ExoFreeze")] = true;
                //    NPC.buffImmune[ModLoader.GetMod("CalamityMod").BuffType("GlacialState")] = true;
                //    NPC.buffImmune[ModLoader.GetMod("CalamityMod").BuffType("TemporalSadness")] = true;
                //    NPC.buffImmune[ModLoader.GetMod("CalamityMod").BuffType("SilvaStun")] = true;
                //    NPC.buffImmune[ModLoader.GetMod("CalamityMod").BuffType("TimeSlow")] = true;
                //    NPC.buffImmune[ModLoader.GetMod("CalamityMod").BuffType("PearlAura")] = true;
                //}
            }

            //if (Fargowiltas.Instance.MasomodeEXLoaded)
            //{
            //    music = Fargowiltas.Instance.MasomodeEXCompatibility.ModInstance.GetSoundSlot(SoundType.Music, "Assets/Music/rePrologue");
            //}
            //else
            //{
            //    Mod musicMod = ModLoader.GetMod("FargowiltasMusic");
            //    if (musicMod == null)
            //        music = MusicID.LunarBoss;
            //    else
            //        music = musicMod.GetSoundSlot(SoundType.Music, "Sounds/Music/SteelRed");
            //}
            //musicPriority = (MusicPriority)12;

            Music = ModLoader.TryGetMod("FargowiltasMusic", out Mod musicMod)
                ? MusicLoader.GetMusicSlot(musicMod, "Assets/Music/SteelRed") : MusicID.OtherworldlyTowers;
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            NPC.damage = (int)(NPC.damage * 0.5f);
            NPC.lifeMax = (int)(NPC.lifeMax * 0.5f * bossLifeScale);
        }

        public override bool CanHitPlayer(Player target, ref int CooldownSlot)
        {
            CooldownSlot = 1;
            return NPC.Distance(FargoSoulsUtil.ClosestPointInHitbox(target, NPC.Center)) < Player.defaultHeight && NPC.ai[0] > -1;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(NPC.localAI[0]);
            writer.Write(NPC.localAI[1]);
            writer.Write(NPC.localAI[2]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            NPC.localAI[0] = reader.ReadSingle();
            NPC.localAI[1] = reader.ReadSingle();
            NPC.localAI[2] = reader.ReadSingle();
        }

        public override void AI()
        {
            EModeGlobalNPC.mutantBoss = NPC.whoAmI;

            if (FargoSoulsWorld.MasochistModeReal && Main.LocalPlayer.active && !Main.LocalPlayer.dead && !Main.LocalPlayer.ghost)
                Main.LocalPlayer.AddBuff(ModContent.BuffType<MutantPresence>(), 2);

            NPC.dontTakeDamage = false;
            if (NPC.ai[0] < 0)
                NPC.dontTakeDamage = true;

            if (NPC.localAI[3] == 0)
            {
                NPC.TargetClosest();
                if (NPC.timeLeft < 30)
                    NPC.timeLeft = 30;
                if (NPC.Distance(Main.player[NPC.target].Center) < 1500)
                {
                    NPC.localAI[3] = 1;
                    Terraria.Audio.SoundEngine.PlaySound(SoundID.Roar, (int)NPC.Center.X, (int)NPC.Center.Y, 0);
                    //EdgyBossText("I hope you're ready to embrace suffering.");
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        //if (FargowiltasSouls.Instance.MasomodeEXLoaded) Projectile.NewProjectile(NPC.GetSpawnSource_ForProjectile(), NPC.Center, Vector2.Zero, ModLoader.GetMod("MasomodeEX").ProjectileType("MutantText"), 0, 0f, Main.myPlayer, NPC.whoAmI);

                        if (FargoSoulsWorld.downedAbom && FargoSoulsWorld.AngryMutant)//(FargowiltasSouls.Instance.MasomodeEXLoaded || Fargowiltas.Instance.CalamityLoaded))
                            Projectile.NewProjectile(NPC.GetSpawnSource_ForProjectile(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<BossRush>(), 0, 0f, Main.myPlayer, NPC.whoAmI);
                    }
                }
            }
            else if (NPC.localAI[3] == 1)
            {
                EModeGlobalNPC.Aura(NPC, 2000f, ModContent.BuffType<GodEater>(), true, 86);
            }
            else
            {
                if (Main.LocalPlayer.active && NPC.Distance(Main.LocalPlayer.Center) < 3000f)
                {
                    if (Main.expertMode)
                    {
                        Main.LocalPlayer.AddBuff(ModContent.BuffType<MutantPresence>(), 2);
                    }
                    //if (FargowiltasSouls.Instance.CalamityLoaded)
                    //{
                    //    Main.LocalPlayer.buffImmune[ModLoader.GetMod("CalamityMod").BuffType("RageMode")] = true;
                    //    Main.LocalPlayer.buffImmune[ModLoader.GetMod("CalamityMod").BuffType("AdrenalineMode")] = true;
                    //}
                }
            }

            if (Main.netMode != NetmodeID.MultiplayerClient) //checks for needed projs
            {
                if (FargoSoulsWorld.EternityMode && NPC.ai[0] != -7 && (NPC.ai[0] < 0 || NPC.ai[0] > 10) && FargoSoulsUtil.ProjectileExists(ritualProj, ModContent.ProjectileType<MutantRitual>()) == null)
                    ritualProj = Projectile.NewProjectile(NPC.GetSpawnSource_ForProjectile(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<MutantRitual>(), NPC.damage / 4, 0f, Main.myPlayer, 0f, NPC.whoAmI);

                if (FargoSoulsUtil.ProjectileExists(ringProj, ModContent.ProjectileType<MutantRitual5>()) == null)
                    ringProj = Projectile.NewProjectile(NPC.GetSpawnSource_ForProjectile(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<MutantRitual5>(), 0, 0f, Main.myPlayer, 0f, NPC.whoAmI);

                if (FargoSoulsUtil.ProjectileExists(spriteProj, ModContent.ProjectileType<Projectiles.MutantBoss.MutantBoss>()) == null)
                {
                    /*if (Main.netMode == NetmodeID.Server)
                        ChatHelper.BroadcastChatMessage(NetworkText.FromLiteral("wheres my sprite"), Color.LimeGreen);
                    else
                        Main.NewText("wheres my sprite");*/
                    if (Main.netMode == NetmodeID.SinglePlayer)
                    {
                        int number = 0;
                        for (int index = 999; index >= 0; --index)
                        {
                            if (!Main.projectile[index].active)
                            {
                                number = index;
                                break;
                            }
                        }
                        if (number >= 0)
                        {
                            Projectile projectile = Main.projectile[number];
                            projectile.SetDefaults(ModContent.ProjectileType<Projectiles.MutantBoss.MutantBoss>());
                            projectile.Center = NPC.Center;
                            projectile.owner = Main.myPlayer;
                            projectile.velocity.X = 0;
                            projectile.velocity.Y = 0;
                            projectile.damage = 0;
                            projectile.knockBack = 0f;
                            projectile.identity = number;
                            projectile.gfxOffY = 0f;
                            projectile.stepSpeed = 1f;
                            projectile.ai[1] = NPC.whoAmI;

                            spriteProj = number;
                        }
                    }
                    else //server
                    {
                        spriteProj = Projectile.NewProjectile(NPC.GetSpawnSource_ForProjectile(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<Projectiles.MutantBoss.MutantBoss>(), 0, 0f, Main.myPlayer, 0f, NPC.whoAmI);
                        /*if (Main.netMode == NetmodeID.Server)
                            ChatHelper.BroadcastChatMessage(NetworkText.FromLiteral($"got sprite {spriteProj}"), Color.LimeGreen);
                        else
                            Main.NewText($"got sprite {spriteProj}");*/
                    }
                }
            }

            void ChooseNextAttack(params int[] args)
            {
                float buffer = NPC.ai[0] + 1;
                NPC.ai[0] = 46;
                NPC.ai[1] = 0;
                NPC.ai[2] = buffer;
                NPC.ai[3] = 0;
                NPC.localAI[0] = 0;
                NPC.localAI[1] = 0;
                NPC.localAI[2] = 0;
                //NPC.TargetClosest();
                NPC.netUpdate = true;

                /*string text = "-------------------------------------------------";
                Main.NewText(text);

                text = "";
                foreach (float f in attackHistory)
                    text += f.ToString() + " ";
                Main.NewText($"history: {text}");*/

                if (FargoSoulsWorld.EternityMode)
                {
                    //become more likely to use randoms as life decreases
                    bool useRandomizer = NPC.localAI[3] >= 3 && (FargoSoulsWorld.MasochistModeReal || Main.rand.NextFloat(0.8f) + 0.2f > (float)Math.Pow((float)NPC.life / NPC.lifeMax, 2));

                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Queue<float> recentAttacks = new Queue<float>(attackHistory); //copy of attack history that i can remove elements from freely

                        //if randomizer, start with a random attack, else use the previous state + 1 as starting attempt BUT DO SOMETHING ELSE IF IT'S ALREADY USED
                        if (useRandomizer)
                            NPC.ai[2] = Main.rand.Next(args);

                        //Main.NewText(useRandomizer ? "(Starting with random)" : "(Starting with regular next attack)");

                        while (recentAttacks.Count > 0)
                        {
                            bool foundAttackToUse = false;

                            for (int i = 0; i < 5; i++) //try to get next attack that isnt in this queue
                            {
                                if (!recentAttacks.Contains(NPC.ai[2]))
                                {
                                    foundAttackToUse = true;
                                    break;
                                }
                                NPC.ai[2] = Main.rand.Next(args);
                            }

                            if (foundAttackToUse)
                                break;

                            //couldn't find an attack to use after those attempts, forget 1 attack and repeat
                            recentAttacks.Dequeue();

                            //Main.NewText("REDUCE");
                        }

                        /*text = "";
                        foreach (float f in recentAttacks)
                            text += f.ToString() + " ";
                        Main.NewText($"recent: {text}");*/
                    }
                }

                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    int maxMemory = 16;

                    if (++attackCount > maxMemory * 1.25) //after doing this many attacks, shorten queue so i can be more random again
                    {
                        attackCount = 0;
                        maxMemory /= 2;
                    }

                    attackHistory.Enqueue(NPC.ai[2]);
                    while (attackHistory.Count > maxMemory)
                        attackHistory.Dequeue();
                }

                /*text = "";
                foreach (float f in attackHistory)
                    text += f.ToString() + " ";
                Main.NewText($"after: {text}");*/
            };

            float GetSpinOffset()
            {
                float newRotation = NPC.DirectionTo(Main.player[NPC.target].Center).ToRotation();
                float difference = MathHelper.WrapAngle(newRotation - NPC.ai[3]);
                float rotationDirection = 2f * (float)Math.PI * 1f / 6f / 60f;
                rotationDirection *= FargoSoulsWorld.MasochistModeReal ? 1.05f : 0.95f;
                return Math.Min(rotationDirection, Math.Abs(difference)) * Math.Sign(difference);
            }

            void SpawnSphereRing(int max, float speed, int damage, float rotationModifier, float offset = 0)
            {
                if (Main.netMode == NetmodeID.MultiplayerClient) return;
                float rotation = 2f * (float)Math.PI / max;
                int type = ModContent.ProjectileType<MutantSphereRing>();
                for (int i = 0; i < max; i++)
                {
                    Vector2 vel = speed * Vector2.UnitY.RotatedBy(rotation * i + offset);
                    Projectile.NewProjectile(NPC.GetSpawnSource_ForProjectile(), NPC.Center, vel, type, damage, 0f, Main.myPlayer, rotationModifier * NPC.spriteDirection, speed);
                }
                Terraria.Audio.SoundEngine.PlaySound(SoundID.Item84, NPC.Center);
            }

            /*void Aura(float distance, int buff, bool reverse = false, int dustid = DustID.GoldFlame, bool checkDuration = false, bool targetEveryone = true)
            {
                //works because buffs are client side anyway :ech:
                Player p = targetEveryone ? Main.LocalPlayer : Main.player[NPC.target];
                float range = NPC.Distance(p.Center);
                if (reverse ? range > distance && range < 5000f : range < distance)
                    p.AddBuff(buff, checkDuration && Main.expertMode && Main.expertDebuffTime > 1 ? 1 : 2);

                for (int i = 0; i < 30; i++)
                {
                    Vector2 offset = new Vector2();
                    double angle = Main.rand.NextDouble() * 2d * Math.PI;
                    offset.X += (float)(Math.Sin(angle) * distance);
                    offset.Y += (float)(Math.Cos(angle) * distance);
                    Dust dust = Main.dust[Dust.NewDust(
                        NPC.Center + offset - new Vector2(4, 4), 0, 0,
                        dustid, 0, 0, 100, Color.White, 1.5f)];
                    dust.velocity = NPC.velocity;
                    if (Main.rand.NextBool(3))
                        dust.velocity += Vector2.Normalize(offset) * (reverse ? 5f : -5f);
                    dust.noGravity = true;
                }
            }*/

            bool AliveCheck(Player p)
            {
                if (FargoSoulsWorld.SwarmActive || ((!p.active || p.dead || Vector2.Distance(NPC.Center, p.Center) > 5000f) && NPC.localAI[3] > 0))
                {
                    NPC.TargetClosest();
                    p = Main.player[NPC.target];
                    if (FargoSoulsWorld.SwarmActive || !p.active || p.dead || Vector2.Distance(NPC.Center, p.Center) > 5000f)
                    {
                        if (NPC.timeLeft > 30)
                            NPC.timeLeft = 30;
                        NPC.velocity.Y -= 1f;
                        if (NPC.timeLeft == 1)
                        {
                            if (NPC.position.Y < 0)
                                NPC.position.Y = 0;
                            if (Main.netMode != NetmodeID.MultiplayerClient && ModContent.TryFind("Fargowiltas", "Mutant", out ModNPC modNPC) && !NPC.AnyNPCs(modNPC.Type))
                            {
                                FargoSoulsUtil.ClearHostileProjectiles(2, NPC.whoAmI);
                                int n = NPC.NewNPC(NPC.GetSpawnSourceForNPCFromNPCAI(), (int)NPC.Center.X, (int)NPC.Center.Y, modNPC.Type);
                                if (n != Main.maxNPCs)
                                {
                                    Main.npc[n].homeless = true;
                                    if (Main.netMode == NetmodeID.Server)
                                        NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, n);
                                }
                            }
                        }
                        return false;
                    }
                }
                if (NPC.timeLeft < 600)
                    NPC.timeLeft = 600;
                return true;
            }

            bool Phase2Check()
            {
                if (Main.expertMode && NPC.life < NPC.lifeMax / 2)
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        NPC.ai[0] = 10;
                        NPC.ai[1] = 0;
                        NPC.ai[2] = 0;
                        NPC.ai[3] = 0;
                        NPC.netUpdate = true;
                        FargoSoulsUtil.ClearHostileProjectiles(1, NPC.whoAmI);
                        //EdgyBossText("Time to stop playing around.");
                    }
                    return true;
                }
                return false;
            }

            void Movement(Vector2 target, float speed, bool fastX = true)
            {
                if (Math.Abs(NPC.Center.X - target.X) > 10)
                {
                    if (NPC.Center.X < target.X)
                    {
                        NPC.velocity.X += speed;
                        if (NPC.velocity.X < 0)
                            NPC.velocity.X += speed * (fastX ? 2 : 1);
                    }
                    else
                    {
                        NPC.velocity.X -= speed;
                        if (NPC.velocity.X > 0)
                            NPC.velocity.X -= speed * (fastX ? 2 : 1);
                    }
                }
                if (NPC.Center.Y < target.Y)
                {
                    NPC.velocity.Y += speed;
                    if (NPC.velocity.Y < 0)
                        NPC.velocity.Y += speed * 2;
                }
                else
                {
                    NPC.velocity.Y -= speed;
                    if (NPC.velocity.Y > 0)
                        NPC.velocity.Y -= speed * 2;
                }
                if (Math.Abs(NPC.velocity.X) > 24)
                    NPC.velocity.X = 24 * Math.Sign(NPC.velocity.X);
                if (Math.Abs(NPC.velocity.Y) > 24)
                    NPC.velocity.Y = 24 * Math.Sign(NPC.velocity.Y);
            }

            /*if (NPC.ai[0] < 0 && NPC.ai[0] > -6)
            {
                if (++NPC.localAI[2] > 360)
                {
                    NPC.localAI[2] = 0;
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                        Item.NewItem(NPC.Hitbox, ItemID.NebulaPickup2);
                }
            }*/

            Player player = Main.player[NPC.target];

            void DramaticTransition(bool fightIsOver, bool normalAnimation = true)
            {
                if (fightIsOver)
                {
                    player.ClearBuff(ModContent.BuffType<MutantFang>());
                    player.ClearBuff(ModContent.BuffType<AbomRebirth>());
                }

                Terraria.Audio.SoundEngine.PlaySound(SoundID.Item, (int)NPC.Center.X, (int)NPC.Center.Y, 27, 1.5f);

                if (normalAnimation)
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                        Projectile.NewProjectile(NPC.GetSpawnSource_ForProjectile(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<MutantBomb>(), 0, 0f, Main.myPlayer);
                }
                
                const int max = 40;
                for (int i = 0; i < max; i++)
                {
                    int heal = (int)(Main.rand.NextFloat(0.9f, 1.1f) * (fightIsOver ? player.statLifeMax2 / 4 : NPC.lifeMax * 0.6f) / max);
                    Vector2 vel = normalAnimation
                        ? Main.rand.NextFloat(2f, 18f) * -Vector2.UnitY.RotatedByRandom(MathHelper.TwoPi) //looks messier normally
                        : 0.1f * -Vector2.UnitY.RotatedBy(MathHelper.TwoPi / max * i); //looks controlled during mutant p1 skip
                    float ai0 = fightIsOver ? -player.whoAmI - 1 : NPC.whoAmI; //player -1 necessary for edge case of player 0
                    float ai1 = vel.Length() / Main.rand.Next(fightIsOver ? 90 : 150, 180); //window in which they begin homing in
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                        Projectile.NewProjectile(NPC.GetSpawnSource_ForProjectile(), NPC.Center, vel, ModContent.ProjectileType<MutantHeal>(), heal, 0f, Main.myPlayer, ai0, ai1);
                }
            };

            NPC.direction = NPC.spriteDirection = NPC.Center.X < player.Center.X ? 1 : -1;
            Vector2 targetPos;
            float speedModifier;
            switch ((int)NPC.ai[0])
            {
                case -7: //fade out, drop a mutant
                    NPC.velocity = Vector2.Zero;
                    for (int i = 0; i < 5; i++)
                    {
                        int d = Dust.NewDust(NPC.position, NPC.width, NPC.height, 229, 0f, 0f, 0, default(Color), 2.5f);
                        Main.dust[d].noGravity = true;
                        Main.dust[d].noLight = true;
                        Main.dust[d].velocity *= 12f;
                    }
                    if (--NPC.localAI[0] < 0)
                    {
                        NPC.localAI[0] = Main.rand.Next(5);
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            Vector2 spawnPos = NPC.Center + Main.rand.NextVector2Circular(240, 240);
                            int type = ModContent.ProjectileType<Projectiles.BossWeapons.PhantasmalBlast>();
                            Projectile.NewProjectile(NPC.GetSpawnSource_ForProjectile(), spawnPos, Vector2.Zero, type, 0, 0f, Main.myPlayer);
                        }
                    }
                    if (++NPC.ai[1] % 3 == 0 && Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Projectile.NewProjectile(NPC.GetSpawnSource_ForProjectile(), NPC.Center, 24f * Vector2.UnitX.RotatedBy(NPC.ai[3]), ModContent.ProjectileType<MutantEyeWavy>(), 0, 0f, Main.myPlayer, 
                            Main.rand.NextFloat(0.75f, 1.5f) * (Main.rand.NextBool() ? -1 : 1), Main.rand.Next(10, 90));
                    }
                    if (++NPC.alpha > 255)
                    {
                        NPC.alpha = 255;
                        NPC.life = 0;
                        NPC.dontTakeDamage = false;
                        NPC.checkDead();
                        if (Main.netMode != NetmodeID.MultiplayerClient && ModContent.TryFind("Fargowiltas", "Mutant", out ModNPC modNPC) && !NPC.AnyNPCs(modNPC.Type))
                        {
                            int n = NPC.NewNPC(NPC.GetSpawnSourceForNPCFromNPCAI(), (int)NPC.Center.X, (int)NPC.Center.Y, modNPC.Type);
                            if (n != Main.maxNPCs)
                            {
                                Main.npc[n].homeless = true;
                                if (Main.netMode == NetmodeID.Server)
                                    NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, n);
                            }
                        }
                        //EdgyBossText("Oh, right... my revive...");
                    }
                    break;

                case -6: //actually defeated
                    if (!AliveCheck(player))
                        break;
                    NPC.ai[3] -= (float)Math.PI / 6f / 60f;
                    NPC.velocity = Vector2.Zero;
                    if (++NPC.ai[1] > 120)
                    {
                        NPC.netUpdate = true;
                        NPC.ai[0]--;
                        NPC.ai[1] = 0;
                        NPC.ai[3] = (float)-Math.PI / 2;
                        NPC.netUpdate = true;
                        if (Main.netMode != NetmodeID.MultiplayerClient) //shoot harmless mega ray
                            Projectile.NewProjectile(NPC.GetSpawnSource_ForProjectile(), NPC.Center, Vector2.UnitY * -1, ModContent.ProjectileType<MutantGiantDeathray2>(), 0, 0f, Main.myPlayer, 0, NPC.whoAmI);
                        //EdgyBossText("I have not a single regret in my existence!");
                    }
                    if (--NPC.localAI[0] < 0)
                    {
                        NPC.localAI[0] = Main.rand.Next(15);
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            Vector2 spawnPos = NPC.position + new Vector2(Main.rand.Next(NPC.width), Main.rand.Next(NPC.height));
                            int type = ModContent.ProjectileType<Projectiles.BossWeapons.PhantasmalBlast>();
                            Projectile.NewProjectile(NPC.GetSpawnSource_ForProjectile(), spawnPos, Vector2.Zero, type, 0, 0f, Main.myPlayer);
                        }
                    }
                    for (int i = 0; i < 5; i++)
                    {
                        int d = Dust.NewDust(NPC.position, NPC.width, NPC.height, 229, 0f, 0f, 0, default(Color), 1.5f);
                        Main.dust[d].noGravity = true;
                        Main.dust[d].noLight = true;
                        Main.dust[d].velocity *= 4f;
                    }
                    break;

                case -5: //FINAL SPARK
                    if (--NPC.localAI[0] < 0)
                    {
                        NPC.localAI[0] = Main.rand.Next(30);
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            Vector2 spawnPos = NPC.position + new Vector2(Main.rand.Next(NPC.width), Main.rand.Next(NPC.height));
                            int type = ModContent.ProjectileType<Projectiles.BossWeapons.PhantasmalBlast>();
                            Projectile.NewProjectile(NPC.GetSpawnSource_ForProjectile(), spawnPos, Vector2.Zero, type, 0, 0f, Main.myPlayer);
                        }
                    }

                    if (++NPC.ai[1] > 120)
                    {
                        NPC.ai[1] = 0;
                        if (AliveCheck(player) && Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            int damage = NPC.damage / 4;
                            if (FargoSoulsWorld.MasochistModeReal)
                                damage /= 4; //because they inflict timestop instead.
                            SpawnSphereRing(10, 6f, damage, 0.5f);
                            SpawnSphereRing(10, 6f, damage, -.5f);
                        }
                    }

                    if (NPC.ai[2] == 0)
                    {
                        if (!AliveCheck(player))
                            break;
                    }
                    else if (NPC.ai[2] == 420 - 90) //dramatic telegraph
                    {
                        Terraria.Audio.SoundEngine.PlaySound(SoundID.Roar, (int)NPC.Center.X, (int)NPC.Center.Y, 0);
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            const int max = 8;
                            for (int i = 0; i < max; i++)
                            {
                                float offset = i - 0.5f;
                                Projectile.NewProjectile(NPC.GetSpawnSource_ForProjectile(), NPC.Center, (NPC.ai[3] + MathHelper.TwoPi / max * offset).ToRotationVector2(), ModContent.ProjectileType<Projectiles.GlowLine>(), 0, 0f, Main.myPlayer, 13f, NPC.whoAmI);
                            }
                        }
                    }

                    if (NPC.ai[2] < 420)
                    {
                        NPC.ai[3] = NPC.DirectionFrom(player.Center).ToRotation(); //hold it here for glow line effect
                        if (FargoSoulsWorld.MasochistModeReal)
                            NPC.ai[3] += MathHelper.PiOver4;
                    }
                    else if (NPC.ai[2] % 3 == 0)
                    {
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            Projectile.NewProjectile(NPC.GetSpawnSource_ForProjectile(), NPC.Center, 24f * Vector2.UnitX.RotatedBy(NPC.ai[3]), ModContent.ProjectileType<MutantEyeWavy>(), 0, 0f, Main.myPlayer, 
                                Main.rand.NextFloat(0.5f, 1.25f) * (Main.rand.NextBool() ? -1 : 1), Main.rand.Next(10, 60));
                        }
                    }

                    if (++NPC.ai[2] > (FargoSoulsWorld.MasochistModeReal ? 1020 + 150 : 1020))
                    {
                        NPC.netUpdate = true;
                        NPC.ai[0]--;
                        NPC.ai[1] = 0;
                        NPC.ai[2] = 0;
                        FargoSoulsUtil.ClearAllProjectiles(2, NPC.whoAmI);
                    }
                    else if (NPC.ai[2] == 420)
                    {
                        if (!AliveCheck(player))
                        {
                            NPC.ai[2]--;
                            break;
                        }

                        NPC.netUpdate = true;

                        if (!FargoSoulsWorld.MasochistModeReal)
                        {
                            NPC.ai[3] -= MathHelper.ToRadians(10); //bias it in one direction
                        }

                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            Projectile.NewProjectile(NPC.GetSpawnSource_ForProjectile(), NPC.Center, Vector2.UnitX.RotatedBy(NPC.ai[3]),
                                ModContent.ProjectileType<MutantGiantDeathray2>(), NPC.damage / 8, 0f, Main.myPlayer, 0, NPC.whoAmI);
                        }
                    }
                    else if (NPC.ai[2] < 300) //charging up dust
                    {
                        float num1 = 0.99f;
                        if (NPC.ai[2] >= 60)
                            num1 = 0.79f;
                        if (NPC.ai[2] >= 120)
                            num1 = 0.58f;
                        if (NPC.ai[2] >= 180)
                            num1 = 0.43f;
                        if (NPC.ai[2] >= 240)
                            num1 = 0.33f;
                        for (int i = 0; i < 9; ++i)
                        {
                            if (Main.rand.NextFloat() >= num1)
                            {
                                float f = Main.rand.NextFloat() * 6.283185f;
                                float num2 = Main.rand.NextFloat();
                                Dust dust = Dust.NewDustPerfect(NPC.Center + f.ToRotationVector2() * (110 + 600 * num2), 229, (f - 3.141593f).ToRotationVector2() * (14 + 8 * num2), 0, default, 1f);
                                dust.scale = 0.9f;
                                dust.fadeIn = 1.15f + num2 * 0.3f;
                                //dust.color = new Color(1f, 1f, 1f, num1) * (1f - num1);
                                dust.noGravity = true;
                                //dust.noLight = true;
                            }
                        }
                    }

                    /*for (int i = 0; i < 5; i++)
                    {
                        int d = Dust.NewDust(NPC.position, NPC.width, NPC.height, 229, 0f, 0f, 0, default(Color), 1.5f);
                        Main.dust[d].noGravity = true;
                        Main.dust[d].noLight = true;
                        Main.dust[d].velocity *= 4f;
                    }*/
                    NPC.ai[3] += GetSpinOffset();

                    NPC.velocity = Vector2.Zero;
                    break;

                case -4: //true boundary
                    if (NPC.localAI[0] == 0)
                    {
                        if (!AliveCheck(player))
                            break;
                        NPC.localAI[0] = Math.Sign(NPC.Center.X - player.Center.X);
                    }
                    if (++NPC.ai[1] > 3)
                    {
                        Terraria.Audio.SoundEngine.PlaySound(SoundID.Item12, NPC.Center);
                        NPC.ai[1] = 0;
                        NPC.ai[2] += (float)Math.PI / 5 / 420 * NPC.ai[3] * NPC.localAI[0] * (FargoSoulsWorld.MasochistModeReal ? 2f : 1);
                        if (NPC.ai[2] > (float)Math.PI)
                            NPC.ai[2] -= (float)Math.PI * 2;
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            int max = FargoSoulsWorld.MasochistModeReal ? 9 : 8;
                            for (int i = 0; i < max; i++)
                            {
                                Projectile.NewProjectile(NPC.GetSpawnSource_ForProjectile(), NPC.Center, new Vector2(0f, -6f).RotatedBy(NPC.ai[2] + MathHelper.TwoPi / max * i),
                                    ModContent.ProjectileType<MutantEye>(), NPC.damage / 4, 0f, Main.myPlayer);
                            }
                        }
                    }
                    if (++NPC.ai[3] > 480)
                    {
                        //NPC.TargetClosest();
                        NPC.ai[0]--;
                        NPC.ai[1] = 0;
                        NPC.ai[2] = 0;
                        NPC.ai[3] = 0;
                        NPC.localAI[0] = 0;
                        NPC.netUpdate = true;
                    }
                    for (int i = 0; i < 5; i++)
                    {
                        int d = Dust.NewDust(NPC.position, NPC.width, NPC.height, 229, 0f, 0f, 0, default(Color), 1.5f);
                        Main.dust[d].noGravity = true;
                        Main.dust[d].noLight = true;
                        Main.dust[d].velocity *= 4f;
                    }

                    NPC.velocity = Vector2.Zero;
                    break;

                case -3: //okuu nonspell
                    if (NPC.ai[2] == 0)
                    {
                        if (!AliveCheck(player))
                            break;
                        NPC.ai[2] = Main.rand.NextBool() ? -1 : 1;
                        NPC.ai[3] = Main.rand.NextFloat((float)Math.PI * 2);
                    }

                    if (++NPC.ai[1] > 10 && NPC.ai[3] > 60 && NPC.ai[3] < 300)
                    {
                        NPC.ai[1] = 0;
                        float rotation = MathHelper.ToRadians(45) * (NPC.ai[3] - 60) / 240 * NPC.ai[2];
                        float speed = FargoSoulsWorld.MasochistModeReal ? 11f : 10f;
                        SpawnSphereRing(11, speed, NPC.damage / 4, -0.75f, rotation);
                        SpawnSphereRing(11, speed, NPC.damage / 4, 0.75f, rotation);
                    }

                    if (++NPC.ai[3] > 420)
                    {
                        NPC.netUpdate = true;
                        NPC.ai[0]--;
                        NPC.ai[1] = 0;
                        NPC.ai[2] = 0;
                        NPC.ai[3] = 0;
                        //NPC.TargetClosest();
                    }
                    for (int i = 0; i < 5; i++)
                    {
                        int d = Dust.NewDust(NPC.position, NPC.width, NPC.height, 229, 0f, 0f, 0, default(Color), 1.5f);
                        Main.dust[d].noGravity = true;
                        Main.dust[d].noLight = true;
                        Main.dust[d].velocity *= 4f;
                    }

                    NPC.velocity = Vector2.Zero;
                    break;

                case -2: //final void rays
                    if (--NPC.ai[1] < 0)
                    {
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            Projectile.NewProjectile(NPC.GetSpawnSource_ForProjectile(), NPC.Center, new Vector2(2, 0).RotatedBy(NPC.ai[2]), ModContent.ProjectileType<MutantMark1>(), NPC.damage / 4, 0f, Main.myPlayer);
                        }
                        NPC.ai[1] = 1;
                        NPC.ai[2] += NPC.ai[3];
                        if (NPC.localAI[0]++ == 40 || NPC.localAI[0] == 80)
                        {
                            NPC.netUpdate = true;
                            NPC.ai[2] -= NPC.ai[3] / 2;
                        }
                        else if (NPC.localAI[0] >= 120)
                        {
                            NPC.netUpdate = true;
                            NPC.ai[0]--;
                            NPC.ai[1] = 0;
                            NPC.ai[2] = 0;
                            NPC.ai[3] = 0;
                            NPC.localAI[0] = 0;
                        }
                    }
                    for (int i = 0; i < 5; i++)
                    {
                        int d = Dust.NewDust(NPC.position, NPC.width, NPC.height, 229, 0f, 0f, 0, default(Color), 1.5f);
                        Main.dust[d].noGravity = true;
                        Main.dust[d].noLight = true;
                        Main.dust[d].velocity *= 4f;
                    }

                    NPC.velocity = Vector2.Zero;
                    break;

                case -1: //defeated
                    NPC.localAI[3] = 3;

                    if (FargoSoulsWorld.EternityMode)
                    {
                        if (!SkyManager.Instance["FargowiltasSouls:MutantBoss"].IsActive())
                            SkyManager.Instance.Activate("FargowiltasSouls:MutantBoss");

                        if (SoulConfig.Instance.MutantMusicIsRePrologue && ModLoader.TryGetMod("FargowiltasMusic", out Mod musicMod))
                            Music = MusicLoader.GetMusicSlot(musicMod, "Assets/Music/rePrologue");
                    }

                    //NPC.damage = 0;
                    if (NPC.buffType[0] != 0)
                        NPC.DelBuff(0);
                    if (NPC.ai[1] == 0) //entering final phase, give healing
                    {
                        DramaticTransition(true);
                    }

                    if (NPC.ai[1] < 60 && !Main.dedServ && Main.LocalPlayer.active)
                        Main.LocalPlayer.GetModPlayer<FargoSoulsPlayer>().Screenshake = 2;

                    if (++NPC.ai[1] > 300)
                    {
                        if (!AliveCheck(player))
                            break;
                        targetPos = player.Center;
                        targetPos.Y -= 300;
                        Movement(targetPos, 1f);
                        if (NPC.Distance(targetPos) < 50 || NPC.ai[1] > 600)
                        {
                            NPC.netUpdate = true;
                            NPC.velocity = Vector2.Zero;
                            NPC.localAI[0] = 0;
                            NPC.ai[0]--;
                            NPC.ai[1] = 0;
                            NPC.ai[2] = NPC.DirectionFrom(player.Center).ToRotation();
                            NPC.ai[3] = (float)Math.PI / 20f;
                            Terraria.Audio.SoundEngine.PlaySound(SoundID.Roar, (int)NPC.Center.X, (int)NPC.Center.Y, 0);
                            if (player.Center.X < NPC.Center.X)
                                NPC.ai[3] *= -1;
                            //EdgyBossText("But we're not done yet!");
                        }
                    }
                    else
                    {
                        NPC.velocity *= 0.9f;

                        //make you stop attacking
                        if (Main.LocalPlayer.active && !Main.LocalPlayer.dead && !Main.LocalPlayer.ghost && NPC.Distance(Main.LocalPlayer.Center) < 3000)
                        {
                            Main.LocalPlayer.itemTime = 0;
                            Main.LocalPlayer.itemAnimation = 0;
                            Main.LocalPlayer.reuseDelay = 2;
                        }

                        if (--NPC.localAI[0] < 0)
                        {
                            NPC.localAI[0] = Main.rand.Next(15);
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                Vector2 spawnPos = NPC.position + new Vector2(Main.rand.Next(NPC.width), Main.rand.Next(NPC.height));
                                int type = ModContent.ProjectileType<Projectiles.BossWeapons.PhantasmalBlast>();
                                Projectile.NewProjectile(NPC.GetSpawnSource_ForProjectile(), spawnPos, Vector2.Zero, type, 0, 0f, Main.myPlayer);
                            }
                        }
                    }

                    for (int i = 0; i < 5; i++)
                    {
                        int d = Dust.NewDust(NPC.position, NPC.width, NPC.height, 229, 0f, 0f, 0, default(Color), 1.5f);
                        Main.dust[d].noGravity = true;
                        Main.dust[d].noLight = true;
                        Main.dust[d].velocity *= 4f;
                    }
                    break;

                case 0: //track player, throw penetrators
                    if (!AliveCheck(player))
                        break;
                    if (Phase2Check())
                        break;
                    NPC.localAI[2] = 0;
                    targetPos = player.Center;
                    targetPos.X += 500 * (NPC.Center.X < targetPos.X ? -1 : 1);
                    if (NPC.Distance(targetPos) > 50)
                    {
                        speedModifier = NPC.localAI[3] > 0 ? 0.5f : 2f;
                        if (NPC.Center.X < targetPos.X)
                        {
                            NPC.velocity.X += speedModifier;
                            if (NPC.velocity.X < 0)
                                NPC.velocity.X += speedModifier * 2;
                        }
                        else
                        {
                            NPC.velocity.X -= speedModifier;
                            if (NPC.velocity.X > 0)
                                NPC.velocity.X -= speedModifier * 2;
                        }
                        if (NPC.Center.Y < targetPos.Y)
                        {
                            NPC.velocity.Y += speedModifier;
                            if (NPC.velocity.Y < 0)
                                NPC.velocity.Y += speedModifier * 2;
                        }
                        else
                        {
                            NPC.velocity.Y -= speedModifier;
                            if (NPC.velocity.Y > 0)
                                NPC.velocity.Y -= speedModifier * 2;
                        }
                        if (NPC.localAI[3] > 0)
                        {
                            if (Math.Abs(NPC.velocity.X) > 24)
                                NPC.velocity.X = 24 * Math.Sign(NPC.velocity.X);
                            if (Math.Abs(NPC.velocity.Y) > 24)
                                NPC.velocity.Y = 24 * Math.Sign(NPC.velocity.Y);
                        }
                    }

                    if (NPC.localAI[3] > 0) //dont begin proper ai timer until in range to begin fight
                        NPC.ai[1]++;

                    if (NPC.ai[1] < 145) //track player up until just before attack
                    {
                        NPC.localAI[0] = NPC.DirectionTo(player.Center + player.velocity * 30f).ToRotation();
                    }

                    if (NPC.ai[1] > 150) //120)
                    {
                        NPC.netUpdate = true;
                        //NPC.TargetClosest();
                        NPC.ai[1] = 60;
                        if (++NPC.ai[2] > 5)
                        {
                            NPC.ai[0]++;
                            NPC.ai[1] = 0;
                            NPC.ai[2] = 0;
                            NPC.velocity = NPC.DirectionTo(player.Center) * 2f;
                        }
                        else if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            Vector2 vel = NPC.localAI[0].ToRotationVector2() * 25f;
                            //Projectile.NewProjectile(NPC.GetSpawnSource_ForProjectile(), NPC.Center, Vector2.Normalize(vel), ModContent.ProjectileType<MutantDeathray2>(), NPC.damage / 5, 0f, Main.myPlayer);
                            //Projectile.NewProjectile(NPC.GetSpawnSource_ForProjectile(), NPC.Center, -Vector2.Normalize(vel), ModContent.ProjectileType<MutantDeathray2>(), NPC.damage / 5, 0f, Main.myPlayer);
                            Projectile.NewProjectile(NPC.GetSpawnSource_ForProjectile(), NPC.Center, vel, ModContent.ProjectileType<MutantSpearThrown>(), NPC.damage / 4, 0f, Main.myPlayer, NPC.target);

                            //Projectile.NewProjectile(NPC.GetSpawnSource_ForProjectile(), NPC.Center, NPC.DirectionTo(player.Center) * 25f, ModContent.ProjectileType<MutantSpearThrown>(), NPC.damage / 4, 0f, Main.myPlayer, NPC.target);
                        }
                        NPC.localAI[0] = 0;
                    }
                    else if (NPC.ai[1] == 61 && NPC.ai[2] < 5 && Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        if (FargoSoulsWorld.EternityMode && FargoSoulsWorld.skipMutantP1 >= 10)
                        {
                            NPC.ai[0] = 10;
                            NPC.ai[1] = 0;
                            NPC.ai[2] = 0;
                            NPC.ai[3] = 0;
                            NPC.localAI[0] = 0;
                            NPC.netUpdate = true;

                            if (FargoSoulsWorld.skipMutantP1 == 10)
                                FargoSoulsUtil.PrintText("Mutant tires of the charade...", Color.LimeGreen);

                            if (FargoSoulsWorld.skipMutantP1 >= 10)
                                NPC.ai[2] = 1; //flag for different p2 transition animation
                            break;
                        }

                        Projectile.NewProjectile(NPC.GetSpawnSource_ForProjectile(), NPC.Center, NPC.DirectionTo(player.Center + player.velocity * 30f), ModContent.ProjectileType<MutantDeathrayAim>(), 0, 0f, Main.myPlayer, 85f, NPC.whoAmI);
                        Projectile.NewProjectile(NPC.GetSpawnSource_ForProjectile(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<MutantSpearAim>(), NPC.damage / 4, 0f, Main.myPlayer, NPC.whoAmI, 3);

                        //Projectile.NewProjectile(NPC.GetSpawnSource_ForProjectile(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<MutantSpearAim>(), NPC.damage / 4, 0f, Main.myPlayer, NPC.whoAmI);
                    }
                    break;

                case 1: //slow drift, shoot phantasmal rings
                    if (Phase2Check())
                        break;
                    if (--NPC.ai[1] < 0)
                    {
                        NPC.netUpdate = true;
                        NPC.ai[1] = 90;
                        if (++NPC.ai[2] > 4)
                        {
                            NPC.ai[0]++;
                            NPC.ai[1] = 0;
                            NPC.ai[2] = 0;
                            NPC.ai[3] = 0;
                            NPC.netUpdate = true;
                            //NPC.TargetClosest();
                        }
                        else
                        {
                            SpawnSphereRing(6, 9f, NPC.damage / 5, 1f);
                            SpawnSphereRing(6, 9f, NPC.damage / 5, -0.5f);
                        }
                    }
                    break;

                case 2: //fly up to corner above player and then dive
                    if (!AliveCheck(player))
                        break;
                    if (Phase2Check())
                        break;
                    targetPos = player.Center;
                    targetPos.X += 700 * (NPC.Center.X < targetPos.X ? -1 : 1);
                    targetPos.Y -= 400;
                    Movement(targetPos, 0.6f);
                    if (NPC.Distance(targetPos) < 50 || ++NPC.ai[1] > 180) //dive here
                    {
                        NPC.velocity.X = 35f * (NPC.position.X < player.position.X ? 1 : -1);
                        if (NPC.velocity.Y < 0)
                            NPC.velocity.Y *= -1;
                        NPC.velocity.Y *= 0.3f;
                        NPC.ai[0]++;
                        NPC.ai[1] = 0;
                        NPC.netUpdate = true;
                        Terraria.Audio.SoundEngine.PlaySound(SoundID.Roar, (int)NPC.Center.X, (int)NPC.Center.Y, 0);
                    }
                    break;

                case 3: //diving, spawning true eyes
                    if (NPC.ai[2] > 3)
                    {
                        targetPos = player.Center;
                        targetPos.X += NPC.Center.X < player.Center.X ? -500 : 500;
                        if (NPC.Distance(targetPos) > 50)
                            Movement(targetPos, 0.3f);
                    }
                    else
                    {
                        NPC.velocity *= 0.99f;
                    }

                    if (--NPC.ai[1] < 0)
                    {
                        NPC.ai[1] = 15;
                        if (++NPC.ai[2] > 8)
                        {
                            if (NPC.ai[0] == 3)
                            {
                                NPC.ai[0]++;
                                NPC.ai[1] = 0;
                                NPC.ai[2] = 0;
                                NPC.netUpdate = true;
                                //NPC.TargetClosest();
                            }
                            else
                            {
                                ChooseNextAttack(13, 18, 21, 26, 33, 33, 33, 41);
                            }
                            
                        }
                        else if (NPC.ai[2] <= 3)
                        {
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                int type = ModContent.ProjectileType<MutantTrueEyeL>();
                                if (NPC.ai[2] == 2)
                                    type = ModContent.ProjectileType<MutantTrueEyeR>();
                                else if (NPC.ai[2] == 3)
                                    type = ModContent.ProjectileType<MutantTrueEyeS>();
                                Projectile.NewProjectile(NPC.GetSpawnSource_ForProjectile(), NPC.Center, Vector2.Zero, type, NPC.damage / 5, 0f, Main.myPlayer, NPC.target);
                            }
                            Terraria.Audio.SoundEngine.PlaySound(SoundID.Item92, NPC.Center);
                            for (int i = 0; i < 30; i++)
                            {
                                int d = Dust.NewDust(NPC.position, NPC.width, NPC.height, 135, 0f, 0f, 0, default(Color), 3f);
                                Main.dust[d].noGravity = true;
                                Main.dust[d].noLight = true;
                                Main.dust[d].velocity *= 12f;
                            }
                        }
                    }
                    break;

                case 4: //maneuvering under player while spinning penetrator
                    if (Phase2Check())
                        break;
                    if (NPC.ai[3] == 0)
                    {
                        if (!AliveCheck(player))
                            break;
                        NPC.ai[3] = 1;
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                            Projectile.NewProjectile(NPC.GetSpawnSource_ForProjectile(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<MutantSpearSpin>(), NPC.damage / 4, 0f, Main.myPlayer, NPC.whoAmI, 240); // 250);
                    }
                    
                    if (++NPC.ai[1] > 240)
                    {
                        if (!AliveCheck(player))
                            break;
                        NPC.netUpdate = true;
                        NPC.ai[0]++;
                        NPC.ai[1] = 0;
                        NPC.ai[3] = 0;
                    }

                    targetPos = player.Center;
                    targetPos.Y += 400f;
                    Movement(targetPos, 0.7f, false);
                    break;

                case 5: //pause and then initiate dash
                    if (Phase2Check())
                        break;
                    NPC.velocity *= 0.9f;
                    if (++NPC.ai[1] > 10)
                    {
                        NPC.netUpdate = true;
                        NPC.ai[0]++;
                        NPC.ai[1] = 0;
                        if (++NPC.ai[2] > 5)
                        {
                            NPC.ai[0]++; //go to next attack after dashes
                            NPC.ai[2] = 0;
                        }
                        else
                        {
                            NPC.velocity = NPC.DirectionTo(player.Center + player.velocity) * 30f;
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                                Projectile.NewProjectile(NPC.GetSpawnSource_ForProjectile(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<MutantSpearDash>(), NPC.damage / 4, 0f, Main.myPlayer, NPC.whoAmI);
                        }
                    }
                    break;

                case 6: //while dashing
                    NPC.direction = NPC.spriteDirection = Math.Sign(NPC.velocity.X);
                    if (++NPC.ai[1] > 30)
                    {
                        if (!AliveCheck(player))
                            break;
                        NPC.netUpdate = true;
                        NPC.ai[0]--;
                        NPC.ai[1] = 0;
                    }
                    break;

                case 7: //approach for lasers
                    if (!AliveCheck(player))
                        break;
                    if (Phase2Check())
                        break;
                    targetPos = player.Center + player.DirectionTo(NPC.Center) * 250;
                    if (NPC.Distance(targetPos) > 50 && ++NPC.ai[2] < 180)
                    {
                        Movement(targetPos, 0.5f);
                    }
                    else
                    {
                        NPC.netUpdate = true;
                        NPC.ai[0]++;
                        NPC.ai[1] = 0;
                        NPC.ai[2] = player.DirectionTo(NPC.Center).ToRotation();
                        NPC.ai[3] = (float)Math.PI / 10f;
                        if (player.Center.X < NPC.Center.X)
                            NPC.ai[3] *= -1;
                    }
                    break;

                case 8: //fire lasers in ring
                    if (Phase2Check())
                        break;
                    NPC.velocity = Vector2.Zero;
                    if (--NPC.ai[1] < 0)
                    {
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                            Projectile.NewProjectile(NPC.GetSpawnSource_ForProjectile(), NPC.Center, new Vector2(2, 0).RotatedBy(NPC.ai[2]), ModContent.ProjectileType<MutantMark1>(), NPC.damage / 4, 0f, Main.myPlayer);
                        NPC.ai[1] = 5;
                        NPC.ai[2] += NPC.ai[3];
                        if (NPC.localAI[0]++ == 20)
                        {
                            NPC.netUpdate = true;
                            NPC.ai[2] -= NPC.ai[3] / 2;
                        }
                        else if (NPC.localAI[0] >= 40)
                        {
                            NPC.netUpdate = true;
                            NPC.ai[0]++;
                            NPC.ai[1] = 0;
                            NPC.ai[2] = 0;
                            NPC.ai[3] = 0;
                            NPC.localAI[0] = 0;
                        }
                    }
                    break;

                case 9: //boundary lite
                    switch ((int)NPC.localAI[2])
                    {
                        case 0:
                            if (NPC.ai[3] == 0)
                            {
                                if (AliveCheck(player))
                                    NPC.ai[3] = 1;
                                else
                                    break;
                            }
                            if (Phase2Check())
                                break;
                            NPC.velocity = Vector2.Zero;
                            if (++NPC.ai[1] > 2)
                            {
                                Terraria.Audio.SoundEngine.PlaySound(SoundID.Item12, NPC.Center);
                                NPC.ai[1] = 0;
                                NPC.ai[2] += (float)Math.PI / 77f;
                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                    for (int i = 0; i < 4; i++)
                                        Projectile.NewProjectile(NPC.GetSpawnSource_ForProjectile(), NPC.Center, new Vector2(6f, 0).RotatedBy(NPC.ai[2] + Math.PI / 2 * i), ModContent.ProjectileType<MutantEye>(), NPC.damage / 4, 0f, Main.myPlayer);
                            }
                            if (++NPC.ai[3] > 241)
                            {
                                //NPC.TargetClosest();
                                NPC.localAI[2]++;
                                NPC.ai[1] = 0;
                                NPC.ai[2] = 0;
                                NPC.ai[3] = 0;
                                NPC.netUpdate = true;
                            }
                            break;

                        case 1: //spawn sword
                            if (Main.LocalPlayer.active && NPC.Distance(Main.LocalPlayer.Center) < 3000f && Main.expertMode)
                                Main.LocalPlayer.AddBuff(ModContent.BuffType<MutantPresence>(), 2);
                            NPC.velocity = Vector2.Zero;
                            if (NPC.ai[2] == 0 && Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                double angle = NPC.position.X < player.position.X ? -Math.PI / 4 : Math.PI / 4;
                                NPC.ai[2] = (float)angle * -4f / 30;
                                const int spacing = 80;
                                Vector2 offset = Vector2.UnitY.RotatedBy(angle) * -spacing;
                                for (int i = 0; i < 12; i++)
                                    Projectile.NewProjectile(NPC.GetSpawnSource_ForProjectile(), NPC.Center + offset * i, Vector2.Zero, ModContent.ProjectileType<MutantSword>(), NPC.damage / 3, 0f, Main.myPlayer, NPC.whoAmI, spacing * i);
                                Projectile.NewProjectile(NPC.GetSpawnSource_ForProjectile(), NPC.Center + offset.RotatedBy(MathHelper.ToRadians(20)) * 7, Vector2.Zero, ModContent.ProjectileType<MutantSword>(), NPC.damage / 3, 0f, Main.myPlayer, NPC.whoAmI, 60 * 4);
                                Projectile.NewProjectile(NPC.GetSpawnSource_ForProjectile(), NPC.Center + offset.RotatedBy(MathHelper.ToRadians(-20)) * 7, Vector2.Zero, ModContent.ProjectileType<MutantSword>(), NPC.damage / 3, 0f, Main.myPlayer, NPC.whoAmI, 60 * 4);
                                Projectile.NewProjectile(NPC.GetSpawnSource_ForProjectile(), NPC.Center + offset.RotatedBy(MathHelper.ToRadians(40)) * 28, Vector2.Zero, ModContent.ProjectileType<MutantSword>(), NPC.damage / 3, 0f, Main.myPlayer, NPC.whoAmI, 60 * 4);
                                Projectile.NewProjectile(NPC.GetSpawnSource_ForProjectile(), NPC.Center + offset.RotatedBy(MathHelper.ToRadians(-40)) * 28, Vector2.Zero, ModContent.ProjectileType<MutantSword>(), NPC.damage / 3, 0f, Main.myPlayer, NPC.whoAmI, 60 * 4);
                            }
                            if (++NPC.ai[1] > 120)
                            {
                                targetPos = player.Center;
                                targetPos.X -= 300 * NPC.ai[2];
                                NPC.velocity = (targetPos - NPC.Center) / 30;

                                NPC.localAI[2]++;
                                NPC.ai[1] = 0;
                                NPC.netUpdate = true;
                            }

                            NPC.direction = NPC.spriteDirection = Math.Sign(NPC.ai[2]);
                            break;

                        case 2: //swinging sword dash
                            if (Main.LocalPlayer.active && NPC.Distance(Main.LocalPlayer.Center) < 3000f && Main.expertMode)
                                Main.LocalPlayer.AddBuff(ModContent.BuffType<MutantPresence>(), 2);

                            NPC.ai[3] += NPC.ai[2];
                            NPC.direction = NPC.spriteDirection = Math.Sign(NPC.ai[2]);

                            if (++NPC.ai[1] > 35)
                            {
                                if (!Main.dedServ && Main.LocalPlayer.active)
                                    Main.LocalPlayer.GetModPlayer<FargoSoulsPlayer>().Screenshake = 30;

                                NPC.ai[0] = 0;
                                NPC.ai[1] = 0;
                                NPC.ai[2] = 0;
                                NPC.ai[3] = 0;
                                NPC.localAI[2] = 0;
                                NPC.netUpdate = true;
                            }
                            break;

                        default:
                            break;
                    }
                    break;

                case 10: //phase 2 begins
                    NPC.velocity *= 0.9f;
                    NPC.dontTakeDamage = true;

                    if (NPC.buffType[0] != 0)
                        NPC.DelBuff(0);
                    
                    if (FargoSoulsWorld.EternityMode)
                    {
                        if (SoulConfig.Instance.MutantMusicIsRePrologue && ModLoader.TryGetMod("FargowiltasMusic", out Mod musicMod))
                            Music = MusicLoader.GetMusicSlot(musicMod, "Assets/Music/rePrologue");

                        if (!SkyManager.Instance["FargowiltasSouls:MutantBoss"].IsActive())
                            SkyManager.Instance.Activate("FargowiltasSouls:MutantBoss");
                    }

                    if (NPC.ai[2] == 0)
                    {
                        if (NPC.ai[1] < 60 && !Main.dedServ && Main.LocalPlayer.active)
                            Main.LocalPlayer.GetModPlayer<FargoSoulsPlayer>().Screenshake = 2;
                    }
                    else
                    {
                        NPC.velocity = Vector2.Zero;
                    }

                    if (NPC.ai[1] < 240)
                    {
                        //make you stop attacking
                        if (Main.LocalPlayer.active && !Main.LocalPlayer.dead && !Main.LocalPlayer.ghost && NPC.Distance(Main.LocalPlayer.Center) < 3000)
                        {
                            Main.LocalPlayer.itemTime = 0;
                            Main.LocalPlayer.itemAnimation = 0;
                            Main.LocalPlayer.reuseDelay = 2;
                        }
                    }

                    if (NPC.ai[1] == 0)
                    {
                        FargoSoulsUtil.ClearAllProjectiles(2, NPC.whoAmI);

                        if (FargoSoulsWorld.EternityMode)
                        {
                            DramaticTransition(false, NPC.ai[2] == 0);

                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                ritualProj = Projectile.NewProjectile(NPC.GetSpawnSource_ForProjectile(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<MutantRitual>(), NPC.damage / 4, 0f, Main.myPlayer, 0f, NPC.whoAmI);

                                if (FargoSoulsWorld.MasochistModeReal)
                                {
                                    Projectile.NewProjectile(NPC.GetSpawnSource_ForProjectile(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<MutantRitual2>(), 0, 0f, Main.myPlayer, 0f, NPC.whoAmI);
                                    Projectile.NewProjectile(NPC.GetSpawnSource_ForProjectile(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<MutantRitual3>(), 0, 0f, Main.myPlayer, 0f, NPC.whoAmI);
                                    Projectile.NewProjectile(NPC.GetSpawnSource_ForProjectile(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<MutantRitual4>(), 0, 0f, Main.myPlayer, 0f, NPC.whoAmI);
                                }
                            }
                        }
                    }
                    else if (NPC.ai[1] == 150)
                    {
                        NPC.life = NPC.lifeMax / 2; //for the background effect LOLE

                        Terraria.Audio.SoundEngine.PlaySound(SoundID.Roar, (int)NPC.Center.X, (int)NPC.Center.Y, 0);

                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            Projectile.NewProjectile(NPC.GetSpawnSource_ForProjectile(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<Projectiles.GlowRingHollow>(), 0, 0f, Main.myPlayer, 5);
                            //Projectile.NewProjectile(NPC.GetSpawnSource_ForProjectile(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<Projectiles.GlowRing>(), 0, 0f, Main.myPlayer, NPC.whoAmI, -22);
                        }

                        if (FargoSoulsWorld.EternityMode && FargoSoulsWorld.skipMutantP1 <= 10)
                        {
                            FargoSoulsWorld.skipMutantP1++;
                            if (Main.netMode == NetmodeID.Server)
                                NetMessage.SendData(MessageID.WorldData);
                        }

                        for (int i = 0; i < 50; i++)
                        {
                            int d = Dust.NewDust(Main.LocalPlayer.position, Main.LocalPlayer.width, Main.LocalPlayer.height, 229, 0f, 0f, 0, default(Color), 2.5f);
                            Main.dust[d].noGravity = true;
                            Main.dust[d].noLight = true;
                            Main.dust[d].velocity *= 9f;
                        }
                        //if (player.GetModPlayer<FargoSoulsPlayer>().TerrariaSoul) EdgyBossText("Hand it over. That thing, your soul toggles.");
                    }
                    else if (NPC.ai[1] > 150)
                    {
                        //Main.dayTime = false; //for empress

                        /*for (int i = 0; i < 5; i++)
                        {
                            int d = Dust.NewDust(NPC.position, NPC.width, NPC.height, 229, 0f, 0f, 0, default(Color), 1.5f);
                            Main.dust[d].noGravity = true;
                            Main.dust[d].noLight = true;
                            Main.dust[d].velocity *= 4f;
                        }*/

                        //NPC.localAI[3] = 2;
                        NPC.localAI[3] = 3;

                        /*if (FargoSoulsWorld.MasochistMode)
                        {
                            int heal = (int)(NPC.lifeMax / 120 * Main.rand.NextFloat(1f, 1.5f));
                            NPC.life += heal;
                            if (NPC.life > NPC.lifeMax)
                                NPC.life = NPC.lifeMax;
                            CombatText.NewText(NPC.Hitbox, CombatText.HealLife, heal);
                        }*/
                    }

                    if (++NPC.ai[1] > 270)
                    {
                        if (FargoSoulsWorld.EternityMode)
                        {
                            NPC.life = NPC.lifeMax;
                            NPC.ai[0] = Main.rand.Next(new int[] { 11, 13, 16, 18, 20, 21, 24, 26, 29, 35, 37, 39, 42 }); //force a random choice
                        }
                        else
                        {
                            NPC.ai[0]++;
                        }
                        NPC.ai[1] = 0;
                        NPC.ai[2] = 0;
                        //NPC.TargetClosest();
                        NPC.netUpdate = true;

                        attackHistory.Enqueue(NPC.ai[0]);
                    }
                    break;

                case 11: //approach for laser
                    if (!AliveCheck(player))
                        break;
                    targetPos = player.Center + player.DirectionTo(NPC.Center) * 300;
                    if (NPC.Distance(targetPos) > 50 && ++NPC.ai[2] < 180)
                    {
                        Movement(targetPos, FargoSoulsWorld.MasochistModeReal ? 2.4f : 0.8f);
                    }
                    else
                    {
                        NPC.netUpdate = true;
                        NPC.ai[0]++;
                        NPC.ai[1] = 0;
                        NPC.ai[2] = player.DirectionTo(NPC.Center).ToRotation();
                        NPC.ai[3] = (float)Math.PI / 10f;
                        NPC.localAI[0] = 0;
                        Terraria.Audio.SoundEngine.PlaySound(SoundID.Roar, NPC.Center, 0);
                        if (player.Center.X < NPC.Center.X)
                            NPC.ai[3] *= -1;
                    }
                    break;

                case 12: //fire lasers in ring
                    NPC.velocity = Vector2.Zero;
                    if (--NPC.ai[1] < 0)
                    {
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                            Projectile.NewProjectile(NPC.GetSpawnSource_ForProjectile(), NPC.Center, new Vector2(2, 0).RotatedBy(NPC.ai[2]), ModContent.ProjectileType<MutantMark1>(), NPC.damage / 4, 0f, Main.myPlayer);
                        NPC.ai[1] = 3;
                        NPC.ai[2] += NPC.ai[3];
                        if (NPC.localAI[0]++ == 20 || NPC.localAI[0] == 40)
                        {
                            NPC.netUpdate = true;
                            NPC.ai[2] -= NPC.ai[3] / 2;
                        }
                        else if (NPC.localAI[0] >= 60)
                        {
                            ChooseNextAttack(13, 18, 21, 24, 31, 35, 39, 41, 42);
                        }
                    }
                    break;

                case 13: //maneuvering under player while spinning penetrator
                    if (NPC.ai[3] == 0)
                    {
                        if (!AliveCheck(player))
                            break;
                        NPC.ai[3] = 1;
                        //NPC.velocity = NPC.DirectionFrom(player.Center) * NPC.velocity.Length();
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                            Projectile.NewProjectile(NPC.GetSpawnSource_ForProjectile(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<MutantSpearSpin>(), NPC.damage / 4, 0f, Main.myPlayer, NPC.whoAmI, 180); // + 60);
                    }
                    
                    if (++NPC.ai[1] > 180)
                    {
                        if (!AliveCheck(player))
                            break;
                        NPC.netUpdate = true;
                        NPC.ai[0]++;
                        NPC.ai[1] = 0;
                        NPC.ai[3] = 0;
                        //NPC.TargetClosest();
                    }

                    targetPos = player.Center;
                    targetPos.Y += 400f * Math.Sign(NPC.Center.Y - player.Center.Y); //can be above or below
                    Movement(targetPos, 0.7f, false);
                    if (NPC.Distance(player.Center) < 200)
                        Movement(NPC.Center + NPC.DirectionFrom(player.Center), 1.4f);
                    break;

                case 14: //pause and then initiate dash
                    if (NPC.localAI[1] == 0)
                        NPC.localAI[1] = FargoSoulsWorld.EternityMode ? Main.rand.Next(5, 8) : 5; //random max number of attacks

                    if (NPC.ai[1] == 0) //telegraph
                    {
                        if (!AliveCheck(player))
                            return;

                        if (NPC.ai[2] == NPC.localAI[1] - 1 && NPC.Distance(player.Center) > 450)
                        {
                            Movement(player.Center, 0.6f);
                            return;
                        }

                        if (NPC.ai[2] < NPC.localAI[1])
                        {
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                                Projectile.NewProjectile(NPC.GetSpawnSource_ForProjectile(), NPC.Center, NPC.DirectionTo(player.Center + player.velocity * 30f), ModContent.ProjectileType<MutantDeathrayAim>(), 0, 0f, Main.myPlayer, 55, NPC.whoAmI);

                            if (NPC.ai[2] == NPC.localAI[1] - 1)
                            {
                                Terraria.Audio.SoundEngine.PlaySound(SoundID.Roar, (int)NPC.Center.X, (int)NPC.Center.Y, 0);

                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                    Projectile.NewProjectile(NPC.GetSpawnSource_ForProjectile(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<MutantSpearAim>(), NPC.damage / 4, 0f, Main.myPlayer, NPC.whoAmI, 4);
                            }
                        }
                    }

                    NPC.velocity *= 0.9f;

                    if (NPC.ai[1] < 55) //track player up until just before dash
                    {
                        NPC.localAI[0] = NPC.DirectionTo(player.Center + player.velocity * 30f).ToRotation();
                    }

                    if (++NPC.ai[1] > (NPC.ai[2] == NPC.localAI[1] - 1 ? 80 : 60))
                    {
                        NPC.netUpdate = true;
                        NPC.ai[0]++;
                        NPC.ai[1] = 0;
                        NPC.ai[3] = 0;
                        if (++NPC.ai[2] > NPC.localAI[1])
                        {
                            ChooseNextAttack(16, 18, 20, 26, 29, 31, 33, 39, 42, 44);
                        }
                        else
                        {
                            NPC.velocity = NPC.localAI[0].ToRotationVector2() * 45f;
                            float spearAi = 1f;
                            if (NPC.ai[2] == NPC.localAI[1])
                                spearAi = -2f;

                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                Projectile.NewProjectile(NPC.GetSpawnSource_ForProjectile(), NPC.Center, Vector2.Normalize(NPC.velocity), ModContent.ProjectileType<MutantDeathray2>(), NPC.damage / 5, 0f, Main.myPlayer);
                                Projectile.NewProjectile(NPC.GetSpawnSource_ForProjectile(), NPC.Center, -Vector2.Normalize(NPC.velocity), ModContent.ProjectileType<MutantDeathray2>(), NPC.damage / 5, 0f, Main.myPlayer);
                                Projectile.NewProjectile(NPC.GetSpawnSource_ForProjectile(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<MutantSpearDash>(), NPC.damage / 4, 0f, Main.myPlayer, NPC.whoAmI, spearAi);
                            }
                        }
                        NPC.localAI[0] = 0;
                    }
                    break;

                case 15: //while dashing
                    NPC.direction = NPC.spriteDirection = Math.Sign(NPC.velocity.X);
                    if (++NPC.ai[1] > 30)
                    {
                        if (!AliveCheck(player))
                            break;
                        NPC.netUpdate = true;
                        NPC.ai[0]--;
                        NPC.ai[1] = 0;

                        if (NPC.ai[0] == 14 && NPC.ai[2] == NPC.localAI[1] - 1 && NPC.Distance(player.Center) > 450)
                            NPC.velocity = NPC.DirectionTo(player.Center) * 16f;
                    }
                    break;

                case 16: //approach for bullet hell
                    goto case 11;

                case 17: //BOUNDARY OF WAVE AND PARTICLE
                    NPC.velocity = Vector2.Zero;
                    if (NPC.localAI[0] == 0)
                    {
                        NPC.localAI[0] = Math.Sign(NPC.Center.X - player.Center.X);
                        //if (FargoSoulsWorld.MasochistMode) NPC.ai[2] = NPC.DirectionTo(player.Center).ToRotation(); //starting rotation offset to avoid hitting at close range
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                            Projectile.NewProjectile(NPC.GetSpawnSource_ForProjectile(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<Projectiles.GlowRing>(), 0, 0f, Main.myPlayer, NPC.whoAmI, -2);
                    }
                    if (NPC.ai[3] > 60 && ++NPC.ai[1] > 2)
                    {
                        Terraria.Audio.SoundEngine.PlaySound(SoundID.Item12, NPC.Center);
                        NPC.ai[1] = 0;
                        NPC.ai[2] += (float)Math.PI / 8 / 480 * NPC.ai[3] * NPC.localAI[0];
                        if (NPC.ai[2] > (float)Math.PI)
                            NPC.ai[2] -= (float)Math.PI * 2;
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            int max = 4;
                            if (FargoSoulsWorld.EternityMode)
                                max++;
                            if (FargoSoulsWorld.MasochistModeReal)
                                max++;
                            for (int i = 0; i < max; i++)
                            {
                                Projectile.NewProjectile(NPC.GetSpawnSource_ForProjectile(), NPC.Center, new Vector2(0f, -6f).RotatedBy(NPC.ai[2] + Math.PI * 2 / max * i),
                                      ModContent.ProjectileType<MutantEye>(), NPC.damage / 4, 0f, Main.myPlayer);
                            }
                        }
                    }
                    if (++NPC.ai[3] > 360 + 60)
                    {
                        ChooseNextAttack(13, 18, 20, 26, 33, 41);
                    }
                    break;

                case 18: //spawn illusions for next attack
                    Terraria.Audio.SoundEngine.PlaySound(SoundID.Roar, (int)NPC.Center.X, (int)NPC.Center.Y, 0);
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        int n = NPC.NewNPC(NPC.GetSpawnSourceForNPCFromNPCAI(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<MutantIllusion>(), NPC.whoAmI, NPC.whoAmI, -1, 1, 240);
                        if (n != Main.maxNPCs && Main.netMode == NetmodeID.Server)
                            NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, n);
                        n = NPC.NewNPC(NPC.GetSpawnSourceForNPCFromNPCAI(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<MutantIllusion>(), NPC.whoAmI, NPC.whoAmI, 1, -1, 120);
                        if (n != Main.maxNPCs && Main.netMode == NetmodeID.Server)
                            NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, n);
                        n = NPC.NewNPC(NPC.GetSpawnSourceForNPCFromNPCAI(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<MutantIllusion>(), NPC.whoAmI, NPC.whoAmI, 1, 1, 180);
                        if (n != Main.maxNPCs && Main.netMode == NetmodeID.Server)
                            NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, n);
                    }
                    NPC.ai[0]++;
                    break;

                case 19: //QUADRUPLE PILLAR ROAD ROLLER
                    if (!AliveCheck(player))
                        break;

                    if (NPC.ai[2] == 0 && NPC.ai[3] == 0) //target one corner of arena
                    {
                        NPC.netUpdate = true;
                        NPC.ai[2] = NPC.Center.X;
                        NPC.ai[3] = NPC.Center.Y;
                        for (int i = 0; i < Main.maxProjectiles; i++)
                        {
                            if (Main.projectile[i].active && Main.projectile[i].type == ModContent.ProjectileType<MutantRitual>() && Main.projectile[i].ai[1] == NPC.whoAmI)
                            {
                                NPC.ai[2] = Main.projectile[i].Center.X;
                                NPC.ai[3] = Main.projectile[i].Center.Y;
                                break;
                            }
                        }

                        Vector2 offset = 1000f * Vector2.UnitX.RotatedBy(MathHelper.ToRadians(45));
                        if (Main.rand.NextBool()) //always go to a side player isn't in but pick a way to do it randomly
                        {
                            if (player.Center.X > NPC.ai[2])
                                offset.X *= -1;
                            if (Main.rand.NextBool())
                                offset.Y *= -1;
                        }
                        else
                        {
                            if (Main.rand.NextBool())
                                offset.X *= -1;
                            if (player.Center.Y > NPC.ai[3])
                                offset.Y *= -1;
                        }

                        NPC.localAI[1] = NPC.ai[2]; //for illusions
                        NPC.localAI[2] = NPC.ai[3];

                        NPC.ai[2] = offset.Length();
                        NPC.ai[3] = offset.ToRotation();
                    }

                    /*if (NPC.ai[1] < 360)
                    {
                        targetPos = new Vector2(NPC.localAI[1], NPC.localAI[2]);

                        Vector2 offset = NPC.ai[3].ToRotationVector2();
                        offset *= Math.Min(NPC.ai[2], (targetPos - player.Center).Length() + 500); //do this to not go totally offscreen asap.
                        targetPos += offset;
                    }
                    else
                    {
                        targetPos = player.Center + NPC.DirectionFrom(player.Center) * 450;
                    }*/

                    targetPos = player.Center;
                    targetPos.X += NPC.Center.X < player.Center.X ? -700 : 700;
                    targetPos.Y += NPC.ai[1] < 240 ? 400 : 150;

                    if (NPC.Distance(targetPos) > 50)
                        Movement(targetPos, 1f);
                    
                    if (++NPC.ai[1] > 420)
                    {
                        if (FargoSoulsWorld.MasochistModeReal)
                            ChooseNextAttack(11, 16, 33, 42, 44);
                        else
                            ChooseNextAttack(11, 13, 20, 21, 26, 33, 41);
                    }
                    else if (NPC.ai[1] == 60)
                    {
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                            Projectile.NewProjectile(NPC.GetSpawnSource_ForProjectile(), NPC.Center, Vector2.UnitY * -5, ModContent.ProjectileType<MutantPillar>(), NPC.damage / 3, 0, Main.myPlayer, 3, NPC.whoAmI);
                    }
                    break;

                case 20: //eoc bullet hell, was blood sickle mines
                    if (!AliveCheck(player))
                        break;

                    if (NPC.ai[1] == 0)
                    {
                        float ai1 = 0;
                        
                        if (FargoSoulsWorld.MasochistModeReal) //begin attack much faster
                        {
                            ai1 = 60;
                            NPC.ai[1] = 60;
                        }

                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            int p = Projectile.NewProjectile(NPC.GetSpawnSource_ForProjectile(), NPC.Center, -Vector2.UnitY, ModContent.ProjectileType<MutantEyeOfCthulhu>(), NPC.damage / 4, 0f, Main.myPlayer, NPC.target, ai1);
                            if (p != Main.maxProjectiles)
                                Main.projectile[p].timeLeft -= 60;
                        }
                    }

                    if (NPC.ai[1] < 120) //stop tracking when eoc begins attacking, this locks arena in place
                    {
                        NPC.ai[2] = player.Center.X;
                        NPC.ai[3] = player.Center.Y;
                    }

                    /*if (NPC.Distance(player.Center) < 200)
                    {
                        Movement(NPC.Center + 200 * NPC.DirectionFrom(player.Center), 0.9f);
                    }
                    else
                    {*/
                    targetPos = new Vector2(NPC.ai[2], NPC.ai[3]);
                    targetPos += NPC.DirectionFrom(targetPos).RotatedBy(MathHelper.ToRadians(-5)) * 450f;
                    if (NPC.Distance(targetPos) > 50)
                        Movement(targetPos, 0.25f);
                    //}

                    if (++NPC.ai[1] > 450)
                    {
                        ChooseNextAttack(11, 13, 16, 21, 26, 29, 31, 33, 35, 37, 41, 44);
                    }

                    /*if (Math.Abs(targetPos.X - player.Center.X) < 150) //avoid crossing up player
                    {
                        targetPos.X = player.Center.X + 150 * Math.Sign(targetPos.X - player.Center.X);
                        Movement(targetPos, 0.3f);
                    }
                    if (NPC.Distance(targetPos) > 50)
                    {
                        Movement(targetPos, 0.5f);
                    }

                    if (--NPC.ai[1] < 0)
                    {
                        NPC.ai[1] = 60;
                        if (++NPC.ai[2] > (FargoSoulsWorld.MasochistMode ? 3 : 1))
                        {
                            //float[] options = { 13, 18, 21, 24, 26, 31, 33, 40 }; NPC.ai[0] = options[Main.rand.Next(options.Length)];
                            NPC.ai[0]++;
                            NPC.ai[2] = 0;
                            NPC.TargetClosest();
                        }
                        else
                        {
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                                for (int i = 0; i < 8; i++)
                                    Projectile.NewProjectile(NPC.GetSpawnSource_ForProjectile(), NPC.Center, Vector2.UnitX.RotatedBy(Math.PI / 4 * i) * 10f, ModContent.ProjectileType<MutantScythe1>(), NPC.damage / 5, 0f, Main.myPlayer, NPC.whoAmI);
                            Terraria.Audio.SoundEngine.PlaySound(SoundID.ForceRoar, (int)NPC.Center.X, (int)NPC.Center.Y, -1, 1f, 0);
                        }
                        NPC.netUpdate = true;
                        break;
                    }*/
                    break;

                case 21: //maneuver above while spinning penetrator
                    if (NPC.ai[3] == 0)
                    {
                        if (!AliveCheck(player))
                            break;
                        NPC.ai[3] = 1;
                        //NPC.velocity = NPC.DirectionFrom(player.Center) * NPC.velocity.Length();
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                            Projectile.NewProjectile(NPC.GetSpawnSource_ForProjectile(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<MutantSpearSpin>(), NPC.damage / 4, 0f, Main.myPlayer, NPC.whoAmI, 180);// + (FargoSoulsWorld.MasochistMode ? 10 : 20));
                    }
                    
                    if (++NPC.ai[1] > 180)
                    {
                        if (!AliveCheck(player))
                            break;
                        NPC.netUpdate = true;
                        NPC.ai[0]++;
                        NPC.ai[1] = 0;
                        NPC.ai[3] = 0;
                        //NPC.TargetClosest();
                    }

                    targetPos = player.Center;
                    targetPos.Y += 450f * Math.Sign(NPC.Center.Y - player.Center.Y); //can be above or below
                    Movement(targetPos, 0.7f, false);
                    if (NPC.Distance(player.Center) < 200)
                        Movement(NPC.Center + NPC.DirectionFrom(player.Center), 1.4f);
                    break;

                case 22: //pause and then initiate dash
                    NPC.velocity *= 0.9f;

                    if (NPC.localAI[1] == 0)
                        NPC.localAI[1] = FargoSoulsWorld.EternityMode ? Main.rand.Next(5, 9) : 5; //random max number of attacks

                    if (++NPC.ai[1] > (FargoSoulsWorld.EternityMode ? 5 : 20))
                    {
                        NPC.netUpdate = true;
                        NPC.ai[0]++;
                        NPC.ai[1] = 0;
                        if (++NPC.ai[2] > NPC.localAI[1])
                        {
                            ChooseNextAttack(11, 16, 31, 35, 37, 39, 42, 44);
                        }
                        else
                        {
                            NPC.velocity = NPC.DirectionTo(player.Center) * 45f;
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                Projectile.NewProjectile(NPC.GetSpawnSource_ForProjectile(), NPC.Center, Vector2.Normalize(NPC.velocity), ModContent.ProjectileType<MutantDeathray2>(), NPC.damage / 5, 0f, Main.myPlayer);
                                Projectile.NewProjectile(NPC.GetSpawnSource_ForProjectile(), NPC.Center, -Vector2.Normalize(NPC.velocity), ModContent.ProjectileType<MutantDeathray2>(), NPC.damage / 5, 0f, Main.myPlayer);
                                Projectile.NewProjectile(NPC.GetSpawnSource_ForProjectile(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<MutantSpearDash>(), NPC.damage / 4, 0f, Main.myPlayer, NPC.whoAmI);
                            }
                        }
                    }
                    break;

                case 23: //while dashing
                    if (NPC.ai[1] % 3 == 0)
                        NPC.ai[1]++;
                    goto case 15;

                case 24: //destroyers
                    if (!AliveCheck(player))
                        break;

                    if (FargoSoulsWorld.EternityMode)
                    {
                        targetPos = player.Center + NPC.DirectionFrom(player.Center) * 300;
                        if (Math.Abs(targetPos.X - player.Center.X) < 150) //avoid crossing up player
                        {
                            targetPos.X = player.Center.X + 150 * Math.Sign(targetPos.X - player.Center.X);
                            Movement(targetPos, 0.3f);
                        }
                        if (NPC.Distance(targetPos) > 50)
                        {
                            Movement(targetPos, 0.9f);
                        }
                    }
                    else
                    {
                        targetPos = player.Center;
                        targetPos.X += 500 * (NPC.Center.X < targetPos.X ? -1 : 1);
                        if (NPC.Distance(targetPos) > 50)
                        {
                            Movement(targetPos, 0.4f);
                        }
                    }

                    if (NPC.localAI[1] == 0)
                        NPC.localAI[1] = FargoSoulsWorld.EternityMode ? Main.rand.Next(5, 9) : 5; //random max number of attacks, YES this carries into next state

                    if (++NPC.ai[1] > 60)
                    {
                        NPC.netUpdate = true;
                        NPC.ai[1] = 30;
                        int cap = 3;
                        if (FargoSoulsWorld.EternityMode)
                        {
                            cap += 2;
                        }
                        if (FargoSoulsWorld.MasochistModeReal)
                        {
                            cap += 2;
                            NPC.ai[1] += 15; //faster
                        }

                        if (++NPC.ai[2] > cap)
                        {
                            //NPC.TargetClosest();
                            NPC.ai[0]++;
                            NPC.ai[1] = 0;
                            NPC.ai[2] = 0;
                        }
                        else
                        {
                            Terraria.Audio.SoundEngine.PlaySound(SoundID.NPCKilled, (int)NPC.Center.X, (int)NPC.Center.Y, 13);
                            if (Main.netMode != NetmodeID.MultiplayerClient) //spawn worm
                            {
                                Vector2 vel = NPC.DirectionFrom(player.Center).RotatedByRandom(MathHelper.ToRadians(120)) * 10f;
                                float ai1 = 0.8f + 0.4f * NPC.ai[2] / 5f;
                                int current = Projectile.NewProjectile(NPC.GetSpawnSource_ForProjectile(), NPC.Center, vel, ModContent.ProjectileType<MutantDestroyerHead>(), NPC.damage / 4, 0f, Main.myPlayer, NPC.target, ai1);
                                //timeleft: remaining duration of this case + duration of next case + extra delay after + successive death
                                Main.projectile[current].timeLeft = 30 * (cap - (int)NPC.ai[2]) + 60 * (int)NPC.localAI[1] + 30 + (int)NPC.ai[2] * 6;
                                for (int i = 0; i < 18; i++)
                                    current = Projectile.NewProjectile(NPC.GetSpawnSource_ForProjectile(), NPC.Center, vel, ModContent.ProjectileType<MutantDestroyerBody>(), NPC.damage / 4, 0f, Main.myPlayer, Main.projectile[current].identity);
                                int previous = current;
                                current = Projectile.NewProjectile(NPC.GetSpawnSource_ForProjectile(), NPC.Center, vel, ModContent.ProjectileType<MutantDestroyerTail>(), NPC.damage / 4, 0f, Main.myPlayer, Main.projectile[current].identity);
                                Main.projectile[previous].localAI[1] = Main.projectile[current].identity;
                                Main.projectile[previous].netUpdate = true;
                            }
                        }
                    }
                    break;

                case 25: //improved throw
                    if (!AliveCheck(player))
                        break;

                    targetPos = player.Center;
                    targetPos.X += 500 * (NPC.Center.X < targetPos.X ? -1 : 1);
                    if (NPC.Distance(targetPos) > 25)
                        Movement(targetPos, 0.8f);

                    if (++NPC.ai[1] > 60)
                    {
                        NPC.netUpdate = true;
                        NPC.ai[1] = 0;
                        if (++NPC.ai[2] > NPC.localAI[1])
                        {
                            ChooseNextAttack(11, 18, 20, 26, 26, 26, 29, 31, 33, 35, 37, 39, 42, 44);
                        }
                        else if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            Vector2 vel = NPC.DirectionTo(player.Center + player.velocity * 30f) * 30f;
                            Projectile.NewProjectile(NPC.GetSpawnSource_ForProjectile(), NPC.Center, Vector2.Normalize(vel), ModContent.ProjectileType<MutantDeathray2>(), NPC.damage / 5, 0f, Main.myPlayer);
                            Projectile.NewProjectile(NPC.GetSpawnSource_ForProjectile(), NPC.Center, -Vector2.Normalize(vel), ModContent.ProjectileType<MutantDeathray2>(), NPC.damage / 5, 0f, Main.myPlayer);
                            Projectile.NewProjectile(NPC.GetSpawnSource_ForProjectile(), NPC.Center, vel, ModContent.ProjectileType<MutantSpearThrown>(), NPC.damage / 4, 0f, Main.myPlayer, NPC.target, 1f);
                        }
                    }
                    else if (NPC.ai[1] == 1 && NPC.ai[2] < NPC.localAI[1] && Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Projectile.NewProjectile(NPC.GetSpawnSource_ForProjectile(), NPC.Center, NPC.DirectionTo(player.Center + player.velocity * 30f), ModContent.ProjectileType<MutantDeathrayAim>(), 0, 0f, Main.myPlayer, 60f, NPC.whoAmI);
                        Projectile.NewProjectile(NPC.GetSpawnSource_ForProjectile(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<MutantSpearAim>(), NPC.damage / 4, 0f, Main.myPlayer, NPC.whoAmI, 2);
                    }
                    break;

                case 26: //back away, prepare for ultra laser spam
                    if (!AliveCheck(player))
                        break;

                    if (NPC.ai[1] == 30 && !FargoSoulsWorld.MasochistModeReal)
                    {
                        Terraria.Audio.SoundEngine.PlaySound(SoundID.ForceRoar, NPC.Center, -1); //eoc roar
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                            Projectile.NewProjectile(NPC.GetSpawnSource_ForProjectile(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<Projectiles.GlowRing>(), 0, 0f, Main.myPlayer, NPC.whoAmI, NPCID.Retinazer);
                    }

                    if (NPC.ai[1] < 30)
                    {
                        targetPos = player.Center + NPC.DirectionFrom(player.Center).RotatedBy(MathHelper.ToRadians(15)) * 500f;
                        if (NPC.Distance(targetPos) > 50)
                            Movement(targetPos, 0.3f);
                    }
                    else
                    {
                        for (int i = 0; i < 3; i++)
                        {
                            int d = Dust.NewDust(NPC.Center, 0, 0, DustID.Torch, 0f, 0f, 0, default(Color), 3f);
                            Main.dust[d].noGravity = true;
                            Main.dust[d].noLight = true;
                            Main.dust[d].velocity *= 12f;
                        }

                        targetPos = player.Center;
                        targetPos.X += 600 * (NPC.Center.X < targetPos.X ? -1 : 1);
                        Movement(targetPos, 1.2f, false);
                    }

                    if (++NPC.ai[1] > 150 || (FargoSoulsWorld.MasochistModeReal && NPC.ai[1] > 30 && NPC.Distance(targetPos) < 16))
                    {
                        NPC.netUpdate = true;
                        NPC.ai[0]++;
                        NPC.ai[1] = 0;
                        NPC.ai[2] = 0;
                        NPC.ai[3] = 0;
                        Terraria.Audio.SoundEngine.PlaySound(SoundID.Roar, NPC.Center, 0);
                        //NPC.TargetClosest();
                    }
                    break;

                case 27: //reti fan and prime rain
                    NPC.velocity = Vector2.Zero;

                    if (NPC.ai[2] == 0)
                        NPC.ai[2] = Main.rand.NextBool() ? -1 : 1; //randomly aim either up or down

                    if (NPC.ai[3] == 0 && Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        int max = 7;
                        for(int i = 0; i <= max; i++)
                        {
                            Vector2 dir = Vector2.UnitX.RotatedBy(NPC.ai[2] * i * MathHelper.Pi / max) * 6; //rotate initial velocity of telegraphs by 180 degrees depending on velocity of lasers
                            Projectile.NewProjectile(NPC.GetSpawnSource_ForProjectile(), NPC.Center + dir, Vector2.Zero, ModContent.ProjectileType<MutantGlowything>(), 0, 0f, Main.myPlayer, dir.ToRotation(), NPC.whoAmI);
                        }

                        //Vector2 spawnPos = NPC.Center;
                        //spawnPos.Y -= NPC.ai[2] * 600;
                        //for (int i = -3; i <= 3; i++)
                        //{
                        //    Projectile.NewProjectile(NPC.GetSpawnSource_ForProjectile(), spawnPos + Vector2.UnitY * 120 * i, Vector2.Zero,
                        //        ModContent.ProjectileType<MutantReticle2>(), 0, 0f, Main.myPlayer);
                        //}
                    }

                    if (NPC.ai[3] > 60 && ++NPC.ai[1] > 10)
                    {
                        NPC.ai[1] = 0;
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            float rotation = MathHelper.ToRadians(245) * NPC.ai[2] / 80f;

                            Projectile.NewProjectile(NPC.GetSpawnSource_ForProjectile(), NPC.Center, MathHelper.ToRadians(8 * NPC.ai[2]).ToRotationVector2(),
                                ModContent.ProjectileType<MutantDeathray3>(), NPC.damage / 4, 0, Main.myPlayer, rotation, NPC.whoAmI);
                            Projectile.NewProjectile(NPC.GetSpawnSource_ForProjectile(), NPC.Center, -MathHelper.ToRadians(-8 * NPC.ai[2]).ToRotationVector2(),
                                ModContent.ProjectileType<MutantDeathray3>(), NPC.damage / 4, 0, Main.myPlayer, -rotation, NPC.whoAmI);

                            if (FargoSoulsWorld.MasochistModeReal)
                            {
                                Vector2 spawnPos = NPC.Center + NPC.ai[2] * -1200 * Vector2.UnitY;
                                Projectile.NewProjectile(NPC.GetSpawnSource_ForProjectile(), spawnPos, MathHelper.ToRadians(-8 * NPC.ai[2]).ToRotationVector2(),
                                ModContent.ProjectileType<MutantDeathray3>(), NPC.damage / 4, 0, Main.myPlayer, -rotation, NPC.whoAmI);
                                Projectile.NewProjectile(NPC.GetSpawnSource_ForProjectile(), spawnPos, -MathHelper.ToRadians(8 * NPC.ai[2]).ToRotationVector2(),
                                    ModContent.ProjectileType<MutantDeathray3>(), NPC.damage / 4, 0, Main.myPlayer, rotation, NPC.whoAmI);
                            }
                        }
                    }

                    if (NPC.ai[3] < 180 && ++NPC.localAI[0] > 1)
                    {
                        NPC.localAI[0] = 0;

                        Terraria.Audio.SoundEngine.PlaySound(SoundID.Item21, NPC.Center);

                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            //ai3 check for ending behaviour consistency
                            float spawnOffset = (Main.rand.NextBool() && NPC.ai[3] < 120 ? -1 : 1) * Main.rand.NextFloat(1400, 1800);
                            float maxVariance = MathHelper.ToRadians(7.5f);
                            Vector2 spawnPos = NPC.Center + spawnOffset * Vector2.UnitY.RotatedByRandom(maxVariance);
                            Vector2 vel = 32f * NPC.DirectionFrom(spawnPos);
                            Projectile.NewProjectile(NPC.GetSpawnSource_ForProjectile(), spawnPos, vel, ModContent.ProjectileType<MutantGuardian>(), NPC.damage / 3, 0f, Main.myPlayer);
                        }
                    }

                    if (++NPC.ai[3] > 60 + 180)
                    {
                        NPC.ai[0]++;
                        NPC.ai[1] = 0;
                        NPC.ai[2] = 0;
                        NPC.ai[3] = 0;
                        NPC.localAI[0] = 0;
                        NPC.localAI[1] = 0;
                        NPC.netUpdate = true;
                        //NPC.TargetClosest();
                    }
                    break;

                case 28: //on standby, wait for previous attack to clear
                    if (++NPC.ai[3] > 270)
                    {
                        if (FargoSoulsWorld.EternityMode && NPC.localAI[3] > 2) //use full moveset
                        {
                            ChooseNextAttack(11, 13, 16, 18, 21, 24, 29, 31, 33, 35, 37, 39, 41, 42);
                        }
                        else
                        {
                            NPC.ai[0] = 11;
                            NPC.ai[1] = 0;
                            NPC.ai[2] = 0;
                            NPC.ai[3] = 0;
                        }
                        NPC.netUpdate = true;
                    }
                    break;

                case 29: //prepare to fishron dive
                    if (!AliveCheck(player))
                        break;
                    targetPos = new Vector2(player.Center.X, player.Center.Y + 600 * Math.Sign(NPC.Center.Y - player.Center.Y));
                    Movement(targetPos, 1.4f, false);
                    if (++NPC.ai[1] > 60 || NPC.Distance(targetPos) < 100) //dive here
                    {
                        NPC.velocity.X = 30f * (NPC.position.X < player.position.X ? 1 : -1);
                        NPC.velocity.Y = 0f;
                        NPC.ai[0]++;
                        NPC.ai[1] = 0;
                        NPC.netUpdate = true;
                    }
                    break;

                case 30: //spawn fishrons
                    {
                        NPC.velocity *= 0.975f;
                        if (NPC.ai[1] == 0)
                        {
                            NPC.ai[2] = Main.rand.NextBool() ? 1 : 0;
                        }
                        const int fishronDelay = 3;
                        if (NPC.ai[1] % fishronDelay == 0 && NPC.ai[1] <= fishronDelay * 2)
                        {
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                for (int j = -1; j <= 1; j += 2) //to both sides of player
                                {
                                    int max = (int)NPC.ai[1] / fishronDelay;
                                    for (int i = -max; i <= max; i++) //fan of fishron
                                    {
                                        if (Math.Abs(i) != max) //only spawn the outmost ones
                                            continue;
                                        Vector2 offset = NPC.ai[2] == 0 ? Vector2.UnitY.RotatedBy(Math.PI / 3 / 3 * i) * -450f * j : Vector2.UnitX.RotatedBy(Math.PI / 3 / 3 * i) * 475f * j;
                                        Projectile.NewProjectile(NPC.GetSpawnSource_ForProjectile(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<MutantFishron>(), NPC.damage / 4, 0f, Main.myPlayer, offset.X, offset.Y);
                                    }
                                }
                            }
                            for (int i = 0; i < 30; i++)
                            {
                                int d = Dust.NewDust(NPC.position, NPC.width, NPC.height, 135, 0f, 0f, 0, default(Color), 3f);
                                Main.dust[d].noGravity = true;
                                Main.dust[d].noLight = true;
                                Main.dust[d].velocity *= 12f;
                            }
                        }
                        if (++NPC.ai[1] > 120)
                        {
                            ChooseNextAttack(13, 18, 20, 21, 26, 31, 31, 31, 33, 35, 39, 41, 42, 44);
                        }
                    }
                    break;

                case 31: //maneuver below for dive
                    if (!AliveCheck(player))
                        break;
                    targetPos = player.Center;
                    targetPos.X += 400 * (NPC.Center.X < targetPos.X ? -1 : 1);
                    targetPos.Y += 400;
                    Movement(targetPos, 1.2f);
                    if (++NPC.ai[1] > 60) //dive here
                    {
                        NPC.velocity.X = 30f * (NPC.position.X < player.position.X ? 1 : -1);
                        if (NPC.velocity.Y > 0)
                            NPC.velocity.Y *= -1;
                        NPC.velocity.Y *= 0.3f;
                        NPC.ai[0]++;
                        NPC.ai[1] = 0;
                        NPC.netUpdate = true;
                    }
                    break;

                case 32: //spawn eyes
                    goto case 3;

                case 33: //toss nuke, set vel
                    if (!AliveCheck(player))
                        break;
                    targetPos = player.Center;
                    targetPos.X += 400 * (NPC.Center.X < targetPos.X ? -1 : 1);
                    targetPos.Y -= 400;
                    Movement(targetPos, 1.2f, false);
                    if (++NPC.ai[1] > 60)
                    {
                        Terraria.Audio.SoundEngine.PlaySound(SoundID.Roar, (int)NPC.Center.X, (int)NPC.Center.Y, 0);
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            float gravity = 0.2f;
                            float time = FargoSoulsWorld.MasochistModeReal ? 120f : 180f;
                            Vector2 distance = player.Center - NPC.Center;
                            distance.X = distance.X / time;
                            distance.Y = distance.Y / time - 0.5f * gravity * time;
                            Projectile.NewProjectile(NPC.GetSpawnSource_ForProjectile(), NPC.Center, distance, ModContent.ProjectileType<MutantNuke>(), FargoSoulsWorld.MasochistModeReal ? NPC.damage / 3 : 0, 0f, Main.myPlayer, gravity);
                            Projectile.NewProjectile(NPC.GetSpawnSource_ForProjectile(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<MutantFishronRitual>(), NPC.damage / 3, 0f, Main.myPlayer, NPC.whoAmI);
                        }
                        NPC.ai[0]++;
                        NPC.ai[1] = 0;
                        if (Math.Sign(player.Center.X - NPC.Center.X) == Math.Sign(NPC.velocity.X))
                            NPC.velocity.X *= -1f;
                        if (NPC.velocity.Y < 0)
                            NPC.velocity.Y *= -1f;
                        NPC.netUpdate = true;
                        //NPC.TargetClosest();
                    }
                    break;

                case 34: //slow drift, protective aura above self
                    if (!AliveCheck(player))
                        break;
                    
                    NPC.velocity.Normalize();
                    NPC.velocity *= 3f;

                    if (NPC.ai[1] > (FargoSoulsWorld.MasochistModeReal ? 120 : 180))
                    {
                        if (!Main.dedServ && Main.LocalPlayer.active)
                            Main.LocalPlayer.GetModPlayer<FargoSoulsPlayer>().Screenshake = 2;

                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            Vector2 safeZone = NPC.Center;
                            safeZone.Y -= 100;
                            const float safeRange = 150 + 200;
                            for (int i = 0; i < 3; i++)
                            {
                                Vector2 spawnPos = NPC.Center + Main.rand.NextVector2Circular(1200, 1200);
                                if (Vector2.Distance(safeZone, spawnPos) < safeRange)
                                {
                                    Vector2 directionOut = spawnPos - safeZone;
                                    directionOut.Normalize();
                                    spawnPos = safeZone + directionOut * Main.rand.NextFloat(safeRange, 1200);
                                }
                                Projectile.NewProjectile(NPC.GetSpawnSource_ForProjectile(), spawnPos, Vector2.Zero, ModContent.ProjectileType<MutantBomb>(), NPC.damage / 3, 0f, Main.myPlayer);
                            }
                        }
                    }

                    if (++NPC.ai[1] > 360)
                    {
                        ChooseNextAttack(11, 13, 16, 18, 24, 29, 31, 35, 37, 39, 41, 42);
                    }

                    if (NPC.ai[1] > 45)
                    {
                        for (int i = 0; i < 20; i++)
                        {
                            Vector2 offset = new Vector2();
                            offset.Y -= 100;
                            double angle = Main.rand.NextDouble() * 2d * Math.PI;
                            offset.X += (float)(Math.Sin(angle) * 150);
                            offset.Y += (float)(Math.Cos(angle) * 150);
                            Dust dust = Main.dust[Dust.NewDust(NPC.Center + offset - new Vector2(4, 4), 0, 0, 229, 0, 0, 100, Color.White, 1.5f)];
                            dust.velocity = NPC.velocity;
                            if (Main.rand.NextBool(3))
                                dust.velocity += Vector2.Normalize(offset) * 5f;
                            dust.noGravity = true;
                        }
                    }
                    break;

                case 35: //flee to prepare for slime rain
                    if (!AliveCheck(player))
                        break;
                    targetPos = player.Center;
                    targetPos.X += 700 * (NPC.Center.X < targetPos.X ? -1 : 1);
                    targetPos.Y += 200;
                    Movement(targetPos, 2f);
                    /*if (++NPC.ai[1] > 6)
                    {
                        NPC.ai[1] = 0;
                        Terraria.Audio.SoundEngine.PlaySound(SoundID.Item34, player.Center);
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            Vector2 spawnPos = NPC.Center;
                            spawnPos.X += (NPC.Center.X < player.Center.X) ? 900 : -900;
                            spawnPos.Y -= 1200;
                            for (int i = 0; i < 15; i++)
                                Projectile.NewProjectile(NPC.GetSpawnSource_ForProjectile(), spawnPos.X + Main.rand.Next(-300, 301), spawnPos.Y + Main.rand.Next(-100, 101), Main.rand.NextFloat(-5f, 5f), Main.rand.NextFloat(30f, 35f), ModContent.ProjectileType<MutantSlimeBall>(), NPC.damage / 5, 0f, Main.myPlayer);
                        }
                    }
                    if (NPC.ai[3] == 0)
                    {
                        NPC.ai[3] = 1;
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                            Projectile.NewProjectile(NPC.GetSpawnSource_ForProjectile(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<MutantSlimeRain>(), NPC.damage / 4, 0f, Main.myPlayer, NPC.whoAmI);
                    }*/
                    if (++NPC.ai[2] > 30)//180)
                    {
                        NPC.ai[0]++;
                        NPC.ai[1] = 0;
                        NPC.ai[2] = 0;
                        NPC.ai[3] = 0;
                        NPC.netUpdate = true;
                        //NPC.TargetClosest();
                    }
                    break;

                case 36: //slime rain
                    if (NPC.ai[3] == 0)
                    {
                        NPC.ai[3] = 1;
                        //Main.NewText(NPC.position.Y);
                        Terraria.Audio.SoundEngine.PlaySound(SoundID.Roar, (int)NPC.Center.X, (int)NPC.Center.Y, 0);
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                            Projectile.NewProjectile(NPC.GetSpawnSource_ForProjectile(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<MutantSlimeRain>(), NPC.damage / 4, 0f, Main.myPlayer, NPC.whoAmI);
                    }

                    if (NPC.ai[1] == 0) //telegraphs for where slime will fall
                    {
                        bool first = NPC.localAI[0] == 0;
                        NPC.localAI[0] = Main.rand.Next(5, 9) * 120;
                        if (first) //always start on the same side as the player
                        {
                            if (player.Center.X < NPC.Center.X && NPC.localAI[0] > 1200)
                                NPC.localAI[0] -= 1200;
                            else if (player.Center.X > NPC.Center.X && NPC.localAI[0] < 1200)
                                NPC.localAI[0] += 1200;
                        }
                        else //after that, always be on opposite side from player
                        {
                            if (player.Center.X < NPC.Center.X && NPC.localAI[0] < 1200)
                                NPC.localAI[0] += 1200;
                            else if (player.Center.X > NPC.Center.X && NPC.localAI[0] > 1200)
                                NPC.localAI[0] -= 1200;
                        }
                        NPC.localAI[0] += 60;
                        
                        Vector2 basePos = NPC.Center;
                        basePos.X -= 1200;
                        for (int i = -360; i <= 2760; i += 120) //spawn telegraphs
                        {
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                if (i + 60 == (int)NPC.localAI[0])
                                    continue;
                                Projectile.NewProjectile(NPC.GetSpawnSource_ForProjectile(), basePos.X + i + 60, basePos.Y, 0f, 0f, ModContent.ProjectileType<MutantReticle>(), 0, 0f, Main.myPlayer);
                            }
                        }

                        if (FargoSoulsWorld.MasochistModeReal)
                        {
                            NPC.ai[1] += 30; //less startup
                            NPC.ai[2] += 30; //stay synced
                        }
                    }

                    if (NPC.ai[1] > 120 && NPC.ai[1] % 5 == 0) //rain down slime balls
                    {
                        Terraria.Audio.SoundEngine.PlaySound(SoundID.Item34, player.Center);
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            Vector2 basePos = NPC.Center;
                            basePos.X -= 1200;
                            basePos.Y -= 1300;

                            const float safeRange = 110;
                            for (int i = -360; i <= 2760; i += 75)
                            {
                                float xOffset = i + Main.rand.Next(75);
                                if (Math.Abs(xOffset - NPC.localAI[0]) < safeRange) //dont fall over safespot
                                    continue;
                                Vector2 spawnPos = basePos;
                                spawnPos.X += xOffset;
                                Vector2 velocity = Vector2.UnitY * Main.rand.NextFloat(15f, 20f);

                                Projectile.NewProjectile(NPC.GetSpawnSource_ForProjectile(), spawnPos, velocity, ModContent.ProjectileType<MutantSlimeBall>(), NPC.damage / 5, 0f, Main.myPlayer);
                            }
                            
                            //spawn right on safespot borders
                            Projectile.NewProjectile(NPC.GetSpawnSource_ForProjectile(), basePos + Vector2.UnitX * (NPC.localAI[0] + safeRange),
                                Vector2.UnitY * Main.rand.NextFloat(15f, 20f),
                                ModContent.ProjectileType<MutantSlimeBall>(), NPC.damage / 5, 0f, Main.myPlayer);
                            Projectile.NewProjectile(NPC.GetSpawnSource_ForProjectile(), basePos + Vector2.UnitX * (NPC.localAI[0] - safeRange),
                                Vector2.UnitY * Main.rand.NextFloat(15f, 20f),
                                ModContent.ProjectileType<MutantSlimeBall>(), NPC.damage / 5, 0f, Main.myPlayer);
                        }
                    }
                    if (++NPC.ai[1] > 180)
                    {
                        if (!AliveCheck(player))
                            break;
                        NPC.ai[1] = 0;
                    }

                    NPC.velocity = Vector2.Zero;

                    /*if (--NPC.ai[1] < 0)
                    {
                        NPC.ai[1] = 100;
                        if (NPC.ai[2] < 330 && Main.netMode != NetmodeID.MultiplayerClient) //spawn irisu walls of slime balls
                        {
                            const int xRange = 1400;
                            Vector2 start = NPC.Center;
                            start.X -= xRange;
                            start.Y -= 1400 * NPC.localAI[0];

                            const int safeRange = 160;
                            int safespot = Main.rand.Next(1000, 1800) - safeRange / 2;

                            int end = 2 * xRange;
                            int type = ModContent.ProjectileType<MutantSlimeBall2>();
                            float speed = 5f * NPC.localAI[0];
                            for (int i = 0; i < safespot; i+= 48)
                            {
                                Projectile.NewProjectile(NPC.GetSpawnSource_ForProjectile(), start.X + i + Main.rand.NextFloat(-24, 0), start.Y + Main.rand.NextFloat(-24, 24),
                                        0f, speed, type, NPC.damage / 4, 0f, Main.myPlayer, NPC.whoAmI, Main.rand.NextFloat(-0.3f, 0.3f));
                                Projectile.NewProjectile(NPC.GetSpawnSource_ForProjectile(), start.X + i + Main.rand.NextFloat(-24, 0), start.Y - 48 + Main.rand.NextFloat(-24, 24),
                                    0f, speed, type, NPC.damage / 4, 0f, Main.myPlayer, NPC.whoAmI, Main.rand.NextFloat(-0.3f, 0.3f));
                            }
                            Projectile.NewProjectile(NPC.GetSpawnSource_ForProjectile(), start.X + safespot, start.Y - 24,
                                0f, speed, type, NPC.damage / 4, 0f, Main.myPlayer, NPC.whoAmI, Main.rand.NextFloat(-0.3f, 0.3f));
                            for (int i = safespot + safeRange; i < end; i += 48)
                            {
                                Projectile.NewProjectile(NPC.GetSpawnSource_ForProjectile(), start.X + i + Main.rand.NextFloat(24), start.Y + Main.rand.NextFloat(-24, 24),
                                        0f, speed, type, NPC.damage / 4, 0f, Main.myPlayer, NPC.whoAmI, Main.rand.NextFloat(-0.3f, 0.3f));
                                Projectile.NewProjectile(NPC.GetSpawnSource_ForProjectile(), start.X + i + Main.rand.NextFloat(24), start.Y - 48 + Main.rand.NextFloat(-24, 24),
                                    0f, speed, type, NPC.damage / 4, 0f, Main.myPlayer, NPC.whoAmI, Main.rand.NextFloat(-0.3f, 0.3f));
                            }
                            Projectile.NewProjectile(NPC.GetSpawnSource_ForProjectile(), start.X + safespot + safeRange, start.Y - 24,
                                0f, speed, type, NPC.damage / 4, 0f, Main.myPlayer, NPC.whoAmI, Main.rand.NextFloat(-0.3f, 0.3f));
                        }
                    }*/
                    if (++NPC.ai[2] > 180 * 3)
                    {
                        ChooseNextAttack(11, 16, 18, 20, 24, 29, 31, 33, 37, 39, 41, 42);
                    }
                    break;

                case 37: //go above to initiate dash
                    if (!AliveCheck(player))
                        break;
                    targetPos = player.Center;
                    targetPos.X += 400 * (NPC.Center.X < targetPos.X ? -1 : 1);
                    targetPos.Y -= 400;
                    Movement(targetPos, 0.9f);
                    if (++NPC.ai[1] > 60) //dive here
                    {
                        NPC.velocity.X = 35f * (NPC.position.X < player.position.X ? 1 : -1);
                        NPC.velocity.Y = 10f;
                        NPC.ai[0]++;
                        NPC.ai[1] = 0;
                        NPC.netUpdate = true;
                        //NPC.TargetClosest();
                    }
                    break;

                case 38: //spawn fishrons
                    goto case 30;

                case 39: //approach for okuu nonspell
                    if (!AliveCheck(player))
                        break;
                    targetPos = player.Center + player.DirectionTo(NPC.Center) * 300;
                    if (++NPC.ai[1] < 180 && NPC.Distance(targetPos) > 50)
                    {
                        Movement(targetPos, 0.8f);
                    }
                    else
                    {
                        NPC.netUpdate = true;
                        NPC.ai[0]++;
                        NPC.ai[1] = 0;
                        NPC.ai[2] = 0;
                        NPC.ai[3] = 0;
                    }
                    break;

                case 40: //SPHERE RING SPAMMMMM
                    NPC.velocity = Vector2.Zero;
                    if (++NPC.ai[1] > 10 && NPC.ai[3] > 60 && NPC.ai[3] < 360)
                    {
                        NPC.ai[1] = 0;
                        float rotation = MathHelper.ToRadians(60) * (NPC.ai[3] - 45) / 240 * NPC.ai[2];
                        float speed = FargoSoulsWorld.MasochistModeReal ? 11f : 10f;
                        SpawnSphereRing(9, speed, NPC.damage / 4, -1f, rotation);
                        SpawnSphereRing(9, speed, NPC.damage / 4, 1f, rotation);
                    }
                    if (NPC.ai[2] == 0)
                    {
                        NPC.ai[2] = Main.rand.NextBool() ? -1 : 1;
                        NPC.ai[3] = Main.rand.NextFloat((float)Math.PI * 2);
                        Terraria.Audio.SoundEngine.PlaySound(SoundID.Roar, (int)NPC.Center.X, (int)NPC.Center.Y, 0);
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                            Projectile.NewProjectile(NPC.GetSpawnSource_ForProjectile(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<Projectiles.GlowRing>(), 0, 0f, Main.myPlayer, NPC.whoAmI, -2);
                    }
                    if (++NPC.ai[3] > 420)
                    {
                        ChooseNextAttack(13, 18, 20, 21, 26, 33, 41, 44);
                    }
                    for (int i = 0; i < 5; i++)
                    {
                        int d = Dust.NewDust(NPC.position, NPC.width, NPC.height, 229, 0f, 0f, 0, default(Color), 1.5f);
                        Main.dust[d].noGravity = true;
                        Main.dust[d].noLight = true;
                        Main.dust[d].velocity *= 4f;
                    }
                    break;

                case 41: //throw penetrator again
                    if (!AliveCheck(player))
                        break;

                    if (NPC.ai[1] == 0)
                    {
                        NPC.localAI[0] = MathHelper.WrapAngle((NPC.Center - player.Center).ToRotation()); //remember initial angle offset
                        NPC.localAI[1] = FargoSoulsWorld.EternityMode ? Main.rand.Next(5, 9) : 5; //random max number of attacks
                        if (FargoSoulsWorld.MasochistModeReal)
                            NPC.localAI[1] += 3;
                        NPC.localAI[2] = Main.rand.NextBool() ? -1 : 1; //pick a random rotation direction
                        NPC.netUpdate = true;
                    }

                    //slowly rotate in full circle around player
                    targetPos = player.Center + 500f * Vector2.UnitX.RotatedBy(MathHelper.TwoPi / 300 * NPC.ai[3] * NPC.localAI[2] + NPC.localAI[0]);
                    if (NPC.Distance(targetPos) > 25)
                    {
                        Movement(targetPos, 0.6f);
                    }

                    ++NPC.ai[3]; //for keeping track of how much time has actually passed (ai1 jumps around)

                    if (++NPC.ai[1] > 180)
                    {
                        NPC.netUpdate = true;
                        NPC.ai[1] = 150;
                        if (++NPC.ai[2] > NPC.localAI[1])
                        {
                            ChooseNextAttack(11, 16, 18, 20, 26, 31, 33, 35, 42, 44);
                        }
                        else if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            Vector2 vel = NPC.DirectionTo(player.Center) * 30f;
                            Projectile.NewProjectile(NPC.GetSpawnSource_ForProjectile(), NPC.Center, Vector2.Normalize(vel), ModContent.ProjectileType<MutantDeathray2>(), NPC.damage / 5, 0f, Main.myPlayer);
                            Projectile.NewProjectile(NPC.GetSpawnSource_ForProjectile(), NPC.Center, -Vector2.Normalize(vel), ModContent.ProjectileType<MutantDeathray2>(), NPC.damage / 5, 0f, Main.myPlayer);
                            Projectile.NewProjectile(NPC.GetSpawnSource_ForProjectile(), NPC.Center, vel, ModContent.ProjectileType<MutantSpearThrown>(), NPC.damage / 4, 0f, Main.myPlayer, NPC.target);
                        }
                    }
                    else if (NPC.ai[1] == 151)
                    {
                        if (NPC.ai[2] > 0 && NPC.ai[2] < NPC.localAI[1] && Main.netMode != NetmodeID.MultiplayerClient)
                            Projectile.NewProjectile(NPC.GetSpawnSource_ForProjectile(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<MutantSpearAim>(), NPC.damage / 4, 0f, Main.myPlayer, NPC.whoAmI, 1);
                    }
                    else if (NPC.ai[1] == 1)
                    {
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                            Projectile.NewProjectile(NPC.GetSpawnSource_ForProjectile(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<MutantSpearAim>(), NPC.damage / 4, 0f, Main.myPlayer, NPC.whoAmI, -1);
                    }
                    break;

                case 42: //prepare for glaive/crystal attack
                    if (!AliveCheck(player))
                        break;
                    targetPos = player.Center;
                    targetPos.X += 500 * (NPC.Center.X < targetPos.X ? -1 : 1);
                    if (NPC.Distance(targetPos) > 50)
                        Movement(targetPos, 0.8f);
                    if (++NPC.ai[1] > 45)
                    {
                        NPC.netUpdate = true;
                        NPC.ai[0]++;
                        NPC.ai[1] = 0;
                        NPC.ai[2] = 0;
                        NPC.ai[3] = 0;
                        //NPC.TargetClosest();
                    }
                    break;

                case 43: //boomerangs
                    {
                        NPC.velocity = Vector2.Zero;

                        if (NPC.ai[3] == 0)
                        {
                            NPC.localAI[0] = NPC.DirectionFrom(player.Center).ToRotation();

                            if (!FargoSoulsWorld.MasochistModeReal && Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                for (int i = 0; i < 4; i++)
                                {
                                    Projectile.NewProjectile(NPC.GetSpawnSource_ForProjectile(), NPC.Center + Vector2.UnitX.RotatedBy(Math.PI / 2 * i) * 525, Vector2.Zero, ModContent.ProjectileType<Projectiles.GlowRingHollow>(), NPC.damage / 4, 0f, Main.myPlayer, 1f);
                                    Projectile.NewProjectile(NPC.GetSpawnSource_ForProjectile(), NPC.Center + Vector2.UnitX.RotatedBy(Math.PI / 2 * i + Math.PI / 4) * 350, Vector2.Zero, ModContent.ProjectileType<Projectiles.GlowRingHollow>(), NPC.damage / 4, 0f, Main.myPlayer, 2f);
                                }
                            }
                        }

                        const int ringDelay = 15;
                        const int ringMax = 4;
                        if (NPC.ai[3] % ringDelay == 0 && NPC.ai[3] < ringDelay * ringMax)
                        {
                            Terraria.Audio.SoundEngine.PlaySound(SoundID.Item84, NPC.Center);
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                float rotationOffset = MathHelper.TwoPi / ringMax * NPC.ai[3] / ringDelay + NPC.localAI[0];
                                int baseDelay = 60;
                                float flyDelay = 120 + NPC.ai[3] / ringDelay * 50;
                                int p = Projectile.NewProjectile(NPC.GetSpawnSource_ForProjectile(), NPC.Center, 300f / baseDelay * Vector2.UnitX.RotatedBy(rotationOffset), ModContent.ProjectileType<MutantMark2>(), NPC.damage / 4, 0f, Main.myPlayer, baseDelay, baseDelay + flyDelay);
                                if (p != Main.maxProjectiles)
                                {
                                    const int max = 5;
                                    const float distance = 125f;
                                    float rotation = MathHelper.TwoPi / max;
                                    for (int i = 0; i < max; i++)
                                    {
                                        float myRot = rotation * i + rotationOffset;
                                        Vector2 spawnPos = NPC.Center + new Vector2(distance, 0f).RotatedBy(myRot);
                                        Projectile.NewProjectile(NPC.GetSpawnSource_ForProjectile(), spawnPos, Vector2.Zero, ModContent.ProjectileType<MutantCrystalLeaf>(), NPC.damage / 4, 0f, Main.myPlayer, Main.projectile[p].identity, myRot);
                                    }
                                }
                            }
                        }

                        if (NPC.ai[3] > 45 && --NPC.ai[1] < 0)
                        {
                            NPC.netUpdate = true;
                            NPC.ai[1] = 20;
                            NPC.ai[2] = NPC.ai[2] > 0 ? -1 : 1;

                            Terraria.Audio.SoundEngine.PlaySound(SoundID.Item92, NPC.Center);

                            if (Main.netMode != NetmodeID.MultiplayerClient && NPC.ai[3] < 330)
                            {
                                const float retiRad = 525;
                                const float spazRad = 350;
                                float retiSpeed = 2 * (float)Math.PI * retiRad / 300;
                                float spazSpeed = 2 * (float)Math.PI * spazRad / 180;
                                float retiAcc = retiSpeed * retiSpeed / retiRad * NPC.ai[2];
                                float spazAcc = spazSpeed * spazSpeed / spazRad * -NPC.ai[2];
                                for (int i = 0; i < 4; i++)
                                {
                                    Projectile.NewProjectile(NPC.GetSpawnSource_ForProjectile(), NPC.Center, Vector2.UnitX.RotatedBy(Math.PI / 2 * i) * retiSpeed, ModContent.ProjectileType<MutantRetirang>(), NPC.damage / 4, 0f, Main.myPlayer, retiAcc, 300);
                                    Projectile.NewProjectile(NPC.GetSpawnSource_ForProjectile(), NPC.Center, Vector2.UnitX.RotatedBy(Math.PI / 2 * i + Math.PI / 4) * spazSpeed, ModContent.ProjectileType<MutantSpazmarang>(), NPC.damage / 4, 0f, Main.myPlayer, spazAcc, 180);
                                }
                            }
                        }
                        if (++NPC.ai[3] > 450)
                        {
                            ChooseNextAttack(11, 13, 16, 21, 24, 29, 31, 33, 35, 39, 41, 44);
                        }
                    }
                    break;

                case 44: //empress sword waves
                    {
                        if (!AliveCheck(player))
                            break;

                        if (!FargoSoulsWorld.EternityMode)
                        {
                            NPC.ai[0]++; //dont do this attack in expert
                            break;
                        }

                        NPC.velocity *= 0.95f;

                        const int startup = 60;
                        const int attackThreshold = 60;
                        const int timesToAttack = 4;

                        if (NPC.ai[1] == 0)
                        {
                            Terraria.Audio.SoundEngine.PlaySound(SoundID.Roar, NPC.Center, 0);
                            NPC.ai[3] = Main.rand.NextFloat(MathHelper.TwoPi);
                        }

                        void Sword(Vector2 pos, float ai0, float ai1)
                        {
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                Projectile.NewProjectile(NPC.GetSpawnSource_ForProjectile(), pos, Vector2.Zero,
                                    ProjectileID.FairyQueenLance, NPC.damage / 4, 0f, Main.myPlayer, ai0, ai1);
                            }
                        }

                        if (NPC.ai[1] >= startup && NPC.ai[1] < startup + attackThreshold * timesToAttack && --NPC.ai[2] < 0) //walls of swords
                        {
                            NPC.ai[2] = attackThreshold;

                            if (Math.Abs(MathHelper.WrapAngle(NPC.DirectionFrom(player.Center).ToRotation() - NPC.ai[3])) > MathHelper.PiOver2)
                                NPC.ai[3] += MathHelper.Pi; //swords always spawn closer to player

                            const int max = 10;
                            const float maxHorizSpread = 1600 * 2;
                            const float gap = maxHorizSpread / max;
                            const float arenaRadius = 1200;
                            float mirrorLength = 2f * (float)Math.Sqrt(2f * arenaRadius * arenaRadius);// + (float)Math.Sqrt(2f * gap * gap);

                            Vector2 focusPoint = NPC.Center + Main.rand.NextVector2Circular(gap, gap);
                            float attackAngle = NPC.ai[3];// + Main.rand.NextFloat(MathHelper.ToDegrees(10)) * (Main.rand.NextBool() ? -1 : 1);
                            Vector2 spawnOffset = -attackAngle.ToRotationVector2();

                            int swordCounter = 0;
                            for (int i = -max; i <= max; i++)
                            {
                                Vector2 spawnPos = focusPoint + spawnOffset * arenaRadius + spawnOffset.RotatedBy(MathHelper.PiOver2) * gap * i;
                                float Ai1 = swordCounter++ / (max * 2f + 1);
                                Sword(spawnPos, attackAngle + MathHelper.PiOver4, Ai1);
                                Sword(spawnPos, attackAngle - MathHelper.PiOver4, Ai1);

                                if (FargoSoulsWorld.MasochistModeReal)
                                {
                                    Sword(spawnPos + mirrorLength * (attackAngle + MathHelper.PiOver4).ToRotationVector2(), attackAngle + MathHelper.PiOver4 + MathHelper.Pi, Ai1);
                                    Sword(spawnPos + mirrorLength * (attackAngle - MathHelper.PiOver4).ToRotationVector2(), attackAngle - MathHelper.PiOver4 + MathHelper.Pi, Ai1);
                                }
                            }

                            NPC.ai[3] += MathHelper.PiOver4 * (Main.rand.NextBool() ? -1 : 1) //rotate 90 degrees
                                + Main.rand.NextFloat(MathHelper.PiOver4 / 2) * (Main.rand.NextBool() ? -1 : 1); //variation

                            NPC.netUpdate = true;
                        }

                        //massive sword barrage
                        const int swordSwarmTime = startup + attackThreshold * timesToAttack + 30;
                        if (NPC.ai[1] == swordSwarmTime)
                        {
                            float safeAngle = NPC.ai[3];
                            float safeRange = MathHelper.ToRadians(10);
                            int max = FargoSoulsWorld.MasochistModeReal ? 80 : 40;
                            for (int i = 0; i < max; i++)
                            {
                                float rotationOffset = Main.rand.NextFloat(safeRange, MathHelper.Pi - safeRange);
                                Vector2 offset = Main.rand.NextFloat(600f, 2400f) * (safeAngle + rotationOffset).ToRotationVector2();
                                if (Main.rand.NextBool())
                                    offset *= -1;

                                Vector2 target = player.Center;
                                if (FargoSoulsWorld.MasochistModeReal) //block one side so only one real exit exists
                                    target += Main.rand.NextFloat(600) * safeAngle.ToRotationVector2();

                                Vector2 spawnPos = player.Center + offset;
                                Sword(spawnPos, (target - spawnPos).ToRotation(), (float)i / max);
                            }
                        }

                        if (++NPC.ai[1] > swordSwarmTime + 30)
                        {
                            ChooseNextAttack(11, 13, 16, 21, 24, 29, 31, 35, 37, 39, 41);
                        }
                    }
                    break;

                    //gap in the numbers here so the ai loops right

                case 46: //choose next attack but actually, this also gives breathing space for mp to sync up
                    if (!AliveCheck(player))
                        break;
                    targetPos = player.Center + NPC.DirectionFrom(player.Center) * 400;
                    Movement(targetPos, 0.3f);
                    if (NPC.Distance(targetPos) > 200) //faster if offscreen
                        Movement(targetPos, 0.3f);
                    if (++NPC.ai[1] > 60 || (NPC.Distance(targetPos) < 200 && NPC.ai[1] > (NPC.localAI[3] >= 3 ? 15 : 30)))
                    {
                        /*EModeGlobalNPC.PrintAI(npc);
                        string output = "";
                        foreach (float attack in attackHistory)
                            output += attack.ToString() + " ";
                        Main.NewText(output);*/

                        NPC.velocity *= 0.75f;

                        //NPC.TargetClosest();
                        NPC.ai[0] = NPC.ai[2];
                        NPC.ai[1] = 0;
                        NPC.ai[2] = 0;
                        NPC.netUpdate = true;
                    }
                    break;

                default:
                    NPC.ai[0] = 11;
                    //NPC.ai[2] = 180; //just start the laser attack without moving
                    goto case 11;
            }

            if (player.immune || player.hurtCooldowns[0] != 0 || player.hurtCooldowns[1] != 0)
                playerInvulTriggered = true;

            //drop summon
            if (FargoSoulsWorld.downedAbom && !FargoSoulsWorld.downedMutant && Main.netMode != NetmodeID.MultiplayerClient && NPC.HasPlayerTarget && !droppedSummon)
            {
                Item.NewItem(NPC.GetItemSource_Loot(), player.Hitbox, ModContent.ItemType<MutantsCurse>());
                droppedSummon = true;
            }
        }

        /*private void EdgyBossText(string text)
        {
            if (Fargowiltas.Instance.CalamityLoaded) //edgy boss text
            {
                if (Main.netMode == NetmodeID.SinglePlayer)
                    Main.NewText(text, Color.LimeGreen);
                else if (Main.netMode == NetmodeID.Server)
                    ChatHelper.BroadcastChatMessage(NetworkText.FromLiteral(text), Color.LimeGreen);
            }
        }*/

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            if (FargoSoulsWorld.EternityMode)
            {
                target.GetModPlayer<FargoSoulsPlayer>().MaxLifeReduction += 100;
                target.AddBuff(ModContent.BuffType<OceanicMaul>(), 5400);
                target.AddBuff(ModContent.BuffType<MutantFang>(), 180);
            }
            target.AddBuff(ModContent.BuffType<CurseoftheMoon>(), 600);
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            for (int i = 0; i < 3; i++)
            {
                int d = Dust.NewDust(NPC.position, NPC.width, NPC.height, 229, 0f, 0f, 0, default(Color), 1f);
                Main.dust[d].noGravity = true;
                Main.dust[d].noLight = true;
                Main.dust[d].velocity *= 3f;
            }
        }

        public override bool StrikeNPC(ref double damage, int defense, ref float knockback, int hitDirection, ref bool crit)
        {
            if (FargoSoulsWorld.AngryMutant)// || FargowiltasSouls.Instance.CalamityLoaded)
                damage *= 0.7f;
            return true;
        }

        public override bool CheckDead()
        {
            if (NPC.ai[0] == -7)
                return true;

            NPC.life = 1;
            NPC.active = true;
            if (Main.netMode != NetmodeID.MultiplayerClient && NPC.ai[0] > -1)
            {
                NPC.ai[0] = FargoSoulsWorld.EternityMode ? -1 : -6;
                NPC.ai[1] = 0;
                NPC.ai[2] = 0;
                NPC.ai[3] = 0;
                NPC.localAI[0] = 0;
                NPC.localAI[1] = 0;
                NPC.localAI[2] = 0;
                NPC.dontTakeDamage = true;
                NPC.netUpdate = true;
                FargoSoulsUtil.ClearHostileProjectiles(2, NPC.whoAmI);
                //EdgyBossText("You're pretty good...");
            }
            return false;
        }

        public override void OnKill()
        {
            base.OnKill();

            if (!playerInvulTriggered && FargoSoulsWorld.EternityMode)
            {
                Item.NewItem(NPC.GetItemSource_Loot(), NPC.Hitbox, ModContent.ItemType<PhantasmalEnergy>());
                Item.NewItem(NPC.GetItemSource_Loot(), NPC.Hitbox, ModContent.ItemType<SpawnSack>());
            }

            if (FargoSoulsWorld.EternityMode)
            {
                if (Main.LocalPlayer.active)
                {
                    if (!Main.LocalPlayer.GetModPlayer<FargoSoulsPlayer>().Toggler.CanPlayMaso && Main.netMode != NetmodeID.Server)
                        Main.NewText("Mutant's Gift surges with new power...!", new Color(51, 255, 191, 0));
                    Main.LocalPlayer.GetModPlayer<FargoSoulsPlayer>().Toggler.CanPlayMaso = true;
                }
                FargoSoulsWorld.CanPlayMaso = true;
            }

            FargoSoulsWorld.skipMutantP1 = 0;

            NPC.SetEventFlagCleared(ref FargoSoulsWorld.downedMutant, -1);
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            base.ModifyNPCLoot(npcLoot);

            npcLoot.Add(ItemDropRule.BossBag(ModContent.ItemType<MutantBag>()));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<MutantTrophy>(), 10));

            LeadingConditionRule emodeRule = new LeadingConditionRule(new EModeDropCondition());
            emodeRule.OnSuccess(FargoSoulsUtil.BossBagDropCustom(ModContent.ItemType<Items.Accessories.Masomode.MutantEye>()));
            npcLoot.Add(emodeRule);
        }

        public override void BossLoot(ref string name, ref int potionType)
        {
            potionType = ItemID.SuperHealingPotion;
        }

        public override void FindFrame(int frameHeight)
        {
            if (++NPC.frameCounter > 4)
            {
                NPC.frameCounter = 0;
                NPC.frame.Y += frameHeight;
                if (NPC.frame.Y >= Main.npcFrameCount[NPC.type] * frameHeight)
                    NPC.frame.Y = 0;
            }
        }

        public override void BossHeadSpriteEffects(ref SpriteEffects spriteEffects)
        {
            //spriteEffects = NPC.spriteDirection < 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Npc[NPC.type].Value;
            Rectangle rectangle = NPC.frame;
            Vector2 origin2 = rectangle.Size() / 2f;

            SpriteEffects effects = NPC.spriteDirection < 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Main.EntitySpriteDraw(texture2D13, NPC.Center - screenPos + new Vector2(0f, NPC.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), NPC.GetAlpha(drawColor), NPC.rotation, origin2, NPC.scale, effects, 0);
            return false;
        }
    }
}