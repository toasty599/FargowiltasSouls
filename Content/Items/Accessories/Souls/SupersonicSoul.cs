using FargowiltasSouls.Content.Items.Accessories.Masomode;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Toggler.Content;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Accessories.Souls
{
    //[AutoloadEquip(EquipType.Shoes)]
    public class SupersonicSoul : BaseSoul
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            // DisplayName.SetDefault("Supersonic Soul");

            //string tooltip_ch =
            //@"'我就是速度'
            //获得超音速奔跑,飞行,以及额外的冰上移动力
            //在没有装备翅膀时,允许使用者进行五段跳
            //增加跳跃高度,跳跃速度,允许自动跳跃
            //获得游泳能力以及极长的水下呼吸时间
            //获得水/岩浆上行走能力
            //免疫岩浆和坠落伤害
            //拥有飞毯效果";

            // Tooltip.SetDefault(tooltip);
            //DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "超音速之魂");
            //Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, tooltip_ch);

        }

        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.value = 750000;
        }

        protected override Color? nameColor => new Color(238, 0, 69);

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            FargoSoulsPlayer modPlayer = player.FargoSouls();
            AddEffects(player, Item, hideVisual);
        }
        public static void AddEffects(Player player, Item item, bool hideVisual)
        {
            Player Player = player;
            FargoSoulsPlayer modPlayer = player.FargoSouls();

            player.AddEffect<MasoAeolusFrog>(item);
            player.AddEffect<MasoAeolusFlower>(item);
            player.AddEffect<MasoAeolus>(item);

            if (Player.AddEffect<SupersonicSpeedEffect>(item) && !Player.FargoSouls().noSupersonic && !FargoSoulsUtil.AnyBossAlive())
            {
                // 5 is the default value, I removed the config for it because the new toggler doesn't have sliders
                Player.runAcceleration += 5f * .1f;
                Player.maxRunSpeed += 5f * 2;
                //frog legs
                if (player.HasEffect<MasoAeolusFrog>())
                {
                    Player.autoJump = true;
                }
                Player.jumpSpeedBoost += 2.4f;
                Player.maxFallSpeed += 5f;
                Player.jumpBoost = true;
            }
            else
            {
                //calculated to match flight mastery soul, 6.75 same as frostspark
                Player.accRunSpeed = player.AddEffect<RunSpeed>(item) ? 15.6f : 6.75f;
            }

            if (player.AddEffect<NoMomentum>(item))
                modPlayer.NoMomentum = true;

            Player.moveSpeed += 0.5f;

            if (player.AddEffect<SupersonicRocketBoots>(item))
            {
                Player.rocketBoots = Player.vanityRocketBoots = ArmorIDs.RocketBoots.TerrasparkBoots;
                Player.rocketTimeMax = 10;
            }

            Player.iceSkate = true;

            //lava waders
            Player.waterWalk = true;
            Player.fireWalk = true;
            Player.lavaImmune = true;
            Player.noFallDmg = true;

            //bundle
            if (Player.AddEffect<SupersonicJumps>(item) && Player.wingTime == 0)
            {
                Player.GetJumpState(ExtraJump.CloudInABottle).Enable();
                Player.GetJumpState(ExtraJump.SandstormInABottle).Enable();
                Player.GetJumpState(ExtraJump.BlizzardInABottle).Enable();
                Player.GetJumpState(ExtraJump.FartInAJar).Enable();
            }

            //magic carpet
            if (Player.whoAmI == Main.myPlayer && Player.AddEffect<SupersonicCarpet>(item))
            {
                Player.carpet = true;
                if (Main.netMode == NetmodeID.MultiplayerClient)
                    NetMessage.SendData(MessageID.SyncPlayer, number: Player.whoAmI);

                if (Player.canCarpet)
                {
                    modPlayer.extraCarpetDuration = true;
                }
                else if (modPlayer.extraCarpetDuration)
                {
                    modPlayer.extraCarpetDuration = false;
                    Player.carpetTime = 1000;
                }
            }

            //EoC Shield
            if (Player.AddEffect<CthulhuShield>(item))
                Player.dashType = 2;

            //ninja gear
            if (Player.AddEffect<SupersonicTabi>(item))
                Player.dashType = 1;
            if (Player.AddEffect<BlackBelt>(item))
                Player.blackBelt = true;
            if (Player.AddEffect<BlackBelt>(item))
                Player.spikedBoots = 2;

            //sweetheart necklace
            if (Player.HasEffect<DefenseBeeEffect>() || Player.AddEffect<DefenseBeeEffect>(item))
            {
                Player.honeyCombItem = item;
            }
            if (Player.AddEffect<SupersonicPanic>(item))
            {
                Player.panic = true;
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe()

            .AddIngredient(ModContent.ItemType<AeolusBoots>()) //add terraspark boots
            .AddIngredient(ItemID.FlyingCarpet)
            .AddIngredient(ItemID.SweetheartNecklace)
            .AddIngredient(ItemID.BalloonHorseshoeHoney)
            .AddIngredient(ItemID.HorseshoeBundle)
            .AddIngredient(ItemID.EoCShield)
            .AddIngredient(ItemID.MasterNinjaGear)

            .AddIngredient(ItemID.MinecartMech)
            .AddIngredient(ItemID.BlessedApple)
            .AddIngredient(ItemID.AncientHorn)
            .AddIngredient(ItemID.ReindeerBells)
            .AddIngredient(ItemID.BrainScrambler)

            .AddTile(ModContent.Find<ModTile>("Fargowiltas", "CrucibleCosmosSheet"))


            .Register();
        }
    }
    // AAAAAAAAAAAAAAAAAAA
    public class RunSpeed : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<SupersonicHeader>();
        public override bool IgnoresMutantPresence => true;
    }
    public class NoMomentum : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<SupersonicHeader>();
        public override bool IgnoresMutantPresence => true;
    }
    public class SupersonicRocketBoots : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<SupersonicHeader>();
        public override bool IgnoresMutantPresence => true;
    }
    public class SupersonicJumps : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<SupersonicHeader>();
    }
    public class SupersonicCarpet : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<SupersonicHeader>();
        public override bool IgnoresMutantPresence => true;
    }
    public class CthulhuShield : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<SupersonicHeader>();
    }
    public class SupersonicTabi : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<SupersonicHeader>();
        public override bool IgnoresMutantPresence => true;
    }
    public class BlackBelt : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<SupersonicHeader>();
    }
    public class SupersonicClimbing : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<SupersonicHeader>();
    }
    public class SupersonicPanic : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<SupersonicHeader>();
    }
}