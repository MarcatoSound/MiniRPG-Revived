using BasicRPGTest_Mono.Engine.Entities;
using BasicRPGTest_Mono.Engine.Maps;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Tiled;
using RPGEngine;
using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;

namespace BasicRPGTest_Mono.Engine
{
    public class Player : LivingEntity
    {
        public bool isDashing;
        public Timer dashTimer;

        public SwordSwing swordSwing;
        public bool isAttacking;
        public Timer attackTimer;
        public Player(Texture2D texture, GraphicsDeviceManager graphics) : base("player", new GraphicAnimated(texture, 3, 4), new Rectangle(0, 0, 28, 26), graphics, 125f)
        {
            graphicsManager = graphics;
            position = new Vector2(MapManager.activeMap.widthInPixels / 2, MapManager.activeMap.heightInPixels / 2);
            boundingBox = new Rectangle((int)position.X, (int)position.Y, 28, 26);
            Camera.camPos = new Vector2(position.X - (Camera.camera.BoundingRectangle.Width / 2), position.Y - (Camera.camera.BoundingRectangle.Height / 2));
            maxVelocity = new Vector2(speed, speed);
            dashTimer = new Timer(200);
            dashTimer.Elapsed += (sender, args) =>
            {
                isDashing = false;
                dashTimer.Stop();
                maxVelocity = new Vector2(speed, speed);
            };

            kbResist = 0.75f;
        }
        public void attack(Direction direction)
        {
            if (isAttacking) return;

            isAttacking = true;
            swordSwing = new SwordSwing(direction, 150, this);
            attackTimer = new Timer(150);
            attackTimer.Elapsed += (sender, args) =>
            {
                isAttacking = false;
                attackTimer.Stop();
                attackTimer = null;
            };
            attackTimer.Start();
            
        }
        public override void move()
        {
            Vector2 _cameraPosition = Camera.camera.Position;
            Vector2 newPlayerPos = position;
            Vector2 newCameraPos = _cameraPosition;

            if (velocity.X > 0)
            {

                newPlayerPos.X += velocity.X * (float)Core.globalTime.ElapsedGameTime.TotalSeconds;

                if (newPlayerPos.X > (MapManager.activeMap.widthInPixels - (graphic.width / 2)))
                    newPlayerPos.X = MapManager.activeMap.widthInPixels - (graphic.width / 2);

                if (!(newPlayerPos.X <= graphicsManager.PreferredBackBufferWidth / 2))
                    newCameraPos.X += velocity.X * (float)Core.globalTime.ElapsedGameTime.TotalSeconds;

                if (newCameraPos.X >= (MapManager.activeMap.widthInPixels - Camera.camera.BoundingRectangle.Width))
                    newCameraPos.X = MapManager.activeMap.widthInPixels - Camera.camera.BoundingRectangle.Width;


                if (isColliding(getBox(newPlayerPos)))
                {
                    newPlayerPos.X = position.X;
                    newCameraPos.X = _cameraPosition.X;
                }

            }
            else if (velocity.X < 0)
            {

                newPlayerPos.X += velocity.X * (float)Core.globalTime.ElapsedGameTime.TotalSeconds;

                if (newPlayerPos.X < 0 + (graphic.width / 2))
                    newPlayerPos.X = 0 + (graphic.width / 2);

                if (!(newPlayerPos.X >= MapManager.activeMap.widthInPixels - (graphicsManager.PreferredBackBufferWidth / 2)))
                    newCameraPos.X += velocity.X * (float)Core.globalTime.ElapsedGameTime.TotalSeconds;

                if (newCameraPos.X <= 0)
                    newCameraPos.X = 0;


                if (isColliding(getBox(newPlayerPos)))
                {
                    newPlayerPos.X = position.X;
                    newCameraPos.X = _cameraPosition.X;
                }

            }

            if (velocity.Y > 0)
            {

                newPlayerPos.Y += velocity.Y * (float)Core.globalTime.ElapsedGameTime.TotalSeconds;

                if (newPlayerPos.Y > (MapManager.activeMap.heightInPixels - (graphic.width / 2)))
                    newPlayerPos.Y = MapManager.activeMap.heightInPixels - (graphic.width / 2);

                if (!(newPlayerPos.Y <= graphicsManager.PreferredBackBufferHeight / 2))
                    newCameraPos.Y += velocity.Y * (float)Core.globalTime.ElapsedGameTime.TotalSeconds;

                if (newCameraPos.Y >= (MapManager.activeMap.heightInPixels - Camera.camera.BoundingRectangle.Height))
                    newCameraPos.Y = MapManager.activeMap.heightInPixels - Camera.camera.BoundingRectangle.Height;


                if (isColliding(getBox(newPlayerPos)))
                {
                    newPlayerPos.Y = position.Y;
                    newCameraPos.Y = _cameraPosition.Y;
                }

            }
            else if (velocity.Y < 0)
            {

                newPlayerPos.Y += velocity.Y * (float)Core.globalTime.ElapsedGameTime.TotalSeconds;

                if (newPlayerPos.Y < 0 + (graphic.height / 2))
                    newPlayerPos.Y = 0 + (graphic.height / 2);

                if (!(newPlayerPos.Y >= MapManager.activeMap.heightInPixels - (graphicsManager.PreferredBackBufferHeight / 2)))
                    newCameraPos.Y += velocity.Y * (float)Core.globalTime.ElapsedGameTime.TotalSeconds;

                if (newCameraPos.Y <= 0)
                    newCameraPos.Y = 0;


                if (isColliding(getBox(newPlayerPos)))
                {
                    newPlayerPos.Y = position.Y;
                    newCameraPos.Y = _cameraPosition.Y;
                }

            }


            position = new Vector2(newPlayerPos.X, newPlayerPos.Y);
            Camera.camPos = new Vector2(newCameraPos.X, newCameraPos.Y);

        }



