using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using RPGEngine;
using System;
using System.Collections.Generic;
using System.Text;

namespace BasicRPGTest_Mono.Engine.Maps
{
    public class Region
    {
        // Region sizes should be in multiples of 4
        public const int regionSize = 16;

        public Vector2 pos { get; set; }
        public Vector2 regionPos { get; set; }
        public Rectangle box { get; set; }
        public List<Tile> tiles;

        public Region(Vector2 pos, Vector2 regionPos)
        {
            this.pos = pos;
            this.regionPos = regionPos;
            box = new Rectangle(Convert.ToInt32(pos.X), Convert.ToInt32(pos.Y), regionSize * TileManager.dimensions, regionSize * TileManager.dimensions);
            tiles = new List<Tile>();
        }

        public void addTiles(List<Tile> tiles)
        {
            this.tiles = tiles;
        }
        public void addTile(Tile tile)
        {
            this.tiles.Add(tile);
        }

        public void draw(SpriteBatch batch, TileLayer layer)
        {
            foreach (Tile tile in tiles)
            {
                if (tile.layer == layer)
                    tile.drawAdjacentTiles(batch);

            }
            batch.DrawRectangle(box, Color.White);
        }

    }
}
