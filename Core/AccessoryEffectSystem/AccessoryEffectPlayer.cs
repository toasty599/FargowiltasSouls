using FargowiltasSouls.Content.Bosses.AbomBoss;
using FargowiltasSouls.Content.Bosses.DeviBoss;
using FargowiltasSouls.Content.Bosses.MutantBoss;
using FargowiltasSouls.Content.Buffs.Masomode;
using FargowiltasSouls.Content.Buffs.Souls;
using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using FargowiltasSouls.Content.Items.Accessories.Expert;
using FargowiltasSouls.Content.Items.Accessories.Masomode;
using FargowiltasSouls.Content.Items.Accessories.Souls;
using FargowiltasSouls.Content.Items.Armor;
using FargowiltasSouls.Content.Projectiles.Masomode;
using FargowiltasSouls.Content.Projectiles.Minions;
using FargowiltasSouls.Content.Projectiles.Souls;
using FargowiltasSouls.Content.Projectiles;
using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.Systems;
using FargowiltasSouls.Core.Toggler;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.Collision;
using FargowiltasSouls.Content.Items.Consumables;
using FargowiltasSouls.Content.Items.Weapons.Challengers;
using FargowiltasSouls.Content.Items.Weapons.SwarmDrops;
using System.Data;
using Terraria.DataStructures;
using Terraria.Graphics.Shaders;
using Terraria.UI;
using Terraria.WorldBuilding;
using Microsoft.CodeAnalysis;
using Terraria.ModLoader.Core;

namespace FargowiltasSouls.Core.AccessoryEffectSystem
{
    public class AccessoryEffectPlayer : ModPlayer
    {
        public HashSet<AccessoryEffect> ActiveEffects = new();
        public Dictionary<AccessoryEffect, Item> EffectItems;
        internal EffectFields[] EffectInstances;
        public override void Initialize()
        {
            int instanceCount = AccessoryEffectLoader.EffectFields.Count;
            EffectInstances = new EffectFields[instanceCount];
            for (int i = 0; i < instanceCount; i++)
            {
                EffectInstances[i] = AccessoryEffectLoader.EffectFields[i];
            }
        }
        #region Hooks
        public override void ResetEffects()
        {
            EffectItems.Clear();
            ActiveEffects.Clear();
            foreach (EffectFields effectInstance in EffectInstances)
            {
                effectInstance.ResetEffects();
            }
        }
        public override void UpdateDead()
        {
            ResetEffects();
            foreach (EffectFields effectInstance in EffectInstances)
            {
                effectInstance.UpdateDead();
            }
        }

        public override void PreUpdate()
        {
            foreach (AccessoryEffect effect in ActiveEffects)
            {
                effect.PreUpdate(Player);
            }
        }

        public override void PostUpdateEquips()
        {
            foreach (AccessoryEffect effect in ActiveEffects)
            {
                effect.PostUpdateEquips(Player);
            }
        }
        public override void UpdateBadLifeRegen()
        {
            foreach (AccessoryEffect effect in ActiveEffects)
            {
                effect.UpdateBadLifeRegen(Player);
            }
        }
        public override void PostUpdate()
        {
            foreach (AccessoryEffect effect in ActiveEffects)
            {
                effect.PostUpdate(Player);
            }
        }

        public override void PostUpdateMiscEffects()
        {
            foreach (AccessoryEffect effect in ActiveEffects)
            {
                effect.PostUpdateMiscEffects(Player);
            }
        }

        public void TryAdditionalAttacks(int damage, DamageClass damageType)
        {
            foreach (AccessoryEffect effect in ActiveEffects)
            {
                effect.TryAdditionalAttacks(Player, damage, damageType);
            }
        }
        public override void ModifyHitNPCWithProj(Projectile proj, NPC target, ref NPC.HitModifiers modifiers)
        {
            if (proj.hostile)
                return;

            foreach (AccessoryEffect effect in ActiveEffects)
            {
                effect.ModifyHitNPCWithProj(Player, proj, target, ref modifiers);
            }

            ModifyHitNPCBoth(target, ref modifiers, proj.DamageType);
        }

        public override void ModifyHitNPCWithItem(Item item, NPC target, ref NPC.HitModifiers modifiers)
        {
            foreach (AccessoryEffect effect in ActiveEffects)
            {
                effect.ModifyHitNPCWithItem(Player, item, target, ref modifiers);
            }

            ModifyHitNPCBoth(target, ref modifiers, item.DamageType);
        }
        public void ModifyHitNPCBoth(NPC target, ref NPC.HitModifiers modifiers, DamageClass damageClass)
        {
            modifiers.ModifyHitInfo += (ref NPC.HitInfo hitInfo) =>
            {
                ModifyHitInfo(target, ref hitInfo, damageClass);
            };
            foreach (AccessoryEffect effect in ActiveEffects)
            {
                effect.ModifyHitNPCBoth(Player, target, ref modifiers, damageClass);
            }
        }
        public void ModifyHitInfo(NPC target, ref NPC.HitInfo hitInfo, DamageClass damageClass)
        {
            foreach (AccessoryEffect effect in ActiveEffects)
            {
                effect.ModifyHitInfo(Player, target, ref hitInfo, damageClass);
            }
        }

