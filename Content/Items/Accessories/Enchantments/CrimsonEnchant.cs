using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using FargowiltasSouls.Content.Buffs.Souls;


namespace FargowiltasSouls.Content.Items.Accessories.Enchantments
{
	public class CrimsonEnchant : BaseEnchant
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            // DisplayName.SetDefault("Crimson Enchantment");
            /* Tooltip.SetDefault(
@"After taking a hit, regen is greatly increased until half the hit is healed off
If you take another hit before it's healed, the heal ends early
This does not affect hits dealing less than 10 damage
'The blood of your enemy is your rebirth'"); */
        }

        protected override Color nameColor => new(200, 54, 75);
        

        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.rare = ItemRarityID.Orange;
            Item.value = 50000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CrimsonEffect(player, Item);
        }

        public static void CrimsonEffect(Player player, Item item)
        {
            player.DisplayToggle("Crimson");
            FargoSoulsPlayer modPlayer = player.FargoSouls();
            modPlayer.CrimsonEnchantActive = true;
            modPlayer.CrimsonEnchantItem = item;
        }

        public static void CrimsonHurt(Player player, FargoSoulsPlayer modPlayer, ref Player.HurtModifiers modifiers)
        {
            //if was already healing, stop the heal and do nothing
            if (player.HasBuff(ModContent.BuffType<CrimsonRegenBuff>()))
            {
                player.ClearBuff(ModContent.BuffType<CrimsonRegenBuff>());
            }
            else
            {
                modifiers.ModifyHurtInfo += (ref Player.HurtInfo hurtInfo) =>
                {
                    if (hurtInfo.Damage < 10) return; //ignore hits under 10 damage

                    modPlayer.CrimsonRegenTime = 7 * 60; //how long crimson heal takes. 7 seconds

                    float returnHeal = 0.5f; //% of damage given back
                    modPlayer.CrimsonRegenAmount = (int)(hurtInfo.Damage * returnHeal); //50% return heal

                    modPlayer.CrimsonWizCharge = modPlayer.ForceEffect(modPlayer.CrimsonEnchantItem.type); //if crimson wiz active, set this to true

                    player.AddBuff(ModContent.BuffType<CrimsonRegenBuff>(), 2222); //should never reach that time lol. buff gets removed in buff itself after its done
                };
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ItemID.CrimsonHelmet)
            .AddIngredient(ItemID.CrimsonScalemail)
            .AddIngredient(ItemID.CrimsonGreaves)
            .AddIngredient(ItemID.TheUndertaker)
            .AddIngredient(ItemID.TheMeatball)
            .AddIngredient(ItemID.CrimsonHeart)

            .AddTile(TileID.DemonAltar)
            .Register();
        }
    }
}
