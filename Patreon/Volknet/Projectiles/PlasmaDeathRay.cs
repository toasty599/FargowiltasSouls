using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.Enums;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace FargowiltasSouls.Patreon.Volknet.Projectiles
{
    public class PlasmaDeathRay : ModProjectile
    {
        public float LaserWidth = 20;
        public float LaserHeight = 30;
        public float LaserLen = 5000;
        public int LaserTime = 30;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Plasma Deathray");
            //DisplayName.AddTranslation(GameCulture.Chinese, "等离子致命光");
        }
        public override void SetDefaults()
        {
            Projectile.width = 2;
            Projectile.height = 2;
            Projectile.scale = 1.0f;
            Projectile.friendly = true;
            Projectile.timeLeft = LaserTime;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 0;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.hide = true;
            Projectile.light = 1.1f;
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            behindProjectiles.Add(index);
        }

        public override void AI()
        {
            CastLights();
            if (Projectile.ai[1] < LaserWidth / 2)       //20
            {
                Projectile.ai[1] += 1;
            }
            if (Projectile.timeLeft < LaserWidth)
            {
                Projectile.ai[1] = (float)Projectile.timeLeft / 2;
            }

            if (Main.player[Projectile.owner].active)
            {
                Player owner = Main.player[Projectile.owner];
                if (!owner.dead && owner.HeldItem.type == ModContent.ItemType<NanoCore>())
                {
                    if (owner.GetModPlayer<NanoPlayer>().NanoCoreMode == 3 && owner.channel)
                    {
                        Projectile.Center = owner.Center;
                        Projectile.rotation = (Main.MouseWorld - owner.Center).ToRotation();
                        for (int i = 0; i < 9; i++)
                        {
                            Dust dust = Dust.NewDustDirect(owner.Center, 0, 0, 157, default, default, default, default, 1.5f);
                            dust.position = owner.Center + Projectile.rotation.ToRotationVector2() * (50 + Main.rand.Next(80));
                            dust.velocity = (Main.rand.NextFloat() * MathHelper.TwoPi).ToRotationVector2() * Main.rand.NextFloat() * 3;
                            dust.fadeIn = 0.9f;
                            dust.noGravity = true;
                            dust.customData = owner;
                        }
                    }
                    else
                    {
                        Projectile.Kill();
                    }
                }
                else
                {
                    Projectile.Kill();
                }
            }
            else
            {
                Projectile.Kill();
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Rectangle Frame = new Rectangle((int)(LaserWidth / 2 - Projectile.ai[1]), 0, (int)(Projectile.ai[1] * 2), (int)LaserHeight);
            float maxDistance = LaserLen;

            float step = LaserHeight;


            Vector2 unit = Projectile.rotation.ToRotationVector2();
            float r = Projectile.rotation;
            for (float i = 0; i <= maxDistance; i += step)
            {
                Main.EntitySpriteDraw(Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value, new Vector2(0, Projectile.gfxOffY) + Projectile.Center + unit * (i + 45) - Main.screenPosition, Frame,
                    Color.White * 0.8f, r - MathHelper.Pi / 2, Frame.Size() * 0.5f, 1f, SpriteEffects.None, 0);
            }
            return false;
        }
        
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.WitheredArmor, 1800);
            target.AddBuff(BuffID.Frostburn, 1800);
            target.AddBuff(BuffID.CursedInferno, 1800);
            target.AddBuff(BuffID.Ichor, 1800);
            target.AddBuff(BuffID.ShadowFlame, 1800);
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            Vector2 unit = Projectile.rotation.ToRotationVector2();
            float point = 0f;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center,
                Projectile.Center + unit * LaserLen, LaserWidth, ref point);
        }
        public override bool? CanCutTiles()
        {
            return true;
        }
        public override void CutTiles()
        {
            DelegateMethods.tilecut_0 = TileCuttingContext.AttackProjectile;
            Vector2 unit = Projectile.rotation.ToRotationVector2();
            Terraria.Utils.PlotTileLine(Projectile.Center, Projectile.Center + unit * LaserLen, (LaserWidth + 16) * Projectile.scale, DelegateMethods.CutTiles);
        }

        private void CastLights()
        {
            DelegateMethods.v3_1 = new Vector3(0.8f, 0.8f, 1f);
            Terraria.Utils.PlotTileLine(Projectile.Center, Projectile.Center + Projectile.rotation.ToRotationVector2() * LaserLen, LaserWidth, DelegateMethods.CastLight);
        }

    }
}