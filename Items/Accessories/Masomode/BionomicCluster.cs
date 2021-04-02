using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using FargowiltasSouls.Toggler;

namespace FargowiltasSouls.Items.Accessories.Masomode
{
    public class BionomicCluster : SoulsItem
    {
        public override bool Eternity => true;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bionomic Cluster");
            Tooltip.SetDefault("Grants immunity to Frostburn, Shadowflame, Squeaky Toy, Guilty, Mighty Wind, and Suffocation" +
                "\nGrants immunity to Flames of the Universe, Clipped Wings, Crippled, Webbed, and Purified" +
                "\nGrants immunity to Lovestruck, Stinky, Midas, Hexed, cactus damage, and enemies that steal items" +
                "\nYour attacks can inflict Clipped Wings, spawn Frostfireballs, and produce hearts" +
                "\nYou have autofire, improved night vision, and faster respawn when no boss is alive" +
                "\nAutomatically use mana potions when needed\r\nAttacks have a chance to squeak and deal 1 damage to you" +
                "\nYou erupt into Shadowflame tentacles when injured and respawn with more life" +
                "\nCertain enemies will drop potions when defeated and 50% discount on reforges" +
                "\nSummons a friendly rainbow slime" +
                "\nUse to teleport to your last death point and right click to zoom" +
                "\n'The amalgamate born of a thousand common enemies'");

            DisplayName.AddTranslation(GameCulture.Chinese, "生态集群");
            Tooltip.AddTranslation(GameCulture.Chinese, "'由上千普通敌人融合而成'" +
                "\n免疫寒焰,暗影烈焰,吱吱响的玩具,内疚,强风和窒息" +
                "\n免疫宇宙之火,剪除羽翼,残疾,织网和净化" +
                "\n免疫热恋,恶臭,点金手,着魔,仙人掌的伤害和偷取物品的敌人" +
                "\n攻击造成剪除羽翼,发射霜火球,并且产生心" +
                "\n一键连发,提高夜视能力," +
                "\n没有Boss存活时,重生速度加快" +
                "\n在需要时自动使用魔力药水,并给予词缀保护" +
                "\n敌人攻击概率无效,只造成1点伤害" +
                "\n受伤时爆发暗影烈焰触须\r\n召唤一个友善的彩虹史莱姆" +
                "\n按下快捷键传送到上次死亡地点");
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.accessory = true;
            item.rare = ItemRarityID.Yellow;
            item.value = Item.sellPrice(0, 6);
            item.defense = 6;
            item.useTime = 90;
            item.useAnimation = 90;
            item.useStyle = ItemUseStyleID.HoldingUp;
            item.useTurn = true;
            item.UseSound = SoundID.Item6;
        }

