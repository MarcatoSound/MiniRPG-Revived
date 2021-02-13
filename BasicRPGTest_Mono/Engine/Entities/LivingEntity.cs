using BasicRPGTest_Mono.Engine.Datapacks;
using BasicRPGTest_Mono.Engine.GUI.Text;
using BasicRPGTest_Mono.Engine.Items;
using BasicRPGTest_Mono.Engine.Maps;
using BasicRPGTest_Mono.Engine.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RPGEngine;
using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;
using YamlDotNet.RepresentationModel;

namespace BasicRPGTest_Mono.Engine
{
    public class LivingEntity : Entity, IDisposable
    {
        public bool isMoving;

        public string displayName { get; set; }
        public float speed { get; set; }
        public Vector2 velocity { get; set; }
        public Vector2 maxVelocity { get; set; }

        private Timer tickTimer { get; set; }
        private int ticksSinceMove { get; set; }
        private int ticksToMove = 30;
        private int moveCount;
        public Direction direction = Direction.None;

        public Timer immunityTimer;
        public bool isImmunity;
        public int immunityTime = 200;

        public double maxHealth = 50;
        public double _health;
        public double health
        {
            get { return _health; }
            set
            {
                _health = value;
                if (_health < 0)
                {
                    _health = 0;
                    kill();
                }
                else if (_health > maxHealth)
                    _health = maxHealth;
            }
        }
        public Timer knockbackTimer { get; set; }
        public bool isGettingKnockedBack { get; set; }
        public float kbResist = 0f;
        public double damage { get; set; } = 5;

        public Map map;

        // Drop data
        public DropTable dropTable = new DropTable();

        public LivingEntity(string name, Texture2D texture, Rectangle box, float speed = 90f) : base(new Graphic(texture), box)
        {
            if (GetType() == typeof(LivingEntity)) id = EntityManager.livingEntities.Count;
            this.name = name;
            this.speed = speed;
            this.Position = Vector2.Zero;
            boundingBox = getBox(Position);

            //EntityManager.add(this);
        }
        public LivingEntity(string name, Graphic graphic, Rectangle box, float speed = 90f) : base(graphic, box)
        {
            if (GetType() == typeof(LivingEntity)) id = EntityManager.livingEntities.Count;
            this.name = name;
            this.speed = speed;
            this.Position = Vector2.Zero;

            //EntityManager.add(this);
        }
        public LivingEntity(DataPack pack, YamlSection config) : base(new Graphic(), new Rectangle())
        {
            if (GetType() == typeof(LivingEntity)) id = EntityManager.livingEntities.Count;
            this.name = config.getName();
            this.displayName = config.getString("display_name", name);
            this.maxHealth = config.getInt("max_health", 50);
            this.speed = config.getInt("movement_speed", 90);
            this.kbResist = (float)config.getDouble("knockback_resist");
            this.immunityTime = config.getInt("immunity_ticks", 200);
            this.boundingBox = new Rectangle(0, 0, config.getInt("hitbox.width", 32), config.getInt("hitbox.height", 32));

            // These take a little more processing to validate...

            // GRAPHIC
            string imgPath = config.getString("texture");
            Texture2D texture;
            if (!imgPath.Equals(""))
                texture = Util.loadTexture($"{pack.packPath}\\textures\\{imgPath}");
            else
                texture = Util.loadTexture($"{pack.packPath}\\textures\\missing.png");
            graphic = new Graphic(texture);

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

            // SPAWNING
            YamlNode spawnYaml = config.get("spawning");
            if (spawnYaml != null && spawnYaml.NodeType == YamlNodeType.Sequence)
            {
                YamlSequenceNode sequence = (YamlSequenceNode)spawnYaml;

                foreach (var entry in sequence)
                {
                    if (entry.NodeType != YamlNodeType.Mapping) continue;
                    YamlMappingNode mapNode = (YamlMappingNode)entry;

                    YamlSection spawnConfig = new YamlSection(mapNode);

                    string worldName = spawnConfig.getString("map");
                    Console.WriteLine($"// ││├┬ Processing spawn entry on map '{worldName}'...");
                    Map map = MapManager.getByName(worldName);
                    if (map == null)
                    {
                        Console.WriteLine($"// │││└╾ ERR: Unable to find map '{worldName}' when configuring spawns! Skipping...");
                        continue;
                    }
                    map.spawns.TryAdd(map.spawns.Count, new Entities.Spawn(this, spawnConfig.getDouble("weight", 1)));
                    Console.WriteLine($"// │││└╾ SUCCESS!");

                }
            }
            //Map map = MapManager.getByName(mapName);
        }
        public LivingEntity(LivingEntity entity, Vector2 pos, int instanceId, Map map) : base(entity.graphic, new Rectangle((int)pos.X, (int)pos.Y, entity.boundingBox.Width, entity.boundingBox.Height))
        {
            this.speed = entity.speed;
            this.Position = pos;
            this.instanceId = instanceId;

            tickTimer = new Timer(50);
            tickTimer.Elapsed += tryMove;
            tickTimer.Start();


            moveCount = 0;

            maxVelocity = new Vector2(speed, speed);
            maxHealth = entity.maxHealth;
            health = maxHealth;

            this.dropTable = entity.dropTable;

            this.map = map;
        }

