using FargowiltasSouls.Content.Projectiles;
using Microsoft.CodeAnalysis;
using Microsoft.Xna.Framework;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Accessories.Enchantments
{
	public class HallowEnchant : BaseEnchant
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            // DisplayName.SetDefault("Hallowed Enchantment");

            /* Tooltip.SetDefault(
@"Become immune after striking an enemy
'Hit me with your best shot'"); */
            //             DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "神圣魔石");
            //             Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, 
            // @"使你获得一面可以反弹弹幕的盾牌
            // 召唤一柄附魔剑，附魔剑的伤害取决于你的召唤伤害
            // '愿人都尊你的剑与盾为圣'");
        }

        protected override Color nameColor => new(150, 133, 100);
        

        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.rare = ItemRarityID.LightPurple;
            Item.value = 180000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            HallowEffect(player, Item);
        }

        public static void HallowEffect(Player player, Item item)
        {
            player.DisplayToggle("HallowDodge");

            if (player.GetToggleValue("HallowDodge"))
            {
                player.FargoSouls().HallowEnchantItem = item;
            }
                //player.onHitDodge = true;
        }
        public const int RepelRadius = 350;
        public static void HealRepel(Player player, Item item)
        {
            SoundEngine.PlaySound(SoundID.Item72);
            Projectile.NewProjectile(player.GetSource_Accessory(item), player.Center, Vector2.Zero, ModContent.ProjectileType<GlowRing>(), 0, 0f, Main.myPlayer, player.whoAmI, -25);
            
            foreach (Projectile projectile in Main.projectile.Where(p => p.hostile && FargoSoulsUtil.CanDeleteProjectile(p) && p.Distance(player.Center) <= RepelRadius))
            {
                projectile.velocity = Vector2.Normalize(projectile.Center - player.Center) * projectile.velocity.Length();
                projectile.hostile = false;
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddRecipeGroup("FargowiltasSouls:AnyHallowHead")
                .AddIngredient(ItemID.HallowedPlateMail)
                .AddIngredient(ItemID.HallowedGreaves)
                .AddIngredient(ItemID.HallowJoustingLance)
                .AddIngredient(ItemID.RainbowRod)
                .AddIngredient(ItemID.MajesticHorseSaddle)

            .AddTile(TileID.CrystalBall)
            .Register();
        }
    }
}
