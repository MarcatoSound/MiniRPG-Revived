using BasicRPGTest_Mono.Engine;
using BasicRPGTest_Mono.Engine.Maps;
using BasicRPGTest_Mono.Engine.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;

namespace RPGEngine
{
    public class Tile
    {
        //====================================================================================
        // VARIABLES
        //====================================================================================

        public Map map;

        public const int dimensions = 32;
        public string name { get; set; }
        public int id { get; set; }
        public Graphic graphic { get; set; }
        public Dictionary<TileSide, Graphic> sideGraphics { get; set; }
        public Rectangle box { get; set; }
        public bool isCollidable { get; set; }
        public int zIndex { get; set; }

        public double maxHealth { get; set; }
        public bool destructable { get; set; }

        // Instance variables
        public bool isInstance { get; set; }
        public Tile parent { get; set; }
        public Vector2 pos { get; set; } = new Vector2(0, 0);
        public Vector2 drawPos { get; set; } = new Vector2(0, 0);
        public Vector2 tilePos { get; set; } = new Vector2(0, 0);
        public Vector2 region { get; set; } = new Vector2(0, 0);
        public TileLayer layer { get; set; }
        public Dictionary<TileSide, bool> sides { get; set; }
        public Biome biome { get; set; }

        private double _health;
        public double health
        {
            get { return _health; }
            set
            {
                if (value <= 0)
                {
                    Destroy();
                }
                else
                    _health = value;
            }
        }
        public bool isBeingDamaged;

        public Tile(string name, Texture2D texture, bool collidable = false, bool instance = true, int z = 1, double maxHP = 20, bool destructable = true)
        {
            id = TileManager.tiles.Count;
            this.name = name;
            this.zIndex = z;
            isCollidable = collidable;
            isInstance = instance;
            sideGraphics = new Dictionary<TileSide, Graphic>();
            this.maxHealth = maxHP;
            this.destructable = destructable;

            if (texture.Width > dimensions)
            {
                graphic = new Graphic(Util.getSpriteFromSet(texture, 1, 1));
                sideGraphics.Add(TileSide.NorthWest, new Graphic(Util.getSpriteFromSet(texture, 0, 0)));
                sideGraphics.Add(TileSide.North, new Graphic(Util.getSpriteFromSet(texture, 0, 1)));
                sideGraphics.Add(TileSide.NorthEast, new Graphic(Util.getSpriteFromSet(texture, 0, 2)));
                sideGraphics.Add(TileSide.West, new Graphic(Util.getSpriteFromSet(texture, 1, 0)));
                sideGraphics.Add(TileSide.East, new Graphic(Util.getSpriteFromSet(texture, 1, 2)));
                sideGraphics.Add(TileSide.SouthWest, new Graphic(Util.getSpriteFromSet(texture, 2, 0)));
                sideGraphics.Add(TileSide.South, new Graphic(Util.getSpriteFromSet(texture, 2, 1)));
                sideGraphics.Add(TileSide.SouthEast, new Graphic(Util.getSpriteFromSet(texture, 2, 2)));
            }
            else
            {
                graphic = new Graphic(texture);
            }

            box = new Rectangle(Convert.ToInt32(pos.X), Convert.ToInt32(pos.Y), dimensions, dimensions);
        }

        // For instantiating an existing tile
        public Tile(Tile tile, Vector2 tilePos, Biome biome)
        {

            this.parent = tile;
            this.name = tile.name;
            this.id = tile.id;
            this.isCollidable = tile.isCollidable;
            this.isInstance = true;
            this.zIndex = tile.zIndex;
            this.tilePos = tilePos;
            this.biome = biome;
            this.pos = new Vector2(tilePos.X * dimensions, tilePos.Y * dimensions);
            this.drawPos = new Vector2(pos.X + (dimensions / 2), pos.Y + (dimensions / 2));
            sideGraphics = tile.sideGraphics;
            sides = new Dictionary<TileSide, bool>();

            box = new Rectangle(Convert.ToInt32(pos.X), Convert.ToInt32(pos.Y), dimensions, dimensions);

            maxHealth = tile.maxHealth;
            health = maxHealth;
            this.destructable = tile.destructable;

        }

        private Graphic getSideGraphic(TileSide side)
        {
            if (sideGraphics.ContainsKey(side))
                return sideGraphics[side];
            else
                return null;
        }

