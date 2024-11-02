using BasicRPGTest_Mono.Engine.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace BasicRPGTest_Mono.Engine.Entities
{
    public class LightSource
    {
        public static Texture2D GlowTexture = Util.loadTexture("glow.png");

        public float GlowBrightness { get; set; } = 1;
        public float GlowSize { get; set; } = 0;
        public Color GlowColor { get; set; } = Color.White;
        private Vector2 _position;
        public virtual Vector2 Position
        {
            get { return _position; }
            set 
            { 
                _position = value;
                offsetPosition = new Vector2(value.X - (GlowTexture.Width * GlowSize / 2), value.Y - (GlowTexture.Height * GlowSize / 2));
            }
        }
        private Vector2 offsetPosition;

        public void DrawGlow(SpriteBatch batch, float brightness = 1)
        {
            if (GlowSize == 0) return; // Redundant, as we should be filtering these out BEFORE iterating over light sources. But just in case.
            Color color = new Color(GlowColor, GlowBrightness * brightness);
            batch.Draw(GlowTexture, offsetPosition, null, color, 0, Vector2.Zero, GlowSize, SpriteEffects.None, 0);
        }
    }
}
