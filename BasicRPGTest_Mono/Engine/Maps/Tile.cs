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

        // Instance variables
        public bool isInstance { get; set; }
        public Tile parent { get; set; }
        public Vector2 pos { get; set; } = new Vector2(0, 0);
        public Vector2 drawPos { get; set; } = new Vector2(0, 0);
        public Vector2 tilePos { get; set; } = new Vector2(0, 0);
        public Vector2 region { get; set; } = new Vector2(0, 0);
        public TileLayer layer { get; set; }
        public Dictionary<TileSide, bool> sides { get; set; }

        private Dictionary<Graphic, Vector2> edgeCache = new Dictionary<Graphic, Vector2>();
        private bool v_Visible = true;
        public bool seeThrough = false;

        
        public int tileSetGraphicIndex = 0;

        public Texture2D tileSetTexture;
        public Rectangle textureRect;



        //====================================================================================
        // CONSTRUCTORS
        //====================================================================================

        public Tile(string name, Texture2D texture, bool collidable = false, bool instance = true, int z = 1)
        {
            id = TileManager.tiles.Count;
            this.name = name;
            this.zIndex = z;
            isCollidable = collidable;
            isInstance = instance;
            sideGraphics = new Dictionary<TileSide, Graphic>();

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
        public Tile(Tile tile, Vector2 tilePos)
        {
            this.parent = tile;
            this.name = tile.name;
            this.id = tile.id;
            this.isCollidable = tile.isCollidable;
            this.isInstance = true;
            this.zIndex = tile.zIndex;
            this.tilePos = tilePos;
            this.pos = new Vector2(tilePos.X * dimensions, tilePos.Y * dimensions);
            this.drawPos = new Vector2(pos.X + (dimensions / 2), pos.Y + (dimensions / 2));
            sideGraphics = tile.sideGraphics;
            sides = new Dictionary<TileSide, bool>();

            box = new Rectangle(Convert.ToInt32(pos.X), Convert.ToInt32(pos.Y), dimensions, dimensions);

        }


        //====================================================================================
        // PROPERTIES
        //====================================================================================

        public bool hasEdges
        {
            get
            {
                // If NO edges exist
                if (sideGraphics.Count == 0) { return false; }
                // Otherwise...
                return true;
            }
        }



        //====================================================================================
        // FUNCTIONS
        //====================================================================================

        private Graphic getSideGraphic(TileSide side)
        {
            if (sideGraphics.ContainsKey(side))
                return sideGraphics[side];
            else
                return null;
        }


        public void drawAdjacentTiles2(SpriteBatch batch)
        {

            int drawnEdgesCount = 0;

            foreach (KeyValuePair<Graphic, Vector2> pair in edgeCache)
            {
                //if (pair.Key == null) { continue; }
                pair.Key.draw(batch, pair.Value, 0f, Vector2.Zero, 1, false, 0);

                //batch.DrawRectangle(new Rectangle((int)pair.Value.X, (int)pair.Value.Y, 32, 32), Color.Red);

                drawnEdgesCount++;
            }

            // Show count of this Tile's number of Drawn Edges
            //batch.DrawString(Core.mainFont, drawnEdgesCount.ToString(), pos, Microsoft.Xna.Framework.Color.Black);

        }


        public void drawAdjacentTiles(SpriteBatch batch)
        {

            //drawAdjacentTiles2(batch);
            //return;

            
            if (sideGraphics.Count == 0) return;
            // Otherwise...

            Vector2 drawPos;
            TileSide side;
            Graphic sideGraphic;
            int tileSize = TileManager.dimensions;
            int drawnEdgesCount = 0;


            foreach (KeyValuePair<TileSide, bool> pair in sides)
            {

                // If NO Value (skip to next)
                if (!pair.Value) { continue; }
                // Otherwise...

                side = pair.Key;
                // If Side Graphic does NOT Exist
                if (!sideGraphics.ContainsKey(side)) { continue; }
                // Otherwise... Get it
                sideGraphic = sideGraphics[side];

                drawPos = pos;

                switch (side)
                {
                    case TileSide.NorthWest:
                        drawPos.X -= tileSize;
                        drawPos.Y -= tileSize;
                        break;
                    case TileSide.North:
                        drawPos.Y -= tileSize;
                        break;
                    case TileSide.NorthEast:
                        drawPos.X += tileSize;
                        drawPos.Y -= tileSize;
                        break;
                    case TileSide.West:
                        drawPos.X -= tileSize;
                        break;
                    case TileSide.East:
                        drawPos.X += tileSize;
                        break;
                    case TileSide.SouthWest:
                        drawPos.X -= tileSize;
                        drawPos.Y += tileSize;
                        break;
                    case TileSide.South:
                        drawPos.Y += tileSize;
                        break;
                    case TileSide.SouthEast:
                        drawPos.X += tileSize;
                        drawPos.Y += tileSize;
                        break;
                }

                sideGraphic.draw(batch, drawPos, 0f, Vector2.Zero, 1, false, 0);
                //sideGraphics[side].draw(batch, drawPos, 0f, Vector2.Zero, 1, false, 0);
                //batch.DrawRectangle(new Rectangle(Convert.ToInt32(drawPos.X), Convert.ToInt32(drawPos.Y), 32, 32), Color.White);

                drawnEdgesCount++;
            }

            // Show count of this Tile's number of Drawn Edges
            //batch.DrawString(Core.mainFont, drawnEdgesCount.ToString(), pos, Microsoft.Xna.Framework.Color.Black);

            return;

        }


        public void Break()
        {
            layer.clearTile(tilePos);
            //Map map = MapManager.activeMap;
            Region region = map.regions[this.region];
            region.removeTile(this);
            map.buildVisibleTileCache();
        }


        public void update()
        {
            if (sideGraphics.Count == 0) return;
            Graphic graphic;
            Vector2 checkPos;
            Tile checkTile;

            edgeCache.Clear();

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

                    /*
                    if (graphic != null)
                    {
                        Vector2 cacheVector = pos;
                        cacheVector.X += (checkPos.X * TileManager.dimensions);
                        cacheVector.Y += (checkPos.Y * TileManager.dimensions);

                        edgeCache.Add(graphic, cacheVector);
                    }
                    */
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
            parent.graphic.draw(batch, pos, 0f, Vector2.Zero, 1, false, 0.1f);
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
