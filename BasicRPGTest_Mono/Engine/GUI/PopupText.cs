using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;

namespace BasicRPGTest_Mono.Engine.GUI
{
    public class PopupText : IDisposable
    {
        public string text { get; set; }
        public Color color;
        public SpriteFont font { get; set; }
        public Vector2 pos;
        public int duration { get; set; }
        public Timer timer { get; set; }

        public int alpha { get; set; }

        public PopupText(string text, SpriteFont font, Vector2 startPos, Color color, int duration)
        {
            this.text = text;
            this.font = font;
            this.pos = startPos;
            this.color = color;
            this.duration = duration;
            this.alpha = 512;

            Core.popupTexts.Add(this);
            timer = new Timer(duration);
            timer.Elapsed += (sender, args) =>
            {
                timer.Stop();
                Core.popupTexts.Remove(this);
                Dispose();
            };
            timer.Start();

        }

        public void draw(SpriteBatch batch)
        {
            batch.DrawString(font, text, pos, color);

            if (alpha < 256 && alpha >= 0)
            {
                color.A = (byte)alpha;
            }
            alpha -= 20;
            pos.Y -= 0.65f;
        }

        public void Dispose()
        {

        }
    }
}
