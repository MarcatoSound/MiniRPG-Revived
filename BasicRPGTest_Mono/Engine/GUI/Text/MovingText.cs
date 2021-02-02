using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace BasicRPGTest_Mono.Engine.GUI.Text
{
    class MovingText : PopupText
    {
        private Direction direction;
        private float rate;
        // No color, no font.
        public MovingText(string text, Vector2 startPos, int duration, Direction direction = Direction.Up, float rate = 0.65f) : 
            this(text, FontLibrary.getFont("dmg"), startPos, Color.White, duration, direction, rate) { }
        // No font
        public MovingText(string text, Vector2 startPos, TextColor textColor, int duration, Direction direction = Direction.Up, float rate = 0.65f) : 
            this(text, FontLibrary.getFont("dmg"), startPos, textColor, duration, direction, rate) { }
        // No color
        public MovingText(string text, SpriteFont font, Vector2 startPos, int duration, Direction direction = Direction.Up, float rate = 0.65f) : 
            this(text, font, startPos, Color.White, duration, direction, rate) { }

        // MASTER
        public MovingText(string text, SpriteFont font, Vector2 startPos, TextColor textColor, int duration, Direction direction = Direction.Up, float rate = 0.65f) : base(text, font, startPos, textColor, duration)
        {
            this.direction = direction;
            this.rate = rate;
        }

        public override void draw(SpriteBatch batch)
        {
            base.draw(batch);

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
