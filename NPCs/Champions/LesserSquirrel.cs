using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using FargowiltasSouls.Projectiles.DeviBoss;
using Terraria.GameContent.Bestiary;

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
            //Main.npcCatchable[NPC.type] = true;
            //NPC.catchItem = (short)ModContent.ItemType<TophatSquirrel>();
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.value = 0f;
            NPC.knockBackResist = .25f;
            //banner = NPC.type;
            //bannerItem = ModContent.ItemType<TophatSquirrelBanner>();

            AnimationType = NPCID.Squirrel;
            NPC.aiStyle = 7;
            AIType = NPCID.Squirrel;

            //NPCID.Sets.TownCritter[NPC.type] = true;

            //NPC.closeDoor;

            NPC.dontTakeDamage = true;
        }

        public override void AI()
        {
            if (NPC.velocity.Y == 0)
                NPC.dontTakeDamage = false;

            if (++counter > 600)
            {
                NPC.StrikeNPCNoInteraction(9999, 0f, 0);
            }
        }

        public override bool CheckDead()
        {
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                int p = Player.FindClosest(NPC.Center, 0, 0);
                int n = NPC.FindFirstNPC(ModContent.NPCType<TimberChampion>());
                if (p != -1 && n != -1)
                {
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, 4f * NPC.DirectionTo(Main.player[p].Center),
                        ModContent.ProjectileType<DeviLostSoul>(), FargoSoulsUtil.ScaledProjectileDamage(Main.npc[n].damage), 0, Main.myPlayer);
                }
            }
            return true;
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            if (NPC.life <= 0)
                for (int k = 0; k < 20; k++)
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, 5, hitDirection, -1f);
        }
    }
}
