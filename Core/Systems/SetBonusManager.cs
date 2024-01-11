using Fargowiltas.Common.Configs;
using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using FargowiltasSouls.Content.Items.Armor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Core.Systems
{
    public class SetBonusManager : ModSystem
    {
        public override void Load()
        {
            On_Player.KeyDoubleTap += new On_Player.hook_KeyDoubleTap(SetBonusKeyEffects);
        }
        public override void Unload()
        {
            On_Player.KeyDoubleTap -= new On_Player.hook_KeyDoubleTap(SetBonusKeyEffects);
        }
        public void SetBonusKeyEffects(On_Player.orig_KeyDoubleTap orig, Player player, int keyDir)
        {
            orig.Invoke(player, keyDir);
            if (keyDir == (Main.ReversedUpDownArmorSetBonuses ? 1 : 0))
            {
                GladiatorEnchant.ActivateGladiatorBanner(player);
                PalmwoodEffect.ActivatePalmwoodSentry(player);
                EridanusHat.EridanusSetBonusKey(player);
                GaiaHelmet.GaiaSetBonusKey(player);
                NekomiHood.NekomiSetBonusKey(player);
                StyxCrown.StyxSetBonusKey(player);
                ForbiddenEnchant.ActivateForbiddenStorm(player);
                VortexEnchant.ActivateVortex(player);
            }

        }
    }
}
