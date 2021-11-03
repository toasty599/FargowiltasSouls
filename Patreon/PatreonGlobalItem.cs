using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Items.Patreon
{
    public class PatreonGlobalItem : GlobalItem
    {
        public override bool CanUseItem(Item item, Player player)
        {
            if (item.damage > 0 && player.GetModPlayer<PatreonPlayer>().CompOrb && !item.magic && !item.summon && item.pick == 0 && item.hammer == 0 && item.axe == 0)
            {
                if (!player.CheckMana(10, true, false))
                    return false;

                player.GetModPlayer<PatreonPlayer>().CompOrbDrainCooldown = item.useTime + item.reuseDelay + 30;
            }

            return true;
        }
    }
}