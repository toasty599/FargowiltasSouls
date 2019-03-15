using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Items.Accessories.Essences
{
    public class SnipersEssence : ModItem
    {
        private readonly Mod thorium = ModLoader.GetMod("ThoriumMod");

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sniper's Essence");
            Tooltip.SetDefault(
@"'This is only the beginning..'
18% increased ranged damage
5% increased ranged critical chance
5% increased firing speed");
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.accessory = true;
            item.value = 150000;
            item.rare = 4;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.rangedDamage += .18f;
            player.rangedCrit += 5;
            player.GetModPlayer<FargoPlayer>(mod).RangedEssence = true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);

            if (Fargowiltas.Instance.ThoriumLoaded)
            {
                //just thorium
                recipe.AddIngredient(ItemID.RangerEmblem);
                recipe.AddIngredient(ItemID.PainterPaintballGun);
                recipe.AddIngredient(ItemID.SnowballCannon);
                recipe.AddIngredient(ItemID.RedRyder);
                recipe.AddIngredient(ItemID.Harpoon);
                recipe.AddIngredient(ItemID.Musket);
                recipe.AddIngredient(thorium.ItemType("GuanoGunner"));
                recipe.AddIngredient(thorium.ItemType("SharkStorm"));
                recipe.AddIngredient(ItemID.BeesKnees);
                recipe.AddIngredient(thorium.ItemType("EnergyStormBolter"));
                recipe.AddIngredient(thorium.ItemType("HeroTripleBow"));
                recipe.AddIngredient(thorium.ItemType("HitScanner"));
                recipe.AddIngredient(thorium.ItemType("RangedThorHammer"));
                recipe.AddIngredient(ItemID.HellwingBow);
            }
            else
            {
                //no others
                recipe.AddIngredient(ItemID.RangerEmblem);
                recipe.AddIngredient(ItemID.PainterPaintballGun);
                recipe.AddIngredient(ItemID.SnowballCannon);
                recipe.AddIngredient(ItemID.RedRyder);
                recipe.AddIngredient(ItemID.Harpoon);
                recipe.AddIngredient(ItemID.Musket);
                recipe.AddIngredient(ItemID.Boomstick);
                recipe.AddIngredient(ItemID.BeesKnees);
                recipe.AddIngredient(ItemID.HellwingBow);
            }

            recipe.AddTile(TileID.TinkerersWorkbench);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
