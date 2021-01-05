using BasicRPGTest_Mono.Engine;
using BasicRPGTest_Mono.Engine.Maps;
using BasicRPGTest_Mono.Engine.Utility;
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
        public Tile parent;
        public const int dimensions = 32;
        public string name { get; set; }
        public int id { get; set; }
        public Vector2 pos { get; set; } = new Vector2(0, 0);
        public Vector2 drawPos { get; set; } = new Vector2(0, 0);
        public Vector2 tilePos { get; set; } = new Vector2(0, 0);
        public Vector2 region { get; set; } = new Vector2(0, 0);
        public TileLayer layer { get; set; }
        public Rectangle box { get; set; }
        public Graphic graphic { get; set; }
        public Dictionary<TileSide, Graphic> sideGraphics { get; set; }
        public Dictionary<TileSide, bool> sides { get; set; }

        public bool isCollidable { get; set; }
        public bool isInstance { get; set; } 

        public Tile(string name, Texture2D texture, bool collidable = false, bool instance = true)
        {
            id = TileManager.tiles.Count;
            this.name = name;
            graphic = new Graphic(texture);
            isCollidable = collidable;
            isInstance = instance;
            sideGraphics = new Dictionary<TileSide, Graphic>();

            box = new Rectangle(Convert.ToInt32(pos.X), Convert.ToInt32(pos.Y), dimensions, dimensions);
        }
        public Tile(string name, Graphic graphic, bool collidable = false, bool instance = true)
        {
            id = TileManager.tiles.Count;
            this.name = name;
            this.graphic = graphic;
            isCollidable = collidable;
            isInstance = instance;
            sideGraphics = new Dictionary<TileSide, Graphic>();

            box = new Rectangle(Convert.ToInt32(pos.X), Convert.ToInt32(pos.Y), dimensions, dimensions);
        }
        public Tile(string name, Texture2D texture, bool useSides, bool collidable = false, bool instance = true)
        {
            // TODO Replace this constructor with a check for if the texture provided is larger than the tile dimensions.
            this.name = name;
            graphic = new Graphic(Util.getSpriteFromSet(texture, 1, 1));
            isCollidable = collidable;
            isInstance = instance;
            sideGraphics = new Dictionary<TileSide, Graphic>();

            sideGraphics.Add(TileSide.NorthWest, new Graphic(Util.getSpriteFromSet(texture, 0, 0)));
            sideGraphics.Add(TileSide.North, new Graphic(Util.getSpriteFromSet(texture, 0, 1)));
            sideGraphics.Add(TileSide.NorthEast, new Graphic(Util.getSpriteFromSet(texture, 0, 2)));
            sideGraphics.Add(TileSide.West, new Graphic(Util.getSpriteFromSet(texture, 1, 0)));
            sideGraphics.Add(TileSide.East, new Graphic(Util.getSpriteFromSet(texture, 1, 2)));
            sideGraphics.Add(TileSide.SouthWest, new Graphic(Util.getSpriteFromSet(texture, 2, 0)));
            sideGraphics.Add(TileSide.South, new Graphic(Util.getSpriteFromSet(texture, 2, 1)));
            sideGraphics.Add(TileSide.SouthEast, new Graphic(Util.getSpriteFromSet(texture, 2, 2)));

            box = new Rectangle(Convert.ToInt32(pos.X), Convert.ToInt32(pos.Y), dimensions, dimensions);
        }

        public Tile(Tile tile, Vector2 tilePos)
        {
            this.parent = tile;
            this.id = tile.id;
            this.isCollidable = tile.isCollidable;
            this.isInstance = true;
            this.tilePos = tilePos;
            this.pos = new Vector2(tilePos.X * dimensions, tilePos.Y * dimensions);
            this.drawPos = new Vector2(pos.X + (dimensions / 2), pos.Y + (dimensions / 2));
            sideGraphics = tile.sideGraphics;
            sides = new Dictionary<TileSide, bool>();

            box = new Rectangle(Convert.ToInt32(pos.X), Convert.ToInt32(pos.Y), dimensions, dimensions);

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

            Vector2 drawPos;
            foreach (KeyValuePair<TileSide, bool> pair in sides)
            {
                bool draw = pair.Value;
                if (!draw) continue;
                TileSide side = pair.Key;
                if (getSideGraphic(side) == null) continue;
                drawPos = this.drawPos;

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
                sideGraphics[side].draw(batch, drawPos, false);
                //batch.DrawRectangle(new Rectangle(Convert.ToInt32(drawPos.X), Convert.ToInt32(drawPos.Y), 32, 32), Color.White);
            }

            return;

            //

            // TODO Replace the constant "drawAdjacentTiles" call with an "update" call that snapshots
            //   the tile's data. Perhaps a set of bools determining which sides of the tile need to be drawn?
            if (parent.sideGraphics.Count == 0) return;
            Graphic northGraphic = getSideGraphic(TileSide.North);
            Graphic southGraphic = getSideGraphic(TileSide.South);
            Graphic westGraphic = getSideGraphic(TileSide.West);
            Graphic eastGraphic = getSideGraphic(TileSide.East);
            Vector2 northPos;
            Vector2 southPos;
            Vector2 westPos;
            Vector2 eastPos;
            Tile northTile;
            Tile southTile;
            Tile westTile;
            Tile eastTile;


            if (northGraphic != null)
            {
                northPos = new Vector2(tilePos.X, tilePos.Y - 1); ;
                northTile = layer.getTile(northPos);
                if (northTile == null)
                {
                    northPos.X = northPos.X * TileManager.dimensions + (TileManager.dimensions / 2);
                    northPos.Y = northPos.Y * TileManager.dimensions + (TileManager.dimensions / 2);
                    northGraphic.draw(batch, northPos, false);
                    //batch.DrawRectangle(new Rectangle(Convert.ToInt32(northPos.X - 16), Convert.ToInt32(northPos.Y - 16), 32, 32), Color.White);
                }
            }

            if (southGraphic != null)
            {
                southPos = new Vector2(tilePos.X, tilePos.Y + 1);
                southTile = layer.getTile(southPos);
                if (southTile == null)
                {
                    southPos.X = southPos.X * TileManager.dimensions + (TileManager.dimensions / 2);
                    southPos.Y = southPos.Y * TileManager.dimensions + (TileManager.dimensions / 2);
                    southGraphic.draw(batch, southPos, false);
                    //batch.DrawRectangle(new Rectangle(Convert.ToInt32(southPos.X - 16), Convert.ToInt32(southPos.Y - 16), 32, 32), Color.White);
                }
            }

            if (westGraphic != null)
            {
                westPos = new Vector2(tilePos.X - 1, tilePos.Y);
                westTile = layer.getTile(westPos);
                if (westTile == null)
                {
                    westPos.X = westPos.X * TileManager.dimensions + (TileManager.dimensions / 2);
                    westPos.Y = westPos.Y * TileManager.dimensions + (TileManager.dimensions / 2);
                    westGraphic.draw(batch, westPos, false);
                    //batch.DrawRectangle(new Rectangle(Convert.ToInt32(westPos.X - 16), Convert.ToInt32(westPos.Y - 16), 32, 32), Color.White);
                }
            }

            if (eastGraphic != null)
            {
                eastPos = new Vector2(tilePos.X + 1, tilePos.Y);
                eastTile = layer.getTile(eastPos);
                if (eastTile == null)
                {
                    eastPos.X = eastPos.X * TileManager.dimensions + (TileManager.dimensions / 2);
                    eastPos.Y = eastPos.Y * TileManager.dimensions + (TileManager.dimensions / 2);
                    eastGraphic.draw(batch, eastPos, false);
                    //batch.DrawRectangle(new Rectangle(Convert.ToInt32(eastPos.X - 16), Convert.ToInt32(eastPos.Y - 16), 32, 32), Color.White);
                }
            }

        }

        public void update()
        {

            Graphic graphic;
            Vector2 checkPos;
            Tile checkTile;
            foreach (TileSide side in Enum.GetValues(typeof(TileSide)))
            {
                graphic = getSideGraphic(side);
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
                if (checkTile == null)
                    sides.Add(side, true);
                else
                    sides.Add(side, false);
            }

            return;

            //

            if (parent.sideGraphics.Count == 0) return;
            Graphic northGraphic = getSideGraphic(TileSide.North);
            Graphic southGraphic = getSideGraphic(TileSide.South);
            Graphic westGraphic = getSideGraphic(TileSide.West);
            Graphic eastGraphic = getSideGraphic(TileSide.East);
            Vector2 northPos;
            Vector2 southPos;
            Vector2 westPos;
            Vector2 eastPos;
            Tile northTile;
            Tile southTile;
            Tile westTile;
            Tile eastTile;


            if (northGraphic != null)
            {
                northPos = new Vector2(tilePos.X, tilePos.Y - 1); ;
                northTile = layer.getTile(northPos);
                if (northTile == null)
                {
                    sides.Remove(TileSide.North);
                    sides.Add(TileSide.North, true);
                }
                else
                {
                    sides.Remove(TileSide.North);
                    sides.Add(TileSide.North, false);
                }
            }

            if (southGraphic != null)
            {
                southPos = new Vector2(tilePos.X, tilePos.Y + 1);
                southTile = layer.getTile(southPos);
                if (southTile == null)
                {
                    sides.Remove(TileSide.South);
                    sides.Add(TileSide.South, true);
                } else
                {
                    sides.Remove(TileSide.South);
                    sides.Add(TileSide.South, false);
                }
            }

            if (westGraphic != null)
            {
                westPos = new Vector2(tilePos.X - 1, tilePos.Y);
                westTile = layer.getTile(westPos);
                if (westTile == null)
                {
                    sides.Remove(TileSide.West);
                    sides.Add(TileSide.West, true);
                }
                else
                {
                    sides.Remove(TileSide.West);
                    sides.Add(TileSide.West, false);
                }
            }

            if (eastGraphic != null)
            {
                eastPos = new Vector2(tilePos.X + 1, tilePos.Y);
                eastTile = layer.getTile(eastPos);
                if (eastTile == null)
                {
                    sides.Remove(TileSide.East);
                    sides.Add(TileSide.East, true);
                }
                else
                {
                    sides.Remove(TileSide.East);
                    sides.Add(TileSide.East, false);
                }
            }
        }

        public void draw(SpriteBatch batch)
        {
            if (!isInstance) return;
            parent.graphic.draw(batch, drawPos, false);
            drawAdjacentTiles(batch);
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
