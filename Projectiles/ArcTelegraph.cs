using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Graphics;
using System.Collections.Generic;
using FargowiltasSouls.NPCs.Challengers;
using Terraria.DataStructures;
using System.IO;

namespace FargowiltasSouls.Projectiles
{
    /// <summary>
    /// ai0 determines type, ai1 additional hook, velocity rotation, cone width and length determined by ai0
    /// </summary>
    public class ArcTelegraph : ModProjectile
    {
        public override string Texture => "Terraria/Images/Extra_" + ExtrasID.MartianProbeScanWave;//this is unrelated actually
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.DrawScreenCheckFluff[Projectile.type] = 99999999;
        }
        public override void SetDefaults()
        {
            Projectile.tileCollide = false;
        }

        int npc = -1;
        float coneWidth = 0;
        float coneLength = 0;
        public override void OnSpawn(IEntitySource source)
        {
            switch (Projectile.ai[0])
            {
                case 1: //lieflight area sweep
                    if (source is EntitySource_Parent parent && parent.Entity is NPC parentNpc && parentNpc.type == ModContent.NPCType<LifeChallenger>())
                    {
                        npc = parentNpc.whoAmI;
                        float angleToMe = (Projectile.Center - parentNpc.Center).ToRotation();
                        float angleToPlayer = (Main.player[parentNpc.target].Center - parentNpc.Center).ToRotation();
                        Projectile.localAI[1] = MathHelper.WrapAngle(angleToMe - angleToPlayer);
                    }
                    break;
                default:
                    break;
            }
        }
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write7BitEncodedInt(npc);
            writer.Write(Projectile.localAI[1]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            npc = reader.Read7BitEncodedInt();
            Projectile.localAI[1] = reader.ReadSingle();
        }
        public override void AI()
        {
            switch (Projectile.ai[0])
            {
                case 1: //lieflight area sweep
                    NPC parent = FargoSoulsUtil.NPCExists(npc);
                    if (parent != null)
                    {
                        Vector2 offset = Main.player[parent.target].Center - parent.Center;
                        Projectile.Center = parent.Center;
                        Projectile.velocity = offset.RotatedBy(Projectile.localAI[1]);
                    }
                    Projectile.position -= Projectile.velocity;
                    coneWidth = MathHelper.Pi * (20/48);
                    coneLength = 1000;
                    break;
                default:
                    break;
            }
        }
        public float WidthFunction(float progress)
        {
            return coneLength;
        }
        public float CollisionWidthFunction(float progress)
        {
            return WidthFunction(progress);
        }
        public Color ColorFunction(float progress)
        {
            return telegraphColor;//new Color(255,0,255,128) * 0.5f;
        }
        public void SetEffectParameters(Effect effect)
        {
            effect.Parameters["WorldViewProjection"].SetValue(GetWorldViewProjectionMatrixIdioticVertexShaderBoilerplate());
        }
        public static Matrix GetWorldViewProjectionMatrixIdioticVertexShaderBoilerplate()
        {
            Matrix view = Matrix.CreateLookAt(Vector3.Zero, Vector3.UnitZ, Vector3.Up) * Matrix.CreateTranslation(Main.graphics.GraphicsDevice.Viewport.Width / 2, Main.graphics.GraphicsDevice.Viewport.Height / -2, 0) * Matrix.CreateRotationZ(MathHelper.Pi) * Matrix.CreateScale(Main.GameViewMatrix.Zoom.X, Main.GameViewMatrix.Zoom.Y, 1f);
            Matrix projection = Matrix.CreateOrthographic(Main.graphics.GraphicsDevice.Viewport.Width, Main.graphics.GraphicsDevice.Viewport.Height, 0, 1000);
            return view * projection;
        }
        Effect shader;
        public Color telegraphColor = Color.White;
        /// <summary>
        /// ai0 determines type, ai1 additional hook, velocity rotation, cone width and length determined by ai0
        /// </summary>
        /// <param name="lightColor"></param>
        /// <returns></returns>
        public override bool PreDraw(ref Color lightColor)
        {
            float midrot = Projectile.velocity.ToRotation();

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            if(shader == null)
                shader = ModContent.Request<Effect>("FargowiltasSouls/Effects/Vertex_ArcTelegraph", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            SetEffectParameters(shader);
            shader.CurrentTechnique.Passes[0].Apply();
            VertexStrip vertexStrip = new();
            List<Vector2> positions = new();
            List<float> rotations = new();
            float increment = 0.01f;
            for (float i = 0; i < 1; i += increment)
            {
                float rotation = midrot - coneWidth * 0.5f + coneWidth * i;
                positions.Add(rotation.ToRotationVector2() * coneLength + Projectile.Center);
                //rotations.Add(rotation + MathHelper.PiOver2);
            } 
            vertexStrip.PrepareStrip(positions.ToArray(), rotations.ToArray(), ColorFunction, WidthFunction, -Main.screenPosition, includeBacksides: true);
            vertexStrip.DrawTrail();
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);         
            return false; 
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return false;
        }
    }
}
