using FargowiltasSouls.Buffs.Masomode;
using FargowiltasSouls.EternityMode.Net;
using FargowiltasSouls.EternityMode.Net.Strategies;
using FargowiltasSouls.EternityMode.NPCMatching;
using FargowiltasSouls.NPCs;
using FargowiltasSouls.Projectiles.Masomode;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.EternityMode.Content.Enemy.GoblinInvasion
{
    public class Goblins : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() =>  new NPCMatcher().MatchTypeRange(
            NPCID.GoblinArcher,
            NPCID.GoblinPeon,
            NPCID.GoblinScout,
            NPCID.GoblinSorcerer,
            NPCID.GoblinSummoner,
            NPCID.GoblinThief,
            NPCID.GoblinWarrior
        );

        public override void SetDefaults(NPC npc)
        {
            base.SetDefaults(npc);

            if (npc.type == NPCID.GoblinWarrior)
                npc.knockBackResist /= 10;
        }

        public override void OnSpawn(NPC npc)
        {
            base.OnSpawn(npc);

            if ((npc.type == NPCID.GoblinWarrior || npc.type == NPCID.GoblinThief || npc.type == NPCID.GoblinArcher)
                && !Main.hardMode && !NPC.downedSlimeKing && !NPC.downedBoss1 && NPC.CountNPCS(npc.type) > 5)
            {
                npc.Transform(NPCID.GoblinPeon);
            }
        }

        public override void AI(NPC npc)
        {
            base.AI(npc);

            if (npc.type != NPCID.GoblinSummoner)
            {
                if (npc.HasPlayerTarget && (!Main.player[npc.target].active || Main.player[npc.target].dead))
                {
                    npc.TargetClosest();
                    if (npc.HasPlayerTarget && (!Main.player[npc.target].active || Main.player[npc.target].dead))
                    {
                        npc.noTileCollide = true;
                    }
                }
                if (npc.noTileCollide) //fall through the floor
                {
                    npc.position.Y++;
                    npc.velocity.Y++;
                }
            }
        }

        public override void OnHitPlayer(NPC npc, Player target, int damage, bool crit)
        {
            base.OnHitPlayer(npc, target, damage, crit);

            if (Main.hardMode)
                target.AddBuff(ModContent.BuffType<Shadowflame>(), 300);

            if (npc.type == NPCID.GoblinThief)
            {
                target.AddBuff(ModContent.BuffType<Midas>(), 600);

                if (target.whoAmI == Main.myPlayer && !target.GetModPlayer<FargoSoulsPlayer>().SecurityWallet && Main.rand.NextBool())
                {
                    //try stealing mouse item, then selected item
                    bool stolen = EModeGlobalNPC.StealFromInventory(target, ref Main.mouseItem);
                    if (!stolen)
                        stolen = EModeGlobalNPC.StealFromInventory(target, ref target.inventory[target.selectedItem]);

                    if (stolen)
                    {
                        Main.NewText("An item was stolen from you!", new Color(255, 50, 50));
                        CombatText.NewText(target.Hitbox, new Color(255, 50, 50), "An item was stolen from you!", true);
                    }

                    /*byte extraTries = 30;
                    for (int i = 0; i < 3; i++)
                    {
                        bool successfulSteal = StealFromInventory(target, ref target.inventory[Main.rand.Next(target.inventory.Length)]);

                        if (!successfulSteal && extraTries > 0)
                        {
                            extraTries--;
                            i--;
                        }
                    }*/
                }
            }
        }

        public override void OnKill(NPC npc)
        {
            base.OnKill(npc);

            if (Main.netMode != NetmodeID.MultiplayerClient)
                Projectile.NewProjectile(npc.GetSpawnSource_ForProjectile(), npc.Center, new Vector2(Main.rand.NextFloat(-2f, 2f), -5), ModContent.ProjectileType<GoblinSpikyBall>(), FargoSoulsUtil.ScaledProjectileDamage(npc.damage), 0, Main.myPlayer);
            
            if (NPC.downedGoblins && !FargoSoulsWorld.haveForcedAbomFromGoblins)
            {
                FargoSoulsWorld.haveForcedAbomFromGoblins = true;

                if (ModContent.TryFind("Fargowiltas", "Abominationn", out ModNPC modNPC) && !NPC.AnyNPCs(modNPC.Type))
                {
                    int p = Player.FindClosest(npc.Center, 0, 0);
                    if (p != -1)
                        NPC.SpawnOnPlayer(p, modNPC.Type);
                }
            }
        }
    }
}
