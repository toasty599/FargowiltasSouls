using FargowiltasSouls.Buffs.Masomode;
using FargowiltasSouls.EternityMode.Net;
using FargowiltasSouls.EternityMode.Net.Strategies;
using FargowiltasSouls.EternityMode.NPCMatching;
using FargowiltasSouls.NPCs;
using FargowiltasSouls.Projectiles;
using FargowiltasSouls.Projectiles.Masomode;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.EternityMode.Content.Enemy
{
    public class EarlyBirdEnemies : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchTypeRange(
            NPCID.WyvernHead,
            NPCID.WyvernBody,
            NPCID.WyvernBody2,
            NPCID.WyvernBody3,
            NPCID.WyvernLegs,
            NPCID.WyvernTail,
            NPCID.Mimic,
            NPCID.IceMimic,
            NPCID.Medusa,
            NPCID.PigronCorruption,
            NPCID.PigronCrimson,
            NPCID.PigronHallow,
            NPCID.IchorSticker,
            NPCID.SeekerHead,
            NPCID.AngryNimbus,
            NPCID.RedDevil,
            NPCID.MushiLadybug,
            NPCID.AnomuraFungus,
            NPCID.ZombieMushroom,
            NPCID.ZombieMushroomHat
        );

        public override void SetDefaults(NPC npc)
        {
            base.SetDefaults(npc);

            if (!Main.hardMode)
                npc.defense /= 2;
        }

        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            base.ModifyNPCLoot(npc, npcLoot);

            switch (npc.type)
            {
                case NPCID.Medusa:
                    npcLoot.RemoveWhere(rule => rule is CommonDrop drop && drop.itemId == ItemID.MedusaHead && EModeUtils.LockEarlyBirdDrop(npcLoot, rule));
                    break;

                case NPCID.WyvernHead:
                    npcLoot.RemoveWhere(rule => rule is DropBasedOnExpertMode drop && drop.ruleForNormalMode is CommonDrop drop2 && drop2.itemId == ItemID.SoulofFlight && EModeUtils.LockEarlyBirdDrop(npcLoot, rule));
                    break;

                case NPCID.RedDevil:
                    npcLoot.RemoveWhere(rule => rule is CommonDrop drop && drop.itemId == ItemID.UnholyTrident && EModeUtils.LockEarlyBirdDrop(npcLoot, rule));
                    EModeUtils.AddEarlyBirdDrop(npcLoot, ItemDropRule.Common(ItemID.DemonScythe, 3));
                    break;

                case NPCID.IchorSticker:
                    npcLoot.RemoveWhere(rule => rule is CommonDrop drop && drop.itemId == ItemID.Ichor && EModeUtils.LockEarlyBirdDrop(npcLoot, rule));
                    EModeUtils.AddEarlyBirdDrop(npcLoot, ItemDropRule.OneFromOptions(1, ItemID.TheUndertaker, ItemID.TheRottedFork, ItemID.CrimsonRod, ItemID.CrimsonHeart, ItemID.PanicNecklace));
                    break;

                case NPCID.SeekerHead:
                    npcLoot.RemoveWhere(rule => rule is CommonDrop drop && drop.itemId == ItemID.CursedFlame && EModeUtils.LockEarlyBirdDrop(npcLoot, rule));
                    EModeUtils.AddEarlyBirdDrop(npcLoot, ItemDropRule.OneFromOptions(1, ItemID.BallOHurt, ItemID.BandofStarpower, ItemID.Musket, ItemID.ShadowOrb, ItemID.Vilethorn));
                    break;

                case NPCID.Mimic:
                    npcLoot.RemoveWhere(rule => rule is OneFromOptionsDropRule drop && drop.dropIds.Contains(ItemID.DualHook) && EModeUtils.LockEarlyBirdDrop(npcLoot, rule));
                    EModeUtils.AddEarlyBirdDrop(npcLoot, ItemDropRule.OneFromOptions(1, ItemID.TitanGlove, ItemID.PhilosophersStone, ItemID.CrossNecklace, ItemID.DualHook));
                    break;

                case NPCID.IceMimic:
                    npcLoot.RemoveWhere(rule => rule is CommonDrop drop && drop.itemId == ItemID.ToySled && EModeUtils.LockEarlyBirdDrop(npcLoot, rule));
                    EModeUtils.AddEarlyBirdDrop(npcLoot, ItemDropRule.OneFromOptions(1, ItemID.TitanGlove, ItemID.PhilosophersStone, ItemID.CrossNecklace, ItemID.DualHook));
                    break;

                case NPCID.AngryNimbus:
                    npcLoot.RemoveWhere(rule => rule is CommonDrop drop && drop.itemId == ItemID.NimbusRod && EModeUtils.LockEarlyBirdDrop(npcLoot, rule));
                    EModeUtils.AddEarlyBirdDrop(npcLoot, ItemDropRule.Common(ItemID.FloatingIslandFishingCrate));
                    break;

                case NPCID.DuneSplicerHead:
                    EModeUtils.AddEarlyBirdDrop(npcLoot, ItemDropRule.Common(ItemID.SandstorminaBottle, 3));
                    EModeUtils.AddEarlyBirdDrop(npcLoot, ItemDropRule.Common(ItemID.OasisCrate));
                    break;

                /*case NPCID.PigronCorruption:
                case NPCID.PigronCrimson:
                case NPCID.PigronHallow:
                    if (!Main.hardMode && !npc.SpawnedFromStatue)
                    {
                        Item.NewItem(npc.Hitbox, ItemID.GoldCoin, 1 + Main.rand.Next(5));
                        Item.NewItem(npc.Hitbox, ItemID.Bacon, 1 + Main.rand.Next(15));
                        return false;
                    }
                    break;*/

                default: break;
            }
        }
    }
}
