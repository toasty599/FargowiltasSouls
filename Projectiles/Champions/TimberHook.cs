using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Champions
{
    public class TimberHook : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_13";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Squirrel Hook");
            ProjectileID.Sets.DrawScreenCheckFluff[Projectile.type] = 2400;
        }

        public override void SetDefaults()
        {
            Projectile.width = 18;
            Projectile.height = 18;
            Projectile.aiStyle = -1;
            Projectile.timeLeft = 240;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;

            Projectile.GetGlobalProjectile<FargoSoulsGlobalProjectile>().DeletionImmuneRank = 2;
        }

        public override void AI()
        {
            NPC npc = FargoSoulsUtil.NPCExists(Projectile.ai[0], ModContent.NPCType<NPCs.Champions.TimberChampion>());
            if (npc == null)
            {
                Projectile.Kill();
                return;
            }
            Player player = Main.player[npc.target];

            const float speed = 24f;

            if (!npc.HasValidTarget)
            {
                Projectile.velocity = Projectile.DirectionTo(npc.Center) * speed;
                Projectile.ai[1] = 0;
                return;
            }

            if (Projectile.ai[1] == 0)
            {
                Projectile.velocity = Projectile.DirectionTo(player.Center) * speed + player.velocity / 4f;
                Projectile.rotation = Projectile.velocity.ToRotation() + (float)Math.PI / 2;

                if (Projectile.Distance(player.Center) < speed) //in range
                {
                    Projectile.ai[1] = 1;
                    Projectile.netUpdate = true;
                }
            }
            else
            {
                Projectile.rotation = npc.DirectionTo(player.Center).ToRotation() + (float)Math.PI / 2;

                if (Projectile.Distance(player.Center) > 64) //out of range somehow
                {
                    Projectile.ai[1] = 0;
                    Projectile.netUpdate = true;
                }
                else
                {
                    float dragSpeed = Projectile.Distance(npc.Center) / 50;
                    
                    player.position += Projectile.DirectionTo(npc.Center) * dragSpeed;
                    Projectile.Center = player.Center - player.velocity;

                    if (Projectile.timeLeft == 1)
                        player.velocity /= 3;
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            NPC npc = FargoSoulsUtil.NPCExists(Projectile.ai[0], ModContent.NPCType<NPCs.Champions.TimberChampion>());
            if (npc != null && TextureAssets.Chain.IsLoaded)
            {
                Texture2D texture = TextureAssets.Chain.Value;
                Vector2 position = Projectile.Center;
                Vector2 mountedCenter = Main.npc[(int)Projectile.ai[0]].Center;
                Rectangle? sourceRectangle = new Rectangle?();
                Vector2 origin = new Vector2(texture.Width * 0.5f, texture.Height * 0.5f);
                float num1 = texture.Height;
                Vector2 vector24 = mountedCenter - position;
                float rotation = (float)Math.Atan2(vector24.Y, vector24.X) - 1.57f;
                bool flag = true;
                if (float.IsNaN(position.X) && float.IsNaN(position.Y))
                    flag = false;
                if (float.IsNaN(vector24.X) && float.IsNaN(vector24.Y))
                    flag = false;
                while (flag)
                    if (vector24.Length() < num1 + 1.0)
                    {
                        flag = false;
                    }
                    else
                    {
                        Vector2 vector21 = vector24;
                        vector21.Normalize();
                        position += vector21 * num1;
                        vector24 = mountedCenter - position;
                        Color color2 = Lighting.GetColor((int)position.X / 16, (int)(position.Y / 16.0));
                        color2 = Projectile.GetAlpha(color2);
                        Main.EntitySpriteDraw(texture, position - Main.screenPosition, sourceRectangle, color2, rotation, origin, 1f, SpriteEffects.None, 0);
                    }
            }

            Texture2D texture2D13 = TextureAssets.Projectile[Projectile.type].Value;
            int num156 = TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new Rectangle(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;
            SpriteEffects effects = SpriteEffects.None;
            Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Projectile.GetAlpha(lightColor), Projectile.rotation, origin2, Projectile.scale, effects, 0);
            return false;
        }
    }
}