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
        public bool isMoving;

        public float speed { get; set; }
        public Vector2 velocity { get; set; }
        public Vector2 maxVelocity { get; set; }

        private Timer tickTimer { get; set; }
        private int ticksSinceMove { get; set; }
        private int ticksToMove = 30;
        private int moveCount;
        public Direction direction = Direction.None;

        public Timer immunityTimer;
        public bool isImmunity;
        public int immunityTime = 150;

        public Timer knockbackTimer { get; set; }
        public bool isGettingKnockedBack { get; set; }
        public float kbResist = 0f;

        public LivingEntity(string name, Texture2D texture, Rectangle box, GraphicsDeviceManager graphicsManager, float speed = 90f, Vector2 position = new Vector2()) : base(new Graphic(texture), box, graphicsManager)
        {
            if (GetType() == typeof(LivingEntity)) id = EntityManager.livingEntities.Count;
            this.name = name;
            this.speed = speed;
            this.Position = position;
            boundingBox = getBox(position);

            //EntityManager.add(this);
        }
        public LivingEntity(string name, Graphic graphic, Rectangle box, GraphicsDeviceManager graphicsManager, float speed = 90f, Vector2 position = new Vector2()) : base(graphic, box, graphicsManager)
        {
            if (GetType() == typeof(LivingEntity)) id = EntityManager.livingEntities.Count;
            this.name = name;
            this.speed = speed;
            this.Position = position;

            //EntityManager.add(this);
        }
        public LivingEntity(LivingEntity entity, Vector2 pos, int instanceId) : base(entity.graphic, new Rectangle((int)pos.X, (int)pos.Y, entity.boundingBox.Width, entity.boundingBox.Height), entity.graphicsManager)
        {
            this.speed = entity.speed;
            this.Position = pos;
            this.instanceId = instanceId;

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
            Vector2 newPos = Position;
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
                    newPos.X = Position.X;
                    newBox.X = boundingBox.X;
                }
            }
            // Left
            else if (velocity.X < 0)
            {
                newPos.X += (float)(velocity.X / 1.5) * (float)Core.globalTime.ElapsedGameTime.TotalSeconds;

                if (newPos.Y > (MapManager.activeMap.heightInPixels - (graphic.width / 2)))
                    newPos.Y = MapManager.activeMap.heightInPixels - (graphic.width / 2);


                newBox = getBox(newPos);

                if (isColliding(newBox))
                {
                    newPos.X = Position.X;
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
                    newPos.Y = Position.Y;
                    newBox.Y = boundingBox.Y;
                }
            }
            // Up
            else if (velocity.Y < 0)
            {
                newPos.Y += (float)(velocity.Y / 1.5) * (float)Core.globalTime.ElapsedGameTime.TotalSeconds;

                if (newPos.Y > (MapManager.activeMap.heightInPixels - (graphic.width / 2)))
                    newPos.Y = MapManager.activeMap.heightInPixels - (graphic.width / 2);


                newBox = getBox(newPos);

                if (isColliding(newBox))
                {
                    newPos.Y = Position.Y;
                    newBox.Y = boundingBox.Y;
                }
            }

            Position = new Vector2(newPos.X, newPos.Y);
            boundingBox = new Rectangle(newBox.X, newBox.Y, newBox.Width, newBox.Height);

        }


        public virtual void hurt(Vector2 sourcePos)
        {
            if (isImmunity) return;
            isImmunity = true;
            tintColor = Color.LightCoral;

            immunityTimer = new Timer(immunityTime);
            immunityTimer.Elapsed += (sender, args) =>
            {
                isImmunity = false;
                tintColor = Color.White;
                immunityTimer.Stop();
                immunityTimer.Dispose();
                immunityTimer = null;
            };
            immunityTimer.Start();

            knockback(sourcePos);
        }

        public virtual void knockback(Vector2 sourcePos)
        {
            if (isGettingKnockedBack) return;
            isGettingKnockedBack = true;

            Vector2 screenPos = Position;
            screenPos.X = screenPos.X + (graphic.texture.Width / 2);
            screenPos.Y = screenPos.Y + (graphic.texture.Height / 2);
            Vector2 targetDist = new Vector2();
            targetDist.X = screenPos.X - sourcePos.X;
            targetDist.Y = screenPos.Y - sourcePos.Y;

            double z = Math.Sqrt((Math.Pow(targetDist.X, 2)) + (Math.Pow(targetDist.Y, 2)));
            float maxKbVel = 600f;
            int knockbackStr = (int)targetDist.X + (int)targetDist.Y;
            Vector2 kbRatio = new Vector2((float)(targetDist.X / z), (float)(targetDist.Y / z));
            velocity = new Vector2(maxKbVel / 2 * kbRatio.X, maxKbVel / 2 * kbRatio.Y);


            int maxKbTime = 200;
            double zOut = Math.Pow(z, -1) * 10 + 0.3;
            int kbTime = Convert.ToInt32(Math.Min(Math.Max(1000 * zOut - 400, 25), maxKbTime));
            kbTime = Convert.ToInt32(kbTime - (kbTime * kbResist));
            knockbackTimer = new Timer(kbTime);
            knockbackTimer.Elapsed += (sender, args) =>
            {
                isGettingKnockedBack = false;
                knockbackTimer.Stop();
                knockbackTimer.Dispose();
                knockbackTimer = null;
            };
            knockbackTimer.Start();

        }

        public void kill()
        {
            MapManager.activeMap.entities.TryRemove(instanceId, out _);
        }

    }
}