        public override void UpdateInventory(Player player)
        {
            FargoPlayer fargoPlayer = player.GetModPlayer<FargoPlayer>();
            player.buffImmune[BuffID.WindPushed] = true;
            fargoPlayer.SandsofTime = true;
            player.buffImmune[BuffID.Suffocation] = true;
            player.manaFlower = true;
            fargoPlayer.SecurityWallet = true;
            fargoPlayer.TribalCharm = true;
            fargoPlayer.NymphsPerfumeRespawn = true;
            player.nightVision = true;
            if (player.GetToggleValue("MasoCarrot", false))
                player.scope = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            FargoPlayer fargoPlayer = player.GetModPlayer<FargoPlayer>();

            // Concentrated rainbow matter
            player.buffImmune[mod.BuffType("FlamesoftheUniverse")] = true;
            if (player.GetToggleValue("MasoRainbow"))
                player.AddBuff(mod.BuffType("RainbowSlime"), 2);

            // Dragon fang
            player.buffImmune[mod.BuffType("ClippedWings")] = true;
            player.buffImmune[mod.BuffType("Crippled")] = true;
            if (player.GetToggleValue("MasoClipped"))
                fargoPlayer.DragonFang = true;

            // Frigid gemstone
            player.buffImmune[BuffID.Frostburn] = true;
            if (player.GetToggleValue("MasoFrigid"))
            {
                fargoPlayer.FrigidGemstone = true;
                if (fargoPlayer.FrigidGemstoneCD > 0)
                    fargoPlayer.FrigidGemstoneCD--;
            }

            // Wretched pouch
            player.buffImmune[BuffID.ShadowFlame] = true;
            player.buffImmune[mod.BuffType("Shadowflame")] = true;
            player.GetModPlayer<FargoPlayer>().WretchedPouch = true;

            // Sands of time
            player.buffImmune[BuffID.WindPushed] = true;
            fargoPlayer.SandsofTime = true;

            // Squeaky toy
            player.buffImmune[mod.BuffType("SqueakyToy")] = true;
            player.buffImmune[mod.BuffType("Guilty")] = true;
            fargoPlayer.SqueakyAcc = true;

            // Tribal charm
            player.buffImmune[BuffID.Webbed] = true;
            player.buffImmune[mod.BuffType("Purified")] = true;
            fargoPlayer.TribalCharm = true;

            // Mystic skull
            player.buffImmune[BuffID.Suffocation] = true;
            player.manaFlower = true;

            // Security wallet
            player.buffImmune[mod.BuffType("Midas")] = true;
            fargoPlayer.SecurityWallet = true;

            // Carrot
            player.nightVision = true;
            if (player.GetToggleValue("MasoCarrot", false))
                player.scope = true;

            // Nymph's perfume
            player.buffImmune[BuffID.Lovestruck] = true;
            player.buffImmune[mod.BuffType("Lovestruck")] = true;
            player.buffImmune[mod.BuffType("Hexed")] = true;
            player.buffImmune[BuffID.Stinky] = true;
            fargoPlayer.NymphsPerfumeRespawn = true;
            if (player.GetToggleValue("MasoNymph"))
            {
                fargoPlayer.NymphsPerfume = true;
                if (fargoPlayer.NymphsPerfumeCD > 0)
                    fargoPlayer.NymphsPerfumeCD--;
            }

            // Tim's concoction
            if (player.whoAmI == Main.myPlayer && player.GetToggleValue("MasoConcoction"))
                player.GetModPlayer<FargoPlayer>().TimsConcoction = true;
        }

        public override bool CanUseItem(Player player) => player.lastDeathPostion != Vector2.Zero;

        public override bool UseItem(Player player)
        {
            for (int index = 0; index < 70; ++index)
            {
                int d = Dust.NewDust(player.position, player.width, player.height, 87, player.velocity.X * 0.5f, player.velocity.Y * 0.5f, 150, new Color(), 1.5f);
                Main.dust[d].velocity *= 4f;
                Main.dust[d].noGravity = true;
            }

            player.grappling[0] = -1;
            player.grapCount = 0;
            for (int index = 0; index < 1000; ++index)
            {
                if (Main.projectile[index].active && Main.projectile[index].owner == player.whoAmI && Main.projectile[index].aiStyle == 7)
                    Main.projectile[index].Kill();
            }

            if (player.whoAmI == Main.myPlayer)
            {
                player.Teleport(player.lastDeathPostion, 1);
                player.velocity = Vector2.Zero;
                if (Main.netMode == NetmodeID.MultiplayerClient)
                    NetMessage.SendData(MessageID.Teleport, -1, -1, null, 0, player.whoAmI, player.lastDeathPostion.X, player.lastDeathPostion.Y, 1);
            }

            for (int index = 0; index < 70; ++index)
            {
                int d = Dust.NewDust(player.position, player.width, player.height, 87, 0.0f, 0.0f, 150, new Color(), 1.5f);
                Main.dust[d].velocity *= 4f;
                Main.dust[d].noGravity = true;
            }

            return true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);

            recipe.AddIngredient(mod.ItemType("ConcentratedRainbowMatter"));
            recipe.AddIngredient(mod.ItemType("WyvernFeather"));
            recipe.AddIngredient(mod.ItemType("FrigidGemstone"));
            recipe.AddIngredient(mod.ItemType("SandsofTime"));
            recipe.AddIngredient(mod.ItemType("SqueakyToy"));
            recipe.AddIngredient(mod.ItemType("TribalCharm"));
            recipe.AddIngredient(mod.ItemType("MysticSkull"));
            recipe.AddIngredient(mod.ItemType("SecurityWallet"));
            recipe.AddIngredient(mod.ItemType("OrdinaryCarrot"));
            recipe.AddIngredient(mod.ItemType("WretchedPouch"));
            recipe.AddIngredient(mod.ItemType("NymphsPerfume"));
            recipe.AddIngredient(mod.ItemType("TimsConcoction"));
            //recipe.AddIngredient(ItemID.SoulofLight, 20);
            //recipe.AddIngredient(ItemID.SoulofNight, 20);
            recipe.AddIngredient(mod.ItemType("DeviatingEnergy"), 10);

            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}