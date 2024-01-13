using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using FargowiltasSouls.Content.Projectiles.Souls;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.ModPlayers;
using FargowiltasSouls.Core.Systems;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Buffs.Souls
{
    public class MonkBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Meditation");
            // Description.SetDefault("Your Monk Dash is ready");
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            if (!(player.whoAmI == Main.myPlayer && !player.HasEffect<MonkDashEffect>()))
                player.buffTime[buffIndex] = 2;
            if (player.mount.Active)
            {
                return;
            }
            player.FargoSouls().HasDash = true;
            player.FargoSouls().FargoDash = DashManager.DashType.Monk;
        }
        public static void MonkDash(Player player, bool vertical, int direction)
        {
            //horizontal
            if (!vertical)
            {
                FargoSoulsPlayer modPlayer = player.FargoSouls();
                modPlayer.MonkDashing = 30;
                player.velocity.X = 14 * (float)direction;

                player.immune = true;
                int invul = 30;
                player.immuneTime = Math.Max(player.immuneTime, invul);
                player.hurtCooldowns[0] = Math.Max(player.hurtCooldowns[0], invul);
                player.hurtCooldowns[1] = Math.Max(player.hurtCooldowns[1], invul);
                bool monkForce = player.GetEffectFields<MonkFields>().ShinobiEnchantActive || modPlayer.ForceEffect<MonkEnchant>();
                bool shinobiForce = modPlayer.ForceEffect<ShinobiEnchant>();

                Vector2 pos = player.Center;

                int damage = monkForce ? (shinobiForce ? 1500 : 1000) : 500;

                Projectile.NewProjectile(player.GetSource_FromThis(), pos, Vector2.Zero, ModContent.ProjectileType<MonkDashDamage>(), damage, 0);
            }
            else
            {
                player.FargoSouls().MonkDashing = -20;
                player.velocity.Y = 35 * (float)direction;
            }

            player.dashDelay = 115;
            if (player.FargoSouls().IsDashingTimer < 20)
                player.FargoSouls().IsDashingTimer = 20;

            if (Main.netMode == NetmodeID.MultiplayerClient)
                NetMessage.SendData(MessageID.PlayerControls, number: player.whoAmI);

            //dash dust n stuff
            for (int num17 = 0; num17 < 20; num17++)
            {
                int num18 = Dust.NewDust(new Vector2(player.position.X, player.position.Y), player.width, player.height, DustID.Smoke, 0f, 0f, 100, default, 2f);
                Dust expr_CDB_cp_0 = Main.dust[num18];
                expr_CDB_cp_0.position.X += Main.rand.Next(-5, 6);
                Dust expr_D02_cp_0 = Main.dust[num18];
                expr_D02_cp_0.position.Y += Main.rand.Next(-5, 6);
                Main.dust[num18].velocity *= 0.2f;
                Main.dust[num18].scale *= 1f + Main.rand.Next(20) * 0.01f;
                //Main.dust[num18].shader = GameShaders.Armor.GetSecondaryShader(player.cShoe, this);
            }
            int num19 = Gore.NewGore(player.GetSource_FromThis(), new Vector2(player.position.X + player.width / 2 - 24f, player.position.Y + player.height / 2 - 34f), default, Main.rand.Next(61, 64), 1f);
            Main.gore[num19].velocity.X = Main.rand.Next(-50, 51) * 0.01f;
            Main.gore[num19].velocity.Y = Main.rand.Next(-50, 51) * 0.01f;
            Main.gore[num19].velocity *= 0.4f;
            num19 = Gore.NewGore(player.GetSource_FromThis(), new Vector2(player.position.X + player.width / 2 - 24f, player.position.Y + player.height / 2 - 14f), default, Main.rand.Next(61, 64), 1f);
            Main.gore[num19].velocity.X = Main.rand.Next(-50, 51) * 0.01f;
            Main.gore[num19].velocity.Y = Main.rand.Next(-50, 51) * 0.01f;
            Main.gore[num19].velocity *= 0.4f;
        }
    }
}