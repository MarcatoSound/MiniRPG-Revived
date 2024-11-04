using BasicRPGTest_Mono.Engine.Maps;
using BasicRPGTest_Mono.Engine.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using YamlDotNet.Core.Tokens;

namespace BasicRPGTest_Mono.Engine.Entities
{
    public interface LightSource
    {
        public static Texture2D GlowTexture = Util.loadTexture("glow.png");

        public float GlowBrightness { get; set; }
        public float GlowSize { get; set; }
        public Color GlowColor { get; set; }
        public Vector2 Position { get; set; }

        /*public void DrawGlow(SpriteBatch batch, float brightness = 1)
        {
            if (GlowSize == 0) return; // Redundant, as we should be filtering these out BEFORE iterating over light sources. But just in case.
            Color color = new Color(GlowColor, GlowBrightness * brightness);
            batch.Draw(GlowTexture, offsetPosition, null, color, 0, Vector2.Zero, GlowSize, SpriteEffects.None, 0);
        }*/
    }

    public static class ILightSourceExtensions
    {
        public static void DrawGlow(this LightSource lightSource, SpriteBatch batch, float brightness = 1)
        {
            DrawGlow(lightSource, batch, Vector2.Zero, brightness);
        }
        public static void DrawGlow(this LightSource lightSource, SpriteBatch batch, Vector2 offset, float brightness = 1)
        {
            if (lightSource.GlowSize == 0) return; // Redundant, as we should be filtering these out BEFORE iterating over light sources. But just in case.
            Vector2 offsetPosition = new Vector2(lightSource.Position.X - (LightSource.GlowTexture.Width * lightSource.GlowSize / 2) + offset.X, lightSource.Position.Y - (LightSource.GlowTexture.Height * lightSource.GlowSize / 2) + offset.X);
            Color color = new Color(lightSource.GlowColor, lightSource.GlowBrightness * brightness);
            batch.Draw(LightSource.GlowTexture, offsetPosition, null, color, 0, Vector2.Zero, lightSource.GlowSize, SpriteEffects.None, 0);
        }
    }
}
