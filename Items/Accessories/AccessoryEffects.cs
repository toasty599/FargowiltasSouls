using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameInput;
using Terraria.Graphics.Effects;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Graphics.Capture;
using FargowiltasSouls.Toggler;
using FargowiltasSouls.Projectiles.Souls;
using FargowiltasSouls.Projectiles;
using FargowiltasSouls.Buffs.Souls;
using System.Collections.Generic;
using FargowiltasSouls.Projectiles.Minions;
using FargowiltasSouls.Items.Accessories.Enchantments;
using FargowiltasSouls.NPCs;
using FargowiltasSouls.NPCs.MutantBoss;

namespace FargowiltasSouls
{
    public partial class FargoSoulsPlayer
    {
        //        public void FlowerBoots()
        //        {
        //            if (!Player.GetToggleValue("SupersonicFlower"))
        //                return;

        //            int x = (int)Player.Center.X / 16;
        //            int y = (int)(Player.position.Y + Player.height - 1f) / 16;

        //            if (x > -1 && x < Main.maxTilesX && y > -1 && y < Main.maxTilesY)
        //            {
        //                if (Main.tile[x, y] == null)
        //                {
        //                    Main.tile[x, y] = new Tile();
        //                }

        //                if (!Main.tile[x, y].active() && Main.tile[x, y].liquid == 0 && Main.tile[x, y + 1] != null && WorldGen.SolidTile(x, y + 1))
        //                {
        //                    Main.tile[x, y].frameY = 0;
        //                    Main.tile[x, y].slope(0);
        //                    Main.tile[x, y].halfBrick(false);

        //                    if (Main.tile[x, y + 1].type == 2)
        //                    {
        //                        if (Main.rand.NextBool())
        //                        {
        //                            Main.tile[x, y].active(true);
        //                            Main.tile[x, y].type = 3;
        //                            Main.tile[x, y].frameX = (short)(18 * Main.rand.Next(6, 11));
        //                            while (Main.tile[x, y].frameX == 144)
        //                            {
        //                                Main.tile[x, y].frameX = (short)(18 * Main.rand.Next(6, 11));
        //                            }
        //                        }
        //                        else
        //                        {
        //                            Main.tile[x, y].active(true);
        //                            Main.tile[x, y].type = 73;
        //                            Main.tile[x, y].frameX = (short)(18 * Main.rand.Next(6, 21));

        //                            while (Main.tile[x, y].frameX == 144)
        //                            {
        //                                Main.tile[x, y].frameX = (short)(18 * Main.rand.Next(6, 21));
        //                            }
        //                        }

        //                        if (Main.netMode == NetmodeID.MultiplayerClient)
        //                        {
        //                            NetMessage.SendTileSquare(-1, x, y, 1, TileChangeType.None);
        //                        }
        //                    }
        //                    else if (Main.tile[x, y + 1].type == 109)
        //                    {
        //                        if (Main.rand.NextBool())
        //                        {
        //                            Main.tile[x, y].active(true);
        //                            Main.tile[x, y].type = 110;
        //                            Main.tile[x, y].frameX = (short)(18 * Main.rand.Next(4, 7));

        //                            while (Main.tile[x, y].frameX == 90)
        //                            {
        //                                Main.tile[x, y].frameX = (short)(18 * Main.rand.Next(4, 7));
        //                            }
        //                        }
        //                        else
        //                        {
        //                            Main.tile[x, y].active(true);
        //                            Main.tile[x, y].type = 113;
        //                            Main.tile[x, y].frameX = (short)(18 * Main.rand.Next(2, 8));

        //                            while (Main.tile[x, y].frameX == 90)
        //                            {
        //                                Main.tile[x, y].frameX = (short)(18 * Main.rand.Next(2, 8));
        //                            }
        //                        }
        //                        if (Main.netMode == NetmodeID.MultiplayerClient)
        //                        {
        //                            NetMessage.SendTileSquare(-1, x, y, 1, TileChangeType.None);
        //                        }
        //                    }
        //                    else if (Main.tile[x, y + 1].type == 60)
        //                    {
        //                        Main.tile[x, y].active(true);
        //                        Main.tile[x, y].type = 74;
        //                        Main.tile[x, y].frameX = (short)(18 * Main.rand.Next(9, 17));

        //                        if (Main.netMode == NetmodeID.MultiplayerClient)
        //                        {
        //                            NetMessage.SendTileSquare(-1, x, y, 1, TileChangeType.None);
        //                        }
        //                    }
        //                }
        //            }
        //        }

        //        #region enchantments

        public void BeeEffect(bool hideVisual)
        {
            Player.strongBees = true;
            //bees ignore defense
            BeeEnchantActive = true;
        }

        public void BeetleEffect()
        {
            if (!Player.GetToggleValue("Beetle"))
                return;

            //don't let this stack
            if (Player.beetleDefense || Player.beetleOffense)
                return;

            BeetleEnchantActive = true;

            if (BeetleEnchantDefenseTimer > 0) //do defensive beetle things
            {
                Player.beetleDefense = true;
                Player.beetleCounter += 1f;
                int cap = TerrariaSoul ? 3 : 2;
                int time = 180 * 3 / cap;
                if (Player.beetleCounter >= time)
                {
                    if (Player.beetleOrbs > 0 && Player.beetleOrbs < cap)
                    {
                        for (int k = 0; k < Player.MaxBuffs; k++)
                        {
                            if (Player.buffType[k] >= 95 && Player.buffType[k] <= 96)
                            {
                                Player.DelBuff(k);
                            }
                        }
                    }
                    if (Player.beetleOrbs < cap)
                    {
                        Player.AddBuff(BuffID.BeetleEndurance1 + Player.beetleOrbs, 5, false);
                        Player.beetleCounter = 0f;
                    }
                    else
                    {
                        Player.beetleCounter = time;
                    }
                }
            }
            else
            {
                Player.beetleOffense = true;

                Player.beetleCounter -= 3f;
                Player.beetleCounter = Player.beetleCounter - Player.beetleCountdown / 10f;

                Player.beetleCountdown += 1;

                if (Player.beetleCounter < 0)
                    Player.beetleCounter = 0;

                int beetles = 0;
                int num2 = 400;
                int num3 = 1200;
                int num4 = 4600;

                if (Player.beetleCounter > num2 + num3 + num4 + num3)
                    Player.beetleCounter = num2 + num3 + num4 + num3;
                if (Player.beetleCounter > num2 + num3 + num4)
                {
                    Player.AddBuff(BuffID.BeetleMight3, 5, false);
                    beetles = 3;
                }
                else if (Player.beetleCounter > num2 + num3)
                {
                    Player.AddBuff(BuffID.BeetleMight2, 5, false);
                    beetles = 2;
                }
                else if (Player.beetleCounter > num2)
                {
                    Player.AddBuff(BuffID.BeetleMight1, 5, false);
                    beetles = 1;
                }
                
                if (beetles < Player.beetleOrbs)
                    Player.beetleCountdown = 0;
                else if (beetles > Player.beetleOrbs)
                    Player.beetleCounter = Player.beetleCounter + 200f;

                float damage = beetles * 0.10f;
                Player.GetDamage(DamageClass.Generic) += damage;
                Player.GetDamage(DamageClass.Melee) -= damage; //offset the actual vanilla beetle buff

                if (beetles != Player.beetleOrbs && Player.beetleOrbs > 0)
                {
                    for (int b = 0; b < Player.MaxBuffs; ++b)
                    {
                        if (Player.buffType[b] >= 98 && Player.buffType[b] <= 100 && Player.buffType[b] != 97 + beetles)
                            Player.DelBuff(b);
                    }
                }
            }

            //vanilla code for beetle visuals
            if (!Player.beetleDefense && !Player.beetleOffense)
            {
                Player.beetleCounter = 0f;
            }
            else
            {
                Player.beetleFrameCounter++;
                if (Player.beetleFrameCounter >= 1)
                {
                    Player.beetleFrameCounter = 0;
                    Player.beetleFrame++;
                    if (Player.beetleFrame > 2)
                    {
                        Player.beetleFrame = 0;
                    }
                }
                for (int l = Player.beetleOrbs; l < 3; l++)
                {
                    Player.beetlePos[l].X = 0f;
                    Player.beetlePos[l].Y = 0f;
                }
                for (int m = 0; m < Player.beetleOrbs; m++)
                {
                    Player.beetlePos[m] += Player.beetleVel[m];
                    Vector2[] expr_6EcCp0 = Player.beetleVel;
                    int expr_6EcCp1 = m;
                    expr_6EcCp0[expr_6EcCp1].X = expr_6EcCp0[expr_6EcCp1].X + Main.rand.Next(-100, 101) * 0.005f;
                    Vector2[] expr71ACp0 = Player.beetleVel;
                    int expr71ACp1 = m;
                    expr71ACp0[expr71ACp1].Y = expr71ACp0[expr71ACp1].Y + Main.rand.Next(-100, 101) * 0.005f;
                    float num6 = Player.beetlePos[m].X;
                    float num7 = Player.beetlePos[m].Y;
                    float num8 = (float)Math.Sqrt(num6 * num6 + num7 * num7);
                    if (num8 > 100f)
                    {
                        num8 = 20f / num8;
                        num6 *= -num8;
                        num7 *= -num8;
                        int num9 = 10;
                        Player.beetleVel[m].X = (Player.beetleVel[m].X * (num9 - 1) + num6) / num9;
                        Player.beetleVel[m].Y = (Player.beetleVel[m].Y * (num9 - 1) + num7) / num9;
                    }
                    else if (num8 > 30f)
                    {
                        num8 = 10f / num8;
                        num6 *= -num8;
                        num7 *= -num8;
                        int num10 = 20;
                        Player.beetleVel[m].X = (Player.beetleVel[m].X * (num10 - 1) + num6) / num10;
                        Player.beetleVel[m].Y = (Player.beetleVel[m].Y * (num10 - 1) + num7) / num10;
                    }
                    num6 = Player.beetleVel[m].X;
                    num7 = Player.beetleVel[m].Y;
                    num8 = (float)Math.Sqrt(num6 * num6 + num7 * num7);
                    if (num8 > 2f)
                    {
                        Player.beetleVel[m] *= 0.9f;
                    }
                    Player.beetlePos[m] -= Player.velocity * 0.25f;
                }
            }
        }



