using Microsoft.Xna.Framework;
using System;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using FargowiltasSouls.NPCs;
using FargowiltasSouls.Projectiles;

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

        public static bool IsMinionDamage(Projectile projectile)
        {
            if (projectile.melee || projectile.ranged || projectile.magic)
                return false;
            return projectile.minion || projectile.sentry || projectile.minionSlots > 0 || ProjectileID.Sets.MinionShot[projectile.type] || ProjectileID.Sets.SentryShot[projectile.type];
        }

        public static bool CanDeleteProjectile(Projectile projectile, bool obeyGuttedHeartImmune = false, bool obeyDeletionImmune = true)
        {
            if (projectile.damage <= 0)
                return false;
            if (obeyGuttedHeartImmune && projectile.GetGlobalProjectile<FargoGlobalProjectile>().ImmuneToGuttedHeart)
                return false;
            if (obeyDeletionImmune && projectile.GetGlobalProjectile<FargoGlobalProjectile>().ImmuneToDeletion)
                return false;
            if (projectile.friendly)
            {
                if (projectile.whoAmI == Main.player[projectile.owner].heldProj)
                    return false;
                if (ProjectileID.Sets.MinionShot[projectile.type] || ProjectileID.Sets.SentryShot[projectile.type])
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
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                if (Main.npc[i].active && Main.npc[i].boss && i != npcID)
                    return true;
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

        public static void ClearAllProjectiles(bool clearHostile, bool clearFriendly, int bossNpc)
        {
            if (OtherBossAlive(bossNpc))
                clearHostile = false;
            ClearAllProjectiles(clearHostile, clearFriendly);
        }

        public static void ClearAllProjectiles(bool clearHostile, bool clearFriendly)
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
                return;

            for (int j = 0; j < 2; j++) //do twice to wipe out projectiles spawned by projectiles
            {
                for (int i = 0; i < Main.maxProjectiles; i++)
                {
                    Projectile projectile = Main.projectile[i];
                    if (projectile.active && ((projectile.hostile && clearHostile) || (projectile.friendly && clearFriendly)) && CanDeleteProjectile(projectile))
                    {
                        projectile.Kill();
                    }
                }
            }
        }

        public static void PrintAI(NPC npc)
        {
            Main.NewText($"ai: {npc.ai[0]} {npc.ai[1]} {npc.ai[2]} {npc.ai[3]}, local: {npc.localAI[0]} {npc.localAI[1]} {npc.localAI[2]} {npc.localAI[3]}");
        }

        public static void PrintAI(Projectile projectile)
        {
            Main.NewText($"ai: {projectile.ai[0]} {projectile.ai[1]}, local: {projectile.localAI[0]} {projectile.localAI[1]}");
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
    }
}
