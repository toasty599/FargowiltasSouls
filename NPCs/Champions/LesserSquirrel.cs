using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using FargowiltasSouls.Projectiles.DeviBoss;
using Terraria.GameContent.Bestiary;
using System.Linq;
using FargowiltasSouls.Projectiles.Champions;

namespace FargowiltasSouls.NPCs.Champions
{
    public class LesserSquirrel : ModNPC
    {
        public override string Texture => "FargowiltasSouls/NPCs/Critters/TophatSquirrelCritter";

        public int counter;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Lesser Squirrel");
            DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "小松鼠");
            Main.npcFrameCount[NPC.type] = 6;
            NPCID.Sets.BossBestiaryPriority.Add(NPC.type);
            NPCID.Sets.CantTakeLunchMoney[Type] = true;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.UIInfoProvider = new CommonEnemyUICollectionInfoProvider(
                   ContentSamples.NpcBestiaryCreditIdsByNpcNetIds[ModContent.NPCType<TimberChampion>()],
                   quickUnlock: true
               );
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface,
                new FlavorTextBestiaryInfoElement($"Mods.FargowiltasSouls.Bestiary.{Name}")
            });
        }

        public override void SetDefaults()
        {
            NPC.width = 50;
            NPC.height = 32;
            NPC.damage = 0;
            NPC.defense = 0;
            NPC.lifeMax = 1800;
            
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.value = 0f;
            NPC.knockBackResist = .1f;
            
            AnimationType = NPCID.Squirrel;

            NPC.aiStyle = 7;
            AIType = NPCID.Squirrel;
            if (FargoSoulsWorld.EternityMode)
            {
                NPC.aiStyle = NPCAIStyleID.Herpling;
                AIType = NPCID.Herpling;
            }

            NPC.dontTakeDamage = true;
        }

        public override void AI()
        {
            if (NPC.velocity.Y == 0)
            {
                if (!Main.projectile.Any(p => p.active && (p.type == ModContent.ProjectileType<TimberTree>() || p.type == ModContent.ProjectileType<TimberTreeAcorn>())))
                    NPC.dontTakeDamage = false;
            }

            if (++counter > 900)
            {
                NPC.StrikeNPCNoInteraction(9999, 0f, 0);
            }

            if (!NPC.dontTakeDamage && FargoSoulsWorld.EternityMode)
            {
                Vector2 nextPos = NPC.position;
                nextPos.X += NPC.velocity.X * 1.5f;
                if (NPC.velocity.Y < 0)
                    nextPos.Y += NPC.velocity.Y;
                if (!Collision.SolidTiles(nextPos, NPC.width, NPC.height))
                    NPC.position = nextPos;
            }
        }

        public override bool CheckDead()
        {
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                int p = Player.FindClosest(NPC.Center, 0, 0);
                NPC npc = Main.npc.FirstOrDefault(n => n.active && (n.type == ModContent.NPCType<TimberChampion>() || n.type == ModContent.NPCType<TimberChampionHead>()));
                if (p != -1 && npc is NPC)
                {
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, 4f * NPC.DirectionTo(Main.player[p].Center),
                        ModContent.ProjectileType<DeviLostSoul>(), FargoSoulsUtil.ScaledProjectileDamage(npc.damage), 0, Main.myPlayer);
                }
            }
            return true;
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            if (NPC.life <= 0)
            {
                for (int k = 0; k < 20; k++)
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, 5, hitDirection, -1f);

                if (!Main.dedServ)
                {
                    int g = Gore.NewGore(NPC.GetSource_FromThis(), NPC.Center, Main.rand.NextFloat(6f) * -Vector2.UnitY.RotatedByRandom(MathHelper.PiOver4), ModContent.Find<ModGore>(Mod.Name, Main.rand.NextBool() ? "TrojanSquirrelGore2" : "TrojanSquirrelGore2_2").Type, NPC.scale);
                    Main.gore[g].rotation = Main.rand.NextFloat(MathHelper.TwoPi);
                }
            }
        }
    }
}
