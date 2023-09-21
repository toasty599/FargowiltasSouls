using FargowiltasSouls.Content.Buffs.Souls;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;

namespace FargowiltasSouls.Content.Items.Accessories.Enchantments
{
    public class SquireEnchant : BaseEnchant
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }

        protected override Color nameColor => new(148, 143, 140);

        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.rare = ItemRarityID.Yellow;
            Item.value = 150000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            SquireEffect(player, Item);
        }

        public static void SquireEffect(Player player, Item item)
        {
            FargoSoulsPlayer modPlayer = player.GetModPlayer<FargoSoulsPlayer>();
            modPlayer.SquireEnchantItem = item;

            player.DisplayToggle("SquirePanic");

            if (!player.GetToggleValue("SquirePanic"))
                player.buffImmune[BuffID.BallistaPanic] = true;

            Mount mount = player.mount;

            if (mount.Active)
            {
                if (modPlayer.BaseMountType != mount.Type)
                {
                    Mount.MountData original = Mount.mounts[mount.Type];
                    //copy over ANYTHING that will be changed
                    modPlayer.BaseSquireMountData = new Mount.MountData();
                    modPlayer.BaseSquireMountData.acceleration = original.acceleration;
                    modPlayer.BaseSquireMountData.dashSpeed = original.dashSpeed;
                    modPlayer.BaseSquireMountData.fallDamage = original.fallDamage;

                    modPlayer.BaseMountType = mount.Type;
                }

                if (modPlayer.ValhallaEnchantActive)
                {
                    player.DisplayToggle("Valhalla");
                    player.statDefense += 15;

                    if (modPlayer.IsDashingTimer == 0)
                    {
                        //mount dash
                        //if ((player.controlDown && player.releaseDown))
                        //{
                        //    if (player.doubleTapCardinalTimer[0] > 0 && player.doubleTapCardinalTimer[0] != 15)
                        //    {
                        //        ValhallaDash(player, true, 1);
                        //    }
                        //}
                        ////up
                        //else if ((player.controlUp && player.releaseUp))
                        //{
                        //    if (player.doubleTapCardinalTimer[1] > 0 && player.doubleTapCardinalTimer[1] != 15)
                        //    {
                        //        ValhallaDash(player, true, -1);
                        //    }
                        //}
                        if (player.controlRight && player.releaseRight)
                        {
                            if (player.doubleTapCardinalTimer[2] > 0 && player.doubleTapCardinalTimer[2] != 15)
                            {
                                ValhallaDash(player, false, 1);
                            }
                        }
                        else if (player.controlLeft && player.releaseLeft)
                        {
                            if (player.doubleTapCardinalTimer[3] > 0 && player.doubleTapCardinalTimer[3] != 15)
                            {
                                ValhallaDash(player, false, -1);
                            }
                        }
                    }

                    //spawn lance..... eh
                    //Projectile.NewProjectile(player.GetSource_Accessory(item), player.Center, player.velocity, ProjectileID.ShadowJoustingLance, 10, 0, player.whoAmI);
                }

                player.statDefense += 10;
                player.hasJumpOption_Fart = true;

                mount._data.acceleration = modPlayer.BaseSquireMountData.acceleration * 3f;
                mount._data.dashSpeed = modPlayer.BaseSquireMountData.dashSpeed * 2f;
                //mount._data.jumpHeight = modPlayer.BaseSquireMountData.jumpHeight * 3;
                mount._data.fallDamage = 0;
            }
        }

        public static void ValhallaDash(Player player, bool vertical, int direction)
        {
            //horizontal
            if (!vertical)
            {
                player.GetModPlayer<FargoSoulsPlayer>().MonkDashing = 15;
                player.velocity.X = 50 * (float)direction;
            }
            else
            {
                player.GetModPlayer<FargoSoulsPlayer>().MonkDashing = -15;
                player.velocity.Y = 30 * (float)direction;
            }

            player.dashDelay = 20;
            if (player.GetModPlayer<FargoSoulsPlayer>().IsDashingTimer < 20)
                player.GetModPlayer<FargoSoulsPlayer>().IsDashingTimer = 20;

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

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ItemID.SquireGreatHelm)
            .AddIngredient(ItemID.SquirePlating)
            .AddIngredient(ItemID.SquireGreaves)
            .AddIngredient(ItemID.DD2BallistraTowerT2Popper)
            .AddIngredient(ItemID.MajesticHorseSaddle)
            .AddIngredient(ItemID.JoustingLance)

            .AddTile(TileID.CrystalBall)
            .Register();
        }
    }
}
