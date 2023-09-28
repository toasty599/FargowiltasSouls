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

namespace FargowiltasSouls.Core.Systems
{
    public class DashPlayer : ModPlayer
    {
        public int latestXDirPressed = 0;
        public int latestXDirReleased = 0;
        private bool LeftLastPressed = false;
        private bool RightLastPressed = false;
        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            if (Main.LocalPlayer.controlLeft && !LeftLastPressed)
            {
                latestXDirPressed = -1;
            }
            if (Main.LocalPlayer.controlRight && !RightLastPressed)
            {
                latestXDirPressed = 1;
            }
            if (!Main.LocalPlayer.controlLeft && !Main.LocalPlayer.releaseLeft)
            {
                latestXDirReleased = -1;
            }
            if (!Main.LocalPlayer.controlRight && !Main.LocalPlayer.releaseRight)
            {
                latestXDirReleased = 1;
            }
            LeftLastPressed = Main.LocalPlayer.controlLeft;
            RightLastPressed = Main.LocalPlayer.controlRight;
        }
    }
    public class CustomDashManager : ModSystem
    {
        public static void DashHandleThatMimicsVanillaPlusMutantModKeyBecauseTheRealOneIsFuckingPrivate(Player player, out int dir, out bool dashing)
        {
            dir = 0;
            dashing = false;
            Mod fargo = ModLoader.GetMod("Fargowiltas");
            if (player.whoAmI == Main.myPlayer && !(bool)fargo.Call("DoubleTapDashDisabled"))//!ModContent.GetInstance<FargoClientConfig>().DoubleTapDashDisabled)
            {
                //vanilla dash check here
                if (player.dashTime > 0)
                {
                    player.dashTime--;
                }
                if (player.dashTime < 0)
                {
                    player.dashTime++;
                }
                if (player.controlRight && player.releaseRight)
                {
                    if (player.dashTime > 0)
                    {
                        dir = 1;
                        dashing = true;
                        player.dashTime = 0;
                        player.timeSinceLastDashStarted = 0;
                    }
                    else
                    {
                        player.dashTime = 15;
                    }
                }
                else if (player.controlLeft && player.releaseLeft)
                {
                    if (player.dashTime < 0)
                    {
                        dir = -1;
                        dashing = true;
                        player.dashTime = 0;
                        player.timeSinceLastDashStarted = 0;
                    }
                    else
                    {
                        player.dashTime = -15;
                    }
                }
                return;
            }
            if ((bool)fargo.Call("DashKeyJustPressed")) //(fargo.DashKey.JustPressed)
            {
                DashPlayer modPlayer = player.GetModPlayer<DashPlayer>();
                if (player.controlRight && player.controlLeft)
                {
                    dir = modPlayer.latestXDirPressed;
                }
                else if (player.controlRight)
                {
                    dir = 1;
                }
                else if (player.controlLeft)
                {
                    dir = -1;
                }
                else if (modPlayer.latestXDirReleased != 0)
                {
                    dir = modPlayer.latestXDirReleased;
                }
                else
                {
                    dir = player.direction;
                }
                player.direction = dir;
                dashing = true;
                if (player.dashTime > 0)
                {
                    player.dashTime--;
                }
                if (player.dashTime < 0)
                {
                    player.dashTime++;
                }
                if ((player.dashTime <= 0 && player.direction == -1) || (player.dashTime >= 0 && player.direction == 1))
                {
                    player.dashTime = 15;
                    return;
                }
                dashing = true;
                player.dashTime = 0;
                player.timeSinceLastDashStarted = 0;
            }
        }
        /*
        public override void PostSetupContent()
        {
            Terraria.On_Player.DoCommonDashHandle += CustomDashes;
        }
        public override void Unload()
        {
            Terraria.On_Player.DoCommonDashHandle -= CustomDashes;
        }
        private static void CustomDashes(Terraria.On_Player.orig_DoCommonDashHandle orig, Terraria.Player player, out int dir, out bool dashing, Player.DashStartAction dashStartAction)
        {
            
            FargoSoulsPlayer modPlayer = player.FargoSouls();
            //add these in order of priority
            if (dashing && modPlayer.HasDash)
            {
                if (modPlayer.MonkDashReady && player.HasBuff<MonkBuff>())
                {
                    player.ClearBuff(ModContent.BuffType<MonkBuff>());
                    MonkBuff.MonkDash(player, false, dir);
                    modPlayer.MonkDashReady = false; //making sure
                                                     //ALWAYS DO THESE THREE:
                    player.dashType = 0;
                    player.dash = 0;
                    dashing = false; //don't do vanilla effects
                    dir = 0;
                }
                if (modPlayer.JungleDashReady && modPlayer.JungleEnchantActive)
                {
                    float dashSpeed = modPlayer.ChloroEnchantActive ? 12f : 9f;
                    modPlayer.dashCD = 60;
                    if (modPlayer.IsDashingTimer < 10)
                        modPlayer.IsDashingTimer = 10;
                    player.velocity.X = dashSpeed;
                    modPlayer.JungleDashReady = false; //making sure
                                                       //ALWAYS DO THESE THREE:
                    player.dashType = 0;
                    player.dash = 0;
                    dashing = false; //don't do vanilla effects
                    dir = 0;
                }
            }
            else
            {

            }
        }
        */
    }
}