        public void ChloroEffect(Item item, bool hideVisual)
        {
            ChloroEnchantActive = true;

            ChloroEnchantItem = item;

            if (Player.whoAmI == Main.myPlayer && Player.GetToggleValue("Chlorophyte") && Player.ownedProjectileCounts[ModContent.ProjectileType<Chlorofuck>()] == 0)
            {
                int dmg = NatureForce ? 150 : 75;
                const int max = 5;
                float rotation = 2f * (float)Math.PI / max;
                for (int i = 0; i < max; i++)
                {
                    Vector2 spawnPos = Player.Center + new Vector2(60, 0f).RotatedBy(rotation * i);
                    FargoSoulsUtil.NewSummonProjectile(Player.GetSource_Misc(""), spawnPos, Vector2.Zero, ModContent.ProjectileType<Chlorofuck>(), dmg, 10f, Player.whoAmI, 0, rotation * i);
                }
            }
        }


        public void CrimsonEffect(bool hideVisual)
        {
            if (CrimsonRegen && ++CrimsonRegenTimer > 30)
            {
                CrimsonRegenTimer = 0;

                int heal = 5;

                HealPlayer(heal);

                CrimsonRegenSoFar += heal;

                //done regenning
                if (CrimsonRegenSoFar >= CrimsonTotalToRegen)
                {
                    CrimsonTotalToRegen = 0;
                    CrimsonRegenSoFar = 0;
                    Player.ClearBuff(ModContent.BuffType<CrimsonRegen>());
                }
            }

            //Player.crimsonRegen = true;

            CrimsonEnchantActive = true;
        }

        public void DarkArtistEffect(bool hideVisual)
        {
            if (Player.ownedProjectileCounts[ModContent.ProjectileType<FlameburstMinion>()] == 0)
            {
                DarkArtistSpawn = true;
                //DarkSpawnCD = 60;
            }

            ApprenticeEnchantActive = true;
            DarkArtistEnchantActive = true;

            //int maxTowers = 3;

            //if (TerrariaSoul)
            //{
            //    maxTowers = 5;
            //}
            //else if (ShadowForce)
            //{
            //    maxTowers = 4;
            //}

            //spawn tower boi
            if (Player.whoAmI == Main.myPlayer && DarkArtistSpawn && DarkArtistSpawnCD <= 0 && Player.GetToggleValue("DarkArt"))
            //&& Player.ownedProjectileCounts[ModContent.ProjectileType<FlameburstMinion>()] < maxTowers)
            {
                Projectile proj = Projectile.NewProjectileDirect(Player.GetSource_Misc(""), Player.Center, Vector2.Zero, ModContent.ProjectileType<FlameburstMinion>(), 0, 0f, Player.whoAmI);
                proj.netUpdate = true; // TODO make this proj sync meme

                DarkArtistSpawn = false;
                DarkArtistSpawnCD = 60;
            }

            if (DarkArtistSpawnCD > 0)
            {
                DarkArtistSpawnCD--;
            }


        }

        public void ForbiddenEffect()
        {
            if (!Player.GetToggleValue("Forbidden"))
                return;
            ForbiddenEnchantActive = true;

            //Player.setForbidden = true;
            //add cd

            if (DoubleTap)
            {
                CommandForbiddenStorm();

                /*Vector2 mouse = Main.MouseWorld;

                if (Player.ownedProjectileCounts[ModContent.ProjectileType<ForbiddenTornado>()] > 0)
                {
                    for (int i = 0; i < 1000; i++)
                    {
                        Projectile proj = Main.projectile[i];

                        if (proj.type == ModContent.ProjectileType<ForbiddenTornado>())
                        {
                            proj.Kill();
                        }
                    }
                }

                Projectile.NewProjectile(mouse.X, mouse.Y - 10, 0f, 0f, ModContent.ProjectileType<ForbiddenTornado>(), (WoodForce) ? 45 : 15, 0f, Player.whoAmI);*/
            }


            //Player.UpdateForbiddenSetLock();
            Lighting.AddLight(Player.Center, 0.8f, 0.7f, 0.2f);
            //storm boosted
        }

        public void CommandForbiddenStorm()
        {
            List<int> list = new List<int>();
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile projectile = Main.projectile[i];
                if (projectile.active && projectile.type == ModContent.ProjectileType<ForbiddenTornado>() && projectile.owner == Player.whoAmI)
                {
                    list.Add(i);
                }
            }

            Vector2 center = Player.Center;
            Vector2 mouse = Main.MouseWorld;

            bool flag3 = false;
            float[] array = new float[10];
            Vector2 v = mouse - center;
            Collision.LaserScan(center, v.SafeNormalize(Vector2.Zero), 60f, v.Length(), array);
            float num = 0f;
            for (int j = 0; j < array.Length; j++)
            {
                if (array[j] > num)
                {
                    num = array[j];
                }
            }
            float[] array2 = array;
            for (int k = 0; k < array2.Length; k++)
            {
                float num2 = array2[k];
                if (Math.Abs(num2 - v.Length()) < 10f)
                {
                    flag3 = true;
                    break;
                }
            }
            if (list.Count <= 1)
            {
                Vector2 vector = center + v.SafeNormalize(Vector2.Zero) * num;
                Vector2 value2 = vector - center;
                if (value2.Length() > 0f)
                {
                    for (float num3 = 0f; num3 < value2.Length(); num3 += 15f)
                    {
                        Vector2 position = center + value2 * (num3 / value2.Length());
                        Dust dust = Main.dust[Dust.NewDust(position, 0, 0, 269, 0f, 0f, 0, default(Color), 1f)];
                        dust.position = position;
                        dust.fadeIn = 0.5f;
                        dust.scale = 0.7f;
                        dust.velocity *= 0.4f;
                        dust.noLight = true;
                    }
                }
                for (float num4 = 0f; num4 < 6.28318548f; num4 += 0.209439516f)
                {
                    Dust dust2 = Main.dust[Dust.NewDust(vector, 0, 0, 269, 0f, 0f, 0, default(Color), 1f)];
                    dust2.position = vector;
                    dust2.fadeIn = 1f;
                    dust2.scale = 0.3f;
                    dust2.noLight = true;
                }
            }

            //Main.NewText(" " + (list.Count <= 1) + " " + flag3 + " " + Player.CheckMana(20, true, false));

            bool flag = (list.Count <= 1);
            flag &= flag3;



