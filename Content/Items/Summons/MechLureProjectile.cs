using FargowiltasSouls.Content.Bosses.BanishedBaron;
using FargowiltasSouls.Content.Bosses.TrojanSquirrel;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Summons
{
    public class MechLureProjectile : ModProjectile
    {

        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 6;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 33;
            Projectile.height = 42;
            Projectile.hostile = false;
            Projectile.friendly = false;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 60 * 8;
            Projectile.light = 1;
        }
        public override void AI()
        {
            if (++Projectile.frameCounter >= 5)
            {
                Projectile.frameCounter = 0;
                Projectile.frame = ++Projectile.frame % Main.projFrames[Projectile.type];
            }

            if (Projectile.ai[0] == 120)
            {
                SoundEngine.PlaySound(new SoundStyle("FargowiltasSouls/Assets/Sounds/BaronSummon"), Projectile.Center);

                int playerID = Player.FindClosest(Projectile.Center, 1, 1);
                if (Main.netMode != NetmodeID.MultiplayerClient && Main.player[playerID] != null && Main.player[playerID].active)
                {
                    NPC.SpawnOnPlayer(playerID, ModContent.NPCType<BanishedBaron>());

                }
            }
            Projectile.velocity *= 0.978f;

            if (Projectile.velocity.ToRotation() > MathHelper.Pi)
            {
                Projectile.rotation = 0f - MathHelper.Pi * Projectile.velocity.X / 25;
            }
            else
            {
                Projectile.rotation = 0f + MathHelper.Pi * Projectile.velocity.X / 25;
            }

            int baronType = ModContent.NPCType<BanishedBaron>();
            int baronID = NPC.FindFirstNPC(baronType);
            if (baronID >= 0 && baronID < Main.maxNPCs)
            {
                NPC baron = Main.npc[baronID];
                if (baron != null && baron.active && baron.type == baronType)
                {
                    if (Projectile.Colliding(Projectile.Hitbox, baron.Hitbox))
                    {
                        Projectile.Kill();
                    }
                }
            }
            
            Projectile.ai[0]++;
        }
    }
}