using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.GameContent.Events;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace FargowiltasSouls
{
    public class FargoSoulsWorld : ModSystem
    {
        public static bool SwarmActive => ModLoader.TryGetMod("Fargowiltas", out Mod fargo) && (bool)fargo.Call("SwarmActive");

        public static bool downedBetsy;
        private static bool _downedBoss;

        //masomode
        public const int MaxCountPreHM = 560;
        public const int MaxCountHM = 240;

        public static bool EternityMode;
        public static bool MasochistModeReal;
        public static bool CanPlayMaso;
        public static bool downedFishronEX;
        public static bool downedDevi;
        public static bool downedAbom;
        public static bool downedMutant;
        public static bool AngryMutant;
        public static bool SuppressRandomMutant;

        public static bool downedMM;
        public static bool firstGoblins;
        public static int skipMutantP1;

        public static bool NoMasoBossScaling = true;
        public static bool ReceivedTerraStorage;
        public static bool spawnedDevi;

        public static bool[] downedChampions = new bool[9];

        public override void OnWorldLoad()
        {
            downedBetsy = false;
            _downedBoss = false;

            downedMM = false;

            EternityMode = false;
            CanPlayMaso = false;
            MasochistModeReal = false;
            downedFishronEX = false;
            downedDevi = false;
            downedAbom = false;
            downedMutant = false;
            AngryMutant = false;
            SuppressRandomMutant = false;

            firstGoblins = true;
            skipMutantP1 = 0;

            NoMasoBossScaling = true;
            ReceivedTerraStorage = false;
            spawnedDevi = false;

            for (int i = 0; i < downedChampions.Length; i++)
                downedChampions[i] = false;
        }

        public override void SaveWorldData(TagCompound tag)
        {

            List<string> downed = new List<string>();
            if (downedBetsy) downed.Add("betsy");
            if (_downedBoss) downed.Add("boss");
            if (EternityMode) downed.Add("eternity");
            if (CanPlayMaso) downed.Add("CanPlayMaso");
            if (MasochistModeReal) downed.Add("getReal");
            if (downedFishronEX) downed.Add("downedFishronEX");
            if (downedDevi) downed.Add("downedDevi");
            if (downedAbom) downed.Add("downedAbom");
            if (downedMutant) downed.Add("downedMutant");
            if (AngryMutant) downed.Add("AngryMutant");
            if (SuppressRandomMutant) downed.Add("SuppressRandomMutant");
            if (downedMM) downed.Add("downedMadhouse");
            if (firstGoblins) downed.Add("forceMeteor");
            if (NoMasoBossScaling) downed.Add("NoMasoBossScaling");
            if (ReceivedTerraStorage) downed.Add("ReceivedTerraStorage");
            if (spawnedDevi) downed.Add("spawnedDevi");

            for (int i = 0; i < downedChampions.Length; i++)
            {
                if (downedChampions[i])
                    downed.Add("downedChampion" + i.ToString());
            }

            tag.Add("downed", downed);
            tag.Add("mutantP1", skipMutantP1);
        }

        public override void LoadWorldData(TagCompound tag)
        {
            IList<string> downed = tag.GetList<string>("downed");
            downedBetsy = downed.Contains("betsy");
            _downedBoss = downed.Contains("boss");
            EternityMode = downed.Contains("eternity") || downed.Contains("masochist");
            CanPlayMaso = downed.Contains("CanPlayMaso");
            MasochistModeReal = downed.Contains("getReal");
            downedFishronEX = downed.Contains("downedFishronEX");
            downedDevi = downed.Contains("downedDevi");
            downedAbom = downed.Contains("downedAbom");
            downedMutant = downed.Contains("downedMutant");
            AngryMutant = downed.Contains("AngryMutant");
            SuppressRandomMutant = downed.Contains("SuppressRandomMutant");
            downedMM = downed.Contains("downedMadhouse");
            firstGoblins = downed.Contains("forceMeteor");
            NoMasoBossScaling = downed.Contains("NoMasoBossScaling");
            ReceivedTerraStorage = downed.Contains("ReceivedTerraStorage");
            spawnedDevi = downed.Contains("spawnedDevi");

            for (int i = 0; i < downedChampions.Length; i++)
                downedChampions[i] = downed.Contains($"downedChampion{i}");

            if (tag.ContainsKey("mutantP1"))
                skipMutantP1 = tag.GetAsInt("mutantP1");
        }

        public override void NetReceive(BinaryReader reader)
        {
            skipMutantP1 = reader.ReadInt32();

            BitsByte flags = reader.ReadByte();
            downedBetsy = flags[0];
            _downedBoss = flags[1];
            EternityMode = flags[2];
            downedFishronEX = flags[3];
            downedDevi = flags[4];
            downedAbom = flags[5];
            downedMutant = flags[6];
            AngryMutant = flags[7];
            downedMM = flags[8];
            firstGoblins = flags[9];
            NoMasoBossScaling = flags[10];
            ReceivedTerraStorage = flags[11];
            spawnedDevi = flags[12];
            SuppressRandomMutant = flags[13];
            MasochistModeReal = flags[14];
            CanPlayMaso = flags[15];

            const int offset = 16;
            for (int i = 0; i < downedChampions.Length; i++)
            {
                downedChampions[i] = flags[i + offset];
            }
        }

        public override void NetSend(BinaryWriter writer)
        {
            writer.Write(skipMutantP1);

            BitsByte flags = new BitsByte
            {
                [0] = downedBetsy,
                [1] = _downedBoss,
                [2] = EternityMode,
                [3] = downedFishronEX,
                [4] = downedDevi,
                [5] = downedAbom,
                [6] = downedMutant,
                [7] = AngryMutant,
                [8] = downedMM,
                [9] = firstGoblins,
                [10] = NoMasoBossScaling,
                [11] = ReceivedTerraStorage,
                [12] = spawnedDevi,
                [13] = SuppressRandomMutant,
                [14] = MasochistModeReal,
                [15] = CanPlayMaso,
                [16] = downedChampions[0],
                [17] = downedChampions[1],
                [18] = downedChampions[2],
                [19] = downedChampions[3],
                [20] = downedChampions[4],
                [21] = downedChampions[5],
                [22] = downedChampions[6],
                [23] = downedChampions[7],
                [24] = downedChampions[8]
            };

            writer.Write(flags);
        }

        public override void PostUpdateWorld()
        {
            NPC.LunarShieldPowerExpert = 150;

            if (EternityMode)
            {
                NPC.LunarShieldPowerExpert = 50;

                if (!FargoSoulsUtil.WorldIsExpertOrHarder())
                {
                    EternityMode = false;
                    FargoSoulsUtil.PrintText("Difficulty too low, Eternity Mode deactivated...", new Color(175, 75, 255));
                    if (Main.netMode == NetmodeID.Server)
                        NetMessage.SendData(MessageID.WorldData);
                    if (!Main.dedServ)
                        Terraria.Audio.SoundEngine.PlaySound(SoundID.Roar, Main.LocalPlayer.Center, 0);
                }

                if (!NPC.downedSlimeKing && !NPC.downedBoss1 && !Main.hardMode //pre boss, disable some events
                    && ModContent.TryFind("Fargowiltas", "Abominationn", out ModNPC abom) && !NPC.AnyNPCs(abom.Type))
                {
                    if (Main.raining || Sandstorm.Happening || Main.bloodMoon)
                    {
                        Main.raining = false;
                        Main.rainTime = 0;
                        Main.maxRaining = 0;
                        Sandstorm.Happening = false;
                        Sandstorm.TimeLeft = 0;
                        Main.bloodMoon = false;
                        if (Main.netMode == NetmodeID.Server)
                            NetMessage.SendData(MessageID.WorldData);
                    }
                }

                if (!MasochistModeReal && EternityMode && FargoSoulsUtil.WorldIsMaster() && CanPlayMaso)
                {
                    MasochistModeReal = true;
                    FargoSoulsUtil.PrintText("Master and Eternity combined: Masochist Mode activated!!", new Color(51, 255, 191, 0));
                    if (Main.netMode == NetmodeID.Server)
                        NetMessage.SendData(MessageID.WorldData);
                    if (!Main.dedServ)
                        Terraria.Audio.SoundEngine.PlaySound(SoundID.Roar, Main.LocalPlayer.Center, 0);
                }
            }

            if (MasochistModeReal && !(EternityMode && FargoSoulsUtil.WorldIsMaster() && CanPlayMaso))
            {
                MasochistModeReal = false;
                FargoSoulsUtil.PrintText("Masochist Mode deactivated.", new Color(51, 255, 191, 0));
                if (Main.netMode == NetmodeID.Server)
                    NetMessage.SendData(MessageID.WorldData);
                if (!Main.dedServ)
                    Terraria.Audio.SoundEngine.PlaySound(SoundID.Roar, Main.LocalPlayer.Center, 0);
            }

            //Main.NewText(BuilderMode);

            #region commented

            //right when day starts
            /*if(/*Main.time == 0 && Main.dayTime && !Main.eclipse && FargoSoulsWorld.masochistMode)
            {
                    Terraria.Audio.SoundEngine.PlaySound(SoundID.Roar, (int)player.position.X, (int)player.position.Y, 0, 1f, 0);

                    if (Main.netMode == NetmodeID.SinglePlayer)
                    {
                        Main.eclipse = true;
                        //Main.NewText(Lang.misc[20], 50, 255, 130, false);
                    }
                    else
                    {
                        //NetMessage.SendData(61, -1, -1, "", player.whoAmI, -6f, 0f, 0f, 0, 0, 0);
                    }


            }*/

            // if (this.itemTime == 0 && this.itemAnimation > 0 && item.type == 361 && Main.CanStartInvasion(1, true))
            // {
            // this.itemTime = item.useTime;
            // Terraria.Audio.SoundEngine.PlaySound(SoundID.Roar, (int)this.position.X, (int)this.position.Y, 0, 1f, 0);
            // if (Main.netMode != NetmodeID.MultiplayerClient)
            // {
            // if (Main.invasionType == 0)
            // {
            // Main.invasionDelay = 0;
            // Main.StartInvasion(1);
            // }
            // }
            // else
            // {
            // NetMessage.SendData(61, -1, -1, "", this.whoAmI, -1f, 0f, 0f, 0, 0, 0);
            // }
            // }
            // if (this.itemTime == 0 && this.itemAnimation > 0 && item.type == 602 && Main.CanStartInvasion(2, true))
            // {
            // this.itemTime = item.useTime;
            // Terraria.Audio.SoundEngine.PlaySound(SoundID.Roar, (int)this.position.X, (int)this.position.Y, 0, 1f, 0);
            // if (Main.netMode != NetmodeID.MultiplayerClient)
            // {
            // if (Main.invasionType == 0)
            // {
            // Main.invasionDelay = 0;
            // Main.StartInvasion(2);
            // }
            // }
            // else
            // {
            // NetMessage.SendData(61, -1, -1, "", this.whoAmI, -2f, 0f, 0f, 0, 0, 0);
            // }
            // }
            // if (this.itemTime == 0 && this.itemAnimation > 0 && item.type == 1315 && Main.CanStartInvasion(3, true))
            // {
            // this.itemTime = item.useTime;
            // Terraria.Audio.SoundEngine.PlaySound(SoundID.Roar, (int)this.position.X, (int)this.position.Y, 0, 1f, 0);
            // if (Main.netMode != NetmodeID.MultiplayerClient)
            // {
            // if (Main.invasionType == 0)
            // {
            // Main.invasionDelay = 0;
            // Main.StartInvasion(3);
            // }
            // }
            // else
            // {
            // NetMessage.SendData(61, -1, -1, "", this.whoAmI, -3f, 0f, 0f, 0, 0, 0);
            // }
            // }
            // if (this.itemTime == 0 && this.itemAnimation > 0 && item.type == 1844 && !Main.dayTime && !Main.pumpkinMoon && !Main.snowMoon && !DD2Event.Ongoing)
            // {
            // this.itemTime = item.useTime;
            // Terraria.Audio.SoundEngine.PlaySound(SoundID.Roar, (int)this.position.X, (int)this.position.Y, 0, 1f, 0);
            // if (Main.netMode != NetmodeID.MultiplayerClient)
            // {
            // Main.NewText(Lang.misc[31], 50, 255, 130, false);
            // Main.startPumpkinMoon();
            // }
            // else
            // {
            // NetMessage.SendData(61, -1, -1, "", this.whoAmI, -4f, 0f, 0f, 0, 0, 0);
            // }
            // }

            // if (this.itemTime == 0 && this.itemAnimation > 0 && item.type == 3601 && NPC.downedGolemBoss && Main.hardMode && !NPC.AnyDanger() && !NPC.AnyoneNearCultists())
            // {
            // Terraria.Audio.SoundEngine.PlaySound(SoundID.Roar, (int)this.position.X, (int)this.position.Y, 0, 1f, 0);
            // this.itemTime = item.useTime;
            // if (Main.netMode == NetmodeID.SinglePlayer)
            // {
            // WorldGen.StartImpendingDoom();
            // }
            // else
            // {
            // NetMessage.SendData(61, -1, -1, "", this.whoAmI, -8f, 0f, 0f, 0, 0, 0);
            // }
            // }
            // if (this.itemTime == 0 && this.itemAnimation > 0 && item.type == 1958 && !Main.dayTime && !Main.pumpkinMoon && !Main.snowMoon && !DD2Event.Ongoing)
            // {
            // this.itemTime = item.useTime;
            // Terraria.Audio.SoundEngine.PlaySound(SoundID.Roar, (int)this.position.X, (int)this.position.Y, 0, 1f, 0);
            // if (Main.netMode != NetmodeID.MultiplayerClient)
            // {
            // Main.NewText(Lang.misc[34], 50, 255, 130, false);
            // Main.startSnowMoon();
            // }
            // else
            // {
            // NetMessage.SendData(61, -1, -1, "", this.whoAmI, -5f, 0f, 0f, 0, 0, 0);
            // }
            // }

            #endregion
        }

        public override void PostWorldGen()
        {
            /*WorldGen.PlaceTile(Main.spawnTileX - 1, Main.spawnTileY, TileID.GrayBrick, false, true);
            WorldGen.PlaceTile(Main.spawnTileX, Main.spawnTileY, TileID.GrayBrick, false, true);
            WorldGen.PlaceTile(Main.spawnTileX + 1, Main.spawnTileY, TileID.GrayBrick, false, true);
            Main.tile[Main.spawnTileX - 1, Main.spawnTileY].slope(0);
            Main.tile[Main.spawnTileX, Main.spawnTileY].slope(0);
            Main.tile[Main.spawnTileX + 1, Main.spawnTileY].slope(0);
            WorldGen.PlaceTile(Main.spawnTileX, Main.spawnTileY - 1, ModContent.Find<ModTile>("Fargowiltas", "RegalStatueSheet"), false, true);*/

            bool TryPlacingStatue(int baseCheckX, int baseCheckY)
            {
                List<int> legalBlocks = new List<int> { 
                    TileID.Stone, 
                    TileID.Grass,
                    TileID.Dirt,
                    TileID.SnowBlock,
                    TileID.IceBlock, 
                    TileID.ClayBlock,
                    TileID.Mud,
                    TileID.JungleGrass,
                    TileID.Sand
                };

                bool canPlaceStatueHere = true;
                for (int i = 0; i < 3; i++) //check no obstructing blocks
                    for (int j = 0; j < 4; j++)
                    {
                        Tile tile = Framing.GetTileSafely(baseCheckX + i, baseCheckY + j);
                        if (WorldGen.SolidOrSlopedTile(tile))
                        {
                            canPlaceStatueHere = false;
                            break;
                        }
                    }
                for (int i = 0; i < 3; i++) //check for solid foundation
                {
                    Tile tile = Framing.GetTileSafely(baseCheckX + i, baseCheckY + 4);
                    if (!WorldGen.SolidTile(tile) || !legalBlocks.Contains(tile.TileType))
                    {
                        canPlaceStatueHere = false;
                        break;
                    }
                }

                if (canPlaceStatueHere)
                {
                    for (int i = 0; i < 3; i++) //MAKE SURE nothing in the way
                        for (int j = 0; j < 4; j++)
                            WorldGen.KillTile(baseCheckX + i, baseCheckY + j);

                    WorldGen.PlaceTile(baseCheckX, baseCheckY + 4, TileID.GrayBrick, false, true);
                    WorldGen.PlaceTile(baseCheckX + 1, baseCheckY + 4, TileID.GrayBrick, false, true);
                    WorldGen.PlaceTile(baseCheckX + 2, baseCheckY + 4, TileID.GrayBrick, false, true);
                    Tile tile = Main.tile[baseCheckX, baseCheckY + 4]; tile.Slope = 0;
                    tile = Main.tile[baseCheckX + 1, baseCheckY + 4]; tile.Slope = 0;
                    tile = Main.tile[baseCheckX + 2, baseCheckY + 4]; tile.Slope = 0;
                    WorldGen.PlaceTile(baseCheckX + 1, baseCheckY + 3, ModContent.TileType<Tiles.MutantStatueGift>(), false, true);

                    return true;
                }

                return false;
            }

            int positionX = Main.spawnTileX - 1; //offset by dimensions of statue
            int positionY = Main.spawnTileY - 4;
            bool placed = false;
            for (int offsetX = -50; offsetX <= 50; offsetX++)
            {
                for (int offsetY = -30; offsetY <= 10; offsetY++)
                {
                    if (TryPlacingStatue(positionX + offsetX, positionY + offsetY))
                    {
                        placed = true;
                        break;
                    }
                }

                if (placed)
                    break;
            }
        }
    }
}
