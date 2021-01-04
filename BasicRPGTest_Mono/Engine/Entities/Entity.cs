using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using RPGEngine;
using System;
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


        public bool isColliding(Rectangle box)
        {
            if (MapManager.activeMap == null) return true;
            foreach (Rectangle tileBox in MapManager.activeMap.collidables)
            {
                if (box.Intersects(tileBox))
                    return true;
            }


            return false;
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
