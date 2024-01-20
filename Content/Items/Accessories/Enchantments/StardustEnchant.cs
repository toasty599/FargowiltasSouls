using FargowiltasSouls.Common.Graphics.Shaders;
using FargowiltasSouls.Content.Buffs.Souls;
using FargowiltasSouls.Content.Projectiles;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Toggler.Content;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Accessories.Enchantments
{
	public class StardustEnchant : BaseEnchant
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            // DisplayName.SetDefault("Stardust Enchantment");
            /* Tooltip.SetDefault(
@"A stardust guardian will protect you from nearby enemies
Press the Freeze Key to freeze time for 5 seconds
While time is frozen, your minions will continue to attack
There is a 60 second cooldown for this effect
'The power of the Stand is yours'"); */
            //             DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "星尘魔石");
            //             Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, 
            // @"双击'下'键将你的守卫引至光标位置
            // 按下'冻结'键后会冻结5秒时间
            // 你的召唤物不受时间冻结影响且星尘守卫在时间冻结期间获得全新的攻击
            // 此效果有60秒冷却时间
            // '替身之力归你所有'");
        }

        public override Color nameColor => new(0, 174, 238);
        

        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.rare = ItemRarityID.Red;
            Item.value = 400000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.AddEffect<StardustMinionEffect>(Item);
            player.AddEffect<StardustEffect>(Item);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ItemID.StardustHelmet)
            .AddIngredient(ItemID.StardustBreastplate)
            .AddIngredient(ItemID.StardustLeggings)
            //stardust wings
            //.AddIngredient(ItemID.StardustPickaxe);
            .AddIngredient(ItemID.StardustCellStaff) //estee pet
            .AddIngredient(ItemID.StardustDragonStaff)
            .AddIngredient(ItemID.RainbowCrystalStaff)
            //MoonlordTurretStaff


            .AddTile(TileID.LunarCraftingStation)
            .Register();
        }
    }
    public class StardustMinionEffect : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<CosmoHeader>();
        public override int ToggleItemType => ModContent.ItemType<StardustEnchant>();
        public override bool MinionEffect => true;
        public override void PostUpdateEquips(Player player)
        {
            if (player.whoAmI == Main.myPlayer && player.ownedProjectileCounts[ProjectileID.StardustGuardian] < 1)
            {
                FargoSoulsUtil.NewSummonProjectile(GetSource_EffectItem(player), player.Center, Vector2.Zero, ProjectileID.StardustGuardian, 30, 10f, Main.myPlayer);
            }
        }
    }
    public class StardustEffect : AccessoryEffect
    {
        public override Header ToggleHeader => null;
        public const int TIMESTOP_DURATION = 540; //300
        public override void PostUpdateEquips(Player player)
        {
            player.setStardust = true;
            FargoSoulsPlayer modPlayer = player.FargoSouls();
            if (modPlayer.FreezeTime && modPlayer.freezeLength > 0)
            {
                player.buffImmune[ModContent.BuffType<TimeFrozenBuff>()] = true;

                if (Main.netMode != NetmodeID.Server)
                    ShaderManager.GetFilterIfExists("Invert").SetFocusPosition(player.Center);

                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    NPC npc = Main.npc[i];
                    if (npc.active && !npc.HasBuff(ModContent.BuffType<TimeFrozenBuff>()))
                        npc.AddBuff(ModContent.BuffType<TimeFrozenBuff>(), modPlayer.freezeLength);
                }

                for (int i = 0; i < Main.maxProjectiles; i++)
                {
                    Projectile p = Main.projectile[i];
                    FargoSoulsGlobalProjectile globalProj = p.FargoSouls();
                    if (p.active && !(p.minion && !ProjectileID.Sets.MinionShot[p.type]) && !globalProj.TimeFreezeImmune && globalProj.TimeFrozen == 0)
                    {
                        globalProj.TimeFrozen = modPlayer.freezeLength;

                        /*if (p.owner == Player.whoAmI && p.friendly && !p.hostile)
                        {
                            //p.maxPenetrate = 1;
                            if (!p.usesLocalNPCImmunity && !p.usesIDStaticNPCImmunity)
                            {
                                p.usesLocalNPCImmunity = true;
                                p.localNPCHitCooldown = 1;
                            }
                        }*/
                    }
                }

                modPlayer.freezeLength--;

                FargowiltasSouls.ManageMusicTimestop(modPlayer.freezeLength < 5);

                if (modPlayer.freezeLength == 90)
                {
                    if (!Main.dedServ)
                        SoundEngine.PlaySound(new SoundStyle("FargowiltasSouls/Assets/Sounds/ZaWarudoResume"), player.Center);
                }

                if (modPlayer.freezeLength <= 0)
                {
                    modPlayer.FreezeTime = false;
                    modPlayer.freezeLength = TIMESTOP_DURATION;

                    for (int i = 0; i < Main.maxNPCs; i++)
                    {
                        NPC npc = Main.npc[i];
                        if (npc.active && !npc.dontTakeDamage && npc.life == 1 && npc.lifeMax > 1)
                            npc.SimpleStrikeNPC(int.MaxValue, 0, false, 0, null, false, 0, true);
                    }
                }
            }
        }
    }
}
