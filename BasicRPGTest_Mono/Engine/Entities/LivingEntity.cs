using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;

namespace BasicRPGTest_Mono.Engine
{
    public class LivingEntity : Entity
    {
        public float speed { get; set; }

        private Timer tickTimer { get; set; }
        private int ticksSinceMove { get; set; }
        private int ticksToMove = 30;
        private int moveCount;
        private Direction direction = Direction.None;

        public LivingEntity(Texture2D texture, Rectangle box, GraphicsDeviceManager graphicsManager, float speed = 75f, Vector2 position = new Vector2()) : base(new Graphic(texture), box, graphicsManager)
        {
            if (GetType() == typeof(LivingEntity)) id = EntityManager.livingEntities.Count;
            this.speed = speed;
            this.position = position;
            boundingBox = getBox(position);

            EntityManager.add(this);
        }
        public LivingEntity(Graphic graphic, Rectangle box, GraphicsDeviceManager graphicsManager, float speed = 75f, Vector2 position = new Vector2()) : base(graphic, box, graphicsManager)
        {
            if (GetType() == typeof(LivingEntity)) id = EntityManager.livingEntities.Count;
            this.speed = speed;
            this.position = position;

            EntityManager.add(this);
        }
        public LivingEntity(LivingEntity entity, Vector2 pos) : base(entity.graphic, new Rectangle((int)pos.X, (int)pos.Y, entity.boundingBox.Width, entity.boundingBox.Height), entity.graphicsManager)
        {
            this.speed = entity.speed;
            this.position = pos;

            tickTimer = new Timer(50);
            tickTimer.Elapsed += tryMove;
            tickTimer.Start();

            moveCount = 0;
        }

        public override void update()
        {
            move(Core.globalTime, direction);
            base.update();
        }
        public void tryMove(Object source, ElapsedEventArgs args)
        {
            ticksSinceMove++;
            Random rand = new Random();
            if (ticksSinceMove > ticksToMove * (moveCount+1))
            {

                if (rand.Next(0, 100) < 50)
                {
                    moveCount = 0;
                    ticksSinceMove = 0;
                    direction = Direction.None;
                    return;
                }
                moveCount++;

                randomizeDirection();

                return;
            }
            if (moveCount > 0) return;

            if (ticksSinceMove < ticksToMove) return;
            if (rand.Next(0, 100) < 40) return;

            randomizeDirection();

            moveCount++;
        }
        public void randomizeDirection()
        {
            Random rand = new Random();
            int selection = rand.Next(0, 4);
            System.Diagnostics.Debug.WriteLine("Selected " + selection);

            switch (selection)
            {
                case 0:
                    direction = Direction.Up;
                    break;
                case 1:
                    direction = Direction.Down;
                    break;
                case 2:
                    direction = Direction.Left;
                    break;
                case 3:
                    direction = Direction.Right;
                    break;
            }
        }
        public virtual void move(GameTime gameTime, Direction direction)
        {
            Vector2 newPos = position;
            Rectangle newBox = boundingBox;

            if (MapManager.activeMap == null) return;
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
