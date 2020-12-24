using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RPGEngine;
using System;
using System.Collections.Generic;
using System.Text;

namespace BasicRPGTest_Mono.Engine
{
    public class Entity
    {
        public int id { get; set; }
        public Graphic graphic { get; set; }
        public Rectangle boundingBox { get; set; }
        public Vector2 position { get; set; }

        public GraphicsDeviceManager graphicsManager { get; set; }

        public Entity(Graphic graphic, Rectangle box, GraphicsDeviceManager graphicsDevice)
        {
            this.graphic = graphic;
            boundingBox = box;
            if (GetType() == typeof(Entity)) id = EntityManager.entities.Count;
            graphicsManager = graphicsDevice;


            EntityManager.add(this);
        }


        public bool isColliding(Rectangle box)
        {

            foreach (Rectangle tileBox in MapManager.activeMap.collidables)
            {
                if (box.Intersects(tileBox))
                    return true;
            }

            return false;
        }

        public virtual void update()
        {
            if (graphic is GraphicAnimated)
                ((GraphicAnimated)graphic).update();
        }
        public virtual void draw(SpriteBatch batch)
        {
            if (!Core.camera.BoundingRectangle.Intersects(boundingBox)) return;
            Vector2 screenPos = getScreenPosition();
            graphic.draw(batch, screenPos);
        }

        public Vector2 getScreenPosition()
        {
            Vector2 pos = new Vector2(position.X, position.Y);

            pos.X = pos.X - Core.camPos.X;
            pos.Y = pos.Y - Core.camPos.Y;

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
        Right
    }
}
