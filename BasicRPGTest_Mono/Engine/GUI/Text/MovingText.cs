using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;

namespace BasicRPGTest_Mono.Engine.GUI.Text
{
    class MovingText : PopupText
    {
        private Direction direction;
        private int delay;
        private float rate;

        private Timer delayTracker;
        // No color, no font.
        public MovingText(string text, Vector2 startPos, int duration, Direction direction = Direction.Up, float rate = 0.65f, int moveDelay = 0, bool anchor = false) : 
            this(text, FontLibrary.getFont("dmg"), startPos, Color.White, duration, direction, rate, moveDelay, anchor) { }
        // No font
        public MovingText(string text, Vector2 startPos, TextColor textColor, int duration, Direction direction = Direction.Up, float rate = 0.65f, int moveDelay = 0, bool anchor = false) : 
            this(text, FontLibrary.getFont("dmg"), startPos, textColor, duration, direction, rate, moveDelay, anchor) { }
        // No color
        public MovingText(string text, SpriteFont font, Vector2 startPos, int duration, Direction direction = Direction.Up, float rate = 0.65f, int moveDelay = 0, bool anchor = false) : 
            this(text, font, startPos, Color.White, duration, direction, rate, moveDelay, anchor) { }

        // MASTER
        public MovingText(string text, SpriteFont font, Vector2 startPos, TextColor textColor, int duration, Direction direction = Direction.Up, float rate = 0.65f, int moveDelay = 0, bool anchor = false) : base(text, font, startPos, textColor, duration, anchor)
        {
            this.direction = direction;
            this.delay = moveDelay;
            this.rate = rate;

            delayTracker = new Timer(delay + 1);
            delayTracker.Elapsed += (sender, args) =>
            {
                delayTracker.Stop();
                delayTracker.Enabled = false;
            };
            delayTracker.Start();
        }

        public override void draw(SpriteBatch batch)
        {
            base.draw(batch);

            if (!delayTracker.Enabled)
            {
                switch (direction)
                {
                    case Direction.Up:
                        pos.Y -= rate;
                        break;
                    case Direction.Down:
                        pos.Y += rate;
                        break;
                    case Direction.Left:
                        pos.X -= rate;
                        break;
                    case Direction.Right:
                        pos.X += rate;
                        break;
                }
            }

        }
    }
}
