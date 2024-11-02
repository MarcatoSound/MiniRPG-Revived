using BasicRPGTest_Mono.Engine.Entities;
using BasicRPGTest_Mono.Engine.Maps;
using BasicRPGTest_Mono.Engine.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using RPGEngine;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace BasicRPGTest_Mono.Engine.Graphics
{
    public class LightManager
    {
        private static bool initialized = false;
        private static Texture2D overlayPixel = new Texture2D(Core.graphics, 1, 1);
        public static float OverlayStrength { get; set; }
        /*{
            get { return (float)Overlay.A / 255; }
            set { Overlay = new Color(Overlay.R, Overlay.G, Overlay.B, (byte)(value * 255)); }
        }*/
        public static Color OverlayColor
        {
            get 
            {
                return new Color(Overlay.R, Overlay.G, Overlay.B);
            }
            set
            {
                Overlay = new Color(value.R, value.G, value.B);
            }
        }
        private static Color Overlay { get; set; } = Color.Black;

        static LightManager()
        {
            Camera.camera.CameraTileChange += (sender, args) =>
            {
                VisibleLights = getLightSourcesInBounds(Camera.camera.BoundingRectangle);
            };
        }

        // DEBUG METHOD, REMOVE LATER
        internal static void ToggleDarkness()
        {
            if (OverlayStrength <= 0.5)
            {
                OverlayStrength = 1;
            } else
            {
                OverlayStrength = 0;
            }
        }
        internal static void AddDarkness()
        {
            OverlayStrength += 0.1F;
            if (OverlayStrength > 1.01) OverlayStrength = 0;
        }


        // STORAGE
        public static RenderTarget2D LightMap { get; private set; }
        private static List<LightSource> LightSources { get; set; } = new List<LightSource>();
        private static List<LightSource> VisibleLights { get; set; } = new List<LightSource>();
        public static void AddLight(LightSource source)
        {
            LightSources.Add(source);
        }
        public static void RemoveLight(LightSource source)
        {
            LightSources.Remove(source);
        }
        public static bool HasLight(LightSource source)
        {
            return LightSources.Contains(source);
        }

        internal static void Update()
        {
            if (!initialized)
            {
                overlayPixel.SetData(new[] { Color.White });
                initialized = true;
            }

            SpriteBatch batch = new SpriteBatch(Core.graphics);

            if (LightMap != null) LightMap.Dispose();
            LightMap = new RenderTarget2D(Core.graphics, Core.graphics.Viewport.Width, Core.graphics.Viewport.Height);
            Core.graphics.SetRenderTarget(LightMap);
            Core.graphics.Clear(Color.White);

            /*BlendState state = new BlendState();
            state.AlphaSourceBlend = Blend.Zero;
            state.ColorSourceBlend = Blend.One;
            state.AlphaDestinationBlend = Blend.InverseSourceAlpha;
            state.ColorDestinationBlend = Blend.InverseSourceAlpha;
            state.AlphaSourceBlend = Blend.Zero;
            state.ColorSourceBlend = Blend.Zero;
            state.AlphaDestinationBlend = Blend.InverseSourceColor;
            state.ColorDestinationBlend = Blend.InverseSourceAlpha;
            BlendState Multiply = new BlendState()
            {
                AlphaSourceBlend = Blend.DestinationAlpha,
                AlphaDestinationBlend = Blend.Zero,
                AlphaBlendFunction = BlendFunction.Add,
                ColorSourceBlend = Blend.DestinationColor,
                ColorDestinationBlend = Blend.Zero,
                ColorBlendFunction = BlendFunction.Add
            };*/
            batch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied);
            batch.Draw(overlayPixel, Vector2.Zero, null, Overlay * OverlayStrength, 0, Vector2.Zero, new Vector2(Core.graphics.Viewport.Width, Core.graphics.Viewport.Height), SpriteEffects.None, 0);
            batch.End();
            batch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, transformMatrix: Camera.camera.Transform);

            //batch.DrawRectangle(Core.graphics.Viewport.Bounds, Overlay * OverlayStrength, 10);

            //List<LightSource> lightSources = getLightSourcesInBounds(Camera.camera.BoundingRectangle);
            if (OverlayStrength != 0)
            {
                foreach (LightSource source in VisibleLights)
                {
                    //if (!Camera.camera.IsInViewWithBuffer(source.Position, LightSource.GlowTexture, 64)) continue;
                    source.DrawGlow(batch, OverlayStrength);
                }
            }
            batch.End();

            //Util.saveTexture(target, "savedLight.png");

            Core.graphics.SetRenderTarget(null);
        }

        private static List<LightSource> getLightSourcesInBounds(Rectangle bounds)
        {
            Vector2 tileTopLeft = Util.getTilePosition(new Vector2(bounds.Left, bounds.Top));
            Rectangle tileBounds = new Rectangle((int)tileTopLeft.X, (int)tileTopLeft.Y, bounds.Width / TileManager.dimensions, bounds.Height / TileManager.dimensions);
            List<LightSource> lightSources = new List<LightSource>();

            // TILE LIGHTS
            for (int x = tileBounds.Left; x <= tileBounds.Right; x++)
            {
                for (int y = tileBounds.Top; y <= tileBounds.Bottom; y++)
                {
                    Tile tile;
                    if (!MapManager.activeMap.lightTiles.TryGetValue(new Vector2(x, y), out tile)) continue;
                    if (tile.GlowSize > 0) lightSources.Add(tile);
                }
            }

            // ENTITY LIGHTS
            foreach (LightSource light in LightSources)
            {
                if (light.GlowSize <= 0) continue;
                //if (!Camera.camera.IsInViewWithBuffer(light.Position, LightSource.GlowTexture, 64)) continue;
                lightSources.Add(light);
            }

            return lightSources;
        }
    }
}
