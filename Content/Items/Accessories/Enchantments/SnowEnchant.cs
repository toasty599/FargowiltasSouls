using FargowiltasSouls.Content.Buffs.Souls;
using FargowiltasSouls.Content.Projectiles;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Globals;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Accessories.Enchantments
{
    public class SnowEnchant : BaseEnchant
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            // DisplayName.SetDefault("Snow Enchantment");
            /* Tooltip.SetDefault(
@"Your attacks briefly inflict Frostburn
Press the Freeze Key to chill everything for 15 seconds
There is a 60 second cooldown for this effect
'It's Burning Cold Outside'"); */
        }

        public override Color nameColor => new(37, 195, 242);

        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.rare = ItemRarityID.Blue;
            Item.value = 50000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.AddEffect<SnowEffect>(Item);
        }

        public override void AddRecipes()
        {
            CreateRecipe()

            .AddIngredient(ItemID.EskimoHood)
            .AddIngredient(ItemID.EskimoCoat)
            .AddIngredient(ItemID.EskimoPants)
            //hand warmer
            //fruitcake chakram
            .AddIngredient(ItemID.IceBoomerang)
            .AddIngredient(ItemID.FrostMinnow)
            .AddIngredient(ItemID.AtlanticCod)

            .AddTile(TileID.DemonAltar)
            .Register();

            CreateRecipe()

            .AddIngredient(ItemID.EskimoHood)
            .AddIngredient(ItemID.EskimoCoat)
            .AddIngredient(ItemID.EskimoPants)
            //hand warmer
            //fruitcake chakram
            .AddIngredient(ItemID.IceBlade)
            .AddIngredient(ItemID.FrostMinnow)
            .AddIngredient(ItemID.AtlanticCod)

            .AddTile(TileID.DemonAltar)
            .Register();
        }
    }
    public class SnowEffect : AccessoryEffect
    {
        public override Header ToggleHeader => null;
        public override int ToggleItemType => ModContent.ItemType<SnowEnchant>();
        public override void OnHitNPCEither(Player player, NPC target, NPC.HitInfo hitInfo, DamageClass damageClass, int baseDamage, Projectile projectile, Item item)
        {
            target.AddBuff(BuffID.Frostburn, 120);
        }
        public override void PostUpdateEquips(Player player)
        {
            FargoSoulsPlayer modPlayer = player.FargoSouls();
            if (modPlayer.ChillSnowstorm)
            {
                modPlayer.SnowVisual = true;

                for (int i = 0; i < Main.maxProjectiles; i++)
                {
                    Projectile proj = Main.projectile[i];

                    if (proj.active && proj.hostile && proj.damage > 0 && FargoSoulsUtil.CanDeleteProjectile(proj) && !proj.FargoSouls().TimeFreezeImmune)
                    {
                        FargoSoulsGlobalProjectile globalProj = proj.FargoSouls();
                        globalProj.ChilledProj = true;
                        globalProj.ChilledTimer = 6;
                        proj.netUpdate = true;
                    }
                }

                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    NPC npc = Main.npc[i];

                    if (npc.active && !npc.friendly && npc.damage > 0 && !npc.dontTakeDamage && !npc.buffImmune[ModContent.BuffType<TimeFrozenBuff>()])
                    {
                        npc.AddBuff(BuffID.Frostburn, 2);
                        FargoSoulsGlobalNPC soulsNPC = npc.FargoSouls();
                        soulsNPC.SnowChilled = true;
                        if (soulsNPC.SnowChilledTimer <= 0)
                            soulsNPC.SnowChilledTimer = 6;
                        npc.netUpdate = true;
                    }
                }

                if (--modPlayer.chillLength <= 0)
                    modPlayer.ChillSnowstorm = false;

                const int warning = 180;
                if (modPlayer.chillLength <= warning && modPlayer.chillLength % 60 == 0)
                {
                    float rampup = MathHelper.Lerp(1.5f, 0.5f, (float)modPlayer.chillLength / warning);

                    SoundEngine.PlaySound(SoundID.Item27 with { Volume = rampup }, player.Center);

                    for (int i = 0; i < 20; i++)
                    {
                        int d = Dust.NewDust(player.position, player.width, player.height, DustID.GemSapphire, 0, 0, 0, default, 2f * rampup);
                        Main.dust[d].noGravity = true;
                        Main.dust[d].velocity *= 6f * rampup;
                    }
                }
            }
        }
    }
}