            if (flag)
            {
                flag = Player.CheckMana(20, true, false);
                if (flag)
                {
                    Player.manaRegenDelay = (int)Player.maxRegenDelay;
                }
            }
            if (!flag)
            {
                return;
            }
            foreach (int current in list)
            {
                Projectile projectile2 = Main.projectile[current];
                if (projectile2.ai[0] < 780f)
                {
                    projectile2.ai[0] = 780f + projectile2.ai[0] % 60f;
                    projectile2.netUpdate = true;
                }
            }

            int damage = (int)(20f * (1f + Player.GetDamage(DamageClass.Magic).Additive + Player.GetDamage(DamageClass.Summon).Additive - 2f));
            Projectile.NewProjectile(Player.GetSource_Misc(""), mouse, Vector2.Zero, ModContent.ProjectileType<ForbiddenTornado>(), damage, 0f, Main.myPlayer, 0f, 0f);
        }



        public void FrostEffect(bool hideVisual)
        {
            FrostEnchantActive = true;

            if (Player.whoAmI == Main.myPlayer && Player.GetToggleValue("Frost"))
            {
                if (icicleCD <= 0 && IcicleCount < 10 && Player.ownedProjectileCounts[ModContent.ProjectileType<FrostIcicle>()] < 10)
                {
                    IcicleCount++;

                    //kill all current ones
                    for (int i = 0; i < Main.maxProjectiles; i++)
                    {
                        Projectile proj = Main.projectile[i];

                        if (proj.active && proj.type == ModContent.ProjectileType<FrostIcicle>() && proj.owner == Player.whoAmI)
                        {
                            proj.active = false;
                            proj.netUpdate = true;
                        }
                    }

                    //respawn in formation
                    for (int i = 0; i < IcicleCount; i++)
                    {
                        float radians = (360f / IcicleCount) * i * (float)(Math.PI / 180);
                        Projectile frost = FargoSoulsUtil.NewProjectileDirectSafe(Player.GetSource_Misc(""), Player.Center, Vector2.Zero, ModContent.ProjectileType<FrostIcicle>(), 0, 0f, Player.whoAmI, 5, radians);
                        frost.netUpdate = true;
                    }

                    float dustScale = 1.5f;

                    if (IcicleCount % 10 == 0)
                    {
                        dustScale = 3f;
                    }

                    //dust
                    for (int j = 0; j < 20; j++)
                    {
                        Vector2 vector6 = Vector2.UnitY * 5f;
                        vector6 = vector6.RotatedBy((j - (20 / 2 - 1)) * 6.28318548f / 20) + Player.Center;
                        Vector2 vector7 = vector6 - Player.Center;
                        int d = Dust.NewDust(vector6 + vector7, 0, 0, 15);
                        Main.dust[d].noGravity = true;
                        Main.dust[d].scale = dustScale;
                        Main.dust[d].velocity = vector7;

                        if (IcicleCount % 10 == 0)
                        {
                            Main.dust[d].velocity *= 2;
                        }
                    }

                    icicleCD = 30;
                }

                if (icicleCD > 0)
                    icicleCD--;

                if (IcicleCount >= 1 && Player.controlUseItem && Player.HeldItem.damage > 0 && Player.HeldItem.createTile == -1 && Player.HeldItem.createWall == -1 && Player.HeldItem.ammo == AmmoID.None && Player.HeldItem.hammer == 0 && Player.HeldItem.pick == 0 && Player.HeldItem.axe == 0)
                {
                    int dmg = NatureForce ? 100 : 50;

                    for (int i = 0; i < Main.maxProjectiles; i++)
                    {
                        Projectile proj = Main.projectile[i];

                        if (proj.active && proj.type == ModContent.ProjectileType<FrostIcicle>() && proj.owner == Player.whoAmI)
                        {
                            Vector2 vel = (Main.MouseWorld - proj.Center).SafeNormalize(-Vector2.UnitY) * 20f;

                            int p = Projectile.NewProjectile(Player.GetSource_Misc(""), proj.Center, vel, ProjectileID.Blizzard, FargoSoulsUtil.HighestDamageTypeScaling(Player, dmg), 1f, Player.whoAmI);
                            if (p != Main.maxProjectiles)
                            {
                                Main.projectile[p].GetGlobalProjectile<FargoSoulsGlobalProjectile>().CanSplit = false;
                                Main.projectile[p].GetGlobalProjectile<FargoSoulsGlobalProjectile>().FrostFreeze = true;
                            }

                            proj.Kill();
                        }
                    }

                    IcicleCount = 0;
                    icicleCD = 120;
                }
            }


        }


        public void GoldEffect(bool hideVisual)
        {
            //gold ring
            Player.goldRing = true;
            //lucky coin
            if (Player.whoAmI == Main.myPlayer && Player.GetToggleValue("Gold"))
                Player.hasLuckyCoin = true;
            //discount card
            Player.discount = true;
            //midas
            GoldEnchantActive = true;


        }

        public void IronEffect()
        {
            IronEnchantShield = true;
        }

        public bool CanJungleJump = false;
        private bool jungleJumping = false;
        private int savedRocketTime;

        public void JungleEffect()
        {
            if (Player.whoAmI != Main.myPlayer)
                return;

            if (dashCD <= 0 && Player.GetToggleValue("JungleDash", false) && !Player.mount.Active)
            {
                float dashSpeed = ChloroEnchantActive ? 12f : 9f;

                if (Player.controlRight && Player.releaseRight)
                {
                    if (Player.doubleTapCardinalTimer[2] > 0 && Player.doubleTapCardinalTimer[2] != 15)
                    {
                        dashCD = 60;
                        if (DeerclawpsDashTimer < 10)
                            DeerclawpsDashTimer = 10;
                        Player.velocity.X = dashSpeed;
                        if (Main.netMode == NetmodeID.MultiplayerClient)
                            NetMessage.SendData(MessageID.PlayerControls, number: Player.whoAmI);
                    }
                }

                if (Player.controlLeft && Player.releaseLeft)
                {
                    if (Player.doubleTapCardinalTimer[3] > 0 && Player.doubleTapCardinalTimer[3] != 15)
                    {
                        dashCD = 60;
                        if (DeerclawpsDashTimer < 10)
                            DeerclawpsDashTimer = 10;
                        Player.velocity.X = -dashSpeed;
                        if (Main.netMode == NetmodeID.MultiplayerClient)
                            NetMessage.SendData(MessageID.PlayerControls, number: Player.whoAmI);
                    }
                }
            }

            if (Player.controlJump && Player.GetToggleValue("Jungle"))
            {
                if (Player.canJumpAgain_Blizzard || Player.canJumpAgain_Sandstorm || Player.canJumpAgain_Cloud || Player.canJumpAgain_Fart || Player.canJumpAgain_Sail || Player.canJumpAgain_Unicorn)
                {
                }
                else
                {
                    if (Player.jump == 0 && Player.releaseJump && Player.velocity.Y != 0f && !Player.mount.Active && CanJungleJump)
                    {
                        Player.velocity.Y = -Player.jumpSpeed * Player.gravDir;
                        Player.jump = (int)((double)Player.jumpHeight * 3);

                        jungleJumping = true;
                        JungleCD = 0;
                        CanJungleJump = false;

                        if (Main.netMode == NetmodeID.MultiplayerClient)
                            NetMessage.SendData(MessageID.PlayerControls, number: Player.whoAmI);
                    }
                }
            }

            if (jungleJumping && Player.GetToggleValue("Jungle"))
            {
                if (Player.rocketBoots > 0)
                {
                    savedRocketTime = Player.rocketTimeMax;
                    Player.rocketTime = 0;
                }

                Player.runAcceleration *= 3f;
                //Player.maxRunSpeed *= 2f;

                //spwn cloud
                if (JungleCD == 0)
                {
                    int tier = 1;
                    if (ChloroEnchantActive)
                        tier++;
                    if (NatureForce)
                        tier++;

                    JungleCD = 17 - tier * tier;
                    int dmg = 12 * tier * tier;

                    Terraria.Audio.SoundEngine.PlaySound(SoundID.Item, (int)Player.Center.X, (int)Player.Center.Y, 62, 0.5f);

                    foreach (Projectile p in FargoSoulsUtil.XWay(10, Player.GetSource_Misc(""), Player.Bottom, ProjectileID.SporeCloud, 4f, FargoSoulsUtil.HighestDamageTypeScaling(Player, dmg), 0f))
                    {
                        if (p == null)
                            continue;
                        p.usesIDStaticNPCImmunity = true;
                        p.idStaticNPCHitCooldown = 10;
                        p.GetGlobalProjectile<FargoSoulsGlobalProjectile>().noInteractionWithNPCImmunityFrames = true;
                    }
                }

                if (Player.jump == 0 || Player.velocity == Vector2.Zero)
                {
                    jungleJumping = false;
                    Player.rocketTime = savedRocketTime;
                }
            }
            else if (Player.jump <= 0 && Player.velocity.Y == 0f)
            {
                CanJungleJump = true;
            }

            if (JungleCD != 0)
            {
                JungleCD--;
            }
        }

