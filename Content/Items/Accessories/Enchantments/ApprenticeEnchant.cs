using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Accessories.Enchantments
{
	public class ApprenticeEnchant : BaseEnchant
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }

        protected override Color nameColor => new(93, 134, 166);

        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.rare = ItemRarityID.Yellow;
            Item.value = 150000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            ApprenticeEffect(player);
        }

        public static void ApprenticeEffect(Player player)
        {
            player.DisplayToggle("Apprentice");

            FargoSoulsPlayer modPlayer = player.FargoSouls();
            bool forceEffect = modPlayer.ForceEffect(ModContent.ItemType<ApprenticeEnchant>()) || modPlayer.ForceEffect(ModContent.ItemType<DarkArtistEnchant>());

            //update item cds
            for (int i = 0; i < 10; i++)
            {
                int itemCD = modPlayer.ApprenticeItemCDs[i];

                if (itemCD > 0)
                {
                    itemCD--;
                    modPlayer.ApprenticeItemCDs[i] = itemCD;
                }
            }
            if (player.GetToggleValue("Apprentice") && player.controlUseItem)
            {
                int numExtraSlotsToUse = 1;

                if (modPlayer.DarkArtistEnchantItem != null && forceEffect)
                {
                    numExtraSlotsToUse = 3;
                }
                else if (modPlayer.DarkArtistEnchantItem != null || forceEffect)
                {
                    numExtraSlotsToUse = 2;
                }
                

                if (player.controlUseItem)
                {
                    Item item = player.HeldItem;

                    //non weapons and weapons with no ammo begone
                    if (item.damage <= 0 || !player.HasAmmo(item) || (item.mana > 0 && player.statMana < item.mana)
                        || item.createTile != -1 || item.createWall != -1 || item.ammo != AmmoID.None || item.hammer != 0 || item.pick != 0 || item.axe != 0) return;

                    int startingSlot = 0;

                    //first we need to find what slot the current weapon is
                    for (int j = 0; j < 10; j++) //hotbar
                    {
                        Item item2 = player.inventory[j];

                        if (item2.type == item.type)
                        {
                            startingSlot = j;
                            break;
                        }
                    }

                    int weaponsUsed = 0;

                    //then go from there and find the next weapon to fire
                    for (int j = startingSlot; j < 10; j++) //hotbar
                    {
                        Item item2 = player.inventory[j];

                        if (item2 != null && item2.damage > 0 && item2.shoot > 0 && item2.ammo <= 0 && item.type != item2.type && !item2.channel)
                        {
                            if (!player.HasAmmo(item2) || (item2.mana > 0 && player.statMana < item2.mana) || item2.sentry || ContentSamples.ProjectilesByType[item2.shoot].minion)
                            {
                                continue;
                            }

                            weaponsUsed++;

                            if (weaponsUsed > numExtraSlotsToUse)
                            {
                                break;
                            }

                            int itemCD = modPlayer.ApprenticeItemCDs[j];

                            if (itemCD > 0)
                            {
                                continue;
                            }

                            Vector2 pos = new Vector2(player.Center.X + Main.rand.Next(-50, 50), player.Center.Y + Main.rand.Next(-50, 50));
                            Vector2 velocity = Vector2.Normalize(Main.MouseWorld - pos) * item2.shootSpeed;

                            int shoot = item2.shoot;

                            if (shoot == 10) //purification powder
                            {
                                float speed;
                                int damage;
                                float kb;
                                int usedAmmo;
                                player.PickAmmo(item2, out shoot, out speed, out damage, out kb, out usedAmmo);
                                ItemLoader.ModifyShootStats(item2, player, ref pos, ref velocity, ref shoot, ref damage, ref item2.knockBack);
                            }

                            if (item2.mana > 0)
                            {
                                if (player.CheckMana(item2.mana / 2, true, false))
                                {
                                    player.manaRegenDelay = 300;
                                }
                            }

                            int p = Projectile.NewProjectile(player.GetSource_ItemUse(item), pos, velocity, shoot, item2.damage / 2, item2.knockBack, player.whoAmI);
                            Projectile proj = Main.projectile[p];

                            if (item2.consumable)
                            {
                                item2.stack--;
                            }

                            //proj.usesLocalNPCImmunity = true;
                            //proj.localNPCHitCooldown = 5;
                            proj.noDropItem = true;

                            modPlayer.ApprenticeItemCDs[j] = item2.useTime * 4;
                        }
                    }
                }
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ItemID.ApprenticeHat)
            .AddIngredient(ItemID.ApprenticeRobe)
            .AddIngredient(ItemID.ApprenticeTrousers)
            .AddIngredient(ItemID.DD2FlameburstTowerT2Popper)
            //magic missile
            //ice rod
            //golden shower
            .AddIngredient(ItemID.BookStaff)
            .AddIngredient(ItemID.ClingerStaff)

            .AddTile(TileID.CrystalBall)
            .Register();
        }
    }
}
