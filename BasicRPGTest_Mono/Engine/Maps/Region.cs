using BasicRPGTest_Mono.Engine.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using RPGEngine;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using YamlDotNet.RepresentationModel;
using YamlDotNet.Serialization;

namespace BasicRPGTest_Mono.Engine.Maps
{
    public class Region
    {
        // Region sizes should be in multiples of 4
        public const int regionSize = 32;

        public Map map { get; private set; }

        public bool isLoaded { get; set; } = false;
        public Vector2 pos { get; private set; }
        public Vector2 regionPos { get; private set; }
        public Rectangle box { get; set; }
        public List<Tile> tiles;

        public Region(Vector2 pos, Vector2 regionPos, Map map)
        {
            this.pos = pos;
            this.regionPos = regionPos;
            this.map = map;
            box = new Rectangle(Convert.ToInt32(pos.X), Convert.ToInt32(pos.Y), regionSize * TileManager.dimensions, regionSize * TileManager.dimensions);
            tiles = new List<Tile>();
        }

        public void addTile(Tile tile)
        {
            tiles.Add(tile);
        }
        public void removeTile(Tile mTile)
        {
            // Remove Tile from Region
            tiles.Remove(mTile);
            map.regionManager.updateRegion(this);
        }

        public void draw(SpriteBatch batch, TileLayer layer)
        {
            List<Tile> tiles = new List<Tile>(this.tiles);
            foreach (Tile tile in tiles)
            {
                /*if (tile.layer == layer)
                    tile.drawAdjacentTiles(batch);*/

                tile.draw(batch);

            }
            batch.DrawRectangle(box, Color.White);
        }


        public void save()
        {

            string path = $"save\\{map.world}\\maps\\{map.name}\\regions"; 
            if (!Directory.Exists(path))
            {
                DirectoryInfo dInfo = Directory.CreateDirectory(path);
            }

            StreamWriter writer = new StreamWriter($"{path}\\reg_{regionPos.X}-{regionPos.Y}.yml", false);

            try
            {
                writer.Write((YamlSection)this);
            }
            finally
            {
                writer.Close();
            }

        }


        public static implicit operator YamlSection(Region r)
        {
            YamlSection config = new YamlSection($"{r.regionPos.X}-{r.regionPos.Y}");

            config.setDouble("position.x", r.regionPos.X);
            config.setDouble("position.y", r.regionPos.Y);

            YamlSequenceNode sequence = new YamlSequenceNode();
            foreach (Tile tile in r.tiles)
            {
                sequence.Add((YamlSection)tile);
            }
            config.set("tiles", sequence);

            return config;
        }

    }
}
