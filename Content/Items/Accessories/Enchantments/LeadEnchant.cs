using FargowiltasSouls.Content.Buffs.Souls;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Toggler.Content;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Accessories.Enchantments
{
	public class LeadEnchant : BaseEnchant
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            // DisplayName.SetDefault("Lead Enchantment");

            string tooltip =
@"You take 40% less from damage over time
Attacks inflict enemies with Lead Poisoning
Lead Poisoning deals damage over time and spreads to nearby enemies
'Not recommended for eating'";
            // Tooltip.SetDefault(tooltip);
        }

        protected override Color nameColor => new(67, 69, 88);
        

        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.rare = ItemRarityID.Blue;
            Item.value = 20000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.AddEffect<LeadEffect>(Item);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.LeadHelmet)
                .AddIngredient(ItemID.LeadChainmail)
                .AddIngredient(ItemID.LeadGreaves)
                .AddIngredient(ItemID.LeadShortsword)
                .AddIngredient(ItemID.GrayPaint, 100)
                .AddIngredient(ItemID.Peach)

            .AddTile(TileID.DemonAltar)
            .Register();
        }
    }
    public class LeadEffect : AccessoryEffect
    {
        public override bool HasToggle => true;
        public override Header ToggleHeader => Header.GetHeader<TerraHeader>();
        public override void OnHitNPCEither(Player player, NPC target, NPC.HitInfo hitInfo, DamageClass damageClass, int baseDamage, Projectile projectile, Item item)
        {
            target.AddBuff(ModContent.BuffType<LeadPoisonBuff>(), 30);
        }
        public static void ProcessLeadEffectLifeRegen(Player player)
        {
            if (player.HasEffect<LeadEffect>())
            {
                if (player.FargoSouls().ForceEffect<LeadEnchant>())
                {
                    player.lifeRegen = (int)(player.lifeRegen * 0.4f);
                }
                else
                {
                    player.lifeRegen = (int)(player.lifeRegen * 0.6f);
                }
            }
            
        }
    }
}
