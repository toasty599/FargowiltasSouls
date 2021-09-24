using Microsoft.Xna.Framework;
using System;
using System.Linq;
using Terraria;
using Terraria.ID;
using FargowiltasSouls.NPCs;
using FargowiltasSouls.Projectiles;
using Terraria.Localization;

namespace FargowiltasSouls
{
    public static class FargoSoulsUtil
    {
        public static Projectile[] XWay(int num, Vector2 pos, int type, float speed, int damage, float knockback)
        {
            Projectile[] projs = new Projectile[num];
            double spread = 2 * Math.PI / num;
            for (int i = 0; i < num; i++)
                projs[i] = NewProjectileDirectSafe(pos, new Vector2(speed, speed).RotatedBy(spread * i), type, damage, knockback, Main.myPlayer);
            return projs;
        }

        public static Projectile NewProjectileDirectSafe(Vector2 pos, Vector2 vel, int type, int damage, float knockback, int owner = 255, float ai0 = 0f, float ai1 = 0f)
        {
            int p = Projectile.NewProjectile(pos, vel, type, damage, knockback, owner, ai0, ai1);
            return p < Main.maxProjectiles ? Main.projectile[p] : null;
        }

        public static int GetByUUIDReal(int player, float projectileIdentity, params int[] projectileType)
        {
            return GetByUUIDReal(player, (int)projectileIdentity, projectileType);
        }

        public static int GetByUUIDReal(int player, int projectileIdentity, params int[] projectileType)
        {
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                if (Main.projectile[i].active && Main.projectile[i].identity == projectileIdentity && Main.projectile[i].owner == player
                    && (projectileType.Length == 0 || projectileType.Contains(Main.projectile[i].type)))
                {
                    return i;
                }
            }
            return -1;
        }

        public static bool IsMinionDamage(Projectile projectile, bool includeMinionShot = true)
        {
            if (projectile.melee || projectile.ranged || projectile.magic)
                return false;
            return projectile.minion || projectile.sentry || projectile.minionSlots > 0 || (includeMinionShot && (ProjectileID.Sets.MinionShot[projectile.type] || ProjectileID.Sets.SentryShot[projectile.type]));
        }

        public static bool CanDeleteProjectile(Projectile projectile, int deletionRank = 0)
        {
            if (!projectile.active)
                return false;
            if (projectile.damage <= 0)
                return false;
            if (projectile.GetGlobalProjectile<FargoGlobalProjectile>().DeletionImmuneRank > deletionRank)
                return false;
            if (projectile.friendly)
            {
                if (projectile.whoAmI == Main.player[projectile.owner].heldProj)
                    return false;
                if (IsMinionDamage(projectile, false))
                    return false;
            }
            return true;
        }

        public static Player PlayerExists(int whoAmI)
        {
            return whoAmI > -1 && whoAmI < Main.maxPlayers && Main.player[whoAmI].active /*&& !Main.player[whoAmI].dead*/ && !Main.player[whoAmI].ghost ? Main.player[whoAmI] : null;
        }

        public static Player PlayerExists(float whoAmI)
        {
            return PlayerExists((int)whoAmI);
        }

        public static Projectile ProjectileExists(int whoAmI, params int[] types)
        {
            return whoAmI > -1 && whoAmI < Main.maxProjectiles && Main.projectile[whoAmI].active && (types.Length == 0 || types.Contains(Main.projectile[whoAmI].type)) ? Main.projectile[whoAmI] : null;
        }

        public static Projectile ProjectileExists(float whoAmI, params int[] types)
        {
            return ProjectileExists((int)whoAmI, types);
        }

        public static NPC NPCExists(int whoAmI, params int[] types)
        {
            return whoAmI > -1 && whoAmI < Main.maxNPCs && Main.npc[whoAmI].active && (types.Length == 0 || types.Contains(Main.npc[whoAmI].type)) ? Main.npc[whoAmI] : null;
        }

        public static NPC NPCExists(float whoAmI, params int[] types)
        {
            return NPCExists((int)whoAmI, types);
        }

