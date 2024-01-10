using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using Terraria.Localization;

namespace FargowiltasSouls.Core.AccessoryEffect
{
    //Proof of concept file for an accessory effect rework

    /// <summary>
    /// Contains the behavior for an accessory effect. <para/>
    /// Each accessory effect needs a localized toggle description as Mods.YourMod.Toggler.YourAccessoryEffectName.<para/>
    /// This type is not instanced per player. Put instanced things (such as fields) in an AccessoryEffectInstance.
    /// </summary>
    public abstract class AccessoryEffect : ModType
    {
        public string ToggleDescription => Language.GetTextValue($"Mods.{Mod}.Toggler.{Name}");
        public abstract string ToggleCategory { get; }

        public bool MinionEffect = false;
        public bool ExtraAttackEffect = false;
        public virtual void PostUpdateEquips() { }

        // Add more methods as needed here
        protected sealed override void Register()
        {
            AccessoryEffectLoader.Register(this);
            ModTypeLookup<AccessoryEffect>.Register(this);
        }
    }
    /// <summary>
    /// Contains the parts of an accessory effect that should be instanced - for example fields.
    /// </summary>
    public abstract class AccessoryEffectInstance : ModType
    {
        /// <summary>
        /// Runs in ModPlayer ResetEffects (once per frame). See ModPlayer.ResetEffects for more info.
        /// </summary>
        public virtual void ResetEffects() { }
        protected sealed override void Register()
        {
            AccessoryEffectLoader.Register(this);
            ModTypeLookup<AccessoryEffectInstance>.Register(this);
        }
    }
}