        public Vector2 getPlayerTilePosition()
        {
            Vector2 pos = new Vector2(position.X, position.Y);

            pos.X = (int)pos.X / TileManager.dimensions;
            pos.Y = (int)pos.Y / TileManager.dimensions;
            pos.Y = (int)pos.Y / TileManager.dimensions;

            return pos;
        }
        public Vector2 getPlayerTilePosition(Vector2 truePos)
        {
            Vector2 pos = new Vector2(truePos.X, truePos.Y);

            pos.X = (int)pos.X / TileManager.dimensions;
            pos.Y = (int)pos.Y / TileManager.dimensions;

            return pos;
        }
        public Vector2 getPlayerTilePositionPrecise()
        {
            Vector2 pos = new Vector2(position.X, position.Y);

            pos.X = pos.X / TileManager.dimensions;
            pos.Y = pos.Y / TileManager.dimensions;

            return pos;
        }
        public Vector2 getPlayerScreenPosition()
        {
            Vector2 pos = new Vector2(graphicsManager.PreferredBackBufferWidth / 2, graphicsManager.PreferredBackBufferHeight / 2);

            // Check if the player is already considered "centered" first
            if (position.X == pos.X && position.Y == pos.Y)
                return pos;

            if (position.X < pos.X) pos.X = position.X;
            if (position.Y < pos.Y) pos.Y = position.Y;
            if (position.X > MapManager.activeMap.widthInPixels - pos.X) pos.X = pos.X + (position.X - (MapManager.activeMap.widthInPixels - pos.X));
            if (position.Y > MapManager.activeMap.heightInPixels - pos.Y) pos.Y = pos.Y + (position.Y - (MapManager.activeMap.heightInPixels - pos.Y));

            return pos;
        }
        public override Rectangle getScreenBox()
        {
            Vector2 pos = getScreenPosition();
            Rectangle box = getBox(pos);

            return box;
        }
        public override Rectangle getBox(Vector2 pos)
        {
            int x = (int)(pos.X - (boundingBox.Width - (boundingBox.Width / 2)));
            int y = (int)(pos.Y - (boundingBox.Height - (graphic.height / 2)));

            Rectangle box = new Rectangle(x, y, boundingBox.Width, boundingBox.Height);

            return box;
        }

