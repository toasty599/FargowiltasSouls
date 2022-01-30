using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using FargowiltasSouls.Toggler;

namespace FargowiltasSouls.Items.Accessories.Masomode
{
    public class LumpOfFlesh : SoulsItem
    {
        public override bool Eternity => true;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Lump of Flesh");
            Tooltip.SetDefault(@"Grants immunity to Blackout, Obstructed, Dazed, and Stunned
Increases minion damage by 16% but slightly decreases defense
Increases your max number of minions by 2
Increases your max number of sentries by 2
The pungent eyeball charges energy to fire a laser as you attack
Enemies are less likely to target you
'It's growing'");
            DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "肉团");
            Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, @"'它在增长'
免疫致盲,阻塞和眩晕
增加16%召唤伤害,但略微减少防御
+2最大召唤栏
+2最大哨兵栏
当你攻击时,尖刻眼球会充能来发射激光
敌人不太可能以你为目标
地牢外的装甲和魔法骷髅敌意减小");
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.accessory = true;
            item.rare = ItemRarityID.Cyan;
            item.value = Item.sellPrice(0, 7);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.buffImmune[BuffID.Blackout] = true;
            player.buffImmune[BuffID.Obstructed] = true;
            player.buffImmune[BuffID.Dazed] = true;
            player.buffImmune[ModContent.BuffType<Buffs.Masomode.Stunned>()] = true;
            player.minionDamage += 0.16f;
            player.statDefense -= 6;
            player.aggro -= 400;
            player.GetModPlayer<FargoSoulsPlayer>().SkullCharm = true;
            /*if (!player.ZoneDungeon)
            {
                player.npcTypeNoAggro[NPCID.SkeletonSniper] = true;
                player.npcTypeNoAggro[NPCID.SkeletonCommando] = true;
                player.npcTypeNoAggro[NPCID.TacticalSkeleton] = true;
                player.npcTypeNoAggro[NPCID.DiabolistRed] = true;
                player.npcTypeNoAggro[NPCID.DiabolistWhite] = true;
                player.npcTypeNoAggro[NPCID.Necromancer] = true;
                player.npcTypeNoAggro[NPCID.NecromancerArmored] = true;
                player.npcTypeNoAggro[NPCID.RaggedCaster] = true;
                player.npcTypeNoAggro[NPCID.RaggedCasterOpenCoat] = true;
            }*/
            player.maxMinions += 2;
            player.maxTurrets += 2;
            if (player.GetToggleValue("MasoPugent"))
            {
                player.buffImmune[ModContent.BuffType<CrystalSkull>()] = true;
                player.AddBuff(ModContent.BuffType<PungentEyeball>(), 5);
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe()

            .AddIngredient(ModContent.ItemType<PungentEyeball>())
            .AddIngredient(ModContent.ItemType<SkullCharm>())
            .AddIngredient(ItemID.SpectreBar, 10)
            .AddIngredient(ModContent.ItemType<DeviatingEnergy>(), 10)

            .AddTile(TileID.MythrilAnvil)
            
            .Register();
        }
    }
}