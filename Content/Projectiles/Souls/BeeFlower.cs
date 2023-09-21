using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using FargowiltasSouls.Core.ModPlayers;
using Microsoft.CodeAnalysis;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace FargowiltasSouls.Content.Projectiles.Souls
{
    public class BeeFlower : ModProjectile
    {

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 5;
        }

        public override void SetDefaults()
        {
            Projectile.width = 34;
            Projectile.height = 43;
            Projectile.scale = 1f;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Generic;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 60 * 15;
            Projectile.penetrate = -1;
            Projectile.light = 1;
        }
        public override bool? CanDamage() => false;
        public override void AI()
        {
            if (Projectile.frame < Main.projFrames[Projectile.type] - 1) //petalinate
            {
                if (++Projectile.frameCounter % 60 == 0)
                {
                    Projectile.frame++;
                }
            }
            else
            {
                if (Main.LocalPlayer.active && !Main.LocalPlayer.dead && !Main.LocalPlayer.ghost && Main.LocalPlayer.Hitbox.Intersects(Projectile.Hitbox))
                {
                    Main.LocalPlayer.AddBuff(BuffID.Honey, 60 * 15);
                    int damage = Main.LocalPlayer.GetModPlayer<FargoSoulsPlayer>().ForceEffect(ModContent.ItemType<BeeEnchant>()) ? 60 : 12;
                    float kb = 0.5f;
                    for (int i = 0; i < 7; i++)
                    {
                        Vector2 pos = Main.rand.NextVector2FromRectangle(Projectile.Hitbox);
                        int p = Projectile.NewProjectile(Projectile.GetSource_FromThis(), pos, (pos - Projectile.Center) / 12,
                            Main.LocalPlayer.beeType(), Main.LocalPlayer.beeDamage(damage), Main.LocalPlayer.beeKB(kb), Main.LocalPlayer.whoAmI);
                        if (p != Main.maxProjectiles)
                            Main.projectile[p].DamageType = Projectile.DamageType;
                    }
                    Projectile.Kill();
                }
            }

        }

        public override void Kill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.LiquidsHoneyWater, Projectile.Center);
        }
    }
}