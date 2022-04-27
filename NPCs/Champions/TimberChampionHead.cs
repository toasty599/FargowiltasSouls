using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using FargowiltasSouls.Projectiles.Champions;
using Microsoft.Xna.Framework.Graphics;
using FargowiltasSouls.Buffs.Masomode;
using FargowiltasSouls.ItemDropRules.Conditions;
using FargowiltasSouls.Items.Accessories.Enchantments;

namespace FargowiltasSouls.NPCs.Champions
{
    [AutoloadBossHead]
    public class TimberChampionHead : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Champion of Timber");
            DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "木英灵");
            Main.npcFrameCount[NPC.type] = 3;
            NPCID.Sets.TrailCacheLength[NPC.type] = 6;
            NPCID.Sets.TrailingMode[NPC.type] = 1;

            NPCID.Sets.DebuffImmunitySets.Add(NPC.type, new Terraria.DataStructures.NPCDebuffImmunityData
            {
                SpecificallyImmuneTo = new int[]
                {
                    BuffID.Confused,
                    BuffID.Chilled,
                    BuffID.OnFire,
                    BuffID.Suffocation,
                    ModContent.BuffType<Lethargic>(),
                    ModContent.BuffType<ClippedWings>()
                }
            });

            NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, new NPCID.Sets.NPCBestiaryDrawModifiers(0)
            {
                Hide = true
            });
        }

        public override void SetDefaults()
        {
            NPC.width = 98;
            NPC.height = 76;
            NPC.damage = 130;
            NPC.defense = 50;
            NPC.lifeMax = 240000;
            NPC.HitSound = SoundID.NPCHit7;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.knockBackResist = 0f;
            NPC.lavaImmune = true;
            NPC.aiStyle = -1;
            NPC.value = Item.buyPrice(0, 15);
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

        public override bool CanHitPlayer(Player target, ref int CooldownSlot)
        {
            CooldownSlot = 1;
            return true;
        }

        public override void AI()
        {
            if (NPC.localAI[2] == 0)
            {
                Terraria.Audio.SoundEngine.PlaySound(SoundID.Roar, NPC.Center, 0);
                NPC.TargetClosest(false);
                NPC.localAI[2] = 1;
            }

            EModeGlobalNPC.championBoss = NPC.whoAmI;

            Player player = Main.player[NPC.target];
            NPC.direction = NPC.spriteDirection = NPC.Center.X < player.position.X ? 1 : -1;
            Vector2 targetPos;

            switch ((int)NPC.ai[0])
            {
                case 0: //laser rain
                    if (!player.active || player.dead || Vector2.Distance(NPC.Center, player.Center) > 2500f) //despawn code
                    {
                        NPC.TargetClosest(false);
                        if (NPC.timeLeft > 30)
                            NPC.timeLeft = 30;

                        NPC.noTileCollide = true;
                        NPC.noGravity = true;
                        NPC.velocity.Y -= 1f;

                        break;
                    }
                    else
                    {
                        targetPos = player.Center + NPC.DirectionFrom(player.Center) * 400f;
                        if (NPC.Distance(targetPos) > 50)
                            Movement(targetPos, 0.2f, 24f);
                    }

                    if (++NPC.ai[1] > 30)
                    {
                        NPC.TargetClosest();
                        NPC.ai[0]++;
                        NPC.ai[1] = 0;
                        NPC.ai[2] = 0;
                        NPC.ai[3] = 0;
                        NPC.netUpdate = true;
                    }
                    break;

                case 1: //laser rain
                    if (NPC.ai[1] == 0 && Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, (player.Center - NPC.Center) / 120,
                            ModContent.ProjectileType<TimberSquirrel>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 0f, Main.myPlayer);
                    }

                    if (NPC.ai[1] > 90)
                    {
                        NPC.velocity *= 0.9f;
                    }
                    else
                    {
                        targetPos = player.Center;
                        targetPos.Y -= 300;

                        if (NPC.Distance(targetPos) > 50)
                            Movement(targetPos, 0.25f, 24f);
                    }

                    if (++NPC.ai[1] < 120)
                    {
                        /*for (int i = 0; i < 5; i++) //warning dust
                        {
                            int d = Dust.NewDust(NPC.position, NPC.width, NPC.height, 16, NPC.velocity.X * 0.4f, NPC.velocity.Y * 0.4f, 100, default, 1.5f);
                            Main.dust[d].velocity *= 3f;
                            Main.dust[d].noGravity = true;
                        }*/
                    }
                    else if (NPC.ai[1] == 120)
                    {
                        Terraria.Audio.SoundEngine.PlaySound(SoundID.Roar, NPC.Center, 0);
                        NPC.netUpdate = true;
                    }
                    else if (NPC.ai[1] < 270) //spam lasers everywhere
                    {
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            for (int i = 0; i < 4; i++)
                            {
                                Vector2 spawnPos = player.Center;
                                spawnPos.X += Main.rand.NextFloat(-1000, 1000);
                                spawnPos.Y -= Main.rand.NextFloat(600, 800);
                                Vector2 speed = Main.rand.NextFloat(7.5f, 12.5f) * Vector2.UnitY;
                                Projectile.NewProjectile(NPC.GetSource_FromThis(), spawnPos, speed, ModContent.ProjectileType<TimberLaser>(), 
                                    FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 0f, Main.myPlayer, NPC.whoAmI, 80f);
                            }
                        }
                    }
                    else
                    {
                        NPC.TargetClosest();
                        NPC.ai[0]++;
                        NPC.ai[1] = 0;
                        NPC.ai[2] = 0;
                        NPC.ai[3] = 0;
                        NPC.netUpdate = true;
                    }
                    break;

                case 2:
                    goto case 0;

                case 3: //shoot acorns
                    targetPos = player.Center;
                    targetPos.X += NPC.Center.X < player.Center.X ? -400 : 400;
                    targetPos.Y -= 100;
                    if (NPC.Distance(targetPos) > 50)
                        Movement(targetPos, 0.25f, 32f);

                    if (++NPC.ai[2] > 35) //acorn
                    {
                        NPC.ai[2] = 0;
                        const float gravity = 0.2f;
                        float time = 40f;
                        Vector2 distance = player.Center - NPC.Center;// + player.velocity * 30f;
                        distance.X += player.velocity.X * 40;
                        distance.X = distance.X / time;
                        distance.Y = distance.Y / time - 0.5f * gravity * time;
                        for (int i = 0; i < 20; i++)
                        {
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, distance + Main.rand.NextVector2Square(-0.5f, 0.5f) * 3,
                                ModContent.ProjectileType<TimberAcorn>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 0f, Main.myPlayer);
                        }
                    }

                    if (++NPC.ai[1] > 200)
                    {
                        NPC.TargetClosest();
                        NPC.ai[0]++;
                        NPC.ai[1] = 0;
                        NPC.ai[2] = 0;
                        NPC.ai[3] = 0;
                        NPC.netUpdate = true;
                    }
                    break;

                case 4:
                    goto case 0;

                case 5: //trees that shoot acorns
                    targetPos = player.Center + NPC.DirectionFrom(player.Center) * 300;
                    if (targetPos.Y > player.position.Y - 100)
                        targetPos.Y = player.position.Y - 100;
                    if (NPC.Distance(targetPos) > 50)
                        Movement(targetPos, 0.3f, 24f);

                    if (--NPC.ai[2] < 0)
                    {
                        NPC.ai[2] = 70;

                        if (NPC.ai[1] < 300)
                        {
                            for (int i = 0; i < 5; i++) //spawn trees
                            {
                                Vector2 spawnPos = player.Center;
                                spawnPos.X += Main.rand.NextFloat(-1500, 1500) + player.velocity.X * 75;
                                spawnPos.Y -= Main.rand.NextFloat(300);
                                for (int j = 0; j < 100; j++) //go down until solid tile found
                                {
                                    Tile tile = Main.tile[(int)spawnPos.X / 16, (int)spawnPos.Y / 16];
                                    if (tile == null)
                                        tile = new Tile();
                                    if (tile.HasUnactuatedTile && (Main.tileSolid[tile.TileType] || Main.tileSolidTop[tile.TileType]))
                                        break;
                                    spawnPos.Y += 16;
                                }
                                for (int j = 0; j < 50; j++) //go up until non-solid tile found
                                {
                                    Tile tile = Main.tile[(int)spawnPos.X / 16, (int)spawnPos.Y / 16];
                                    if (tile == null)
                                        tile = new Tile();
                                    if (!(tile.HasUnactuatedTile && (Main.tileSolid[tile.TileType] || Main.tileSolidTop[tile.TileType])))
                                        break;
                                    spawnPos.Y -= 16;
                                }

                                const float gravity = 0.2f;
                                float time = 90f;
                                Vector2 distance = spawnPos - NPC.Center;
                                distance.X = distance.X / time;
                                distance.Y = distance.Y / time - 0.5f * gravity * time;
                                Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, distance, ModContent.ProjectileType<TimberTreeAcorn>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 0f, Main.myPlayer, NPC.target);

                                //spawnPos.Y -= 152; //offset for height of tree
                                //Projectile.NewProjectile(npc.GetSource_FromThis(), spawnPos, Vector2.Zero, ModContent.ProjectileType<TimberTree>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 0f, Main.myPlayer, NPC.target);
                            }
                        }
                    }

                    if (++NPC.ai[1] > 390)
                    {
                        NPC.TargetClosest();
                        NPC.ai[0]++;
                        NPC.ai[1] = 0;
                        NPC.ai[2] = 0;
                        NPC.ai[3] = 0;
                        NPC.netUpdate = true;
                    }
                    break;

                case 6:
                    goto case 0;

                case 7: //chains
                    targetPos = player.Center + 150 * NPC.DirectionFrom(player.Center);
                    Movement(targetPos, 0.25f, 24f);
                    if (NPC.Distance(player.Center) < 150)
                        Movement(targetPos, 0.5f, 24f);
                    NPC.position += (player.position - player.oldPosition) / 3;

                    if (NPC.ai[1] < 240)
                    {
                        if (++NPC.ai[2] > 8)
                        {
                            NPC.ai[2] = 0;

                            Terraria.Audio.SoundEngine.PlaySound(SoundID.Item92, NPC.Center);

                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                Vector2 speed = 32f * NPC.DirectionTo(player.Center).RotatedByRandom(Math.PI / 2);
                                Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, speed,
                                    ModContent.ProjectileType<TimberHook2>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 0f, Main.myPlayer, NPC.whoAmI);
                            }
                        }
                    }
                    else
                    {
                        NPC.velocity *= 0.9f;
                    }

                    if (++NPC.ai[1] > 300)
                    {
                        NPC.TargetClosest();
                        NPC.ai[0]++;
                        NPC.ai[1] = 0;
                        NPC.ai[2] = 0;
                        NPC.ai[3] = 0;
                        NPC.netUpdate = true;
                    }
                    break;

                case 8: //electrify chains
                    NPC.velocity = Vector2.Zero;

                    if (NPC.ai[1] == 0)
                    {
                        Terraria.Audio.SoundEngine.PlaySound(SoundID.Roar, NPC.Center, 0);
                    }

                    if (++NPC.ai[1] > 120)
                    {
                        NPC.TargetClosest();
                        NPC.ai[0]++;
                        NPC.ai[1] = 0;
                        NPC.ai[2] = 0;
                        NPC.ai[3] = 0;
                        NPC.netUpdate = true;
                    }
                    break;

                case 9:
                    goto case 0;

                case 10:
                    goto case 3;

                case 11:
                    goto case 0;

                case 12: //noah snowballs
                    targetPos = player.Center;
                    targetPos.X += player.velocity.X * 45f;
                    targetPos.Y -= 200;
                    Movement(targetPos, 0.5f, 32f);
                    
                    if (++NPC.ai[2] > 5 && NPC.ai[1] < 420)
                    {
                        NPC.ai[2] = 0;

                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            int damage = NPC.ai[1] > 120 ? FargoSoulsUtil.ScaledProjectileDamage(NPC.damage, 4f / 4 ): 0;
                            for (int i = -2; i <= 2; i++)
                            {
                                Vector2 speed = new Vector2(5f * i, -20f);
                                Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, speed, ModContent.ProjectileType<TimberSnowball2>(), damage, 0f, Main.myPlayer, NPC.target, NPC.whoAmI);
                            }
                        }
                    }

                    if (++NPC.ai[1] > 510)
                    {
                        NPC.TargetClosest();
                        NPC.ai[0]++;
                        NPC.ai[1] = 0;
                        NPC.ai[2] = 0;
                        NPC.ai[3] = 0;
                        NPC.netUpdate = true;
                    }
                    break;

                default:
                    NPC.ai[0] = 0;
                    goto case 0;
            }
        }

        private void Movement(Vector2 targetPos, float speedModifier, float cap = 12f)
        {
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
            if (Math.Abs(NPC.velocity.X) > cap)
                NPC.velocity.X = cap * Math.Sign(NPC.velocity.X);
            if (Math.Abs(NPC.velocity.Y) > cap)
                NPC.velocity.Y = cap * Math.Sign(NPC.velocity.Y);
        }

        public override void FindFrame(int frameHeight)
        {
            if (++NPC.frameCounter > 3)
            {
                NPC.frameCounter = 0;
                NPC.frame.Y += frameHeight;
                if (NPC.frame.Y >= frameHeight * Main.npcFrameCount[NPC.type])
                    NPC.frame.Y = 0;
            }
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            if (FargoSoulsWorld.EternityMode)
                target.AddBuff(ModContent.BuffType<Buffs.Masomode.Guilty>(), 600);
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            if (NPC.life <= 0)
            {
                Vector2 pos = NPC.position + new Vector2(Main.rand.NextFloat(NPC.width), Main.rand.NextFloat(NPC.height));
                if (!Main.dedServ)
                    Gore.NewGore(NPC.GetSource_FromThis(), pos, NPC.velocity, ModContent.Find<ModGore>(Mod.Name, "TimberGore1").Type, NPC.scale);
                pos = NPC.position + new Vector2(Main.rand.NextFloat(NPC.width), Main.rand.NextFloat(NPC.height));
                if (!Main.dedServ)
                    Gore.NewGore(NPC.GetSource_FromThis(), pos, NPC.velocity, ModContent.Find<ModGore>(Mod.Name, "TimberGore2").Type, NPC.scale);
            }
        }

        public override void BossLoot(ref string name, ref int potionType)
        {
            potionType = ItemID.SuperHealingPotion;
        }

        public override void OnKill()
        {
            NPC.SetEventFlagCleared(ref FargoSoulsWorld.downedChampions[(int)FargoSoulsWorld.Downed.TimberChampion], -1);
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            //npcLoot.Add(new ChampionEnchDropRule(ModContent.ItemType<Items.Accessories.Forces.TimberForce>()));
            npcLoot.Add(new ChampionEnchDropRule(new int[] {
                ModContent.ItemType<WoodEnchant>(),
                ModContent.ItemType<BorealWoodEnchant>(),
                ModContent.ItemType<RichMahoganyEnchant>(),
                ModContent.ItemType<EbonwoodEnchant>(),
                ModContent.ItemType<ShadewoodEnchant>(),
                ModContent.ItemType<PalmWoodEnchant>(),
                ModContent.ItemType<PearlwoodEnchant>()
            }));
        }

        public override void BossHeadSpriteEffects(ref SpriteEffects spriteEffects)
        {
            spriteEffects = NPC.direction < 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Npc[NPC.type].Value;
            Texture2D texture2D14 = FargowiltasSouls.Instance.Assets.Request<Texture2D>("NPCs/Champions/TimberChampionHead_Trail", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            Texture2D texture2D15 = FargowiltasSouls.Instance.Assets.Request<Texture2D>("NPCs/Champions/TimberChampionHead_Glow", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            //int num156 = Terraria.GameContent.TextureAssets.Npc[NPC.type].Value.Height / Main.npcFrameCount[NPC.type]; //ypos of lower right corner of sprite to draw
            //int y3 = num156 * NPC.frame.Y; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = NPC.frame;//new Rectangle(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;

            SpriteEffects effects = NPC.spriteDirection < 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            for (int i = 0; i < NPCID.Sets.TrailCacheLength[NPC.type]; i++)
            {
                Color color27 = Color.White * 0.5f;
                color27 *= (float)(NPCID.Sets.TrailCacheLength[NPC.type] - i) / NPCID.Sets.TrailCacheLength[NPC.type];
                Vector2 value4 = NPC.oldPos[i];
                float num165 = NPC.rotation; //NPC.oldRot[i];
                Main.EntitySpriteDraw(texture2D14, value4 + NPC.Size / 2f - screenPos + new Vector2(0, NPC.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color27, num165, origin2, NPC.scale, effects, 0);
            }

            Main.EntitySpriteDraw(texture2D13, NPC.Center - screenPos + new Vector2(0f, NPC.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), NPC.GetAlpha(drawColor), NPC.rotation, origin2, NPC.scale, effects, 0);
            Main.EntitySpriteDraw(texture2D15, NPC.Center - screenPos + new Vector2(0f, NPC.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Color.White, NPC.rotation, origin2, NPC.scale, effects, 0);
            return false;
        }
    }
}
