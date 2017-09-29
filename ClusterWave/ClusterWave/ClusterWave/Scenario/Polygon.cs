﻿using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Common;
using FarseerPhysics.Common.Decomposition;

namespace ClusterWave.Scenario
{
    class Polygon : Shape
    {
        public const byte Type = 1;

        Texture2D fillTx;
        VertexBuffer fillBuffer;
        int fillPrimitiveCount;

        public Polygon(Vector2[] vertices, Body physicsBody, Texture2D texture)
        {
            //god forbid the unreadability of this constructor

            #region CreateLineBuffer
            lineBuffer = new VertexBuffer(Game1.game.GraphicsDevice, typeof(VertexPositionTexture), vertices.Length + 1, BufferUsage.WriteOnly);
            VertexPositionTexture[] data = new VertexPositionTexture[vertices.Length + vertices.Length + 2];
            int i = 0;
            for (; i < vertices.Length; i++)
                data[i] = CreateVertex(vertices[i]);
            data[i] = data[0];
            lineBuffer.SetData(data, 0, vertices.Length + 1);
            linePrimitiveCount = vertices.Length;

            lightBuffer = new VertexBuffer(Game1.game.GraphicsDevice, typeof(VertexPositionTexture), vertices.Length + vertices.Length + 2, BufferUsage.WriteOnly);
            int index = 0;
            Vector2 min = vertices[0], max = min;
            for (i = 0; i < vertices.Length; i++)
            {
                data[index++] = CreateVertex(new Vector3(vertices[i], 0));
                data[index++] = CreateVertex(new Vector3(vertices[i], LightWallZ));

                if (vertices[i].X < min.X) min.X = vertices[i].X;
                else if (vertices[i].X > max.X) max.X = vertices[i].X;

                if (vertices[i].Y < min.Y) min.Y = vertices[i].Y;
                else if (vertices[i].Y > max.Y) max.Y = vertices[i].Y;
            }
            data[index++] = data[0];
            data[index++] = data[1];

            lightBuffer.SetData(data);
            lightPrimitiveCount = data.Length - 2;
            #endregion
            
            List<Vertices> vert = Triangulate.ConvexPartition(new Vertices(vertices), TriangulationAlgorithm.Bayazit);
            i = 0;
            for (int c = 0; c < vert.Count; c++)
                i += vert[c].Count * 3;
            List<VertexPositionColorTexture> fillList = new List<VertexPositionColorTexture>(i);

            #region FillBuffer
            for (int c = 0; c < vert.Count; c++)
            {
                #region Physics
                Fixture f = physicsBody.CreateFixture(new PolygonShape(vert[c], 1f));
                f.CollisionCategories = Constants.WallsCategory;
                f.CollidesWith = Constants.WallsCollideWith;
                f.Friction = Friction;
                f.Restitution = Restitution;
                #endregion

                //Calculates the centroid of the convex polygon (polygons are split into convex)
                //and makes a polygon fan to cover it's area

                Vector2 centroid = Vector2.Zero;
                for (int a = 0; a < vert[c].Count; a++)
                    centroid += vert[c][a];
                centroid /= vert[c].Count;

                List<Vector3> l = new List<Vector3>(vert[c].Count);
                for (int a = 0; a < vert[c].Count; a++)
                {
                    Vector2 p = vert[c][a];
                    l.Add(new Vector3(p, (float)Math.Atan2(p.X - centroid.X, p.Y - centroid.Y)));
                }
                l.Sort((x, y) => x.Z.CompareTo(y.Z));

                VertexPositionColorTexture centVert = new VertexPositionColorTexture(new Vector3(centroid, 0), Color.White, Vector2.Zero);
                for (int a = 0; a < l.Count - 1;)
                {
                    fillList.Add(centVert);
                    fillList.Add(new VertexPositionColorTexture(new Vector3(l[a].X, l[a].Y, 0), Color.White, Vector2.Zero));
                    a++;
                    fillList.Add(new VertexPositionColorTexture(new Vector3(l[a].X, l[a].Y, 0), Color.White, Vector2.Zero));
                }
                int lesscount = l.Count-1;
                fillList.Add(centVert);
                fillList.Add(new VertexPositionColorTexture(new Vector3(l[lesscount].X, l[lesscount].Y, 0), Color.White, Vector2.Zero));
                fillList.Add(new VertexPositionColorTexture(new Vector3(l[0].X, l[0].Y, 0), Color.White, Vector2.Zero));
            }

            fillBuffer = new VertexBuffer(Game1.game.GraphicsDevice, typeof(VertexPositionColorTexture), fillList.Count, BufferUsage.WriteOnly);
            fillBuffer.SetData(fillList.ToArray());
            fillPrimitiveCount = fillList.Count / 3;
            #endregion
            this.fillTx = texture;
        }

        public override void DrawFill(GraphicsDevice device, Effect effect)
        {
            effect.Parameters[3].SetValue(fillTx);
            device.SetVertexBuffer(fillBuffer);
            device.DrawPrimitives(PrimitiveType.TriangleList, 0, fillPrimitiveCount);
        }

        public override void Dispose()
        {
            base.Dispose();
            fillBuffer.Dispose();
        }
    }
}
