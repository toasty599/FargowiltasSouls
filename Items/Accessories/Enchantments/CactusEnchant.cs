using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using FargowiltasSouls.Projectiles.Souls;
using FargowiltasSouls.Projectiles;
using FargowiltasSouls.Toggler;

namespace FargowiltasSouls.Items.Accessories.Enchantments
{
    public class CactusEnchant : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cactus Enchantment");
            Tooltip.SetDefault(
@"25% of contact damage is reflected
Enemies may explode into needles on death

Spawn rolling cacti (not real)
Hit a rolling cacti into enemies to impale them with spikes
The spikes will explode from them when killed or after some time (current effect)

'It's the quenchiest!'");

            DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "仙人掌魔石");
            Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, 
@"反弹25%接触伤害
敌人死亡时有几率爆裂出针刺
'太解渴了！'");
        }

        public override void SafeModifyTooltips(List<TooltipLine> list)
        {
            foreach (TooltipLine tooltipLine in list)
            {
                if (tooltipLine.mod == "Terraria" && tooltipLine.Name == "ItemName")
                {
                    tooltipLine.overrideColor = new Color(121, 158, 29);
                }
            }
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.accessory = true;
            ItemID.Sets.ItemNoGravity[Item.type] = true;
            Item.rare = ItemRarityID.Green;
            Item.value = 20000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CactusEffect(player);
        }

        public static void CactusEffect(Player player)
        {
            player.thorns = .25f;

            if (player.GetToggleValue("Cactus"))
            {
                player.GetModPlayer<FargoSoulsPlayer>().CactusEnchantActive = true;
            }
        }

        public static void CactusProc(NPC npc, Player player)
        {
            FargoSoulsPlayer modPlayer = player.GetModPlayer<FargoSoulsPlayer>();
            int dmg = 20;
            int numNeedles = 8;

            if (modPlayer.LifeForce || modPlayer.WizardEnchantActive)
            {
                dmg = 75;
                numNeedles = 16;
            }

            Projectile[] projs = FargoSoulsUtil.XWay(numNeedles, player.GetProjectileSource_Item(player.HeldItem), npc.Center, /*ModContent.ProjectileType<CactusNeedle>()*/ProjectileID.RollingCactusSpike, 4, FargoSoulsUtil.HighestDamageTypeScaling(player, dmg), 5f);

            for (int i = 0; i < projs.Length; i++)
            {
                if (projs[i] == null) continue;
                Projectile p = projs[i];
                p.GetGlobalProjectile<FargoSoulsGlobalProjectile>().CanSplit = false;
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.CactusHelmet)
                .AddIngredient(ItemID.CactusBreastplate)
                .AddIngredient(ItemID.CactusLeggings)
                .AddIngredient(ItemID.Waterleaf)
                .AddIngredient(ItemID.Flounder)
                .AddIngredient(ItemID.SecretoftheSands)
                .AddTile(TileID.DemonAltar)
                .Register();
        }
    }
}
