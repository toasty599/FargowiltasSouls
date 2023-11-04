using FargowiltasSouls.Core.ModPlayers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.Souls
{
    internal class PearlwoodStareline : ModProjectile
    {

        public override void SetStaticDefaults()
        {

        }
        public override void SetDefaults()
        {
            Projectile.width = 22;
            Projectile.height = 22;
            Projectile.aiStyle = -1;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 22;
        }
        
        public override string Texture => "Terraria/Images/Projectile_723";

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            FargoSoulsPlayer modPlayer = player.FargoSouls();

            if (modPlayer.PearlwoodEnchantItem == null || !player.GetToggleValue("Pearl") || player == null || !player.active || player.dead) //just making sure
            {
                Projectile.Kill();
                return;
            } //kill projkcetoiele when unequip or toggled off or player dies or leaves or commits a war crime idfk

            if (modPlayer.ForceEffect(modPlayer.PearlwoodEnchantItem.type)) { Projectile.friendly = true; } //ability to hit enemy with force
            else { Projectile.friendly = false; }

            //refresh lifetime
            Projectile.timeLeft = 22;

            //spin
            Projectile.rotation = MathHelper.ToRadians(MathHelper.ToDegrees(Projectile.rotation) + 6f);

            //sparkle on queen
            Dust.NewDust(modPlayer.PStarelinePos, Projectile.width, Projectile.height, DustID.TintableDustLighted, default, default, 200, Color.Fuchsia); //idk how to make dust look good


            //follow the player
            Projectile.position = modPlayer.PStarelinePos;
        }

        public override void OnSpawn(IEntitySource source)
        {
            SoundEngine.PlaySound(SoundID.Item84, Projectile.position);
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo info) //womp womp
        {
            SoundEngine.PlaySound(SoundID.Item25, target.position);
            Projectile.Kill();
        }

        public override void ModifyHitPlayer(Player target, ref Player.HurtModifiers modifiers)
        {
            modifiers.SetMaxDamage((int)(target.statLifeMax2 * 0.2)); //hits player for 1/5th max hp damage
            modifiers.ScalingArmorPenetration += 1;
        }



        public override void OnKill(int timeLeft)
        {
            Player player = Main.player[Projectile.owner];
            FargoSoulsPlayer modPlayer = player.FargoSouls();

            for (int i = 0; i < 20; i++) //idk how to make dust look good (2)
            {
                Dust.NewDust(modPlayer.PStarelinePos, 22, 22, DustID.GoldFlame, 0f, 0f, 175, default, 1.75f);
            }
            
            modPlayer.PStarelineActive = false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Main.EntitySpriteDraw(TextureAssets.Projectile[Type].Value, Projectile.Center - Main.screenPosition, null, Color.MediumVioletRed, Projectile.rotation, TextureAssets.Projectile[Type].Size() / 2, Projectile.scale, SpriteEffects.None);
            return false;
        }

    }
}
