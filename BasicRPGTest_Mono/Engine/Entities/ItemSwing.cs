using BasicRPGTest_Mono.Engine.Items;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using RPGEngine;
using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;

namespace BasicRPGTest_Mono.Engine.Entities
{
    public static class SwingData
    {
        public static Dictionary<Direction, GraphicAnimated> swings;

        static SwingData()
        {
            swings = new Dictionary<Direction, GraphicAnimated>();
        }
    }
    public class ItemSwing
    {
        public Timer swingTimer;
        public Direction direction;
        public Player player;
        public Tool item;
        public GraphicAnimated graphic;
        public Vector2 swingPos;
        public Rectangle hitBox;
        public Rectangle itemBox;

        public ItemSwing(Direction direction, int swingTime, Player player, Tool tool)
        {
            this.player = player;
            this.direction = direction;
            this.item = tool;
            GraphicAnimated newGraphic;
            SwingData.swings.TryGetValue(direction, out newGraphic);
            if (newGraphic == null) return;
            graphic = new GraphicAnimated(newGraphic);
            itemBox = tool.hitbox;
            itemBox.X = itemBox.Width / 2;
            itemBox.Y = itemBox.Height / 2;

            swingTimer = new Timer(swingTime / 7);
            swingTimer.Elapsed += Update;
            swingTimer.Start();
        }
        public ItemSwing(Direction direction, int swingTime, Player player)
        {
            this.player = player;
            this.direction = direction;
            GraphicAnimated newGraphic;
            SwingData.swings.TryGetValue(direction, out newGraphic);
            if (newGraphic == null) return;
            graphic = new GraphicAnimated(newGraphic);

            itemBox = new Rectangle(0, 0, 32, 128);
            itemBox.X = itemBox.Width / 2;
            itemBox.Y = itemBox.Height / 2;

            swingTimer = new Timer(swingTime/7);
            swingTimer.Elapsed += Update;
            swingTimer.Start();
        }

        public void Update(Object source, ElapsedEventArgs args)
        {
            if (graphic.currentFrame == 6)
            {
                Stop();
                return;
            }
            graphic.update();
        }
        public void Draw(SpriteBatch batch, Vector2 position)
        {
            swingPos = new Vector2(position.X, position.Y);

            int offset = itemBox.Height - itemBox.Y - 16;
            if (offset < 0) offset = 0;
            //if () offset = itemBox.Height - itemBox.Height;

            switch (direction)
            {
                case Direction.Up:
                    swingPos.Y -= 28;
                    hitBox = new Rectangle((int)swingPos.X - itemBox.X, (int)swingPos.Y - itemBox.Y - offset, itemBox.Width, itemBox.Height);
                    break;
                case Direction.Left:
                    swingPos.X -= 28;
                    hitBox = new Rectangle((int)swingPos.X - itemBox.Y - offset, (int)swingPos.Y - itemBox.X, itemBox.Width - (itemBox.Width - itemBox.Height), itemBox.Height + (itemBox.Width - itemBox.Height));
                    break;
                case Direction.Down:
                    swingPos.Y += 28;
                    hitBox = new Rectangle((int)swingPos.X - itemBox.X, (int)swingPos.Y - itemBox.Y + offset, itemBox.Width, itemBox.Height);
                    break;
                case Direction.Right:
                    swingPos.X += 28;
                    hitBox = new Rectangle((int)swingPos.X - itemBox.Y + offset, (int)swingPos.Y - itemBox.X, itemBox.Width - (itemBox.Width - itemBox.Height), itemBox.Height + (itemBox.Width - itemBox.Height));
                    break;
            }
            batch.Begin();
            batch.DrawRectangle(hitBox, Color.White);
            batch.End();


            graphic.draw(batch, swingPos, Color.White);
        }
        public void Stop()
        {
            swingTimer.Stop();
            player.swordSwing = null;
        }
    }
}
