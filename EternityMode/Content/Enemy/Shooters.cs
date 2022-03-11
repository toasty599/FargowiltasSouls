using FargowiltasSouls.Buffs.Masomode;
using FargowiltasSouls.EternityMode.Net;
using FargowiltasSouls.EternityMode.Net.Strategies;
using FargowiltasSouls.EternityMode.NPCMatching;
using FargowiltasSouls.Projectiles.Masomode;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.EternityMode.Content.Enemy
{
    public abstract class Shooters : EModeNPCBehaviour
    {
        protected readonly int AttackThreshold;
        protected readonly int ProjectileType;
        protected readonly int DustType;
        protected readonly float Distance;
        protected readonly float Speed;
        protected readonly float DamageMultiplier;

        protected Shooters(int attackThreshold, int projectileType, float speed, float damageMultiplier = 1f, int dustType = -1, float distance = 1000)
        {
            AttackThreshold = attackThreshold;
            ProjectileType = projectileType;
            DustType = dustType;
            Distance = distance;
            Speed = speed;
            DamageMultiplier = damageMultiplier;
        }

        public int AttackTimer;

        public override Dictionary<Ref<object>, CompoundStrategy> GetNetInfo() =>
            new Dictionary<Ref<object>, CompoundStrategy> {
                { new Ref<object>(AttackTimer), IntStrategies.CompoundStrategy },
            };

        public override void AI(NPC npc)
        {
            base.AI(npc);

            AttackTimer++;

            if (AttackTimer >= AttackThreshold - 90)
            {
                if (AttackTimer == AttackThreshold - 90)
                {
                    if (!npc.HasPlayerTarget || npc.Distance(Main.player[npc.target].Center) > Distance)
                        AttackTimer = 0;

                    npc.netUpdate = true;
                    NetSync(npc);
                }

                int d = Dust.NewDust(npc.position, npc.width, npc.height, DustType, npc.velocity.X * 0.4f, npc.velocity.Y * 0.4f);
                Main.dust[d].noGravity = true;
                Main.dust[d].velocity *= 2f;
                Main.dust[d].scale = 0.5f + 2.5f * (AttackTimer - AttackThreshold + 90) / 60f;
            }

            if (AttackTimer > AttackThreshold)
            {
                AttackTimer = -Main.rand.Next(60);
                npc.velocity = Vector2.Zero;

                npc.netUpdate = true;
                NetSync(npc);

                if (npc.HasPlayerTarget && npc.Distance(Main.player[npc.target].Center) < Distance && Main.netMode != NetmodeID.MultiplayerClient)
                    Projectile.NewProjectile(npc.GetSpawnSource_ForProjectile(), npc.Center, Speed * npc.DirectionTo(Main.player[npc.target].Center), ProjectileType, (int)(npc.damage / 4f * DamageMultiplier), 0, Main.myPlayer);
            }
        }
    }
}
