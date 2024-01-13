using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Toggler.Content;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Accessories.Souls
{
    //[AutoloadEquip(EquipType.Back)]
    public class WorldShaperSoul : BaseSoul
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            // DisplayName.SetDefault("World Shaper Soul");
            /* Tooltip.SetDefault(
@"Increased block and wall placement speed by 50%
Near infinite block placement and mining reach
Mining speed tripled
Shows the location of enemies, traps, and treasures
Auto paint and actuator effect
Provides light and allows gravity control
Grants toggleable Builder Mode:
    Tiles and walls will not be consumed
	Increased placement speed and range
Effect can be disabled in Soul Toggles menu
Effects of the Cell Phone and Royal Gel
Summons a pet Magic Lantern
'Limitless possibilities'"); */
            //DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "铸世者之魂");
            //Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese,
            //@"增加50%物块和墙壁的放置速度
            //近乎无限的放置和挖掘距离
            //挖掘速度x3
            //高亮标记敌人、陷阱和宝藏
            //自动刷漆和放置促动器
            //允许你控制重力且你会散发光芒
            //使你获得开启建造模式的能力：
            //放置物块时不会消耗物块且大幅增加放置速度
            //敌人不会生成
            //可以在 魂 选项菜单中禁用此效果
            //拥有手机和皇家凝胶效果
            //召唤一个魔法灯笼
            //'无限的可能性'");
        }

        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.value = 750000;

            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.UseSound = SoundID.Item6;
            Item.useTime = Item.useAnimation = 90;
        }

        protected override Color? nameColor => new Color(255, 239, 2);

        public override bool? UseItem(Player player) => true;

        public override void UseItemFrame(Player player)
        {
            if (player.itemTime == player.itemTimeMax / 2)
            {
                player.Spawn(PlayerSpawnContext.RecallFromItem);

                for (int d = 0; d < 70; d++)
                    Dust.NewDust(player.position, player.width, player.height, DustID.MagicMirror, 0f, 0f, 150, default, 1.5f);
            }
        }

        public override void UpdateInventory(Player player)
        {
            //cell phone
            player.accWatch = 3;
            player.accDepthMeter = 1;
            player.accCompass = 1;
            player.accFishFinder = true;
            player.accDreamCatcher = true;
            player.accOreFinder = true;
            player.accStopwatch = true;
            player.accCritterGuide = true;
            player.accJarOfSouls = true;
            player.accThirdEye = true;
            player.accCalendar = true;
            player.accWeatherRadio = true;
            player.chiselSpeed = true;
            player.treasureMagnet = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            AddEffects(player, Item, hideVisual);
        }
        public static void AddEffects(Player player, Item item, bool hideVisual)
        {
            Player Player = player;
            //mining speed, spelunker, dangersense, light, hunter, pet
            MinerEnchant.AddEffects(Player, .66f, item);
            //placing speed up
            Player.tileSpeed += 0.5f;
            Player.wallSpeed += 0.5f;
            //toolbox
            if (Player.whoAmI == Main.myPlayer)
            {
                Player.tileRangeX += 10;
                Player.tileRangeY += 10;
            }
            //gizmo pack
            Player.autoPaint = true;
            //presserator
            Player.autoActuator = true;
            //royal gel
            Player.npcTypeNoAggro[1] = true;
            Player.npcTypeNoAggro[16] = true;
            Player.npcTypeNoAggro[59] = true;
            Player.npcTypeNoAggro[71] = true;
            Player.npcTypeNoAggro[81] = true;
            Player.npcTypeNoAggro[138] = true;
            Player.npcTypeNoAggro[121] = true;
            Player.npcTypeNoAggro[122] = true;
            Player.npcTypeNoAggro[141] = true;
            Player.npcTypeNoAggro[147] = true;
            Player.npcTypeNoAggro[183] = true;
            Player.npcTypeNoAggro[184] = true;
            Player.npcTypeNoAggro[204] = true;
            Player.npcTypeNoAggro[225] = true;
            Player.npcTypeNoAggro[244] = true;
            Player.npcTypeNoAggro[302] = true;
            Player.npcTypeNoAggro[333] = true;
            Player.npcTypeNoAggro[335] = true;
            Player.npcTypeNoAggro[334] = true;
            Player.npcTypeNoAggro[336] = true;
            Player.npcTypeNoAggro[537] = true;

            player.AddEffect<BuilderEffect>(item);

            //cell phone
            Player.accWatch = 3;
            Player.accDepthMeter = 1;
            Player.accCompass = 1;
            Player.accFishFinder = true;
            Player.accDreamCatcher = true;
            Player.accOreFinder = true;
            Player.accStopwatch = true;
            Player.accCritterGuide = true;
            Player.accJarOfSouls = true;
            Player.accThirdEye = true;
            Player.accCalendar = true;
            Player.accWeatherRadio = true;
            Player.chiselSpeed = true;
            Player.treasureMagnet = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()

            //step stool
            //greedy ring

            .AddIngredient(null, "MinerEnchant")
            .AddIngredient(ItemID.Toolbelt)
            .AddIngredient(ItemID.Toolbox)
            .AddIngredient(ItemID.HandOfCreation)
            .AddIngredient(ItemID.ActuationAccessory)
            .AddIngredient(ItemID.LaserRuler)
            .AddIngredient(ItemID.RoyalGel)
            .AddIngredient(ItemID.Shellphone)
            //.AddIngredient(ItemID.BloodHamaxe) //haemoraxe
            .AddRecipeGroup("FargowiltasSouls:AnyDrax")
            .AddIngredient(ItemID.ShroomiteDiggingClaw)
            .AddIngredient(ItemID.DrillContainmentUnit)
            //.AddIngredient(ItemID.BallOfFuseWire) //dynamite kitten pet

            .AddTile(ModContent.Find<ModTile>("Fargowiltas", "CrucibleCosmosSheet"))


            .Register();
        }
    }
    public class BuilderEffect : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<WorldShaperHeader>();
        public override void PostUpdateEquips(Player player)
        {
            if (player.whoAmI != Main.myPlayer)
                return;
            player.FargoSouls().BuilderMode = true;
            //if (Main.netMode == NetmodeID.MultiplayerClient) NetMessage.SendData(MessageID.SyncPlayer, number: Player.whoAmI);

            for (int i = 0; i < TileLoader.TileCount; i++)
            {
                player.adjTile[i] = true;
            }

            //placing speed up
            player.tileSpeed += 0.5f;
            player.wallSpeed += 0.5f;

            //toolbox
            if (player.HeldItem.createWall == 0) //tiles
            {
                Player.tileRangeX += 60;
                Player.tileRangeY += 60;
            }
            else //walls
            {
                Player.tileRangeX += 20;
                Player.tileRangeY += 20;
            }
        }
    }
}
