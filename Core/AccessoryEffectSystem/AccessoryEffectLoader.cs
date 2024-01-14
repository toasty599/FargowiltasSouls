using FargowiltasSouls.Content.Items.Accessories.Expert;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace FargowiltasSouls.Core.AccessoryEffectSystem
{
    public static class AccessoryEffectLoader
    {
        public static HashSet<AccessoryEffect> AccessoryEffects = new();
        internal static void Register(AccessoryEffect effect)
        {
            AccessoryEffects.Add(effect);

            ToggleLoader.RegisterToggle(new Toggle(effect, effect.Mod.Name));

        }
        /// <summary>
        /// Adds the effect to the player. Lasts one frame. 
        /// Returns whether the effect was successfully added or not (it's not added if it's blocked by, for example, the toggle)
        /// </summary>
        public static bool AddEffect<T>(this Player player, Item item) where T : AccessoryEffect
        {
            AccessoryEffect effect = ModContent.GetInstance<T>();

            //if (effect.HasToggle)
                //player.DisplayToggle<T>();

            if (effect.MinionEffect || effect.ExtraAttackEffect)
            {
                FargoSoulsPlayer modPlayer = player.FargoSouls();
                if (modPlayer.PrimeSoulActive)
                {
                    if (!player.HasEffect(effect)) // Don't stack per item
                        modPlayer.PrimeSoulItemCount++;
                    return false;
                }
            }
            
            if (player.FargoSouls().MutantPresence)
                if (!effect.IgnoresMutantPresence)
                    return false;

            if (!effect.HasToggle || player.GetToggleValue(effect, true))
            {
                AccessoryEffectPlayer effectPlayer = player.AccessoryEffects();
                if (effectPlayer.ActiveEffectBuffer.Add(effect))
                {
                    effectPlayer.ActiveEffects.Add(effect);
                    effectPlayer.EffectItems[effect] = item;
                    return true;
                }
            }
            return false;
        }
        public static bool HasEffect<T>(this Player player) where T : AccessoryEffect => player.HasEffect(ModContent.GetInstance<AccessoryEffect>());
        public static bool HasEffect(this Player player, AccessoryEffect accessoryEffect) => player.AccessoryEffects().ActiveEffects.Contains(accessoryEffect);
        public static Item EffectItem<T>(this Player player) where T : AccessoryEffect => player.AccessoryEffects().EffectItems.TryGetValue(ModContent.GetInstance<T>(), out Item item) ? item : null;
        public static IEntitySource GetSource_EffectItem<T>(this Player player) where T : AccessoryEffect => ModContent.GetInstance<T>().GetSource_EffectItem(player);
        public static T EffectType<T>() where T : AccessoryEffect => ModContent.GetInstance<T>();
        public static AccessoryEffect EffectType(string internalName)  => ModContent.Find<AccessoryEffect>(internalName);
    }
}
