using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using System.Collections.Generic;
using FargowiltasSouls.Toggler;
using System;

namespace FargowiltasSouls.Items.Accessories.Enchantments
{
    public class TinEnchant : BaseEnchant
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            DisplayName.SetDefault("Tin Enchantment");
            
            string tooltip =
@"Sets your critical strike chance to 5%
Every crit will increase it by 5% up to double your critical strike chance or 15%
Getting hit resets your crit to 5%
'Return of the Crit'";
            Tooltip.SetDefault(tooltip);

            DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "锡魔石");
            string tooltip_ch =
@"将你的基础暴击率设为5%
每次暴击时都会增加5%暴击率，增加的暴击率的最大值为你当前最大暴击率数值x2
被击中后会降低暴击率
'暴击回归'";
            Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, tooltip_ch);
        }

        protected override Color nameColor => new Color(162, 139, 78);

        public override void SetDefaults()
        {
            base.SetDefaults();
            
            Item.rare = ItemRarityID.Blue;
            Item.value = 30000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            TinEffect(player);
        }

        public static void TinEffect(Player player)
        {
            if (!player.GetToggleValue("Tin", false)) return;

            FargoSoulsPlayer modPlayer = player.GetModPlayer<FargoSoulsPlayer>();
            modPlayer.TinEnchantActive = true;
            
            //                if (SpiderEnchant && TinCritMax < SummonCrit * 2)
            //                    TinCritMax = SummonCrit * 2;

            //if (modPlayer.TinCrit > modPlayer.TinCritMax)
            //    modPlayer.TinCrit = modPlayer.TinCritMax;
            

            //                if (Eternity)
            //                {
            //                    if (eternityDamage > 47.5f)
            //                        eternityDamage = 47.5f;
            //                    AllDamageUp(eternityDamage);
            //                    player.statDefense += (int)(eternityDamage * 100); //10 defense per .1 damage
            //                }

            if (modPlayer.TinProcCD > 0)
                modPlayer.TinProcCD--;
        }

        //set max crit and current crit with no interference from accessory order
        public static void TinPostUpdate(FargoSoulsPlayer modPlayer)
        {
            modPlayer.TinCritMax = Math.Max((FargoSoulsUtil.HighestCritChance(modPlayer.Player) * 2), 15);

            if (modPlayer.TinCritMax > 100)
            {
                modPlayer.TinCritMax = 100;
            }

            FargoSoulsUtil.AllCritEquals(modPlayer.Player, modPlayer.TinCrit);
        }

        //increase crit
        public static void TinOnHitEnemy(FargoSoulsPlayer modPlayer, bool crit)
        {
            if (modPlayer.TinProcCD <= 0)
            {
                //                if (Eternity)
                //                {
                //                    if (crit && TinCrit < 100)
                //                    {
                //                        TinCrit += 10;
                //                    }
                //                    else if (TinCrit >= 100)
                //                    {
                //                        if (damage / 10 > 0 && !player.moonLeech)
                //                        {
                //                            player.statLife += damage / 10;
                //                            player.HealEffect(damage / 10);
                //                            int max = MutantNibble ? StatLifePrevious : player.statLifeMax2;
                //                            if (player.statLife > max)
                //                                player.statLife = max;
                //                        }

                //                        if (player.GetToggleValue("Eternity", false))
                //                        {
                //                            eternityDamage += .05f;
                //                        }
                //                    }
                //                }
                //                else if (TerrariaSoul)
                //                {
                //                    if (crit && TinCrit < 100)
                //                    {
                //                        TinCrit += 5;
                //                        tinCD = 15;
                //                    }
                //                    else if (TinCrit >= 100)
                //                    {
                //                        if (HealTimer <= 0 && damage / 25 > 0)
                //                        {
                //                            if (!player.moonLeech)
                //                            {
                //                                player.statLife += damage / 25;
                //                                player.HealEffect(damage / 25);
                //                                int max = MutantNibble ? StatLifePrevious : player.statLifeMax2;
                //                                if (player.statLife > max)
                //                                    player.statLife = max;
                //                            }
                //                            HealTimer = 10;
                //                        }
                //                        else
                //                        {
                //                            HealTimer--;
                //                        }
                //                    }
                //                }
                /*else */
                if (modPlayer.TinEnchantActive && crit && modPlayer.TinCrit < modPlayer.TinCritMax)
                {
                    //if (TerraForce)
                    //{
                    //    TinCrit += 5;
                    //    tinCD = 20;
                    //}
                    //else
                    //{
                    modPlayer.TinCrit += 5;
                    modPlayer.TinProcCD = 30;

                    //}

                    if (modPlayer.TinCrit > modPlayer.TinCritMax)
                        modPlayer.TinCrit = modPlayer.TinCritMax;

                    CombatText.NewText(modPlayer.Player.Hitbox, Color.OrangeRed, "" + modPlayer.TinCrit + "%", true);
                }
            }
        }

        //reset crit
        public static void TinHurt(FargoSoulsPlayer modPlayer)
        {
            //                if (Eternity)
            //                {
            //                    TinCrit = 50;
            //                    eternityDamage = 0;
            //                }
            //                else if (TerrariaSoul && TinCrit != 20)
            //                {
            //                    TinCrit = 20;
            //                }
            //                else if((TerraForce) && TinCrit != 10)
            //                {
            //                    TinCrit = 10;
            //                }
            /*else */
            int oldCrit = modPlayer.TinCrit;
            modPlayer.TinCrit = 5;
            int diff = oldCrit - modPlayer.TinCrit;
            if (diff > 0)
                CombatText.NewText(modPlayer.Player.Hitbox, Color.OrangeRed, $"-{diff}% crit", true);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.TinHelmet)
                .AddIngredient(ItemID.TinChainmail)
                .AddIngredient(ItemID.TinGreaves)
                .AddIngredient(ItemID.TinBow)
                .AddIngredient(ItemID.Musket)
                .AddIngredient(ItemID.PainterPaintballGun)
                .AddTile(TileID.DemonAltar)
                .Register();
        }
    }
}
