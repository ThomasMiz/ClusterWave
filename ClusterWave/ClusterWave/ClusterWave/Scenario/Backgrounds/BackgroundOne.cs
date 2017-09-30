﻿using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace ClusterWave.Scenario.Backgrounds
{
    class BackgroundOne : Background
    {
        public static Texture2D shapeTexture;
        public static Effect RayFx, LinesFx, TextureFx;

        public static Texture2D BackgroundTexture;
        public static Effect BackgroundFx;
        private static EffectParameter worldParameter, viewParameter, projParameter, timeParameter, sizeParameter, pointsParameter;
        public static void Load1(ContentManager Content)
        {
            shapeTexture = Content.Load<Texture2D>("Scenario/Backgrounds/1/shape");
            RayFx = Content.Load<Effect>("Scenario/Backgrounds/1/ray");
            LinesFx = Content.Load<Effect>("Scenario/Backgrounds/1/lines");
            TextureFx = Content.Load<Effect>("Scenario/Backgrounds/1/texturizer");

            BackgroundTexture = Content.Load<Texture2D>("Scenario/Backgrounds/1/texture");
            BackgroundFx = Content.Load<Effect>("Scenario/Backgrounds/1/bgFx");
            worldParameter = BackgroundFx.Parameters["World"];
            viewParameter = BackgroundFx.Parameters["View"];
            projParameter = BackgroundFx.Parameters["Proj"];
            timeParameter = BackgroundFx.Parameters["time"];
            sizeParameter = BackgroundFx.Parameters["size"];
            pointsParameter = BackgroundFx.Parameters["points"];

            worldParameter.SetValue(Matrix.Identity);
            viewParameter.SetValue(Matrix.Identity);
            projParameter.SetValue(Matrix.Identity);
            BackgroundFx.Parameters["colors"].SetValue(BackgroundTexture);
        }

        VertexBuffer buffer;

        public BackgroundOne()
        {
            buffer = new VertexBuffer(Game1.game.GraphicsDevice, typeof(VertexPositionColorTexture), 4, BufferUsage.WriteOnly);
            buffer.SetData(new VertexPositionColorTexture[]{
                new VertexPositionColorTexture(new Vector3(-1, -1, 0), new Color(255, 0, 0), new Vector2(0, 1)),
                new VertexPositionColorTexture(new Vector3(1, -1, 0), new Color(0, 255, 0), new Vector2(1, 1)),
                new VertexPositionColorTexture(new Vector3(-1, 1, 0), new Color(0, 0, 255), new Vector2(0, 0)),
                new VertexPositionColorTexture(new Vector3(1, 1, 0), new Color(255, 255, 255), new Vector2(1, 0)),
            });
        }

        public override Texture2D ShapeTexture { get { return shapeTexture; } }
        public override Effect ShapeFillFx { get { return TextureFx; } }
        public override Effect ShapeLineFx { get { return LinesFx; } }
        public override Effect RayLightFx { get { return RayFx; } }

        public override void Update()
        {
            
        }

        public override void PreDraw(GraphicsDevice device, SpriteBatch batch)
        {
            
        }

        public override void Draw(GraphicsDevice device, SpriteBatch batch)
        {
            float time = Game1.Time;
            timeParameter.SetValue(time);
            time *= 0.2f;
            float hw = Game1.HalfScreenWidth, hh = Game1.HalfScreenHeight;
            Vector2[] values = new Vector2[]{
                new Vector2((float)Math.Sin(time*0.177)*hw+hw, (float)Math.Sin(time*1.92+9.0)*hh+hh),
                new Vector2((float)Math.Sin(time*0.316+1.0)*hw+hw, (float)Math.Sin(time*1.284+5.0)*hh+hh),
                new Vector2((float)Math.Sin(time*0.583+2.0)*hw+hw, (float)Math.Sin(time*0.195+6.0)*hh+hh),
                new Vector2((float)Math.Sin(time*0.815+3.0)*hw+hw, (float)Math.Sin(time*0.553+7.0)*hh+hh),
                new Vector2((float)Math.Sin(time*1.174+4.0)*hw+hw, (float)Math.Sin(time*0.817+8.0)*hh+hh),
            };
            pointsParameter.SetValue(values);

            BackgroundFx.CurrentTechnique.Passes[0].Apply();
            device.SetVertexBuffer(buffer);
            device.RasterizerState = RasterizerState.CullNone;
            device.DrawPrimitives(PrimitiveType.TriangleStrip, 0, 2);

        }

        public override void Resize()
        {
            sizeParameter.SetValue(new Vector2(Game1.ScreenWidth, Game1.ScreenHeight));
        }

        public override void Dispose()
        {
            buffer.Dispose();
        }
    }
}
