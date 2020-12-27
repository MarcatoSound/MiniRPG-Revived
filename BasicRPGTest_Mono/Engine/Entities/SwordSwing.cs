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
    public class SwordSwing
    {
        public Player player;
        public Direction direction;
        public GraphicAnimated graphic;
        public Timer swingTimer;
        public Vector2 swordPos;
        public Rectangle hitBox;

        public SwordSwing(Direction direction, int swingTime, Player player)
        {
            this.player = player;
            this.direction = direction;
            GraphicAnimated newGraphic;
            SwingData.swings.TryGetValue(direction, out newGraphic);
            if (newGraphic == null) return;
            graphic = new GraphicAnimated(newGraphic);

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
            swordPos = new Vector2(position.X, position.Y);
            switch (direction)
            {
                case Direction.Up:
                    swordPos.Y -= 28;
                    hitBox = new Rectangle((int)swordPos.X - 24, (int)swordPos.Y - 16, 48, 32);
                    break;
                case Direction.Left:
                    swordPos.X -= 28;
                    hitBox = new Rectangle((int)swordPos.X - 16, (int)swordPos.Y - 24, 32, 48);
                    break;
                case Direction.Down:
                    swordPos.Y += 28;
                    hitBox = new Rectangle((int)swordPos.X - 24, (int)swordPos.Y - 16, 48, 32);
                    break;
                case Direction.Right:
                    swordPos.X += 28;
                    hitBox = new Rectangle((int)swordPos.X - 16, (int)swordPos.Y - 24, 32, 48);
                    break;
            }
            batch.Begin();
            batch.DrawRectangle(hitBox, Color.White);
            batch.End();


            graphic.draw(batch, swordPos);
        }
        public void Stop()
        {
            swingTimer.Stop();
            player.swordSwing = null;
        }
    }
}