        public override void update()
        {
            
            Vector2 newVel = velocity;

            if (!isGettingKnockedBack)
            {

                if (direction == Direction.Up)
                {
                    newVel = new Vector2(velocity.X, velocity.Y - 10f);
                    if (newVel.Y < -maxVelocity.Y) newVel.Y = -maxVelocity.Y;
                    velocity = newVel;
                }
                else
                {
                    if (newVel.Y < 0)
                    {
                        newVel = new Vector2(velocity.X, velocity.Y + 20f);
                        if (newVel.Y > 0)
                            newVel = new Vector2(velocity.X, 0);
                    }
                    velocity = newVel;
                }

                if (direction == Direction.Down)
                {
                    newVel = new Vector2(velocity.X, velocity.Y + 10f);
                    if (newVel.Y > maxVelocity.Y) newVel.Y = maxVelocity.Y;
                    velocity = newVel;
                }
                else
                {
                    if (newVel.Y > 0)
                    {
                        newVel = new Vector2(velocity.X, velocity.Y - 20f);
                        if (newVel.Y < 0)
                            newVel = new Vector2(velocity.X, 0);
                    }
                    velocity = newVel;
                }

                if (direction == Direction.Left)
                {
                    newVel = new Vector2(velocity.X - 10f, velocity.Y);
                    if (newVel.X < -maxVelocity.X) newVel.X = -maxVelocity.X;
                    velocity = newVel;
                }
                else
                {
                    if (newVel.X < 0)
                    {
                        newVel = new Vector2(velocity.X + 20f, velocity.Y);
                        if (newVel.X > 0)
                            newVel = new Vector2(0, velocity.X);
                    }
                    velocity = newVel;
                }

                if (direction == Direction.Right)
                {
                    newVel = new Vector2(velocity.X + 10f, velocity.Y);
                    if (newVel.X > maxVelocity.X) newVel.X = maxVelocity.X;
                    velocity = newVel;
                }
                else
                {
                    if (newVel.X > 0)
                    {
                        newVel = new Vector2(velocity.X - 20f, velocity.Y);
                        if (newVel.X < 0)
                            newVel = new Vector2(0, velocity.X);
                    }
                    velocity = newVel;
                }

            }


            move();
            base.update();
        }
        public void tryMove(Object source, ElapsedEventArgs args)
        {
            ticksSinceMove++;
            Random rand = new Random();
            if (ticksSinceMove > ticksToMove * (moveCount+1))
            {

                if (rand.Next(0, 100) < 50)
                {
                    moveCount = 0;
                    ticksSinceMove = 0;
                    direction = Direction.None;
                    return;
                }
                moveCount++;

                randomizeDirection();

                return;
            }
            if (moveCount > 0) return;

            if (ticksSinceMove < ticksToMove) return;
            if (rand.Next(0, 100) < 40) return;

            randomizeDirection();

            moveCount++;
        }
        public void randomizeDirection()
        {
            Random rand = new Random();
            int selection = rand.Next(0, 4);

            switch (selection)
            {
                case 0:
                    direction = Direction.Up;
                    break;
                case 1:
                    direction = Direction.Down;
                    break;
                case 2:
                    direction = Direction.Left;
                    break;
                case 3:
                    direction = Direction.Right;
                    break;
            }
        }
        public virtual void move()
        {
            Vector2 newPos = Position;
            Rectangle newBox = boundingBox;

            // Right
            if (velocity.X > 0)
            {
                newPos.X += (float)(velocity.X / 1.5) * (float)Core.globalTime.ElapsedGameTime.TotalSeconds;
                if (newPos.X < 0 + (graphic.height / 2))
                    newPos.X = 0 + (graphic.height / 2);


                newBox = getBox(newPos);

                if (isColliding(newBox))
                {
                    newPos.X = Position.X;
                    newBox.X = boundingBox.X;
                }
            }
            // Left
            else if (velocity.X < 0)
            {
                newPos.X += (float)(velocity.X / 1.5) * (float)Core.globalTime.ElapsedGameTime.TotalSeconds;

                if (newPos.Y > (MapManager.activeMap.heightInPixels - (graphic.width / 2)))
                    newPos.Y = MapManager.activeMap.heightInPixels - (graphic.width / 2);


                newBox = getBox(newPos);

                if (isColliding(newBox))
                {
                    newPos.X = Position.X;
                    newBox.X = boundingBox.X;
                }
            }
            // Down
            if (velocity.Y > 0)
            {
                newPos.Y += (float)(velocity.Y / 1.5) * (float)Core.globalTime.ElapsedGameTime.TotalSeconds;
                if (newPos.Y < 0 + (graphic.height / 2))
                    newPos.Y = 0 + (graphic.height / 2);


                newBox = getBox(newPos);

                if (isColliding(newBox))
                {
                    newPos.Y = Position.Y;
                    newBox.Y = boundingBox.Y;
                }
            }
            // Up
            else if (velocity.Y < 0)
            {
                newPos.Y += (float)(velocity.Y / 1.5) * (float)Core.globalTime.ElapsedGameTime.TotalSeconds;

                if (newPos.Y > (MapManager.activeMap.heightInPixels - (graphic.width / 2)))
                    newPos.Y = MapManager.activeMap.heightInPixels - (graphic.width / 2);


                newBox = getBox(newPos);

                if (isColliding(newBox))
                {
                    newPos.Y = Position.Y;
                    newBox.Y = boundingBox.Y;
                }
            }

            Position = new Vector2(newPos.X, newPos.Y);
            boundingBox = new Rectangle(newBox.X, newBox.Y, newBox.Width, newBox.Height);

        }


