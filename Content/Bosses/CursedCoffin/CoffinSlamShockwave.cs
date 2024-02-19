using Microsoft.Xna.Framework;
using System;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using FargowiltasSouls.Content.Buffs.Masomode;

namespace FargowiltasSouls.Content.Bosses.CursedCoffin
{
	public class CoffinSlamShockwave : ModProjectile
    {
        public override string Texture => FargoSoulsUtil.EmptyTexture;
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Banished Baron Scrap");
        }
        public override void SetDefaults()
        {
            Projectile.width = 70;
            Projectile.height = 70;
            Projectile.aiStyle = -1;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.scale = 1f;
            Projectile.light = 1;
            Projectile.timeLeft = 60 * 3;
        }

        public override void AI()
        {
            if (Math.Abs(Projectile.velocity.X) < 15)
                Projectile.velocity.X *= 1.035f;

            int p = Player.FindClosest(Projectile.Center, 0, 0);
            if (p.IsWithinBounds(Main.maxPlayers) && Main.player[p] is Player player && player.Alive())
            {
                Projectile.light = Math.Abs(player.Center.X - Projectile.Center.X) < 300 ? 1 : 0;
            }

            int i = 0;
            const int maxIter = 12;
            while (i < maxIter)
            {
                i++;
                Point tilePos = Projectile.Center.ToTileCoordinates();
                Tile tile = Main.tile[tilePos.X, tilePos.Y];
                Tile tileBelow = Main.tile[tilePos.X, tilePos.Y + 1];
                if (tileBelow.HasUnactuatedTile && !tile.HasUnactuatedTile)
                    break;
                if (tile.HasUnactuatedTile)
                    Projectile.Center -= Vector2.UnitY * 16;
                else
                    Projectile.Center += Vector2.UnitY * 16;
            }
            if (i >= maxIter - 1)
                Projectile.Kill();
            for (int j = 0; j < 5; j++)
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Sand, Projectile.velocity.X / 2, Main.rand.NextFloat(-5, 5), Scale: Main.rand.NextFloat(1, 3));
            }
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(ModContent.BuffType<StunnedBuff>(), 60 * 2);
        }
    }
}
