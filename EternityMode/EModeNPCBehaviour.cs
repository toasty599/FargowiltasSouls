using FargowiltasSouls.EternityMode.Net;
using FargowiltasSouls.EternityMode.NPCMatching;
using Microsoft.Xna.Framework;
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
        internal static Mod mod => Fargowiltas.Instance;

        public static List<EModeNPCBehaviour> AllEModeNpcBehaviours = new List<EModeNPCBehaviour>();

        public NPCMatcher Matcher;

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

        public virtual bool PreAI(NPC npc) => true;

        public virtual void OnSpawn(NPC npc) { }

        public virtual void AI(NPC npc) { }

        public virtual bool PreNPCLoot(NPC npc) => true;

        public virtual void NPCLoot(NPC npc) { }

        public virtual bool CanHitPlayer(NPC npc, Player target, ref int cooldownSlot) => true;

        public virtual void OnHitPlayer(NPC npc, Player target, int damage, bool crit) { }

        public virtual void ModifyHitByAnything(NPC npc, Player player, ref int damage, ref float knockback, ref bool crit) { }

        public virtual void ModifyHitByItem(NPC npc, Player player, Item item, ref int damage, ref float knockback, ref bool crit) => ModifyHitByAnything(npc, player, ref damage, ref knockback, ref crit);

        public virtual void ModifyHitByProjectile(NPC npc, Projectile projectile, ref int damage, ref float knockback, ref bool crit, ref int hitDirection) => ModifyHitByAnything(npc, Main.player[projectile.owner], ref damage, ref knockback, ref crit);

        public virtual void OnHitByAnything(NPC npc, Player player, int damage, float knockback, bool crit) { }

        public virtual void OnHitByItem(NPC npc, Player player, Item item, int damage, float knockback, bool crit) => OnHitByAnything(npc, player, damage, knockback, crit);

        public virtual void OnHitByProjectile(NPC npc, Projectile projectile, int damage, float knockback, bool crit) => OnHitByAnything(npc, Main.player[projectile.owner], damage, knockback, crit);

        public virtual bool StrikeNPC(NPC npc, ref double damage, int defense, ref float knockback, int hitDirection, ref bool crit) => true;

        public virtual void HitEffect(NPC npc, int hitDirection, double damage) { }

        public virtual bool CheckDead(NPC npc) => true;

        public virtual Color? GetAlpha(NPC npc, Color drawColor) => null;

        public virtual bool? CanBeHitByItem(NPC npc, Player player, Item item) => null;

        public virtual bool? CanBeHitByProjectile(NPC npc, Projectile projectile) => null;

        protected static void NetSync(NPC npc, bool onlySendFromServer = true)
        {
            if (onlySendFromServer && Main.netMode != NetmodeID.Server)
                return;

            npc.GetGlobalNPC<NewEModeGlobalNPC>().NetSync(npc.whoAmI);
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
