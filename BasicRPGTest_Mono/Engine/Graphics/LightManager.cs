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
    public static class LightManager
    {
        private static readonly BlendState lightBlend = new BlendState()
        {
            ColorBlendFunction = BlendFunction.Add,
            ColorSourceBlend = Blend.DestinationColor,
            ColorDestinationBlend = Blend.Zero,
        };
        private static readonly Vector2 lightOffset = new Vector2(-TileManager.dimensions, -TileManager.dimensions);


        private static bool initialized = false;
        private static Texture2D overlayPixel = new Texture2D(Core.graphics, 1, 1);
        public static float OverlayStrength { get; set; }
        private static Color Overlay { get; set; } = Color.Black;

        static LightManager()
        {
            Camera.camera.CameraTileChange += (sender, args) => UpdateStatic();
        }

        // DEBUG METHODS, REMOVE LATER
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
        public static RenderTarget2D StaticLightMap { get; private set; }
        public static RenderTarget2D LightMap { get; private set; }
        private static List<LightSource> DynamicLights { get; set; } = new List<LightSource>();
        private static LightSource[] VisibleLights { get; set; } = Array.Empty<LightSource>();
        internal static Vector2 LightMapPosition { get; set; }
        public static void AddDynamicLight(LightSource source) => DynamicLights.Add(source);
        public static void RemoveDynamicLight(LightSource source) => DynamicLights.Remove(source);
        public static bool HasDynamicLight(LightSource source) => DynamicLights.Contains(source);


        // FUNCTIONALITY
        internal static void Draw()
        {
            if (StaticLightMap is null) return;

            SpriteBatch batch = new SpriteBatch(Core.graphics);

            batch.Begin(SpriteSortMode.Immediate, lightBlend);
            batch.Draw(LightMap, Vector2.Zero, Color.White);
            batch.End();
        }


        internal static void UpdateStatic()
        {
            if (!initialized)
            {
                overlayPixel.SetData(new[] { Color.White });
                initialized = true;
            }

            Rectangle lightbounds = new Rectangle(Camera.camera.BoundingRectangle.Left - TileManager.dimensions, Camera.camera.BoundingRectangle.Top - TileManager.dimensions, Camera.camera.BoundingRectangle.Width + TileManager.dimensions, Camera.camera.BoundingRectangle.Height + TileManager.dimensions);
            //lightbounds.Inflate(TileManager.dimensions, TileManager.dimensions);
            VisibleLights = getLightSourcesInBounds(lightbounds);

            SpriteBatch batch = new SpriteBatch(Core.graphics);

            if (StaticLightMap != null) StaticLightMap.Dispose();
            StaticLightMap = new RenderTarget2D(Core.graphics, Core.graphics.Viewport.Width + 64, Core.graphics.Viewport.Height + 64);
            Core.graphics.SetRenderTarget(StaticLightMap);
            Core.graphics.Clear(Color.White);

            batch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied);
            batch.Draw(overlayPixel, Vector2.Zero, null, Overlay * OverlayStrength, 0, Vector2.Zero, new Vector2(Core.graphics.Viewport.Width + 64, Core.graphics.Viewport.Height + 64), SpriteEffects.None, 0);
            batch.End();
            batch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, transformMatrix: Camera.camera.Transform);

            if (OverlayStrength != 0)
            {
                foreach (LightSource source in VisibleLights)
                {
                    source.DrawGlow(batch, new Vector2(TileManager.dimensions, TileManager.dimensions), OverlayStrength);
                }
            }
            batch.End();

            LightMapPosition = new Vector2(Camera.camera.BoundingRectangle.Left - TileManager.dimensions, Camera.camera.BoundingRectangle.Top - TileManager.dimensions);

            Core.graphics.SetRenderTarget(null);
        }

        internal static void UpdateDynamic()
        {
            SpriteBatch batch = new SpriteBatch(Core.graphics);

            if (LightMap is not null) LightMap.Dispose(); // Clean up the old render target to avoid memory leaking.
            LightMap = new RenderTarget2D(Core.graphics, Core.graphics.Viewport.Width + 64, Core.graphics.Viewport.Height + 64);

            Core.graphics.SetRenderTarget(LightMap);
            Core.graphics.Clear(Color.Transparent);

            batch.Begin(SpriteSortMode.Deferred, transformMatrix: Camera.camera.Transform);
            batch.Draw(StaticLightMap, LightMapPosition, Color.White);
            batch.End();

            batch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, transformMatrix: Camera.camera.Transform);
            // DYNAMIC LIGHTS - Lights that need to have their position updated every frame.
            foreach (LightSource light in DynamicLights)
            {
                if (light.GlowSize <= 0) continue;
                light.DrawGlow(batch);
            }
            batch.End();

            Core.graphics.SetRenderTarget(null);
        }


        private static LightSource[] getLightSourcesInBounds(Rectangle bounds)
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

            return lightSources.ToArray();
        }
    }
}
