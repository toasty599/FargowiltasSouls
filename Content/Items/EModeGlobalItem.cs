using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using FargowiltasSouls.Content.Buffs.Masomode;
using FargowiltasSouls.Core.Systems;
using Terraria.DataStructures;
using Microsoft.CodeAnalysis;
using FargowiltasSouls.Content.Projectiles.Souls;
using FargowiltasSouls.Content.Projectiles;
using System;

namespace FargowiltasSouls.Content.Items
{
	public class EModeGlobalItem : GlobalItem
    {
        public override void PickAmmo(Item weapon, Item ammo, Player player, ref int type, ref float speed, ref StatModifier damage, ref float knockback)
        {
            if (!WorldSavingSystem.EternityMode)
                return;
            //ammo nerf
            //if (ammo.ammo == AmmoID.Arrow || ammo.ammo == AmmoID.Bullet || ammo.ammo == AmmoID.Dart)
            //{
            //    damage -= (int)Math.Round(ammo.damage * player.GetDamage(DamageClass.Ranged).Additive * 0.5, MidpointRounding.AwayFromZero); //always round up
            //}
        }
        public override void HoldItem(Item item, Player player)
        {
            if (!WorldSavingSystem.EternityMode)
            {
                base.HoldItem(item, player);
                return;
            }
            EModePlayer ePlayer = player.GetModPlayer<EModePlayer>();
            if (item.type == ItemID.MythrilHalberd || item.type == ItemID.MythrilSword)
            {
                if (!player.ItemAnimationActive)
                {
                    if (ePlayer.MythrilHalberdTimer < 121)
                        ePlayer.MythrilHalberdTimer++;
                }
                if (player.itemAnimation == 1) //reset on last frame
                {
                    ePlayer.MythrilHalberdTimer = 0;
                }

                if (ePlayer.MythrilHalberdTimer == 120 && player.whoAmI == Main.myPlayer)
                {
                    SoundEngine.PlaySound(new SoundStyle("FargowiltasSouls/Assets/Sounds/ChargeSound"), player.Center);
                }
            }
            else
            {
                ePlayer.MythrilHalberdTimer = 0;
            }
            base.HoldItem(item, player);
        }
        public override bool CanUseItem(Item item, Player player)
        {
            if (!WorldSavingSystem.EternityMode)
            {
                if (item.type == ItemID.OrichalcumSword) //reset stats to default
                {
                    Item dummy = new Item(ItemID.OrichalcumSword);
                    item.shoot = dummy.shoot;
                    item.shootSpeed = dummy.shootSpeed;
                    dummy.active = false;
                    dummy = null;
                }
                return base.CanUseItem(item, player);
            }
                

            EModePlayer ePlayer = player.GetModPlayer<EModePlayer>();

            if (item.damage <= 0 && (item.type == ItemID.RodofDiscord || item.type == ItemID.ActuationRod || item.type == ItemID.WireKite || item.type == ItemID.WireCutter || item.type == ItemID.Wrench || item.type == ItemID.BlueWrench || item.type == ItemID.GreenWrench || item.type == ItemID.MulticolorWrench || item.type == ItemID.YellowWrench || item.type == ItemID.Actuator))
            {
                //either player is affected by lihzahrd curse, or cursor is targeting a place in temple (player standing outside)
                if (player.GetModPlayer<FargoSoulsPlayer>().LihzahrdCurse || Framing.GetTileSafely(Main.MouseWorld).WallType == WallID.LihzahrdBrickUnsafe && !player.buffImmune[ModContent.BuffType<LihzahrdCurseBuff>()])
                    return false;
            }

            if (item.type == ItemID.RodofDiscord && FargoSoulsUtil.AnyBossAlive())
            {
                player.chaosState = true;
            }

            if (item.type == ItemID.OrichalcumSword)
            {
                item.shoot = ProjectileID.FlowerPetal;
                item.shootSpeed = 5;
            }
            if (item.type == ItemID.CobaltSword)
            {
                ePlayer.CobaltHitCounter = 0;
            }

            if (item.type == ItemID.RodOfHarmony && FargoSoulsUtil.AnyBossAlive())
            {
                player.hurtCooldowns[0] = 0;
                var defense = player.statDefense;
                float endurance = player.endurance;
                player.statDefense.FinalMultiplier *= 0;
                player.endurance = 0;
                player.Hurt(PlayerDeathReason.ByCustomReason(player.name + " didn't materialize."), player.statLifeMax2 / 7, 0, false, false, 0, false);
                player.statDefense = defense;
                player.endurance = endurance;
                
            }

            if (item.healMana > 0)
            {
                return !player.HasBuff(BuffID.ManaSickness);
            }

            return base.CanUseItem(item, player);
        }
        public override void GetHealMana(Item item, Player player, bool quickHeal, ref int healValue)
        {
            if (WorldSavingSystem.EternityMode)
            {
                healValue = (int)(healValue * 1.5f);
            }
            base.GetHealMana(item, player, quickHeal, ref healValue);
        }
        public override bool? UseItem(Item item, Player player)
        {
            if (!WorldSavingSystem.EternityMode)
                return base.UseItem(item, player);
            EModePlayer ePlayer = player.GetModPlayer<EModePlayer>();

            if (item.type == ItemID.MechdusaSummon && Main.zenithWorld)
            {
                Main.time = 18000;
            }
            
            return base.UseItem(item, player);
        }
        public override void ModifyWeaponDamage(Item item, Player player, ref StatModifier damage)
        {
            if (player.GetModPlayer<EModePlayer>().MythrilHalberdTimer >= 120 && (item.type == ItemID.MythrilSword))
            {
                damage *= 3;
            }
        }
        public override bool Shoot(Item item, Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (!WorldSavingSystem.EternityMode)
                return base.Shoot(item, player, source, position, velocity, type, damage, knockback);
            switch (item.type)
            {
                case ItemID.OrichalcumSword:
                    {
                        for (int i = 0; i < 2; i++)
                        {
                            Projectile.NewProjectile(item.GetSource_FromThis(), position, velocity.RotatedByRandom(MathHelper.Pi / 14), type, damage / 2, knockback / 2, Main.myPlayer);
                        }
                        return false;
                    }
            }
            return base.Shoot(item, player, source, position, velocity, type, damage, knockback);
        }
        public override void OnHitNPC(Item item, Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (!WorldSavingSystem.EternityMode)
                return;
            EModePlayer ePlayer = player.GetModPlayer<EModePlayer>();
            switch (item.type)
            {
                case ItemID.CobaltSword:
                    if (ePlayer.CobaltHitCounter < 2) //only twice per swing
                    {
                        Projectile p = FargoSoulsUtil.NewProjectileDirectSafe(player.GetSource_OnHit(target), target.Center, Vector2.Zero, ModContent.ProjectileType<CobaltExplosion>(), hit.Damage / 2, 0f, Main.myPlayer);
                        if (p != null)
                            p.GetGlobalProjectile<FargoSoulsGlobalProjectile>().CanSplit = false;
                        ePlayer.CobaltHitCounter++;
                    }
                    break;
                case ItemID.PalladiumSword:
                    {
                        if (target.type != NPCID.TargetDummy && !target.friendly) //may add more checks here idk
                            player.AddBuff(BuffID.RapidHealing, 60 * 5);
                        break;
                    }
            }
        }
        public override void ModifyShootStats(Item item, Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            if (!WorldSavingSystem.EternityMode)
                return;
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

            if (player.GetModPlayer<EModePlayer>().MythrilHalberdTimer >= 120 && (item.type == ItemID.MythrilHalberd))
            {
                damage *= 3;
                player.GetModPlayer<EModePlayer>().MythrilHalberdTimer = 0;
            }
        }
    }
}