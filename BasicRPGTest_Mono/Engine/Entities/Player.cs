using BasicRPGTest_Mono.Engine.Entities;
using BasicRPGTest_Mono.Engine.Graphics;
using BasicRPGTest_Mono.Engine.GUI;
using BasicRPGTest_Mono.Engine.Inventories;
using BasicRPGTest_Mono.Engine.Items;
using BasicRPGTest_Mono.Engine.Maps;
using BasicRPGTest_Mono.Engine.Menus;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Tiled;
using RPGEngine;
using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;

namespace BasicRPGTest_Mono.Engine
{
    public class Player : LivingEntity, IFocusable
    {

        public bool isDashing;
        public Timer dashTimer;

        public ItemSwing itemSwing;
        public bool isAttacking;
        public Timer attackTimer;

        public PlayerInventory inventory;
        public Player(Texture2D texture) : 
            base("player", new GraphicSet(texture), new Rectangle(0, 0, 24, 32), 125f)
        {
            Core.player = this;
            Camera.camera.Focus = this;

            Random rand = new Random();
            Vector2 spawnPoint = new Vector2(rand.Next(32, MapManager.activeMap.widthInPixels / 2 - 32), rand.Next(32, MapManager.activeMap.heightInPixels / 2 - 32));
            Rectangle spawnBox = new Rectangle(Convert.ToInt32(spawnPoint.X), Convert.ToInt32(spawnPoint.Y), 24, 32);
            if (!MapManager.activeMap.isLocationSafe(spawnBox))
            {
                System.Diagnostics.Debug.WriteLine($"Spawn point {spawnPoint} unsafe! Finding another one...");
                spawnPoint.X = rand.Next(32, MapManager.activeMap.widthInPixels / 2 - 32);
                spawnPoint.Y = rand.Next(32, MapManager.activeMap.heightInPixels / 2 - 32);
            }
            Position = spawnPoint;
            map = MapManager.activeMap;

            boundingBox = new Rectangle((int)Position.X, (int)Position.Y, 24, 32);
            Camera.camPos = new Vector2(Position.X - (Camera.camera.BoundingRectangle.Width / 2), Position.Y - (Camera.camera.BoundingRectangle.Height / 2));
            maxVelocity = new Vector2(speed, speed);
            dashTimer = new Timer(200);
            dashTimer.Elapsed += (sender, args) =>
            {
                isDashing = false;
                dashTimer.Stop();
                maxVelocity = new Vector2(speed, speed);
            };

            kbResist = 0.75f;

            inventory = new PlayerInventory();
            inventory.addItem(new Item(ItemManager.getByNamespace("hollysong")));
            inventory.addItem(new Item(ItemManager.getByNamespace("arcticfoxtail")));
            inventory.addItem(new Item(ItemManager.getByNamespace("ironroot")));
            inventory.addItem(new Item(ItemManager.getByNamespace("sunfeather")));
            inventory.addItem(new Item(ItemManager.getByNamespace("cryorose")));
            inventory.addItem(new Item(ItemManager.getByNamespace("unicornhorn")));
            inventory.addItem(new Item(ItemManager.getByNamespace("sunfeather")));
            inventory.addItem(new Item(ItemManager.getByNamespace("arcticfoxtail")));
            inventory.addItem(new Item(ItemManager.getByNamespace("ironroot")));
            inventory.addItem(new Item(ItemManager.getByNamespace("unicornhorn")));
            inventory.addItem(new Item(ItemManager.getByNamespace("cryorose")));
            inventory.addItem(new Item(ItemManager.getByNamespace("arcticfoxtail")));
            inventory.addItem(new Item(ItemManager.getByNamespace("ironroot")));
            inventory.addItem(new Item(ItemManager.getByNamespace("sunfeather")));
            inventory.addItem(new Item(ItemManager.getByNamespace("cryorose")));
            inventory.addItem(new Item(ItemManager.getByNamespace("unicornhorn")));
            inventory.addItem(new Item(ItemManager.getByNamespace("sunfeather")));
            inventory.addItem(new Item(ItemManager.getByNamespace("arcticfoxtail")));
            inventory.setItem(23, new Item(ItemManager.getByNamespace("ironroot")));
            inventory.setItem(35, new Item(ItemManager.getByNamespace("unicornhorn")));
            inventory.setItem(30, new Item(ItemManager.getByNamespace("cryorose")));
            inventory.setItem(52, new Item(ItemManager.getByNamespace("hollysong")));
            inventory.addItem(new Item(ItemManager.getByNamespace("hollysong")));
            inventory.addItem(new Item(ItemManager.getByNamespace("crystalsword")));
            inventory.hotbarPrimary.setItem(0, new Item(ItemManager.getByNamespace("hollysong")));
            inventory.hotbarPrimary.setItem(1, new Item(ItemManager.getByNamespace("ironroot")));
            inventory.hotbarPrimary.setItem(2, new Item(ItemManager.getByNamespace("cryorose")));
            inventory.hotbarSecondary.setItem(0, new Item(ItemManager.getByNamespace("arcticfoxtail")));
            inventory.hotbarSecondary.setItem(1, new Item(ItemManager.getByNamespace("sunfeather")));

            GuiWindowManager.playerInv.updateGui();
        }
        public void toggleInv()
        {
            if (GuiWindowManager.activeWindow == null)
            {
                GuiPlayerInventory invGui = (GuiPlayerInventory)GuiWindowManager.getByName("playerinventory");
                invGui.updateGui();
                GuiWindowManager.openWindow(invGui);
            }
            else GuiWindowManager.closeWindow();
        }
        public void swapHotbars()
        {
            // TODO: A bit of extra overhead... maybe find a way to avoid the temporary variables?
            Hotbar oldPrimary = inventory.hotbarPrimary;
            Hotbar oldSecondary = inventory.hotbarSecondary;

            inventory.hotbarPrimary = oldSecondary;
            inventory.hotbarSecondary = oldPrimary;
        }

