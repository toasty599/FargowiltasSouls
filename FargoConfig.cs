using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;

namespace FargowiltasSouls
{
    class SoulConfig : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ClientSide;
        public static SoulConfig Instance => ModContent.GetInstance<SoulConfig>();

        private void SetAll(bool val)
        {
            //bool backgroundValue = MutantBackground;
            bool recolorsValue = BossRecolors;

            IEnumerable<FieldInfo> configs = typeof(SoulConfig).GetFields(BindingFlags.Public | BindingFlags.Instance).Where(
                i => i.FieldType == true.GetType() && !i.Name.Contains("Patreon"));
            foreach (FieldInfo config in configs)
            {
                config.SetValue(this, val);
            }

            //MutantBackground = backgroundValue;
            BossRecolors = recolorsValue;

            /*IEnumerable<FieldInfo> walletConfigs = typeof(WalletToggles).GetFields(BindingFlags.Public | BindingFlags.Instance).Where(i => i.FieldType == true.GetType());
            foreach (FieldInfo walletConfig in walletConfigs)
            {
                walletConfig.SetValue(walletToggles, val);
            }*/

            /*IEnumerable<FieldInfo> thoriumConfigs = typeof(ThoriumToggles).GetFields(BindingFlags.Public | BindingFlags.Instance).Where(i => i.FieldType == true.GetType());
            foreach (FieldInfo thoriumConfig in thoriumConfigs)
            {
                thoriumConfig.SetValue(thoriumToggles, val);
            }

            IEnumerable<FieldInfo> calamityConfigs = typeof(CalamityToggles).GetFields(BindingFlags.Public | BindingFlags.Instance).Where(i => i.FieldType == true.GetType());
            foreach (FieldInfo calamityConfig in calamityConfigs)
            {
                calamityConfig.SetValue(calamityToggles, val);
            }*/
        }

        [Header("$Mods.FargowiltasSouls.PresetHeader")]
        [Label("All Toggles On")]
        public bool PresetA
        {
            get => false;
            set
            {
                if (value)
                {
                    SetAll(true);
                }
            }
        }
        [Label("All Toggles Off")]
        public bool PresetB
        {
            get => false;
            set
            {
                if (value)
                {
                    SetAll(false);
                }
            }
        }
        /*[Label("Minimal Effects Only")]
        public bool PresetC
        {
            get => false;
            set
            {
                if (value)
                {
                    SetAll(false);

                    //MythrilSpeed = true;
                    //PalladiumHeal = true;
                    //IronMagnet = true;
                    //CthulhuShield = true;
                    //TinCrit = true;
                    //BeetleEffect = true;
                    //SpiderCrits = true;
                    //ShinobiTabi = true;
                    //NebulaBoost = true;
                    //SolarShield = true;
                    //Graze = true;
                    //SinisterIconDrops = true;
                    //NymphPerfume = true;
                    //TribalCharm = true;
                    //StabilizedGravity = true;
                }
            }
        }*/

        [Label("Only show effect toggler when inventory is open")]
        [Description("If true, the effect toggler is automatically hidden when your inventory is closed.")]
        [DefaultValue(false)]
        public bool HideTogglerWhenInventoryIsClosed;

        [Label("Mutant boss music effect")]
        [DefaultValue(true)]
        public bool MutantMusicIsRePrologue;

        #region maso accessories

        [Header("$Mods.FargowiltasSouls.MasoHeader")]
        /*[Label("$Mods.FargowiltasSouls.MasoBossBG")]
        [DefaultValue(true)]
        public bool MutantBackground;*/

        [Label("$Mods.FargowiltasSouls.MasoBossRecolors")]
        [DefaultValue(true)]
        public bool BossRecolors;

        /*[Label("$Mods.FargowiltasSouls.WalletHeader")]
        public WalletToggles walletToggles = new WalletToggles();*/
        #endregion

        [Header("$Mods.FargowiltasSouls.PatreonHeader")]
        [Label("$Mods.FargowiltasSouls.PatreonRoomba")]
        [DefaultValue(true)]
        public bool PatreonRoomba;

