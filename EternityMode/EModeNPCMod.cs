using FargowiltasSouls.EternityMode.Net;
using FargowiltasSouls.EternityMode.NPCMatching;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Terraria;

namespace FargowiltasSouls.EternityMode
{
    public abstract class EModeNPCMod
    {
        public static List<EModeNPCMod> AllEModeNPCMods = new List<EModeNPCMod>();

        public NPCMatcher Matcher;

        public void Register()
        {
            AllEModeNPCMods.Add(this);
        }

        public void NetSend(BinaryWriter writer)
        {
            Dictionary<Ref<object>, CompoundStrategy> netInfo = GetNetInfo();
            if (netInfo == default)
                return;

            for (int i = 0; i < netInfo.Count; i++)
            {
                KeyValuePair<Ref<object>, CompoundStrategy> query = netInfo.ElementAt(i);
                query.Value.Send(query.Key.Value, writer);
            }
        }

        public void NetRecieve(BinaryReader reader)
        {
            Dictionary<Ref<object>, CompoundStrategy> netInfo = GetNetInfo();
            if (netInfo == default)
                return;

            for (int i = 0; i < netInfo.Count; i++)
            {
                KeyValuePair<Ref<object>, CompoundStrategy> query = netInfo.ElementAt(i);
                query.Value.Recieve(ref query.Key.Value, reader);
            }
        }

        public virtual void Load()
        {

        }

        public virtual Dictionary<Ref<object>, CompoundStrategy> GetNetInfo() => default;

        public abstract void CreateMatcher();

        public virtual void SetDefaults(NPC npc) { }

        public virtual bool PreAI(NPC npc) => true;

        public virtual void AI(NPC npc) { }

        public virtual void NPCLoot(NPC npc) { }
    }
}
