//this currently does not work: it still does the vanilla dash, i do not know how, i do not know how to fix it.


using FargowiltasSouls.Content.Buffs.Souls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Terraria;
using System.Reflection;
using FargowiltasSouls.Content.Items.Accessories.Enchantments;

namespace FargowiltasSouls.Core.Systems
{
    public class DashManager : ModSystem
    {
        public enum DashType
        {
            None,
            Monk,
            Shinobi,
            Jungle,
            DeerSinew
        }
        public static void ManageDashes(Player Player)
        {
            if (Player.whoAmI != Main.myPlayer)
                return;
            FargoSoulsPlayer modPlayer = Player.FargoSouls();

            if (modPlayer.FargoDash == DashType.None)
                return;

            Player.dashType = 22;


            if (Player.dashDelay == 0 && !Player.mount.Active)
            {
                HandleDash(out bool dashing, out int dir);
                if (dashing && dir != 0)
                {
                    switch (modPlayer.FargoDash)
                    {
                        case DashType.Monk:
                            {
                                int monkBuff = ModContent.BuffType<MonkBuff>();
                                if (Player.HasBuff(monkBuff))
                                {
                                    MonkBuff.MonkDash(Player, false, dir);
                                    Player.ClearBuff(monkBuff);
                                }
                            }
                            break;
                        case DashType.Shinobi:
                            {
                                ShinobiEnchant.ShinobiDash(Player, dir);
                            }
                            break;
                        case DashType.Jungle:
                            {
                                JungleEnchant.JungleDash(Player, dir);
                            }
                            break;
                        case DashType.DeerSinew:
                            {
                                modPlayer.DeerSinewDash(dir);
                            }
                            break;
                        default:
                            {
                                Main.NewText("Fargo dash manager: dash not registered");
                            }
                            break;
                    }
                }
            }
        }

        public static MethodInfo DashHandleMethod { get; set; }
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
            //action = (Player.DashStartAction)args[2];
        }

    }
}
