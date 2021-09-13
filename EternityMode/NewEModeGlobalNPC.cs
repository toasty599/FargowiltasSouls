using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.EternityMode
{
    public class NewEModeGlobalNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;

        public List<EModeNPCMod> EModeNPCMods = new List<EModeNPCMod>();

        public override void SetDefaults(NPC npc)
        {
            base.SetDefaults(npc);

            if (!FargoSoulsWorld.MasochistMode)
                return;

            // Init EModeNPCMod list
            EModeNPCMods = EModeNPCMod.AllEModeNPCMods.Where(m => m.Matcher.Satisfies(npc.type)).ToList();
            // To make sure they're always in the same order
            // Possible future check - is ordering needed? Do they always have the same order?
            EModeNPCMods.OrderBy(m => m.GetType().FullName, StringComparer.InvariantCulture);

            // Call the mods setdefaults
            for (int i = 0; i < EModeNPCMods.Count; i++)
            {
                EModeNPCMods[i].SetDefaults(npc);
            }

            bool recolor = SoulConfig.Instance.BossRecolors && FargoSoulsWorld.MasochistMode;
            if (recolor || Fargowiltas.Instance.LoadedNewSprites)
            {
                Fargowiltas.Instance.LoadedNewSprites = true;
                for (int i = 0; i < EModeNPCMods.Count; i++)
                {
                    EModeNPCMods[i].LoadSprites(npc, recolor);
                }
            }
        }

        public override bool PreAI(NPC npc)
        {
            if (!FargoSoulsWorld.MasochistMode)
                return true;

            bool result = true;

            for (int i = 0; i < EModeNPCMods.Count; i++)
            {
                result &= EModeNPCMods[i].PreAI(npc);
            }

            return result;
        }

        public override void AI(NPC npc)
        {
            base.AI(npc);

            if (!FargoSoulsWorld.MasochistMode)
                return;

            for (int i = 0; i < EModeNPCMods.Count; i++)
            {
                EModeNPCMods[i].AI(npc);
            }
        }

        public override void NPCLoot(NPC npc)
        {
            base.NPCLoot(npc);

            if (!FargoSoulsWorld.MasochistMode)
                return;

            for (int i = 0; i < EModeNPCMods.Count; i++)
            {
                EModeNPCMods[i].NPCLoot(npc);
            }
        }

        public override void OnHitPlayer(NPC npc, Player target, int damage, bool crit)
        {
            if (!FargoSoulsWorld.MasochistMode)
                return;

            base.OnHitPlayer(npc, target, damage, crit);
        }

        public override bool StrikeNPC(NPC npc, ref double damage, int defense, ref float knockback, int hitDirection, ref bool crit)
        {
            bool result = base.StrikeNPC(npc, ref damage, defense, ref knockback, hitDirection, ref crit);

            if (FargoSoulsWorld.MasochistMode)
            {
                for (int i = 0; i < EModeNPCMods.Count; i++)
                {
                    result &= EModeNPCMods[i].StrikeNPC(npc, ref damage, defense, ref knockback, hitDirection, ref crit);
                }
            }

            return result;
        }

        public override bool CheckDead(NPC npc)
        {
            bool result = base.CheckDead(npc);

            if (FargoSoulsWorld.MasochistMode)
            {
                for (int i = 0; i < EModeNPCMods.Count; i++)
                {
                    result &= EModeNPCMods[i].CheckDead(npc);
                }
            }

            return result;
        }

        public void NetSync(byte whoAmI)
        {
            if (Main.netMode == NetmodeID.SinglePlayer)
                return;

            ModPacket packet = mod.GetPacket();
            packet.Write((byte)22); // New maso sync packet id
            packet.Write(whoAmI);

            for (int i = 0; i < EModeNPCMods.Count; i++)
            {
                EModeNPCMods[i].NetSend(packet);
            }

            packet.Send();
        }

        public void NetRecieve(BinaryReader reader)
        {
            for (int i = 0; i < EModeNPCMods.Count; i++)
            {
                EModeNPCMods[i].NetRecieve(reader);
            }
        }
    }

    public static class NewEModeGlobalNPCStatic
    {
        public static T GetEModeNPCMod<T>(this NPC npc) where T : EModeNPCMod => npc.GetGlobalNPC<NewEModeGlobalNPC>().EModeNPCMods.FirstOrDefault(m => m is T) as T;
    }
}
