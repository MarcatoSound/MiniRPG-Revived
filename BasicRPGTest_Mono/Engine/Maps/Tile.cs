using BasicRPGTest_Mono.Engine;
using BasicRPGTest_Mono.Engine.Maps;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Text;

namespace RPGEngine
{
    public class Tile
    {
        public const int dimensions = 32;
        public string name { get; set; }
        public int id { get; set; }
        public Vector2 pos { get; set; } = new Vector2(0, 0);
        public Vector2 drawPos { get; set; } = new Vector2(0, 0);
        public Vector2 tilePos { get; set; } = new Vector2(0, 0);
        public Rectangle box { get; set; }
        public Graphic graphic { get; set; }

        public bool isCollidable { get; set; }
        public bool isInstance { get; set; } 

        public Tile(string name, Texture2D texture, bool collidable = false, bool instance = true)
        {
            id = TileManager.tiles.Count;
            this.name = name;
            graphic = new Graphic(texture);
            isCollidable = collidable;
            isInstance = instance;

            box = new Rectangle(Convert.ToInt32(pos.X), Convert.ToInt32(pos.Y), dimensions, dimensions);
        }
        public Tile(string name, Graphic graphic, bool collidable = false, bool instance = true)
        {
            id = TileManager.tiles.Count;
            this.name = name;
            this.graphic = graphic;
            isCollidable = collidable;
            isInstance = instance;

            box = new Rectangle(Convert.ToInt32(pos.X), Convert.ToInt32(pos.Y), dimensions, dimensions);
        }

        public Tile(Tile tile, Vector2 tilePos)
        {
            this.id = tile.id;
            this.graphic = tile.graphic;
            this.isCollidable = tile.isCollidable;
            this.isInstance = true;
            this.tilePos = tilePos;
            this.pos = new Vector2(tilePos.X * dimensions, tilePos.Y * dimensions);
            this.drawPos = new Vector2(pos.X + (dimensions / 2), pos.Y + (dimensions / 2));

            box = new Rectangle(Convert.ToInt32(pos.X), Convert.ToInt32(pos.Y), dimensions, dimensions);
        }

        public void draw(SpriteBatch spriteBatch)
        {
            if (!isInstance) return;
            graphic.draw(spriteBatch, drawPos, false);
        }

    }
}
