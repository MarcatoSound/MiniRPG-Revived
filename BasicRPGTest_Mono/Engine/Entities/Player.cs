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

namespace BasicRPGTest_Mono.Engine
{
    public class Player : LivingEntity
    {
        public Player(Texture2D texture, GraphicsDeviceManager graphics) : base(new GraphicAnimated(texture, 3, 4), new Rectangle(0, 0, 28, 26), graphics, 100f)
        {
            graphicsManager = graphics;
            position = new Vector2(MapManager.activeMap.tiledMap.WidthInPixels / 2, MapManager.activeMap.tiledMap.HeightInPixels / 2);
            boundingBox = new Rectangle((int)position.X, (int)position.Y, 28, 26);
            Camera.camPos = new Vector2(position.X - (Camera.camera.BoundingRectangle.Width / 2), position.Y - (Camera.camera.BoundingRectangle.Height / 2));
            maxVelocity = new Vector2(100, 100);
        }

        public override void move()
        {
            Vector2 _cameraPosition = Camera.camera.Position;
            Vector2 newPlayerPos = position;
            Vector2 newCameraPos = _cameraPosition;

            if (velocity.X > 0)
            {

                newPlayerPos.X += velocity.X * (float)Core.globalTime.ElapsedGameTime.TotalSeconds;

                if (newPlayerPos.X > (MapManager.activeMap.tiledMap.WidthInPixels - (graphic.width / 2)))
                    newPlayerPos.X = MapManager.activeMap.tiledMap.WidthInPixels - (graphic.width / 2);

                if (!(newPlayerPos.X <= graphicsManager.PreferredBackBufferWidth / 2))
                    newCameraPos.X += velocity.X * (float)Core.globalTime.ElapsedGameTime.TotalSeconds;

                if (newCameraPos.X >= (MapManager.activeMap.tiledMap.WidthInPixels - Camera.camera.BoundingRectangle.Width))
                    newCameraPos.X = MapManager.activeMap.tiledMap.WidthInPixels - Camera.camera.BoundingRectangle.Width;


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

                if (!(newPlayerPos.X >= MapManager.activeMap.tiledMap.WidthInPixels - (graphicsManager.PreferredBackBufferWidth / 2)))
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

                if (newPlayerPos.Y > (MapManager.activeMap.tiledMap.HeightInPixels - (graphic.width / 2)))
                    newPlayerPos.Y = MapManager.activeMap.tiledMap.HeightInPixels - (graphic.width / 2);

                if (!(newPlayerPos.Y <= graphicsManager.PreferredBackBufferHeight / 2))
                    newCameraPos.Y += velocity.Y * (float)Core.globalTime.ElapsedGameTime.TotalSeconds;

                if (newCameraPos.Y >= (MapManager.activeMap.tiledMap.HeightInPixels - Camera.camera.BoundingRectangle.Height))
                    newCameraPos.Y = MapManager.activeMap.tiledMap.HeightInPixels - Camera.camera.BoundingRectangle.Height;


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

                if (!(newPlayerPos.Y >= MapManager.activeMap.tiledMap.HeightInPixels - (graphicsManager.PreferredBackBufferHeight / 2)))
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
        /*public void move(GameTime gameTime)
        {
            Vector2 _cameraPosition = Camera.camera.Position;

            var kstate = Keyboard.GetState();
            Vector2 newPlayerPos = position;
            Vector2 newCameraPos = _cameraPosition;

            if (kstate.IsKeyDown(Keys.Up))
            {

                newPlayerPos.Y -= speed * (float)gameTime.ElapsedGameTime.TotalSeconds;

                if (newPlayerPos.Y < 0 + (graphic.height / 2))
                    newPlayerPos.Y = 0 + (graphic.height / 2);

                if (!(newPlayerPos.Y >= MapManager.activeMap.tiledMap.HeightInPixels - (graphicsManager.PreferredBackBufferHeight / 2)))
                    newCameraPos.Y -= speed * (float)gameTime.ElapsedGameTime.TotalSeconds;

                if (newCameraPos.Y <= 0)
                    newCameraPos.Y = 0;


                if (isColliding(getBox(newPlayerPos)))
                {
                    newPlayerPos.Y = position.Y;
                    newCameraPos.Y = _cameraPosition.Y;
                }

            }

            if (kstate.IsKeyDown(Keys.Down))
            {

                newPlayerPos.Y += speed * (float)gameTime.ElapsedGameTime.TotalSeconds;

                if (newPlayerPos.Y > (MapManager.activeMap.tiledMap.HeightInPixels - (graphic.width / 2)))
                    newPlayerPos.Y = MapManager.activeMap.tiledMap.HeightInPixels - (graphic.width / 2);

                if (!(newPlayerPos.Y <= graphicsManager.PreferredBackBufferHeight / 2))
                    newCameraPos.Y += speed * (float)gameTime.ElapsedGameTime.TotalSeconds;

                if (newCameraPos.Y >= (MapManager.activeMap.tiledMap.HeightInPixels - Camera.camera.BoundingRectangle.Height))
                    newCameraPos.Y = MapManager.activeMap.tiledMap.HeightInPixels - Camera.camera.BoundingRectangle.Height;


                if (isColliding(getBox(newPlayerPos)))
                {
                    newPlayerPos.Y = position.Y;
                    newCameraPos.Y = _cameraPosition.Y;
                }

            }

            if (kstate.IsKeyDown(Keys.Left))
            {

                newPlayerPos.X -= speed * (float)gameTime.ElapsedGameTime.TotalSeconds;

                if (newPlayerPos.X < 0 + (graphic.width / 2))
                    newPlayerPos.X = 0 + (graphic.width / 2);

                if (!(newPlayerPos.X >= MapManager.activeMap.tiledMap.WidthInPixels - (graphicsManager.PreferredBackBufferWidth / 2)))
                    newCameraPos.X -= speed * (float)gameTime.ElapsedGameTime.TotalSeconds;

                if (newCameraPos.X <= 0)
                    newCameraPos.X = 0;


                if (isColliding(getBox(newPlayerPos)))
                {
                    newPlayerPos.X = position.X;
                    newCameraPos.X = _cameraPosition.X;
                }

            }

            if (kstate.IsKeyDown(Keys.Right))
            {


                newPlayerPos.X += speed * (float)gameTime.ElapsedGameTime.TotalSeconds;

                if (newPlayerPos.X > (MapManager.activeMap.tiledMap.WidthInPixels - (graphic.width / 2)))
                    newPlayerPos.X = MapManager.activeMap.tiledMap.WidthInPixels - (graphic.width / 2);

                if (!(newPlayerPos.X <= graphicsManager.PreferredBackBufferWidth / 2))
                    newCameraPos.X += speed * (float)gameTime.ElapsedGameTime.TotalSeconds;

                if (newCameraPos.X >= (MapManager.activeMap.tiledMap.WidthInPixels - Camera.camera.BoundingRectangle.Width))
                    newCameraPos.X = MapManager.activeMap.tiledMap.WidthInPixels - Camera.camera.BoundingRectangle.Width;


                if (isColliding(getBox(newPlayerPos)))
                {
                    newPlayerPos.X = position.X;
                    newCameraPos.X = _cameraPosition.X;
                }

            }

            position = new Vector2(newPlayerPos.X, newPlayerPos.Y);
            Camera.camPos = new Vector2(newCameraPos.X, newCameraPos.Y);

        }*/



        public Vector2 getPlayerTilePosition()
        {
            Vector2 pos = new Vector2(position.X, position.Y);

            pos.X = (int)pos.X / MapManager.activeMap.tiledMap.TileWidth;
            pos.Y = (int)pos.Y / MapManager.activeMap.tiledMap.TileHeight;

            return pos;
        }
        public Vector2 getPlayerTilePosition(Vector2 truePos)
        {
            Vector2 pos = new Vector2(truePos.X, truePos.Y);

            pos.X = (int)pos.X / MapManager.activeMap.tiledMap.TileWidth;
            pos.Y = (int)pos.Y / MapManager.activeMap.tiledMap.TileHeight;

            return pos;
        }
        public Vector2 getPlayerTilePositionPrecise()
        {
            Vector2 pos = new Vector2(position.X, position.Y);

            pos.X = pos.X / MapManager.activeMap.tiledMap.TileWidth;
            pos.Y = pos.Y / MapManager.activeMap.tiledMap.TileHeight;

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
            if (position.X > MapManager.activeMap.tiledMap.WidthInPixels - pos.X) pos.X = pos.X + (position.X - (MapManager.activeMap.tiledMap.WidthInPixels - pos.X));
            if (position.Y > MapManager.activeMap.tiledMap.HeightInPixels - pos.Y) pos.Y = pos.Y + (position.Y - (MapManager.activeMap.tiledMap.HeightInPixels - pos.Y));

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

            var kstate = Keyboard.GetState();
            Vector2 newVel = velocity;
            if (kstate.IsKeyDown(Keys.Up))
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

            if (kstate.IsKeyDown(Keys.Down))
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

            if (kstate.IsKeyDown(Keys.Left))
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

            if(kstate.IsKeyDown(Keys.Right))
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

            move();

        }
        public override void draw(SpriteBatch batch)
        {
            Vector2 screenPos = getPlayerScreenPosition();
            graphic.draw(batch, screenPos);

            batch.Begin();
            batch.DrawRectangle(getScreenBox(), Color.White);
            batch.End();
        }

    }
}