        public override void update()
        {
            boundingBox = getBox(position);
            ((GraphicAnimated)graphic).update();

            foreach (LivingEntity entity in MapManager.activeMap.livingEntities)
            {
                Vector2 screenPos = getPlayerScreenPosition();
                if (swordSwing != null && swordSwing.hitBox.Intersects(entity.getScreenBox()) && !entity.isImmunity)
                    entity.hurt(screenPos);

                if (getBox(screenPos).Intersects(entity.getScreenBox()) && !entity.isImmunity)
                    hurt(entity.getScreenPosition(true));
            }

            var kstate = Keyboard.GetState();
            Vector2 newVel = velocity;

            if (!isGettingKnockedBack)
            {
                if (kstate.IsKeyDown(Keys.W))
                {
                    newVel = new Vector2(velocity.X, velocity.Y - (float)(maxVelocity.Y / 5));
                    if (newVel.Y < -maxVelocity.Y) newVel.Y = -maxVelocity.Y;
                    velocity = newVel;
                }
                else
                {
                    if (newVel.Y < 0)
                    {
                        newVel = new Vector2(velocity.X, velocity.Y + (float)(maxVelocity.Y / 5));
                        if (newVel.Y > 0)
                            newVel = new Vector2(velocity.X, 0);
                    }
                    velocity = newVel;
                }

                if (kstate.IsKeyDown(Keys.S))
                {
                    newVel = new Vector2(velocity.X, velocity.Y + (float)(maxVelocity.Y / 5));
                    if (newVel.Y > maxVelocity.Y) newVel.Y = maxVelocity.Y;
                    velocity = newVel;
                }
                else
                {
                    if (newVel.Y > 0)
                    {
                        newVel = new Vector2(velocity.X, velocity.Y - (float)(maxVelocity.Y / 5));
                        if (newVel.Y < 0)
                            newVel = new Vector2(velocity.X, 0);
                    }
                    velocity = newVel;
                }

                if (kstate.IsKeyDown(Keys.A))
                {
                    newVel = new Vector2(velocity.X - (float)(maxVelocity.X / 5), velocity.Y);
                    if (newVel.X < -maxVelocity.X) newVel.X = -maxVelocity.X;
                    velocity = newVel;
                }
                else
                {
                    if (newVel.X < 0)
                    {
                        newVel = new Vector2(velocity.X + (float)(maxVelocity.X / 5), velocity.Y);
                        if (newVel.X > 0)
                            newVel = new Vector2(0, velocity.X);
                    }
                    velocity = newVel;
                }

                if (kstate.IsKeyDown(Keys.D))
                {
                    newVel = new Vector2(velocity.X + (float)(maxVelocity.X / 5), velocity.Y);
                    if (newVel.X > maxVelocity.X) newVel.X = maxVelocity.X;
                    velocity = newVel;
                }
                else
                {
                    if (newVel.X > 0)
                    {
                        newVel = new Vector2(velocity.X - (float)(maxVelocity.X / 5), velocity.Y);
                        if (newVel.X < 0)
                            newVel = new Vector2(0, velocity.X);
                    }
                    velocity = newVel;
                }
            }

            move();

        }
        public override void hurt(Vector2 sourcePos)
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
        public override void knockback(Vector2 sourcePos)
        {
            if (isGettingKnockedBack) return;
            isGettingKnockedBack = true;

            Vector2 screenPos = getPlayerScreenPosition();
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
            System.Diagnostics.Debug.WriteLine("zOut: " + zOut);
            int kbTime = Convert.ToInt32(Math.Min(Math.Max(1000 * zOut - 400, 25), maxKbTime));
            kbTime = Convert.ToInt32(kbTime - (kbTime * kbResist));
            System.Diagnostics.Debug.WriteLine("KB Time: " + kbTime);
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
        public void Dash()
        {
            if (!isDashing)
            {
                isDashing = true;
                maxVelocity = new Vector2(Convert.ToInt32(speed * 4), Convert.ToInt32(speed * 4));
                dashTimer.Start();
            }
        }
        public override void draw(SpriteBatch batch)
        {
            Vector2 screenPos = getPlayerScreenPosition();
            graphic.draw(batch, screenPos, tintColor);

            batch.Begin();
            batch.DrawRectangle(getScreenBox(), Color.White);
            batch.End();

            if (swordSwing != null)
            {
                swordSwing.Draw(batch, getPlayerScreenPosition());
            }
        }

    }
}
