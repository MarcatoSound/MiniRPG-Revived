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
        public Vector2 position { get; set; }

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
            Vector2 screenPos = getScreenPosition();
            Vector2 screenPosCentered = getScreenPosition(true);
            graphic.draw(batch, screenPos, tintColor);

            batch.Begin();
            batch.DrawRectangle(new Rectangle((int)screenPos.X, (int)screenPos.Y, 5, 5), Color.AliceBlue);
            batch.DrawRectangle(new Rectangle((int)screenPosCentered.X, (int)screenPosCentered.Y, 1, 1), Color.AliceBlue);
            batch.End();
        }

        public Vector2 getScreenPosition(bool centered = false)
        {
            Vector2 pos = new Vector2(position.X, position.Y);

            pos.X = pos.X - Camera.camPos.X;
            pos.Y = pos.Y - Camera.camPos.Y;

            if (centered)
            {
                pos.X += graphic.texture.Width / 2;
                pos.Y += graphic.texture.Height / 2;
            }

            return pos;
        }

        public virtual Rectangle getScreenBox()
        {
            Vector2 pos = getScreenPosition();
            Rectangle box = new Rectangle(Convert.ToInt32(pos.X + ((32 - boundingBox.Width) / 2)), Convert.ToInt32(pos.Y + (32 - boundingBox.Height)), boundingBox.Width, boundingBox.Height);

            return box;
        }
        public virtual Rectangle getBox(Vector2 pos)
        {
            Rectangle box = new Rectangle(Convert.ToInt32(pos.X + (32 - boundingBox.Width) / 2), Convert.ToInt32(pos.Y + (32 - boundingBox.Height)), boundingBox.Width, boundingBox.Height);

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
