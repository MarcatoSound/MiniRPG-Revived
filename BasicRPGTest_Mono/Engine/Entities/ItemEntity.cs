using BasicRPGTest_Mono.Engine.GUI.Text;
using BasicRPGTest_Mono.Engine.Items;
using BasicRPGTest_Mono.Engine.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;

namespace BasicRPGTest_Mono.Engine.Entities
{
    public class ItemEntity : Entity, IDisposable
    {
        const int timeToLive = 600000;
        private Timer despawnTimer;

        public Map map;

        private Rectangle box = new Rectangle(0, 0, 24, 24);
        private Item item;
        public ItemEntity(Map map, Item item, Vector2 pos) : base(item.graphic, new Rectangle((int)pos.X, (int)pos.Y, item.graphic.width, item.graphic.height))
        {
            this.item = item;
            Position = new Vector2((int)pos.X, (int)pos.Y);
            this.map = map;

            List<ItemEntity> nearbyItems = getNearbyItems(50);
            foreach (ItemEntity nearbyItem in nearbyItems)
            {
                if (nearbyItem.item.id != item.id) continue;
                item.quantity += nearbyItem.item.quantity;
                nearbyItem.remove();
                // Add code for increasing the quantity of this new item here.
            }


            map.items.Add(Position, this);
            despawnTimer = new Timer(timeToLive);
            despawnTimer.Elapsed += (senders, args) => {
                despawnTimer.Stop();
                remove();
            };
            despawnTimer.Start();
        }

        public List<ItemEntity> getNearbyItems(int pixelRadius)
        {
            List<ItemEntity> items = new List<ItemEntity>();

            Vector2 startingPos = new Vector2(Position.X - pixelRadius, Position.Y - pixelRadius);
            Vector2 endingPos = new Vector2(Position.X + pixelRadius, Position.Y + pixelRadius);

            Vector2 targetPos = new Vector2();
            for (int x = (int)startingPos.X; x < endingPos.X; x++)
            {
                for (int y = (int)startingPos.Y; y < endingPos.Y; y++)
                {
                    targetPos.X = x;
                    targetPos.Y = y;
                    if (map.items.ContainsKey(targetPos))
                    {
                        items.Add(map.items[targetPos]);
                        //System.Diagnostics.Debug.WriteLine($"Found item in merge radius!");
                    }
                }

            }

            return items;
        }

        public void pickUp(Player player)
        {
            int slot = player.inventory.getFirstEmpty();
            if (slot == -1) return;

            player.inventory.addItem(item);

            Vector2 pos = new Vector2(player.Position.X + 32, player.Position.Y);
            new MovingText($"+{item.quantity} {item.displayName}", pos, Color.Cyan, 1500, Direction.Up, 0.65f, 500);

            remove();

        }

        public void remove()
        {
            map.items.Remove(Position);
            Dispose();
        }

        public override void draw(SpriteBatch batch)
        {
            if (!Camera.camera.BoundingRectangle.Intersects(boundingBox)) return;
            float scale = (float)box.Width / item.graphic.texture.Width;
            graphic.draw(batch, Position, Color.White, 0f, Vector2.Zero, scale);
            batch.DrawString(FontLibrary.getFont("itemcount"), $"{item.quantity}", new Vector2(Position.X + 12, Position.Y + 12), Color.Black);
        }

        public void Dispose()
        {
        }



        public static implicit operator YamlSection(ItemEntity ent)
        {
            YamlSection config = new YamlSection($"{ent.name}");

            config.set("item", (YamlSection)ent.item);
            config.setDouble("position.x", ent.Position.X);
            config.setDouble("position.y", ent.Position.Y);

            return config;
        }
    }
}