        public void MeteorEffect()
        {
            MeteorEnchantActive = true;

            if (Player.whoAmI == Main.myPlayer && Player.GetToggleValue("Meteor"))
            {
                int damage = 50;

                if (meteorShower)
                {
                    if (meteorTimer % 2 == 0)
                    {
                        int p = Projectile.NewProjectile(Player.GetSource_Misc(""), Player.Center.X + Main.rand.Next(-1000, 1000), Player.Center.Y - 1000, Main.rand.Next(-2, 2), 0f + Main.rand.Next(8, 12), Main.rand.Next(424, 427), FargoSoulsUtil.HighestDamageTypeScaling(Player, damage), 0f, Player.whoAmI, 0f, 0.5f + (float)Main.rand.NextDouble() * 0.3f);
                        if (p != Main.maxProjectiles)
                        {
                            Main.projectile[p].GetGlobalProjectile<FargoSoulsGlobalProjectile>().CanSplit = false;
                            Main.projectile[p].netUpdate = true;
                            if (ModLoader.GetMod("Fargowiltas") != null)
                                ModLoader.GetMod("Fargowiltas").Call("LowRenderProj", Main.projectile[p]);
                        }
                    }

                    meteorTimer--;

                    if (meteorTimer <= 0)
                    {
                        meteorCD = 300;

                        if (CosmoForce)
                        {
                            meteorCD = 200;
                        }

                        meteorTimer = 150;
                        meteorShower = false;
                    }
                }
                else
                {
                    if (Player.controlUseItem)
                    {
                        meteorCD--;

                        if (meteorCD == 0)
                        {
                            meteorShower = true;
                        }
                    }
                    else
                    {
                        meteorCD = 300;
                    }
                }
            }
        }



        public void MoltenEffect()
        {
            MoltenEnchantActive = true;

            Player.lavaImmune = true;

            if (Player.GetToggleValue("Molten") && Player.whoAmI == Main.myPlayer)
            {
                Player.inferno = true;
                Lighting.AddLight((int)(Player.Center.X / 16f), (int)(Player.Center.Y / 16f), 0.65f, 0.4f, 0.1f);
                int buff = BuffID.OnFire;
                float distance = 200f;

                int baseDamage = 30;

                if (NatureForce)
                {
                    baseDamage *= 2;
                }

                int damage = FargoSoulsUtil.HighestDamageTypeScaling(Player, baseDamage);

                if (Player.whoAmI == Main.myPlayer)
                {
                    for (int i = 0; i < Main.maxNPCs; i++)
                    {
                        NPC npc = Main.npc[i];
                        if (npc.active && !npc.friendly && !npc.dontTakeDamage && !(npc.damage == 0 && npc.lifeMax == 5)) //critters
                        {
                            if (Vector2.Distance(Player.Center, npc.Center) <= distance)
                            {
                                int dmgRate = 60;

                                if (npc.FindBuffIndex(buff) == -1)
                                {
                                    npc.AddBuff(buff, 120);
                                }

                                if (Vector2.Distance(Player.Center, npc.Center) <= 50)
                                {
                                    dmgRate /= 10;
                                }
                                else if (Vector2.Distance(Player.Center, npc.Center) <= 100)
                                {
                                    dmgRate /= 5;
                                }
                                else if (Vector2.Distance(Player.Center, npc.Center) <= 150)
                                {
                                    dmgRate /= 2;
                                }

                                if (Player.infernoCounter % dmgRate == 0)
                                {
                                    Player.ApplyDamageToNPC(npc, damage, 0f, 0, false);
                                }
                            }
                        }
                    }
                }
            }
        }

        public void NebulaEffect()
        {
            if (!Player.GetToggleValue("Nebula", false)) return;

            NebulaEnchantActive = true;
        }



        public void ObsidianEffect()
        {
            Player.buffImmune[BuffID.OnFire] = true;
            Player.fireWalk = true;

            Player.lavaImmune = true;

            //that new acc effect e

            //in lava effects
            if (Player.lavaWet)
            {
                Player.gravity = Player.defaultGravity;
                Player.ignoreWater = true;
                Player.accFlipper = true;

                if (Player.GetToggleValue("Obsidian"))
                {
                    Player.AddBuff(ModContent.BuffType<ObsidianLavaWetBuff>(), 600);
                }

            }

            ObsidianEnchantActive = (TerraForce) || Player.lavaWet || LavaWet;
        }

        public void OrichalcumEffect()
        {
            OriEnchantActive = true;

            if (!Player.GetToggleValue("Orichalcum"))
                return;

            Player.onHitPetal = true;
        }

        public void PalladiumEffect()
        {
            //no lifesteal needed here for SoE
            if (Eternity) return;

            if (Player.GetToggleValue("Palladium"))
            {
                if (EarthForce || TerrariaSoul)
                    Player.onHitRegen = true;
                PalladEnchantActive = true;

                /*if (palladiumCD > 0)
                    palladiumCD--;*/
            }
        }



        public void RainEffect(Item item)
        {
            Player.buffImmune[BuffID.Wet] = true;
            RainEnchantActive = true;

            AddMinion(item, Player.GetToggleValue("Rain"), ModContent.ProjectileType<RainCloud>(), 36, 0);
        }

        public void RedRidingEffect(bool hideVisual)
        {
            RedEnchantActive = true;
            HuntressEnchantActive = true;
        }

        public void ShadowEffect(bool hideVisual)
        {
            ShadowEnchantActive = true;
        }

        public void ShadowEffectPostEquips()
        {
            if (Player.whoAmI == Main.myPlayer && Player.GetToggleValue("Shadow"))
            {
                int currentOrbs = Player.ownedProjectileCounts[ModContent.ProjectileType<ShadowEnchantOrb>()];

                int max = 2;

                if (TerrariaSoul)
                {
                    max = 5;
                }
                else if (ShadowForce)
                {
                    max = 4;
                }
                else if (AncientShadowEnchantActive)
                {
                    max = 3;
                }

                //spawn for first time
                if (currentOrbs == 0)
                {
                    float rotation = 2f * (float)Math.PI / max;

                    for (int i = 0; i < max; i++)
                    {
                        Vector2 spawnPos = Player.Center + new Vector2(60, 0f).RotatedBy(rotation * i);
                        int p = Projectile.NewProjectile(Player.GetSource_Misc(""), spawnPos, Vector2.Zero, ModContent.ProjectileType<ShadowEnchantOrb>(), 0, 10f, Player.whoAmI, 0, rotation * i);
                        Main.projectile[p].GetGlobalProjectile<FargoSoulsGlobalProjectile>().CanSplit = false;
                    }
                }
                //equipped somwthing that allows for more or less, respawn
                else if (currentOrbs != max)
                {
                    for (int i = 0; i < Main.maxProjectiles; i++)
                    {
                        Projectile proj = Main.projectile[i];

                        if (proj.active && proj.type == ModContent.ProjectileType<ShadowEnchantOrb>() && proj.owner == Player.whoAmI)
                        {
                            proj.Kill();
                        }
                    }

                    float rotation = 2f * (float)Math.PI / max;

                    for (int i = 0; i < max; i++)
                    {
                        Vector2 spawnPos = Player.Center + new Vector2(60, 0f).RotatedBy(rotation * i);
                        int p = Projectile.NewProjectile(Player.GetSource_Misc(""), spawnPos, Vector2.Zero, ModContent.ProjectileType<ShadowEnchantOrb>(), 0, 10f, Player.whoAmI, 0, rotation * i);
                        Main.projectile[p].GetGlobalProjectile<FargoSoulsGlobalProjectile>().CanSplit = false;
                    }
                }
            }
        }

