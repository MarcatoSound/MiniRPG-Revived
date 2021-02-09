using BasicRPGTest_Mono.Engine;
using BasicRPGTest_Mono.Engine.Datapacks;
using BasicRPGTest_Mono.Engine.GUI;
using BasicRPGTest_Mono.Engine.GUI.Text;
using BasicRPGTest_Mono.Engine.Items;
using BasicRPGTest_Mono.Engine.Maps;
using BasicRPGTest_Mono.Engine.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;
using YamlDotNet.RepresentationModel;

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
        public Dictionary<TileSide, Graphic> sideGraphics { get; set; } = new Dictionary<TileSide, Graphic>();
        public Rectangle box { get; set; }
        public bool isCollidable { get; set; }
        public int zIndex { get; set; }

        public double maxHealth { get; set; }
        public bool indestructable { get; set; }
        public DropTable dropTable = new DropTable();

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
                else if (value > maxHealth)
                {
                    _health = maxHealth;
                }
                else
                    _health = value;
                healthPercent = _health / maxHealth;
            }
        }
        public bool isBeingDamaged;
        public double healthPercent;
        private int breakTexture;
        private Timer restoreTimer;


        public Tile(string name, Texture2D texture, bool collidable = false, bool instance = true, int z = 1, double maxHP = 20, bool indestructable = false)
        {
            id = TileManager.getTiles().Count;
            this.name = name;
            this.zIndex = z;
            isCollidable = collidable;
            isInstance = instance;
            this.maxHealth = maxHP;
            this.indestructable = indestructable;

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
        public Tile(DataPack pack, YamlSection config)
        {
            id = TileManager.getTiles().Count;
            this.name = config.getName();
            this.zIndex = config.getInt("zindex", 1);
            this.isCollidable = config.getBool("collidable", false);
            this.maxHealth = config.getInt("max_health", 10);
            this.indestructable = config.getBool("indestructable", false);

            // These take a little more processing to validate...

            // GRAPHIC
            string imgPath = config.getString("texture");
            Texture2D texture;
            if (!imgPath.Equals(""))
                texture = Util.loadTexture($"{pack.packPath}\\textures\\{imgPath}");
            else
                texture = Util.loadTexture($"{pack.packPath}\\textures\\missing.png");

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

            // DROPTABLE
            YamlNode tableInfo = config.get("droptable");
            if (tableInfo != null)
            {
                if (tableInfo.NodeType == YamlNodeType.Scalar)
                    this.dropTable = DropTableManager.getByNamespace((string)tableInfo);
                else
                {
                    // TODO: Code for converting this sub-section into a datapack.
                    YamlSection tableConfig = new YamlSection((YamlMappingNode)tableInfo);
                    dropTable = new DropTable(pack, tableConfig);

                }
            }
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
            dropTable = tile.dropTable;

            box = new Rectangle(Convert.ToInt32(pos.X), Convert.ToInt32(pos.Y), dimensions, dimensions);

            maxHealth = tile.maxHealth;
            health = maxHealth;
            this.indestructable = tile.indestructable;
            restoreTimer = new Timer(10000);
            restoreTimer.Elapsed += (sender, args) =>
            {
                Heal(maxHealth);
                restoreTimer.Stop();
            };

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
            int drawnEdgesCount = 0;
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
                        drawPos.X -= TileManager.dimensions;
                        drawPos.Y -= TileManager.dimensions;
                        break;
                    case TileSide.North:
                        drawPos.Y -= TileManager.dimensions;
                        break;
                    case TileSide.NorthEast:
                        drawPos.X += TileManager.dimensions;
                        drawPos.Y -= TileManager.dimensions;
                        break;
                    case TileSide.West:
                        drawPos.X -= TileManager.dimensions;
                        break;
                    case TileSide.East:
                        drawPos.X += TileManager.dimensions;
                        break;
                    case TileSide.SouthWest:
                        drawPos.X -= TileManager.dimensions;
                        drawPos.Y += TileManager.dimensions;
                        break;
                    case TileSide.South:
                        drawPos.Y += TileManager.dimensions;
                        break;
                    case TileSide.SouthEast:
                        drawPos.X += TileManager.dimensions;
                        drawPos.Y += TileManager.dimensions;
                        break;
                }

                sideGraphics[side].draw(batch, drawPos, 0f, Vector2.Zero, 1, 0);
                //batch.DrawRectangle(new Rectangle(Convert.ToInt32(drawPos.X), Convert.ToInt32(drawPos.Y), 32, 32), Color.White);

                drawnEdgesCount++;
            }

            return;

        }
        public void drawAdjacentTiles2(SpriteBatch batch)
        {

            int drawnEdgesCount = 0;

            foreach (KeyValuePair<Graphic, Vector2> pair in edgeCache)
            {
                //if (pair.Key == null) { continue; }
                pair.Key.draw(batch, pair.Value, 0f, Vector2.Zero, 1, 0);

                //batch.DrawRectangle(new Rectangle((int)pair.Value.X, (int)pair.Value.Y, 32, 32), Color.Red);

                drawnEdgesCount++;
            }

            // Show count of this Tile's number of Drawn Edges
            //batch.DrawString(Core.mainFont, drawnEdgesCount.ToString(), pos, Microsoft.Xna.Framework.Color.Black);

        }

        public void update()
        {
            if (sideGraphics.Count == 0) return;
            Graphic graphic;
            Vector2 checkPos;
            Tile checkTile;
            bool isCorner;
            foreach (TileSide side in Enum.GetValues(typeof(TileSide)))
            {
                isCorner = false;
                graphic = getSideGraphic(side);
                if (graphic == null) continue;
                checkPos = tilePos;
                switch (side)
                {
                    case TileSide.NorthWest:
                        checkPos.X -= 1;
                        checkPos.Y -= 1;
                        isCorner = true;
                        break;
                    case TileSide.North:
                        checkPos.Y -= 1;
                        break;
                    case TileSide.NorthEast:
                        checkPos.X += 1;
                        checkPos.Y -= 1;
                        isCorner = true;
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
                        isCorner = true;
                        break;
                    case TileSide.South:
                        checkPos.Y += 1;
                        break;
                    case TileSide.SouthEast:
                        checkPos.X += 1;
                        checkPos.Y += 1;
                        isCorner = true;
                        break;
                }
                checkTile = layer.getTile(checkPos);
                if (checkTile == null || checkTile.zIndex < zIndex)
                {
                    if (isCorner)
                    {
                        Tile north = layer.getTile(checkPos.X, checkPos.Y - 1);
                        Tile south = layer.getTile(checkPos.X, checkPos.Y + 1);
                        Tile east = layer.getTile(checkPos.X - 1, checkPos.Y);
                        Tile west = layer.getTile(checkPos.X + 1, checkPos.Y);
                        if ((north == null || north.zIndex < zIndex) && (south == null || south.zIndex < zIndex) && (east == null || east.zIndex < zIndex) && (west == null || west.zIndex < zIndex))
                        {
                            sides.Remove(side);
                            sides.Add(side, true);
                        }

                    } 
                    else
                    {
                        sides.Remove(side);
                        sides.Add(side, true);
                    }
                }
                else
                {
                    if (isCorner)
                    {

                    }
                    else
                    {
                        sides.Remove(side);
                        sides.Add(side, false);
                    }
                }
            }

            return;
        }

        public void draw(SpriteBatch batch)
        {
            if (!isInstance) return;

            if (healthPercent < 1)
                drawBreakTexture(batch);

            if (isBeingDamaged)
                batch.DrawRectangle(box, Color.Red);
        }
        public void drawBreakTexture(SpriteBatch batch)
        {
            Texture2D spriteSet = TileManager.breakTexture;
            if (spriteSet == null) return;

            Rectangle targetRect = new Rectangle(TileManager.dimensions * breakTexture, 0, TileManager.dimensions, TileManager.dimensions);

            batch.Draw(spriteSet, pos, targetRect, Color.White);
        }


        public void Damage(double dmg)
        {
            restoreTimer.Stop();
            restoreTimer.Start();
            health -= dmg;

            if (healthPercent < 1 && healthPercent >= 0.75)
                breakTexture = 1;
            else if (healthPercent < 0.75 && healthPercent >= 0.5)
                breakTexture = 2;
            else if (healthPercent < 0.5 && healthPercent >= 0.25)
                breakTexture = 3;
            else if (healthPercent < 0.25)
                breakTexture = 4;

            isBeingDamaged = true;
            Timer timer = new Timer(500);
            timer.Elapsed += (sender, args) =>
            {
                isBeingDamaged = false;
                timer.Stop();
            };
            timer.Start();

            showDamageText(dmg);
        }
        public void Heal(double gain)
        {
            health += gain;
        }
        public void Destroy()
        {
            map.removeTile(this);

            // Attempt to spawn the items that drop from this tile.
            Vector2 dropPos = new Vector2(pos.X + (TileManager.dimensions / 3), pos.Y + (TileManager.dimensions / 3));
            dropTable.dropItems(map, dropPos);
            /*foreach (ItemDrop drop in drops)
            {
                dropPos = Util.randomizePosition(dropPos, 8);

                drop.tryDrop(map, dropPos);
            }*/

            if (this.name == "grass")
            {
                Tile replacement = new Tile(TileManager.getByName("dirt"), tilePos, biome);
                replacement.layer = layer;
                map.addTile(replacement);
            }

            List<Tile> surroundings = Util.getSurroundingTiles(map, 1, tilePos);
            foreach (Tile tile in surroundings)
            {
                if (tile == null) continue;
                if (tile.sideGraphics.Count == 0) continue;
                tile.update();
            }

            // Rebuilds updated Visible Tile Cache
            map.buildVisibleTileCache();

        }

        public void showDamageText(double dmg)
        {
            // TODO: Implement check for critical hit.
            SpriteFont font = FontLibrary.getFont("dmg");
            Vector2 stringPos = new Vector2(drawPos.X, drawPos.Y);
            Vector2 stringSize = font.MeasureString(dmg.ToString());
            stringPos.X -= stringSize.X / 2;
            stringPos.Y -= 20;

            Random rand = new Random();
            stringPos.X += rand.Next(-5, 5);

            new MovingText(dmg.ToString(), font, stringPos, new TextColor(Color.Crimson), 500);
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
