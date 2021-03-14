using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using FargowiltasSouls.Buffs.Masomode;

namespace FargowiltasSouls.Projectiles.BossWeapons
{
    public class HentaiSpearDive : HentaiSpear
    {
        public override string Texture => "FargowiltasSouls/Projectiles/BossWeapons/HentaiSpear";
        
        public override void AI()
        {
            base.AI();
            projectile.localAI[0]++;
        }

        public override bool CanDamage()
        {
            return projectile.localAI[0] > 2;
        }
    }
}