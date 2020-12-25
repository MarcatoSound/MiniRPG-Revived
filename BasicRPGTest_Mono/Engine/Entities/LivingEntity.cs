using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace BasicRPGTest_Mono.Engine
{
    public class LivingEntity : Entity
    {
        public float speed { get; set; }

        public LivingEntity(Texture2D texture, Rectangle box, GraphicsDeviceManager graphicsManager, float speed = 100f, Vector2 position = new Vector2()) : base(new Graphic(texture), box, graphicsManager)
        {
            if (GetType() == typeof(LivingEntity)) id = EntityManager.livingEntities.Count;
            this.speed = speed;
            this.position = position;
            boundingBox = getBox(position);

            EntityManager.add(this);
        }
        public LivingEntity(Graphic graphic, Rectangle box, GraphicsDeviceManager graphicsManager, float speed = 100f, Vector2 position = new Vector2()) : base(graphic, box, graphicsManager)
        {
            if (GetType() == typeof(LivingEntity)) id = EntityManager.livingEntities.Count;
            this.speed = speed;
            this.position = position;

            EntityManager.add(this);
        }

        public override void update()
        {
            base.update();
        }
        public virtual void move(GameTime gameTime, Direction direction)
        {
            Vector2 newPos = position;
            Rectangle newBox = boundingBox;

            switch (direction)
            {
                case Direction.Up:
                    newPos.Y -= speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                    if (newPos.Y < 0 + (graphic.height / 2))
                        newPos.Y = 0 + (graphic.height / 2);


                    newBox = getBox(newPos);

                    if (isColliding(newBox))
                    {
                        newPos.Y = position.Y;
                        newBox.Y = boundingBox.Y;
                    }

                    break;
                case Direction.Down:
                    newPos.Y += speed * (float)gameTime.ElapsedGameTime.TotalSeconds;

                    if (newPos.Y > (MapManager.activeMap.tiledMap.HeightInPixels - (graphic.width / 2)))
                        newPos.Y = MapManager.activeMap.tiledMap.HeightInPixels - (graphic.width / 2);


                    newBox = getBox(newPos);

                    if (isColliding(newBox))
                    {
                        newPos.Y = position.Y;
                        newBox.Y = boundingBox.Y;
                    }

                    break;

                case Direction.Left:
                    newPos.X -= speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                    if (newPos.X < 0 + (graphic.height / 2))
                        newPos.X = 0 + (graphic.height / 2);


                    newBox = getBox(newPos);

                    if (isColliding(newBox))
                    {
                        newPos.X = position.X;
                        newBox.X = boundingBox.X;
                    }

                    break;

                case Direction.Right:
                    newPos.X += speed * (float)gameTime.ElapsedGameTime.TotalSeconds;

                    if (newPos.Y > (MapManager.activeMap.tiledMap.HeightInPixels - (graphic.width / 2)))
                        newPos.Y = MapManager.activeMap.tiledMap.HeightInPixels - (graphic.width / 2);


                    newBox = getBox(newPos);

                    if (isColliding(newBox))
                    {
                        newPos.X = position.X;
                        newBox.X = boundingBox.X;
                    }

                    break;

            }

            position = new Vector2(newPos.X, newPos.Y);
            boundingBox = new Rectangle(newBox.X, newBox.Y, newBox.Width, newBox.Height);

        }


    }
}
