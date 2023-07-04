﻿using FargowiltasSouls.Content.Bosses.Champions.Shadow;
using FargowiltasSouls.Content.Bosses.TrojanSquirrel;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Linq;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI.BigProgressBar;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.BossBars
{
    public class CompositeBossBar : ModBossBar
    {
        private int bossHeadIndex = -1;

        public override Asset<Texture2D> GetIconTexture(ref Rectangle? iconFrame)
        {
            if (bossHeadIndex != -1)
                return TextureAssets.NpcHeadBoss[bossHeadIndex];

            return base.GetIconTexture(ref iconFrame);
        }

        public override bool? ModifyInfo(ref BigProgressBarInfo info, ref float life, ref float lifeMax, ref float shield, ref float shieldMax)/* tModPorter Note: life and shield current and max values are now separate to allow for hp/shield number text draw */
        {
            NPC npc = FargoSoulsUtil.NPCExists(info.npcIndexToAimAt);

            if (npc == null || !npc.active)
                return false;

            bossHeadIndex = npc.GetBossHeadTextureIndex();

            bool retval = true;

            if (npc.ModNPC is TrojanSquirrel trojanSquirrel)
            {
                if (trojanSquirrel.head != null)
                    life += trojanSquirrel.head.life;
                if (trojanSquirrel.arms != null)
                    life += trojanSquirrel.arms.life;

                lifeMax += trojanSquirrel.lifeMaxHead;
                lifeMax += trojanSquirrel.lifeMaxArms;
            }
            else if (npc.type == ModContent.NPCType<ShadowChampion>())
            {
                if (npc.ai[0] == -1)
                {
                    shield = 1;
                    shieldMax = 1;
                    // shieldPercent = 1;
                }
                else
                {
                    foreach (NPC n in Main.npc.Where(n => n.active && n.type == ModContent.NPCType<ShadowOrbNPC>() && n.ai[0] == npc.whoAmI))
                    {
                        shieldMax++;
                        if (!n.dontTakeDamage)
                            shield++;
                    }
                }
            }
            else
            {
                retval = false;
            }

            return retval;
        }
    }
}