        private void ShinobiDash(int direction)
        {
            dashCD = 90;

            var teleportPos = Player.position;

            const int length = 400; //make sure this is divisible by 16 btw

            if (Player.GetToggleValue("Shinobi")) //go through walls
            {
                teleportPos.X += length * direction;

                while (Collision.SolidCollision(teleportPos, Player.width, Player.height))
                {
                    if (direction == 1)
                    {
                        teleportPos.X++;
                    }
                    else
                    {
                        teleportPos.X--;
                    }
                }
            }
            else
            {
                for (int i = 0; i < length; i += 16)
                {
                    if (DeerclawpsItem != null)
                    {
                        if (Player.whoAmI == Main.myPlayer && Player.GetToggleValue("Deerclawps"))
                        {
                            int dam = 64;
                            dam = (int)(dam * Player.ActualClassDamage(DamageClass.Melee));
                            Vector2 vel = 16f * -Vector2.UnitY.RotatedByRandom(MathHelper.ToRadians(30));
                            float ai1 = Main.rand.NextFloat(0.8f, 1.3f);
                            if (Player.velocity.Y == 0)
                                Projectile.NewProjectile(Player.GetSource_Accessory(DeerclawpsItem), new Vector2(teleportPos.X, Player.Bottom.Y), vel, ProjectileID.DeerclopsIceSpike, dam, 8f, Main.myPlayer, 0, ai1);
                            else
                                Projectile.NewProjectile(Player.GetSource_Accessory(DeerclawpsItem), new Vector2(teleportPos.X, Player.Bottom.Y), vel * (Main.rand.NextBool() ? 1 : -1), ProjectileID.DeerclopsIceSpike, dam, 8f, Main.myPlayer, 0, ai1 / 2);
                        }
                    }

                    teleportPos.X += 16 * direction;

                    if (Collision.SolidCollision(teleportPos, Player.width, Player.height))
                    {
                        teleportPos.X -= 16 * direction;
                        break;
                    }
                }
            }

            if (teleportPos.X > 50 && teleportPos.X < (double)(Main.maxTilesX * 16 - 50) && teleportPos.Y > 50 && teleportPos.Y < (double)(Main.maxTilesY * 16 - 50))
            {
                FargoSoulsUtil.GrossVanillaDodgeDust(Player);
                Player.Teleport(teleportPos, 1);
                FargoSoulsUtil.GrossVanillaDodgeDust(Player);
                NetMessage.SendData(MessageID.Teleport, -1, -1, null, 0, Player.whoAmI, teleportPos.X, teleportPos.Y, 1);

                Player.velocity.X = 12f * direction;
                if (Main.netMode == NetmodeID.MultiplayerClient)
                    NetMessage.SendData(MessageID.PlayerControls, number: Player.whoAmI);
            }
        }

        private void ShinobiDashChecks()
        {
            if (Player.GetToggleValue("ShinobiDash") && Player.whoAmI == Main.myPlayer & dashCD <= 0 && !Player.mount.Active)
            {
                if ((Player.controlRight && Player.releaseRight))
                {
                    if (Player.doubleTapCardinalTimer[2] > 0 && Player.doubleTapCardinalTimer[2] != 15)
                    {
                        ShinobiDash(1);
                    }
                }

                if ((Player.controlLeft && Player.releaseLeft))
                {
                    if (Player.doubleTapCardinalTimer[3] > 0 && Player.doubleTapCardinalTimer[3] != 15)
                    {
                        ShinobiDash(-1);
                    }
                }
            }
        }

        private void DeerclawpsAttack(Vector2 pos)
        {
            if (Player.whoAmI == Main.myPlayer && Player.GetToggleValue("Deerclawps"))
            {
                int dam = 64;
                dam = (int)(dam * Player.ActualClassDamage(DamageClass.Melee));
                Vector2 vel = 16f * -Vector2.UnitY.RotatedByRandom(MathHelper.ToRadians(30));
                
                int type = ProjectileID.DeerclopsIceSpike;
                float ai1 = Main.rand.NextFloat(0.5f, 1f);
                if (LumpOfFlesh)
                {
                    type = ProjectileID.SharpTears;
                    ai1 *= 2; //bigger
                }

                if (Player.velocity.Y == 0)
                    Projectile.NewProjectile(Player.GetSource_Accessory(DeerclawpsItem), pos, vel, type, dam, 4f, Main.myPlayer, 0, ai1);
                else
                    Projectile.NewProjectile(Player.GetSource_Accessory(DeerclawpsItem), pos, vel * (Main.rand.NextBool() ? 1 : -1), type, dam, 4f, Main.myPlayer, 0, ai1 / 2);
            }
        }

        public void ShinobiEffect(bool hideVisual)
        {
            //tele through wall until open space on dash into wall
            /*if (Player.GetToggleValue("Shinobi") && Player.whoAmI == Main.myPlayer && Player.dashDelay == -1 && Player.mount.Type == -1 && Player.velocity.X == 0)
            {
                var teleportPos = new Vector2();
                int direction = Player.direction;

                teleportPos.X = Player.position.X + direction;
                teleportPos.Y = Player.position.Y;

                while (Collision.SolidCollision(teleportPos, Player.width, Player.height))
                {
                    if (direction == 1)
                    {
                        teleportPos.X++;
                    }
                    else
                    {
                        teleportPos.X--;
                    }
                }
                if (teleportPos.X > 50 && teleportPos.X < (double)(Main.maxTilesX * 16 - 50) && teleportPos.Y > 50 && teleportPos.Y < (double)(Main.maxTilesY * 16 - 50))
                {
                    Player.Teleport(teleportPos, 1);
                    NetMessage.SendData(MessageID.Teleport, -1, -1, null, 0, Player.whoAmI, teleportPos.X, teleportPos.Y, 1);
                }
            }*/

            ShinobiEnchantActive = true;
            MonkEnchantActive = true;
        }

        public void ShroomiteEffect(bool hideVisual)
        {
            if (!TerrariaSoul && Player.GetToggleValue("Shroomite"))
                Player.shroomiteStealth = true;

            ShroomEnchantActive = true;

        }

        public void SolarEffect()
        {
            if (!Player.GetToggleValue("Solar"))
                return;

            SolarEnchantActive = true;
        }

        public void SpectreEffect(bool hideVisual)
        {
            SpectreEnchantActive = true;

        }

        public void SpectreHeal(NPC npc, Projectile proj)
        {
            if (npc.canGhostHeal && !Player.moonLeech)
            {
                float num = 0.2f;
                num -= proj.numHits * 0.05f;
                if (num <= 0f)
                {
                    return;
                }
                float num2 = proj.damage * num;
                if ((int)num2 <= 0)
                {
                    return;
                }
                if (Main.player[Main.myPlayer].lifeSteal <= 0f)
                {
                    return;
                }
                Main.player[Main.myPlayer].lifeSteal -= num2 * 5; //original damage

                float num3 = 0f;
                int num4 = proj.owner;
                for (int i = 0; i < 255; i++)
                {
                    if (Main.player[i].active && !Main.player[i].dead && ((!Main.player[proj.owner].hostile && !Main.player[i].hostile) || Main.player[proj.owner].team == Main.player[i].team))
                    {
                        float num5 = Math.Abs(Main.player[i].position.X + (Main.player[i].width / 2) - proj.position.X + (proj.width / 2)) + Math.Abs(Main.player[i].position.Y + (Main.player[i].height / 2) - proj.position.Y + (proj.height / 2));
                        if (num5 < 1200f && (Main.player[i].statLifeMax2 - Main.player[i].statLife) > num3)
                        {
                            num3 = (Main.player[i].statLifeMax2 - Main.player[i].statLife);
                            num4 = i;
                        }
                    }
                }
                Projectile.NewProjectile(proj.GetSource_FromThis(), proj.position.X, proj.position.Y, 0f, 0f, ProjectileID.SpiritHeal, 0, 0f, proj.owner, num4, num2);
            }
        }