        public void attack(Direction direction)
        {
            if (isAttacking) return;
            if (inventory.hotbarPrimary.hand == null) return;

            ((GraphicSet)graphic).setSprite(GraphicType.Attack, direction);

            Item mainhand = inventory.hotbarPrimary.hand;

            isAttacking = true;
            itemSwing = new ItemSwing(direction, 200, this, mainhand, Position, mainhand.swingStyle, mainhand.swingDist);
            attackTimer = new Timer(200);
            attackTimer.Elapsed += (sender, args) =>
            {
                isAttacking = false;
                attackTimer.Stop();
                attackTimer = null;
            };
            attackTimer.Start();

            // Tool handling
            if (mainhand.GetType() == typeof(Tool))
            {
                Tool tool = (Tool)mainhand;

                Vector2 targetPos = Utility.Util.getTilePosition(Position);
                Tile tile = null;
                switch (direction)
                {
                    case Direction.Up:
                        targetPos.Y = Utility.Util.trueCoordToTileCoord(itemSwing.hitBox.Top);
                        tile = map.getTopTile(targetPos);
                        break;
                    case Direction.Down:
                        targetPos.Y = Utility.Util.trueCoordToTileCoord(itemSwing.hitBox.Bottom);
                        tile = map.getTopTile(targetPos);
                        break;
                    case Direction.Left:
                        targetPos.X = Utility.Util.trueCoordToTileCoord(itemSwing.hitBox.Left);
                        tile = map.getTopTile(targetPos);
                        break;
                    case Direction.Right:
                        targetPos.X = Utility.Util.trueCoordToTileCoord(itemSwing.hitBox.Right);
                        tile = map.getTopTile(targetPos);
                        break;
                }
                if (tile == null) return;
                if (tool.damageTypes.ContainsKey(DamageType.Mining))
                {
                    double damage;
                    if (tile.destructable)
                        damage = tool.damageTypes[DamageType.Mining];
                    else
                        damage = 0;
                    tile.Damage(damage);
                    System.Diagnostics.Debug.WriteLine($"Dealt {damage} damage to tile at {tile.tilePos}");
                }
            }

        }
        public void Dash()
        {
            if (!isDashing)
            {
                isDashing = true;
                maxVelocity = new Vector2(Convert.ToInt32(speed * 4), Convert.ToInt32(speed * 4));
                dashTimer.Start();
            }
        }
        public void setDirection(Direction direction)
        {
            this.direction = direction;
        }
        public override void move()
        {
            Vector2 newPlayerPos = Position;

            if (velocity.X > 0)
            {

                newPlayerPos.X += velocity.X * (float)Core.globalTime.ElapsedGameTime.TotalSeconds;

                if (newPlayerPos.X > (MapManager.activeMap.widthInPixels - (graphic.width / 2)))
                    newPlayerPos.X = MapManager.activeMap.widthInPixels - (graphic.width / 2);


                if (isColliding(getBox(newPlayerPos)))
                {
                    newPlayerPos.X = Position.X;
                }

            }
            else if (velocity.X < 0)
            {

                newPlayerPos.X += velocity.X * (float)Core.globalTime.ElapsedGameTime.TotalSeconds;

                if (newPlayerPos.X < 0 + (graphic.width / 2))
                    newPlayerPos.X = 0 + (graphic.width / 2);



                if (isColliding(getBox(newPlayerPos)))
                {
                    newPlayerPos.X = Position.X;
                }

            }

            if (velocity.Y > 0)
            {

                newPlayerPos.Y += velocity.Y * (float)Core.globalTime.ElapsedGameTime.TotalSeconds;

                if (newPlayerPos.Y > (MapManager.activeMap.heightInPixels - (graphic.width / 2)))
                    newPlayerPos.Y = MapManager.activeMap.heightInPixels - (graphic.width / 2);


                if (isColliding(getBox(newPlayerPos)))
                {
                    newPlayerPos.Y = Position.Y;
                }

            }
            else if (velocity.Y < 0)
            {

                newPlayerPos.Y += velocity.Y * (float)Core.globalTime.ElapsedGameTime.TotalSeconds;

                if (newPlayerPos.Y < 0 + (graphic.height / 2))
                    newPlayerPos.Y = 0 + (graphic.height / 2);



                if (isColliding(getBox(newPlayerPos)))
                {
                    newPlayerPos.Y = Position.Y;
                }

            }

            Position = new Vector2(newPlayerPos.X, newPlayerPos.Y);

        }



