using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using FargowiltasSouls.Toggler;

namespace FargowiltasSouls.Items.Accessories.Masomode
{
    [AutoloadEquip(EquipType.Shoes)]
    public class AeolusBoots : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Aeolus Boots");
            Tooltip.SetDefault(@"Allows flight, super fast running, and extra mobility on ice
The wearer can run even faster on sand
8% increased movement speed
Provides the ability to walk on water, honey & lava
Grants immunity to fire blocks and 7 seconds of immunity to lava
Reduces damage from touching lava
Increases jump speed and allows auto-jump
Flowers grow on the grass you walk on
Allows the holder to double jump
Increases jump height and negates fall damage
'Run like the wind'");
            DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "埃俄罗斯之靴");
            Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, @"使你获得飞行和快速奔跑能力，提供冰面上的额外机动性
增加8%移动速度
使你获得二段跳能力
增加跳跃高度并免疫摔落伤害
'像风一样奔跑'");

            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.accessory = true;
            Item.rare = ItemRarityID.Cyan;
            Item.value = Item.sellPrice(0, 20);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            //frostspark
            player.accRunSpeed = 6.75f;
            player.rocketBoots = 4; //terraspark
            player.moveSpeed += 0.08f;
            player.iceSkate = true;

            //lava wader
            player.waterWalk = true;
            player.fireWalk = true;
            player.lavaMax += 420;
            player.lavaRose = true;

            //amph boot
            player.autoJump = true;
            player.frogLegJumpBoost = true;
            player.accFlipper = true;
            player.spikedBoots += 2;

            //fairy boot
            if (player.whoAmI == Main.myPlayer && player.GetToggleValue("MasoAeolusFlower"))
                player.flowerBoots = true;

            //dunerider boot
            player.desertBoots = true;

            //fart balloon
            if (player.whoAmI == Main.myPlayer && player.GetToggleValue("MasoAeolus"))
            {
                player.hasJumpOption_Fart = true;
                if (Main.netMode == NetmodeID.MultiplayerClient)
                    NetMessage.SendData(MessageID.SyncPlayer, number: player.whoAmI);
            }
            player.jumpBoost = true;
            player.noFallDmg = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.TerrasparkBoots)
                .AddIngredient(ItemID.AmphibianBoots)
                .AddIngredient(ItemID.FairyBoots)
                .AddIngredient(ItemID.SandBoots)
                .AddIngredient(ItemID.BalloonHorseshoeFart)
                .AddIngredient(ModContent.ItemType<EurusSock>())
                .AddIngredient(ModContent.ItemType<Misc.DeviatingEnergy>(), 10)
                .AddTile(TileID.TinkerersWorkbench)
                .Register();
        }
    }
}
