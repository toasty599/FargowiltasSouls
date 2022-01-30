using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Items.Weapons.FinalUpgrades
{
    public class SparklingLove : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sparkling Love");
            Tooltip.SetDefault(@"Right click to summon the soul of Deviantt
Right click pattern becomes denser with up to 12 empty minion slots
'The soul-consuming demon axe of love and justice from a defeated foe...'");
        }

        public override void SetDefaults()
        {
            item.damage = 1700;
            item.useStyle = ItemUseStyleID.Swing;
            item.useAnimation = 27;
            item.useTime = 27;
            item.shootSpeed = 16f;
            item.knockBack = 14f;
            item.width = 32;
            item.height = 32;
            item.scale = 2f;
            item.rare = ItemRarityID.Purple;
            item.UseSound = SoundID.Item1;
            item.shoot = ModContent.ProjectileType<Projectiles.BossWeapons.SparklingLove>();
            item.value = Item.sellPrice(0, 70);
            item.noMelee = true; //no melee hitbox
            item.noUseGraphic = true; //dont draw item
            Item.DamageType = DamageClass.Melee;
            item.autoReuse = true;
        }

        public override bool AltFunctionUse(Player player)
        {
            return true;
        }

        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                item.shoot = ModContent.ProjectileType<Projectiles.BossWeapons.SparklingDevi>();
                item.useStyle = ItemUseStyleID.Swing;
                Item.DamageType = DamageClass.Summon;
                item.melee = false;
                item.noUseGraphic = false;
                item.noMelee = false;
                item.useAnimation = 66;
                item.useTime = 66;
                item.mana = 100;
            }
            else
            {
                item.shoot = ModContent.ProjectileType<Projectiles.BossWeapons.SparklingLove>();
                item.useStyle = ItemUseStyleID.Swing;
                item.summon = false;
                Item.DamageType = DamageClass.Melee;
                item.noUseGraphic = true;
                item.noMelee = true;
                item.useAnimation = 27;
                item.useTime = 27;
                item.mana = 0;
            }
            return true;
        }

        public override bool PreDrawTooltipLine(DrawableTooltipLine line, ref int yOffset)
        {
            if (line.mod == "Terraria" && line.Name == "ItemName")
            {
                Main.spriteBatch.End(); //end and begin main.spritebatch to apply a shader
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, null, null, null, null, null, Main.UIScaleMatrix);
                var lineshader = GameShaders.Misc["PulseCircle"].UseColor(new Color(255, 48, 154)).UseSecondaryColor(new Color(255, 169, 240));
                lineshader.Apply(null);
                Utils.DrawBorderString(Main.spriteBatch, line.text, new Vector2(line.X, line.Y), new Color(255, 169, 240), 1); //draw the tooltip manually
                Main.spriteBatch.End(); //then end and begin again to make remaining tooltip lines draw in the default way
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, Main.UIScaleMatrix);
                return false;
            }
            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()

            //.AddIngredient(ModLoader.GetMod("Fargowiltas").ItemType("EnergizerMoon"));
            .AddIngredient(ModContent.ItemType<Sadism>(), 30);
            .AddIngredient(ModContent.ItemType<AbomEnergy>(), 30);
            .AddIngredient(ModContent.ItemType<DeviatingEnergy>(), 30);
            .AddIngredient(ModContent.ItemType<BrokenBlade>());
            .AddIngredient(ModContent.ItemType<SparklingAdoration>());

            recipe.AddTile(ModLoader.GetMod("Fargowiltas").TileType("CrucibleCosmosSheet"));
            recipe.SetResult(this);
            .Register();
        }
    }
}