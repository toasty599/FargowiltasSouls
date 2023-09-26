
using System;
using System.IO;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System.Collections.Generic;
using Terraria.DataStructures;
using FargowiltasSouls.Content.Buffs.Masomode;
using Terraria.GameContent.Bestiary;
using Terraria.Audio;
using FargowiltasSouls.Content.Items.BossBags;
using FargowiltasSouls.Content.Items.Weapons.Challengers;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent.ItemDropRules;
using Terraria.Graphics.Shaders;
using FargowiltasSouls.Content.Buffs.Boss;
using FargowiltasSouls.Content.Projectiles;
using Color = Microsoft.Xna.Framework.Color;
using Rectangle = Microsoft.Xna.Framework.Rectangle;
using FargowiltasSouls.Core.Systems;
using FargowiltasSouls.Content.Projectiles.Deathrays;
using FargowiltasSouls.Content.Items.Summons;
using FargowiltasSouls.Content.Items.Placables.Trophies;

namespace FargowiltasSouls.Content.Bosses.BanishedBaron
{
	[AutoloadBossHead]
    public class BanishedBaron : ModNPC
    {
        Player player => Main.player[NPC.target];
        //TODO: re-enable boss checklist compat, localizationhelper addSpawnInfo

        SoundStyle BaronRoar = new SoundStyle("FargowiltasSouls/Assets/Sounds/BaronRoar");
        SoundStyle BaronYell = new SoundStyle("FargowiltasSouls/Assets/Sounds/BaronYell");

        #region Variables
        private enum StateEnum //ALL states
        {
            Opening,
            Phase2Transition,
            P1BigNuke,
            P1RocketStorm,
            P1SurfaceMines,
            P1FadeDash,
            P1SineSwim,
            P1Whirlpool,
            P2PredictiveDash,
            P2CarpetBomb,
            P2RocketStorm,
            P2SpinDash,
            P2MineFlurry,
            P2Whirlpool,
            P2LaserSweep,
            Swim,
        }

        private static List<int> P1Attacks = new List<int>() //these are randomly chosen attacks in p1
        {
            (int)StateEnum.P1BigNuke,
            (int)StateEnum.P1RocketStorm,
            (int)StateEnum.P1SurfaceMines,
            (int)StateEnum.P1FadeDash,
            (int)StateEnum.P1SineSwim,
            (int)StateEnum.P1Whirlpool
        };
        private static List<int> P2Attacks = new List<int>() //these are randomly chosen attacks in p2
        {
            (int)StateEnum.P2PredictiveDash,
            (int)StateEnum.P2CarpetBomb,
            (int)StateEnum.P2RocketStorm,
            (int)StateEnum.P2SpinDash,
            (int)StateEnum.P2MineFlurry,
            (int)StateEnum.P2Whirlpool,
            (int)StateEnum.P2LaserSweep
        };

        private bool Attacking = true;
        bool HitPlayer = true;
        private int Trail = 0;

        private int Frame = 0;
        private int Phase = 1;
        private int Anim = 0;

        public int ArenaProjID = -1;

        public const int MaxWhirlpools = 40;

        private List<int> availablestates = new List<int>(0);

        private Vector2 LockVector1 = Vector2.Zero;
        private Vector2 LockVector2 = Vector2.Zero;

        //NPC.ai[] overrides
        public ref float Timer => ref NPC.ai[0];
        public ref float State => ref NPC.ai[1];
        public ref float AI2 => ref NPC.ai[2];
        public ref float AI3 => ref NPC.ai[3];
        public ref float AI4 => ref NPC.localAI[0];
        #endregion
        #region Standard
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 19;
            NPCID.Sets.TrailCacheLength[NPC.type] = 10;
            NPCID.Sets.TrailingMode[NPC.type] = 3;
            NPCID.Sets.MPAllowedEnemies[Type] = true;

            NPCID.Sets.BossBestiaryPriority.Add(NPC.type);

