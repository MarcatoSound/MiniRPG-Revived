using BasicRPGTest_Mono.Engine.Maps;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using RPGEngine;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace BasicRPGTest_Mono.Engine
{
    public class Entity
    {
        public string name { get; set; }
        public int id { get; set; }
        public int instanceId { get; set; }
        public Graphic graphic { get; set; }
        public Rectangle boundingBox { get; set; }
        public Vector2 Position { get; set; }
        public Vector2 TilePosition
        {
            get
            {
                Vector2 pos = new Vector2(Position.X, Position.Y);

                pos.X = pos.X / TileManager.dimensions;
                pos.Y = pos.Y / TileManager.dimensions;

                return pos;
            }
        }
        public Vector2 CenteredPosition
        {
            get
            {
                return new Vector2(Position.X + (graphic.texture.Width / 2), Position.Y + (graphic.texture.Height / 2));
            }
        }

        private Color _tintColor = Color.White;
        public Color tintColor
        {
            get { return _tintColor; }
            set
            {
                _tintColor = value;
            }
        }

        public GraphicsDeviceManager graphicsManager { get; set; }

        public Entity(Graphic graphic, Rectangle box, GraphicsDeviceManager graphicsDevice)
        {
            this.graphic = graphic;
            boundingBox = box;
            if (GetType() == typeof(Entity)) id = EntityManager.entities.Count;
            graphicsManager = graphicsDevice;


            //EntityManager.add(this);
        }

        public bool isCollidingWith(Rectangle box)
        {
            if (boundingBox.Intersects(box)) return true;
            return false;
        }

        public List<Tile> getSurroundingTiles(Map map, int radius, Vector2 pos)
        {
            List<Tile> tiles = new List<Tile>();

            Vector2 startingPos = new Vector2(TilePosition.X - radius, TilePosition.Y - radius);

            Vector2 targetPos = new Vector2();
            for (int x = (int)startingPos.X; x < startingPos.X + (radius * 2); x++)
            {
                for (int y = (int)startingPos.Y; y < startingPos.Y + (radius * 2); y++)
                {
                    foreach (TileLayer layer in map.layers)
                    {
                        targetPos.X = x;
                        targetPos.Y = y;
                        tiles.Add(layer.getTile(targetPos));
                    }
                    //System.Diagnostics.Debug.WriteLine($"Scanned position {targetPos}.");
                }
            }

            //System.Diagnostics.Debug.WriteLine($"Collected {tiles.Count} tiles.");

            return tiles;
        }

        public virtual void update()
        {
        }
        public virtual void draw(SpriteBatch batch)
        {
            if (!Camera.camera.BoundingRectangle.Intersects(boundingBox)) return;
            graphic.draw(batch, Position, tintColor);

            batch.Begin(transformMatrix: Camera.camera.Transform);
            batch.DrawRectangle(new Rectangle((int)CenteredPosition.X, (int)CenteredPosition.Y, 5, 5), Color.AliceBlue);
            batch.End();
        }


        public virtual Rectangle getBox(Vector2 pos)
        {
            int x = (int)(pos.X - (boundingBox.Width - (boundingBox.Width / 2)));
            int y = (int)(pos.Y - (boundingBox.Height - (graphic.texture.Height / 2)));

            Rectangle box = new Rectangle(x, y, boundingBox.Width, boundingBox.Height);

            return box;
        }

    }

    public enum Direction
    {
        Up,
        Down,
        Left,
        Right,
        None
    }
}
