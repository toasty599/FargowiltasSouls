using FargowiltasSouls.Buffs.Masomode;
using FargowiltasSouls.EternityMode.Net;
using FargowiltasSouls.EternityMode.Net.Strategies;
using FargowiltasSouls.EternityMode.NPCMatching;
using FargowiltasSouls.ItemDropRules.Conditions;
using FargowiltasSouls.Items.Accessories.Masomode;
using FargowiltasSouls.NPCs;
using FargowiltasSouls.Projectiles;
using FargowiltasSouls.Projectiles.Masomode;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.GameContent.Events;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.EternityMode.Content.Boss.HM
{
    public class EmpressofLight : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.HallowBoss);

        public int AttackTimer;
        public int DashCounter;

        public bool DroppedSummon;

        private float startRotation;
        private Vector2 targetPos;

        public override Dictionary<Ref<object>, CompoundStrategy> GetNetInfo() =>
            new Dictionary<Ref<object>, CompoundStrategy> {
                { new Ref<object>(AttackTimer), IntStrategies.CompoundStrategy },
                { new Ref<object>(DashCounter), IntStrategies.CompoundStrategy },
            };

        public override void SetDefaults(NPC npc)
        {
            base.SetDefaults(npc);

            npc.lifeMax = (int)Math.Round(npc.lifeMax * 1.5, MidpointRounding.ToEven);
        }

        public override bool CanHitPlayer(NPC npc, Player target, ref int CooldownSlot)
        {
            //only have contact damage when dashing
            if ((npc.ai[0] == 8 || npc.ai[0] == 9) && npc.ai[1] >= 40)
                return base.CanHitPlayer(npc, target, ref CooldownSlot);

            return false;
        }

        public override bool PreAI(NPC npc)
        {
            EModeGlobalNPC.empressBoss = npc.whoAmI;

            if (FargoSoulsWorld.SwarmActive)
                return true;

            if (Main.LocalPlayer.active && !Main.LocalPlayer.dead && !Main.LocalPlayer.ghost)
                Main.LocalPlayer.AddBuff(ModContent.BuffType<Purged>(), 2);

            bool useP2Attacks = npc.ai[3] != 0 || FargoSoulsWorld.MasochistModeReal;

            //defdamage multiplier because expert empress actually has 184 defdamage for some reason and not 110?????
            int baseDamage = Main.dayTime ? 2000 : (int)(npc.defDamage * 0.6);

            switch ((int)npc.ai[0])
            {
                //0 spawn

                case 1: //move over player
                    AttackTimer = 0;
                    break;

                case 2: //homing bolts, ends at ai1=130
                    if (useP2Attacks && npc.ai[1] > 80 && !FargoSoulsWorld.MasochistModeReal)
                        npc.ai[1] -= 0.5f; //p2, more delay before next attack
                    break;

                case 4: //pseudorandom swords following you, ends at ai1=100
                    if (npc.ai[1] == 0)
                    {
                        AttackTimer = 0;
                        NetSync(npc);
                    }

                    //p2 only, sword circle around player
                    if (npc.ai[1] > 97 && useP2Attacks)
                    {
                        int startDelay = FargoSoulsWorld.MasochistModeReal ? 40 : 60;

                        if (AttackTimer == 0)
                        {
                            Terraria.Audio.SoundEngine.PlaySound(SoundID.Roar, npc.HasValidTarget ? Main.player[npc.target].Center : npc.Center, 0);
                        }
                        else if (AttackTimer == startDelay)
                        {
                            targetPos = Main.player[npc.target].Center;
                            startRotation = npc.HasValidTarget ? Main.player[npc.target].velocity.ToRotation() : 0;
                        }

                        AttackTimer++;

                        const float radius = 600;
                        if (Main.player[npc.target].Distance(targetPos) > radius)
                            targetPos = Main.player[npc.target].Center + Main.player[npc.target].DirectionTo(targetPos) * radius;

                        if (AttackTimer % 90 == 0) //rapid fire sound effect
                            Terraria.Audio.SoundEngine.PlaySound(SoundID.Item164, Main.player[npc.target].Center);

                        int spinTime = /*FargoSoulsWorld.MasochistModeReal ? 120 :*/ 160;
                        float spins = FargoSoulsWorld.MasochistModeReal ? 2 : 1.5f;
                        if (AttackTimer > startDelay && AttackTimer <= spinTime * spins + startDelay && AttackTimer % 2 == 0)
                        {
                            int max = FargoSoulsWorld.MasochistModeReal ? 3 : 2;
                            for (int i = 0; i < max; i++)
                            {
                                Vector2 offsetDirection = Vector2.UnitX.RotatedBy(startRotation + MathHelper.TwoPi / spinTime * AttackTimer + MathHelper.TwoPi / max * i);
                                Vector2 spawnPos = targetPos + radius * offsetDirection;
                                Vector2 vel = Vector2.Normalize(targetPos - spawnPos);
                                float ai1 = ((float)(AttackTimer - startDelay) / spinTime) % 1;
                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    Vector2 appearVel = -vel;
                                    appearVel *= FargoSoulsWorld.MasochistModeReal ? 7.5f : 2.5f;
                                    Projectile.NewProjectile(npc.GetSource_FromThis(), spawnPos - appearVel * 60, appearVel, ProjectileID.FairyQueenLance, FargoSoulsUtil.ScaledProjectileDamage(baseDamage, 1.5f), 0f, Main.myPlayer, vel.ToRotation(), ai1);

                                    float angleOffset = MathHelper.ToRadians(45);
                                    for (int j = -1; j <= 1; j++)
                                    {
                                        float length = 128f * (j + 2) + radius;
                                        Vector2 newPos = targetPos + length * offsetDirection;
                                        Vector2 fancyVel = 2.5f * (j + 2) * vel.RotatedBy(MathHelper.PiOver2 * j);
                                        Projectile.NewProjectile(npc.GetSource_FromThis(), newPos - fancyVel * 60f, fancyVel, ProjectileID.FairyQueenLance, FargoSoulsUtil.ScaledProjectileDamage(baseDamage, 1.5f), 0f, Main.myPlayer, vel.ToRotation() + MathHelper.Pi + angleOffset * j, ai1);
                                    }
                                }
                            }
                        }

                        if (!npc.HasValidTarget)
                        {
                            npc.TargetClosest(false);
                            if (!npc.HasValidTarget)
                                AttackTimer += 9000;
                        }

                        if (AttackTimer < spinTime * spins + startDelay * 2)
                            npc.ai[1] = 97; //stop vanilla ai from progressing
                    }
                    break;

                case 5: //stupid long trail circle ring
                    //if (useP2Attacks && npc.ai[1] > 10 && !FargoSoulsWorld.MasochistModeReal)
                    //    npc.ai[1] -= 0.5f; //p2, more delay before next attack

                    //p2, do sword rings
                    if (useP2Attacks)
                    {
                        if (npc.ai[1] == 20)
                            startRotation = Main.rand.NextFloat(MathHelper.TwoPi);

                        if (npc.ai[1] >= 20 && npc.ai[1] <= (FargoSoulsWorld.MasochistModeReal ? 60 : 45))
                        {
                            npc.position -= npc.velocity;
                            npc.velocity = Vector2.Zero;

                            if (npc.ai[1] % 5 == 0)
                            {
                                for (float i = 0; i < 1; i += 1f / 24f)
                                {
                                    Vector2 spinningpoint = Vector2.UnitY.RotatedBy(MathHelper.PiOver2 + MathHelper.TwoPi * i + startRotation);
                                    if (Main.netMode != NetmodeID.MultiplayerClient)
                                        Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center + spinningpoint.RotatedBy(-MathHelper.PiOver2) * 30f, Vector2.Zero, ProjectileID.FairyQueenLance, FargoSoulsUtil.ScaledProjectileDamage(baseDamage, 1.3f), 0f, Main.myPlayer, spinningpoint.ToRotation(), i);
                                }
                            }
                        }
                    }
                    break;

                case 6: //sun rays
                    if (!FargoSoulsWorld.MasochistModeReal)
                    {
                        if (npc.ai[1] == 0 && AttackTimer == 0 && Main.projectile.Any(p => p.active && p.type == ProjectileID.HallowBossRainbowStreak))
                        {
                            npc.ai[1] -= 30;
                            npc.netUpdate = true;
                        }

                        npc.position -= npc.velocity / (npc.ai[3] == 0 ? 2 : 4); //move slower
                    }
                    break;

                case 7: //sword walls, ends at ai1=260
                    {
                        int start = FargoSoulsWorld.MasochistModeReal ? -15 : -45;

                        if (npc.ai[1] == 0)
                        {
                            AttackTimer = start;

                            if (FargoSoulsWorld.MasochistModeReal) //RANDOM ATTACKS
                                npc.ai[2] += Main.rand.Next(3);

                            npc.netUpdate = true;
                            NetSync(npc);
                        }

                        if (npc.ai[1] > 255) //add parallel sword walls
                        {
                            if (AttackTimer == start)
                            {
                                Terraria.Audio.SoundEngine.PlaySound(SoundID.Roar, npc.HasValidTarget ? Main.player[npc.target].Center : npc.Center, 0);
                                
                                startRotation = Main.rand.NextFloat(MathHelper.TwoPi);
                            }

                            int waveDelay = /*FargoSoulsWorld.MasochistModeReal ? 20 :*/ 30;
                            const int spaceCovered = 800;
                            if (++AttackTimer > 0)
                            {
                                if (AttackTimer % waveDelay == 0)
                                {
                                    float ai1 = 1;
                                    Vector2 spawnPos = targetPos;
                                    spawnPos += 600f * Vector2.UnitX.RotatedBy(startRotation);
                                    spawnPos += MathHelper.Lerp(-spaceCovered, spaceCovered, ai1) * Vector2.UnitY.RotatedBy(startRotation);
                                    if (Main.netMode != NetmodeID.MultiplayerClient)
                                        Projectile.NewProjectile(npc.GetSource_FromThis(), spawnPos, Vector2.Zero, ProjectileID.FairyQueenLance, FargoSoulsUtil.ScaledProjectileDamage(baseDamage, 1.5f), 0f, Main.myPlayer, startRotation + MathHelper.Pi, ai1);

                                    targetPos = npc.HasValidTarget ? Main.player[npc.target].Center : npc.Center;
                                    startRotation += MathHelper.PiOver2 * (Main.rand.NextBool() ? -1 : 1);
                                    if (Main.rand.NextBool())
                                        startRotation += MathHelper.Pi;
                                    startRotation += MathHelper.ToRadians(FargoSoulsWorld.MasochistModeReal ? 30 : 15) * Main.rand.NextFloat(-1, 1);

                                    //whooshy sound effect
                                    if (AttackTimer % waveDelay * 4 == 0)
                                        Terraria.Audio.SoundEngine.PlaySound(SoundID.Item163, Main.player[npc.target].Center);
                                }

                                if (AttackTimer % /*(FargoSoulsWorld.MasochistModeReal ? 2 : 3)*/ 3 == 0)
                                {
                                    float ai1 = (float)(AttackTimer % waveDelay) / waveDelay;
                                    Vector2 spawnPos = targetPos;
                                    spawnPos += 600f * Vector2.UnitX.RotatedBy(startRotation);
                                    spawnPos += MathHelper.Lerp(-spaceCovered, spaceCovered, ai1) * Vector2.UnitY.RotatedBy(startRotation);

                                    if (Main.netMode != NetmodeID.MultiplayerClient)
                                    {
                                        Vector2 vel = (startRotation + MathHelper.Pi).ToRotationVector2();
                                        Vector2 appearVel = vel.RotatedBy(-MathHelper.PiOver2);
                                        appearVel *= FargoSoulsWorld.MasochistModeReal ? 1f : 0.5f;
                                        Projectile.NewProjectile(npc.GetSource_FromThis(), spawnPos - 60f * appearVel, appearVel, ProjectileID.FairyQueenLance, FargoSoulsUtil.ScaledProjectileDamage(baseDamage, 1.5f), 0f, Main.myPlayer, vel.ToRotation(), ai1);
                                    }
                                }
                            }

                            if (!npc.HasValidTarget)
                            {
                                npc.TargetClosest(false);
                                if (!npc.HasValidTarget)
                                    AttackTimer += 9000;
                            }

                            int waves = FargoSoulsWorld.MasochistModeReal ? 12 : 8;
                            if (AttackTimer < waveDelay * waves + waveDelay * 2)
                                npc.ai[1] = 255; //stop vanilla ai from progressing
                        }
                    }
                    break;

                case 8: //dash from right to left
                case 9: //dash from left to right
                    {
                        const int dashValue = 4;

                        if (npc.ai[1] == 0)
                        {
                            AttackTimer = 0;

                            if (--DashCounter <= 0) //sometimes do two consecutive dashes
                            {
                                DashCounter = dashValue;
                                npc.ai[2] -= 1;
                            }
                            else if (FargoSoulsWorld.MasochistModeReal) //RANDOM ATTACKS
                            {
                                npc.ai[2] += Main.rand.Next(3);
                            }

                            npc.netUpdate = true;
                            NetSync(npc);
                        }

                        //for the second consecutive dash
                        if (DashCounter == dashValue - 1 && npc.ai[1] < 40)
                        {
                            if (npc.ai[1] == 0) //add the sound since the longer startup broke it
                                Terraria.Audio.SoundEngine.PlaySound(SoundID.Item160, npc.Center);

                            if (npc.ai[1] < 39) //more startup on this one
                                npc.ai[1] -= 0.33f;
                            else
                                npc.ai[1] = 39; //fix any rounding issues
                        }

                        if (npc.ai[1] == 40) //add sun wings
                        {
                            float baseDirection = npc.ai[0] == 8 ? 0 : MathHelper.Pi;
                            for (int i = -2; i <= 2; i++)
                            {
                                if (i == 0)
                                    continue;

                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    float ai0 = baseDirection + MathHelper.ToRadians(20) / 2 * i;
                                    Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, Vector2.Zero, ProjectileID.FairyQueenSunDance, FargoSoulsUtil.ScaledProjectileDamage(baseDamage, 1.5f), 0f, Main.myPlayer, ai0, npc.whoAmI);
                                }
                            }
                        }

                        if (npc.ai[1] >= 40)
                        {
                            npc.ai[1] -= 0.33f; //extend the dash

                            if (!FargoSoulsWorld.MasochistModeReal)
                                npc.velocity.Y = 0;

                            if (useP2Attacks && ++AttackTimer % 15 == 0) //extra swords, p2 only
                            {
                                float baseDirection = npc.ai[0] == 8 ? 0 : MathHelper.Pi;
                                for (int i = -2; i <= 2; i++)
                                {
                                    if (i == 0)
                                        continue;

                                    if (Main.netMode != NetmodeID.MultiplayerClient)
                                    {
                                        float ai0 = baseDirection + MathHelper.ToRadians(40) / 2 * i;
                                        float ai1 = (npc.ai[1] - 40f) / 50f;
                                        Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, Vector2.Zero, ProjectileID.FairyQueenLance, FargoSoulsUtil.ScaledProjectileDamage(baseDamage, 1.5f), 0f, Main.myPlayer, ai0, ai1);
                                    }
                                }
                            }
                        }
                    }
                    break;

                case 10: //p2 transition
                    if (npc.dontTakeDamage && npc.ai[1] > 120)
                    {
                        if (npc.life < npc.lifeMax / 2)
                        {
                            npc.HealEffect(npc.lifeMax / 2 - npc.life);
                            npc.life = npc.lifeMax / 2;
                        }
                    }
                    break;

                case 11: //p2 direct sword trail
                    if ((FargoSoulsWorld.MasochistModeReal || npc.ai[1] > 40) && npc.ai[1] % 3 == 0 && npc.HasValidTarget) //add perpendicular swords
                    {
                        Vector2 offset = Main.player[npc.target].velocity;
                        if (offset == Vector2.Zero || offset.Length() < 1)
                            offset.SafeNormalize(-Vector2.UnitY);
                        offset = 90f * offset.RotatedBy(MathHelper.PiOver2);

                        Vector2 spawnPos = Main.player[npc.target].Center + offset;
                        Vector2 vel = Main.player[npc.target].DirectionFrom(spawnPos);

                        if (Main.netMode != NetmodeID.MultiplayerClient)
                            Projectile.NewProjectile(npc.GetSource_FromThis(), spawnPos, Vector2.Zero, ProjectileID.FairyQueenLance, FargoSoulsUtil.ScaledProjectileDamage(baseDamage, 1.3f), 0f, Main.myPlayer, vel.ToRotation(), npc.ai[1] / 100f);
                    }
                    break;

                case 12: //8-way homing bolts
                    {
                        const int max = 24;
                        const int delay = 75;

                        if (npc.ai[1] == delay)
                            startRotation = npc.HasValidTarget ? npc.DirectionTo(Main.player[npc.target].Center).ToRotation() : MathHelper.PiOver2;

                        if (npc.ai[1] >= delay && npc.ai[1] < delay + max)
                        {
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                float ai1 = (npc.ai[1] - delay) / max;

                                if (FargoSoulsWorld.MasochistModeReal)
                                { 
                                    float math = MathHelper.TwoPi / max * (npc.ai[1] - delay);
                                    Vector2 boltVel = -Vector2.UnitY.RotatedBy(-math);
                                    Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, 20f * boltVel, ProjectileID.HallowBossRainbowStreak, FargoSoulsUtil.ScaledProjectileDamage(baseDamage, 1.3f), 0f, Main.myPlayer, npc.target, ai1);
                                }

                                float spread = MathHelper.ToRadians(24);
                                float swordRotation = startRotation + MathHelper.Lerp(-spread, spread, ai1);
                                Vector2 appearVel = swordRotation.ToRotationVector2();
                                Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center + appearVel * 160f + appearVel * 5f * 60f, -appearVel * 5f, ProjectileID.FairyQueenLance, FargoSoulsUtil.ScaledProjectileDamage(baseDamage, 1.3f), 0f, Main.myPlayer, swordRotation, ai1);
                            }
                        }

                        if (npc.ai[1] > delay + max && !FargoSoulsWorld.MasochistModeReal)
                            npc.ai[1] -= 0.65f; //more delay before next attack
                    }
                    break;

                default:
                    break;
            }

            EModeUtils.DropSummon(npc, "PrismaticPrimrose", NPC.downedEmpressOfLight, ref DroppedSummon, Main.hardMode);

            return true;
        }

        public override void OnHitPlayer(NPC npc, Player target, int damage, bool crit)
        {
            base.OnHitPlayer(npc, target, damage, crit);

            target.AddBuff(ModContent.BuffType<Purified>(), 300);
            target.AddBuff(ModContent.BuffType<Smite>(), 1800);
        }

        public override bool StrikeNPC(NPC npc, ref double damage, int defense, ref float knockback, int hitDirection, ref bool crit)
        {
            if (npc.life < npc.lifeMax / 2)
                damage *= 0.5;

            return base.StrikeNPC(npc, ref damage, defense, ref knockback, hitDirection, ref crit);
        }

        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            base.ModifyNPCLoot(npc, npcLoot);

            LeadingConditionRule emodeRule = new LeadingConditionRule(new EModeDropCondition());
            emodeRule.OnSuccess(FargoSoulsUtil.BossBagDropCustom(ModContent.ItemType<PrecisionSeal>()));
            emodeRule.OnSuccess(FargoSoulsUtil.BossBagDropCustom(ItemID.HallowedFishingCrateHard, 5));
            npcLoot.Add(emodeRule);
        }

        public override void LoadSprites(NPC npc, bool recolor)
        {
            base.LoadSprites(npc, recolor);

            //LoadNPCSprite(recolor, npc.type);
            //LoadBossHeadSprite(recolor, 37);
            //LoadGoreRange(recolor, 1262, 1268);
            //extra_188?
        }
    }
}
