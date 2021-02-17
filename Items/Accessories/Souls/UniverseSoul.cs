using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;

namespace FargowiltasSouls.Items.Accessories.Souls
{
    public class UniverseSoul : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Soul of the Universe");

            string tooltip =
@"66% increased all damage
50% increased use speed for all weapons
50% increased shoot speed
25% increased all critical chance
Crits deal 5x damage
All weapons have double knockback
Increases your maximum mana by 300
Increases your max number of minions by 8
Increases your max number of sentries by 4
All attacks inflict Flames of the Universe
Effects of the Fire Gauntlet and Yoyo Bag
Effects of Sniper Scope, Celestial Cuffs and Mana Flower
'The heavens themselves bow to you'";
            string tooltip_ch =
@"'诸天也向你俯首'
增加66%所有伤害
增加50%所有武器使用速度
增加50%射击速度
增加25%所有暴击率
暴击造成5倍伤害
所有武器双倍击退
增加300最大法力值
";

            tooltip_ch +=
@"+8最大召唤栏
+4最大哨兵栏
所有攻击造成宇宙之火效果
拥有烈火手套和悠悠球袋的效果
拥有狙击镜, 星体手铐和魔力花的效果";

            Tooltip.SetDefault(tooltip);

            Main.RegisterItemAnimation(item.type, new DrawAnimationVertical(6, 10));
            DisplayName.AddTranslation(GameCulture.Chinese, "寰宇之魂");
            Tooltip.AddTranslation(GameCulture.Chinese, tooltip_ch);
        }
        public override int NumFrames => 10;

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.accessory = true;
            item.value = 5000000;
            item.rare = -12;
            item.expert = true;
            ItemID.Sets.ItemNoGravity[item.type] = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            FargoPlayer modPlayer = player.GetModPlayer<FargoPlayer>();
            modPlayer.AllDamageUp(.66f);
            modPlayer.AllCritUp(25);
            //use speed, velocity, debuffs, crit dmg, mana up, double knockback
            modPlayer.UniverseEffect = true;

            if (SoulConfig.Instance.GetValue(SoulConfig.Instance.UniverseAttackSpeed))
            {
                modPlayer.AttackSpeed += .5f;
            }

            player.maxMinions += 8;
            player.maxTurrets += 4;

            //accessorys
            if (SoulConfig.Instance.GetValue(SoulConfig.Instance.YoyoBag))
            {
                player.counterWeight = 556 + Main.rand.Next(6);
                player.yoyoGlove = true;
                player.yoyoString = true;
            }
            if (SoulConfig.Instance.GetValue(SoulConfig.Instance.SniperScope))
            {
                player.scope = true;
            }
            player.manaFlower = true;
            player.manaMagnet = true;
            player.magicCuffs = true;

            if (ModLoader.GetMod("FargowiltasSoulsDLC") != null)
            {
                Mod fargoDLC = ModLoader.GetMod("FargowiltasSoulsDLC");

                if (ModLoader.GetMod("ThoriumMod") != null)
                {
                    fargoDLC.GetItem("GuardianAngelsSoul").UpdateAccessory(player, hideVisual);
                    fargoDLC.GetItem("BardSoul").UpdateAccessory(player, hideVisual);
                }
                if (ModLoader.GetMod("CalamityMod") != null)
                {
                    fargoDLC.GetItem("RogueSoul").UpdateAccessory(player, hideVisual);
                }
                if (ModLoader.GetMod("DBZMOD") != null)
                {
                    fargoDLC.GetItem("KiSoul").UpdateAccessory(player, hideVisual);
                }
            }
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "GladiatorsSoul");
            recipe.AddIngredient(null, "SnipersSoul");
            recipe.AddIngredient(null, "ArchWizardsSoul");
            recipe.AddIngredient(null, "ConjuristsSoul");
            //recipe.AddIngredient(null, "OlympiansSoul");

            if (ModLoader.GetMod("FargowiltasSoulsDLC") != null)
            {
                Mod fargoDLC = ModLoader.GetMod("FargowiltasSoulsDLC");

                if (ModLoader.GetMod("ThoriumMod") != null)
                {
                    recipe.AddIngredient(fargoDLC.ItemType("GuardianAngelsSoul"));
                    recipe.AddIngredient(fargoDLC.ItemType("BardSoul"));
                }
                if (ModLoader.GetMod("CalamityMod") != null)
                {
                    recipe.AddIngredient(fargoDLC.ItemType("RogueSoul"));
                }
                if (ModLoader.GetMod("DBZMOD") != null)
                {
                    recipe.AddIngredient(fargoDLC.ItemType("KiSoul"));
                }
            }

            recipe.AddIngredient(null, "MutantScale", 10);

            recipe.AddTile(ModLoader.GetMod("Fargowiltas").TileType("CrucibleCosmosSheet"));

            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}