using FargowiltasSouls.Content.Buffs.Masomode;
using FargowiltasSouls.Core.Globals;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Bosses.DeviBoss
{
    public class DeviGuardianHarmless : DeviGuardian
    {

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.Opacity = 0.5f;
        }
        public override bool? CanDamage() => false;
        public override void PostAI()
        {
            base.PostAI();
            Projectile.Opacity -= 0.5f / 60;
        }
    }
}