            NPC.AddDebuffImmunities(new List<int>
            {
                BuffID.Confused,
                BuffID.Chilled,
                BuffID.Suffocation,
                ModContent.BuffType<LethargicBuff>(),
                ModContent.BuffType<ClippedWingsBuff>()
            });
            NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, new NPCID.Sets.NPCBestiaryDrawModifiers(0)
            {
                Rotation = MathHelper.Pi,
                Position = Vector2.UnitX * 60
            });
        }
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Ocean,
                new FlavorTextBestiaryInfoElement($"Mods.FargowiltasSouls.Bestiary.{Name}")
            });
        }
        public override void SetDefaults()
        {
            NPC.aiStyle = -1;
            NPC.lifeMax = 30500;
            NPC.defense = 15;
            NPC.damage = 69;
            NPC.knockBackResist = 0f;
            NPC.width = 194;
            NPC.height = 132;
            NPC.boss = true;
            NPC.lavaImmune = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.HitSound = SoundID.NPCHit4;
            NPC.DeathSound = new SoundStyle("FargowiltasSouls/Assets/Sounds/BaronDeath");
            NPC.alpha = 255;

            Music = MusicID.DukeFishron; 
            SceneEffectPriority = SceneEffectPriority.BossLow;

            NPC.value = Item.buyPrice(0, 2);

        }
        public override Color? GetAlpha(Color drawColor)
        {
            if (NPC.IsABestiaryIconDummy)
            {
                // This is required because we have NPC.alpha = 255, in the bestiary it would look transparent
                return NPC.GetBestiaryEntryColor();
            }
            return base.GetAlpha(drawColor);
        }
        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * balance);
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(NPC.localAI[0]);
            writer.Write7BitEncodedInt(ArenaProjID);
            writer.Write7BitEncodedInt(Phase);
            writer.WriteVector2(LockVector1);
            writer.WriteVector2(LockVector2);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            NPC.localAI[0] = reader.ReadSingle();
            ArenaProjID = reader.Read7BitEncodedInt();
            Phase = reader.Read7BitEncodedInt();
            LockVector1 = reader.ReadVector2();
            LockVector2 = reader.ReadVector2();
        }
        #endregion
        #region Overrides
        #region Hitbox
        public override bool CanHitPlayer(Player target, ref int CooldownSlot)
        {
            if (HitPlayer)
            {
                Vector2 boxPos = target.position;
                Vector2 boxDim = target.Size;
                return Collides(boxPos, boxDim);
            }
            return false;
        }
        public override bool CanHitNPC(NPC target)
        {
            if (HitPlayer)
            {
                Vector2 boxPos = target.position;
                Vector2 boxDim = target.Size;
                return Collides(boxPos, boxDim);
            }
            return false;
        }
        public bool Collides(Vector2 boxPos, Vector2 boxDim)
        {
            //circular hitbox-inator
            Vector2 ellipseDim = NPC.Size;
            Vector2 ellipseCenter = NPC.position + 0.5f * new Vector2(NPC.width, NPC.height);

            float x = 0f; //ellipse center
            float y = 0f; //ellipse center
            if (boxPos.X > ellipseCenter.X)
            {
                x = boxPos.X - ellipseCenter.X; //left corner
            }
            else if (boxPos.X + boxDim.X < ellipseCenter.X)
            {
                x = boxPos.X + boxDim.X - ellipseCenter.X; //right corner
            }
            if (boxPos.Y > ellipseCenter.Y)
            {
                y = boxPos.Y - ellipseCenter.Y; //top corner
            }
            else if (boxPos.Y + boxDim.Y < ellipseCenter.Y)
            {
                y = boxPos.Y + boxDim.Y - ellipseCenter.Y; //bottom corner
            }
            float a = ellipseDim.X / 2f;
            float b = ellipseDim.Y / 2f;

            return x * x / (a * a) + y * y / (b * b) < 1; //point collision detection
        }
        #endregion
        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position) => !(Timer < 90+50 && State == (int)StateEnum.P1FadeDash);

        public override void BossHeadSlot(ref int index)
        {
            if ((Timer < 90 + 50 && State == (int)StateEnum.P1FadeDash))
            {
                index = -1;
            }
            else
            {
                index = NPCID.Sets.BossHeadTextures[Type];
            }
        }
        public override bool? CanFallThroughPlatforms() => true;
        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            target.AddBuff(BuffID.Bleeding, 60 * 10);
            if (!WorldSavingSystem.EternityMode)
            {
                target.AddBuff(BuffID.Rabies, 60 * 5);
                return;
            }
            target.AddBuff(BuffID.Rabies, 60 * 10);
            target.FargoSouls().MaxLifeReduction += 50;
            target.AddBuff(ModContent.BuffType<OceanicMaulBuff>(), 60 * 30);
        }
        public override bool CheckDead()
        {
            return base.CheckDead();
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D bodytexture = Terraria.GameContent.TextureAssets.Npc[NPC.type].Value;
            Vector2 drawPos = NPC.Center - screenPos;
            float rot = NPC.rotation + (NPC.direction == 1 ? 0 :MathHelper.Pi);
            int currentFrame = NPC.frame.Y / (bodytexture.Height / Main.npcFrameCount[NPC.type]);
            SpriteEffects flip = NPC.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;


            for (int i = 0; i < Math.Min(Trail, NPCID.Sets.TrailCacheLength[NPC.type]); i++) //math.min to safeguard against uncached trail
            {
                float oldrot = NPC.oldRot[i] + (NPC.direction == 1 ? 0 : MathHelper.Pi);
                Vector2 value4 = NPC.oldPos[i];
                int oldFrame = Frame;
                Rectangle oldRectangle = new Rectangle(0, oldFrame * bodytexture.Height / Main.npcFrameCount[NPC.type], bodytexture.Width, bodytexture.Height / Main.npcFrameCount[NPC.type]);
                DrawData oldGlow = new DrawData(bodytexture, value4 + NPC.Size / 2f - screenPos + new Vector2(0, NPC.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(oldRectangle), NPC.GetAlpha(drawColor) * (0.5f / i), oldrot, new Vector2(bodytexture.Width / 2, bodytexture.Height / 2 / Main.npcFrameCount[NPC.type]), NPC.scale, flip, 0);
                GameShaders.Misc["LCWingShader"].UseColor(Color.Blue).UseSecondaryColor(Color.Black);
                GameShaders.Misc["LCWingShader"].Apply(oldGlow);
                oldGlow.Draw(spriteBatch);
            }

            spriteBatch.Draw(origin: new Vector2(bodytexture.Width / 2, bodytexture.Height / 2 / Main.npcFrameCount[NPC.type]), texture: bodytexture, position: drawPos, sourceRectangle: NPC.frame, color: NPC.GetAlpha(drawColor), rotation: rot, scale: NPC.scale, effects: flip, layerDepth: 0f);
            return false;
        }

        float AnimationSpeed = 1f;
        public override void FindFrame(int frameHeight)
        {
            //FRAMES OF ANIMATIONS:
            //P1 Normal fly loop: 0-5 (Anim 0)
            //P1 Open mouth: 6-7 (Anim 1)
            //Unused: 8-10
            //P2 Normal fly loop: 11-16 (Anim 0)
            //P2 Open mouth: 17-18 (Anim 1)
            double fpf =  60 / (10 * AnimationSpeed); //  60/fps
            int StartFrame = 0;
            int EndFrame = 5;
            switch (Phase)
            {
                case 1:
                    switch (Anim)
                    {
                        case 0:
                            //no change required, default values
                            break;
                        case 1:
                            StartFrame = 6;
                            EndFrame = 7;
                            break;
                    }
                    break;
                case 2:
                    switch (Anim)
                    {
                        case 0:
                            StartFrame = 11;
                            EndFrame = 16;
                            break;
                        case 1:
                            StartFrame = 17;
                            EndFrame = 18;
                            break;
                    }
                    break;
            }
            NPC.spriteDirection = NPC.direction;
            if (++NPC.frameCounter >= fpf)
            {
                if (++Frame > EndFrame || Frame < StartFrame)
                {
                    Frame = StartFrame;
                }
                NPC.frameCounter = 0;
            }
            NPC.frame.Y = frameHeight * Frame;
        }
        public override void OnKill()
        {
            NPC.SetEventFlagCleared(ref WorldSavingSystem.downedBoss[(int)WorldSavingSystem.Downed.BanishedBaron], -1);
        }
        public override void BossLoot(ref string name, ref int potionType)
        {
            potionType = ItemID.GreaterHealingPotion;
        }
        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life <= 0)
            {
                for (int i = 1; i <= 4; i++)
                {
                    Vector2 pos = NPC.position + new Vector2(Main.rand.NextFloat(NPC.width), Main.rand.NextFloat(NPC.height));
                    if (!Main.dedServ)
                        Gore.NewGore(NPC.GetSource_FromThis(), pos, NPC.velocity, ModContent.Find<ModGore>(Mod.Name, $"BaronGore{i}").Type, NPC.scale);
                }
            }
        }
        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            //TODO: Add loot
            npcLoot.Add(ItemDropRule.BossBag(ModContent.ItemType<BanishedBaronBag>()));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<BaronTrophy>(), 10));

            //npcLoot.Add(ItemDropRule.MasterModeCommonDrop(ModContent.ItemType<BanishedBaronRelic>()));

            LeadingConditionRule rule = new LeadingConditionRule(new Conditions.NotExpert());
            rule.OnSuccess(ItemDropRule.OneFromOptions(1, ModContent.ItemType<TheBaronsTusk>(), ModContent.ItemType<RoseTintedVisor>(), ModContent.ItemType<NavalRustrifle>(), ModContent.ItemType<DecrepitAirstrikeRemote>()));
            rule.OnSuccess(ItemDropRule.Common(5003, 1, 1, 5)); //seaside crate
            rule.OnSuccess(ItemDropRule.OneFromOptions(3, ItemID.Sextant, ItemID.WeatherRadio, ItemID.FishermansGuide));
            rule.OnSuccess(ItemDropRule.Common(ItemID.FishingBobber, 4, 1, 1));
            rule.OnSuccess(ItemDropRule.Common(ItemID.FishingPotion, 3, 2, 5));
            rule.OnSuccess(ItemDropRule.Common(ItemID.SonarPotion, 2, 2, 5));
            rule.OnSuccess(ItemDropRule.Common(ItemID.CratePotion, 5, 2, 5));
            rule.OnSuccess(ItemDropRule.Common(ItemID.GoldenBugNet, 50, 1, 1));
            rule.OnSuccess(ItemDropRule.Common(ItemID.FishHook, 50, 1, 1));
            rule.OnSuccess(ItemDropRule.Common(ItemID.GoldenFishingRod, 150, 1, 1));

            npcLoot.Add(rule);
        }

        #endregion
        #region AI
        public override void AI()
        {
            //Defaults
            NPC.noTileCollide = 
                Phase == 2 || 
                WorldSavingSystem.MasochistModeReal || 
                Collision.SolidCollision(NPC.position + NPC.Size / 10, (int)(NPC.width * 0.9f), (int)(NPC.height * 0.9f)) || 
                !Collision.CanHitLine(NPC.position, NPC.width, NPC.height, player.position, player.width, player.height); 
            //no tile collide in p2, tile collide in p1 except if in tiles, or if obstructed

            NPC.defense = NPC.defDefense;
            NPC.direction = NPC.spriteDirection = NPC.rotation.ToRotationVector2().X > 0 ? 1 : -1;

            if (HitPlayer && !(Timer < 90 && State == (int)StateEnum.P1FadeDash))
            {
                Lighting.AddLight(NPC.Center, TorchID.Pink);
            }
            
            if (!AliveCheck(player))
                return;

            if (State == 0 && Timer == 0) //opening
            {
                NPC.position = player.Center + new Vector2(Math.Sign(player.Center.X - Main.spawnTileX) * 1400, -100) - NPC.Size / 2;
                NPC.rotation = NPC.DirectionTo(player.Center).ToRotation();
                NPC.velocity = Vector2.Zero;
            }
            //Phase 1/2 specific passive attributes
            switch (Phase)
            {
                case 1:
                    if (State != (float)StateEnum.Opening || Timer >= 60) //also do it during opening but only after 1 sec
                    {
                        //inflict all nearby permanently with Please Go Underwater debuff that PRESSES them down
                        if (NPC.Distance(Main.LocalPlayer.Center) < 2000)
                        {
                            Main.LocalPlayer.AddBuff(ModContent.BuffType<BaronsBurdenBuff>(), 1);
                        }
                    }
                    if (Wet() && NPC.velocity.Length() > 0)
                    {
                        //stream of water dust behind
                        Dust.NewDust(NPC.Center - NPC.rotation.ToRotationVector2() * new Vector2(NPC.width / 2, 0) - new Vector2(5, 5), 10, 10, DustID.Water, -NPC.velocity.X, -NPC.velocity.Y);
                    }
                    break;
                case 2:
                    int type = ModContent.ProjectileType<BaronArenaWhirlpool>();
                    if (ArenaProjID == -1 && Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        ArenaProjID = Projectile.NewProjectile(NPC.GetSource_FromThis(), player.Center, Vector2.Zero, type, FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 3f, Main.myPlayer, NPC.whoAmI, 2);
                        
                    }
                    NPC.netUpdate = true;
                    break;
            }
            //Normal looping attack AI

            if (Attacking) //Phases and random attack choosing
            {
                switch (State) //Attack Choices
                {
                    case (float)StateEnum.Opening:
                        Opening();
                        break;
                    case (float)StateEnum.Phase2Transition:
                        Phase2Transition();
                        break;
                    case (float)StateEnum.Swim:
                        Swim();
                        break;
                    case (float)StateEnum.P1BigNuke:
                        P1BigNuke();
                        break;
                    case (float)StateEnum.P1RocketStorm:
                        if (!Main.expertMode)
                        {
                            goto default;
                        }
                        else
                        {
                            P1RocketStorm();
                        }
                        break;
                    case (float)StateEnum.P1SurfaceMines:
                        P1SurfaceMines();
                        break;
                    case (float)StateEnum.P1FadeDash:
                        P1FadeDash();
                        break;
                    case (float)StateEnum.P1SineSwim:
                        P1SineSwim();
                        break;
                    case (float)StateEnum.P1Whirlpool:
                        if (!WorldSavingSystem.EternityMode)
                        {
                            goto default;
                        }
                        else
                        {
                            P1Whirlpool();
                        }
                        break;
                    case (float)StateEnum.P2PredictiveDash:
                        P2PredictiveDash();
                        break;
                    case (float)StateEnum.P2CarpetBomb:
                        P2CarpetBomb();
                        break;
                    case (float)StateEnum.P2RocketStorm:
                        if (!Main.expertMode)
                        {
                            goto default;
                        }
                        else
                        {
                            P2RocketStorm();
                        }
                        break;
                    case (float)StateEnum.P2SpinDash:
                        if (!Main.expertMode)
                        {
                            goto default;
                        }
                        else
                        {
                            P2SpinDash();
                        }
                        break;
                    case (float)StateEnum.P2MineFlurry:
                        P2MineFlurry();
                        break;
                    case (float)StateEnum.P2Whirlpool:
                        if (!WorldSavingSystem.EternityMode)
                        {
                            goto default;
                        }
                        else
                        {
                            P2Whirlpool();
                        }
                        break;
                    case (float)StateEnum.P2LaserSweep:
                        P2LaserSweep();
                        break;
                    default:
                        StateReset();
                        break;
                }
            }
            Timer++;
        }
        bool Wet(Vector2? pos = null) //small helper
        {
            if (pos == null)
            {
                pos = NPC.position;
            }
            return Collision.WetCollision((Vector2)pos, NPC.width, NPC.height);
        }
        #endregion
        #region States
        void Opening()
        {
            HitPlayer = false;
            Anim = 1;
            //NPC.rotation = (float)(Math.Sin(MathHelper.ToRadians(Timer) * 16) * MathHelper.Pi/24);
            if (LockVector1 == Vector2.Zero)
            {
                for (int i = 0; i < Main.maxProjectiles; i++)
                {
                    Projectile p = Main.projectile[i];
                    if (p != null && p.active && p.type == ModContent.ProjectileType<MechLureProjectile>())
                    {
                        LockVector1 = p.Center;
                        break;
                    }
                }
                if (LockVector1 == Vector2.Zero) //no lure found
                {
                    LockVector1 = player.Center;
                }
            }
            if (NPC.alpha > 0)
            {
                NPC.alpha -= 2;
            }
            else
            {
                NPC.alpha = 0;
            }
            if (LockVector1 != Vector2.Zero)
            {
                RotateTowards(LockVector1, 1.5f);
                NPC.velocity = (LockVector1 - NPC.Center) * (Timer / 90f) * 0.4f;
            }
            
            if (Timer == 60)
            {
                SoundEngine.PlaySound(SoundID.Item21, NPC.Center);
                SoundEngine.PlaySound(SoundID.Item92, NPC.Center);
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<GlowRing>(), 0, 0f, Main.myPlayer, NPC.whoAmI, -24);
                }
                if (WorldSavingSystem.EternityMode && !WorldSavingSystem.DownedBoss[(int)WorldSavingSystem.Downed.BanishedBaron] && Main.netMode != NetmodeID.MultiplayerClient)
                    Item.NewItem(NPC.GetSource_Loot(), Main.player[NPC.target].Hitbox, ModContent.ItemType<MechLure>());
            }
            if (Timer > 90)
            {
                HitPlayer = true;
                Anim = 0;
                StateReset();
            }
        }
        void Phase2Transition()
        {
            
            if (Timer == 1)
            {
                SoundEngine.PlaySound(new SoundStyle("FargowiltasSouls/Assets/Sounds/BaronHit"), NPC.Center);
                if (!Main.dedServ)
                    Main.LocalPlayer.FargoSouls().Screenshake = 100;

                HitPlayer = false;

            }
            if (Timer < 90)
            {
                Vector2 up = NPC.Center + new Vector2(3 * Math.Sign(NPC.DirectionTo(player.Center).X), -10);
                RotateTowards(up, 5f);
                if (Main.LocalPlayer.wet)
                {
                    Main.LocalPlayer.velocity.Y -= 0.05f;
                }
            }
            if (Timer == 90)
            {
                NPC.velocity = NPC.rotation.ToRotationVector2() * 20;
                SoundEngine.PlaySound(BaronYell, NPC.Center);
                Phase = 2;
                FargoSoulsUtil.ClearHostileProjectiles(2, NPC.whoAmI);

                Music = ModLoader.TryGetMod("FargowiltasMusic", out Mod musicMod) ? MusicLoader.GetMusicSlot(musicMod, "Assets/Music/Baron") : MusicID.Boss2;
            }
            if (Timer > 90)
            {
                if (!Wet() || Timer > 90 + 60)
                {
                    NPC.velocity *= 0.95f;
                    if (NPC.velocity.Length() < 0.1f)
                    {
                        HitPlayer = true;
                        availablestates.Clear();
                        StateReset();

                    }
                }
                if (Main.LocalPlayer.wet && Main.LocalPlayer.velocity.Y > -30)
                {
                    Main.LocalPlayer.velocity.Y -= 2.5f;
                    Main.LocalPlayer.position.Y -= 8f;
                }
            }
            
        }
        #region Phase 1 Attacks
        void Swim()
        {
            void Thirsty()
            {
                int x = (int)NPC.Center.X / 16;
                int y = (int)NPC.Center.Y / 16;
                Tile tile = Main.tile[x, y];

                if (tile.LiquidAmount > 0)
                {
                    if (tile.LiquidType == LiquidID.Water)
                    {
                        tile.LiquidAmount = 0;
                        CombatText.NewText(NPC.Hitbox, Color.Blue, "slurp");
                        if (Main.netMode == NetmodeID.Server)
                        {
                            NetMessage.sendWater(x, y);
                            NetMessage.SendTileSquare(-1, x, y, 1);
                        }
                        else
                        {
                            WorldGen.SquareTileFrame(x, y, true);
                        }
                    }
                }
            }
            const int Distance = 400;
            if (Timer == 1)
            {
                
                if (Collision.SolidCollision(NPC.position, NPC.width, NPC.height)) //check if originated in collision, set AI3 to negative
                {
                    AI3 = -2;
                    for (int i = 0; i < 30; i++) //max of 30 checks
                    {
                        LockVector1 = player.Center + Vector2.UnitX.RotatedByRandom(MathHelper.TwoPi) * Distance;
                        if (!Collision.SolidCollision(LockVector1 - NPC.Size / 2, NPC.width, NPC.height)) //if found valid spot, stop searching
                        {
                            break;
                        }
                    }
                }
                else
                {
                    LockVector1 = player.Center + player.DirectionTo(NPC.Center) * Distance;
                }

                if (Wet() && WorldSavingSystem.MasochistModeReal) //chug the ocean in masomode
                {
                    Thirsty();
                }
            }

            //bool NewCollision = (Collision.SolidCollision(NPC.position, NPC.width, NPC.height) && AI3 != -2); //if didn't originate in collision, 
            if (Vector2.Distance(NPC.Center, LockVector1) < 25 || Timer > 150 || (AI3 != -2 && AI3 != 0)/* || NewCollision*/) //if ai3 not 0 (default) or -2 (set when original collision)
            {

                AI3 = (AI3 + 1) * 3; //stays negative if set to -2 by checking original collision, otherwise goes positive
                NPC.velocity *= 0.935f;
                RotateTowards(player.Center, 3);
                if (NPC.velocity.Length() < 0.1f)
                {
                    if (Wet() && WorldSavingSystem.MasochistModeReal) //chug the ocean in masomode
                    {
                        Thirsty();
                    }
                    NPC.velocity = Vector2.Zero;
                    StateReset();
                }
            }
            else
            {
                if (Collision.SolidCollision(NPC.Center + NPC.velocity, NPC.width, NPC.height) && Timer < 135 && AI3 != -2)
                {
                    Timer = 135; //so it doesn't get stuck on tiles too long
                }
                Vector2 vectorToIdlePosition = LockVector1 - NPC.Center;
                float speed = AI3 >= 0 ? 20f : 10f; //less speed if originated in collision
                if (WorldSavingSystem.EternityMode && !WorldSavingSystem.MasochistModeReal) //maso speed is already very fast because ignore water
                {
                    speed *= 1.2f;
                }
                float inertia = AI3 >= 0 ? 20f : 10f;
                vectorToIdlePosition.Normalize();
                vectorToIdlePosition *= speed;
                NPC.velocity = (NPC.velocity * (inertia - 1f) + vectorToIdlePosition) / inertia;
                if (NPC.velocity == Vector2.Zero)
                {
                    NPC.velocity.X = -0.15f;
                    NPC.velocity.Y = -0.05f;
                }
                NPC.rotation = NPC.velocity.ToRotation();
            }
        }
        void P1BigNuke()
        {
            /*
            if (Timer == 1)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<BloomLine>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 0f, Main.myPlayer, 3, NPC.whoAmI);
            }
            */

            RotateTowards(player.Center, 2);
            if (Timer < 30)
                Anim = 1;
            else
                Anim = 0;
            if (Timer == 30)
            {
                SoundEngine.PlaySound(SoundID.Item61, NPC.Center);
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Vector2 vel = NPC.rotation.ToRotationVector2() * 10f;
                    AI2 = Main.rand.NextFloat(190, 220); //nuke duration
                    NPC.netUpdate = true;
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, vel, ModContent.ProjectileType<BaronNuke>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 0f, Main.myPlayer, AI2, player.whoAmI);
                }
            }
            if (Timer > AI2 + 10 && AI2 > 0)
            {
                Anim = 0;
                StateReset();
            }
        }
        void P1RocketStorm()
        {
            RotateTowards(player.Center, 3);
            if (Timer < 60 && Timer % 6 == 0 && Timer > 10)
            {
                Anim = 1;
                SoundEngine.PlaySound(SoundID.Item64, NPC.Center);
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    float progress = (Timer - 10) / 60;
                    float rotation = progress * (MathHelper.Pi / 6);
                    Vector2 vel = (NPC.rotation + ((MathHelper.PiOver2 + rotation) * -NPC.direction)).ToRotationVector2() * (10 * progress + 10);
                    float trackingPower = WorldSavingSystem.MasochistModeReal ? 1.5f : 1f;
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, vel, ModContent.ProjectileType<BaronRocket>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 0f, Main.myPlayer, 3, player.whoAmI, trackingPower); //ai2 is tracking power, above 1 is pseudo-predictive
                }
            }
            if (Timer > 60)
            {
                Anim = 0;
                StateReset();
            }
        }
        void P1SurfaceMines()
        {
            Anim = 1;
            if (Timer == 1)
            {
                LockVector1 = new Vector2(player.Center.X, NPC.Center.Y);
                AI3 = 1;
                SoundEngine.PlaySound(BaronYell, NPC.Center);
            }
            LockVector1.Y = NPC.Center.Y;
            RotateTowards(player.Center, 1);
            if (Timer > 10 && Timer % 30 == 0)
            {
                SoundEngine.PlaySound(SoundID.Item61, NPC.Center);
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Vector2 vel = NPC.rotation.ToRotationVector2() * AI3;
                    vel = new Vector2(vel.Length() * NPC.direction, 0);
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, vel, ModContent.ProjectileType<BaronMine>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 0f, Main.myPlayer, 1, player.whoAmI);
                }
                AI3 += 8;
            }
            if (Timer > 160)
            {
                Anim = 0;
                StateReset();
            }
        }
        void P1FadeDash()
        {
            int ReactionTime = WorldSavingSystem.EternityMode ? 30 : 50;
            if (Timer == 1)
            {
                LockVector1 = player.Center + (Vector2.UnitX * Main.rand.Next(500, 600)).RotatedByRandom(MathHelper.TwoPi);
                NPC.netUpdate = true;
            }
            if (Timer < 90 && Timer % 5 == 0) //keep looking for valid spot every 5 ticks, even when spot is invalidated
            {
                bool collide = false;
                for (float i = 0; i < 0.8f; i += 0.1f) //don't check the entire way, otherwise every spot is invalid if you're standing on ground
                {
                    if (Collision.SolidCollision(LockVector1 - NPC.Size / 2 + ((player.Center - LockVector1) * i), NPC.width, NPC.height)) //if can dash to player at arrival spot
                    {
                        collide = true;
                        break;
                    }

                }
                if (collide)
                {
                    LockVector1 = player.Center + (Vector2.UnitX * Main.rand.Next(600, 700)).RotatedByRandom(MathHelper.TwoPi); //find new spot
                    NPC.netUpdate = true;
                }
            }
            if (Timer < 60)
            {
                if (NPC.buffType[0] != 0) //cleanse all buffs
                    NPC.DelBuff(0);

                RotateTowards(player.Center, 2);
                if (NPC.velocity.Length() < 1)
                {
                    NPC.velocity += NPC.rotation.ToRotationVector2() * 0.02f;
                    NPC.velocity *= 1.03f;
                }
                NPC.velocity = NPC.rotation.ToRotationVector2() * NPC.velocity.Length();
                NPC.Opacity -= 1f / 50;
                if (NPC.Opacity < 0)
                    NPC.Opacity = 0;
                HitPlayer = false;
            }
            if (Timer == 60)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    
                    NPC.netUpdate = true;
                    NPC.rotation = NPC.DirectionTo(LockVector1).ToRotation();
                }
            }
            if (Timer > 60 && Timer < 90) //gone
            {
                if (NPC.buffType[0] != 0) //cleanse all buffs
                    NPC.DelBuff(0);

                NPC.rotation = NPC.DirectionTo(LockVector1).ToRotation();
                NPC.noTileCollide = true;
                NPC.Center = LockVector1;
                NPC.netUpdate = true;
                NPC.dontTakeDamage = true;
            }
            if (Timer == 90)
            {
                if (!Collision.SolidCollision(NPC.position, NPC.width, NPC.height))
                {
                    NPC.noTileCollide = false;
                }
                NPC.velocity = Vector2.Zero;
                NPC.rotation = NPC.DirectionTo(player.Center).ToRotation();
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center + NPC.rotation.ToRotationVector2() * NPC.width * 0.35f, Vector2.Zero, ModContent.ProjectileType<BaronEyeFlash>(), 0, 0, Main.myPlayer);
                }
            }
            if (Timer > 90 && Timer < 90 + ReactionTime)
            {
                RotateTowards(player.Center, 2);
                if (NPC.velocity.Length() < 3)
                {
                    NPC.velocity += NPC.rotation.ToRotationVector2() * 0.03f;
                    NPC.velocity *= 1.03f;
                }
                NPC.velocity = NPC.rotation.ToRotationVector2() * NPC.velocity.Length();
                NPC.Opacity += 1f / ReactionTime;
                if (NPC.Opacity > 1)
                    NPC.Opacity = 1;
                HitPlayer = true;
            }
            if (Timer == 90 + ReactionTime)
            {
                NPC.Opacity = 1;
                NPC.dontTakeDamage = false;
                SoundEngine.PlaySound(BaronRoar, NPC.Center);
                float baseSpeed = 45;
                float extraSpeed = (float)Math.Sqrt((player.Center - NPC.Center).Length()) / 1.5f;

                NPC.velocity = NPC.rotation.ToRotationVector2() * (baseSpeed + extraSpeed);
                
            }
            if (Timer > 90 + ReactionTime)
            {
                if (NPC.noTileCollide) //dash same speed if collision is off
                {
                    NPC.position -= NPC.velocity / 2;
                }
                NPC.dontTakeDamage = false; //here too for safety
                NPC.velocity *= 0.975f;

                if (NPC.velocity.Length() > 8 && WorldSavingSystem.EternityMode)
                {
                    int rocketTime = 11;
                    if (Timer % rocketTime == 0)
                    {
                        SoundEngine.PlaySound(SoundID.Item64, NPC.Center);
                        for (int i = -1; i < 2; i += 2)
                        {
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                Vector2 v = NPC.rotation.ToRotationVector2().RotatedBy(MathHelper.PiOver2 * i) * 1f;
                                Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center + (v * 20), v * 0.3f, ModContent.ProjectileType<BaronRocket>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 0f, Main.myPlayer, 4, player.whoAmI);
                            }
                        }
                    }
                }
            }
            if ((Timer == 90 + ReactionTime + 15 || Timer == 90 + ReactionTime + 20) && WorldSavingSystem.EternityMode)
            {
                SoundEngine.PlaySound(SoundID.Item64, NPC.Center);
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Vector2 vel = NPC.velocity;
                    float trackingPower = WorldSavingSystem.MasochistModeReal ? 1.5f : 1f;
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, vel, ModContent.ProjectileType<BaronRocket>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 0f, Main.myPlayer, 3, player.whoAmI, trackingPower); //ai2 is tracking power, above 1 is pseudo-predictive
                }
            }
            if (Timer > 90 + ReactionTime + 30 && NPC.velocity.Length() < 1.5f)
            {
                NPC.noTileCollide = false;
                StateReset();
            }
        }
        void P1SineSwim()
        {
            int Duration = WorldSavingSystem.MasochistModeReal ? 100 : WorldSavingSystem.EternityMode ? 150 : 180;
            const int Waves = 2;
            const int Ymax = 600;
            const int Xstart = 1000;

            float prog = Timer / Duration;

            NPC.noTileCollide = true;
            if (Timer == 1)
            {
                AI2 = Math.Sign(NPC.Center.X - player.Center.X);
                LockVector1 = player.Center + new Vector2(Xstart * AI2, 0);
                SoundEngine.PlaySound(BaronYell, NPC.Center);
                NPC.netUpdate = true;
            }
            if (AI3 == 0)
            {
                Vector2 target = LockVector1 + new Vector2(-AI2 * prog * (1.75f * Xstart), Ymax * (float)Math.Sin(MathHelper.TwoPi * prog * Waves));
                NPC.rotation = NPC.DirectionTo(target).ToRotation();
                NPC.velocity = NPC.DirectionTo(target) * NPC.Distance(target) / 1.2f;
                if (NPC.velocity.Length() > 20)
                {
                    NPC.velocity *= 20 / NPC.velocity.Length();
                }
                if (Timer == Math.Round(Duration * 0.33f) || Timer == Math.Round(Duration * 0.66f) || Timer == Math.Round(Duration * 0.99f))
                {
                    SoundEngine.PlaySound(SoundID.Item63, NPC.Center);
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Vector2 vel = (-NPC.rotation).ToRotationVector2() * 5;
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, vel, ModContent.ProjectileType<BaronMine>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 0f, Main.myPlayer, 2, player.whoAmI);
                    }
                }
                if (prog > 1)
                {
                    AI3 = 1;
                }
            }
            if (AI3 == 1)
            {
                NPC.velocity *= 0.96f;
            }
            if (NPC.velocity.Length() < 1f)
            {
                NPC.noTileCollide = false; //this is probably automatically set in AI anyway but just to make sure
                NPC.velocity = Vector2.Zero;
                StateReset();
            }
        }
        void P1Whirlpool()
        {
            NPC.noTileCollide = true;
            const int Height = 2000;
            if (Timer == 1)
            {
                const int checks = 20;
                for (int i = 0; i < checks; i++) //find approx surface level above player
                {
                    LockVector1 = player.Center - Vector2.UnitY * Height * (checks - i);
                    
                    if (Wet(LockVector1 + Vector2.UnitY * 2 * Height / checks)) //check 2 checks down
                    {
                        NPC.netUpdate = true;
                        break;
                    }
                }
                
            }
            if (Timer < 60)
            {
                HitPlayer = false;
                NPC.velocity = (LockVector1 - NPC.Center) * (Timer / 60) * 0.04f;
                RotateTowards(LockVector1, 2);
                if (!Wet(NPC.position + Vector2.UnitY * NPC.height / 4))
                {
                    NPC.velocity.Y = 0;
                }
                else
                {
                    NPC.velocity.Y += 0.25f;
                }
                if (NPC.velocity.LengthSquared() >= 30 * 30)
                {
                    NPC.velocity = Vector2.Normalize(NPC.velocity) * 30;
                }
            }
            else
            {
                HitPlayer = true;
                NPC.velocity = Vector2.Zero;
            }
            if (Timer > 50)
            {
                RotateTowards(NPC.Center - Vector2.UnitY.RotatedBy(MathHelper.ToRadians(1)), 16);
            }
            if (Timer == 60 && Main.netMode != NetmodeID.MultiplayerClient)
            {
                AnimationSpeed = 2f;
                Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center + Vector2.UnitY * ((NPC.height / 2) + 24), Vector2.Zero, ModContent.ProjectileType<BaronWhirlpool>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 0f, Main.myPlayer, NPC.whoAmI, MaxWhirlpools);
            }
            if (Timer >= 60 * 8)
            {
                AnimationSpeed = 1f;
                NPC.noTileCollide = false; //this is probably automatically set in AI anyway but just to make sure
                NPC.velocity = Vector2.Zero;
                StateReset();
            }
        }
        #endregion
        #region Phase 2 Attacks
        void P2PredictiveDash()
        {
            float PredictStr = 35;
            const int ReactTime = 72;
            if (Timer == 1)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<BloomLine>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 0f, Main.myPlayer, 2, NPC.whoAmI);
            }
            if (Timer == ReactTime - 5) //slight telegraph sound to be cute
            {
                SoundEngine.PlaySound(SoundID.MaxMana, NPC.Center);
            }
            if (Timer < ReactTime - 5) //stop aligning 5 frames before dashing
            {
                Vector2 target = player.Center + (Vector2.Normalize(NPC.Center - player.Center) * 500);
                if ((NPC.Distance(target) < 25 && AI3 == 0) || (Timer % 60 == 0 && AI3 != 0))
                {
                    AI3 = -1;
                    NPC.netUpdate = true;
                }

                if (AI3 == 0)
                {
                    if (NPC.velocity.Length() < 20 + (player.velocity.Length() / 2))
                    {
                        NPC.velocity += NPC.DirectionTo(target) * 0.02f;
                        NPC.velocity = NPC.DirectionTo(target) * NPC.velocity.Length() * 1.05f;
                    }
                    else
                    {
                        NPC.velocity = NPC.DirectionTo(target) * (20 + (player.velocity.Length() / 2));

                    }
                }
                else
                {
                    NPC.velocity = (target - NPC.Center) / 10;
                }

                LockVector1 = NPC.DirectionTo(player.Center + (player.velocity * PredictStr)) * PredictStr;

                RotateTowards(NPC.Center + LockVector1, 4);
            }
            if (Timer == ReactTime)
            {
                SoundEngine.PlaySound(BaronRoar, NPC.Center);
                Trail = 8;
                NPC.velocity = LockVector1;
                NPC.rotation = NPC.velocity.ToRotation();
                Vector2 uv = Vector2.Normalize(LockVector1);
                Vector2 lp = player.Center - NPC.Center;
                float lambda = Vector2.Dot(uv, lp);
                LockVector2 = NPC.Center + (uv * lambda);

                //below: instantly decelerate if dash is at large angle from player
                Vector2 PV = NPC.DirectionTo(player.Center);
                Vector2 LV = LockVector1;
                float anglediff = FargoSoulsUtil.RotationDifference(LV, PV);
                if (Math.Abs(anglediff) > MathHelper.PiOver2)
                {
                    AI3 = 1;
                }
            }
            if (Timer >= ReactTime && NPC.velocity.Length() > 5)
            {
                int rocketTime = (int)(8 * 20 / NPC.velocity.Length());
                if (Timer % rocketTime == 0)
                {
                    SoundEngine.PlaySound(SoundID.Item64, NPC.Center);
                    for (int i = -1; i < 2; i += 2)
                    {
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            Vector2 v = NPC.rotation.ToRotationVector2().RotatedBy(MathHelper.PiOver2 * i) * 1f;
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center + (v * 20), v * 0.3f, ModContent.ProjectileType<BaronRocket>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 0f, Main.myPlayer, 2, player.whoAmI);
                        }
                    }
                }
            }

            if (NPC.Distance(LockVector2) <= NPC.velocity.Length())
            {
                AI3 = 1;
            }
            if (AI3 == 1)
            {
                NPC.velocity *= 0.97f;
            }
            if (NPC.velocity.Length() < 0.5f && AI3 == 1)
            {
                Trail = 0;
                NPC.velocity = Vector2.Zero;
                StateReset();
            }
        }
        void P2CarpetBomb()
        {
            const int Xstart = 800;
            const int YMax = 500;
            const int YMin = 300;
            if (Timer == 1)
            {
                AI2 = Math.Sign(NPC.Center.X - player.Center.X);
                
            }
            if (AI3 == 0) //startup, reach starting point
            {
                float x = Xstart * AI2;
                LockVector1 = player.Center + new Vector2(x, -YMin);
                NPC.rotation = NPC.DirectionTo(LockVector1).ToRotation();
                if (NPC.velocity.Length() < 30 + player.velocity.Length())
                {
                    NPC.velocity += 0.5f * NPC.rotation.ToRotationVector2();
                    NPC.velocity *= 1.03f;
                }
                NPC.velocity = NPC.velocity.Length() * NPC.rotation.ToRotationVector2();
                if (NPC.Distance(LockVector1) < 25)
                {
                    AI3 = 1; //reached point, start attack
                }
            }
            if (AI3 > 0) //arc attack
            {
                const int ArcTime = 90;

                //funky real math i did on paper and translated to code

                Vector2 Curve(float p) //makes a smooth wide curve
                {
                    float x = AI2 * Xstart * (1 - (2 * p));
                    return new Vector2(x, (float)(-YMin - ((YMax - YMin) * Math.Sin(MathHelper.Pi * p))));
                }
                float prog = AI3 / ArcTime;
                
                Vector2 curve = Curve(prog);
                Vector2 dydx = Curve(prog + 0.00001f) - curve;
                NPC.rotation = dydx.ToRotation();
                LockVector1 = player.Center + curve; 
                NPC.velocity = LockVector1 - NPC.Center;
                AI3++;

                if ((AI3 % (ArcTime/6)) == 0)
                {
                    SoundEngine.PlaySound(SoundID.Item63, NPC.Center);
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Vector2 vel = new Vector2(0, Main.rand.Next(15, 20));
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, vel, ModContent.ProjectileType<BaronMine>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 0f, Main.myPlayer, 2, player.whoAmI);
                    }
                }

                if (prog >= 1)
                {
                    AI3 = -1; //end attack, start next part
                    SoundEngine.PlaySound(BaronYell, NPC.Center);
                }
            }
            if (AI3 < 0) //home into player after arc
            {
                
                if (NPC.Distance(player.Center) < 300 && AI3 < -30)
                {
                    AI2 = -2;
                }
                if (AI2 == -2)
                {
                    NPC.velocity *= 0.975f;
                }
                else
                {
                    float maxSpeed = 15 + (player.velocity.Length() / 2);
                    if (NPC.velocity.Length() < maxSpeed)
                    {
                        NPC.velocity += NPC.rotation.ToRotationVector2() * 0.02f;
                        NPC.velocity = NPC.rotation.ToRotationVector2() * NPC.velocity.Length() * 1.03f;
                    }
                    else
                    {
                        NPC.velocity = NPC.rotation.ToRotationVector2() * maxSpeed;
                    }
                    float rotSpeed = 2.5f;
                    RotateTowards(player.Center, rotSpeed);
                }
                AI3--;

            }
            if ((AI3 < -30 && NPC.velocity.Length() < 1.25) || AI3 < -220) //second condition is failsafe for circle cheese
            {
                NPC.velocity = Vector2.Zero;
                StateReset();
            }
        }
        void P2RocketStorm()
        {
            const int distance = 500;
            float rotModifier = 1;
            float minRot = 0.5f;
            if (Timer == 1)
            {
                AI2 = Math.Sign(NPC.Center.X - player.Center.X);
                LockVector1 = new Vector2(distance * AI2, 0);

            }

            RotateTowards(player.Center, 4);

            float rotation = MathHelper.ToRadians(AI3 + (minRot * Math.Sign(AI3)));
            LockVector1 = LockVector1.RotatedBy(rotation * rotModifier);

            Vector2 target = player.Center + LockVector1;

            if ((NPC.Distance(target) < 25 && AI3 == 0) || (Timer % 60 == 0 && AI3 != 0))
            {
                AI3 = Main.rand.NextFloat(-1, 1);
                NPC.netUpdate = true;
            }

            if (AI3 == 0)
            {
                if (NPC.velocity.Length() < 20 + (player.velocity.Length() / 2))
                {
                    NPC.velocity += NPC.DirectionTo(target) * 0.02f;
                    NPC.velocity = NPC.DirectionTo(target) * NPC.velocity.Length() * 1.05f;
                }
                else
                {
                    NPC.velocity = NPC.DirectionTo(target) * (20 + (player.velocity.Length() / 2));

                }
                if (Timer > 2)
                {
                    Timer = 2;
                }
            }
            else
            {
                NPC.velocity = (target - NPC.Center) / 10;
            }
            

            if (AI3 != 0 && Timer < 300)
            {

                bool volleyTime = (Timer > 40 && Timer < 100) || (Timer > 140 && Timer < 200) || (Timer > 240 && Timer < 300);
                if (volleyTime)
                {
                    Anim = 1;
                    if (Timer % 8 == 0)
                    {
                        SoundEngine.PlaySound(SoundID.Item64, NPC.Center);
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            int side = (Timer > 140 && Timer < 200) ? -1 : 1;
                            float speed = Main.rand.NextFloat(10, 25);
                            Vector2 vel = NPC.rotation.ToRotationVector2().RotatedBy(MathHelper.PiOver2 * side).RotatedByRandom(MathHelper.Pi / 8) * speed;
                            float trackingPower = WorldSavingSystem.MasochistModeReal ? 1 : 1;
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center + (Vector2.Normalize(vel) * 20), vel, ModContent.ProjectileType<BaronRocket>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 0f, Main.myPlayer, 1, player.whoAmI, trackingPower); //ai2 is tracking power
                        }
                    }
                }
                else
                {
                    Anim = 0;
                }
                //fire rockets
            }

            if (Timer > 300)
            {
                Anim = 0;
            }
            if (Timer > 310)
            {
                NPC.velocity = Vector2.Zero;
                StateReset();
            }
        }
        void P2SpinDash()
        {
            const int radius = 500;
            const float startRot = MathHelper.PiOver2 * 0.7f;
            float rotModifier = 3.5f;
            if (Timer == 1)
            {
                AI2 = Math.Sign(NPC.Center.X - player.Center.X);

            }
            if (AI3 == 0) //startup, reach starting point
            {

                LockVector1 = new Vector2(0, radius).RotatedBy(startRot * -AI2);
                Vector2 target = player.Center + LockVector1;
                NPC.rotation = NPC.DirectionTo(target).ToRotation();

                //velocity on rotation = radius * rotation per tick
                NPC.netUpdate = true;
                float rotVelocity = radius * MathHelper.ToRadians(3); //maximum value since random rotation velocity is not determined yet (could be done better by determining random before but would have to rework structure and i'm too lazy)
                if (NPC.velocity.Length() < rotVelocity + player.velocity.Length())
                {
                    NPC.velocity += 0.5f * NPC.rotation.ToRotationVector2();
                    NPC.velocity *= 1.03f;
                }
                NPC.velocity = NPC.velocity.Length() * NPC.rotation.ToRotationVector2();
                if (NPC.Distance(target) < 25)
                {
                    AI3 = Main.rand.NextFloat(0.2f, 0.4f); //reached point, start attack, decide random rotation during charge windup
                    NPC.netUpdate = true;
                }
            }
            if (AI3 > 0) //rotate a random bit below, and then dash
            {
                const int rotStart = 38;
                const int dashStart = 45;
                const int DashSpeed = 27;

                AI2 += Math.Sign(AI2); //internal timer, efficient variable use (?)
                
                Vector2 target = player.Center + LockVector1;
                


                if (Math.Abs(AI2) <= rotStart)
                {
                    NPC.velocity = (target - NPC.Center) / 10;
                    NPC.rotation = NPC.velocity.ToRotation();
                    if (Math.Abs(AI2) == rotStart)
                    {
                        SoundEngine.PlaySound(BaronYell, NPC.Center);
                    }
                }
                else //prepare for dash and dash
                {
                    const int reactTime = 20;
                    rotModifier = 0.025f; //rotate very slowly when preparing for dash
                    RotateTowards(player.Center, 6f);
                    if (Math.Abs(AI2) > dashStart + reactTime)
                    {
                        SoundEngine.PlaySound(BaronRoar, NPC.Center);
                        NPC.velocity = NPC.rotation.ToRotationVector2() * DashSpeed;
                        AI3 = -1;
                        AI2 = Math.Sign(AI2);
                    }
                }

                float rotation = MathHelper.ToRadians(AI3 * Math.Sign(AI2) * rotModifier);
                LockVector1 = LockVector1.RotatedBy(rotation);

            }
            const int end = -110;
            const int bulletStart = -36;
            if (AI3 < 0) //spin after dash
            {
                if (AI3 == -1)
                {
                    AI2 = Math.Sign(NPC.Center.X - player.Center.X);
                }
                AI3--;


                if (AI3 >= end)
                {
                    if (Math.Abs(AI2) < 60)
                    {
                        AI2 += Math.Sign(AI2) * 2;
                    }
                    NPC.velocity *= 0.995f;
                    NPC.velocity.Y += 0.2f;
                    NPC.rotation += -AI2 * MathHelper.Pi / 16 / 60;
                }
                else
                {
                    if (Math.Abs(AI2) > 4)
                    {
                        AI2 -= Math.Sign(AI2) * 2;
                    }

                    NPC.velocity *= 0.955f;
                    NPC.rotation += -AI2 * MathHelper.Pi / 16 / 60;
                }
                if (AI3 < bulletStart)
                {
                    Anim = 1;
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        float speed = Main.rand.NextFloat(8, 13);
                        Vector2 v = Main.rand.NextFloat(MathHelper.TwoPi).ToRotationVector2();
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center + v, v * speed, ModContent.ProjectileType<BaronScrap>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 0f, Main.myPlayer);
                    }
                }
            }
            if (AI3 < end - 20)
            {
                Anim = 0;
                NPC.velocity = Vector2.Zero;
                StateReset();
            }
        }
        void P2MineFlurry()
        {
            const int distance = 500;
            if (Timer == 1)
            {
                AI2 = Math.Sign(NPC.Center.X - player.Center.X);
                LockVector1 = new Vector2(distance * AI2, 0);

            }

            LockVector1 = new Vector2(distance * AI2, 0);
            Vector2 target = player.Center + LockVector1;

            if ((NPC.Distance(target) < 25 && AI3 == 0) || (Timer % 60 == 0 && AI3 != 0))
            {
                AI3 = 1;
            }

            if (AI3 == 0)
            {
                Timer--; //timer doesnt go up until you reach start position (i should probably do this on the other attacks too for the startup)
                RotateTowards(player.Center, 4);
                if (NPC.velocity.Length() < 20 + (player.velocity.Length() / 2))
                {
                    NPC.velocity += NPC.DirectionTo(target) * 0.02f;
                    NPC.velocity = NPC.DirectionTo(target) * NPC.velocity.Length() * 1.05f;
                }
                else
                {
                    NPC.velocity = NPC.DirectionTo(target) * (20 + (player.velocity.Length() / 2));

                }
            }
            else
            {
                NPC.velocity = (target - NPC.Center) / 10;
            }

            const int cd = 5;
            const int Flurry1Time = 20;
            const int Flurry2Time = 130;
            bool time = (Timer >= Flurry1Time && Timer <= Flurry1Time + (cd*6)) || (Timer >= Flurry2Time && Timer <= Flurry2Time + (cd*6));
            bool timeFirstHalf = (Timer >= Flurry1Time && Timer <= Flurry1Time + (cd * 3)) || (Timer >= Flurry2Time && Timer <= Flurry2Time + (cd * 3));
            if (time)
            {
                if (Timer == Flurry1Time) //choose side
                {
                    AI4 = Main.rand.NextBool() ? 1 : -1;
                    NPC.netUpdate = true;
                }
                if (Timer == Flurry2Time)
                {
                    AI4 = -AI4; //switch side
                    NPC.netUpdate = true;
                }
                Anim = 1;
                if (Timer == Flurry1Time + 3 || Timer == Flurry2Time + 3) //choose position and spawn telegraph for time until attack ticks
                {
                    int ExtraDistance = 0;
                    if (AI4 == Math.Sign(player.velocity.X))
                    {
                        ExtraDistance = (int)(player.velocity.X * 10);
                    }
                    LockVector2 = new Vector2(0, Main.rand.NextFloat(250 + ExtraDistance, 350 + ExtraDistance) * AI4);
                    NPC.netUpdate = true;
                    /*
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<BloomLine>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 0f, Main.myPlayer, 4, NPC.whoAmI);
                    */
                }
                RotateTowards(player.Center + LockVector2, 10);
                if (Timer % cd == cd-1 && !timeFirstHalf) //shoot
                {
                    SoundEngine.PlaySound(SoundID.Item63, NPC.Center);
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        float speed = Main.rand.Next(10, 38);
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, speed * NPC.rotation.ToRotationVector2(), ModContent.ProjectileType<BaronMine>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 0f, Main.myPlayer, 2, player.whoAmI);
                    }
                }
            }
            else
            {
                Anim = 0;
            }

            if (Timer > 320)
            {
                NPC.velocity = Vector2.Zero;
                StateReset();
            }
        }
        void P2Whirlpool()
        {
            Projectile arena = Main.projectile[ArenaProjID];
            bool arenaReal = arena != null && arena.active && arena.type == ModContent.ProjectileType<BaronArenaWhirlpool>();
            if (!arenaReal)
            {
                NPC.velocity = Vector2.Zero;
                StateReset();
            }
            if (Timer == 1)
            {
                Anim = 1;
                SoundEngine.PlaySound(SoundID.Item21, NPC.Center);
                SoundEngine.PlaySound(SoundID.Item92, NPC.Center);
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<GlowRing>(), 0, 0f, Main.myPlayer, NPC.whoAmI, -24);
                }
            }
            if (Timer == 61)
            {
                Anim = 0;
                arena.ai[2] = 1; //activate projectile shooting AI of arena
            }
            if (Timer >= 61)
            {
                LockVector1 = Vector2.Normalize(NPC.Center - player.Center) * 500;

                Vector2 target = player.Center + LockVector1;
                if ((NPC.Distance(target) < 25 && AI3 == 0) || (Timer % 60 == 0 && AI3 != 0))
                {
                    AI3 = Main.rand.NextFloat(-1, 1);
                    NPC.netUpdate = true;
                }

                if (AI3 == 0)
                {
                    if (NPC.velocity.Length() < 20 + (player.velocity.Length() / 2))
                    {
                        NPC.velocity += NPC.DirectionTo(target) * 0.02f;
                        NPC.velocity = NPC.DirectionTo(target) * NPC.velocity.Length() * 1.05f;
                    }
                    else
                    {
                        NPC.velocity = NPC.DirectionTo(target) * (20 + (player.velocity.Length() / 2));

                    }
                }
                else
                {
                    NPC.velocity = (target - NPC.Center) / 10;
                }
            }
            RotateTowards(player.Center, 2f);
            if (Timer > 420)
            {
                arena.ai[2] = 0; //deactivate projectile shooting AI of arena
            }
            if (Timer > 500)
            {
                NPC.velocity = Vector2.Zero;
                StateReset();
            }
        }
        void P2LaserSweep()
        {
            const int PositioningTime = 30;
            const int WindupTime = 90;
            int AttackTime = 120;
            const int Endlag = 10;
            const int distance = 500;
            const float MaxRot = MathHelper.Pi / 5;

            if (Timer == 1)
            {
                //go towards center of arena so you don't get fucked by arena border and can't go around
                Projectile arena = Main.projectile[ArenaProjID];
                bool arenaReal = arena != null && arena.active && arena.type == ModContent.ProjectileType<BaronArenaWhirlpool>();
                if (!arenaReal)
                {
                    AI2 = Math.Sign(NPC.Center.X - player.Center.X);
                }
                else
                {
                    AI2 = Math.Sign(arena.Center.X - player.Center.X);
                    HitPlayer = false;
                }
                LockVector1 = new Vector2(distance * AI2, -distance * 0.7f);

            }
            if (Timer < PositioningTime && NPC.Distance(player.Center + LockVector1) > 15) //don't progress time if too far away from start position
            {
                Timer--;
            }
            else if (Timer < PositioningTime) //if close enough, start attack
            {
                Timer = PositioningTime;
                HitPlayer = true;
                if (WorldSavingSystem.EternityMode)
                {
                    SoundEngine.PlaySound(SoundID.Item61, NPC.Center);
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Vector2 vel = NPC.rotation.ToRotationVector2() * 10f;
                        int nukeDur = Main.rand.Next(160, 160); //nuke duration
                        NPC.netUpdate = true;
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, -vel, ModContent.ProjectileType<BaronNuke>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 0f, Main.myPlayer, nukeDur, player.whoAmI);
                    }
                }
            }
            if (Timer < PositioningTime) //get to position
            {
                Vector2 target = player.Center + LockVector1;
                if (NPC.velocity.Length() < 20 + (player.velocity.Length() / 2))
                {
                    NPC.velocity += NPC.DirectionTo(target) * 0.02f;
                    NPC.velocity = NPC.DirectionTo(target) * NPC.velocity.Length() * 1.05f;
                }
                else
                {
                    NPC.velocity = NPC.DirectionTo(target) * (20 + (player.velocity.Length() / 2));

                }
                RotateTowards(player.Center, 2);
            }
            else if (Timer < PositioningTime + WindupTime) //rotate with telegraph line
            {
                NPC.velocity *= 0.8f;
                if (Timer == PositioningTime)
                {
                    SoundEngine.PlaySound(new SoundStyle("FargowiltasSouls/Assets/Sounds/LaserTelegraph") with { Volume = 1.25f }, NPC.Center);
                    AI3 = Main.rand.NextBool() ? 1 : -1; //sign for rotation
                    LockVector1 = (player.Center - NPC.Center).RotatedBy(AI3 * MaxRot);
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<BloomLine>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 0f, Main.myPlayer, 7, NPC.whoAmI, WindupTime + 20);
                    }
                    NPC.netUpdate = true;
                }
                RotateTowards(NPC.Center + LockVector1, 1.75f);
            }
            else if (Timer < PositioningTime + WindupTime + AttackTime) //rotate back with attack deathray
            {
                float RotationSpeed = WorldSavingSystem.EternityMode ? 1.1f : 1f;

                
                if (Timer == PositioningTime + WindupTime)
                {
                    //const float RotationFactor = 0.75f;

                    AI4 = NPC.rotation; //cache rotation
                    LockVector1 = (player.Center - NPC.Center);
                    RotateTowards(NPC.Center + LockVector1, RotationSpeed);

                    SoundEngine.PlaySound(new SoundStyle("FargowiltasSouls/Assets/Sounds/LaserSound_Slow") with { Pitch = -0.2f }, NPC.Center);
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, NPC.rotation.ToRotationVector2(), ModContent.ProjectileType<BaronDeathray>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 0f, Main.myPlayer, ai0: NPC.whoAmI, ai2: AttackTime);
                    }
                    NPC.netUpdate = true;
                }
                else
                {
                    NPC.rotation += Math.Sign(NPC.rotation - AI4) * RotationSpeed * MathHelper.Pi / 180; //keep rotating in direction rotated first frame, no turning direction
                }
                
                
            }
            else if (WorldSavingSystem.EternityMode && Timer > PositioningTime + WindupTime + AttackTime) // in emode, go right into predictive dash without endlag
            {
                StateReset();
            }
            else if (Timer > PositioningTime + WindupTime + AttackTime + Endlag)
            {
                
                NPC.velocity = Vector2.Zero;
                StateReset();
            }
        }
        #endregion
        #endregion
        #region Help Methods
        void StateReset()
        {
            NPC.TargetClosest(false);
            if (WorldSavingSystem.EternityMode && State == (int)StateEnum.P2LaserSweep) //combos into predictive dash in emode
            {
                State = (int)StateEnum.P2PredictiveDash;
                availablestates.Remove((int)StateEnum.P2LaserSweep);
                availablestates.Remove((int)StateEnum.P2PredictiveDash);
            }
            else if ((State == (int)StateEnum.Swim) || Phase == 2)
                RandomizeState();
            else
                State = (int)StateEnum.Swim;
            bool expertP2 = NPC.life < NPC.lifeMax / 2 && Phase == 1 && Main.expertMode;
            bool nonexpertP2 = NPC.life < NPC.lifeMax / 3 && Phase == 1 && !Main.expertMode;
            if (expertP2 || nonexpertP2)
            {
                State = (float)StateEnum.Phase2Transition;
            }
            Timer = 0;
            AI2 = 0;
            AI3 = 0;
        }
        void RandomizeState() //it's done this way so it cycles between attacks in a random order: for increased variety
        {
            int index;
            if (availablestates.Count < 1)
            {
                availablestates.Clear();
                if (Phase == 1)
                {
                    availablestates.AddRange(P1Attacks);
                }
                else if (Phase == 2)
                {
                    availablestates.AddRange(P2Attacks);
                }
                availablestates.Remove((int)State); //avoid possible repeat after refilling list
            }
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                index = Main.rand.Next(availablestates.Count);
                State = availablestates[index];
                availablestates.RemoveAt(index);
            }
            NPC.netUpdate = true;
        }
        bool AliveCheck(Player p, bool forceDespawn = false)
        {
            if (forceDespawn || !p.active || p.dead || Vector2.Distance(NPC.Center, p.Center) > 5000f)
            {
                NPC.TargetClosest();
                p = Main.player[NPC.target];
                if (forceDespawn || !p.active || p.dead || Vector2.Distance(NPC.Center, p.Center) > 5000f)
                {
                    NPC.noTileCollide = true;
                    if (NPC.timeLeft > 30)
                        NPC.timeLeft = 30;
                    NPC.velocity.Y += 1f;
                    if (NPC.timeLeft == 1)
                    {
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            FargoSoulsUtil.ClearHostileProjectiles(2, NPC.whoAmI);
                        }
                    }
                    return false;
                }
            }
            if (NPC.timeLeft < 600)
                NPC.timeLeft = 600;

            return true;
        }
        void RotateTowards(Vector2 target, float speed)
        {
            Vector2 LV = NPC.rotation.ToRotationVector2();
            Vector2 PV = NPC.DirectionTo(target);
            float anglediff = FargoSoulsUtil.RotationDifference(LV, PV);
            //change rotation towards target
            NPC.rotation = NPC.rotation.ToRotationVector2().RotatedBy(Math.Sign(anglediff) * Math.Min(Math.Abs(anglediff), speed * MathHelper.Pi / 180)).ToRotation();
        }
        #endregion
    }
}