        public void SpectreHurt(Projectile proj)
        {
            int num = proj.damage / 2;
            if (proj.damage / 2 <= 1)
            {
                return;
            }
            int num2 = 1000;
            if (Main.player[Main.myPlayer].ghostDmg > (float)num2)
            {
                return;
            }
            Main.player[Main.myPlayer].ghostDmg += (float)num;
            int[] array = new int[200];
            int num3 = 0;
            int num4 = 0;
            for (int i = 0; i < 200; i++)
            {
                if (Main.npc[i].CanBeChasedBy(this, false))
                {
                    float num5 = Math.Abs(Main.npc[i].position.X + (Main.npc[i].width / 2) - proj.position.X + (proj.width / 2)) + Math.Abs(Main.npc[i].position.Y + (Main.npc[i].height / 2) - proj.position.Y + (proj.height / 2));
                    if (num5 < 800f)
                    {
                        if (Collision.CanHit(proj.position, 1, 1, Main.npc[i].position, Main.npc[i].width, Main.npc[i].height) && num5 > 50f)
                        {
                            array[num4] = i;
                            num4++;
                        }
                        else if (num4 == 0)
                        {
                            array[num3] = i;
                            num3++;
                        }
                    }
                }
            }
            if (num3 == 0 && num4 == 0)
            {
                return;
            }
            int num6;
            if (num4 > 0)
            {
                num6 = array[Main.rand.Next(num4)];
            }
            else
            {
                num6 = array[Main.rand.Next(num3)];
            }
            float num7 = 4f;
            float num8 = Main.rand.Next(-100, 101);
            float num9 = Main.rand.Next(-100, 101);
            float num10 = (float)Math.Sqrt((double)(num8 * num8 + num9 * num9));
            num10 = num7 / num10;
            num8 *= num10;
            num9 *= num10;
            Projectile.NewProjectile(proj.GetSource_FromThis(), proj.position.X, proj.position.Y, num8, num9, ProjectileID.SpectreWrath, num, 0f, proj.owner, (float)num6, 0);
        }

        public void SpiderEffect(bool hideVisual)
        {
            //minion crits
            SpiderEnchantActive = true;

            /*if (!TinEnchant)
            {
                SummonCrit = 20;
            }*/


        }

        public void SpookyEffect(bool hideVisual)
        {
            //scythe doom
            SpookyEnchantActive = true;

        }