        public void drawAdjacentTiles(SpriteBatch batch)
        {
            if (sideGraphics.Count == 0) return;
            Vector2 drawPos;
            foreach (KeyValuePair<TileSide, bool> pair in sides)
            {
                bool draw = pair.Value;
                if (!draw) continue;
                TileSide side = pair.Key;
                if (getSideGraphic(side) == null) continue;
                drawPos = pos;

                switch (side)
                {
                    case TileSide.NorthWest:
                        drawPos.X = drawPos.X - TileManager.dimensions;
                        drawPos.Y = drawPos.Y - TileManager.dimensions;
                        break;
                    case TileSide.North:
                        drawPos.Y = drawPos.Y - TileManager.dimensions;
                        break;
                    case TileSide.NorthEast:
                        drawPos.X = drawPos.X + TileManager.dimensions;
                        drawPos.Y = drawPos.Y - TileManager.dimensions;
                        break;
                    case TileSide.West:
                        drawPos.X = drawPos.X - TileManager.dimensions;
                        break;
                    case TileSide.East:
                        drawPos.X = drawPos.X + TileManager.dimensions;
                        break;
                    case TileSide.SouthWest:
                        drawPos.X = drawPos.X - TileManager.dimensions;
                        drawPos.Y = drawPos.Y + TileManager.dimensions;
                        break;
                    case TileSide.South:
                        drawPos.Y = drawPos.Y + TileManager.dimensions;
                        break;
                    case TileSide.SouthEast:
                        drawPos.X = drawPos.X + TileManager.dimensions;
                        drawPos.Y = drawPos.Y + TileManager.dimensions;
                        break;
                }

                sideGraphics[side].draw(batch, drawPos, 0f, Vector2.Zero, 1, false, 0);
                //batch.DrawRectangle(new Rectangle(Convert.ToInt32(drawPos.X), Convert.ToInt32(drawPos.Y), 32, 32), Color.White);
            }

            return;

        }

        public void update()
        {
            if (sideGraphics.Count == 0) return;
            Graphic graphic;
            Vector2 checkPos;
            Tile checkTile;
            foreach (TileSide side in Enum.GetValues(typeof(TileSide)))
            {
                graphic = getSideGraphic(side);
                if (graphic == null) continue;
                checkPos = tilePos;
                switch (side)
                {
                    case TileSide.NorthWest:
                        checkPos.X -= 1;
                        checkPos.Y -= 1;
                        break;
                    case TileSide.North:
                        checkPos.Y -= 1;
                        break;
                    case TileSide.NorthEast:
                        checkPos.X += 1;
                        checkPos.Y -= 1;
                        break;
                    case TileSide.West:
                        checkPos.X -= 1;
                        break;
                    case TileSide.East:
                        checkPos.X += 1;
                        break;
                    case TileSide.SouthWest:
                        checkPos.X -= 1;
                        checkPos.Y += 1;
                        break;
                    case TileSide.South:
                        checkPos.Y += 1;
                        break;
                    case TileSide.SouthEast:
                        checkPos.X += 1;
                        checkPos.Y += 1;
                        break;
                }
                checkTile = layer.getTile(checkPos);
                if (checkTile == null || checkTile.zIndex < zIndex)
                {
                    sides.Remove(side);
                    sides.Add(side, true);
                }
                else
                {
                    sides.Remove(side);
                    sides.Add(side, false);
                }
            }

            return;
        }

        public void draw(SpriteBatch batch)
        {
            if (!isInstance) return;
            /*parent.graphic.draw(batch, pos, 0f, Vector2.Zero, 1, false, 0.1f);
            drawAdjacentTiles(batch);*/
            batch.DrawRectangle(box, Color.Red);
        }


        public void Damage(double dmg)
        {
            health -= dmg;
            // Later for handling coloration or breaking graphic?
            isBeingDamaged = true;
            Timer timer = new Timer(500);
            timer.Elapsed += (sender, args) =>
            {
                isBeingDamaged = false;
                timer.Stop();
            };
            timer.Start();
        }
        public void Heal(double gain)
        {
            health += gain;
        }
        public void Destroy()
        {
            map.removeTile(this);

            List<Tile> surroundings = Util.getSurroundingTiles(map, 2, tilePos);
            foreach (Tile tile in surroundings)
            {
                if (tile == null) continue;
                if (tile.sideGraphics.Count == 0) continue;
                tile.update();
            }
            
        }



        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                //System.Diagnostics.Debug.WriteLine("Provided tile is NULL!");
                return false;
            }
            if (obj.GetType() != typeof(Tile))
            {
                //System.Diagnostics.Debug.WriteLine("Provided tile is not a tile at all!");
                return false;
            }
            Tile compare = (Tile)obj;


            if (this.tilePos != compare.tilePos)
            {
                //System.Diagnostics.Debug.WriteLine("TilePos is different!");
                return false;
            }

            if (this.name != compare.name)
            {
                //System.Diagnostics.Debug.WriteLine("Name is different!");
                return false;
            }

            if (this.parent != compare.parent)
            {
                //System.Diagnostics.Debug.WriteLine("Parent is different!");
                return false;
            }

            if (this.layer != compare.layer)
            {
                //System.Diagnostics.Debug.WriteLine("Layer is different!");
                return false;
            }

            if (this.graphic != compare.graphic)
            {
                //System.Diagnostics.Debug.WriteLine("Graphic is different!");
                return false;
            }


            return true;

        }

    }


    public enum TileSide
    {
        NorthWest,
        North,
        NorthEast,
        West,
        East,
        SouthWest,
        South,
        SouthEast
    }
}
