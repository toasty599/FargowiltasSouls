using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Accessories.Souls
{
    [AutoloadEquip(EquipType.Shield)]
    public class TerrariaSoul : BaseSoul
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            // DisplayName.SetDefault("Soul of Terraria");

            // Tooltip.SetDefault(tooltip);

            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(6, 24));
            ItemID.Sets.AnimatesAsSoul[Item.type] = true;
        }

        public override bool PreDrawTooltipLine(DrawableTooltipLine line, ref int yOffset)
        {
            if (line.Mod == "Terraria" && line.Name == "ItemName")
            {
                Main.spriteBatch.End(); //end and begin main.spritebatch to apply a shader
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, null, null, null, null, null, Main.UIScaleMatrix);
                GameShaders.Armor.Apply(GameShaders.Armor.GetShaderIdFromItemId(ItemID.LivingRainbowDye), Item, null); //use living rainbow dye shader
                Utils.DrawBorderString(Main.spriteBatch, line.Text, new Vector2(line.X, line.Y), Color.White, 1); //draw the tooltip manually
                Main.spriteBatch.End(); //then end and begin again to make remaining tooltip lines draw in the default way
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, Main.UIScaleMatrix);
                return false;
            }
            return true;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.value = 5000000;
            Item.rare = -12;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            FargoSoulsPlayer modPlayer = player.FargoSouls();
            //includes revive, both spectres, adamantite, and star heal
            modPlayer.TerrariaSoul = true;

            //WOOD
            ModContent.Find<ModItem>(Mod.Name, "TimberForce").UpdateAccessory(player, hideVisual);
            //TERRA
            ModContent.Find<ModItem>(Mod.Name, "TerraForce").UpdateAccessory(player, hideVisual);
            //EARTH
            ModContent.Find<ModItem>(Mod.Name, "EarthForce").UpdateAccessory(player, hideVisual);
            //NATURE
            ModContent.Find<ModItem>(Mod.Name, "NatureForce").UpdateAccessory(player, hideVisual);
            //LIFE
            ModContent.Find<ModItem>(Mod.Name, "LifeForce").UpdateAccessory(player, hideVisual);
            //SPIRIT
            ModContent.Find<ModItem>(Mod.Name, "SpiritForce").UpdateAccessory(player, hideVisual);
            //SHADOW
            ModContent.Find<ModItem>(Mod.Name, "ShadowForce").UpdateAccessory(player, hideVisual);
            //WILL
            ModContent.Find<ModItem>(Mod.Name, "WillForce").UpdateAccessory(player, hideVisual);
            //COSMOS
            ModContent.Find<ModItem>(Mod.Name, "CosmoForce").UpdateAccessory(player, hideVisual);
        }

        public override void UpdateVanity(Player player)
        {
            player.FargoSouls().WoodEnchantDiscount = true;
        }

        public override void UpdateInventory(Player player)
        {
            player.FargoSouls().WoodEnchantDiscount = true;
            AshWoodEnchant.PassiveEffect(player);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(null, "TimberForce")
            .AddIngredient(null, "TerraForce")
            .AddIngredient(null, "EarthForce")
            .AddIngredient(null, "NatureForce")
            .AddIngredient(null, "LifeForce")
            .AddIngredient(null, "SpiritForce")
            .AddIngredient(null, "ShadowForce")
            .AddIngredient(null, "WillForce")
            .AddIngredient(null, "CosmoForce")
            .AddIngredient(null, "AbomEnergy", 10)

            .AddTile(ModContent.Find<ModTile>("Fargowiltas", "CrucibleCosmosSheet"))


            .Register();
        }
    }
}