        public override void OnHitNPCWithProj(Projectile proj, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (target.type == NPCID.TargetDummy || target.friendly)
                return;

            foreach (AccessoryEffect effect in ActiveEffects)
            {
                effect.OnHitNPCWithProj(Player, proj, target, hit, damageDone);
            }

            if (proj.minion)// && proj.type != ModContent.ProjectileType<CelestialRuneAncientVision>() && proj.type != ModContent.ProjectileType<SpookyScythe>())
                TryAdditionalAttacks(proj.damage, proj.DamageType);

            OnHitNPCEither(target, hit, proj.DamageType, projectile: proj);

        }
        public override void OnHitNPCWithItem(Item item, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (target.type == NPCID.TargetDummy || target.friendly)
                return;

            foreach (AccessoryEffect effect in ActiveEffects)
            {
                effect.OnHitNPCWithItem(Player, item, target, hit, damageDone);
            }

            OnHitNPCEither(target, hit, item.DamageType, item: item);
        }
        private void OnHitNPCEither(NPC target, NPC.HitInfo hitInfo, DamageClass damageClass, Projectile projectile = null, Item item = null)
        {

            //doing this so that damage-inheriting effects dont double dip or explode due to taking on crit boost
            int GetBaseDamage()
            {
                // TODO: I guess? test this
                int baseDamage = hitInfo.SourceDamage;
                if (projectile != null)
                    baseDamage = projectile.damage;
                else if (item != null)
                    baseDamage = Player.GetWeaponDamage(item);
                return baseDamage;
            }
            int baseDamage = GetBaseDamage();
            foreach (AccessoryEffect effect in ActiveEffects)
            {
                effect.OnHitNPCEither(Player, target, hitInfo, damageClass, baseDamage, projectile, item);
            }
        }


        public override void MeleeEffects(Item item, Rectangle hitbox)
        {
            foreach (AccessoryEffect effect in ActiveEffects)
            {
                effect.MeleeEffects(Player, item, hitbox);
            }
        }
        public float ModifyUseSpeed(Item item)
        {
            float speedModifier = 0;
            foreach (AccessoryEffect effect in ActiveEffects)
            {
                speedModifier += effect.ModifyUseSpeed(Player, item);
            }
            return speedModifier;
        }
        public float ContactDamageDR(NPC npc, ref Player.HurtModifiers modifiers)
        {
            float dr = 0;
            foreach (AccessoryEffect effect in ActiveEffects)
            {
                dr += effect.ContactDamageDR(Player, npc, ref modifiers);
            }
            return dr;
        }
        public float ProjectileDamageDR(Projectile projectile, ref Player.HurtModifiers modifiers)
        {
            float dr = 0;
            foreach (AccessoryEffect effect in ActiveEffects)
            {
                dr += effect.ProjectileDamageDR(Player, projectile, ref modifiers);
            }
            return dr;
        }
        public override void ModifyHitByNPC(NPC npc, ref Player.HurtModifiers modifiers)
        {
            foreach (AccessoryEffect effect in ActiveEffects)
            {
                effect.ModifyHitByNPC(Player, npc, ref modifiers);
            }
        }

        public override void ModifyHitByProjectile(Projectile proj, ref Player.HurtModifiers modifiers)
        {
            foreach (AccessoryEffect effect in ActiveEffects)
            {
                effect.ModifyHitByProjectile(Player, proj, ref modifiers);
            }
        }

        public override void OnHitByNPC(NPC npc, Player.HurtInfo hurtInfo)
        {
            foreach (AccessoryEffect effect in ActiveEffects)
            {
                effect.OnHitByNPC(Player, npc, hurtInfo);
            }
            OnHitByEither(npc, null);
        }

        public override void OnHitByProjectile(Projectile proj, Player.HurtInfo hurtInfo)
        {
            foreach (AccessoryEffect effect in ActiveEffects)
            {
                effect.OnHitByProjectile(Player, proj, hurtInfo);
            }
            OnHitByEither(null, proj);
        }

        public void OnHitByEither(NPC npc, Projectile proj)
        {
            foreach (AccessoryEffect effect in ActiveEffects)
            {
                effect.OnHitByEither(Player, npc, proj);
            }
        }

        public override bool CanBeHitByNPC(NPC npc, ref int CooldownSlot)
        {
            bool result = true;
            foreach (AccessoryEffect effect in ActiveEffects)
            {
                if (!effect.CanBeHitByNPC(Player, npc))
                {
                    result = false;
                }
            }
            return result;
        }

        public override bool CanBeHitByProjectile(Projectile proj)
        {
            bool result = true;
            foreach (AccessoryEffect effect in ActiveEffects)
            {
                if (!effect.CanBeHitByProjectile(Player, proj))
                {
                    result = false;
                }
            }
            return result;
        }
        public override void ModifyHurt(ref Player.HurtModifiers modifiers)/* tModPorter Override ImmuneTo, FreeDodge or ConsumableDodge instead to prevent taking damage */
        {
            foreach (AccessoryEffect effect in ActiveEffects)
            {
                effect.ModifyHurt(Player, ref modifiers);
            }
        }
        public override void OnHurt(Player.HurtInfo info)
        {
            foreach (AccessoryEffect effect in ActiveEffects)
            {
                effect.OnHurt(Player, info);
            }
        }
        public override bool PreKill(double damage, int hitDirection, bool pvp, ref bool playSound, ref bool genGore, ref PlayerDeathReason damageSource)
        {
            bool result = true;
            foreach (AccessoryEffect effect in ActiveEffects)
            {
                if (!effect.PreKill(Player, damage, hitDirection, pvp, ref playSound, ref genGore, ref damageSource))
                {
                    result = false;
                }
            }
            return result;
        }
        public override void DrawEffects(PlayerDrawSet drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright)
        {
            foreach (AccessoryEffect effect in ActiveEffects)
            {
                effect.DrawEffects(Player, drawInfo, ref r, ref g, ref b, ref a, ref fullBright);
            }
        }
        #endregion
    }
}
