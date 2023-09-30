//this currently does not work: it still does the vanilla dash, i do not know how, i do not know how to fix it.


using FargowiltasSouls.Content.Buffs.Souls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Terraria;
using Terraria.GameInput;
using System.Reflection;
using FargowiltasSouls.Content.Items.Accessories.Enchantments;

namespace FargowiltasSouls.Core.Systems
{
    public class DashPlayer : ModPlayer
    {
        public int modDashDelay = 0;
        public int modDashTime = 0;
        public override void PostUpdateEquips()
        {
            if (Player.whoAmI != Main.myPlayer)
            {
                return;
            }
            FargoSoulsPlayer modPlayer = Player.FargoSouls();

            if (modDashDelay == 0 && !Player.mount.Active)
            {
                if (modPlayer.MonkDashReady)
                {
                    CustomDashManager.HandleDash(out bool dashing, out int dir);
                    if (dashing && dir != 0)
                    {
                        MonkBuff.MonkDash(Player, false, dir);
                        Player.ClearBuff(ModContent.BuffType<MonkBuff>());
                    }
                }
                else if (modPlayer.ShinobiEnchantActive && Player.GetToggleValue("ShinobiDash"))
                {
                    CustomDashManager.HandleDash(out bool dashing, out int dir);
                    if (dashing && dir != 0)
                    {
                        ShinobiEnchant.ShinobiDash(Player, dir);
                    }
                }
                else if (modPlayer.JungleDashReady)
                {
                    CustomDashManager.HandleDash(out bool dashing, out int dir);
                    if (dashing && dir != 0)
                    {
                        JungleEnchant.JungleDash(Player, dir);
                    }
                }
                else if (modPlayer.DeerSinewNerf)
                {
                    CustomDashManager.HandleDash(out bool dashing, out int dir);
                    if (dashing && dir != 0)
                    {
                        modPlayer.DeerSinewDash(dir);
                    }
                }
            }
            else
            {
                modDashDelay -= Math.Sign(modDashDelay);
            }
            
        }

    }
    public class CustomDashManager : ModSystem
    {
        public static MethodInfo DashHandleMethod
        {
            get;
            set;
        }
        public override void Load()
        {
            DashHandleMethod = typeof(Player).GetMethod("DoCommonDashHandle", FargoSoulsUtil.UniversalBindingFlags);
        }
        public static void HandleDash(out bool dashing, out int dir)
        {

            dir = 1;
            dashing = true; //these two are overriden by the actual method anyway
            

            Player player = Main.LocalPlayer;
            Player.DashStartAction action = null;
            object[] args = new object[] { dir, dashing, action };
            DashHandleMethod.Invoke(player, args);
            dir = (int)args[0];
            dashing = (bool)args[1];

            bool dashDisabled = false;
            if (ModLoader.TryGetMod("Fargowiltas", out Mod fargos))
            {
                if ((bool)fargos.Call("DoubleTapDashDisabled"))
                {
                    dashDisabled = true;
                }
            }
            if (!dashDisabled)
            {
                HackyDoubletapHandle(player, out bool dashing1, out int dir1);
                if (dashing1)
                {
                    dashing = true;
                }
                if (dir1 != 0)
                {
                    dir = dir1;
                }
            }
            
            //action = (Player.DashStartAction)args[2];
        }
        private static void HackyDoubletapHandle(Player player, out bool dashing, out int dir)
        {
            dashing = false;
            dir = 0;
            DashPlayer dashPlayer = player.GetModPlayer<DashPlayer>();
            if (dashPlayer.modDashTime > 0)
            {
                dashPlayer.modDashTime--;
            }
            if (dashPlayer.modDashTime < 0)
            {
                dashPlayer.modDashTime++;
            }
            if (player.controlRight && player.releaseRight)
            {
                if (dashPlayer.modDashTime > 0)
                {
                    dir = 1;
                    dashing = true;
                    dashPlayer.modDashTime = 0;
                    player.timeSinceLastDashStarted = 0;
                    //dashStartAction?.Invoke(dir);
                }
                else
                {
                    dashPlayer.modDashTime = 15;
                }
            }
            else if (player.controlLeft && player.releaseLeft)
            {
                if (dashPlayer.modDashTime < 0)
                {
                    dir = -1;
                    dashing = true;
                    dashPlayer.modDashTime = 0;
                    player.timeSinceLastDashStarted = 0;
                    //dashStartAction?.Invoke(dir);
                }
                else
                {
                    dashPlayer.modDashTime = -15;
                }
            }
        }
    }
}
