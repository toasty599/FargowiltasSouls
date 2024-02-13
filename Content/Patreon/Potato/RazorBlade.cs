using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Patreon.Potato
{
    public class RazorBlade : ModProjectile
    {

        public override void SetDefaults()
        {
            Projectile.width = Player.defaultHeight / 2;
            Projectile.height = Player.defaultHeight / 2;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 20; //
            Projectile.penetrate = -1;
            Projectile.FargoSouls().CanSplit = false;
        }

        int MaxDistance = 200;

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            if (player.dead || !player.GetModPlayer<PatreonPlayer>().RazorContainer)
            {
                Projectile.Kill();
            }

            Projectile.timeLeft++;

            //MaxDistance = 200;

            int distance = (int)Vector2.Distance(Projectile.Center, player.Center);

            //Main.NewText(distance + " "  + MaxDistance);

            if (distance > MaxDistance)
            {
                Vector2 velocity = Vector2.Normalize(player.Center - Projectile.Center) * 5;// * (2 + player.velocity.Length());

                Projectile.velocity = velocity;
            }



        }

        

        public virtual Asset<Texture2D> ChainTexture => ModContent.Request<Texture2D>("FargowiltasSouls/Content/Patreon/Potato/RazorChain");

        public override bool PreDraw(ref Color lightColor)
        {
            //dont draw when right on the player
            Player player = Main.player[Projectile.owner];

            if (Vector2.Distance(player.Center, Projectile.Center) < 5)
            {
                return true;
            }



            Vector2 position = Projectile.Center;
            Vector2 mountedCenter = Main.player[Projectile.owner].MountedCenter;
            Vector2 origin = new Vector2(ChainTexture.Width() * 0.5f, ChainTexture.Height() * 0.5f);
            float textureHeight = ChainTexture.Height();
            Vector2 mountedPosition = mountedCenter - position;
            float rotation = (float)Math.Atan2(mountedPosition.Y, mountedPosition.X) - 1.57f;
            bool invalidPosition = !(float.IsNaN(position.X) && float.IsNaN(position.Y) ||
                                     float.IsNaN(mountedPosition.X) && float.IsNaN(mountedPosition.Y));

            while (invalidPosition)
            {
                if (mountedPosition.Length() < textureHeight + 1.0)
                    invalidPosition = false;
                else
                {
                    Vector2 mountedClone = mountedPosition;
                    mountedClone.Normalize();
                    position += mountedClone * textureHeight;
                    mountedPosition = mountedCenter - position;
                    Color color2 = Lighting.GetColor((int)position.X / 16, (int)(position.Y / 16.0));

                    Main.spriteBatch.Draw(((Texture2D)ChainTexture), position - Main.screenPosition, null, color2, rotation, origin,
                        1f, SpriteEffects.None, 0.0f);
                }
            }

            return true;
        }

    }
}
