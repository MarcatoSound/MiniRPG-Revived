using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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
            Vector2 swordPos = new Vector2(position.X, position.Y);
            switch (direction)
            {
                case Direction.Up:
                    swordPos.Y -= 28;
                    break;
                case Direction.Left:
                    swordPos.X -= 28;
                    break;
                case Direction.Down:
                    swordPos.Y += 28;
                    break;
                case Direction.Right:
                    swordPos.X += 28;
                    break;
            }
            graphic.draw(batch, swordPos);
        }
        public void Stop()
        {
            swingTimer.Stop();
            player.swordSwing = null;
        }
    }
}
