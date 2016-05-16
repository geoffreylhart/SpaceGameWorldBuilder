using Game1.Geom;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Game1.Modules
{
    class FractalDrawer : IModule
    {
        List<Line> lines = new List<Line>();
        HashSet<Point2D> points = new HashSet<Point2D>();
        Point2D creating = null;
        Point2D lastMousePos = null;

        internal override string GetIconName()
        {
            return "icons/geom";
        }

        internal override void Initialize(ContentManager content, GraphicsDevice graphicsDevice)
        {
            base.Initialize(content, graphicsDevice);
        }

        internal override void Draw(GraphicsDevice graphicsDevice, BasicEffect basicEffect)
        {
            basicEffect.VertexColorEnabled = true;
            foreach(Line line in lines){
                line.Draw(graphicsDevice, basicEffect, Color.White);
            }
            foreach (Point2D point in points)
            {
                point.Draw(graphicsDevice, basicEffect, (point == lastMousePos)?Color.Red:Color.White);
            }
            if (creating != null) creating.Draw(graphicsDevice, basicEffect, Color.Red);
            if (creating != null && lastMousePos != null)
            {
                new Line(creating, lastMousePos).Draw(graphicsDevice, basicEffect, Color.Red);
            }
            basicEffect.VertexColorEnabled = false;
        }

        private static ButtonState prevLeftState;
        internal override void Update(Vector3 relMousePos, double scale)
        {
            MouseState mouseState = Mouse.GetState();
            if (prevLeftState.Equals(ButtonState.Pressed) && mouseState.LeftButton.Equals(ButtonState.Released))
            {
                if (creating == null)
                {
                    creating = lastMousePos;
                }
                else
                {
                    points.Add(creating);
                    points.Add(lastMousePos);
                    lines.Add(new Line(creating, lastMousePos));
                    creating = null;
                }
            }
            prevLeftState = mouseState.LeftButton;
            lastMousePos = new Point2D(relMousePos);
            double minDist = double.MaxValue;
            foreach (Point2D point in points)
            {
                double dist = Vector3.Distance(lastMousePos.pos, point.pos) * scale;
                if (dist < 0.03 && dist < minDist)
                {
                    minDist = dist;
                    lastMousePos = point;
                }
            }
        }

        internal void Save(string file)
        {
            Stream stream = new FileStream(file, FileMode.Create, FileAccess.Write, FileShare.None);
            using (var writer = new BinaryWriter(stream))
            {
                Point2D[] pointArray = points.ToArray();
                Dictionary<Point2D, int> indexHash = new Dictionary<Point2D, int>();
                writer.Write(pointArray.Length);
                for(int i=0; i<pointArray.Length; i++){
                    writer.Write(pointArray[i].pos.X);
                    writer.Write(pointArray[i].pos.Y);
                    writer.Write(pointArray[i].pos.Z);
                    indexHash.Add(pointArray[i], i);
                }
                writer.Write(lines.Count);
                foreach (var line in lines)
                {
                    writer.Write(indexHash[line.p1]);
                    writer.Write(indexHash[line.p2]);
                }
            }
        }

        internal static FractalDrawer Load(string file)
        {
            if (File.Exists(file))
            {
                FractalDrawer newGeom = new FractalDrawer();
                Stream stream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.None);
                using (BinaryReader reader = new BinaryReader(stream))
                {
                    Point2D[] pointArray = new Point2D[reader.ReadInt32()];
                    for (int i = 0; i < pointArray.Length; i++)
                    {
                        pointArray[i] = new Point2D(new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle()));
                        newGeom.points.Add(pointArray[i]);
                    }
                    int lineCount = reader.ReadInt32();
                    for (int i = 0; i < lineCount; i++)
                    {
                        newGeom.lines.Add(new Line(pointArray[reader.ReadInt32()], pointArray[reader.ReadInt32()]));
                    }
                }
                return newGeom;
            }
            {
                return new FractalDrawer();
            }
        }
    }
}