        [Label("$Mods.FargowiltasSouls.PatreonOrb")]
        [DefaultValue(true)]
        public bool PatreonOrb;

        [Label("$Mods.FargowiltasSouls.PatreonFishingRod")]
        [DefaultValue(true)]
        public bool PatreonFishingRod;

        [Label("$Mods.FargowiltasSouls.PatreonDoor")]
        [DefaultValue(true)]
        public bool PatreonDoor;

        [Label("$Mods.FargowiltasSouls.PatreonWolf")]
        [DefaultValue(true)]
        public bool PatreonWolf;

        [Label("$Mods.FargowiltasSouls.PatreonDove")]
        [DefaultValue(true)]
        public bool PatreonDove;

        [Label("$Mods.FargowiltasSouls.PatreonKingSlime")]
        [DefaultValue(true)]
        public bool PatreonKingSlime;

        [Label("$Mods.FargowiltasSouls.PatreonFishron")]
        [DefaultValue(true)]
        public bool PatreonFishron;

        [Label("$Mods.FargowiltasSouls.PatreonPlant")]
        [DefaultValue(true)]
        public bool PatreonPlant;

        [Label("$Mods.FargowiltasSouls.PatreonDevious")]
        [DefaultValue(true)]
        public bool PatreonDevious;

        [Label("$Mods.FargowiltasSouls.PatreonVortex")]
        [DefaultValue(true)]
        public bool PatreonVortex;

        [Label("$Mods.FargowiltasSouls.PatreonPrime")]
        [DefaultValue(true)]
        public bool PatreonPrime;

        [Label("$Mods.FargowiltasSouls.PatreonCrimetroid")]
        [DefaultValue(true)]
        public bool PatreonCrimetroid;



        /*[Label("$Mods.FargowiltasSouls.ThoriumHeader")]
        public ThoriumToggles thoriumToggles = new ThoriumToggles();

        [Label("$Mods.FargowiltasSouls.CalamityHeader")]
        public CalamityToggles calamityToggles = new CalamityToggles();*/


        //soa soon tm

        // Proper cloning of reference types is required because behind the scenes many instances of ModConfig classes co-exist.
        /*public override ModConfig Clone()
        {
            var clone = (SoulConfig)base.Clone();

            clone.walletToggles = walletToggles == null ? null : new WalletToggles();
            clone.thoriumToggles = thoriumToggles == null ? null : new ThoriumToggles();
            clone.calamityToggles = calamityToggles == null ? null : new CalamityToggles();

            return clone;
        }*/

        public bool GetValue(bool toggle, bool checkForMutantPresence = true)
        {
            return checkForMutantPresence && Main.player[Main.myPlayer].GetModPlayer<FargoSoulsPlayer>().MutantPresence ? false : toggle;
        }
    }

    /*public class WalletToggles
    {
        [Label("Warding")]
        [DefaultValue(true)]
        public bool Warding;

        [Label("Violent")]
        [DefaultValue(true)]
        public bool Violent;

        [Label("Quick")]
        [DefaultValue(true)]
        public bool Quick;

        [Label("Lucky")]
        [DefaultValue(true)]
        public bool Lucky;

        [Label("Menacing")]
        [DefaultValue(true)]
        public bool Menacing;

        [Label("Legendary")]
        [DefaultValue(true)]
        public bool Legendary;

        [Label("Unreal")]
        [DefaultValue(true)]
        public bool Unreal;

        [Label("Mythical")]
        [DefaultValue(true)]
        public bool Mythical;

        [Label("Godly")]
        [DefaultValue(true)]
        public bool Godly;

        [Label("Demonic")]
        [DefaultValue(true)]
        public bool Demonic;

        [Label("Ruthless")]
        [DefaultValue(true)]
        public bool Ruthless;

        [Label("Light")]
        [DefaultValue(true)]
        public bool Light;

        [Label("Deadly")]
        [DefaultValue(true)]
        public bool Deadly;

        [Label("Rapid")]
        [DefaultValue(true)]
        public bool Rapid;
    }*/


}
