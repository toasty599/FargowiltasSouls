using System;
using System.Collections.Generic;
//using FargowiltasSouls.Buffs.Souls;
//using FargowiltasSouls.Projectiles.Critters;
using FargowiltasSouls.Toggler;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Items
{
    public class FargoGlobalItem : GlobalItem
    {
        public override void SetDefaults(Item item)
        {
            if (item.type == ItemID.Acorn || item.type == ItemID.Bone)
            {
                item.ammo = item.type;
            }
        }

        public override void UpdateAccessory(Item item, Player player, bool hideVisual)
        {
            if (player.manaCost <= 0f) player.manaCost = 0f;
        }

        public override void GrabRange(Item item, Player player, ref int grabRange)
        {
            FargoSoulsPlayer p = player.GetModPlayer<FargoSoulsPlayer>();
            //ignore money, hearts, mana stars
            if (player.GetToggleValue("IronM", false) && player.whoAmI == Main.myPlayer && p.IronEnchantActive && item.type != ItemID.CopperCoin && item.type != ItemID.SilverCoin && item.type != ItemID.GoldCoin && item.type != ItemID.PlatinumCoin && item.type != ItemID.HermesBoots && item.type != ItemID.CandyApple && item.type != ItemID.SoulCake &&
                item.type != ItemID.Star && item.type != ItemID.CandyCane && item.type != ItemID.SugarPlum && item.type != ItemID.Heart)
            {
                grabRange += (p.TerraForce || p.WizardEnchantActive) ? 1000 : 250;

                //half as effective on nebula bois
                if (!p.TerrariaSoul && (item.type == ItemID.NebulaPickup1 || item.type == ItemID.NebulaPickup2 || item.type == ItemID.NebulaPickup3))
                {
                    grabRange -= (p.TerraForce || p.WizardEnchantActive) ? 500 : 125;
                }
            }
        }

        public override void PickAmmo(Item weapon, Item ammo, Player player, ref int type, ref float speed, ref int damage, ref float knockback)
        {
            FargoSoulsPlayer modPlayer = player.GetModPlayer<FargoSoulsPlayer>();

            if (modPlayer.Jammed && weapon.DamageType == DamageClass.Ranged)
                type = ProjectileID.ConfettiGun;

            if (FargoSoulsWorld.EternityMode && (ammo.ammo == AmmoID.Arrow || ammo.ammo == AmmoID.Bullet || ammo.ammo == AmmoID.Dart)) //ammo nerf
            {
                damage -= (int)Math.Round(ammo.damage * player.GetDamage(DamageClass.Ranged).Additive * 0.5, MidpointRounding.AwayFromZero); //always round up
            }
        }

        //        public override bool ConsumeItem(Item item, Player player)
        //        {
        //            FargoSoulsPlayer p = player.GetModPlayer<FargoSoulsPlayer>();

        //            if (item.makeNPC > 0 && (p.WoodForce || p.WizardEnchant) && Main.rand.NextBool())
        //            {
        //                return false;
        //            }

        //            if (p.BuilderMode && (item.createTile != -1 || item.createWall != -1) && item.type != ItemID.PlatinumCoin && item.type != ItemID.GoldCoin)
        //                return false;
        //            return true;
        //        }

        public override void ModifyWeaponKnockback(Item item, Player player, ref StatModifier knockback, ref float flat)
        {
            FargoSoulsPlayer modPlayer = player.GetModPlayer<FargoSoulsPlayer>();

            if (modPlayer.UniverseSoul || modPlayer.Eternity) 
                knockback *= 2;
        }

        public override bool CanUseItem(Item item, Player player)
        {
            FargoSoulsPlayer modPlayer = player.GetModPlayer<FargoSoulsPlayer>();

            if (modPlayer.AdamantiteEnchantActive && modPlayer.AdamantiteCD == 0)
            {
            // ??? tm
            }

            //            if (modPlayer.IronGuard)
            //            {
            //                //Main.NewText($"iron {modPlayer.ironShieldCD}, {modPlayer.ironShieldTimer}");
            //                modPlayer.IronGuard = false;
            //                modPlayer.wasHoldingShield = false;
            //                player.shield_parry_cooldown = 0; //prevent that annoying tick sound
            //                //check is necessary so if player does a real parry then switches to right click weapon, using it won't reset cooldowns
            //                if (modPlayer.ironShieldCD == 40 && modPlayer.ironShieldTimer == 20)
            //                {
            //                    modPlayer.ironShieldCD = 0;
            //                    modPlayer.ironShieldTimer = 0;
            //                }
            //            }

            //            //dont use hotkeys in stasis
            //            if (player.HasBuff(ModContent.BuffType<GoldenStasis>()))
            //            {
            //                if (item.type == ItemID.RodofDiscord)
            //                {
            //                    player.ClearBuff(ModContent.BuffType<Buffs.Souls.GoldenStasis>());
            //                }
            //                else
            //                {
            //                    return false;
            //                }
            //            }

            if (FargoSoulsWorld.EternityMode)
            {
                if (item.type == ItemID.RodofDiscord &&
                    (modPlayer.LihzahrdCurse ||
                    (Framing.GetTileSafely(Main.MouseWorld).WallType == WallID.LihzahrdBrickUnsafe
                    && !player.buffImmune[ModContent.BuffType<Buffs.Masomode.LihzahrdCurse>()])))
                {
                    return false;
                }

                if (modPlayer.LihzahrdCurse &&
                    (item.type == ItemID.WireKite || item.type == ItemID.WireCutter))
                {
                    return false;
                }
            }

            if (item.damage > 0 && (item.DamageType == DamageClass.Melee || item.DamageType == DamageClass.Ranged || item.DamageType == DamageClass.Magic) && item.pick == 0 && item.axe == 0 && item.hammer == 0)
            {
                modPlayer.MasomodeWeaponUseTimer = Math.Max(item.useTime + item.reuseDelay, 30);
            }

            //            if (item.magic && player.GetModPlayer<FargoSoulsPlayer>().ReverseManaFlow)
            //            {
            //                int damage = (int)(item.mana / (1f - player.endurance) + player.statDefense);
            //                player.Hurt(PlayerDeathReason.ByCustomReason(player.name + " was destroyed by their own magic."), damage, 0);
            //                player.immune = false;
            //                player.immuneTime = 0;
            //            }

            //            if (modPlayer.BuilderMode && (item.createTile != -1 || item.createWall != -1) && item.type != ItemID.PlatinumCoin && item.type != ItemID.GoldCoin)
            //            {
            //                item.useTime = 1;
            //                item.useAnimation = 1;
            //            }

            if (item.damage > 0 && player.HasAmmo(item, true) && !(item.mana > 0 && player.statMana < item.mana) //non weapons and weapons with no ammo begone
                && item.type != ItemID.ExplosiveBunny && item.type != ItemID.Cannonball
                && item.useTime > 0 && item.createTile == -1 && item.createWall == -1 && item.ammo == AmmoID.None && item.hammer == 0 && item.pick == 0 && item.axe == 0)
            {
                modPlayer.TryAdditionalAttacks(item.damage, item.DamageType);
            }

            //            //critter attack timer
            //            if (modPlayer.WoodEnchant && player.altFunctionUse == ItemAlternativeFunctionID.ActivatedAndUsed && item.makeNPC > 0)
            //            {
            //                if (modPlayer.CritterAttackTimer == 0)
            //                {
            //                    Vector2 vel = Vector2.Normalize(Main.MouseWorld - player.Center);
            //                    float damageMultiplier = player.GetDamage(DamageClass.Summon);

            //                    int type = -1;
            //                    int damage = 0;
            //                    int attackCooldown = 0;

            //                    switch (item.type)
            //                    {
            //                        //case ItemID.Bunny:
            //                        //    type = ProjectileID.ExplosiveBunny;
            //                        //    damage = 10;
            //                        //    attackCooldown = 10;
            //                        //    break;

            //                        case ItemID.Bird:
            //                            type = ModContent.ProjectileType<BirdProj>();
            //                            damage = 15;
            //                            attackCooldown = 15;
            //                            break;

            //                        case ItemID.BlueJay:
            //                            type = ModContent.ProjectileType<BlueJayProj>();
            //                            damage = 10;
            //                            attackCooldown = 10;
            //                            break;

            //                        case ItemID.Cardinal:
            //                            type = ModContent.ProjectileType<CardinalProj>();
            //                            damage = 20;
            //                            attackCooldown = 20;
            //                            break;
            //                    }

            //                    if (type != -1)
            //                    {
            //                        Projectile.NewProjectile(player.Center, vel * 2f, type, damage, 2, player.whoAmI);
            //                        modPlayer.CritterAttackTimer = attackCooldown;
            //                    }


            //                }





            //                return false;
            //            }

            if (item.type == ItemID.RodofDiscord)
            {
                if (FargoSoulsWorld.EternityMode && FargoSoulsUtil.AnyBossAlive())
                {
                    /*player.AddBuff(ModContent.BuffType<Buffs.Masomode.ChaosLife>(), 30);
                    modPlayer.MaxLifeReduction += 100;*/
                    player.chaosState = true;

                    /*player.statLife -= player.statLifeMax2 / 5;
                    PlayerDeathReason damageSource = PlayerDeathReason.ByOther(13);
                    if (Main.rand.NextBool())
                        damageSource = PlayerDeathReason.ByOther(player.Male ? 14 : 15);
                    if (player.statLife <= 0 && !player.chaosState) //since chaos state will check and kill anyway, avoid doublekill
                        player.KillMe(damageSource, 1, 0);
                    player.lifeRegenCount = 0;
                    player.lifeRegenTime = 0;*/
                }

                if (player.chaosState)
                    player.GetModPlayer<FargoSoulsPlayer>().WasHurtBySomething = true; //with abom rebirth, die to chaos state
            }

            return true;
        }

        public override bool? UseItem(Item item, Player player)
        {
            FargoSoulsPlayer modPlayer = player.GetModPlayer<FargoSoulsPlayer>();

            if (item.type == ItemID.RodofDiscord)
                player.ClearBuff(ModContent.BuffType<Buffs.Souls.GoldenStasis>());

            return base.UseItem(item, player);
        }

        //        public override bool AltFunctionUse(Item item, Player player)
        //        {
        //            FargoSoulsPlayer modPlayer = player.GetModPlayer<FargoSoulsPlayer>();

        //            if (modPlayer.WoodEnchant)
        //            {
        //                switch (item.type)
        //                {
        //                    case ItemID.Bunny:
        //                    case ItemID.Bird:
        //                    case ItemID.BlueJay:
        //                    case ItemID.Cardinal:
        //                        return true;

        //                }
        //            }



        //            return base.AltFunctionUse(item, player);
        //        }

        //        public override bool NewPreReforge(Item item)
        //        {
        //            /*if (Main.player[item.owner].GetModPlayer<FargoSoulsPlayer>().SecurityWallet)
        //            {
        //                switch(item.prefix)
        //                {
        //                    case PrefixID.Warding:  if (SoulConfig.Instance.walletToggles.Warding)  return false; break;
        //                    case PrefixID.Violent:  if (SoulConfig.Instance.walletToggles.Violent)  return false; break;
        //                    case PrefixID.Quick:    if (SoulConfig.Instance.walletToggles.Quick)    return false; break;
        //                    case PrefixID.Lucky:    if (SoulConfig.Instance.walletToggles.Lucky)    return false; break;
        //                    case PrefixID.Menacing: if (SoulConfig.Instance.walletToggles.Menacing) return false; break;
        //                    case PrefixID.Legendary:if (SoulConfig.Instance.walletToggles.Legendary)return false; break;
        //                    case PrefixID.Unreal:   if (SoulConfig.Instance.walletToggles.Unreal)   return false; break;
        //                    case PrefixID.Mythical: if (SoulConfig.Instance.walletToggles.Mythical) return false; break;
        //                    case PrefixID.Godly:    if (SoulConfig.Instance.walletToggles.Godly)    return false; break;
        //                    case PrefixID.Demonic:  if (SoulConfig.Instance.walletToggles.Demonic)  return false; break;
        //                    case PrefixID.Ruthless: if (SoulConfig.Instance.walletToggles.Ruthless) return false; break;
        //                    case PrefixID.Light:    if (SoulConfig.Instance.walletToggles.Light)    return false; break;
        //                    case PrefixID.Deadly:   if (SoulConfig.Instance.walletToggles.Deadly)   return false; break;
        //                    case PrefixID.Rapid:    if (SoulConfig.Instance.walletToggles.Rapid)    return false; break;
        //                    default: break;
        //                }
        //            }*/
        //            return true;
        //        }

        public override void ModifyShootStats(Item item, Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            FargoSoulsPlayer modPlayer = player.GetModPlayer<FargoSoulsPlayer>();

            if (modPlayer.Eternity)
                velocity *= 2;
            else if (modPlayer.UniverseSoul)
                velocity *= 1.5f;

            if (FargoSoulsWorld.EternityMode)
            {
                if (!NPC.downedBoss3 && item.type == ItemID.WaterBolt)
                {
                    type = ProjectileID.WaterGun;
                    damage = 0;
                }

                if (!NPC.downedBoss2 && item.type == ItemID.SpaceGun)
                {
                    type = ProjectileID.ConfettiGun;
                    damage = 0;
                }
            }
        }

        //        public override bool ReforgePrice(Item item, ref int reforgePrice, ref bool canApplyDiscount)
        //        {
        //            if (Main.LocalPlayer.GetModPlayer<FargoSoulsPlayer>().SecurityWallet)
        //                reforgePrice /= 2;
        //            return true;
        //        }

        //        //summon variants
        //        private static readonly int[] Summon = { ItemID.NimbusRod, ItemID.CrimsonRod, ItemID.BeeGun, ItemID.WaspGun, ItemID.PiranhaGun, ItemID.BatScepter };

        //        public override bool CanRightClick(Item item)
        //        {
        //            if (Array.IndexOf(Summon, item.type) > -1)
        //            {
        //                return true;
        //            }

        //            return base.CanRightClick(item);
        //        }

        //        public override void RightClick(Item item, Player player)
        //        {
        //            int newType = -1;

        //            if (Array.IndexOf(Summon, item.type) > -1)
        //            {
        //                newType = mod.ItemType(ItemID.GetUniqueKey(item.type).Replace("Terraria ", string.Empty) + "Summon");
        //            }

        //            if (newType != -1)
        //            {
        //                int num = Item.NewItem(player.getRect(), newType, prefixGiven: item.prefix);

        //                if (Main.netMode == NetmodeID.MultiplayerClient)
        //                {
        //                    NetMessage.SendData(MessageID.SyncItem, number: num, number2: 1f);
        //                }
        //            }
        //        }

        public override bool WingUpdate(int wings, Player player, bool inUse)
        {
            FargoSoulsPlayer modPlayer = player.GetModPlayer<FargoSoulsPlayer>();

            if (modPlayer.ChloroEnchantActive && player.GetToggleValue("Jungle") && inUse)
            {
                modPlayer.CanJungleJump = false;

                //spwn cloud
                if (modPlayer.JungleCD == 0)
                {
                    int dmg = (modPlayer.NatureForce || modPlayer.WizardEnchantActive) ? 150 : 75;
                    Terraria.Audio.SoundEngine.PlaySound(SoundID.Item, (int)player.position.X, (int)player.position.Y, 62, 0.5f);
                    FargoSoulsUtil.XWay(10, player.GetProjectileSource_Accessory(modPlayer.ChloroEnchantItem), new Vector2(player.Center.X, player.Center.Y + (player.height / 2)), ProjectileID.SporeCloud, 3f, FargoSoulsUtil.HighestDamageTypeScaling(player, dmg), 0);

                    modPlayer.JungleCD = 8;
                }
            }

            return base.WingUpdate(wings, player, inUse);
        }

        public override void VerticalWingSpeeds(Item item, Player player, ref float ascentWhenFalling, ref float ascentWhenRising, ref float maxCanAscendMultiplier, ref float maxAscentMultiplier, ref float constantAscend)
        {
            base.VerticalWingSpeeds(item, player, ref ascentWhenFalling, ref ascentWhenRising, ref maxCanAscendMultiplier, ref maxAscentMultiplier, ref constantAscend);

            //Main.NewText($"vertical: {ascentWhenFalling} {ascentWhenRising} {maxCanAscendMultiplier} {maxAscentMultiplier} {constantAscend}");
        }

        public override void HorizontalWingSpeeds(Item item, Player player, ref float speed, ref float acceleration)
        {
            base.HorizontalWingSpeeds(item, player, ref speed, ref acceleration);

            //Main.NewText($"horiz: {speed} {acceleration}");
        }

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            /*if (Array.IndexOf(Summon, item.type) > -1)
            {
                TooltipLine helperLine = new TooltipLine(mod, "help", "Right click to convert");
                tooltips.Add(helperLine);
            }*/

            if (FargoSoulsWorld.EternityMode)
            {
                if (item.damage > 0 && (item.ammo == AmmoID.Arrow || item.ammo == AmmoID.Bullet || item.ammo == AmmoID.Dart))
                {
                    tooltips.Add(new TooltipLine(FargowiltasSouls.Instance, "masoAmmoNerf", "[c/ff0000:Eternity Mode:] Contributes 50% less damage to weapons"));
                }

                switch (item.type)
                {
                    case ItemID.RodofDiscord:
                        tooltips.Add(new TooltipLine(FargowiltasSouls.Instance, "masoNerf", "[c/ff0000:Eternity Mode:] During boss fights, every use takes life"));
                        break;

                    case ItemID.ArcheryPotion:
                    case ItemID.MagicQuiver:
                    case ItemID.ShroomiteHelmet:
                    case ItemID.ShroomiteHeadgear:
                    case ItemID.ShroomiteMask:
                        tooltips.Add(new TooltipLine(FargowiltasSouls.Instance, "masoNerf", "[c/ff0000:Eternity Mode:] Grants additive damage instead of multiplicative"));
                        break;

                    case ItemID.CrystalBullet:
                    case ItemID.HolyArrow:
                        tooltips.Add(new TooltipLine(FargowiltasSouls.Instance, "masoNerf2", "[c/ff0000:Eternity Mode:] Can only split 4 times per second"));
                        break;

                    case ItemID.ChlorophyteBullet:
                        tooltips.Add(new TooltipLine(FargowiltasSouls.Instance, "masoNerf2", "[c/ff0000:Eternity Mode:] Reduced speed, duration, and damage"));
                        break;

                    case ItemID.WaterBolt:
                        if (!NPC.downedBoss3)
                            tooltips.Add(new TooltipLine(FargowiltasSouls.Instance, "masoNerf", "[c/ff0000:Eternity Mode:] Cannot be used until Skeletron is defeated"));
                        break;

                    case ItemID.HallowedGreaves:
                    case ItemID.HallowedHeadgear:
                    case ItemID.HallowedHelmet:
                    case ItemID.HallowedHood:
                    case ItemID.HallowedMask:
                    case ItemID.HallowedPlateMail:
                    case ItemID.AncientHallowedGreaves:
                    case ItemID.AncientHallowedHeadgear:
                    case ItemID.AncientHallowedHelmet:
                    case ItemID.AncientHallowedHood:
                    case ItemID.AncientHallowedMask:
                    case ItemID.AncientHallowedPlateMail:
                        tooltips.Add(new TooltipLine(FargowiltasSouls.Instance, "masoNerf", "[c/ff0000:Eternity Mode:] Holy Dodge activation will temporarily reduce your damage"));
                        break;

                    case ItemID.SpectreHood:
                        tooltips.Add(new TooltipLine(FargowiltasSouls.Instance, "masoNerf", "[c/ff0000:Eternity Mode:] Healing orbs move slower and disappear quickly"));
                        break;

                    case ItemID.FrozenTurtleShell:
                        tooltips.Add(new TooltipLine(FargowiltasSouls.Instance, "masoNerf", "[c/ff0000:Eternity Mode:] Damage reduction is 15% instead of 25%"));
                        break;

                    case ItemID.Zenith:
                        if (FargoSoulsWorld.downedMutant)
                        {
                            tooltips.Add(new TooltipLine(FargowiltasSouls.Instance, "masoNerf", "[c/ffff00:Eternity Mode:] No nerfs in effect"));
                        }
                        else
                        {
                            string bossesToKill = "";
                            if (!FargoSoulsWorld.downedAbom)
                            {
                                if (!FargoSoulsWorld.downedChampions[(int)FargoSoulsWorld.Downed.CosmosChampion])
                                    bossesToKill += " Eridanus,";

                                bossesToKill += " Abominationn,";
                            }
                            bossesToKill += " Mutant";
                            tooltips.Add(new TooltipLine(FargowiltasSouls.Instance, "masoNerf", $"[c/ffaa00:Eternity Mode:] Vastly reduced hit rate, improves by defeating:{bossesToKill}"));
                        }
                        break;

                    case ItemID.StardustCellStaff:
                    case ItemID.EmpressBlade:
                        tooltips.Add(new TooltipLine(FargowiltasSouls.Instance, "masoNerf", "[c/ff0000:Eternity Mode:] Cell damage slightly reduced as more are summoned"));
                        break;

                    case ItemID.DD2BetsyBow:
                    case ItemID.Uzi:
                    case ItemID.PhoenixBlaster:
                    case ItemID.Handgun:
                    case ItemID.SpikyBall:
                    case ItemID.Xenopopper:
                    case ItemID.PainterPaintballGun:
                    case ItemID.MoltenFury:
                        tooltips.Add(new TooltipLine(FargowiltasSouls.Instance, "masoNerf", "[c/ff0000:Eternity Mode:] Reduced damage by 25%"));
                        break;

                    case ItemID.SkyFracture:
                    case ItemID.SnowmanCannon:
                        tooltips.Add(new TooltipLine(FargowiltasSouls.Instance, "masoNerf", "[c/ff0000:Eternity Mode:] Reduced damage by 20%"));
                        break;

                    case ItemID.StarCannon:
                    case ItemID.ElectrosphereLauncher:
                    case ItemID.DaedalusStormbow:
                    case ItemID.BeesKnees:
                    case ItemID.LaserMachinegun:
                        tooltips.Add(new TooltipLine(FargowiltasSouls.Instance, "masoNerf", "[c/ff0000:Eternity Mode:] Reduced damage by 33%"));
                        break;

                    case ItemID.Beenade:
                    case ItemID.Razorpine:
                        tooltips.Add(new TooltipLine(FargowiltasSouls.Instance, "masoNerf", "[c/ff0000:Eternity Mode:] Reduced damage by 33%"));
                        tooltips.Add(new TooltipLine(FargowiltasSouls.Instance, "masoNerf2", "[c/ff0000:Eternity Mode:] Reduced attack speed by 33%"));
                        break;

                    case ItemID.Tsunami:
                    case ItemID.Flairon:
                    case ItemID.ChlorophyteShotbow:
                    case ItemID.HellwingBow:
                    case ItemID.DartPistol:
                    case ItemID.DartRifle:
                    case ItemID.Megashark:
                    case ItemID.BatScepter:
                    case ItemID.ChainGun:
                    case ItemID.VortexBeater:
                    case ItemID.RavenStaff:
                    case ItemID.XenoStaff:
                    case ItemID.StardustDragonStaff:
                    case ItemID.Phantasm:
                    case ItemID.NebulaArcanum:
                        tooltips.Add(new TooltipLine(FargowiltasSouls.Instance, "masoNerf", "[c/ff0000:Eternity Mode:] Reduced damage by 15%"));
                        break;

                    case ItemID.VampireKnives:
                        tooltips.Add(new TooltipLine(FargowiltasSouls.Instance, "masoNerf", "[c/ff0000:Eternity Mode:] Reduced damage by 25%"));
                        tooltips.Add(new TooltipLine(FargowiltasSouls.Instance, "masoNerf2", "[c/ff0000:Eternity Mode:] Reduced attack speed by 25%"));
                        tooltips.Add(new TooltipLine(FargowiltasSouls.Instance, "masoNerf3", "[c/ff0000:Eternity Mode:] Reduced lifesteal rate when above 33% life"));
                        break;

                    case ItemID.BlizzardStaff:
                        tooltips.Add(new TooltipLine(FargowiltasSouls.Instance, "masoNerf", "[c/ff0000:Eternity Mode:] Reduced damage by 33%"));
                        tooltips.Add(new TooltipLine(FargowiltasSouls.Instance, "masoNerf2", "[c/ff0000:Eternity Mode:] Reduced attack speed by 50%"));
                        break;

                    case ItemID.DemonScythe:
                        if (NPC.downedBoss2)
                        {
                            tooltips.Add(new TooltipLine(FargowiltasSouls.Instance, "masoNerf", "[c/ff0000:Eternity Mode:] Reduced damage by 33%"));
                        }
                        else
                        {
                            tooltips.Add(new TooltipLine(FargowiltasSouls.Instance, "masoNerf", "[c/ff0000:Eternity Mode:] Reduced damage by 50% until an evil boss is defeated"));
                            tooltips.Add(new TooltipLine(FargowiltasSouls.Instance, "masoNerf2", "[c/ff0000:Eternity Mode:] Reduced attack speed by 25% until an evil boss is defeated"));
                        }
                        break;

                    case ItemID.SpaceGun:
                        if (NPC.downedBoss2)
                        {
                            tooltips.Add(new TooltipLine(FargowiltasSouls.Instance, "masoNerf", "[c/ff0000:Eternity Mode:] Reduced damage by 15%"));
                        }
                        else
                        {
                            tooltips.Add(new TooltipLine(FargowiltasSouls.Instance, "masoNerf", "[c/ff0000:Eternity Mode:] Cannot be used until an evil boss is defeated"));
                        }
                        break;

                    case ItemID.BeeGun:
                    case ItemID.Grenade:
                    case ItemID.StickyGrenade:
                    case ItemID.BouncyGrenade:
                        tooltips.Add(new TooltipLine(FargowiltasSouls.Instance, "masoNerf2", "[c/ff0000:Eternity Mode:] Reduced attack speed by 33%"));
                        break;

                    case ItemID.DD2BallistraTowerT1Popper:
                    case ItemID.DD2BallistraTowerT2Popper:
                    case ItemID.DD2BallistraTowerT3Popper:
                    case ItemID.DD2ExplosiveTrapT1Popper:
                    case ItemID.DD2ExplosiveTrapT2Popper:
                    case ItemID.DD2ExplosiveTrapT3Popper:
                    case ItemID.DD2FlameburstTowerT1Popper:
                    case ItemID.DD2FlameburstTowerT2Popper:
                    case ItemID.DD2FlameburstTowerT3Popper:
                    case ItemID.DD2LightningAuraT1Popper:
                    case ItemID.DD2LightningAuraT2Popper:
                    case ItemID.DD2LightningAuraT3Popper:
                        tooltips.Add(new TooltipLine(FargowiltasSouls.Instance, "masoNerf", "[c/ff0000:Eternity Mode:] Reduced attack speed by 33%"));
                        break;

                    case ItemID.MonkAltHead:
                    case ItemID.MonkAltPants:
                    case ItemID.MonkAltShirt:
                    case ItemID.MonkBrows:
                    case ItemID.MonkPants:
                    case ItemID.MonkShirt:
                    case ItemID.SquireAltHead:
                    case ItemID.SquireAltPants:
                    case ItemID.SquireAltShirt:
                    case ItemID.SquireGreatHelm:
                    case ItemID.SquireGreaves:
                    case ItemID.SquirePlating:
                    case ItemID.HuntressAltHead:
                    case ItemID.HuntressAltPants:
                    case ItemID.HuntressAltShirt:
                    case ItemID.HuntressJerkin:
                    case ItemID.HuntressPants:
                    case ItemID.HuntressWig:
                    case ItemID.ApprenticeAltHead:
                    case ItemID.ApprenticeAltPants:
                    case ItemID.ApprenticeAltShirt:
                    case ItemID.ApprenticeHat:
                    case ItemID.ApprenticeRobe:
                    case ItemID.ApprenticeTrousers:
                    case ItemID.AncientBattleArmorHat:
                    case ItemID.AncientBattleArmorPants:
                    case ItemID.AncientBattleArmorShirt:
                        tooltips.Add(new TooltipLine(FargowiltasSouls.Instance, "masoBuff", "[c/00ff00:Eternity Mode:] Set bonus increases minimum summon damage when you attack using other classes"));
                        break;

                    default:
                        break;
                }

                if (item.DamageType == DamageClass.Summon)
                    tooltips.Add(new TooltipLine(FargowiltasSouls.Instance, "masoMinionNerf", "[c/ff0000:Eternity Mode:] Summon damage decreases when you attack using other classes (except in OOA)"));
            }
        }
    }
}