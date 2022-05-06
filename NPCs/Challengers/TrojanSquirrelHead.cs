using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using Microsoft.Xna.Framework.Graphics;
using FargowiltasSouls.Buffs.Masomode;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using FargowiltasSouls.ItemDropRules.Conditions;
using Terraria.DataStructures;
using System.IO;
using FargowiltasSouls.Items.Summons;

namespace FargowiltasSouls.NPCs.Challengers
{
    public class TrojanSquirrelHead : TrojanSquirrelPart
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            NPCID.Sets.NoMultiplayerSmoothingByType[NPC.type] = true;
            NPCID.Sets.CantTakeLunchMoney[Type] = true;

            NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, new NPCID.Sets.NPCBestiaryDrawModifiers(0)
            {
                Hide = true
            });
        }

        public override void SetDefaults()
        {
            base.SetDefaults();

            NPC.lifeMax = 100;
            NPC.width = 80;
            NPC.height = 76;

            NPC.hide = true;
        }

        public override void DrawBehind(int index)
        {
            Main.instance.DrawCacheNPCProjectiles.Add(index);
        }

        NPC body;

        public override void OnSpawn(IEntitySource source)
        {
            base.OnSpawn(source);

            if (source is EntitySource_Parent parent && parent.Entity is NPC sourceNPC)
                body = sourceNPC;
        }

        public override void AI()
        {
            base.AI();

            if (body != null)
                body = FargoSoulsUtil.NPCExists(body.whoAmI, ModContent.NPCType<TrojanSquirrel>());

            if (body == null)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    NPC.life = 0;
                    if (Main.netMode == NetmodeID.Server)
                        NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, NPC.whoAmI);
                    NPC.active = false;
                }
                return;
            }

            NPC.target = body.target;
            NPC.direction = NPC.spriteDirection = body.direction;
            NPC.Center = body.Bottom + new Vector2(42f * NPC.direction, -145f) * body.scale;

            switch ((int)NPC.ai[0])
            {
                case 0:
                    {
                        if (body.ai[0] == 0 && body.localAI[0] <= 0)
                        {
                            NPC.ai[1] += FargoSoulsWorld.EternityMode ? 1.5f : 1f;

                            if (body.dontTakeDamage)
                                NPC.ai[1] += 1f;
                        }

                        int threshold = 360;
                        if (NPC.ai[1] > threshold && Math.Abs(body.velocity.Y) < 0.05f)
                        {
                            NPC.ai[0] = 1;
                            NPC.ai[1] = 0;
                            NPC.netUpdate = true;
                        }
                    }
                    break;

                case 1:
                    if (++NPC.ai[1] % (body.dontTakeDamage || FargoSoulsWorld.MasochistModeReal ? 45 : 60) == 0)
                    {
                        const float gravity = 0.2f;
                        float time = FargoSoulsWorld.MasochistModeReal ? 60f : 90f;
                        Vector2 distance = Main.player[NPC.target].Center - NPC.Center;// + player.velocity * 30f;
                        distance.X = distance.X / time;
                        distance.Y = distance.Y / time - 0.5f * gravity * time;
                        for (int i = 0; i < 10; i++)
                        {
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, distance + Main.rand.NextVector2Square(-0.5f, 0.5f),
                                ModContent.ProjectileType<Projectiles.Champions.TimberAcorn>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 0f, Main.myPlayer);
                        }
                    }

                    if (NPC.ai[1] > 210)
                    {
                        NPC.ai[0] = 0;
                        NPC.ai[1] = 0;
                        NPC.netUpdate = true;
                    }
                    break;
            }
        }

        public override bool CheckActive() => false;

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            base.ModifyNPCLoot(npcLoot);

            npcLoot.Add(ItemDropRule.DropNothing());
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            if (NPC.life <= 0)
            {
                Vector2 pos = NPC.Center;
                if (!Main.dedServ)
                    Gore.NewGore(NPC.GetSource_FromThis(), pos, NPC.velocity, ModContent.Find<ModGore>(Mod.Name, $"TrojanSquirrelGore1").Type, NPC.scale);
            }
        }

        public override void FindFrame(int frameHeight)
        {
            base.FindFrame(frameHeight);

            if (body != null)
                NPC.frame = body.frame;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Npc[NPC.type].Value;
            Rectangle rectangle = NPC.frame;
            Vector2 origin2 = rectangle.Size() / 2f;

            Color color26 = drawColor;
            color26 = NPC.GetAlpha(color26);

            SpriteEffects effects = NPC.direction < 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Vector2 center = body == null ? NPC.Center : body.Center;

            Main.EntitySpriteDraw(texture2D13, center - Main.screenPosition + new Vector2(0f, NPC.gfxOffY - 45), new Microsoft.Xna.Framework.Rectangle?(rectangle), color26, NPC.rotation, origin2, NPC.scale, effects, 0);

            return false;
        }
    }
}