        public Vector2 getPlayerTilePosition()
        {
            Vector2 pos = new Vector2(Position.X, Position.Y);

            pos.X = Convert.ToInt32(pos.X / TileManager.dimensions);
            pos.Y = Convert.ToInt32(pos.Y / TileManager.dimensions);

            return pos;
        }
        public Vector2 getPlayerTilePositionPrecise()
        {
            Vector2 pos = new Vector2(Position.X, Position.Y);

            pos.X = pos.X / TileManager.dimensions;
            pos.Y = pos.Y / TileManager.dimensions;

            return pos;
        }
        public override Rectangle getBox(Vector2 pos)
        {
            int x = (int)(pos.X - (boundingBox.Width - (boundingBox.Width / 2)));
            int y = (int)(pos.Y - (boundingBox.Height - (graphic.texture.Height / 2.5)));

            Rectangle box = new Rectangle(x, y, boundingBox.Width, boundingBox.Height);

            return box;
        }

        public override void update()
        {

            boundingBox = getBox(Position);
            List<LivingEntity> entities = new List<LivingEntity>(MapManager.activeMap.livingEntities.Values);
            foreach (LivingEntity entity in entities)
            {
                if (itemSwing != null && itemSwing.hitBox.Intersects(entity.boundingBox) && !entity.isImmunity)
                    entity.hurt(inventory.mainhand.damage, Position);

                if (boundingBox.Intersects(entity.boundingBox) && !entity.isImmunity)
                    hurt(entity.damage, entity.CenteredPosition);
            }

            var kstate = Keyboard.GetState();

            if (kstate.IsKeyDown(Keys.W) || kstate.IsKeyDown(Keys.S) || kstate.IsKeyDown(Keys.A) || kstate.IsKeyDown(Keys.D))
            {
                isMoving = true;
            } else
            {
                isMoving = false;
                if (!isAttacking) ((GraphicSet)graphic).setSprite(GraphicType.Idle, direction);
            }

            Vector2 newVel = velocity;

            if (!isGettingKnockedBack)
            {
                if (kstate.IsKeyDown(Keys.W))
                {
                    if (!kstate.IsKeyDown(Keys.S))
                        if (!isAttacking) ((GraphicSet)graphic).setSprite(GraphicType.Move, Direction.Up);

                    newVel = new Vector2(velocity.X, velocity.Y - (float)(maxVelocity.Y / 5));
                    if (newVel.Y < -maxVelocity.Y) newVel.Y = -maxVelocity.Y;
                    velocity = newVel;
                }
                else
                {
                    if (newVel.Y < 0)
                    {
                        newVel = new Vector2(velocity.X, velocity.Y + (float)(maxVelocity.Y / 5));
                        if (newVel.Y > 0)
                            newVel = new Vector2(velocity.X, 0);
                    }
                    velocity = newVel;
                }

                if (kstate.IsKeyDown(Keys.S))
                {
                    if (!kstate.IsKeyDown(Keys.W))
                        if (!isAttacking) ((GraphicSet)graphic).setSprite(GraphicType.Move, Direction.Down);

                    newVel = new Vector2(velocity.X, velocity.Y + (float)(maxVelocity.Y / 5));
                    if (newVel.Y > maxVelocity.Y) newVel.Y = maxVelocity.Y;
                    velocity = newVel;
                }
                else
                {
                    if (newVel.Y > 0)
                    {
                        newVel = new Vector2(velocity.X, velocity.Y - (float)(maxVelocity.Y / 5));
                        if (newVel.Y < 0)
                            newVel = new Vector2(velocity.X, 0);
                    }
                    velocity = newVel;
                }

                if (kstate.IsKeyDown(Keys.A))
                {
                    if (!kstate.IsKeyDown(Keys.D))
                        if (!isAttacking) ((GraphicSet)graphic).setSprite(GraphicType.Move, Direction.Left);

                    newVel = new Vector2(velocity.X - (float)(maxVelocity.X / 5), velocity.Y);
                    if (newVel.X < -maxVelocity.X) newVel.X = -maxVelocity.X;
                    velocity = newVel;
                }
                else
                {
                    if (newVel.X < 0)
                    {
                        newVel = new Vector2(velocity.X + (float)(maxVelocity.X / 5), velocity.Y);
                        if (newVel.X > 0)
                            newVel = new Vector2(0, velocity.X);
                    }
                    velocity = newVel;
                }

                if (kstate.IsKeyDown(Keys.D))
                {
                    if (!kstate.IsKeyDown(Keys.A))
                        if (!isAttacking) ((GraphicSet)graphic).setSprite(GraphicType.Move, Direction.Right);

                    newVel = new Vector2(velocity.X + (float)(maxVelocity.X / 5), velocity.Y);
                    if (newVel.X > maxVelocity.X) newVel.X = maxVelocity.X;
                    velocity = newVel;
                }
                else
                {
                    if (newVel.X > 0)
                    {
                        newVel = new Vector2(velocity.X - (float)(maxVelocity.X / 5), velocity.Y);
                        if (newVel.X < 0)
                            newVel = new Vector2(0, velocity.X);
                    }
                    velocity = newVel;
                }
            }

            move();

        }
        public override void hurt(double dmg, Vector2 sourcePos)
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
        }
        public override void knockback(Vector2 sourcePos)
        {
            if (isGettingKnockedBack) return;
            isGettingKnockedBack = true;

            Vector2 screenPos = Position;
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


        public override void draw(SpriteBatch batch)
        {
            Vector2 screenPos = Position;
            ((GraphicSet)graphic).active.draw(batch, screenPos, tintColor);

            batch.DrawRectangle(boundingBox, Color.LightGray);

            if (itemSwing != null)
            {
                itemSwing.Draw(batch, Position);
            }
        }

    }
}
