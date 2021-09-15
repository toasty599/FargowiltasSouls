using FargowiltasSouls.EternityMode.Net;
using FargowiltasSouls.EternityMode.NPCMatching;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.EternityMode
{
    public abstract class EModeNPCBehaviour
    {
        public static List<EModeNPCBehaviour> AllEModeNpcBehaviours = new List<EModeNPCBehaviour>();

        public NPCMatcher Matcher;

        public bool FirstTick;

        public void Register()
        {
            AllEModeNpcBehaviours.Add(this);
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
            Matcher = CreateMatcher();
            Register();
        }

        public virtual Dictionary<Ref<object>, CompoundStrategy> GetNetInfo() => default;

        public abstract NPCMatcher CreateMatcher();
        /// <summary>
        /// Override this and return a new instance of your EModeNPCBehaviour subclass if you have any reference-type variables
        /// </summary>
        public virtual EModeNPCBehaviour NewInstance() => (EModeNPCBehaviour)MemberwiseClone();

        public virtual void SetDefaults(NPC npc) { }

        public virtual bool PreAI(NPC npc)
        {
            if (FirstTick)
            {
                TransformWhenSpawned(npc);

                FirstTick = false;
            }
            return true;
        }

        public virtual void TransformWhenSpawned(NPC npc) { }

        public virtual void AI(NPC npc) { }

        public virtual void NPCLoot(NPC npc) { }

        public virtual void OnHitPlayer(NPC npc, Player target, int damage, bool crit) { }

        public virtual bool StrikeNPC(NPC npc, ref double damage, int defense, ref float knockback, int hitDirection, ref bool crit) => true;

        public virtual bool CheckDead(NPC npc) => true;

        protected static void NetSync(NPC npc, bool onlySendFromServer = true)
        {
            if (onlySendFromServer && Main.netMode != NetmodeID.Server)
                return;

            npc.GetGlobalNPC<NewEModeGlobalNPC>().NetSync((byte)npc.whoAmI);
        }

        public virtual void LoadSprites(NPC npc, bool recolor) { }

        #region Sprite Loading
        protected static Texture2D LoadSprite(bool recolor, string texture)
        {
            return ModContent.GetTexture("FargowiltasSouls/NPCs/" + (recolor ? "Resprites/" : "Vanilla/") + texture);
        }

        protected static void LoadNPCSprite(bool recolor, int type)
        {
            Main.npcTexture[type] = LoadSprite(recolor, "NPC_" + type.ToString());
            Main.NPCLoaded[type] = true;
        }

        protected static void LoadBossHeadSprite(bool recolor, int type)
        {
            Main.npcHeadBossTexture[type] = LoadSprite(recolor, "NPC_Head_Boss_" + type.ToString());
        }

        protected static void LoadGore(bool recolor, int type)
        {
            Main.goreTexture[type] = LoadSprite(recolor, "Gores/Gore_" + type.ToString());
            Main.goreLoaded[type] = true;
        }

        protected static void LoadGoreRange(bool recolor, int type, int lastType)
        {
            for (int i = type; i <= lastType; i++)
                LoadGore(recolor, i);
        }

        protected static void LoadExtra(bool recolor, int type)
        {
            Main.extraTexture[type] = LoadSprite(recolor, "Extra_" + type.ToString());
        }
        #endregion
    }
}