        public static bool OtherBossAlive(int npcID)
        {
            if (npcID > -1 && npcID < Main.maxNPCs)
            {
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    if (Main.npc[i].active && Main.npc[i].boss && i != npcID)
                        return true;
                }
            }
            return false;
        }

        public static bool BossIsAlive(ref int bossID, int bossType)
        {
            if (bossID != -1)
            {
                if (Main.npc[bossID].active && Main.npc[bossID].type == bossType)
                {
                    return true;
                }
                else
                {
                    bossID = -1;
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public static bool AnyBossAlive()
        {
            if (EModeGlobalNPC.boss == -1)
                return false;
            if (Main.npc[EModeGlobalNPC.boss].active && (Main.npc[EModeGlobalNPC.boss].boss || Main.npc[EModeGlobalNPC.boss].type == NPCID.EaterofWorldsHead))
                return true;
            EModeGlobalNPC.boss = -1;
            return false;
        }

        public static void ClearFriendlyProjectiles(int deletionRank = 0, int bossNpc = -1)
        {
            ClearProjectiles(false, true, deletionRank, bossNpc);
        }

        public static void ClearHostileProjectiles(int deletionRank = 0, int bossNpc = -1)
        {
            ClearProjectiles(true, false, deletionRank, bossNpc);
        }

        public static void ClearAllProjectiles(int deletionRank = 0, int bossNpc = -1)
        {
            ClearProjectiles(true, true, deletionRank, bossNpc);
        }

        private static void ClearProjectiles(bool clearHostile, bool clearFriendly, int deletionRank = 0, int bossNpc = -1)
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
                return;

            if (OtherBossAlive(bossNpc))
                clearHostile = false;

            for (int j = 0; j < 2; j++) //do twice to wipe out projectiles spawned by projectiles
            {
                for (int i = 0; i < Main.maxProjectiles; i++)
                {
                    Projectile projectile = Main.projectile[i];
                    if (projectile.active && ((projectile.hostile && clearHostile) || (projectile.friendly && clearFriendly)) && CanDeleteProjectile(projectile, deletionRank))
                    {
                        projectile.Kill();
                    }
                }
            }
        }

        public static void PrintAI(NPC npc)
        {
            Main.NewText($"{npc.whoAmI} ai: {npc.ai[0]} {npc.ai[1]} {npc.ai[2]} {npc.ai[3]}, local: {npc.localAI[0]} {npc.localAI[1]} {npc.localAI[2]} {npc.localAI[3]}");
        }

        public static void PrintAI(Projectile projectile)
        {
            Main.NewText($"{projectile.whoAmI} ai: {projectile.ai[0]} {projectile.ai[1]}, local: {projectile.localAI[0]} {projectile.localAI[1]}");
        }

        public static void GrossVanillaDodgeDust(Entity entity)
        {
            for (int index1 = 0; index1 < 100; ++index1)
            {
                int index2 = Dust.NewDust(entity.position, entity.width, entity.height, 31, 0.0f, 0.0f, 100, new Color(), 2f);
                Main.dust[index2].position.X += Main.rand.Next(-20, 21);
                Main.dust[index2].position.Y += Main.rand.Next(-20, 21);
                Dust dust = Main.dust[index2];
                dust.velocity *= 0.4f;
                Main.dust[index2].scale *= 1f + Main.rand.Next(40) * 0.01f;
                if (Main.rand.Next(2) == 0)
                {
                    Main.dust[index2].scale *= 1f + Main.rand.Next(40) * 0.01f;
                    Main.dust[index2].noGravity = true;
                }
            }

            int index3 = Gore.NewGore(new Vector2(entity.Center.X - 24, entity.Center.Y - 24), new Vector2(), Main.rand.Next(61, 64), 1f);
            Main.gore[index3].scale = 1.5f;
            Main.gore[index3].velocity.X = Main.rand.Next(-50, 51) * 0.01f;
            Main.gore[index3].velocity.Y = Main.rand.Next(-50, 51) * 0.01f;
            Main.gore[index3].velocity *= 0.4f;

            int index4 = Gore.NewGore(new Vector2(entity.Center.X - 24, entity.Center.Y - 24), new Vector2(), Main.rand.Next(61, 64), 1f);
            Main.gore[index4].scale = 1.5f;
            Main.gore[index4].velocity.X = 1.5f + Main.rand.Next(-50, 51) * 0.01f;
            Main.gore[index4].velocity.Y = 1.5f + Main.rand.Next(-50, 51) * 0.01f;
            Main.gore[index4].velocity *= 0.4f;

            int index5 = Gore.NewGore(new Vector2(entity.Center.X - 24, entity.Center.Y - 24), new Vector2(), Main.rand.Next(61, 64), 1f);
            Main.gore[index5].scale = 1.5f;
            Main.gore[index5].velocity.X = -1.5f - Main.rand.Next(-50, 51) * 0.01f;
            Main.gore[index5].velocity.Y = 1.5f + Main.rand.Next(-50, 51) * 0.01f;
            Main.gore[index5].velocity *= 0.4f;

            int index6 = Gore.NewGore(new Vector2(entity.Center.X - 24, entity.Center.Y - 24), new Vector2(), Main.rand.Next(61, 64), 1f);
            Main.gore[index6].scale = 1.5f;
            Main.gore[index6].velocity.X = 1.5f - Main.rand.Next(-50, 51) * 0.01f;
            Main.gore[index6].velocity.Y = -1.5f + Main.rand.Next(-50, 51) * 0.01f;
            Main.gore[index6].velocity *= 0.4f;

            int index7 = Gore.NewGore(new Vector2(entity.Center.X - 24, entity.Center.Y - 24), new Vector2(), Main.rand.Next(61, 64), 1f);
            Main.gore[index7].scale = 1.5f;
            Main.gore[index7].velocity.X = -1.5f - Main.rand.Next(-50, 51) * 0.01f;
            Main.gore[index7].velocity.Y = -1.5f + Main.rand.Next(-50, 51) * 0.01f;
            Main.gore[index7].velocity *= 0.4f;
        }

        public static int FindClosestHostileNPC(Vector2 location, float detectionRange, bool lineCheck = false)
        {
            NPC closestNpc = null;
            foreach (NPC n in Main.npc)
            {
                if (n.CanBeChasedBy() && n.Distance(location) < detectionRange && (!lineCheck || Collision.CanHitLine(location, 0, 0, n.Center, n.width, n.height)))
                {
                    detectionRange = n.Distance(location);
                    closestNpc = n;
                }
            }
            return closestNpc == null ? -1 : closestNpc.whoAmI;
        }

        public static int FindClosestHostileNPCPrioritizingMinionFocus(Projectile projectile, float detectionRange, bool lineCheck = false)
        {
            NPC minionAttackTargetNpc = projectile.OwnerMinionAttackTargetNPC;
            if (minionAttackTargetNpc != null && minionAttackTargetNpc.CanBeChasedBy() && projectile.Distance(minionAttackTargetNpc.Center) < detectionRange
                && (!lineCheck || Collision.CanHitLine(projectile.Center, 0, 0, minionAttackTargetNpc.position, 0, 0)))
            {
                return minionAttackTargetNpc.whoAmI;
            }
            return FindClosestHostileNPC(projectile.Center, detectionRange);
        }

        public static void DustRing(Vector2 location, int max, int dust, float speed, Color color = default, float scale = 1f)
        {
            for (int i = 0; i < max; i++)
            {
                Vector2 velocity = speed * Vector2.UnitY.RotatedBy(MathHelper.TwoPi / max * i);
                int d = Dust.NewDust(location, 0, 0, dust, newColor: color);
                Main.dust[d].noGravity = true;
                Main.dust[d].velocity = velocity;
                Main.dust[d].scale = scale;
            }
        }

        public static void PrintText(string text)
        {
            PrintText(text, Color.White);
        }

        public static void PrintText(string text, Color color)
        {
            if (Main.netMode == NetmodeID.SinglePlayer)
            {
                Main.NewText(text, color);
            }
            else if (Main.netMode == NetmodeID.Server)
            {
                NetMessage.BroadcastChatMessage(NetworkText.FromLiteral(text), color);
            }
        }

        public static Vector2 ClosestPointInHitbox(Entity entity, Vector2 desiredLocation)
        {
            Vector2 offset = desiredLocation - entity.Center;
            offset.X = Math.Min(Math.Abs(offset.X), entity.width / 2) * Math.Sign(offset.X);
            offset.Y = Math.Min(Math.Abs(offset.Y), entity.height / 2) * Math.Sign(offset.Y);
            return entity.Center + offset;
        }
    }
}
