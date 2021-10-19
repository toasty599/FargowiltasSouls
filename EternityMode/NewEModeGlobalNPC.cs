using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.EternityMode
{
    public class NewEModeGlobalNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;

        public List<EModeNPCBehaviour> EModeNpcBehaviours = new List<EModeNPCBehaviour>();

        public override void SetDefaults(NPC npc)
        {
            base.SetDefaults(npc);

            if (!FargoSoulsWorld.MasochistMode)
                return;

            InitBehaviourList(npc);

            foreach (EModeNPCBehaviour behaviour in EModeNpcBehaviours)
            {
                behaviour.SetDefaults(npc);
            }

            bool recolor = SoulConfig.Instance.BossRecolors && FargoSoulsWorld.MasochistMode;
            if (recolor || Fargowiltas.Instance.LoadedNewSprites)
            {
                Fargowiltas.Instance.LoadedNewSprites = true;
                foreach (EModeNPCBehaviour behaviour in EModeNpcBehaviours)
                {
                    behaviour.LoadSprites(npc, recolor);
                }
            }
        }

        private void InitBehaviourList(NPC npc)
        {
            // TODO Try caching this again? Last attempt caused major fails
            IEnumerable<EModeNPCBehaviour> behaviours = EModeNPCBehaviour.AllEModeNpcBehaviours
                .Where(m => m.Matcher.Satisfies(npc.type));

            // To make sure they're always in the same order
            // TODO is ordering needed? Do they always have the same order?
            behaviours.OrderBy(m => m.GetType().FullName, StringComparer.InvariantCulture);

            EModeNpcBehaviours = behaviours.Select(m => m.NewInstance()).ToList();
        }

        #region Behaviour Hooks
        public override bool PreAI(NPC npc)
        {
            if (!FargoSoulsWorld.MasochistMode)
                return true;

            bool result = true;

            foreach (EModeNPCBehaviour behaviour in EModeNpcBehaviours)
            {
                result &= behaviour.PreAI(npc);
            }

            return result;
        }

        public override void AI(NPC npc)
        {
            base.AI(npc);

            if (!FargoSoulsWorld.MasochistMode)
                return;

            foreach (EModeNPCBehaviour behaviour in EModeNpcBehaviours)
            {
                behaviour.AI(npc);
            }
        }

        public override void NPCLoot(NPC npc)
        {
            base.NPCLoot(npc);

            if (!FargoSoulsWorld.MasochistMode)
                return;

            foreach (EModeNPCBehaviour behaviour in EModeNpcBehaviours)
            {
                behaviour.NPCLoot(npc);
            }
        }

        public override bool CanHitPlayer(NPC npc, Player target, ref int cooldownSlot)
        {
            bool result = true;

            if (FargoSoulsWorld.MasochistMode)
            {
                foreach (EModeNPCBehaviour behaviour in EModeNpcBehaviours)
                {
                    result &= behaviour.CanHitPlayer(npc, target, ref cooldownSlot);
                }
            }

            return result;
        }

        public override void OnHitPlayer(NPC npc, Player target, int damage, bool crit)
        {
            base.OnHitPlayer(npc, target, damage, crit);

            if (!FargoSoulsWorld.MasochistMode)
                return;

            foreach (EModeNPCBehaviour behaviour in EModeNpcBehaviours)
            {
                behaviour.OnHitPlayer(npc, target, damage, crit);
            }
        }

        public override void OnHitByItem(NPC npc, Player player, Item item, int damage, float knockback, bool crit)
        {
            base.OnHitByItem(npc, player, item, damage, knockback, crit);

            if (FargoSoulsWorld.MasochistMode)
            {
                foreach (EModeNPCBehaviour behaviour in EModeNpcBehaviours)
                {
                    behaviour.OnHitByItem(npc, player, item, damage, knockback, crit);
                }
            }
        }

        public override void OnHitByProjectile(NPC npc, Projectile projectile, int damage, float knockback, bool crit)
        {
            base.OnHitByProjectile(npc, projectile, damage, knockback, crit);

            if (FargoSoulsWorld.MasochistMode)
            {
                foreach (EModeNPCBehaviour behaviour in EModeNpcBehaviours)
                {
                    behaviour.OnHitByProjectile(npc, projectile, damage, knockback, crit);
                }
            }
        }

        public override bool StrikeNPC(NPC npc, ref double damage, int defense, ref float knockback, int hitDirection, ref bool crit)
        {
            bool result = base.StrikeNPC(npc, ref damage, defense, ref knockback, hitDirection, ref crit);

            if (FargoSoulsWorld.MasochistMode)
            {
                foreach (EModeNPCBehaviour behaviour in EModeNpcBehaviours)
                {
                    result &= behaviour.StrikeNPC(npc, ref damage, defense, ref knockback, hitDirection, ref crit);
                }
            }

            return result;
        }

        public override void HitEffect(NPC npc, int hitDirection, double damage)
        {
            base.HitEffect(npc, hitDirection, damage);

            if (FargoSoulsWorld.MasochistMode)
            {
                foreach (EModeNPCBehaviour behaviour in EModeNpcBehaviours)
                {
                    behaviour.HitEffect(npc, hitDirection, damage);
                }
            }
        }

        public override bool CheckDead(NPC npc)
        {
            bool result = base.CheckDead(npc);

            if (FargoSoulsWorld.MasochistMode)
            {
                foreach (EModeNPCBehaviour behaviour in EModeNpcBehaviours)
                {
                    result &= behaviour.CheckDead(npc);
                }
            }

            return result;
        }

        public override Color? GetAlpha(NPC npc, Color drawColor)
        {
            Color? result = base.GetAlpha(npc, drawColor);

            if (FargoSoulsWorld.MasochistMode)
            {
                foreach (EModeNPCBehaviour behaviour in EModeNpcBehaviours)
                {
                    result = behaviour.GetAlpha(npc, drawColor);
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

            foreach (EModeNPCBehaviour behaviour in EModeNpcBehaviours)
            {
                behaviour.NetSend(packet);
            }

            packet.Send();
        }

        public void NetRecieve(BinaryReader reader)
        {
            foreach (EModeNPCBehaviour behaviour in EModeNpcBehaviours)
            {
                behaviour.NetRecieve(reader);
            }
        }
        #endregion
    }
}
