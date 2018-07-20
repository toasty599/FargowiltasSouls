using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles
{
    public class FargoGlobalProjectile : GlobalProjectile
    {
        public override bool InstancePerEntity
        {
            get
            {
                return true;
            }
        }

        private int _timePass;
        private int _numSplits = 1;
        private bool _instantSplit;
        private int _numSpeedups = 3;
        public bool NinjaTele;
        public bool IsRecolor = false;

        public override void SetDefaults(Projectile projectile)
        {
            if (FargoWorld.MasochistMode)
            {
                if (projectile.type == ProjectileID.SaucerLaser)
                {
                    projectile.tileCollide = false;
                }

                if (projectile.type == ProjectileID.FallingStar)
                {
                    projectile.hostile = true;
                }


            }

        }

        public override bool PreAI(Projectile projectile)
        {
            bool retVal = true;
            FargoPlayer modPlayer = Main.LocalPlayer.GetModPlayer<FargoPlayer>();

            if (modPlayer.Jammed && projectile.ranged && projectile.type != ProjectileID.ConfettiGun)
            {
                Projectile.NewProjectile(projectile.Center, projectile.velocity, ProjectileID.ConfettiGun, 0, 0f);
                projectile.damage = 0;
                projectile.position = new Vector2(Main.maxTilesX);
                projectile.Kill();
            }

            if (modPlayer.Atrophied && projectile.thrown)
            {
                projectile.damage = 0;
                projectile.position = new Vector2(Main.maxTilesX);
                projectile.Kill();
            }

            if (projectile.owner == Main.myPlayer)
            {

                if (modPlayer.AdamantiteEnchant && !projectile.minion && projectile.damage > 0 && Main.rand.Next(4) == 0 /*&& !/*projectile.spear*/ && !_instantSplit)
                {
                    _instantSplit = true;
                    SplitProj(projectile, 3);
                    retVal = false;
                }

                if (projectile.bobber && !_instantSplit)
                {
                    _instantSplit = true;

                    if (modPlayer.FishSoul1)
                    {
                        SplitProj(projectile, 5);
                    }
                    if (modPlayer.FishSoul2)
                    {
                        SplitProj(projectile, 11);
                    }
                }

                if (projectile.thrown)
                {
                    _timePass++;

                    if (modPlayer.GladEnchant && _numSpeedups > 0 && _timePass % 10 == 0)
                    {
                        _numSpeedups--;
                        projectile.velocity = Vector2.Multiply(projectile.velocity, 2);
                    }

                    if (modPlayer.ThrowSoul && _numSplits > 0 && _timePass == 20 * (1 + projectile.extraUpdates))
                    {
                        _numSplits--;
                        SplitProj(projectile, 3);
                        retVal = false;
                    }
                }

                _instantSplit = true;
            }

            return retVal;
        }

        public void SplitProj(Projectile projectile, int number)
        {
            //if its odd, we just keep the original 
            if (number % 2 != 0)
            {
                number--;
            }

            Projectile split;

            double spread = 0.6 / number;

            for (int i = 0; i < number / 2; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    int factor;

                    if (j == 0)
                    {
                        factor = 1;
                    }
                    else
                    {
                        factor = -1;
                    }

                    split = Projectile.NewProjectileDirect(projectile.Center, projectile.velocity.RotatedBy(factor * spread * (i + 1)), projectile.type, projectile.damage, projectile.knockBack, projectile.owner, projectile.ai[0], projectile.ai[1]);
                    split.GetGlobalProjectile<FargoGlobalProjectile>()._numSplits = _numSplits;
                    split.GetGlobalProjectile<FargoGlobalProjectile>()._instantSplit = _instantSplit;
                }

            }
        }

        public override void AI(Projectile projectile)
        {
            Player player = Main.player[projectile.owner];
            FargoPlayer modPlayer = player.GetModPlayer<FargoPlayer>(mod);

            #region pets

            if (projectile.type == 623)
            {
                if (!modPlayer.StardustEnchant && player.FindBuffIndex(187) == -1)
                {
                    projectile.Kill();
                    return;
                }
            }

            if (projectile.type == mod.ProjectileType("HallowProj"))
            {
                if (!modPlayer.HallowEnchant)
                {
                    projectile.Kill();
                    return;
                }
            }



            if (projectile.type == ProjectileID.DD2PetDragon && player.FindBuffIndex(202) == -1)
            {
                if (!modPlayer.DragPet)
                {
                    projectile.Kill();
                    return;
                }
            }

            if (projectile.type == ProjectileID.BabyDino && player.FindBuffIndex(61) == -1)
            {
                if (!modPlayer.DinoPet)
                {
                    projectile.Kill();
                    return;
                }
            }

            if (projectile.type == ProjectileID.Penguin && player.FindBuffIndex(41) == -1)
            {
                if (!modPlayer.PenguinPet)
                {
                    projectile.Kill();
                    return;
                }
            }

            if (projectile.type == ProjectileID.BabySkeletronHead && player.FindBuffIndex(50) == -1)
            {
                if (!modPlayer.SkullPet)
                {
                    projectile.Kill();
                    return;
                }
            }

            /*if(projectile.type == ProjectileID.DD2PetGato && (player.FindBuffIndex(200) == -1))
			{
				if (!modPlayer.mythrilPet)
				{
					projectile.Kill();
					return;
				}
			}
			
			if(projectile.type == ProjectileID.Parrot && (player.FindBuffIndex(54) == -1))
			{
				if (!modPlayer.oriPet)
				{
					projectile.Kill();
					return;
				}
			}*/

            if (projectile.type == ProjectileID.Puppy && player.FindBuffIndex(91) == -1)
            {
                if (!modPlayer.DogPet)
                {
                    projectile.Kill();
                    return;
                }
            }

            

            if (projectile.type == ProjectileID.PetLizard && player.FindBuffIndex(53) == -1)
            {
                if (!modPlayer.LizPet)
                {
                    projectile.Kill();
                    return;
                }
            }

            if (projectile.type == ProjectileID.BlackCat && player.FindBuffIndex(84) == -1)
            {
                if (!modPlayer.CatPet)
                {
                    projectile.Kill();
                    return;
                }
            }

            if (projectile.type == ProjectileID.MiniMinotaur && player.FindBuffIndex(136) == -1)
            {
                if (!modPlayer.MinotaurPet)
                {
                    projectile.Kill();
                    return;
                }
            }

            /*if(projectile.type == ProjectileID.CursedSapling && (player.FindBuffIndex(85) == -1))
			{
				if (!modPlayer.saplingPet)
				{
					projectile.Kill();
					return;
				}
			}*/

            if (projectile.type == ProjectileID.Squashling && player.FindBuffIndex(82) == -1)
            {
                if (!modPlayer.PumpkinPet)
                {
                    projectile.Kill();
                    return;
                }
            }

            if (projectile.type == ProjectileID.BabyEater && player.FindBuffIndex(45) == -1)
            {
                if (!modPlayer.ShadowPet)
                {
                    projectile.Kill();
                    return;
                }
            }

            if (projectile.type == ProjectileID.Wisp && player.FindBuffIndex(57) == -1)
            {
                if (!modPlayer.SpectrePet)
                {
                    projectile.Kill();
                    return;
                }
            }

            if (projectile.type == ProjectileID.Turtle && player.FindBuffIndex(42) == -1)
            {
                if (!modPlayer.TurtlePet)
                {
                    projectile.Kill();
                    return;
                }
            }

            if (projectile.type == ProjectileID.BabySnowman && player.FindBuffIndex(66) == -1)
            {
                if (!modPlayer.SnowmanPet)
                {
                    projectile.Kill();
                    return;
                }
            }

            if (projectile.type == ProjectileID.ZephyrFish && player.FindBuffIndex(127) == -1)
            {
                if (!modPlayer.FishPet)
                {
                    projectile.Kill();
                    return;
                }
            }

            if (projectile.type == ProjectileID.CompanionCube && player.FindBuffIndex(191) == -1)
            {
                if (!modPlayer.CubePet)
                {
                    projectile.Kill();
                    return;
                }
            }

            if (projectile.type == ProjectileID.BabyGrinch && player.FindBuffIndex(92) == -1)
            {
                if (!modPlayer.GrinchPet)
                {
                    projectile.Kill();
                    return;
                }
            }

            if (projectile.type == ProjectileID.SuspiciousTentacle && player.FindBuffIndex(190) == -1)
            {
                if (!modPlayer.SuspiciousEyePet)
                {
                    projectile.Kill();
                    return;
                }
            }

            if (projectile.type == ProjectileID.Spider && player.FindBuffIndex(81) == -1)
            {
                if (!modPlayer.SpiderPet)
                {
                    projectile.Kill();
                    return;
                }
            }


            if (projectile.type == ProjectileID.BabyHornet && player.FindBuffIndex(BuffID.BabyHornet) == -1)
            {
                if (!modPlayer.BeeEnchant || !Soulcheck.GetValue("Baby Hornet Pet"))
                {
                    projectile.Kill();
                    return;
                }
            }

            if (projectile.type == mod.ProjectileType("Chlorofuck"))
            {
                if (!modPlayer.ChloroEnchant || !Soulcheck.GetValue("Leaf Crystal"))
                {
                    projectile.Kill();
                    return;
                }
            }

            if (projectile.type == ProjectileID.Sapling && player.FindBuffIndex(BuffID.PetSapling) == -1)
            {
                if (!modPlayer.ChloroEnchant || !Soulcheck.GetValue("Seedling Pet"))
                {
                    projectile.Kill();
                    return;
                }
            }

            if (projectile.type == ProjectileID.BabyFaceMonster && player.FindBuffIndex(BuffID.BabyFaceMonster) == -1)
            {
                if (!modPlayer.CrimsonEnchant || !Soulcheck.GetValue("Baby Face Monster Pet"))
                {
                    projectile.Kill();
                    return;
                }
            }

            if (projectile.type == ProjectileID.CrimsonHeart && player.FindBuffIndex(BuffID.CrimsonHeart) == -1)
            {
                if (!modPlayer.CrimsonEnchant || !Soulcheck.GetValue("Crimson Heart Pet"))
                {
                    projectile.Kill();
                    return;
                }
            }



            if (projectile.type == ProjectileID.MagicLantern && player.FindBuffIndex(152) == -1)
            {
                if (!modPlayer.LanternPet)
                {
                    projectile.Kill();
                    return;
                }
            }

            if (projectile.type == ProjectileID.ShadowOrb && player.FindBuffIndex(19) == -1)
            {
                if (!modPlayer.ShadowPet2)
                {
                    projectile.Kill();
                }
            }

            

            #endregion

        }

        public override Color? GetAlpha(Projectile projectile, Color lightColor)
        {
            if (IsRecolor)
            {
                if (projectile.type == ProjectileID.HarpyFeather)
                {
                    return Color.Brown;
                }

                if (projectile.type == ProjectileID.SpikyBall)
                {
                    return Color.LimeGreen;
                }

                if (projectile.type == ProjectileID.PineNeedleFriendly)
                {
                    return Color.GreenYellow;
                }
            }

            return null;
        }

        public override bool CanHitPlayer(Projectile projectile, Player target)
        {
            FargoPlayer modPlayer = Main.player[projectile.owner].GetModPlayer<FargoPlayer>();

            //when standing still
            if (modPlayer.TurtleEnchant && Math.Abs(target.velocity.X) < 0.05 && Math.Abs(target.velocity.Y) < 0.05 && target.itemAnimation == 0 && Main.rand.Next(3) == 0)
            {
                return false;
            }
            return true;
        }

        public override void OnHitNPC(Projectile projectile, NPC target, int damage, float knockback, bool crit)
        {
            Player player = Main.player[Main.myPlayer];
            FargoPlayer modPlayer = player.GetModPlayer<FargoPlayer>(mod);

            if (modPlayer.SqueakyToy)
            {
                return;
            }

            //spawn proj on hit
            if (modPlayer.ShroomEnchant && modPlayer.IsStandingStill && (projectile.magic || projectile.thrown || projectile.melee || projectile.minion || projectile.ranged) && Main.rand.Next(5) == 0)
            {
                int shrooms = Projectile.NewProjectile(projectile.Center.X + Main.rand.Next(-40, 40), projectile.Center.Y + Main.rand.Next(-40, 40), 0f, 0f, 590, 32/*dmg*/, 0f, projectile.owner);
                Main.projectile[shrooms].melee = false;
            }

            if (modPlayer.OriEnchant && projectile.magic && Main.rand.Next(6) == 0)
            {
                int[] ball = { 15, 95, 253 };
                int fireball = Projectile.NewProjectile(projectile.Center.X + Main.rand.Next(-40, 40), projectile.Center.Y + Main.rand.Next(-40, 40), 0f + Main.rand.Next(-5, 5), -5f, ball[Main.rand.Next(3)], 32/*dmg*/, 0f, projectile.owner);
                Main.projectile[fireball].melee = false;
            }

            if (projectile.minion && modPlayer.UniverseEffect)
            {
                target.AddBuff(BuffID.Ichor, 240, true);
                target.AddBuff(BuffID.CursedInferno, 240, true);
                target.AddBuff(BuffID.Confused, 120, true);
                target.AddBuff(BuffID.Venom, 240, true);
                target.AddBuff(BuffID.ShadowFlame, 240, true);
                target.AddBuff(BuffID.OnFire, 240, true);
                target.AddBuff(BuffID.Frostburn, 240, true);
                target.AddBuff(BuffID.Daybreak, 240, true);
            }

            //coin portals
            if (modPlayer.VoidSoul && target.type != NPCID.TargetDummy && Main.rand.Next(100) == 0)
            {
                Projectile.NewProjectile(target.Center.X, target.Center.Y, 0f - 10, 0f, 518, 0, 0f, projectile.owner);
            }
        }

        public override bool OnTileCollide(Projectile projectile, Vector2 oldVelocity)
        {
            Player player = Main.player[Main.myPlayer];
            FargoPlayer modPlayer = player.GetModPlayer<FargoPlayer>(mod);

            if (modPlayer.NinjaEnchant && projectile.type == ProjectileID.SmokeBomb && !NinjaTele)
            {
                NinjaTele = true;

                var teleportPos = new Vector2();

                teleportPos.X = projectile.position.X;
                teleportPos.Y = projectile.position.Y;

                //spiral out to find a save spot
                int count = 0;
                int increase = 10;
                while (Collision.SolidCollision(teleportPos, player.width, player.height))
                {
                    switch (count)
                    {
                        case 0:
                            teleportPos.X -= increase;
                            break;
                        case 1:
                            teleportPos.X += increase * 2;
                            break;
                        case 2:
                            teleportPos.Y += increase;
                            break;
                        default:
                            teleportPos.Y -= increase * 2;
                            increase += 10;
                            break;
                    }
                    count++;

                    if (count >= 4)
                    {
                        count = 0;
                    }

                }

                if (teleportPos.X > 50 && teleportPos.X < (double)(Main.maxTilesX * 16 - 50) && teleportPos.Y > 50 && teleportPos.Y < (double)(Main.maxTilesY * 16 - 50))
                {
                    player.Teleport(teleportPos, 1);
                    NetMessage.SendData(65, -1, -1, null, 0, player.whoAmI, teleportPos.X, teleportPos.Y, 1);
                }
            }

            return true;
        }

        public override void ModifyHitPlayer(Projectile projectile, Player target, ref int damage, ref bool crit)
        {
            Player player = Main.player[Main.myPlayer];
            FargoPlayer modPlayer = player.GetModPlayer<FargoPlayer>(mod);

            //Main.NewText(projectile.type.ToString(), 206, 12, 15);

            if (FargoWorld.MasochistMode)
            {
                if (projectile.type == ProjectileID.JavelinHostile)
                {
                    target.AddBuff(mod.BuffType("Defenseless"), 600);
                    target.AddBuff(mod.BuffType("Stunned"), 90);
                }

                if (projectile.type == ProjectileID.DemonSickle)
                {
                    target.AddBuff(BuffID.Darkness, 1800);
                }

                if (projectile.type == ProjectileID.HarpyFeather && Main.rand.Next(2) == 0)
                {
                    target.AddBuff(mod.BuffType("ClippedWings"), 480);
                }

                //so only antlion sand and not falling sand 
                if (projectile.type == ProjectileID.SandBallFalling && projectile.velocity.X != 0)
                {
                    target.AddBuff(mod.BuffType("Stunned"), 120);
                }

                if (projectile.type == ProjectileID.Stinger && NPC.AnyNPCs(NPCID.QueenBee))
                {
                    target.AddBuff(BuffID.Venom, 900);
                    target.AddBuff(BuffID.BrokenArmor, 1200);
                }

                if (projectile.type == ProjectileID.Skull && Main.rand.Next(10) == 0)
                {
                    target.AddBuff(BuffID.Cursed, 360);
                }

                if (projectile.type == ProjectileID.EyeLaser && NPC.AnyNPCs(NPCID.WallofFlesh))
                {
                    target.AddBuff(BuffID.OnFire, 600);
                }

                if (projectile.type == ProjectileID.DeathSickle && !player.HasBuff(mod.BuffType("MarkedforDeath")))
                {
                    target.AddBuff(mod.BuffType("MarkedforDeath"), 900);
                    target.AddBuff(mod.BuffType("LivingWasteland"), 1800);
                }

                if (projectile.type == ProjectileID.DrManFlyFlask)
                {
                    int[] buffs = { BuffID.Venom, BuffID.Confused, BuffID.CursedInferno, BuffID.OgreSpit, mod.BuffType("LivingWasteland"), mod.BuffType("Defenseless"), mod.BuffType("Purified") };

                    target.AddBuff(buffs[Main.rand.Next(buffs.Length)], 600);
                    target.AddBuff(BuffID.Stinky, 1200);
                }

                //CULTIST OP
                if (projectile.type == ProjectileID.CultistBossLightningOrb)
                {
                    target.AddBuff(mod.BuffType("LightningRod"), 600);
                }
                if (projectile.type == ProjectileID.CultistBossLightningOrbArc)
                {
                    target.AddBuff(BuffID.Electrified, 300);
                }
                if (projectile.type == ProjectileID.CultistBossIceMist)
                {
                    target.AddBuff(BuffID.Frozen, 300);
                }
                if (projectile.type == ProjectileID.CultistBossFireBall)
                {
                    target.AddBuff(mod.BuffType("Berserked"), 300);
                    target.AddBuff(BuffID.BrokenArmor, 900);
                    target.AddBuff(BuffID.OnFire, 600);
                }
                if (projectile.type == ProjectileID.CultistBossFireBallClone)
                {
                    target.AddBuff(BuffID.ShadowFlame, 600);
                }

                if (projectile.type == ProjectileID.PaladinsHammerHostile)
                {
                    target.AddBuff(mod.BuffType("Lethargic"), 600);
                }

                if (projectile.type == ProjectileID.RuneBlast)
                {
                    target.AddBuff(mod.BuffType("FlamesoftheUniverse"), 300);
                }

                if (projectile.type == ProjectileID.ThornBall || projectile.type == ProjectileID.PoisonSeedPlantera)
                {
                    target.AddBuff(mod.BuffType("Infested"), 600);
                }

                if (projectile.type == ProjectileID.DesertDjinnCurse && target.ZoneCorrupt)
                {
                    target.AddBuff(BuffID.ShadowFlame, 900);
                }

                if (projectile.type == ProjectileID.DesertDjinnCurse && target.ZoneCrimson)
                {
                    target.AddBuff(BuffID.Ichor, 1800);
                }

                if (projectile.type == ProjectileID.PhantasmalDeathray)
                {
                    target.AddBuff(mod.BuffType("FlamesoftheUniverse"), 300);
                }

            }

            int chance = 0;
            if (modPlayer.HallowEnchant)
            {
                chance = 8;
            }
            else if (modPlayer.TerrariaSoul)
            {
                chance = 2;
            }

            //reflect proj
            if (chance != 0 && projectile.active && !projectile.friendly && projectile.hostile && damage > 0 && Main.rand.Next(chance) == 0)
            {
                target.statLife += damage;

                //Projectile.NewProjectile(player.Center.X, player.Center.Y, -projectile.velocity.X, -projectile.velocity.Y, mod.ProjectileType("HallowSword"), damage, 2f, Main.myPlayer, 0f, 0f);

                // Set ownership
                projectile.hostile = false;
                projectile.friendly = true;
                projectile.owner = player.whoAmI;

                // Turn around
                projectile.velocity *= -1f;
                projectile.penetrate = 1;

                // Flip sprite
                if (projectile.Center.X > player.Center.X * 0.5f)
                {
                    projectile.direction = 1;
                    projectile.spriteDirection = 1;
                }
                else
                {
                    projectile.direction = -1;
                    projectile.spriteDirection = -1;
                }

                // Don't know if this will help but here it is
                projectile.netUpdate = true;

            }
        }

        public override void ModifyHitNPC(Projectile projectile, NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            Player player = Main.player[Main.myPlayer];
            FargoPlayer modPlayer = player.GetModPlayer<FargoPlayer>(mod);

            if (modPlayer.SqueakyToy)
            {
            }
        }

        public override void Kill(Projectile projectile, int timeLeft)
        {
            Player player = Main.player[projectile.owner];
            FargoPlayer modPlayer = player.GetModPlayer<FargoPlayer>(mod);

            if (modPlayer.CobaltEnchant && projectile.type != ProjectileID.CrystalShard && projectile.friendly && projectile.damage > 0 && Main.rand.Next(2) == 0)
            {
                Main.PlaySound(2, (int)player.position.X, (int)player.position.Y, 27);
                XWay(8, projectile.Center, ProjectileID.CrystalShard, 50, 2f);
            }
        }

        static float[] _x = { 0, 5, 0, -5, 5, -5, 5, -5, 2.5f, 5, -5, 2.5f, 5, -2.5f, -5, -2.5f };
        static float[] _y = { 5, 0, -5, 0, 5, -5, -5, 5, 5, 2.5f, 2.5f, -5, -2.5f, 5, -2.5f, -5 };

        public static Projectile[] XWay(int num, Vector2 pos, int type, int damage, float knockback)
        {
            Projectile[] projs = new Projectile[16];

            for (int i = 0; i < num; i++)
            {
                Projectile p = Projectile.NewProjectileDirect(pos, new Vector2(_x[i], _y[i]), type, damage, knockback, Main.myPlayer);
                projs[i] = p;
            }

            return projs;
        }
    }
}