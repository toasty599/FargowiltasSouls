using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Core.AccessoryEffect
{
    public class AccessoryEffectPlayer : ModPlayer
    {
        public List<AccessoryEffect> ActiveEffects = new();
        public List<AccessoryEffectInstance> EffectInstances = new();
        public Dictionary<AccessoryEffect, bool> EffectToggle; // TODO: rework to be implemented properly with toggle-side of rework
        public override void SetStaticDefaults()
        {
            EffectInstances = AccessoryEffectLoader.AccessoryEffectInstances;
        }
        
        public override void ResetEffects()
        {
            foreach (AccessoryEffectInstance effectInstance in EffectInstances)
            {
                effectInstance.ResetEffects();
            }
        }

        public override void PostUpdateEquips()
        {
            foreach (AccessoryEffect effect in ActiveEffects)
            {
                effect.PostUpdateEquips();
            }
        }
        // etc etc
    }
}
