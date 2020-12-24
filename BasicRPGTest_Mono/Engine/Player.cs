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
        public Player(Texture2D texture, GraphicsDeviceManager graphics) : base(new GraphicAnimated(texture, 3, 4), new Rectangle(400, 240, 28, 26), graphics, 100f)
        {
            graphicsManager = graphics;
            position = new Vector2(400, 240);
        }


        public void move(GameTime gameTime)
        {
            Vector2 _cameraPosition = Core.camera.Position;

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

                if (newCameraPos.Y >= (MapManager.activeMap.tiledMap.HeightInPixels - Core.camera.BoundingRectangle.Height))
                    newCameraPos.Y = MapManager.activeMap.tiledMap.HeightInPixels - Core.camera.BoundingRectangle.Height;


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

                if (newCameraPos.X >= (MapManager.activeMap.tiledMap.WidthInPixels - Core.camera.BoundingRectangle.Width))
                    newCameraPos.X = MapManager.activeMap.tiledMap.WidthInPixels - Core.camera.BoundingRectangle.Width;


                if (isColliding(getBox(newPlayerPos)))
                {
                    newPlayerPos.X = position.X;
                    newCameraPos.X = _cameraPosition.X;
                }

            }

            position = new Vector2(newPlayerPos.X, newPlayerPos.Y);
            Core.camPos = new Vector2(newCameraPos.X, newCameraPos.Y);

        }



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
            int x = Convert.ToInt32(pos.X - (boundingBox.Width - (boundingBox.Width / 2)));
            int y = Convert.ToInt32(pos.Y - (boundingBox.Height - (graphic.height / 2)));

            Rectangle box = new Rectangle(x, y, boundingBox.Width, boundingBox.Height);

            return box;
        }

        public override void update()
        {
            boundingBox = getBox(position);
            base.update();
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
