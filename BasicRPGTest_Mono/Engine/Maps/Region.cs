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
        public const int regionSize = 8;

        public Rectangle box { get; set; }
        public List<Tile> tiles;

        public Region(Vector2 pos)
        {
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


        public void draw(SpriteBatch batch)
        {
            foreach (Tile tile in tiles)
            {

                tile.draw(batch);

            }
            //batch.DrawRectangle(box, Color.White);
        }

    }
}
