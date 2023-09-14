using Microsoft.Xna.Framework;
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
            SquireEffect(player);

            player.GetModPlayer<FargoSoulsPlayer>().SquireEnchantActive = true;
            if (!player.GetToggleValue("SquirePanic"))
                player.buffImmune[BuffID.BallistaPanic] = true;
        }

        public static void SquireEffect(Player player)
        {
            Mount mount = player.mount;

            if (mount.Active )
            {
                FargoSoulsPlayer modPlayer = player.GetModPlayer<FargoSoulsPlayer>();

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

                player.statDefense += 10;
                player.canJumpAgain_Fart = true;

                mount._data.acceleration = modPlayer.BaseSquireMountData.acceleration * 5f;
                mount._data.dashSpeed = modPlayer.BaseSquireMountData.dashSpeed * 2f;
                //mount._data.jumpHeight = modPlayer.BaseSquireMountData.jumpHeight * 3;
                mount._data.fallDamage = 0;
            }
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