        public bool isColliding(Rectangle box)
        {
            if (MapManager.activeMap == null) return true;
            List<Tile> tiles = Util.getSurroundingTiles(map, 1, TilePosition);
            foreach (Tile tile in tiles)
            {
                if (tile == null) continue;
                if (!tile.isCollidable) continue;
                //Console.WriteLine($"Tile box: {tile.box}");
                if (box.Intersects(tile.box))
                    return true;
            }


            return false;
        }


        public virtual void hurt(double dmg, Vector2 sourcePos)
        {
            if (isImmunity) return;
            isImmunity = true;
            tintColor = Color.LightCoral;

            immunityTimer = new Timer(immunityTime);
            immunityTimer.Elapsed += (sender, args) =>
            {
                isImmunity = false;
                tintColor = Color.White;
                immunityTimer.Stop();
                immunityTimer.Dispose();
                immunityTimer = null;
            };
            immunityTimer.Start();

            knockback(sourcePos);
            showDamageText(dmg);

            health -= dmg;
        }
        public void showDamageText(double dmg)
        {
            // TODO: Implement check for critical hit.
            SpriteFont font = FontLibrary.getFont("dmg");
            Vector2 stringPos = new Vector2(Position.X, Position.Y);
            Vector2 stringSize = font.MeasureString(dmg.ToString());
            stringPos.X -= stringSize.X / 2;
            stringPos.Y -= 20;

            Random rand = new Random();
            stringPos.X += rand.Next(-5, 5);

            new MovingText(dmg.ToString(), font, stringPos, new TextColor(Color.Crimson), 500);
        }

        public virtual void knockback(Vector2 sourcePos)
        {
            if (isGettingKnockedBack) return;
            isGettingKnockedBack = true;

            Vector2 screenPos = Position;
            screenPos.X = screenPos.X + (graphic.texture.Width / 2);
            screenPos.Y = screenPos.Y + (graphic.texture.Height / 2);
            Vector2 targetDist = new Vector2();
            targetDist.X = screenPos.X - sourcePos.X;
            targetDist.Y = screenPos.Y - sourcePos.Y;

            double z = Math.Sqrt((Math.Pow(targetDist.X, 2)) + (Math.Pow(targetDist.Y, 2)));
            float maxKbVel = 600f;
            int knockbackStr = (int)targetDist.X + (int)targetDist.Y;
            Vector2 kbRatio = new Vector2((float)(targetDist.X / z), (float)(targetDist.Y / z));
            velocity = new Vector2(maxKbVel / 2 * kbRatio.X, maxKbVel / 2 * kbRatio.Y);


            int maxKbTime = 200;
            double zOut = Math.Pow(z, -1) * 10 + 0.3;
            int kbTime = Convert.ToInt32(Math.Min(Math.Max(1000 * zOut - 400, 25), maxKbTime));
            kbTime = Convert.ToInt32(kbTime - (kbTime * kbResist));
            knockbackTimer = new Timer(kbTime);
            knockbackTimer.Elapsed += (sender, args) =>
            {
                isGettingKnockedBack = false;
                knockbackTimer.Stop();
                knockbackTimer.Dispose();
                knockbackTimer = null;
            };
            knockbackTimer.Start();

        }

        public void kill()
        {
            Vector2 dropPos = new Vector2(Position.X + (TileManager.dimensions / 3), Position.Y + (TileManager.dimensions / 3));
            dropTable.dropItems(map, dropPos);
            /*
            Vector2 dropPos;
            foreach (ItemDrop drop in drops)
            {
                dropPos = Util.randomizePosition(Position, 6);

                drop.tryDrop(map, dropPos);
            }*/

            MapManager.activeMap.livingEntities.TryRemove(instanceId, out _);
            Dispose();
        }

        public void Dispose()
        {

        }


        public static implicit operator YamlSection(LivingEntity ent)
        {
            YamlSection config = new YamlSection($"{ent.name}");

            config.setString("display_name", ent.displayName);
            config.setDouble("position.x", ent.Position.X);
            config.setDouble("position.y", ent.Position.Y);
            config.setDouble("stats.health", ent.health);

            return config;
        }
        /*public static implicit operator LivingEntity(YamlSection config)
        {
            return new Player(config);
        }*/

    }
}
