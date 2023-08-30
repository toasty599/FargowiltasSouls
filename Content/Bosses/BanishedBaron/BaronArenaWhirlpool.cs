using System;
using System.Numerics;
using FargowiltasSouls.Core.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Color = Microsoft.Xna.Framework.Color;
using Rectangle = Microsoft.Xna.Framework.Rectangle;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace FargowiltasSouls.Content.Bosses.BanishedBaron
{

    public class BaronArenaWhirlpool : ModProjectile
    {
        public override string Texture => "FargowiltasSouls/Content/Bosses/BanishedBaron/BaronWhirlpool";
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Banished Baron's Mine");
            Main.projFrames[Type] = 16;
        }
        public override void SetDefaults()
        {
            Projectile.width = 186;
            Projectile.height = 48;
            Projectile.aiStyle = -1;
            Projectile.hostile = false;
            Projectile.penetrate = 1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.scale = 1f;
            Projectile.light = 1;
            Projectile.alpha = 255;
            Projectile.timeLeft = 60 * 60 * 60;
        }
        const int MaxDistance = 1000;
        int WaterwallDistance = 0;
        public override void AI()
        {
            ref float ParentID = ref Projectile.ai[0];
            ref float Timer = ref Projectile.ai[1];
            ref float State = ref Projectile.ai[2];

            NPC baron = Main.npc[(int)ParentID];
            if (!baron.active || baron.type != ModContent.NPCType<BanishedBaron>() || !NPC.AnyNPCs(ModContent.NPCType<BanishedBaron>()))
            {
                Projectile.Kill();
            }
            Player player = Main.player[baron.target];
            if (player.active)
            {
                if (Timer == 0) //done this way to work with world borders
                {
                    if (player.Center.X < MaxDistance)
                    {
                        Projectile.Center = new Vector2(MaxDistance, player.Center.Y);
                    }
                    else if (player.Center.X > Main.maxTilesX - MaxDistance)
                    {
                        Projectile.Center = new Vector2(Main.maxTilesX - MaxDistance, player.Center.Y);
                    }
                    else
                    {
                        Projectile.Center = player.Center;
                    }
                    Projectile.Center = Main.screenPosition + new Vector2(Main.screenWidth / 2, Main.screenHeight / 2); //not player center to work with map borders
                }
                Projectile.Center = Projectile.Center.X * Vector2.UnitX + player.Center.Y * Vector2.UnitY;

                int wallAttackTime = WorldSavingSystem.MasochistModeReal ? 60 : WorldSavingSystem.EternityMode ? 70 : 80;
                if (State == 1 && Timer % wallAttackTime == 0)
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        FireBolts(player);
                    }
                    
                }
            }
            if (Projectile.alpha > 0)
            {
                Projectile.alpha -= 3;
            }
            else
            {
                Projectile.alpha = 0;
            }
            if (++Projectile.frameCounter > 2)
            {
                if (++Projectile.frame >= Main.projFrames[Type])
                {
                    Projectile.frame = 0;
                }
                Projectile.frameCounter = 0;
            }
            if (WaterwallDistance < MaxDistance)
                WaterwallDistance += 10;
            WaterWalls(Projectile.Center, WaterwallDistance);
            Timer++;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            int num156 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new(0, y3, texture.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;


            SpriteEffects effects = Projectile.spriteDirection > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            
            for (int Side = -1; Side < 2; Side += 2)
            {
                for (int i = -15; i <= 15; i++)
                {
                    Vector2 center = Projectile.Center + Vector2.UnitX * Side * (WaterwallDistance + (rectangle.Width / 2)) + Vector2.UnitY * i * Projectile.height;
                    center.Y = (float)Math.Floor(center.Y / rectangle.Height) * rectangle.Height; //makes them not smoothly move up and down, but jump one chunk at a time
                    if (Collision.SolidCollision(center - rectangle.Size() / 2, rectangle.Width, rectangle.Height))
                    {
                        continue;
                    }
                    
                    Main.EntitySpriteDraw(texture, center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Rectangle?(rectangle), Projectile.GetAlpha(lightColor), Projectile.rotation, origin2, Projectile.scale, effects, 0);
                }
            }
            return false;
        }
        //private int nearestMultiple(int value, int factor) { return (int)Math.Round((value / (double)factor), MidpointRounding.AwayFromZero) * factor; }

        private void WaterWalls(Vector2 location, int threshold)
        {
            //Aura
            if (true)
            {
                Player player = Main.LocalPlayer;
                bool OutsideRange = Math.Abs(player.Center.X - location.X) >= threshold;
                bool OutsideRangex2 = Math.Abs(player.Center.X - location.X) >= threshold * 2;
                bool OutsideRangex4 = Math.Abs(player.Center.X - location.X) >= threshold * 4;
                if (player.active && !player.dead && !player.ghost) //pull into arena
                {
                    if (OutsideRange && !OutsideRangex4)
                    {
                        if (OutsideRangex2)
                        {
                            player.controlLeft = false;
                            player.controlRight = false;
                            player.controlUp = false;
                            player.controlDown = false;
                            player.controlUseItem = false;
                            player.controlUseTile = false;
                            player.controlJump = false;
                            player.controlHook = false;
                            if (player.grapCount > 0)
                                player.RemoveAllGrapplingHooks();
                            if (player.mount.Active)
                                player.mount.Dismount(player);
                            player.velocity.X = 0f;
                            player.velocity.Y = -0.4f;
                            player.GetModPlayer<FargoSoulsPlayer>().NoUsingItems = 2;
                        }

                        Vector2 movement = new Vector2(location.X - player.Center.X, 0);
                        float difference = movement.Length() - threshold;
                        movement.Normalize();
                        movement *= difference < 17f ? difference : 17f;
                        player.position += movement;

                        //remove outwards velocity
                        int playerDir = Math.Sign(player.Center.X - location.X);
                        if (Math.Sign(player.velocity.X) != 0 && Math.Sign(player.velocity.X) != playerDir)
                        {
                            player.velocity.X = 0;
                        }

                        for (int i = 0; i < 10; i++)
                        {
                            int DustType = DustID.WaterCandle;
                            int d = Dust.NewDust(player.position, player.width, player.height, DustType, 0f, 0f, 0, default(Color), 1.25f);
                            Main.dust[d].noGravity = true;
                            Main.dust[d].velocity *= 5f;
                        }
                    }
                }
            }
        }

        private void FireBolts(Player player)
        {
            int Side = Math.Sign(player.Center.X - Projectile.Center.X);
            for (int i = -10; i <= 10; i++)
            {
                const int speed = 7;
                const float distancePos = 3.2f;
                const float randomPos = 9;
                const float maxRot = MathHelper.Pi / 17;
                float posX = Projectile.Center.X + (Side * (Projectile.width * 0.8f + WaterwallDistance));
                float posY = player.Center.Y + (i * Projectile.height * distancePos) + Main.rand.NextFloat(-randomPos, randomPos);
                Vector2 pos = posX * Vector2.UnitX + posY * Vector2.UnitY;
                Vector2 vel = (Vector2.UnitX * Side).RotatedBy(Main.rand.NextFloat(-maxRot, maxRot)) * speed;
                Projectile.NewProjectile(Projectile.InheritSource(Projectile), pos, vel, ModContent.ProjectileType<BaronWhirlpoolBolt>(), Projectile.damage, Projectile.knockBack, Main.myPlayer, 2, -Side);
            }
            SoundEngine.PlaySound(SoundID.Item21, Projectile.Center + (Vector2.UnitX * Side * (Projectile.width * 0.8f + WaterwallDistance)));
        }
    }
}