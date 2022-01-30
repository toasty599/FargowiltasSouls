using Terraria;
using Terraria.ModLoader;
using Terraria.Localization;

namespace FargowiltasSouls.Buffs.Masomode
{
    public class Stunned : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Stunned");
            Description.SetDefault("You're too dizzy to move");
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "昏迷");
            Description.AddTranslation((int)GameCulture.CultureName.Chinese, "你头晕目眩,动弹不得");
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.controlLeft = false;
            player.controlRight = false;
            player.controlJump = false;
            player.controlDown = false;
            player.controlUseItem = false;
            player.controlHook = false;
            player.releaseHook = true;
            if (player.mount.Active)
                player.mount.Dismount(player);
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            if (!npc.boss)
            {
                npc.velocity.X *= 0;
                npc.velocity.Y *= 0;
                npc.frameCounter = 0;
            }
        }

        public override bool ReApply(Player player, int time, int buffIndex)
        {
            return time > 2;
        }
    }
}