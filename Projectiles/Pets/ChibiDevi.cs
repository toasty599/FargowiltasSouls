using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Pets
{
    public class ChibiDevi : ModProjectile
    {
        private Vector2 oldMouse;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Chibi Devi");
            Main.projFrames[Projectile.type] = 6;
            ProjectileID.Sets.LightPet[Projectile.type] = true;
            Main.projPet[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 22;
            Projectile.height = 44;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.aiStyle = -1;
            Projectile.netImportant = true;
            Projectile.friendly = true;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(Projectile.rotation);
            writer.Write(Projectile.direction);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Projectile.rotation = reader.ReadSingle();
            Projectile.direction = reader.ReadInt32();
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            FargoSoulsPlayer modPlayer = player.GetModPlayer<FargoSoulsPlayer>();
            if (player.dead)
            {
                modPlayer.ChibiDevi = false;
            }
            if (modPlayer.ChibiDevi)
            {
                Projectile.timeLeft = 2;
            }

            DelegateMethods.v3_1 = new Vector3(1f, 0.5f, 0.9f) * 0.75f;
            Utils.PlotTileLine(Projectile.Center, Projectile.Center + Projectile.velocity * 6f, 20f, DelegateMethods.CastLightOpen);
            Utils.PlotTileLine(Projectile.Left, Projectile.Right, 20f, DelegateMethods.CastLightOpen);

            if (Projectile.ai[0] == 1)
            {
                Projectile.tileCollide = true;
                Projectile.ignoreWater = false;

                Projectile.frameCounter = 0;
                Projectile.frame = Projectile.velocity.Y == 0 ? 5 : 4;

                Projectile.velocity.X *= 0.95f;
                Projectile.velocity.Y += 0.3f;

                if (Projectile.owner == Main.myPlayer && Projectile.Distance(Main.MouseWorld) > 180)
                {
                    Projectile.ai[0] = 0;
                }
            }
            else
            {
                if (Projectile.owner == Main.myPlayer)
                {
                    Projectile.tileCollide = false;
                    Projectile.ignoreWater = true;

                    Projectile.direction = Projectile.Center.X < Main.MouseWorld.X ? 1 : -1;

                    float distance = 2500;
                    float possibleDist = Main.player[Projectile.owner].Distance(Main.MouseWorld) / 2 + 100;
                    if (distance < possibleDist)
                        distance = possibleDist;
                    if (Projectile.Distance(Main.player[Projectile.owner].Center) > distance && Projectile.Distance(Main.MouseWorld) > distance)
                    {
                        Projectile.Center = player.Center;
                        Projectile.velocity = Vector2.Zero;
                    }

                    if (Projectile.Distance(Main.MouseWorld) > 30)
                        Movement(Main.MouseWorld, 0.15f, 32f);

                    if (oldMouse == Main.MouseWorld)
                    {
                        Projectile.ai[1]++;
                        if (Projectile.ai[1] > 600)
                        {
                            bool okToRest = !Collision.SolidCollision(Projectile.position, Projectile.width, Projectile.height);

                            if (okToRest)
                            {
                                okToRest = false;

                                Vector2 targetPos = new Vector2(Projectile.Center.X, Projectile.position.Y + Projectile.height);
                                for (int i = 0; i < 10; i++) //collision check below self
                                {
                                    targetPos.Y += 16;
                                    Tile tile = Framing.GetTileSafely(targetPos); //if solid, ok
                                    if (tile.HasUnactuatedTile && Main.tileSolid[tile.TileType])
                                    {
                                        okToRest = true;
                                        break;
                                    }
                                }
                            }

                            if (okToRest) //not in solid tiles, but found tiles within a short distance below
                            {
                                Projectile.ai[0] = 1;
                                Projectile.ai[1] = 0;
                            }
                            else //try again in a bit
                            {
                                Projectile.ai[1] = 540;
                            }
                        }
                    }
                    else
                    {
                        Projectile.ai[1] = 0;
                        oldMouse = Main.MouseWorld;
                    }
                }

                if (++Projectile.frameCounter > 6)
                {
                    Projectile.frameCounter = 0;
                    if (++Projectile.frame >= 4)
                        Projectile.frame = 0;
                }
            }

            Projectile.spriteDirection = Projectile.direction;
        }

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            fallThrough = false;
            return base.TileCollideStyle(ref width, ref height, ref fallThrough, ref hitboxCenterFrac);
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            return false;
        }

        private void Movement(Vector2 targetPos, float speedModifier, float cap = 12f)
        {
            if (Projectile.Center.X < targetPos.X)
            {
                Projectile.velocity.X += speedModifier;
                if (Projectile.velocity.X < 0)
                    Projectile.velocity.X *= 0.95f;
            }
            else
            {
                Projectile.velocity.X -= speedModifier;
                if (Projectile.velocity.X > 0)
                    Projectile.velocity.X *= 0.95f;
            }
            if (Projectile.Center.Y < targetPos.Y)
            {
                Projectile.velocity.Y += speedModifier;
                if (Projectile.velocity.Y < 0)
                    Projectile.velocity.Y *= 0.95f;
            }
            else
            {
                Projectile.velocity.Y -= speedModifier;
                if (Projectile.velocity.Y > 0)
                    Projectile.velocity.Y *= 0.95f;
            }
            if (Math.Abs(Projectile.velocity.X) > cap)
                Projectile.velocity.X = cap * Math.Sign(Projectile.velocity.X);
            if (Math.Abs(Projectile.velocity.Y) > cap)
                Projectile.velocity.Y = cap * Math.Sign(Projectile.velocity.Y);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            int num156 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new Rectangle(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;
            SpriteEffects spriteEffects = Projectile.spriteDirection < 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Projectile.GetAlpha(lightColor), Projectile.rotation, origin2, Projectile.scale, spriteEffects, 0);
            return false;
        }
    }
}