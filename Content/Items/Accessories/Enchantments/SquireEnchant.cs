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
            FargoSoulsPlayer modPlayer = player.FargoSouls();
            modPlayer.SquireEnchantItem = item;

            player.DisplayToggle("SquirePanic");

            if (!player.GetToggleValue("SquirePanic"))
                player.buffImmune[BuffID.BallistaPanic] = true;

            Mount mount = player.mount;

            if (mount.Active)
            {
                if (modPlayer.BaseMountType != mount.Type)
                {
                    //we want to reset the prev mount first or all hell breaks loose
                    if (modPlayer.BaseMountType != -1)
                    {
                        Mount.mounts[modPlayer.BaseMountType].acceleration = modPlayer.BaseSquireMountData.acceleration;
                        Mount.mounts[modPlayer.BaseMountType].dashSpeed = modPlayer.BaseSquireMountData.dashSpeed;
                        Mount.mounts[modPlayer.BaseMountType].fallDamage = modPlayer.BaseSquireMountData.fallDamage;

                        Mount.mounts[modPlayer.BaseMountType].jumpSpeed = modPlayer.BaseSquireMountData.jumpSpeed;
                        Mount.mounts[modPlayer.BaseMountType].swimSpeed = modPlayer.BaseSquireMountData.swimSpeed;
                        Mount.mounts[modPlayer.BaseMountType].runSpeed = modPlayer.BaseSquireMountData.runSpeed;
                    }

                    Mount.MountData original = Mount.mounts[mount.Type];
                    //copy over ANYTHING that will be changed
                    modPlayer.BaseSquireMountData = new Mount.MountData();
                    modPlayer.BaseSquireMountData.acceleration = original.acceleration;
                    modPlayer.BaseSquireMountData.dashSpeed = original.dashSpeed;
                    modPlayer.BaseSquireMountData.fallDamage = original.fallDamage;

                    modPlayer.BaseSquireMountData.jumpSpeed = original.jumpSpeed;
                    modPlayer.BaseSquireMountData.swimSpeed = original.swimSpeed;
                    modPlayer.BaseSquireMountData.runSpeed = original.runSpeed;

                    modPlayer.BaseMountType = mount.Type;
                }

                int defenseBoost;
                float accelBoost;
                float speedBoost;

                if (modPlayer.ValhallaEnchantItem != null && modPlayer.ForceEffect(modPlayer.ValhallaEnchantItem.type))
                {
                    defenseBoost = 30;
                    accelBoost = 3f;
                    speedBoost = 2f;
                }
                else if (modPlayer.ValhallaEnchantItem != null || modPlayer.ForceEffect(modPlayer.SquireEnchantItem.type))
                {
                    defenseBoost = 20;
                    accelBoost = 2f;
                    speedBoost = 2f;
                }
                else
                {
                    defenseBoost = 10;
                    accelBoost = 1.5f;
                    speedBoost = 1.5f;
                }

                if (modPlayer.ValhallaEnchantItem != null)
                {
                    player.DisplayToggle("Valhalla");

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
                        if ((player.controlUp && player.releaseUp))
                        {
                            if (player.doubleTapCardinalTimer[1] > 0 && player.doubleTapCardinalTimer[1] != 15)
                            {
                                ValhallaDash(player, true, -1);
                            }
                        }
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
                }

                player.hasJumpOption_Fart = true;
                player.statDefense += defenseBoost;
                
                mount._data.acceleration = modPlayer.BaseSquireMountData.acceleration * accelBoost;
                mount._data.dashSpeed = modPlayer.BaseSquireMountData.dashSpeed * speedBoost;
                mount._data.fallDamage = 0;

                mount._data.jumpSpeed = modPlayer.BaseSquireMountData.jumpSpeed * speedBoost;
                //mount._data.swimSpeed = modPlayer.BaseSquireMountData.swimSpeed * speedBoost;
                //mount._data.runSpeed = modPlayer.BaseSquireMountData.runSpeed * speedBoost;

                Main.NewText(mount.DashSpeed);
            }
        }

        public static void ValhallaDash(Player player, bool vertical, int direction)
        {
            //horizontal
            if (!vertical)
            {
                player.FargoSouls().MonkDashing = 15;
                player.velocity.X = 50 * (float)direction;
            }
            else
            {
                player.FargoSouls().MonkDashing = -15;
                player.velocity.Y = 30 * (float)direction;
            }

            player.dashDelay = 30;
            if (player.FargoSouls().IsDashingTimer < 15)
                player.FargoSouls().IsDashingTimer = 15;

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