        public void StardustEffect(Item item)
        {
            StardustEnchantActive = true;
            if (Player.ownedProjectileCounts[ProjectileID.StardustGuardian] < 1)
            {
                FargoSoulsUtil.NewSummonProjectile(Player.GetSource_Accessory(item), Player.Center, Vector2.Zero, ProjectileID.StardustGuardian, 30, 10f, Main.myPlayer);
            }

            
            //AddMinion(item, Player.GetToggleValue("Stardust"), ProjectileID.StardustGuardian, 30, 10f);
            //AddPet(Player.GetToggleValue("Stardust"), false, BuffID.StardustGuardianMinion, ProjectileID.StardustGuardian);
            Player.setStardust = true;

            if (FreezeTime && freezeLength > 0)
            {
                if (Main.netMode != NetmodeID.Server)
                {
                    if (!Filters.Scene["FargowiltasSouls:Invert"].IsActive())
                        Filters.Scene.Activate("FargowiltasSouls:Invert");

                    if (Filters.Scene["FargowiltasSouls:Invert"].IsActive())
                        Filters.Scene["FargowiltasSouls:Invert"].GetShader().UseTargetPosition(Player.Center);
                }

                if (FargoSoulsUtil.BossIsAlive(ref EModeGlobalNPC.mutantBoss, ModContent.NPCType<MutantBoss>()))
                {
                    Player.AddBuff(ModContent.BuffType<TimeFrozen>(), freezeLength);

                    if (Main.netMode != NetmodeID.Server && Filters.Scene["FargowiltasSouls:Invert"].IsActive())
                        Filters.Scene["FargowiltasSouls:Invert"].GetShader().UseTargetPosition(Main.npc[EModeGlobalNPC.mutantBoss].Center);
                }

                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    NPC npc = Main.npc[i];
                    if (npc.active && !npc.HasBuff(ModContent.BuffType<TimeFrozen>()))
                        npc.AddBuff(ModContent.BuffType<TimeFrozen>(), freezeLength);
                }

                for (int i = 0; i < Main.maxProjectiles; i++)
                {
                    Projectile p = Main.projectile[i];
                    if (p.active && !(p.minion && !ProjectileID.Sets.MinionShot[p.type]) && !p.GetGlobalProjectile<FargoSoulsGlobalProjectile>().TimeFreezeImmune && p.GetGlobalProjectile<FargoSoulsGlobalProjectile>().TimeFrozen == 0)
                    {
                        p.GetGlobalProjectile<FargoSoulsGlobalProjectile>().TimeFrozen = freezeLength;

                        /*if (p.owner == Player.whoAmI && p.friendly && !p.hostile)
                        {
                            //p.maxPenetrate = 1;
                            if (!p.usesLocalNPCImmunity && !p.usesIDStaticNPCImmunity)
                            {
                                p.usesLocalNPCImmunity = true;
                                p.localNPCHitCooldown = 1;
                            }
                        }*/
                    }
                }

                freezeLength--;

                FargowiltasSouls.Instance.ManageMusicTimestop(freezeLength < 5);

                if (freezeLength == 90)
                {
                    if (!Main.dedServ)
                        Terraria.Audio.SoundEngine.PlaySound(SoundLoader.GetLegacySoundSlot(FargowiltasSouls.Instance, "Sounds/ZaWarudoResume").WithVolume(1f).WithPitchVariance(.5f), Player.Center);
                }

                if (freezeLength <= 0)
                {
                    FreezeTime = false;
                    freezeLength = 540;

                    for (int i = 0; i < Main.maxNPCs; i++)
                    {
                        NPC npc = Main.npc[i];
                        if (npc.active && !npc.dontTakeDamage && npc.life == 1 && npc.lifeMax > 1)
                            npc.StrikeNPC(9999, 0f, 0);
                    }
                }
            }
        }

        public void TikiEffect(bool hideVisual)
        {
            TikiEnchantActive = true;

        }

        private int getNumSentries()
        {
            int count = 0;

            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile p = Main.projectile[i];

                if (p.active && p.owner == Player.whoAmI && p.sentry)
                {
                    count++;
                }
            }

            return count;
        }




        public void TurtleEffect(bool hideVisual)
        {
            Player.turtleThorns = true;
            Player.thorns = 1f;

            TurtleEnchantActive = true;


            if (Player.GetToggleValue("Turtle") && !Player.HasBuff(ModContent.BuffType<BrokenShell>()) 
                && IsStandingStill && !Player.controlUseItem && Player.whoAmI == Main.myPlayer && !noDodge)
            {
                TurtleCounter++;

                if (TurtleCounter > 20)
                {
                    Player.AddBuff(ModContent.BuffType<ShellHide>(), 2);
                }
            }
            else
            {
                TurtleCounter = 0;
            }

            if (TurtleShellHP < 25 && !Player.HasBuff(ModContent.BuffType<BrokenShell>()) && !ShellHide && (LifeForce))
            {
                turtleRecoverCD--;
                if (turtleRecoverCD <= 0)
                {
                    turtleRecoverCD = 240;

                    TurtleShellHP++;
                }
            }

            //Main.NewText($"shell HP: {TurtleShellHP}, counter: {TurtleCounter}, recovery: {turtleRecoverCD}");
        }

        public void ValhallaEffect(bool hideVisual)
        {
            if (!Player.GetToggleValue("SquirePanic"))
                Player.buffImmune[BuffID.BallistaPanic] = true;

            SquireEnchantActive = true;
            ValhallaEnchantActive = true;
        }

        public void VortexEffect(bool hideVisual)
        {
            //portal spawn
            VortexEnchantActive = true;
            //stealth memes
            if (Player.whoAmI == Main.myPlayer && DoubleTap)
            {
                VortexStealth = !VortexStealth;

                if (!Player.GetToggleValue("VortexS"))
                    VortexStealth = false;

                if (Main.netMode == NetmodeID.MultiplayerClient)
                    NetMessage.SendData(MessageID.SyncPlayer, number: Player.whoAmI);

                if (VortexStealth && Player.GetToggleValue("VortexV") && !Player.HasBuff(ModContent.BuffType<VortexCD>()))
                {
                    int p = Projectile.NewProjectile(Player.GetSource_Misc(""), Player.Center.X, Player.Center.Y, 0f, 0f, ModContent.ProjectileType<Projectiles.Souls.Void>(), FargoSoulsUtil.HighestDamageTypeScaling(Player, 60), 5f, Player.whoAmI);
                    Main.projectile[p].GetGlobalProjectile<FargoSoulsGlobalProjectile>().CanSplit = false;
                    Main.projectile[p].netUpdate = true;

                    Player.AddBuff(ModContent.BuffType<VortexCD>(), 3600);
                }
            }

            if (Player.mount.Active)
                VortexStealth = false;

            if (VortexStealth)
            {
                Player.moveSpeed *= 0.3f;
                Player.aggro -= 1200;
                Player.setVortex = true;
                Player.stealth = 0f;
            }
        }







        private int apprenticePrevItem = -1;
        private bool apprenticeSwitchReady = false;
        private bool apprenticeBonusDamage = false;

        public void ApprenticeEffect()
        {
            //shadow shoot meme
            if (Player.GetToggleValue("Apprentice") && Player.controlUseItem)
            {
                Item heldItem = Player.HeldItem;

                //must hold for so long then switch bonus is active
                if (heldItem.type == apprenticePrevItem)
                {
                    ApprenticeCD++;

                    if (ApprenticeCD > 120)
                    {
                        apprenticeSwitchReady = true;

                        //dust
                        int dustId = Dust.NewDust(new Vector2(Player.position.X, Player.position.Y + 2f), Player.width, Player.height + 5, DustID.FlameBurst, 0, 0, 100, Color.Black, 2f);
                        Main.dust[dustId].noGravity = true;
                    }
                }
                else if (apprenticeSwitchReady && heldItem.type != apprenticePrevItem)
                {
                    apprenticeBonusDamage = true;
                }

                apprenticePrevItem = heldItem.type;

                //if (apprenticeCD == 0 && heldItem.damage > 0 && Player.controlUseItem && Player.itemAnimation != 0 && prevPosition != null && heldItem.type != ItemID.ExplosiveBunny && heldItem.type != ItemID.Cannonball
                //&& heldItem.createTile == -1 && heldItem.createWall == -1 && heldItem.ammo == AmmoID.None)
                //{
                //    if (prevPosition != null)
                //    {
                //        Vector2 vel = (Main.MouseWorld - prevPosition).SafeNormalize(-Vector2.UnitY) * 15;

                //        Projectile.NewProjectile(prevPosition, vel, ProjectileID.DD2FlameBurstTowerT3Shot, HighestDamageTypeScaling(heldItem.damage / 2), 1, Player.whoAmI);

                //        for (int i = 0; i < 5; i++)
                //        {
                //            int dustId = Dust.NewDust(new Vector2(prevPosition.X, prevPosition.Y + 2f), Player.width, Player.height + 5, DustID.Shadowflame, 0, 0, 100, Color.Black, 2f);
                //            Main.dust[dustId].noGravity = true;
                //        }
                //    }

                //    prevPosition = Player.position;
                //    apprenticeCD = 20;
                //}

                //if (apprenticeCD > 0)
                //{
                //    apprenticeCD--;
                //}
            }
        }


        

        public void MonkEffect()
        {
            MonkEnchantActive = true;

            if (Player.GetToggleValue("Monk") && !Player.HasBuff(ModContent.BuffType<MonkBuff>()))
            {
                monkTimer++;

                if (monkTimer >= 120)
                {
                    Player.AddBuff(ModContent.BuffType<MonkBuff>(), 2);
                    monkTimer = 0;

                    //dust
                    double spread = 2 * Math.PI / 36;
                    for (int i = 0; i < 36; i++)
                    {
                        Vector2 velocity = new Vector2(2, 2).RotatedBy(spread * i);

                        int index2 = Dust.NewDust(Player.Center, 0, 0, DustID.GoldCoin, velocity.X, velocity.Y, 100);
                        Main.dust[index2].noGravity = true;
                        Main.dust[index2].noLight = true;
                    }
                }
            }
        }

        public void SnowEffect(bool hideVisual)
        {
            SnowEnchantActive = true;

            if (Player.GetToggleValue("Snow"))
            {
                SnowVisual = true;

                if (Player.ownedProjectileCounts[ModContent.ProjectileType<Snowstorm>()] < 1)
                {
                    Vector2 mouse = Main.MouseWorld;
                    Projectile.NewProjectile(Player.GetSource_Misc(""), mouse, Vector2.Zero, ModContent.ProjectileType<Snowstorm>(), 0, 0, Player.whoAmI);
                }

                //int dist = 200;

                //if (FrostEnchant)
                //{
                //    dist = 300;
                //}

                ////dust
                //for (int i = 0; i < 3; i++)
                //{
                //    Vector2 offset = new Vector2();
                //    double angle = Main.rand.NextDouble() * 2d * Math.PI;
                //    offset.X += (float)(Math.Sin(angle) * Main.rand.Next(dist + 1));
                //    offset.Y += (float)(Math.Cos(angle) * Main.rand.Next(dist + 1));
                //    Dust dust = Main.dust[Dust.NewDust(
                //        Player.Center + offset - new Vector2(4, 4), 0, 0,
                //        76, 0, 0, 100, Color.White, .75f)];

                //    dust.noGravity = true;
                //}

                //for (int i = 0; i < 1000; i++)
                //{
                //    Projectile proj = Main.projectile[i];

                //    if (proj.active && proj.hostile && proj.damage > 0 && Vector2.Distance(proj.Center, Player.Center) < dist)
                //    {
                //        proj.GetGlobalProjectile<FargoSoulsGlobalProjectile>().ChilledProj = true;
                //        proj.GetGlobalProjectile<FargoSoulsGlobalProjectile>().ChilledTimer = 30;
                //    }
                //}
            }


        }

        public void AncientShadowEffect()
        {
            //darkness
            AncientShadowEnchantActive = true;


        }

        //        #endregion

        //        #region souls
        public void ColossusSoul(Item item, int maxHP, float damageResist, int lifeRegen, bool hideVisual)
        {
            Player.statLifeMax2 += maxHP;
            Player.endurance += damageResist;
            Player.lifeRegen += lifeRegen;

            //hand warmer, pocket mirror, ankh shield
            Player.buffImmune[BuffID.Chilled] = true;
            Player.buffImmune[BuffID.Frozen] = true;
            Player.buffImmune[BuffID.Stoned] = true;
            Player.buffImmune[BuffID.Weak] = true;
            Player.buffImmune[BuffID.BrokenArmor] = true;
            Player.buffImmune[BuffID.Bleeding] = true;
            Player.buffImmune[BuffID.Poisoned] = true;
            Player.buffImmune[BuffID.Slow] = true;
            Player.buffImmune[BuffID.Confused] = true;
            Player.buffImmune[BuffID.Silenced] = true;
            Player.buffImmune[BuffID.Cursed] = true;
            Player.buffImmune[BuffID.Darkness] = true;
            Player.noKnockback = true;
            Player.fireWalk = true;
            Player.noFallDmg = true;
            //brain of confusion
            if (Player.GetToggleValue("DefenseBrain"))
            {
                Player.brainOfConfusionItem = item;
            }
            //charm of myths
            Player.pStone = true;
            //bee cloak, star veil
            if (Player.GetToggleValue("DefenseStar"))
            {
                Player.starCloakItem = item;
            }
            if (Player.GetToggleValue("DefenseBee"))
            {
                Player.honeyCombItem = item;
            }
            Player.longInvince = true;
            //shiny stone
            Player.shinyStone = true;
            //flesh knuckles
            if (Player.GetToggleValue("DefenseFleshKnuckle", false))
            {
                Player.aggro += 400;
            }

            //frozen turtle shell
            if (Player.statLife <= Player.statLifeMax2 * 0.5) Player.AddBuff(BuffID.IceBarrier, 5, true);
            //paladins shield
            if (Player.GetToggleValue("DefensePaladin", false) && Player.statLife > Player.statLifeMax2 * .25)
            {
                Player.hasPaladinShield = true;
                for (int k = 0; k < Main.maxPlayers; k++)
                {
                    Player target = Main.player[k];

                    if (target.active && Player != target && Vector2.Distance(target.Center, Player.Center) < 400) target.AddBuff(BuffID.PaladinsShield, 30);
                }
            }
        }

        private bool extraCarpetDuration = true;

        public void SupersonicSoul(Item item, bool hideVisual)
        {
            if (Player.GetToggleValue("Supersonic") && !Player.GetModPlayer<FargoSoulsPlayer>().noSupersonic && !FargoSoulsUtil.AnyBossAlive())
            {
                // 5 is the default value, I removed the config for it because the new toggler doesn't have sliders
                Player.runAcceleration += 5f * .1f;
                Player.maxRunSpeed += 5f * 2;
                //frog legs
                Player.autoJump = true;
                Player.jumpSpeedBoost += 2.4f;
                Player.maxFallSpeed += 5f;
                Player.jumpBoost = true;
            }
            else
            {
                //calculated to match flight mastery soul, 6.75 same as frostspark
                Player.accRunSpeed = Player.GetToggleValue("RunSpeed", false) ? 15.6f : 6.75f;
            }

            if (!Player.mount.Active && Player.GetToggleValue("Momentum", false))
            {
                Player.runAcceleration *= 5f;
                Player.runSlowdown *= 5f;

                bool isStillHoldingInSameDirectionAsMovement = (Player.velocity.X > 0 && Player.controlRight) || (Player.velocity.X < 0 && Player.controlLeft);
                if (!isStillHoldingInSameDirectionAsMovement)
                    Player.runSlowdown += 7f;
            }

            Player.moveSpeed += 0.5f;

            if (Player.GetToggleValue("SupersonicRocketBoots", false))
            {
                Player.rocketBoots = Player.vanityRocketBoots = ArmorIDs.RocketBoots.TerrasparkBoots;
                Player.rocketTimeMax = 10;
            }

            Player.iceSkate = true;
            
            //lava waders
            Player.waterWalk = true;
            Player.fireWalk = true;
            Player.lavaImmune = true;
            Player.noFallDmg = true;
            
            //bundle
            if (Player.GetToggleValue("SupersonicJumps") && Player.wingTime == 0)
            {
                Player.hasJumpOption_Cloud = true;
                Player.hasJumpOption_Sandstorm = true;
                Player.hasJumpOption_Blizzard = true;
                Player.hasJumpOption_Fart = true;
            }

            //magic carpet
            if (Player.whoAmI == Main.myPlayer && Player.GetToggleValue("SupersonicCarpet"))
            {
                Player.carpet = true;
                if (Main.netMode == NetmodeID.MultiplayerClient)
                    NetMessage.SendData(MessageID.SyncPlayer, number: Player.whoAmI);

                if (Player.canCarpet)
                {
                    extraCarpetDuration = true;
                }
                else if (extraCarpetDuration)
                {
                    extraCarpetDuration = false;
                    Player.carpetTime = 1000;
                }
            }
            
            //EoC Shield
            if (Player.GetToggleValue("CthulhuShield"))
                Player.dashType = 2;

            //ninja gear
            else if (Player.GetToggleValue("SupersonicTabi", false))
                Player.dashType = 1;
            if (Player.GetToggleValue("BlackBelt"))
                Player.blackBelt = true;
            if (Player.GetToggleValue("SupersonicClimbing"))
                Player.spikedBoots = 2;

            //sweetheart necklace
            if (Player.GetToggleValue("DefenseBee"))
            {
                Player.honeyCombItem = item;
            }
            if (Player.GetToggleValue("DefensePanic"))
            {
                Player.panic = true;
            }

            if (Player.whoAmI == Main.myPlayer && Player.GetToggleValue("MasoAeolusFlower"))
                Player.flowerBoots = true;
        }

        public void FlightMasterySoul()
        {
            Player.wingTimeMax = 999999;
            Player.wingTime = Player.wingTimeMax;
            Player.ignoreWater = true;

            if (Player.GetToggleValue("FlightMasteryInsignia", false)) //soaring insig
                Player.empressBrooch = true;

            if (Player.GetToggleValue("FlightMasteryGravity", false))
                Player.gravity = Player.defaultGravity * 1.5f;

            //hover
            if (Player.controlDown && Player.controlJump && !Player.mount.Active)
            {
                Player.position.Y -= Player.velocity.Y;
                if (Player.velocity.Y > 0.1f)
                    Player.velocity.Y = 0.1f;
                else if (Player.velocity.Y < -0.1f)
                    Player.velocity.Y = -0.1f;
            }

            //grav
            if (Player.GetToggleValue("MasoGrav"))
                Player.gravControl = true;
        }

        public void TrawlerSoul(Item item, bool hideVisual)
        {
            //instacatch
            FishSoul1 = true;
            //extra lures
            if (Player.GetToggleValue("Trawler"))
            {
                FishSoul2 = true;
            }

            //tackle bag
            Player.fishingSkill += 60;
            Player.sonarPotion = true;
            Player.cratePotion = true;
            Player.accFishingLine = true;
            Player.accTackleBox = true;
            Player.accFishFinder = true;

            //spore sac
            if (Player.whoAmI == Main.myPlayer && Player.GetToggleValue("TrawlerSpore"))
            {
                Player.sporeSac = true;
                Player.SporeSac(item);
            }

            //arctic diving gear
            Player.arcticDivingGear = true;
            Player.accFlipper = true;
            Player.accDivingHelm = true;
            Player.iceSkate = true;
            if (Player.wet)
            {
                Lighting.AddLight((int)Player.Center.X / 16, (int)Player.Center.Y / 16, 0.2f, 0.8f, 0.9f);
            }

            //sharkron balloon
            if (Player.GetToggleValue("TrawlerJump") && Player.wingTime == 0)
                Player.hasJumpOption_Sail = true;

            Player.jumpBoost = true;
            Player.noFallDmg = true;
        }

        public void WorldShaperSoul(bool hideVisual)
        {
            //mining speed, spelunker, dangersense, light, hunter, pet
            MinerEnchant.MinerEffect(Player, .66f);
            //placing speed up
            Player.tileSpeed += 0.5f;
            Player.wallSpeed += 0.5f;
            //toolbox
            if (Player.whoAmI == Main.myPlayer)
            {
                Player.tileRangeX += 10;
                Player.tileRangeY += 10;
            }
            //gizmo pack
            Player.autoPaint = true;
            //presserator
            Player.autoActuator = true;
            //royal gel
            Player.npcTypeNoAggro[1] = true;
            Player.npcTypeNoAggro[16] = true;
            Player.npcTypeNoAggro[59] = true;
            Player.npcTypeNoAggro[71] = true;
            Player.npcTypeNoAggro[81] = true;
            Player.npcTypeNoAggro[138] = true;
            Player.npcTypeNoAggro[121] = true;
            Player.npcTypeNoAggro[122] = true;
            Player.npcTypeNoAggro[141] = true;
            Player.npcTypeNoAggro[147] = true;
            Player.npcTypeNoAggro[183] = true;
            Player.npcTypeNoAggro[184] = true;
            Player.npcTypeNoAggro[204] = true;
            Player.npcTypeNoAggro[225] = true;
            Player.npcTypeNoAggro[244] = true;
            Player.npcTypeNoAggro[302] = true;
            Player.npcTypeNoAggro[333] = true;
            Player.npcTypeNoAggro[335] = true;
            Player.npcTypeNoAggro[334] = true;
            Player.npcTypeNoAggro[336] = true;
            Player.npcTypeNoAggro[537] = true;

            if (Player.whoAmI == Main.myPlayer && Player.GetToggleValue("Builder"))
            {
                BuilderMode = true;
                if (Main.netMode == NetmodeID.MultiplayerClient)
                    NetMessage.SendData(MessageID.SyncPlayer, number: Player.whoAmI);

                for (int i = 0; i < TileLoader.TileCount; i++)
                {
                    Player.adjTile[i] = true;
                }

                //placing speed up
                Player.tileSpeed += 0.5f;
                Player.wallSpeed += 0.5f;
                //toolbox
                if (Player.HeldItem.createWall == 0)
                {
                    Player.tileRangeX += 50;
                    Player.tileRangeY += 50;
                }
            }

            //cell phone
            Player.accWatch = 3;
            Player.accDepthMeter = 1;
            Player.accCompass = 1;
            Player.accFishFinder = true;
            Player.accDreamCatcher = true;
            Player.accOreFinder = true;
            Player.accStopwatch = true;
            Player.accCritterGuide = true;
            Player.accJarOfSouls = true;
            Player.accThirdEye = true;
            Player.accCalendar = true;
            Player.accWeatherRadio = true;
        }


        //        #endregion

        //        #region maso acc

        //        #endregion
    }
}
