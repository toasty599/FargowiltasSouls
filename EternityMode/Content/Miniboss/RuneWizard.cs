using FargowiltasSouls.Buffs.Masomode;
using FargowiltasSouls.EternityMode.Content.Enemy;
using FargowiltasSouls.EternityMode.NPCMatching;
using FargowiltasSouls.Items.Accessories.Masomode;
using FargowiltasSouls.NPCs;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.EternityMode.Content.Miniboss
{
    public class RuneWizard : FireImp
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.RuneWizard);

        public int AttackTimer;

        public override void SetDefaults(NPC npc)
        {
            base.SetDefaults(npc);

            npc.buffImmune[BuffID.OnFire] = true;
            npc.lavaImmune = true;
            npc.lifeMax *= 4;
            npc.damage /= 2;
        }

        public override void AI(NPC npc)
        {
            if (++AttackTimer > 300)
            {
                AttackTimer = 0;
                if (Main.netMode != NetmodeID.MultiplayerClient && npc.HasPlayerTarget)
                {
                    Vector2 vel = npc.DirectionFrom(Main.player[npc.target].Center) * 8f;
                    for (int i = 0; i < 5; i++)
                    {
                        int p = Projectile.NewProjectile(npc.Center, vel.RotatedBy(2 * Math.PI / 5 * i),
                            ProjectileID.RuneBlast, 30, 0f, Main.myPlayer, 1);
                        if (p != Main.maxProjectiles)
                            Main.projectile[p].timeLeft = 300;
                    }
                }
            }

            EModeGlobalNPC.Aura(npc, 450f, true, 74, Color.GreenYellow, ModContent.BuffType<Hexed>());
            EModeGlobalNPC.Aura(npc, 150f, false, 73, default, ModContent.BuffType<Hexed>(), BuffID.Suffocation);

            /*if (npc.Distance(Main.player[Main.myPlayer].Center) < 1500f)
            {
                if (npc.Distance(Main.player[Main.myPlayer].Center) > 450f)
                    Main.player[Main.myPlayer].AddBuff(ModContent.BuffType<Hexed>(), 2);

                for (int i = 0; i < 20; i++)
                {
                    Vector2 offset = new Vector2();
                    double angle = Main.rand.NextDouble() * 2d * Math.PI;
                    offset.X += (float)(Math.Sin(angle) * 450);
                    offset.Y += (float)(Math.Cos(angle) * 450);
                    Dust dust = Main.dust[Dust.NewDust(
                        npc.Center + offset - new Vector2(4, 4), 0, 0,
                        74, 0, 0, 100, default(Color), 1f
                        )];
                    dust.velocity = npc.velocity;
                    if (Main.rand.NextBool(3))
                        dust.velocity += Vector2.Normalize(offset) * 5f;
                    dust.noGravity = true;
                    dust.color = Color.GreenYellow;
                }
            }*/

            /*if (npc.Distance(Main.player[Main.myPlayer].Center) < 150)
            {
                Main.player[Main.myPlayer].AddBuff(BuffID.Suffocation, 2);
                Main.player[Main.myPlayer].AddBuff(ModContent.BuffType<Hexed>(), 2);
            }
            for (int i = 0; i < 10; i++)
            {
                Vector2 offset = new Vector2();
                double angle = Main.rand.NextDouble() * 2d * Math.PI;
                offset.X += (float)(Math.Sin(angle) * 150);
                offset.Y += (float)(Math.Cos(angle) * 150);
                Dust dust = Main.dust[Dust.NewDust(
                    npc.Center + offset - new Vector2(4, 4), 0, 0,
                    73, 0, 0, 100, default(Color), 1f
                    )];
                dust.velocity = npc.velocity;
                if (Main.rand.NextBool(3))
                    dust.velocity -= Vector2.Normalize(offset) * 5f;
                dust.noGravity = true;
            }*/
        }

        public override void NPCLoot(NPC npc)
        {
            base.NPCLoot(npc);

            if (Main.rand.NextBool(5))
                Item.NewItem(npc.Hitbox, ModContent.ItemType<MysticSkull>());
        }
    }
}
