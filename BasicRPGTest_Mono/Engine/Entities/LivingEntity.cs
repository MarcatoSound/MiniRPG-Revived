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
        public Vector2 velocity { get; set; }
        public Vector2 maxVelocity { get; set; }

        private Timer tickTimer { get; set; }
        private int ticksSinceMove { get; set; }
        private int ticksToMove = 30;
        private int moveCount;
        public Direction direction = Direction.None;

        public Timer knockbackTimer { get; set; }
        private bool isGettingKnockedBack { get; set; }

        public LivingEntity(Texture2D texture, Rectangle box, GraphicsDeviceManager graphicsManager, float speed = 90f, Vector2 position = new Vector2()) : base(new Graphic(texture), box, graphicsManager)
        {
            if (GetType() == typeof(LivingEntity)) id = EntityManager.livingEntities.Count;
            this.speed = speed;
            this.position = position;
            boundingBox = getBox(position);

            EntityManager.add(this);
        }
        public LivingEntity(Graphic graphic, Rectangle box, GraphicsDeviceManager graphicsManager, float speed = 90f, Vector2 position = new Vector2()) : base(graphic, box, graphicsManager)
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

            maxVelocity = new Vector2(speed, speed);
        }

        public override void update()
        {
            
            Vector2 newVel = velocity;

            if (!isGettingKnockedBack)
            {

                if (direction == Direction.Up)
                {
                    newVel = new Vector2(velocity.X, velocity.Y - 10f);
                    if (newVel.Y < -maxVelocity.Y) newVel.Y = -maxVelocity.Y;
                    velocity = newVel;
                }
                else
                {
                    if (newVel.Y < 0)
                    {
                        newVel = new Vector2(velocity.X, velocity.Y + 20f);
                        if (newVel.Y > 0)
                            newVel = new Vector2(velocity.X, 0);
                    }
                    velocity = newVel;
                }

                if (direction == Direction.Down)
                {
                    newVel = new Vector2(velocity.X, velocity.Y + 10f);
                    if (newVel.Y > maxVelocity.Y) newVel.Y = maxVelocity.Y;
                    velocity = newVel;
                }
                else
                {
                    if (newVel.Y > 0)
                    {
                        newVel = new Vector2(velocity.X, velocity.Y - 20f);
                        if (newVel.Y < 0)
                            newVel = new Vector2(velocity.X, 0);
                    }
                    velocity = newVel;
                }

                if (direction == Direction.Left)
                {
                    newVel = new Vector2(velocity.X - 10f, velocity.Y);
                    if (newVel.X < -maxVelocity.X) newVel.X = -maxVelocity.X;
                    velocity = newVel;
                }
                else
                {
                    if (newVel.X < 0)
                    {
                        newVel = new Vector2(velocity.X + 20f, velocity.Y);
                        if (newVel.X > 0)
                            newVel = new Vector2(0, velocity.X);
                    }
                    velocity = newVel;
                }

                if (direction == Direction.Right)
                {
                    newVel = new Vector2(velocity.X + 10f, velocity.Y);
                    if (newVel.X > maxVelocity.X) newVel.X = maxVelocity.X;
                    velocity = newVel;
                }
                else
                {
                    if (newVel.X > 0)
                    {
                        newVel = new Vector2(velocity.X - 20f, velocity.Y);
                        if (newVel.X < 0)
                            newVel = new Vector2(0, velocity.X);
                    }
                    velocity = newVel;
                }

            }


            move();
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
        public virtual void move()
        {
            Vector2 newPos = position;
            Rectangle newBox = boundingBox;

            // Right
            if (velocity.X > 0)
            {
                newPos.X += (float)(velocity.X / 1.5) * (float)Core.globalTime.ElapsedGameTime.TotalSeconds;
                if (newPos.X < 0 + (graphic.height / 2))
                    newPos.X = 0 + (graphic.height / 2);


                newBox = getBox(newPos);

                if (isColliding(newBox))
                {
                    newPos.X = position.X;
                    newBox.X = boundingBox.X;
                }
            }
            // Left
            else if (velocity.X < 0)
            {
                newPos.X += (float)(velocity.X / 1.5) * (float)Core.globalTime.ElapsedGameTime.TotalSeconds;

                if (newPos.Y > (MapManager.activeMap.tiledMap.HeightInPixels - (graphic.width / 2)))
                    newPos.Y = MapManager.activeMap.tiledMap.HeightInPixels - (graphic.width / 2);


                newBox = getBox(newPos);

                if (isColliding(newBox))
                {
                    newPos.X = position.X;
                    newBox.X = boundingBox.X;
                }
            }
            // Down
            if (velocity.Y > 0)
            {
                newPos.Y += (float)(velocity.Y / 1.5) * (float)Core.globalTime.ElapsedGameTime.TotalSeconds;
                if (newPos.Y < 0 + (graphic.height / 2))
                    newPos.Y = 0 + (graphic.height / 2);


                newBox = getBox(newPos);

                if (isColliding(newBox))
                {
                    newPos.Y = position.Y;
                    newBox.Y = boundingBox.Y;
                }
            }
            // Up
            else if (velocity.Y < 0)
            {
                newPos.Y += (float)(velocity.Y / 1.5) * (float)Core.globalTime.ElapsedGameTime.TotalSeconds;

                if (newPos.Y > (MapManager.activeMap.tiledMap.HeightInPixels - (graphic.width / 2)))
                    newPos.Y = MapManager.activeMap.tiledMap.HeightInPixels - (graphic.width / 2);


                newBox = getBox(newPos);

                if (isColliding(newBox))
                {
                    newPos.Y = position.Y;
                    newBox.Y = boundingBox.Y;
                }
            }

            position = new Vector2(newPos.X, newPos.Y);
            boundingBox = new Rectangle(newBox.X, newBox.Y, newBox.Width, newBox.Height);

        }
        public virtual void move(GameTime gameTime, Direction direction)
        {
            Vector2 newPos = position;
            Rectangle newBox = boundingBox;

            if (MapManager.activeMap == null) return;

            switch (direction)
            {
                case Direction.Up:
                    newPos.Y -= (float)(speed / 1.5) * (float)gameTime.ElapsedGameTime.TotalSeconds;
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
                    newPos.Y += (float)(speed / 1.5) * (float)gameTime.ElapsedGameTime.TotalSeconds;

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
                    newPos.X -= (float)(speed / 1.5) * (float)gameTime.ElapsedGameTime.TotalSeconds;
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
                    newPos.X += (float)(speed / 1.5) * (float)gameTime.ElapsedGameTime.TotalSeconds;

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


        public void hurt()
        {

        }

        public void knockback(Vector2 sourcePos)
        {
            if (isGettingKnockedBack) return;
            isGettingKnockedBack = true;

            knockbackTimer = new Timer(100);
            knockbackTimer.Elapsed += (sender, args) =>
            {
                knockbackTimer.Stop();
                knockbackTimer.Dispose();
                knockbackTimer = null;
                isGettingKnockedBack = false;
            };

            // TODO Finish this by using the source position (source of the hit)
            //  to calculate the direction the living entity is being knocked
            Vector2 enemyDist = new Vector2();
            enemyDist.X = sourcePos.X - position.X;
            enemyDist.Y = sourcePos.Y - position.Y;

            velocity = new Vector2(enemyDist.X * 4, enemyDist.Y * 4);

        }

        public void kill()
        {
            MapManager.activeMap.entities.Remove(this);
        }

    }
}
