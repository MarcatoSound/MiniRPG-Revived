using BasicRPGTest_Mono.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace RPGEngine
{
    public class Tile
    {
        public const int dimensions = 20;
        public bool isInstance { get; set; }
        public string id { get; set; }
        public Vector2 pos { get; set; }
        public Graphic graphic { get; set; }

        public Tile(string id, Texture2D texture)
        {
            this.id = id;
            this.isInstance = false;
            this.pos = new Vector2(0, 0);
            this.graphic = new Graphic(texture);
        }
        public Tile(string id, Graphic graphic)
        {
            this.id = id;
            this.isInstance = false;
            this.pos = new Vector2(0, 0);
            this.graphic = graphic;
        }

        public Tile(Tile tile)
        {
            this.id = tile.id;
            this.graphic = tile.graphic;
            this.isInstance = true;
        }

        public void draw(SpriteBatch spriteBatch)
        {
            if (!isInstance) return;
            graphic.draw(spriteBatch, pos);
        }

    }
}
