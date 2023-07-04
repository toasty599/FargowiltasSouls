using System.IO;
using Terraria.ModLoader.IO;
using FargowiltasSouls.Content.Projectiles.Masomode;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using FargowiltasSouls.Content.Items.Accessories.Masomode;
using FargowiltasSouls.Content.Buffs.Masomode;
using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.NPCMatching;

namespace FargowiltasSouls.Content.NPCs.EternityModeNPCs.VanillaEnemies.SkyAndRain
{
    public class Wyvern : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.WyvernHead);

        public int AttackTimer;

        public override void SendExtraAI(NPC npc, BitWriter bitWriter, BinaryWriter binaryWriter)
        {
            base.SendExtraAI(npc, bitWriter, binaryWriter);

            binaryWriter.Write7BitEncodedInt(AttackTimer);
        }

        public override void ReceiveExtraAI(NPC npc, BitReader bitReader, BinaryReader binaryReader)
        {
            base.ReceiveExtraAI(npc, bitReader, binaryReader);

            AttackTimer = binaryReader.Read7BitEncodedInt();
        }

        public override void SetDefaults(NPC npc)
        {
            base.SetDefaults(npc);

            AttackTimer = Main.rand.Next(180);
        }

        public override void OnFirstTick(NPC npc)
        {
            base.OnFirstTick(npc);

            if (Main.hardMode && Main.rand.NextBool(10))
                EModeGlobalNPC.Horde(npc, 2);
        }

        public override void AI(NPC npc)
        {
            base.AI(npc);

            if (++AttackTimer > 240)
            {
                AttackTimer = 0;
                if (Main.netMode != NetmodeID.MultiplayerClient && npc.velocity != Vector2.Zero)
                {
                    const int max = 12;
                    Vector2 vel = Vector2.Normalize(npc.velocity) * 1.5f;
                    for (int i = 0; i < max; i++)
                    {
                        Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, vel.RotatedBy(2f * MathHelper.Pi / max * i),
                            ModContent.ProjectileType<LightBall>(), FargoSoulsUtil.ScaledProjectileDamage(npc.damage, 0.8f), 0f, Main.myPlayer, 0f, .01f * npc.direction);
                    }
                }
            }
        }

        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            base.ModifyNPCLoot(npc, npcLoot);

            FargoSoulsUtil.EModeDrop(npcLoot, ItemDropRule.Common(ItemID.CloudinaBottle, 20));
            FargoSoulsUtil.EModeDrop(npcLoot, ItemDropRule.Common(ItemID.FloatingIslandFishingCrate));
            FargoSoulsUtil.EModeDrop(npcLoot, ItemDropRule.ByCondition(new Conditions.IsHardmode(), ItemID.FloatingIslandFishingCrateHard));
            FargoSoulsUtil.EModeDrop(npcLoot, ItemDropRule.ByCondition(new Conditions.IsHardmode(), ModContent.ItemType<WyvernFeather>(), 5));

            FargoSoulsUtil.AddEarlyBirdDrop(npcLoot, ItemDropRule.Common(ModContent.ItemType<WyvernFeather>()));
        }
    }

    public class WyvernSegment : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchTypeRange(
            NPCID.WyvernBody,
            NPCID.WyvernBody2,
            NPCID.WyvernBody3,
            NPCID.WyvernHead,
            NPCID.WyvernLegs,
            NPCID.WyvernTail
        );

        public override void SetDefaults(NPC npc)
        {
            base.SetDefaults(npc);

            if (Main.hardMode)
                npc.lifeMax = (int)System.Math.Round(npc.lifeMax * 1.5, System.MidpointRounding.ToEven);
        }

        public override void OnHitPlayer(NPC npc, Player target, Player.HurtInfo hurtInfo)
        {
            base.OnHitPlayer(npc, target, hurtInfo);

            target.AddBuff(ModContent.BuffType<CrippledBuff>(), 240);
            target.AddBuff(ModContent.BuffType<ClippedWingsBuff>(), 240);
        }
    }
}