using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;
using BasicRPGTest_Mono.Engine.GUI.Text;

namespace BasicRPGTest_Mono.Engine.GUI
{
    public class PopupText : IDisposable
    {
        public string text { get; set; }
        public TextColor textColor;
        public SpriteFont font { get; set; }
        public Vector2 pos;
        public int duration { get; set; }
        public Timer timer { get; set; }

        public int alpha { get; set; }
        public int alphaRate { get; set; }

        public PopupText(string text, SpriteFont font, Vector2 startPos, TextColor textColor, int duration)
        {
            this.text = text;
            this.font = font;
            this.pos = startPos;
            this.textColor = textColor;
            this.duration = duration;
            this.alpha = 512;
            this.alphaRate = Convert.ToInt32(((double)this.alpha / (double)duration) * 20);

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

        public virtual void draw(SpriteBatch batch)
        {
            textColor.update();

            if (alpha < 256)
            {
                textColor.color.A = (byte)alpha;
            }
            alpha = Math.Max(alpha - alphaRate, 0);

            batch.DrawString(font, text, pos, textColor);
        }

        public void Dispose()
        {
            textColor.Dispose();
        }
    }
}
