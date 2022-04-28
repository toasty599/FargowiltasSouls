using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using FargowiltasSouls.Toggler;
using FargowiltasSouls.Items.Accessories.Enchantments;

namespace FargowiltasSouls.Items.Accessories.Forces
{
    public class EarthForce : BaseForce
    {
        public static int[] Enchants => new int[]
        {
            ModContent.ItemType<CobaltEnchant>(),
            ModContent.ItemType<PalladiumEnchant>(),
            ModContent.ItemType<MythrilEnchant>(),
            ModContent.ItemType<OrichalcumEnchant>(),
            ModContent.ItemType<AdamantiteEnchant>(),
            ModContent.ItemType<TitaniumEnchant>()
        };

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            Tooltip.SetDefault(Language.GetTextValue("Mods.FargowiltasSouls.ItemExtra.EarthForce", Enchants[0], Enchants[1], Enchants[2], Enchants[3], Enchants[4], Enchants[5]));

            //             DisplayName.SetDefault("Force of Earth");

            //             Tooltip.SetDefault(
            // $"[i:{ModContent.ItemType<CobaltEnchant>()}] 25% chance for your projectiles to explode into shards\n" +
            // $"[i:{ModContent.ItemType<MythrilEnchant>()}] 20% increased weapon use speed\n" +
            // $"[i:{ModContent.ItemType<PalladiumEnchant>()}] Greatly increases life regeneration after striking an enemy\n" +
            // $"[i:{ModContent.ItemType<PalladiumEnchant>()}] You spawn an orb of damaging life energy every 80 life regenerated\n" +
            // $"[i:{ModContent.ItemType<OrichalcumEnchant>()}] Flower petals will cause extra damage to your target\n" +
            // $"[i:{ModContent.ItemType<OrichalcumEnchant>()}] Damaging debuffs deal 5x damage\n" +
            // $"[i:{ModContent.ItemType<AdamantiteEnchant>()}] One of your projectiles will split into 3 every 3/4 of a second\n" +
            // $"[i:{ModContent.ItemType<TitaniumEnchant>()}] Briefly become invulnerable after striking an enemy\n" +
            // "'Gaia's blessing shines upon you'");

            //             DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "大地之力");
            //             Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, 
            // @"你的弹幕有25%几率爆裂成碎片
            // 增加20%武器使用速度
            // 攻击敌人后大幅增加生命恢复速度
            // 你每恢复80点生命值便会生成一个伤害性的生命能量球
            // 花瓣将落到被你攻击的敌人的身上以造成额外伤害
            // 伤害性减益造成的伤害x5
            // 每过3/4秒便会随机使你的一个弹幕分裂成三个
            // 攻击敌人后会使你无敌一小段时间
            // '盖亚的祝福照耀着你'");
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            FargoSoulsPlayer modPlayer = player.GetModPlayer<FargoSoulsPlayer>();
            modPlayer.EarthForce = true;
            //mythril
            if (player.GetToggleValue("Mythril"))
            {
                modPlayer.MythrilEnchantActive = true;
                if (!modPlayer.DisruptedFocus)
                    modPlayer.AttackSpeed += .2f;
            }
            //shards
            modPlayer.CobaltEnchantActive = true;
            //regen on hit, heals
            modPlayer.PalladiumEffect();
            //fireballs and petals
            modPlayer.OrichalcumEffect();
            AdamantiteEnchant.AdamantiteEffect(player);
            TitaniumEnchant.TitaniumEffect(player);
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            foreach (int ench in Enchants)
                recipe.AddIngredient(ench);
            recipe.AddTile(ModContent.Find<ModTile>("Fargowiltas", "CrucibleCosmosSheet"));
            recipe.Register();
        }
    }
}
