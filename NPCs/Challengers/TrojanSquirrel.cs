using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using Microsoft.Xna.Framework.Graphics;
using FargowiltasSouls.Buffs.Masomode;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using FargowiltasSouls.ItemDropRules.Conditions;
using Terraria.DataStructures;
using System.IO;
using FargowiltasSouls.Items.Summons;

namespace FargowiltasSouls.NPCs.Challengers
{
    [AutoloadBossHead]
    public class TrojanSquirrel : ModNPC
    {
        private const float BaseWalkSpeed = 4f;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Trojan Squirrel");
            
            Main.npcFrameCount[NPC.type] = 8;
            NPCID.Sets.BossBestiaryPriority.Add(NPC.type);

            NPCID.Sets.DebuffImmunitySets.Add(NPC.type, new Terraria.DataStructures.NPCDebuffImmunityData
            {
                SpecificallyImmuneTo = new int[]
                {
                    BuffID.Confused,
                    ModContent.BuffType<Lethargic>(),
                    ModContent.BuffType<ClippedWings>()
                }
            });

            NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, new NPCID.Sets.NPCBestiaryDrawModifiers(0)
            {
                CustomTexturePath = $"FargowiltasSouls/NPCs/Challengers/{Name}_Still",
                Position = new Vector2(16 * 4, 16 * 9),
                PortraitPositionXOverride = 16,
                PortraitPositionYOverride = 16 * 7
            });
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface,
                new FlavorTextBestiaryInfoElement($"Mods.FargowiltasSouls.Bestiary.{Name}")
            });
        }

        public override void SetDefaults()
        {
            NPC.width = 100;
            NPC.height = 120; //234
            NPC.damage = 32;
            NPC.defense = 6;
            NPC.lifeMax = 500;
            NPC.HitSound = SoundID.NPCHit7;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.knockBackResist = 0f;
            NPC.lavaImmune = false;
            NPC.aiStyle = -1;
            NPC.value = Item.buyPrice(silver: 75);
            NPC.boss = true;

            Music = ModLoader.TryGetMod("FargowiltasMusic", out Mod musicMod)
                ? MusicLoader.GetMusicSlot(musicMod, "Assets/Music/Champions") : MusicID.OtherworldlyBoss1;
            SceneEffectPriority = SceneEffectPriority.BossLow;
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            //NPC.damage = (int)(NPC.damage * 0.5f);
            NPC.lifeMax = (int)(NPC.lifeMax * Math.Sqrt(bossLifeScale));
        }

        public int head = -1;
        public int arms = -1;

        private bool spawned;

        public override void OnSpawn(IEntitySource source)
        {
            NPC.TargetClosest(false);

            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                //head = FargoSoulsUtil.NewNPCEasy(NPC.GetSource_FromThis(), NPC.Center, ModContent.NPCType<TrojanSquirrelHead>(), NPC.whoAmI, target: NPC.target);
                //arms = FargoSoulsUtil.NewNPCEasy(NPC.GetSource_FromThis(), NPC.Center, ModContent.NPCType<TrojanSquirrelArms>(), NPC.whoAmI, target: NPC.target);
            }
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(NPC.localAI[0]);
            writer.Write(NPC.localAI[1]);
            writer.Write(NPC.localAI[2]);
            writer.Write(NPC.localAI[3]);
            writer.Write(head);
            writer.Write(arms);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            NPC.localAI[0] = reader.ReadSingle();
            NPC.localAI[1] = reader.ReadSingle();
            NPC.localAI[2] = reader.ReadSingle();
            NPC.localAI[3] = reader.ReadSingle();
            head = reader.ReadInt32();
            arms = reader.ReadInt32();
        }

        private void TileCollision(bool fallthrough = false, bool dropDown = false)
        {
            bool onPlatforms = false;
            for (int i = (int)NPC.position.X; i <= NPC.position.X + NPC.width; i += 16)
            {
                if (Framing.GetTileSafely(new Vector2(i, NPC.Bottom.Y + NPC.velocity.Y + 1)).TileType == TileID.Platforms)
                {
                    onPlatforms = true;
                    break;
                }
            }

            bool onCollision = Collision.SolidCollision(NPC.position, NPC.width, NPC.height);

            if (dropDown)
            {
                NPC.velocity.Y += 0.5f;
            }
            else if (onCollision || (onPlatforms && !fallthrough))
            {
                if (NPC.velocity.Y > 0f)
                    NPC.velocity.Y = 0f;

                if (NPC.velocity.Y > -0.2f)
                    NPC.velocity.Y -= 0.025f;
                else
                    NPC.velocity.Y -= 0.2f;

                if (NPC.velocity.Y < -4f)
                    NPC.velocity.Y = -4f;
            }
            else
            {
                if (NPC.velocity.Y < 0f)
                    NPC.velocity.Y = 0f;

                if (NPC.velocity.Y < 0.1f)
                    NPC.velocity.Y += 0.025f;
                else
                    NPC.velocity.Y += 0.5f;
            }

            if (NPC.velocity.Y > 10f)
                NPC.velocity.Y = 10f;
        }

        private void Movement(Vector2 target, bool goFast = false)
        {
            NPC.direction = NPC.spriteDirection = NPC.Center.X < target.X ? 1 : -1;

            if (Math.Abs(target.X - NPC.Center.X) < NPC.width / 2)
            {
                NPC.velocity.X *= 0.9f;
                if (Math.Abs(NPC.velocity.X) < 0.1f)
                    NPC.velocity.X = 0f;
            }
            else
            {
                float maxwalkSpeed = BaseWalkSpeed;

                if (goFast)
                {
                    maxwalkSpeed *= 3f;
                    if (!FargoSoulsWorld.EternityMode)
                        maxwalkSpeed *= 0.75f;
                }
                else if (!FargoSoulsWorld.MasochistModeReal)
                {
                    maxwalkSpeed *= 0.75f;
                }

                if (NPC.dontTakeDamage)
                    maxwalkSpeed *= 0.75f;

                int walkModifier = FargoSoulsWorld.EternityMode ? 20 : 30;
                if (NPC.direction > 0)
                    NPC.velocity.X = (NPC.velocity.X * walkModifier + maxwalkSpeed) / (walkModifier + 1);
                else
                    NPC.velocity.X = (NPC.velocity.X * walkModifier - maxwalkSpeed) / (walkModifier + 1);
            }

            TileCollision(target.Y > NPC.Bottom.Y, Math.Abs(target.X - NPC.Center.X) < NPC.width / 2 && NPC.Bottom.Y < target.Y);
        }


        public override void AI()
        {
            Player player = Main.player[NPC.target];
            NPC.direction = NPC.spriteDirection = NPC.Center.X < player.Center.X ? 1 : -1;

            if (!spawned)
            {
                spawned = true;

                //drop summon
                if (FargoSoulsWorld.EternityMode && !FargoSoulsWorld.downedBoss[(int)FargoSoulsWorld.Downed.TrojanSquirrel] && Main.netMode != NetmodeID.MultiplayerClient)
                    Item.NewItem(NPC.GetSource_Loot(), player.Hitbox, ModContent.ItemType<SquirrelCoatofArms>());

                //start by jumping
                NPC.ai[0] = 1f;
                NPC.ai[3] = 1f;
            }

            bool despawn = false;

            switch ((int)NPC.ai[0])
            {
                case 0: //mourning wood movement
                    {
                        Vector2 target = player.Bottom - Vector2.UnitY;
                        if (NPC.localAI[0] > 0) //doing running attack
                        {
                            NPC.localAI[0] -= 1f;

                            float distance = NPC.Center.X - target.X;
                            bool passedTarget = Math.Sign(distance) == NPC.localAI[1];
                            if (passedTarget && Math.Abs(distance) > 160)
                                NPC.localAI[0] = 0f;

                            target = new Vector2(NPC.Center.X + 256f * NPC.localAI[1], target.Y);

                            if (NPC.localAI[0] == 0f)
                                NPC.TargetClosest(false);
                        }
                        else if (!NPC.HasValidTarget || NPC.Distance(player.Center) > 1000)
                        {
                            target = NPC.Center + new Vector2(256f * Math.Sign(NPC.Center.X - player.Center.X), -128);

                            NPC.TargetClosest(false);

                            despawn = true;
                        }

                        bool goFast = despawn || NPC.localAI[0] > 0;
                        Movement(target, goFast);

                        bool canDoAttacks = !goFast && (!NPC.dontTakeDamage || FargoSoulsWorld.EternityMode);
                        if (canDoAttacks) //decide next action
                        {
                            float increment = 1f;

                            if (target.Y > NPC.Top.Y)
                                NPC.ai[1] += increment;
                            else
                                NPC.ai[2] += increment;

                            if (Math.Abs(NPC.velocity.Y) < 0.05f)
                            {
                                int threshold = 300;

                                if (NPC.ai[1] > threshold)
                                {
                                    NPC.ai[0] = 1f;
                                    NPC.ai[1] = 0f;
                                    //NPC.ai[2] = 0f;
                                    NPC.ai[3] = 0f;
                                    NPC.localAI[0] = 0f;
                                    NPC.netUpdate = true;
                                }
                                else if (NPC.ai[2] > threshold)
                                {
                                    NPC.ai[0] = 1f;
                                    //NPC.ai[1] = 0f;
                                    NPC.ai[2] = 0f;
                                    NPC.ai[3] = 1f;
                                    NPC.localAI[0] = 0f;
                                    NPC.netUpdate = true;
                                }
                            }
                        }
                    }
                    break;

                case 1: //telegraph something
                    NPC.velocity.X *= 0.9f;

                    TileCollision(player.Bottom.Y - 1 > NPC.Bottom.Y, Math.Abs(player.Center.X - NPC.Center.X) < NPC.width / 2 && NPC.Bottom.Y < player.Bottom.Y - 1);

                    if (++NPC.localAI[0] > 90)
                    {
                        NPC.localAI[0] = 0f;
                        NPC.netUpdate = true;

                        if (NPC.ai[3] == 0f)
                        {
                            NPC.ai[0] = 0f;

                            NPC.localAI[0] = 300f;
                            NPC.localAI[1] = Math.Sign(player.Center.X - NPC.Center.X);
                            NPC.localAI[2] = player.Center.X;
                        }
                        else
                        {
                            NPC.ai[0] = 2f;
                        }
                    }
                    break;

                case 2: //jump
                    {
                        const float gravity = 0.4f;
                        const float time = 90f;

                        if (NPC.localAI[0]++ == 0)
                        {
                            Vector2 distance = player.Top - NPC.Bottom;

                            distance.X = distance.X / time;
                            distance.Y = distance.Y / time - 0.5f * gravity * time;
                            NPC.velocity = distance;

                            NPC.netUpdate = true;

                            Terraria.Audio.SoundEngine.PlaySound(SoundID.Item, NPC.Center, 14);
                        }
                        else
                        {
                            NPC.velocity.Y += gravity;
                        }

                        if (NPC.localAI[0] > time)
                        {
                            NPC.TargetClosest(false);

                            float cap = BaseWalkSpeed * 5;
                            NPC.velocity.X = Utils.Clamp(NPC.velocity.X, -cap, cap);
                            NPC.velocity.Y = Utils.Clamp(NPC.velocity.Y, -cap, cap);

                            NPC.ai[0] = 0f;
                            NPC.localAI[0] = 0f;
                            NPC.netUpdate = true;
                        }
                    }
                    break;

                default:
                    NPC.ai[0] = 0;
                    goto case 0;
            }

            //FargoSoulsUtil.PrintAI(NPC);

            if (despawn)
            {
                if (NPC.timeLeft > 60)
                    NPC.timeLeft = 60;
            }
            else
            {
                if (NPC.timeLeft < 600)
                    NPC.timeLeft = 600;
            }

            if (FargoSoulsUtil.NPCExists(head/*, ModContent.NPCType<TrojanSquirrelHead>()*/) == null)
            {
                head = -1;

                Vector2 pos = NPC.Top;
                pos.X += 2f * 16f * NPC.direction;
                pos.Y -= 8f;

                int width = 4 * 16;
                int height = 2 * 16;

                pos.X -= width / 2f;
                pos.Y -= height / 2f;

                for (int i = 0; i < 3; i++)
                {
                    int d = Dust.NewDust(pos, width, height, DustID.Smoke, NPC.velocity.X, NPC.velocity.Y, 50, default(Color), 3f);
                    Main.dust[d].velocity.Y -= 1.5f;
                    Main.dust[d].velocity *= 1.5f;
                    Main.dust[d].noGravity = true;
                }

                if (Main.rand.NextBool(3))
                {
                    int d = Dust.NewDust(pos, width, height, DustID.Torch, NPC.velocity.X * 0.4f, NPC.velocity.Y * 0.4f, 100, default(Color), 3.5f);
                    Main.dust[d].noGravity = true;
                    Main.dust[d].velocity.Y -= 3f;
                    Main.dust[d].velocity *= 1.5f;
                }
            }

            if (FargoSoulsUtil.NPCExists(arms/*, ModContent.NPCType<TrojanSquirrelArms>()*/) == null)
            {
                arms = -1;

                Vector2 pos = NPC.Center;
                pos.X -= 16f * NPC.direction;
                pos.Y -= 3f * 16f;

                int width = 2 * 16;
                int height = 2 * 16;

                pos.X -= width / 2f;
                pos.Y -= height / 2f;

                for (int i = 0; i < 2; i++)
                {
                    int d = Dust.NewDust(pos, width, height, DustID.Smoke, NPC.velocity.X, NPC.velocity.Y, 50, default(Color), 1.5f);
                    Main.dust[d].noGravity = true;
                }

                if (Main.rand.NextBool())
                {
                    int d2 = Dust.NewDust(pos, width, height, DustID.Torch, NPC.velocity.X * 0.4f, NPC.velocity.Y * 0.4f, 100, default(Color), 3f);
                    Main.dust[d2].noGravity = true;
                }
            }
        }

        public override void FindFrame(int frameHeight)
        {
            switch ((int)NPC.ai[0])
            {
                case 0:
                    {
                        NPC.frameCounter += 1f / BaseWalkSpeed * Math.Abs(NPC.velocity.X);

                        if (NPC.frameCounter > 2.5f) //walking animation
                        {
                            NPC.frameCounter = 0;
                            NPC.frame.Y += frameHeight;
                        }

                        if (NPC.frame.Y >= frameHeight * 6)
                            NPC.frame.Y = 0;

                        if (NPC.velocity.X == 0)
                            NPC.frame.Y = frameHeight; //stationary sprite if standing still

                        if (NPC.velocity.Y > 4)
                            NPC.frame.Y = frameHeight * 7; //jumping
                    }
                    break;

                case 1:
                    NPC.frame.Y = frameHeight * 6; //crouching for jump
                    break;

                case 2:
                    NPC.frame.Y = frameHeight * 7; //jumping
                    break;

                default:
                    goto case 0;
            }
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            if (NPC.life <= 0)
            {
                //1, 2 for head

                for (int i = 3; i <= 7; i++)
                {
                    Vector2 pos = NPC.position + new Vector2(Main.rand.NextFloat(NPC.width), Main.rand.NextFloat(NPC.height));
                    if (!Main.dedServ)
                        Gore.NewGore(NPC.GetSource_FromThis(), pos, NPC.velocity, ModContent.Find<ModGore>(Mod.Name, $"TrojanSquirrelGore{i}").Type, NPC.scale);
                }

                //8, 9, 10 for arms
            }
        }

        public override void BossLoot(ref string name, ref int potionType)
        {
            potionType = ItemID.LesserHealingPotion;
        }

        public override void OnKill()
        {
            NPC.SetEventFlagCleared(ref FargoSoulsWorld.downedBoss[(int)FargoSoulsWorld.Downed.TrojanSquirrel], -1);

            if (ModContent.TryFind("Fargowiltas", "Squirrel", out ModNPC squrrl) && !NPC.AnyNPCs(squrrl.Type))
                FargoSoulsUtil.NewNPCEasy(NPC.GetSource_FromThis(), NPC.Center, squrrl.Type);
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            
        }

        public override void BossHeadSpriteEffects(ref SpriteEffects spriteEffects)
        {
            spriteEffects = NPC.direction < 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
        }
    }
}
