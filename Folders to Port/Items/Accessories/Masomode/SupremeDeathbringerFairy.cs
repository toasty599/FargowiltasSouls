using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using FargowiltasSouls.Toggler;

namespace FargowiltasSouls.Items.Accessories.Masomode
{
    [AutoloadEquip(EquipType.Shield)]
    public class SupremeDeathbringerFairy : SoulsItem
    {
        public override bool Eternity => true;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Supreme Deathbringer Fairy");
            Tooltip.SetDefault(@"Grants immunity to Slimed, Berserked, Lethargic, and Infested
Increases damage by 10% and armor penetration by 10
15% increased fall speed
When you land after a jump, slime will fall from the sky over your cursor
While dashing or running quickly you will create a trail of blood scythes
Your attacks inflict Venom and spray honey that increases your life regeneration
Bees and weak Hornets become friendly
Summons 2 Skeletron arms to whack enemies
'Supremacy not necessarily guaranteed'");
            DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "至高告死精灵");
            Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, @"'霸权不一定能得到保证'
免疫黏糊, 狂暴, 昏昏欲睡和感染
增加10%伤害, 增加10点护甲穿透
增加15%掉落速度
跳跃落地后, 在光标处落下史莱姆
冲刺或快速奔跑时发射一串血镰
攻击造成毒液效果
蜜蜂和虚弱黄蜂变得友好
永久蜂蜜Buff效果
召唤2个骷髅王手臂重击敌人");
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.accessory = true;
            item.rare = ItemRarityID.Pink;
            item.value = Item.sellPrice(0, 4);
            item.defense = 2;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            FargoSoulsPlayer fargoPlayer = player.GetModPlayer<FargoSoulsPlayer>();
            fargoPlayer.SupremeDeathbringerFairy = true;

            //slimy shield
            player.buffImmune[BuffID.Slimed] = true;

            if (player.GetToggleValue("SlimeFalling"))
            {
                player.maxFallSpeed *= 1.5f;
            }

            if (player.GetToggleValue("MasoSlime"))
            {
                player.GetModPlayer<FargoSoulsPlayer>().SlimyShield = true;
            }

            //agitating lens
            player.buffImmune[mod.BuffType("Berserked")] = true;
            fargoPlayer.AllDamageUp(.10f);
            fargoPlayer.AgitatingLens = true;

            //queen stinger
            player.buffImmune[mod.BuffType("Infested")] = true;
            //player.honey = true;
            player.armorPenetration += 10;
            player.npcTypeNoAggro[210] = true;
            player.npcTypeNoAggro[211] = true;
            player.npcTypeNoAggro[42] = true;
            player.npcTypeNoAggro[231] = true;
            player.npcTypeNoAggro[232] = true;
            player.npcTypeNoAggro[233] = true;
            player.npcTypeNoAggro[234] = true;
            player.npcTypeNoAggro[235] = true;
            fargoPlayer.QueenStinger = true;

            //necromantic brew
            player.buffImmune[mod.BuffType("Lethargic")] = true;
            fargoPlayer.NecromanticBrew = true;
            if (player.GetToggleValue("MasoSkele"))
                player.AddBuff(mod.BuffType("SkeletronArms"), 2);
        }

        public override void AddRecipes()
        {
            CreateRecipe()

            .AddIngredient(mod.ItemType("SlimyShield"));
            .AddIngredient(mod.ItemType("AgitatingLens"));
            .AddIngredient(mod.ItemType("QueenStinger"));
            .AddIngredient(mod.ItemType("NecromanticBrew"));
            .AddIngredient(ItemID.HellstoneBar, 10);
            .AddIngredient(mod.ItemType("DeviatingEnergy"), 5);

            recipe.AddTile(TileID.DemonAltar);
            recipe.SetResult(this);
            .Register();
        }
    }
}